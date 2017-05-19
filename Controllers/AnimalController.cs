using System;
using System.Linq;
using System.Threading.Tasks;
using ItsyBits.Data;
using ItsyBits.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ItsyBits.Controllers {

    [Authorize]
    public class AnimalController : Controller {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public AnimalController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IConfiguration config) {
            _db = db;
            _userManager = userManager;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> Index() {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            return View(_db.Buildings
                .Where(b => b.UserId == user.Id)
                .Include(b => b.Animals)
                .ThenInclude(a => a.Type)
                .AsEnumerable()
            );
        }
        
        [HttpGet]
        public async Task<IActionResult> Details(int id) {
            Animal animal = await _db.Animals
                .Include(a => a.Building)
                .ThenInclude(b => b.Type)
                .Include(a => a.Type)
                .Include(a => a.AnimalUpgrades)
                .ThenInclude(au => au.Upgrade)
                .SingleOrDefaultAsync(a => a.Id == id);
            if (animal == null) {
                return NotFound();
            }
            if (animal.DeathTime != null) {
                return RedirectToAction("Recover", new { id = animal.Id });
            }
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user.Id != animal.Building.UserId) {
                return Unauthorized();
            }
            ViewData["Upgrades"] = _db.Upgrades.Where(u => u.ForAnimal);
            return View(animal);
        }

        [HttpGet]
        public async Task<IActionResult> Recover(int id) {
            Animal animal = await _db.Animals
                .Include(a => a.Building)
                .Include(a => a.Type)
                .SingleOrDefaultAsync(a => a.Id == id);
            if (animal == null) {
                return NotFound();
            }
            if (animal.DeathTime == null) {
                return RedirectToAction("Details", new { id = animal.Id });
            }
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user.Id != animal.Building.UserId) {
                return Unauthorized();
            }
            return View(animal);
        }

        [HttpPost, ActionName("Recover")]
        public async Task<IActionResult> RecoverAnimal(int id) {
            Animal animal = await _db.Animals
                .Include(a => a.Building)
                .Include(a => a.Type)
                .SingleOrDefaultAsync(a => a.Id == id);
            if (animal == null) {
                return NotFound();
            }
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user.Id != animal.Building.UserId) {
                return Unauthorized();
            }
            animal.HappinessPercentage = int.Parse(_config["AnimalRecoveryHappiness"]);
            _db.Update(animal);
            await _db.SaveChangesAsync();
            return RedirectToAction("Details", new { id = animal.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Feed(int id) {
            Animal animal = await _db.Animals
                .Include(a => a.Building)
                .Include(a => a.Type)
                .SingleOrDefaultAsync(a => a.Id == id);
            if (animal == null) {
                return NotFound();
            }
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user.Id != animal.Building.UserId) {
                return Unauthorized();
            }
            animal.FeedPercentage = 100;
            _db.Update(animal);
            await _db.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> Sleep(int id) {
            Animal animal = await _db.Animals
                .Include(a => a.Building)
                .Include(a => a.Type)
                .SingleOrDefaultAsync(a => a.Id == id);
            if (animal == null) {
                return NotFound();
            }
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user.Id != animal.Building.UserId) {
                return Unauthorized();
            }
            animal.SleepPercentage = 100;
            _db.Update(animal);
            _db.SaveChanges();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> Pet(int id) {
            Animal animal = await _db.Animals
                .Include(a => a.Building)
                .Include(a => a.Type)
                .SingleOrDefaultAsync(a => a.Id == id);
            if (animal == null) {
                return NotFound();
            }
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user.Id != animal.Building.UserId) {
                return Unauthorized();
            }
            animal.PetPercentage = 100;
            _db.Update(animal);
            _db.SaveChanges();
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
