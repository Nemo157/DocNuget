using System.Threading.Tasks;

using Microsoft.Framework.Logging;
using MongoDB.Driver;


namespace DocNuget.Models.Loader {
    public class DbPackageLoader {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMongoDatabase _db;

        public DbPackageLoader(ILoggerFactory loggerFactory, IMongoDatabase db) {
            _loggerFactory = loggerFactory;
            _db = db;
        }

        public async Task<Package> LoadAsync(string id, string version) {
            if (_db == null || version == null) {
                return null;
            }

            var logger = _loggerFactory.CreateLogger<PackageLoader>();

            var collection = _db.GetCollection<Package>("packages");
            logger.LogInformation($"Looking up {id} v{version} from db");
            var package = await collection.Find(pkg => pkg.UniqueId == $"{id}/{version}").FirstOrDefaultAsync();

            if (package == null) {
                logger.LogInformation($"Didn't find {id} v{version} in db");
            } else {
                logger.LogInformation($"Found {id} v{version} in db");
            }

            return package;
        }

        public async Task SaveAsync(Package package) {
            if (_db == null) {
                return;
            }

            var logger = _loggerFactory.CreateLogger<DbPackageLoader>();

            logger.LogInformation($"Saving {package.Id} at {package.Version} to db");
            var collection = _db.GetCollection<Package>("packages");
            await collection.InsertOneAsync(package);
            logger.LogInformation($"Saved {package.Id} at {package.Version} to db");
        }
    }
}
