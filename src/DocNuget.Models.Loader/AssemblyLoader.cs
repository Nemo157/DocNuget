using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Versioning;

using Microsoft.Framework.PackageManager;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Logging;

using Mono.Cecil;
using Newtonsoft.Json;
using NuGet;

using DocNuget.Models;

namespace DocNuget.Models.Loader {
    public class AssemblyLoader {
        private readonly ILoggerFactory _loggerFactory;

        public AssemblyLoader(ILoggerFactory loggerFactory) {
            _loggerFactory = loggerFactory;
        }

        public async Task<Models.Assembly> Load(string package, string version, string assembly, string framework) {
            var logger = _loggerFactory.CreateLogger<AssemblyLoader>();

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
                        reports: logger.CreateReports()))
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

            var pa = new Models.Package {
                Id = zipPackage.Id,
                Title = zipPackage.Title ?? zipPackage.Id,
                Version = zipPackage.Version.ToString(),
                Versions = packages.Select(p => p.Version.ToString()).ToList(),
                Summary = zipPackage.Summary ?? zipPackage.Description,
            };

            var ass = new Models.Assembly {
                Name = assembly,
                Package = pa,
                Framework = dll.Path.Split('\\').Skip(1).First(),
                Frameworks = zipPackage.GetFiles()
                    .Select(file => file.Path.EndsWith(assembly + ".dll") ? file.Path.Split('\\').Skip(1).First() : null)
                    .Where(f => f != null)
                    .Select(f => f.ToString())
                    .ToList(),
            };


            ass.RootNamespace = new Models.Namespace {
                    Name = "<root>",
                    FullName = "",
                    Assembly = ass,
                    Namespaces = new List<Models.Namespace>(),
                    Types = new List<Models.Type>(),
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
                    Namespace = @namespace,
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
                    Assembly = assembly,
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
    }
}
