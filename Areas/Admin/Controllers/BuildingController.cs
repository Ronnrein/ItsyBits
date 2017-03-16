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
            return View(_db.Buildings.Include(b => b.Type).Include(b => b.User));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id) {
            Building building = await _db.Buildings
                .Include(b => b.Type)
                .Include(b => b.User)
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,UserId,TypeId")] Building building) {
            if (!ModelState.IsValid) {
                ViewData["TypeId"] = new SelectList(_db.BuildingTypes, "Id", "Name", building.TypeId);
                ViewData["UserId"] = new SelectList(_db.Users, "Id", "UserName", building.UserId);
                return View(building);
            }
            _db.Add(building);
            await _db.SaveChangesAsync();
            return RedirectToAction("Details", new { id = building.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id) {
            Building building = await _db.Buildings.SingleOrDefaultAsync(m => m.Id == id);
            if (building == null) {
                return NotFound();
            }
            ViewData["TypeId"] = new SelectList(_db.BuildingTypes, "Id", "Name", building.TypeId);
            ViewData["UserId"] = new SelectList(_db.Users, "Id", "UserName", building.UserId);
            return View(building);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Building building) {
            if (id != building.Id) {
                return NotFound();
            }
            if (ModelState.IsValid) {
                ViewData["TypeId"] = new SelectList(_db.BuildingTypes, "Id", "Name", building.TypeId);
                ViewData["UserId"] = new SelectList(_db.Users, "Id", "UserName", building.UserId);
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
    }
}
