using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;

using DocNuget.Models.Loader;

namespace DocNuget.Controllers {
    public class TypeController : Controller {
        [Route("api/packages/{package}/assemblies/{assembly}/types/{type}")]
        [Route("api/packages/{package}/{version}/assemblies/{assembly}/types/{type}")]
        [Route("api/packages/{package}/assemblies/{assembly}/{framework}/types/{type}")]
        [Route("api/packages/{package}/{version}/assemblies/{assembly}/{framework}/types/{type}")]
        public async Task<Models.Type> Show(string package, string version, string assembly, string framework, string type) {
            var loggerFactory = (ILoggerFactory)Resolver.GetService(typeof(ILoggerFactory));
            return GetTypes((await new PackageLoader(loggerFactory).Load(package, version))
                .Assemblies
                .Where(a => a.Name == assembly)
                .Where(a => framework == null || a.TargetFramework.FullName == framework)
                .FirstOrDefault()
                .RootNamespace)
                .Where(t => t.FullName == type)
                .FirstOrDefault();
        }

        public IEnumerable<Models.Type> GetTypes(Models.Namespace @namespace) {
            return @namespace.Types.Concat(@namespace.Namespaces.SelectMany(GetTypes));
        }
    }
}
