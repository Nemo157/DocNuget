using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;

using DocNuget.Models.Loader;

namespace DocNuget.Controllers {
    public class AssemblyController : Controller {
        [Route("api/packages/{package}/assemblies/{assembly}")]
        [Route("api/packages/{package}/{version}/assemblies/{assembly}")]
        [Route("api/packages/{package}/assemblies/{assembly}/{framework}")]
        [Route("api/packages/{package}/{version}/assemblies/{assembly}/{framework}")]
        public async Task<Models.Assembly> Show(string package, string version, string assembly, string framework) {
            var loggerFactory = (ILoggerFactory)Resolver.GetService(typeof(ILoggerFactory));
            return (await new PackageLoader(loggerFactory).Load(package, version))
                .Assemblies
                .Where(a => a.Name == assembly)
                .Where(a => framework == null || a.TargetFramework.FullName == framework)
                .FirstOrDefault();
        }
    }
}
