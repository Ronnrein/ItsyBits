using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ItsyBits.Data;
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
                .Include(b => b.Animals)
                .ThenInclude(a => a.Type)
                .Include(b => b.BuildingUpgrades)
                .ThenInclude(bu => bu.Upgrade);
            return View(_mapper.Map<IEnumerable<Building>, IEnumerable<BuildingViewModel>>(buildings));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id) {
            Building building = await _db.Buildings
                .Include(b => b.Type)
                .Include(b => b.Animals)
                .ThenInclude(a => a.Type)
                .Include(b => b.BuildingUpgrades)
                .ThenInclude(bu => bu.Upgrade)
                .SingleOrDefaultAsync(b => b.Id == id);
            if (building == null) {
                return NotFound();
            }
            if (_userManager.GetUserId(User) != building.UserId) {
                return Unauthorized();
            }
            IEnumerable<Upgrade> upgrades = _db.Upgrades.Where(u => u.ForBuilding);
            return View(new BuildingDetailsViewModel {
                Building = _mapper.Map<Building, BuildingViewModel>(building),
                Upgrades = _mapper.Map<IEnumerable<Upgrade>, IEnumerable<UpgradeViewModel>>(upgrades)
            });
        }

        [HttpGet]
        public IActionResult Plots() {
            string userId = _userManager.GetUserId(User);
            IEnumerable<Building> buildings = _db.Buildings.Where(b => b.UserId == userId).Include(b => b.Type);
            IEnumerable<PlotViewModel> plots = _mapper.Map<IEnumerable<Plot>, IEnumerable<PlotViewModel>>(_db.Plots);
            foreach (PlotViewModel plot in plots) {
                plot.Building = _mapper.Map<Building, BuildingViewModel>(buildings.SingleOrDefault(b => b.PlotId == plot.Id));
            }
            return Json(plots);
        }

    }
}