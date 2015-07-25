using System;
using System.Threading.Tasks;

using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Logging;

using DocNuget.Models;
using DocNuget.Models.Loader;

namespace DocNuget.Controllers {
    public class AssemblyController : Controller {
        [Route("api/packages/{package}/assemblies/{assembly}")]
        [Route("api/packages/{package}/{version}/assemblies/{assembly}")]
        [Route("api/packages/{package}/assemblies/{assembly}/{framework}")]
        [Route("api/packages/{package}/{version}/assemblies/{assembly}/{framework}")]
        public Task<Models.Assembly> Show(string package, string version, string assembly, string framework) {
            var loggerFactory = (ILoggerFactory)Resolver.GetService(typeof(ILoggerFactory));
            return new AssemblyLoader(loggerFactory).Load(package, version, assembly, framework);
        }
    }
}
