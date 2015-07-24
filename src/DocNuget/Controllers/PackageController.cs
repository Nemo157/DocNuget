using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.Mvc;
using Microsoft.Framework.PackageManager;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Logging;

using Newtonsoft.Json;
using NuGet;

using DocNuget.Models;

namespace DocNuget.Controllers {
    public class PackageController : Controller {
        [Route("api/packages/{package}.json")]
        [Route("api/packages/{package}/{version}.json")]
        public async Task<Models.Package> Show(string package, string version) {
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

            var pa = new Package {
                Id = zipPackage.Id,
                Title = zipPackage.Title ?? zipPackage.Id,
                Version = zipPackage.Version.ToString(),
                Versions = packages.Select(p => p.Version.ToString()).ToList(),
                Summary = zipPackage.Summary ?? zipPackage.Description,
                Assemblies = zipPackage.GetFiles()
                    .Select(file => file.Path.StartsWith("lib") && file.Path.EndsWith("dll")
                        ? new {
                            Name = Path.GetFileNameWithoutExtension(file.Path.Split('\\').Last()),
                            Framework = file.Path.Split('\\').Skip(1).First(),
                        } : null)
                    .Where(assembly => assembly != null)
                    .GroupBy(assembly => assembly.Name)
                    .Select(assemblies => new Models.Assembly {
                        Name = assemblies.Key,
                        Framework = assemblies.Select(assembly => assembly.Framework).FirstOrDefault(),
                        Frameworks = assemblies.Select(assembly => assembly.Framework).ToList(),
                    })
                    .ToList(),
            };

            return pa;
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
