using System.Threading.Tasks;
using Microsoft.Framework.Logging;
using MongoDB.Driver;

namespace DocNuget.Models.Loader {
    public class PackageLoader {
        private readonly DbPackageLoader _dbLoader;
        private readonly NuGetPackageLoader _nugetLoader;

        public PackageLoader(ILoggerFactory loggerFactory, IMongoDatabase db) {
            _dbLoader = new DbPackageLoader(loggerFactory, db);
            _nugetLoader = new NuGetPackageLoader(loggerFactory);
        }

        public async Task<Package> LoadAsync(string id, string version) {
            var result = await _dbLoader.LoadAsync(id, version);
            if (result == null) {
                result = await _nugetLoader.LoadAsync(id, version);
                await _dbLoader.SaveAsync(result);
            }
            return result;
        }
    }
}
