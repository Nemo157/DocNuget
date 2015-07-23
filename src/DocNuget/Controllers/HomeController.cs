using Microsoft.AspNet.Mvc;

namespace DocNuget.Controllers {
    public class HomeController : Controller {
        [Route("")]
        public IActionResult Index() {
            return View();
        }
    }
}
