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

            // If the user has no animals, redirect to the select animal page
            if (!user.Animals.Any()) {
                return RedirectToAction("AnimalSelect", "Store");
            }
            return View();
        }

        public IActionResult About() {

            // If the user is not logged in, display using the basic layout
            ViewData["Layout"] = _userManager.GetUserId(User) == null ? "_BasicLayout" : "_CleanLayout";
            return View();
        }

        public IActionResult Contact() {

            // If the user is not logged in, display using the basic layout
            ViewData["Layout"] = _userManager.GetUserId(User) == null ? "_BasicLayout" : "_CleanLayout";
            return View();
        }

        public IActionResult Error() {
            return View();
        }

    }
}
