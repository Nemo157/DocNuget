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

            logger.LogInformation($"Looking up {id} v{version} from nuget");

            var packages = (await feed.FindPackagesByIdAsync(id)).OrderByDescending(p => p.Version);

            var package = version == null
                ? packages.FirstOrDefault()
                : packages.FirstOrDefault(p => p.Version == new SemanticVersion(version));

            if (package == null) {
                logger.LogError($"Unable to locate {id} v{version}");
                return null;
            }

            logger.LogInformation($"Found version {package.Version} of {package.Id}");

            var zipPackage = new ZipPackage(await feed.OpenNupkgStreamAsync(package));

            return zipPackage.ToPackage(packages.Select(p => p.Version.ToString()).ToList(), logger);
        }
    }
}
