using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ItsyBits.Data;
using ItsyBits.Models;
using ItsyBits.Models.ViewModels;
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
                .Include(b => b.Type)
                .Include(b => b.Animals)
                .ThenInclude(a => a.Type)
                .Include(b => b.BuildingUpgrades)
                .ThenInclude(bu => bu.Upgrade)
            );
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id) {
            Building building = await _db.Buildings
                .Include(b => b.Type)
                .Include(b => b.Animals)
                .ThenInclude(a => a.Type)
                .Include(b => b.BuildingUpgrades)
                .ThenInclude(bu => bu.Upgrade)
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

        [HttpGet]
        public IActionResult Plots() {
            string userId = _userManager.GetUserId(User);
            IEnumerable<Building> buildings = _db.Buildings.Where(b => b.UserId == userId);
            IEnumerable<Plot> plots = _db.Plots;
            foreach (Plot plot in plots) {
                Building building = buildings.SingleOrDefault(b => b.PlotId == plot.Id);
                if (building == null) {
                    continue;
                }
                plot.Buildings = new List<Building> {building};
            }
            return Json(plots);
        }

    }
}