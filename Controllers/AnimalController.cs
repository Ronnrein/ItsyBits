using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ItsyBits.Data;
using ItsyBits.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItsyBits.Controllers {

    [Authorize]
    public class AnimalController : Controller {

        private ApplicationDbContext db;
        private UserManager<ApplicationUser> userManager;

        public AnimalController(ApplicationDbContext db, UserManager<ApplicationUser> userManager) {
            this.db = db;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index() {
            ApplicationUser user = await userManager.GetUserAsync(User);
            db.Entry(user).Collection(u => u.Buildings).Load();
            foreach (Building building in user.Buildings) {
                db.Entry(building).Collection(b => b.Animals).Load();
                foreach (Animal animal in building.Animals) {
                    db.Entry(animal).Reference(a => a.Type).Load();
                }
            }
            return View(user.Buildings);
        }
        
        [Route("Animal/{id}")]
        public async Task<IActionResult> Details(int id) {
            Animal animal = db.Animals.Single(a => a.Id == id);
            if (animal == null) {
                return NotFound();
            }
            db.Entry(animal).Reference(a => a.Type).Load();
            db.Entry(animal).Reference(a => a.Building).Load();
            db.Entry(animal.Building).Reference(b => b.User).Load();
            ApplicationUser user = await userManager.GetUserAsync(User);
            if (user.Id != animal.Building.User.Id) {
                return Unauthorized();
            }
            return View(animal);
        }
    }
}
