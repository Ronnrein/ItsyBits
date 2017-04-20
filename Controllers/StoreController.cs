using System;
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
                BuildingTypes = _db.BuildingTypes,
                Upgrades = _db.Upgrades
            });
        }

        [HttpGet]
        public async Task<IActionResult> Animal(int id) {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            IEnumerable<Building> buildings = _db.Buildings
                .Include(b => b.Type)
                .Include(b => b.Animals)
                .Include(b => b.BuildingUpgrades)
                .ThenInclude(bu => bu.Upgrade)
                .Where(b => b.UserId == user.Id && b.Animals.Count < b.Capacity);
            if (!buildings.Any()) {
                TempData.Put("Result", new Result("No buildings!", "You have no buildings with free space to put animals in!", ResultStatus.Error));
                return RedirectToAction("Index");
            }
            ViewData["BuildingId"] = buildings;
            ViewData["AnimalType"] = await _db.AnimalTypes.SingleOrDefaultAsync(a => a.Id == id);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Animal(int id, Animal animal) {
            Result error = null;
            ApplicationUser user = await _userManager.GetUserAsync(User);
            AnimalType animalType = await _db.AnimalTypes.SingleOrDefaultAsync(a => a.Id == id);
            Building building = await _db.Buildings
                .Include(b => b.Type)
                .Include(b => b.Animals)
                .Include(b => b.BuildingUpgrades)
                .ThenInclude(bu => bu.Upgrade)
                .SingleOrDefaultAsync(b => b.Id == animal.BuildingId);
            if (animal.BuildingId == 0) {
                error = new Result("No building!", "You have to give your animal a place to live!", ResultStatus.Error);
            }
            else if (building.Animals.Count >= building.Capacity) {
                error = new Result("No room!", "No room in this building for the animal!", ResultStatus.Error);
            }
            else if (user.Currency < animalType.Price) {
                error = new Result("Cannot afford!", "You cannot afford this animal!", ResultStatus.Error);
            }
            else if (string.IsNullOrWhiteSpace(animal.Name)) {
                error = new Result("No name!", "You have to call your animal something!", ResultStatus.Error);
            }
            if (error != null) {
                ViewData["Result"] = error;
                ViewData["AnimalType"] = animalType;
                ViewData["BuildingId"] = _db.Buildings
                    .Where(b => b.UserId == user.Id)
                    .Include(b => b.Type);
                return View(animal);
            }
            animal.Male = new Random().NextDouble() < 0.5;
            animal.Id = 0;
            animal.TypeId = id;
            _db.Users.Attach(user);
            user.Currency -= animalType.Price;
            _db.Entry(user).Property(u => u.Currency).IsModified = true;
            _db.Add(animal);
            await _db.SaveChangesAsync();
            return RedirectToAction("Details", "Animal", new { id = animal.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Building(int id) {
            ViewData["BuildingType"] = await _db.BuildingTypes.SingleOrDefaultAsync(a => a.Id == id);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Building(int id, Building building) {
            if (string.IsNullOrWhiteSpace(building.Name)) {
                ViewData["Result"] = new Result("No name!", "You have to call your animal something!", ResultStatus.Error);
                return View(building);
            }
            ApplicationUser user = await _userManager.GetUserAsync(User);
            building.UserId = user.Id;
            building.TypeId = id;
            building.Id = 0;
            _db.Add(building);
            await _db.SaveChangesAsync();
            return RedirectToAction("Details", "Building", new { id = building.Id });
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