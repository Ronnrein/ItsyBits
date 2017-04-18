using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ItsyBits.Controllers {

    [Authorize]
    public class StoreController : Controller {
        
        [HttpGet]
        public IActionResult Index() {
            return View();
        }
    }
}