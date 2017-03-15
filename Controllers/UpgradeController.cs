using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ItsyBits.Data;
using ItsyBits.Models;
using Microsoft.AspNetCore.Authorization;

namespace ItsyBits.Controllers {

    [Authorize(Roles = "Administrator")]
    public class UpgradeController : Controller {

        private readonly ApplicationDbContext _db;

        public UpgradeController(ApplicationDbContext db) {
            _db = db;    
        }

        [HttpGet]
        public IActionResult Index() {
            return View(_db.Upgrades);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id) {
            Upgrade upgrade = await _db.Upgrades.SingleOrDefaultAsync(m => m.Id == id);
            if (upgrade == null) {
                return NotFound();
            }
            return View(upgrade);
        }

        [HttpGet]
        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Upgrade upgrade) {
            if (!ModelState.IsValid) {
                return View(upgrade);
            }
            _db.Add(upgrade);
            await _db.SaveChangesAsync();
            return RedirectToAction("Details", new {id = upgrade.Id});
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id) {
            Upgrade upgrade = await _db.Upgrades.SingleOrDefaultAsync(m => m.Id == id);
            if (upgrade == null) {
                return NotFound();
            }
            return View(upgrade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Upgrade upgrade) {
            if (id != upgrade.Id) {
                return NotFound();
            }

            if (!ModelState.IsValid) {
                return View(upgrade);
            }
            try {
                _db.Update(upgrade);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!_db.Upgrades.Any(e => e.Id == id)) {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction("Details", new {id = upgrade.Id});
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id) {
            Upgrade upgrade = await _db.Upgrades.SingleOrDefaultAsync(m => m.Id == id);
            if (upgrade == null) {
                return NotFound();
            }
            return View(upgrade);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            Upgrade upgrade = await _db.Upgrades.SingleOrDefaultAsync(m => m.Id == id);
            _db.Upgrades.Remove(upgrade);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
