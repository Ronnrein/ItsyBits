using System.Linq;
using System.Threading.Tasks;
using ItsyBits.Data;
using ItsyBits.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItsyBits.Controllers {
    public class HomeController : Controller {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext db, UserManager<ApplicationUser> userManager) {
            _db = db;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index() {
            ApplicationUser user = await _db.Users
                .Include(u => u.Buildings)
                .ThenInclude(b => b.Animals)
                .SingleOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));
            if (!user.Animals.Any()) {
                return RedirectToAction("AnimalSelect", "Store");
            }
            return View();
        }

        public IActionResult About() {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact() {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error() {
            return View();
        }

        public IActionResult TempNotFound() {
            return View("~/Views/Error/NotFound.cshtml");
        }

        public IActionResult TempUnauthorized() {
            return View("~/Views/Error/Unauthorized.cshtml");
        }

        public IActionResult TempError() {
            return View("~/Views/Error/Error.cshtml");
        }

    }
}
