using System;
using System.Linq;
using System.Threading.Tasks;
using ItsyBits.Data;
using ItsyBits.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ItsyBits.Controllers {

    [Authorize]
    public class AnimalController : Controller {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public AnimalController(ApplicationDbContext db, UserManager<ApplicationUser> userManager) {
            _db = db;
            _userManager = userManager;
        }

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
        
        public async Task<IActionResult> Details(int id) {
            Animal animal = _db.Animals.Single(a => a.Id == id);
            if (animal == null) {
                return NotFound();
            }
            _db.Entry(animal).Reference(a => a.Type).Load();
            _db.Entry(animal).Reference(a => a.Building).Load();
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user.Id != animal.Building.UserId) {
                return Unauthorized();
            }
            return View(animal);
        }

        [HttpPost]
        public async Task<IActionResult> Feed(int id) {
            Animal animal = _db.Animals.Single(a => a.Id == id);
            if (animal == null) {
                return NotFound();
            }
            _db.Entry(animal).Reference(a => a.Building).Load();
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user.Id != animal.Building.UserId) {
                return Unauthorized();
            }
            animal.LastFeed = DateTime.Now;
            _db.SaveChanges();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> Sleep(int id) {
            Animal animal = _db.Animals.Single(a => a.Id == id);
            if (animal == null) {
                return NotFound();
            }
            _db.Entry(animal).Reference(a => a.Building).Load();
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user.Id != animal.Building.UserId) {
                return Unauthorized();
            }
            animal.LastSleep = DateTime.Now;
            _db.SaveChanges();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> Pet(int id) {
            Animal animal = _db.Animals.Single(a => a.Id == id);
            if (animal == null) {
                return NotFound();
            }
            _db.Entry(animal).Reference(a => a.Building).Load();
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user.Id != animal.Building.UserId) {
                return Unauthorized();
            }
            animal.LastPet = DateTime.Now;
            _db.SaveChanges();
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
