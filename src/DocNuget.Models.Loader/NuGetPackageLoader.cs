using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.PackageManager;
using Microsoft.Framework.Logging;
using NuGet;

namespace DocNuget.Models.Loader {
    public class NuGetPackageLoader {
        private readonly ILoggerFactory _loggerFactory;

        public NuGetPackageLoader(ILoggerFactory loggerFactory) {
            _loggerFactory = loggerFactory;
        }

        public async Task<Package> LoadAsync(string id, string version) {
            var logger = _loggerFactory.CreateLogger<NuGetPackageLoader>();

            var sourceProvider = new PackageSourceProvider(
                NullSettings.Instance,
                new[] {
                    new PackageSource(NuGetConstants.DefaultFeedUrl),
                    new PackageSource("https://www.myget.org/F/aspnetvnext/api/v2"),
                });

            var feeds = sourceProvider
                .LoadPackageSources()
                .Select(source =>
                    PackageSourceUtils.CreatePackageFeed(
                        source,
                        noCache: false,
                        ignoreFailedSources: false,
                        reports: logger.CreateReports()))
                .Where(f => f != null);

            logger.LogInformation($"Looking up {id} v{version} from nuget");

            var packages = (await Task.WhenAll(feeds.Select(feed => feed.FindPackagesByIdAsync(id))))
                .SelectMany(_ => _)
                .OrderByDescending(p => p.Version);

            var package = version == null
                ? packages.FirstOrDefault()
                : packages.FirstOrDefault(p => p.Version == new SemanticVersion(version));

            if (package == null) {
                logger.LogError($"Unable to locate {id} v{version}");
                return null;
            }

            logger.LogInformation($"Found version {package.Version} of {package.Id}");

            var pkgStreams = await Task.WhenAll(feeds.Select(feed => {
                try {
                    return feed.OpenNupkgStreamAsync(package);
                } catch {
                    return null;
                }
            }));
            var pkgStream = pkgStreams.FirstOrDefault(s => s != null);
            var zipPackage = new ZipPackage(pkgStream);

            if (zipPackage == null) {
                logger.LogError($"Unable to open package stream for {id} v{version}");
                return null;
            }

            return zipPackage.ToPackage(packages.Select(p => p.Version.ToString()).ToList(), logger);
        }
    }
}
