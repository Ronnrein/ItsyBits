using System.Linq;
using System.Threading.Tasks;
using ItsyBits.Data;
using ItsyBits.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ItsyBits.Controllers {

    [Authorize]
    public class BuildingController : Controller {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public BuildingController(ApplicationDbContext db, UserManager<ApplicationUser> userManager) {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index() {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            return View(_db.Buildings
                .Where(b => b.UserId == user.Id)
                .Include(b => b.Animals)
                .ThenInclude(a => a.Type)
            );
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id) {
            Building building = await _db.Buildings
                .Include(b => b.Animals)
                .ThenInclude(a => a.Type)
                .SingleOrDefaultAsync(b => b.Id == id);
            if (building == null) {
                return NotFound();
            }
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user.Id != building.UserId) {
                return Unauthorized();
            }
            return View(building);
        }

        [HttpGet]
        public IActionResult Create() {
            ViewData["BuildingTypes"] = new SelectList(_db.BuildingTypes, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Building building) {
            if (!ModelState.IsValid) {
                ViewData["BuildingTypes"] = new SelectList(_db.BuildingTypes, "Id", "Name");
                return View(building);
            }
            building.User = await _userManager.GetUserAsync(User);
            _db.Add(building);
            await _db.SaveChangesAsync();
            return RedirectToAction("Details", new {id = building.Id});
        }

    }
}