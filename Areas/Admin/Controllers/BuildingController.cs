using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ItsyBits.Data;
using ItsyBits.Models;
using Microsoft.AspNetCore.Authorization;

namespace ItsyBits.Areas.Admin.Controllers {

    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class BuildingController : Controller {

        private readonly ApplicationDbContext _db;

        public BuildingController(ApplicationDbContext db) {
            _db = db;    
        }

        [HttpGet]
        public IActionResult Index() {
            return View(_db.Buildings.Include(b => b.Type).Include(b => b.User).Include(b => b.Plot));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id) {
            Building building = await _db.Buildings
                .Include(b => b.Type)
                .Include(b => b.User)
                .Include(b => b.Plot)
                .SingleOrDefaultAsync(m => m.Id == id
            );
            if (building == null) {
                return NotFound();
            }
            return View(building);
        }

        [HttpGet]
        public IActionResult Create() {
            ViewData["TypeId"] = new SelectList(_db.BuildingTypes, "Id", "Name");
            ViewData["UserId"] = new SelectList(_db.Users, "Id", "UserName");
            ViewData["PlotId"] = new SelectList(_db.Plots, "Id", "Description");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Building building) {
            if (!ModelState.IsValid) {
                ViewData["TypeId"] = new SelectList(_db.BuildingTypes, "Id", "Name", building.TypeId);
                ViewData["UserId"] = new SelectList(_db.Users, "Id", "UserName", building.UserId);
                ViewData["PlotId"] = new SelectList(_db.Plots, "Id", "Description", building.PlotId);
                return View(building);
            }
            _db.Add(building);
            await _db.SaveChangesAsync();
            return RedirectToAction("Details", new { id = building.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id) {
            Building building = await _db.Buildings
                .Include(b => b.BuildingUpgrades)
                .ThenInclude(bu => bu.Upgrade)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (building == null) {
                return NotFound();
            }
            ViewData["TypeId"] = new SelectList(_db.BuildingTypes, "Id", "Name", building.TypeId);
            ViewData["UserId"] = new SelectList(_db.Users, "Id", "UserName", building.UserId);
            ViewData["PlotId"] = new SelectList(_db.Plots, "Id", "Description", building.PlotId);
            ViewData["UpgradeId"] = new SelectList(_db.Upgrades.Where(u => building.BuildingUpgrades.All(au => au.UpgradeId != u.Id)), "Id", "Name");
            return View(building);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Building building) {
            if (id != building.Id) {
                return NotFound();
            }
            if (!ModelState.IsValid) {
                Building oldBuilding = await _db.Buildings
                    .Include(b => b.BuildingUpgrades)
                    .ThenInclude(bu => bu.Upgrade)
                    .SingleOrDefaultAsync(b => b.Id == id);
                ViewData["TypeId"] = new SelectList(_db.BuildingTypes, "Id", "Name", oldBuilding.TypeId);
                ViewData["UserId"] = new SelectList(_db.Users, "Id", "UserName", oldBuilding.UserId);
                ViewData["PlotId"] = new SelectList(_db.Plots, "Id", "Description", oldBuilding.PlotId);
                ViewData["UpgradeId"] = new SelectList(_db.Upgrades.Where(u => oldBuilding.BuildingUpgrades.All(au => au.UpgradeId != u.Id)), "Id", "Name");
                return View(building);
            }
            try {
                _db.Update(building);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!_db.Buildings.Any(e => e.Id == building.Id)) {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction("Details", new { id = building.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id) {
            Building building = await _db.Buildings
                .Include(b => b.Type)
                .Include(b => b.User)
                .Include(b => b.Plot)
                .SingleOrDefaultAsync(m => m.Id == id
            );
            if (building == null) {
                return NotFound();
            }
            return View(building);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            Building building = await _db.Buildings.SingleOrDefaultAsync(m => m.Id == id);
            _db.Buildings.Remove(building);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddUpgrade(BuildingUpgrade upgrade) {
            _db.Add(upgrade);
            await _db.SaveChangesAsync();
            return RedirectToAction("Edit", new {upgrade.BuildingId});
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUpgrade(BuildingUpgrade upgrade) {
            _db.BuildingUpgrades.Remove(upgrade);
            await _db.SaveChangesAsync();
            return RedirectToAction("Edit", new {upgrade.BuildingId});
        }
    }
}
