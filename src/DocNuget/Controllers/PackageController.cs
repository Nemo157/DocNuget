using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;

using DocNuget.Models.Loader;

namespace DocNuget.Controllers {
    public class PackageController : Controller {
        [Route("api/packages/{package}/{version?}")]
        public async Task<IActionResult> Show(string package, string version) {
            var loggerFactory = (ILoggerFactory)Resolver.GetService(typeof(ILoggerFactory));
            var pkg = await new PackageLoader(loggerFactory).Load(package, version);

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
