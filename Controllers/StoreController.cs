using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ItsyBits.Data;
using ItsyBits.Helpers;
using ItsyBits.Models;
using ItsyBits.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ItsyBits.Controllers {

    [Authorize]
    public class StoreController : Controller {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public StoreController(ApplicationDbContext db, UserManager<ApplicationUser> userManager) {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index() {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            ViewData["Currency"] = user.Currency;
            return View(new StoreViewModel {
                AnimalTypes = _db.AnimalTypes,
                Upgrades = _db.Upgrades
            });
        }

        [HttpGet]
        public async Task<IActionResult> Animal(int id) {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            IEnumerable<Building> buildings = _db.Buildings.Where(b => b.UserId == user.Id);
            if (!buildings.Any()) {
                TempData.Put("Result", new Result("No buildings!", "You have no buildings to put animals in!", ResultStatus.Error));
                return RedirectToAction("Index");
            }
            ViewData["BuildingId"] = new SelectList(buildings, "Id", "Name");
            return View(await _db.AnimalTypes.SingleOrDefaultAsync(a => a.Id == id));
        }

        [HttpGet]
        public async Task<IActionResult> AnimalUpgrade(int id) {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            user = await _db.Users
                .Include(u => u.Buildings)
                .ThenInclude(b => b.Animals)
                .ThenInclude(a => a.Type)
                .SingleOrDefaultAsync(u => u.Id == user.Id);
            if (!user.Animals.Any()) {
                TempData.Put("Result", new Result("No animals!", "You have no animals to upgrade!", ResultStatus.Error));
                return RedirectToAction("Index");
            }
            ViewData["AnimalId"] = new SelectList(user.Animals, "Id", "Name");
            return View(await _db.Upgrades.SingleOrDefaultAsync(a => a.Id == id));
        }

        [HttpGet]
        public async Task<IActionResult> BuildingUpgrade(int id) {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            IEnumerable<Building> buildings = _db.Buildings.Where(b => b.UserId == user.Id);
            if (!buildings.Any()) {
                TempData.Put("Result", new Result("No buildings!", "You have no buildings to upgrade!", ResultStatus.Error));
                return RedirectToAction("Index");
            }
            ViewData["BuildingId"] = new SelectList(buildings, "Id", "Name");
            return View(await _db.Upgrades.SingleOrDefaultAsync(a => a.Id == id));
        }
    }
}