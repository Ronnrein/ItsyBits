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
            return View();
        }

        [Authorize]
        public IActionResult Contact() {
            return View();
        }

        public IActionResult Error(int? id) {
            if (id == null) {
                return View("~/Views/Error/Error.cshtml");
            }
            switch (id) {
                case 404:
                    return View("~/Views/Error/NotFound.cshtml");
                case 403:
                    return View("~/Views/Error/Unauthorized.cshtml");
                default:
                    return View("~/Views/Error/Error.cshtml");
            }
            
        }

    }
}
