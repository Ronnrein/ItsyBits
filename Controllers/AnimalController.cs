using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using ItsyBits.Data;
using ItsyBits.Helpers;
using ItsyBits.Models;
using ItsyBits.ViewModels;
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
        private readonly IMapper _mapper;

        public AnimalController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IConfiguration config,
            IMapper mapper
        ) {
            _db = db;
            _userManager = userManager;
            _config = config;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index() {
            IEnumerable<Building> buildings = _db.Buildings
                .Where(b => b.UserId == _userManager.GetUserId(User))
                .Include(b => b.Animals).ThenInclude(a => a.Type)
                .Include(b => b.Animals).ThenInclude(a => a.AnimalUpgrades).ThenInclude(au => au.Upgrade)
                .Include(b => b.BuildingUpgrades).ThenInclude(bu => bu.Upgrade)
                .AsEnumerable();
            return View(_mapper.Map<IEnumerable<Building>, IEnumerable<BuildingViewModel>>(buildings));
        }
        
        [HttpGet]
        public async Task<IActionResult> Details(int id) {
            Animal animal = await _db.Animals
                .Include(a => a.Building).ThenInclude(b => b.Type)
                .Include(a => a.Building).ThenInclude(b => b.BuildingUpgrades).ThenInclude(bu => bu.Upgrade)
                .Include(a => a.Type)
                .Include(a => a.AnimalUpgrades).ThenInclude(au => au.Upgrade)
                .SingleOrDefaultAsync(a => a.Id == id);
            if (animal == null) {
                return NotFound();
            }
            if (animal.DeathTime != null) {
                return RedirectToAction("Recover", new { id = animal.Id });
            }
            if (_userManager.GetUserId(User) != animal.Building.UserId) {
                return StatusCode((int) HttpStatusCode.Forbidden);
            }
            IEnumerable<Upgrade> upgrades = _db.Upgrades.Where(u => u.ForAnimal);
            return View(new AnimalDetailsViewModel {
                Animal = _mapper.Map<Animal, AnimalViewModel>(animal),
                AvailableUpgrades = _mapper.Map<IEnumerable<Upgrade>, IEnumerable<UpgradeViewModel>>(upgrades)
            });
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
            if (_userManager.GetUserId(User) != animal.Building.UserId) {
                return StatusCode((int) HttpStatusCode.Forbidden);
            }
            return View(_mapper.Map<Animal, AnimalViewModel>(animal));
        }

        [HttpPost, ActionName("Recover")]
        public async Task<IActionResult> RecoverAnimal(int id) {
            Animal animal = await _db.Animals
                .Include(a => a.AnimalUpgrades).ThenInclude(au => au.Upgrade)
                .Include(a => a.Building).ThenInclude(b => b.BuildingUpgrades).ThenInclude(bu => bu.Upgrade)
                .Include(a => a.Type)
                .SingleOrDefaultAsync(a => a.Id == id);
            if (animal == null) {
                return NotFound();
            }
            if (_userManager.GetUserId(User) != animal.Building.UserId) {
                return StatusCode((int) HttpStatusCode.Forbidden);
            }

            // Set animal happiness to percentage in config
            animal.HappinessPercentage = int.Parse(_config["AnimalRecoveryHappiness"]);
            _db.Update(animal);
            await _db.SaveChangesAsync();
            TempData.Put("Result", new Result("Recovered!", "You recovered your animal", ResultStatus.Success));
            return RedirectToAction("Details", new { id = animal.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Manage(int id) {
            Animal animal = await _db.Animals
                .Include(a => a.Type)
                .Include(a => a.Building)
                .SingleOrDefaultAsync(a => a.Id == id);
            if (animal == null) {
                return NotFound();
            }
            if (animal.DeathTime != null) {
                return RedirectToAction("Recover", new { id = animal.Id });
            }
            if (_userManager.GetUserId(User) != animal.Building.UserId) {
                return StatusCode((int) HttpStatusCode.Forbidden);
            }
            return View(_mapper.Map<Animal, AnimalManageViewModel>(animal));
        }

        [HttpPost]
        public async Task<IActionResult> Manage(int id, AnimalManageViewModel animalVm) {
            Animal animal = await _db.Animals
                .Include(a => a.Type)
                .Include(a => a.Building)
                .SingleOrDefaultAsync(a => a.Id == id);
            if (animal == null) {
                return NotFound();
            }
            if (_userManager.GetUserId(User) != animal.Building.UserId) {
                return StatusCode((int) HttpStatusCode.Forbidden);
            }
            if (!string.IsNullOrEmpty(animalVm.Delete)) {
                _db.Animals.Remove(animal);
                animalVm = _mapper.Map<Animal, AnimalManageViewModel>(animal);
                ApplicationUser user = await _userManager.GetUserAsync(User);
                _db.Users.Attach(user);
                user.Currency += animalVm.Refund;
                _db.Entry(user).Property(u => u.Currency).IsModified = true;
                await _db.SaveChangesAsync();
                TempData.Put("Result", new Result("Sold!", "You sold your animal", ResultStatus.Success));
                return RedirectToAction("Index");
            }
            if (!ModelState.IsValid) {
                return View(animalVm);
            }
            animal.Name = animalVm.Name;
            _db.Update(animal);
            await _db.SaveChangesAsync();
            TempData.Put("Result", new Result("Renamed!", "You renamed your animal", ResultStatus.Success));
            return RedirectToAction("Details", new {id});
        }

        [HttpPost]
        public async Task<IActionResult> Feed(int id) {
            Animal animal = await _db.Animals
                .Include(a => a.AnimalUpgrades).ThenInclude(au => au.Upgrade)
                .Include(a => a.Building).ThenInclude(b => b.BuildingUpgrades).ThenInclude(bu => bu.Upgrade)
                .Include(a => a.Type)
                .SingleOrDefaultAsync(a => a.Id == id);
            if (animal == null) {
                return NotFound();
            }
            if (_userManager.GetUserId(User) != animal.Building.UserId) {
                return StatusCode((int) HttpStatusCode.Forbidden);
            }
            animal.FeedPercentage = 100;
            _db.Update(animal);
            await _db.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> Sleep(int id) {
            Animal animal = await _db.Animals
                .Include(a => a.AnimalUpgrades).ThenInclude(au => au.Upgrade)
                .Include(a => a.Building).ThenInclude(b => b.BuildingUpgrades).ThenInclude(bu => bu.Upgrade)
                .Include(a => a.Type)
                .SingleOrDefaultAsync(a => a.Id == id);
            if (animal == null) {
                return NotFound();
            }
            if (_userManager.GetUserId(User) != animal.Building.UserId) {
                return StatusCode((int) HttpStatusCode.Forbidden);
            }
            animal.SleepPercentage = 100;
            _db.Update(animal);
            _db.SaveChanges();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> Pet(int id) {
            Animal animal = await _db.Animals
                .Include(a => a.AnimalUpgrades).ThenInclude(au => au.Upgrade)
                .Include(a => a.Building).ThenInclude(b => b.BuildingUpgrades).ThenInclude(bu => bu.Upgrade)
                .Include(a => a.Type)
                .SingleOrDefaultAsync(a => a.Id == id);
            if (animal == null) {
                return NotFound();
            }
            if (_userManager.GetUserId(User) != animal.Building.UserId) {
                return StatusCode((int) HttpStatusCode.Forbidden);
            }
            animal.PetPercentage = 100;
            _db.Update(animal);
            _db.SaveChanges();
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
