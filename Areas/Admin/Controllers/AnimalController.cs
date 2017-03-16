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
    public class AnimalController : Controller {

        private readonly ApplicationDbContext _db;

        public AnimalController(ApplicationDbContext db) {
            _db = db;    
        }

        [HttpGet]
        public IActionResult Index() {
            return View(_db.Animals.Include(a => a.Building).Include(a => a.Type));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id) {
            Animal animal = await _db.Animals
                .Include(a => a.Building)
                .Include(a => a.Type)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (animal == null) {
                return NotFound();
            }
            return View(animal);
        }

        [HttpGet]
        public IActionResult Create() {
            ViewData["BuildingId"] = new SelectList(_db.Buildings, "Id", "Name");
            ViewData["TypeId"] = new SelectList(_db.AnimalTypes, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Animal animal) {
            if (!ModelState.IsValid) {
                ViewData["BuildingId"] = new SelectList(_db.Buildings, "Id", "Name", animal.BuildingId);
                ViewData["TypeId"] = new SelectList(_db.AnimalTypes, "Id", "Name", animal.TypeId);
                return View(animal);
            }
            _db.Add(animal);
            await _db.SaveChangesAsync();
            return RedirectToAction("Details", new { id = animal.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id) {
            Animal animal = await _db.Animals.SingleOrDefaultAsync(m => m.Id == id);
            if (animal == null) {
                return NotFound();
            }
            ViewData["BuildingId"] = new SelectList(_db.Buildings, "Id", "Name", animal.BuildingId);
            ViewData["TypeId"] = new SelectList(_db.AnimalTypes, "Id", "Name", animal.TypeId);
            return View(animal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Animal animal) {
            if (id != animal.Id) {
                return NotFound();
            }
            if (!ModelState.IsValid) {
                ViewData["BuildingId"] = new SelectList(_db.Buildings, "Id", "Name", animal.BuildingId);
                ViewData["TypeId"] = new SelectList(_db.AnimalTypes, "Id", "Name", animal.TypeId);
                return View(animal);
            }
            try {
                _db.Update(animal);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!_db.Animals.Any(e => e.Id == animal.Id)) {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction("Details", new { id = animal.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id) {
            Animal animal = await _db.Animals
                .Include(a => a.Building)
                .Include(a => a.Type)
                .SingleOrDefaultAsync(m => m.Id == id
            );
            if (animal == null) {
                return NotFound();
            }
            return View(animal);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            Animal animal = await _db.Animals.SingleOrDefaultAsync(m => m.Id == id);
            _db.Animals.Remove(animal);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
