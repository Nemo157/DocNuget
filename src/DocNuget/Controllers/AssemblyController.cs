using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Versioning;

using Microsoft.AspNet.Mvc;
using Microsoft.Framework.PackageManager;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Logging;

using Mono.Cecil;
using Newtonsoft.Json;
using NuGet;

using DocNuget.Models;

namespace DocNuget.Controllers {
    public class AssemblyController : Controller {
        [Route("api/packages/{package}/assemblies/{assembly}.json")]
        [Route("api/packages/{package}/{version}/assemblies/{assembly}.json")]
        [Route("api/packages/{package}/assemblies/{assembly}/{framework}.json")]
        [Route("api/packages/{package}/{version}/assemblies/{assembly}/{framework}.json")]
        public async Task<Models.Assembly> Show(string package, string version, string assembly, string framework) {
            var loggerFactory = (ILoggerFactory)Resolver.GetService(typeof(ILoggerFactory));
            var logger = loggerFactory.CreateLogger<PackageController>();

            var sourceProvider = new PackageSourceProvider(
                NullSettings.Instance,
                new[] { new PackageSource(NuGetConstants.DefaultFeedUrl) });

            var feed = sourceProvider
                .LoadPackageSources()
                .Select(source =>
                    PackageSourceUtils.CreatePackageFeed(
                        source,
                        noCache: false,
                        ignoreFailedSources: false,
                        reports: new Reports {
                            Information = new LoggerReport {
                                LogLevel = LogLevel.Information,
                                Logger = loggerFactory.CreateLogger("nuget"),
                            },
                            Verbose = new LoggerReport {
                                LogLevel = LogLevel.Verbose,
                                Logger = loggerFactory.CreateLogger("nuget"),
                            },
                            Quiet = new LoggerReport {
                                LogLevel = LogLevel.Debug,
                                Logger = loggerFactory.CreateLogger("nuget"),
                            },
                            Error = new LoggerReport {
                                LogLevel = LogLevel.Error,
                                Logger = loggerFactory.CreateLogger("nuget"),
                            },
                        }))
                .Where(f => f != null)
                .First();

            var packages = (await feed.FindPackagesByIdAsync(package)).OrderByDescending(p => p.Version);

            var result = version == null
                ? packages.FirstOrDefault()
                : packages.FirstOrDefault(p => p.Version == new SemanticVersion(version));

            if (result == null) {
                logger.LogError("Unable to locate {0} v{1}", package, version);
                return null;
            }

            logger.LogInformation("Found version {0} of {1}", result.Version, result.Id);

            var zipPackage = new ZipPackage(await feed.OpenNupkgStreamAsync(result));
            var dlls = zipPackage.GetFiles()
                .Select(file => {
                    logger.LogDebug("Checking file {0}", file.Path);
                    return file;
                })
                .Where(file => file.Path.EndsWith(assembly + ".dll"));
            if (framework != null) {
                dlls = dlls.Where(file => file.Path.StartsWith("lib\\" + framework + "\\"));
            }
            var dll = dlls.FirstOrDefault();

            if (dll == null) {
                logger.LogError("Unable to locate {0} ({1}) from {2} v{3}", assembly, framework, package, version);
                return null;
            }

            logger.LogInformation("Found dll {0}", dll.Path, result.Id);

            var reflectedAssembly = Mono.Cecil.AssemblyDefinition.ReadAssembly(dll.GetStream());

            if (reflectedAssembly == null) {
                logger.LogError("Unable to reflect assembly");
                return null;
            }

            var ass = new Models.Assembly {
                Name = assembly,
                Framework = framework,
                Frameworks = zipPackage.GetFiles()
                    .Select(file => file.Path.EndsWith(assembly + ".dll") ? file.Path.Split('\\').Skip(1).First() : null)
                    .Where(f => f != null)
                    .Select(f => f.ToString())
                    .ToList(),
                RootNamespace = new Models.Namespace {
                    Name = "<root>",
                    FullName = "",
                    Namespaces = new List<Models.Namespace>(),
                    Types = new List<Models.Type>(),
                },
            };

            var types = reflectedAssembly.Modules.SelectMany(module => module.Types).Where(type => type.IsPublic);

            foreach (var @namespace in types.Select(type => type.Namespace).Distinct()) {
                if (@namespace == "") {
                    continue;
                }
                Insert(ass, ass.RootNamespace, @namespace.Split('.'));
            }

            foreach (var group in types.GroupBy(type => type.Namespace)) {
                var @namespace = Walk(ass.RootNamespace, group.Key.Split('.').Where(val => val != ""));
                @namespace.Types = group.Select(type => new Models.Type {
                    Name = type.Name,
                }).ToList();
            }

            return ass;
        }

        private void Insert(Models.Assembly assembly, Models.Namespace @namespace, IEnumerable<string> path) {
            if (!path.Any()) {
                return;
            }

            var match = @namespace.Namespaces.FirstOrDefault(ns => ns.Name == path.First());
            if (match == null) {
                match = new Models.Namespace {
                    Name = path.First(),
                    FullName = (@namespace.FullName == "" ? "" : @namespace.FullName + ".") + path.First(),
                    Namespaces = new List<Models.Namespace>(),
                    Types = new List<Models.Type>(),
                };
                @namespace.Namespaces.Add(match);
            }
            Insert(assembly, @match, path.Skip(1));
        }

        private Models.Namespace Walk(Models.Namespace @namespace, IEnumerable<string> path) {
            if (!path.Any()) {
                return @namespace;
            }

            return Walk(@namespace.Namespaces.FirstOrDefault(ns => ns.Name == path.First()), path.Skip(1));
        }

        private class LoggerReport : IReport {
            public LogLevel LogLevel { get; set; }

            public Microsoft.Framework.Logging.ILogger Logger { get; set; }

            public void WriteLine(string message) {
                Logger.Log(LogLevel, 0, message, null, null);
            }
        }
    }
}
