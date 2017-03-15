using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ItsyBits.Data;
using ItsyBits.Models;
using Microsoft.AspNetCore.Authorization;

namespace ItsyBits.Areas.Admin.Controllers {

    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class AnimalTypeController : Controller {

        private readonly ApplicationDbContext _db;

        public AnimalTypeController(ApplicationDbContext db) {
            _db = db;
        }

        [HttpGet]
        public IActionResult Index() {
            return View(_db.AnimalTypes);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id) {
            AnimalType animalType = await _db.AnimalTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (animalType == null) {
                return NotFound();
            }
            return View(animalType);
        }

        [HttpGet]
        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AnimalType animalType) {
            if (!ModelState.IsValid) {
                return View(animalType);
            }
            _db.Add(animalType);
            await _db.SaveChangesAsync();
            return RedirectToAction("Details", new {id = animalType.Id});
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id) {
            AnimalType animalType = await _db.AnimalTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (animalType == null) {
                return NotFound();
            }
            return View(animalType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AnimalType animalType) {
            if (id != animalType.Id) {
                return NotFound();
            }
            if (!ModelState.IsValid) {
                return View(animalType);
            }
            try {
                _db.Update(animalType);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!_db.AnimalTypes.Any(e => e.Id == id)) {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction("Details", new {id = animalType.Id});
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id) {
            AnimalType animalType = await _db.AnimalTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (animalType == null) {
                return NotFound();
            }
            return View(animalType);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            AnimalType animalType = await _db.AnimalTypes.SingleOrDefaultAsync(m => m.Id == id);
            _db.AnimalTypes.Remove(animalType);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
