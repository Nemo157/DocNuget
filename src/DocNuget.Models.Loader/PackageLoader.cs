using System.Linq;
using System.Threading.Tasks;

using Microsoft.Framework.PackageManager;
using Microsoft.Framework.Logging;

using NuGet;

namespace DocNuget.Models.Loader {
    public class PackageLoader {
        private readonly ILoggerFactory _loggerFactory;

        public PackageLoader(ILoggerFactory loggerFactory) {
            _loggerFactory = loggerFactory;
        }

        public async Task<Package> Load(string package, string version) {
            var logger = _loggerFactory.CreateLogger<PackageLoader>();

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

            return Link(zipPackage.ToPackage(packages.Select(p => p.Version.ToString()).ToList()));
        }

        private Package Link(Package package) {
            foreach (var assembly in package.Assemblies) {
                assembly.Package = package;
                Link(assembly, assembly.RootNamespace);
            }

            return package;
        }

        private void Link(Assembly assembly, Namespace @namespace) {
            @namespace.Assembly = assembly;
            foreach (var childNamespace in @namespace.Namespaces) {
                Link(assembly, childNamespace);
            }
        }
    }
}
