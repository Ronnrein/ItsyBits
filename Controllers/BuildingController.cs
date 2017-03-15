using System.Collections;
using System.Collections.Generic;
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
            _db.Entry(user).Collection(u => u.Buildings).Load();
            foreach (Building building in user.Buildings) {
                _db.Entry(building).Collection(b => b.Animals).Load();
                foreach (Animal animal in building.Animals) {
                    _db.Entry(animal).Reference(a => a.Type).Load();
                }
            }
            return View(user.Buildings);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id) {
            Building building = await _db.Buildings.SingleOrDefaultAsync(b => b.Id == id);
            if (building == null) {
                return NotFound();
            }
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user.Id != building.UserId) {
                return Unauthorized();
            }
            _db.Entry(building).Collection(b => b.Animals).Load();
            foreach (Animal animal in building.Animals) {
                _db.Entry(animal).Reference(a => a.Type).Load();
            }
            return View(building);
        }

        [HttpGet]
        public IActionResult Create() {
            ViewData["BuildingTypes"] = new SelectList(_db.BuildingTypes, "Id", "Name");
            return View(new Building());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Building building) {
            if (!ModelState.IsValid) {
                ViewData["BuildingTypes"] = new SelectList(_db.BuildingTypes, "Id", "Name");
                return View(building);
            }
            building.User = await _userManager.GetUserAsync(User);
            _db.Buildings.Add(building);
            await _db.SaveChangesAsync();
            return RedirectToAction("Details", new {id = building.Id});
        }

    }
}