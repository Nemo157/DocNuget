using System;
using System.Threading.Tasks;

using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Logging;

using DocNuget.Models;
using DocNuget.Models.Loader;

namespace DocNuget.Controllers {
    public class PackageController : Controller {
        [Route("api/packages/{package}")]
        [Route("api/packages/{package}/{version}")]
        public Task<Models.Package> Show(string package, string version) {
            var loggerFactory = (ILoggerFactory)Resolver.GetService(typeof(ILoggerFactory));
            return new PackageLoader(loggerFactory).Load(package, version);
        }
    }
}
