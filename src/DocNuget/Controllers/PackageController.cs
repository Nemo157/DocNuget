using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNet.Mvc;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;

using MongoDB.Driver;

using DocNuget.Models.Loader;

namespace DocNuget.Controllers {
    public class PackageController : Controller {
        [Route("api/packages/{package}/{version?}")]
        public async Task<IActionResult> Show(string package, string version) {
            var loggerFactory = Resolver.GetRequiredService<ILoggerFactory>();
            var db = Resolver.GetService<IMongoDatabase>();
            var pkg = await new PackageLoader(loggerFactory, db).LoadAsync(package, version);

            if (version == null) {
                return new RedirectToActionResult("Show", "Package", new Dictionary<string, object> {
                    ["package"] = package,
                    ["version"] = pkg.Version,
                });
            } else {
                return new ObjectResult(pkg);
            }
        }
    }
}
