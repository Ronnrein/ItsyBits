using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ItsyBits.Data;
using ItsyBits.Models;
using Microsoft.AspNetCore.Authorization;

namespace ItsyBits.Controllers {

    [Authorize(Roles = "Administrator")]
    public class BuildingTypeController : Controller {

        private readonly ApplicationDbContext _db;

        public BuildingTypeController(ApplicationDbContext db) {
            _db = db;    
        }

        [HttpGet]
        public IActionResult Index() {
            return View(_db.BuildingTypes);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id) {
            BuildingType buildingType = await _db.BuildingTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (buildingType == null) {
                return NotFound();
            }
            return View(buildingType);
        }

        [HttpGet]
        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BuildingType buildingType) {
            if (!ModelState.IsValid) {
                return View(buildingType);
            }
            _db.Add(buildingType);
            await _db.SaveChangesAsync();
            return RedirectToAction("Details", new {id = buildingType.Id});
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id) {
            BuildingType buildingType = await _db.BuildingTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (buildingType == null) {
                return NotFound();
            }
            return View(buildingType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BuildingType buildingType) {
            if (id != buildingType.Id) {
                return NotFound();
            }
            if (!ModelState.IsValid) {
                return View(buildingType);
            }
            try {
                _db.Update(buildingType);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!_db.BuildingTypes.Any(e => e.Id == id)) {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction("Details", new {id = buildingType.Id});
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id) {
            BuildingType buildingType = await _db.BuildingTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (buildingType == null) {
                return NotFound();
            }
            return View(buildingType);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            BuildingType buildingType = await _db.BuildingTypes.SingleOrDefaultAsync(m => m.Id == id);
            _db.BuildingTypes.Remove(buildingType);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
