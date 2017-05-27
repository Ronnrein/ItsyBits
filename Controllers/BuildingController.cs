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

namespace ItsyBits.Controllers {

    [Authorize]
    public class BuildingController : Controller {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public BuildingController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IMapper mapper
        ) {
            _db = db;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index() {
            IEnumerable<Building> buildings = _db.Buildings
                .Where(b => b.UserId == _userManager.GetUserId(User))
                .Include(b => b.Type)
                .Include(b => b.Animals).ThenInclude(a => a.Type)
                .Include(b => b.Animals).ThenInclude(a => a.AnimalUpgrades).ThenInclude(au => au.Upgrade)
                .Include(b => b.BuildingUpgrades).ThenInclude(bu => bu.Upgrade);
            return View(_mapper.Map<IEnumerable<Building>, IEnumerable<BuildingViewModel>>(buildings));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id) {
            Building building = await _db.Buildings
                .Include(b => b.Type)
                .Include(b => b.Animals).ThenInclude(a => a.Type)
                .Include(b => b.Animals).ThenInclude(a => a.AnimalUpgrades).ThenInclude(au => au.Upgrade)
                .Include(b => b.BuildingUpgrades).ThenInclude(bu => bu.Upgrade)
                .SingleOrDefaultAsync(b => b.Id == id);
            if (building == null) {
                return NotFound();
            }
            if (_userManager.GetUserId(User) != building.UserId) {
                return StatusCode((int) HttpStatusCode.Forbidden);
            }
            IEnumerable<Upgrade> upgrades = _db.Upgrades.Where(u => u.ForBuilding);
            return View(new BuildingDetailsViewModel {
                Building = _mapper.Map<Building, BuildingViewModel>(building),
                Upgrades = _mapper.Map<IEnumerable<Upgrade>, IEnumerable<UpgradeViewModel>>(upgrades)
            });
        }

        [HttpGet]
        public async Task<IActionResult> Manage(int id) {
            Building building = await _db.Buildings
                .Include(a => a.Type)
                .SingleOrDefaultAsync(a => a.Id == id);
            if (building == null) {
                return NotFound();
            }
            if (_userManager.GetUserId(User) != building.UserId) {
                return StatusCode((int) HttpStatusCode.Forbidden);
            }
            return View(_mapper.Map<Building, BuildingManageViewModel>(building));
        }

        [HttpPost]
        public async Task<IActionResult> Manage(int id, BuildingManageViewModel buildingVm) {
            Building building = await _db.Buildings
                .Include(a => a.Type)
                .SingleOrDefaultAsync(a => a.Id == id);
            if (building == null) {
                return NotFound();
            }
            if (_userManager.GetUserId(User) != building.UserId) {
                return StatusCode((int) HttpStatusCode.Forbidden);
            }
            if (!string.IsNullOrEmpty(buildingVm.Delete)) {
                _db.Buildings.Remove(building);
                buildingVm = _mapper.Map<Building, BuildingManageViewModel>(building);
                ApplicationUser user = await _userManager.GetUserAsync(User);
                _db.Users.Attach(user);
                user.Currency += buildingVm.Refund;
                _db.Entry(user).Property(u => u.Currency).IsModified = true;
                await _db.SaveChangesAsync();
                TempData.Put("Result", new Result("Sold!", "You sold your building", ResultStatus.Success));
                return RedirectToAction("Index");
            }
            if (!ModelState.IsValid) {
                return View(buildingVm);
            }
            building.Name = buildingVm.Name;
            _db.Update(building);
            await _db.SaveChangesAsync();
            TempData.Put("Result", new Result("Renamed!", "You renamed your building", ResultStatus.Success));
            return RedirectToAction("Details", new { id });
        }

        [HttpGet]
        public IActionResult Plots() {
            string userId = _userManager.GetUserId(User);
            IEnumerable<Building> buildings = _db.Buildings
                .Where(b => b.UserId == userId)
                .Include(b => b.Type)
                .Include(b => b.BuildingUpgrades).ThenInclude(bu => bu.Upgrade)
                .Include(b => b.Animals);
            IEnumerable<PlotViewModel> plots = _mapper.Map<IEnumerable<Plot>, IEnumerable<PlotViewModel>>(_db.Plots);

            // Set the building property on each plot of user to the correct one
            foreach (PlotViewModel plot in plots) {
                Building building = buildings.SingleOrDefault(b => b.PlotId == plot.Id);
                if (building == null) {
                    plot.BuildingId = 0;
                    plot.Name = "Empty";
                    continue;
                }
                plot.BuildingId = building.Id;
                plot.SpritePath = building.Type.SpritePath;
                plot.Animals = building.Animals.Count;
                plot.Capacity = building.Capacity;
                plot.Name = building.Name;
            }
            return Json(plots);
        }

    }
}