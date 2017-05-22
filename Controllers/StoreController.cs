using System;
using System.Collections.Generic;
using System.Linq;
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
    public class StoreController : Controller {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public StoreController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IConfiguration config, IMapper mapper
        ) {
            _db = db;
            _userManager = userManager;
            _config = config;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index() {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            ViewData["Currency"] = user.Currency;
            return View(new StoreIndexViewModel {
                AnimalTypes = _mapper.Map<IEnumerable<AnimalType>, IEnumerable<AnimalTypeViewModel>>(_db.AnimalTypes),
                BuildingTypes = _mapper.Map<IEnumerable<BuildingType>, IEnumerable<BuildingTypeViewModel>>(_db.BuildingTypes),
                Upgrades = _mapper.Map<IEnumerable<Upgrade>, IEnumerable<UpgradeViewModel>>(_db.Upgrades)
            });
        }

        [HttpGet]
        public async Task<IActionResult> Animal(int id) {
            Result error = null;

            ApplicationUser user = await _userManager.GetUserAsync(User);
            IEnumerable<Building> buildings = _db.Buildings
                .Include(b => b.Type)
                .Include(b => b.Animals)
                .Include(b => b.BuildingUpgrades)
                .ThenInclude(bu => bu.Upgrade)
                .Where(b => b.UserId == user.Id && b.Animals.Count < b.Capacity);
            AnimalType type = await _db.AnimalTypes.SingleOrDefaultAsync(a => a.Id == id);

            if (type == null) {
                return NotFound();
            }

            // Check whether the user fulfills requirements to buy this animal
            if (!buildings.Any()) {
                error = new Result("No buildings!", "You have no buildings with free space to put animals in!", ResultStatus.Error);
            }
            else if (user.Currency < type.Price) {
                error = new Result("Cannot afford!", "You cannot afford this animal!", ResultStatus.Error);
            }
            if (error != null) {
                TempData.Put("Result", error);
                return RedirectToAction("Index");
            }

            return View(new StoreAnimalViewModel {
                Buildings = _mapper.Map<IEnumerable<Building>, IEnumerable<BuildingViewModel>>(buildings),
                AnimalType = _mapper.Map<AnimalType, AnimalTypeViewModel>(type)
            });
        }

        [HttpPost]
        public async Task<IActionResult> Animal(int id, StoreAnimalViewModel animalVm) {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            AnimalType type = await _db.AnimalTypes.SingleOrDefaultAsync(a => a.Id == id);
            Building building = await _db.Buildings
                .Include(b => b.Type)
                .Include(b => b.Animals)
                .Include(b => b.BuildingUpgrades)
                .ThenInclude(bu => bu.Upgrade)
                .SingleOrDefaultAsync(b => b.Id == animalVm.BuildingId);

            // Check whether the building has room for more animals
            if (ModelState.IsValid && building.Animals.Count >= building.Capacity) {
                ModelState.SetModelValue(nameof(animalVm.BuildingId), "No room in this building for the animal");
            }  

            // If there are errors, or the modelstate is invalid, show view with errors displayed
            if (!ModelState.IsValid) {
                IEnumerable<Building> buildings = _db.Buildings
                    .Include(b => b.Type)
                    .Include(b => b.Animals)
                    .Include(b => b.BuildingUpgrades)
                    .ThenInclude(bu => bu.Upgrade)
                    .Where(b => b.UserId == user.Id && b.Animals.Count < b.Capacity
                );
                return View(new StoreAnimalViewModel {
                    Buildings = _mapper.Map<IEnumerable<Building>, IEnumerable<BuildingViewModel>>(buildings),
                    AnimalType = _mapper.Map<AnimalType, AnimalTypeViewModel>(type)
                });
            }

            // Check for requests that should not be possible
            if (type == null) {
                return NotFound();
            }
            if (building.UserId != user.Id) {
                return Unauthorized();
            }
            if (user.Currency < type.Price) {
                return BadRequest();
            }

            // At this point all should be well, set up the model and update database
            Animal animal = _mapper.Map<StoreAnimalViewModel, Animal>(animalVm);
            animal.Male = new Random().NextDouble() < 0.5;
            animal.Type = await _db.AnimalTypes.SingleOrDefaultAsync(at => at.Id == id);
            animal.HappinessPercentage = int.Parse(_config["AnimalStartingHappiness"]);
            _db.Users.Attach(user);
            user.Currency -= type.Price;
            _db.Entry(user).Property(u => u.Currency).IsModified = true;
            _db.Add(animal);

            _db.Add(new Notification {
                Message = $"Your farm welcomes a new pet!",
                Title = "New pet!",
                Image = $"animals/{type.SpritePath}/portrait.png",
                Link = "/animal",
                UserId = user.Id
            });

            await _db.SaveChangesAsync();
            return RedirectToAction("Details", "Animal", new { id = animal.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Building(int id) {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            BuildingType type = await _db.BuildingTypes.SingleOrDefaultAsync(a => a.Id == id);

            if (type == null) {
                return NotFound();
            }

            // Check whether the user fulfills requirements to buy this 
            if (user.Currency < type.Price) {
                TempData.Put("Result", new Result("Cannot afford!", "You cannot afford this animal!", ResultStatus.Error));
                return RedirectToAction("Index");
            }

            return View(new StoreBuildingViewModel {
                BuildingType = _mapper.Map<BuildingType, BuildingTypeViewModel>(type)
            });
        }

        [HttpPost]
        public async Task<IActionResult> Building(int id, StoreBuildingViewModel buildingVm) {

            // If the modelstate is invalid, show view with errors displayed
            if (!ModelState.IsValid) {
                return View(new StoreBuildingViewModel {
                    BuildingType = _mapper.Map<BuildingType, BuildingTypeViewModel>(await _db.BuildingTypes.SingleOrDefaultAsync(a => a.Id == id))
                });
            }

            // At this point all should be well, set up the model and update database
            Building building = _mapper.Map<StoreBuildingViewModel, Building>(buildingVm);
            ApplicationUser user = await _userManager.GetUserAsync(User);
            BuildingType type = await _db.BuildingTypes.SingleOrDefaultAsync(bt => bt.Id == id);

            // Check for requests that should not be possible
            if (type == null) {
                return NotFound();
            }
            if (user.Currency < type.Price) {
                return BadRequest();
            }

            Building plotBuilding = await _db.Buildings
                .Include(b => b.Animals)
                .SingleOrDefaultAsync(b => b.UserId == user.Id && b.PlotId == buildingVm.PlotId
            );

            // Check whether the plot is already occupied
            if (plotBuilding != null) {

                // Check if the new building can support the animals of the previous building, if so simply update the old building
                if (plotBuilding.Animals.Count > type.Capacity) {
                    ModelState.SetModelValue(nameof(buildingVm.PlotId), "You cannot replace this building as it does not have enough room to store your existing animals");
                    return View(new StoreBuildingViewModel {
                        BuildingType = _mapper.Map<BuildingType, BuildingTypeViewModel>(type)
                    });
                }
                string name = building.Name;
                building = plotBuilding;
                building.Name = name;
                _db.Update(building);
            }
            else {
                building.UserId = user.Id;
                building.Id = 0;
                _db.Add(building);
            }

            building.TypeId = id;
            _db.Users.Attach(user);
            user.Currency -= type.Price;
            _db.Entry(user).Property(u => u.Currency).IsModified = true;

            _db.Add(new Notification {
                Message = $"You have a new building!",
                Title = "New building!",
                Image = $"buildings/{type.SpritePath}/portrait.png",
                Link = "/building",
                UserId = user.Id
            });

            await _db.SaveChangesAsync();
            return RedirectToAction("Details", "Building", new { id = building.Id });
        }

        [HttpGet]
        public async Task<IActionResult> AnimalUpgrade(int id) {
            Result error = null;

            ApplicationUser user = await _db.Users
                .Include(u => u.Buildings)
                .ThenInclude(b => b.Animals)
                .ThenInclude(a => a.Type)
                .SingleOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));
            Upgrade upgrade = await _db.Upgrades.SingleOrDefaultAsync(a => a.Id == id);

            if (upgrade == null) {
                return NotFound();
            }

            // Check whether the user fulfills requirements to buy this upgrade
            if (!user.Animals.Any()) {
                error = new Result("No animals!", "You have no animals to upgrade!", ResultStatus.Error);
            }
            else if (user.Currency < upgrade.Price) {
                error = new Result("Cannot afford!", "You cannot afford this upgrade!", ResultStatus.Error);
            }
            if (error != null) {
                TempData.Put("Result", error);
                return RedirectToAction("Index");
            }
            
            return View(new StoreAnimalUpgradeViewModel {
                Animals = _mapper.Map<IEnumerable<Animal>, IEnumerable<AnimalViewModel>>(user.Animals),
                Upgrade = _mapper.Map<Upgrade, UpgradeViewModel>(upgrade)
            });
        }

        [HttpPost]
        public async Task<IActionResult> AnimalUpgrade(int id, StoreAnimalUpgradeViewModel upgradeVm) {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            Upgrade upgrade = await _db.Upgrades.SingleOrDefaultAsync(x => x.Id == id);
            Animal animal = await _db.Animals
                .Include(a => a.AnimalUpgrades)
                .Include(a => a.Building)
                .SingleOrDefaultAsync(a => a.Id == upgradeVm.AnimalId);

            // Check whether record exists
            if (upgrade == null) {
                return NotFound();
            }

            // Check whether this upgrade is stackable, and if it is not, if it is already applied
            if (animal != null && !upgrade.IsStackable && animal.AnimalUpgrades.Any(au => au.UpgradeId == id)) {
                ModelState.SetModelValue(nameof(upgradeVm.AnimalId), "You already upgraded this animal with that upgrade!");
            }

            // If the modelstate is invalid, show view with errors displayed
            if (!ModelState.IsValid) {
                ApplicationUser viewUser = await _db.Users
                    .Include(u => u.Buildings)
                    .ThenInclude(b => b.Animals)
                    .ThenInclude(a => a.Type)
                    .SingleOrDefaultAsync(u => u.Id == user.Id);
                return View(new StoreAnimalUpgradeViewModel {
                    Animals = _mapper.Map<IEnumerable<Animal>, IEnumerable<AnimalViewModel>>(viewUser.Animals),
                    Upgrade = _mapper.Map<Upgrade, UpgradeViewModel>(upgrade)
                });
            }

            // Check for requests that should not be possible
            if (animal.Building.UserId != user.Id) {
                return Unauthorized();
            }
            if (user.Currency < upgrade.Price) {
                return BadRequest();
            }

            // At this point all should be well, set up the model and update database
            AnimalUpgrade animalUpgrade = _mapper.Map<StoreAnimalUpgradeViewModel, AnimalUpgrade>(upgradeVm);
            animalUpgrade.UpgradeId = id;
            _db.Users.Attach(user);
            user.Currency -= upgrade.Price;
            _db.Entry(user).Property(x => x.Currency).IsModified = true;
            _db.Add(animalUpgrade);

            _db.Add(new Notification {
                Message = $"You upgraded your animal!",
                Title = "New upgrade!",
                Image = "upgrades/"+upgrade.SpritePath,
                Link = "/animal/"+animalUpgrade.AnimalId,
                UserId = user.Id
            });
            
            await _db.SaveChangesAsync();
            return RedirectToAction("Details", "Animal", new { id = animalUpgrade.AnimalId });
        }

        [HttpGet]
        public async Task<IActionResult> BuildingUpgrade(int id) {
            Result error = null;

            ApplicationUser user = await _userManager.GetUserAsync(User);
            IEnumerable<Building> buildings = _db.Buildings
                .Where(b => b.UserId == user.Id)
                .Include(b => b.Type);
            Upgrade upgrade = await _db.Upgrades.SingleOrDefaultAsync(a => a.Id == id);

            if (upgrade == null) {
                return NotFound();
            }

            // Check whether the user fulfills requirements to buy this upgrade
            if (!buildings.Any()) {
                error = new Result("No buildings!", "You have no buildings to upgrade!", ResultStatus.Error);
            }
            else if (user.Currency < upgrade.Price) {
                error = new Result("Cannot afford!", "You cannot afford this upgrade!", ResultStatus.Error);
            }
            if (error != null) {
                TempData.Put("Result", error);
                return RedirectToAction("Index");
            }

            return View(new StoreBuildingUpgradeViewModel {
                Buildings = _mapper.Map<IEnumerable<Building>, IEnumerable<BuildingViewModel>>(buildings),
                Upgrade = _mapper.Map<Upgrade, UpgradeViewModel>(upgrade)
            });
        }

        [HttpPost]
        public async Task<IActionResult> BuildingUpgrade(int id, StoreBuildingUpgradeViewModel upgradeVm) {
            Building building = await _db.Buildings
                .Include(b => b.Type)
                .Include(b => b.BuildingUpgrades)
                .ThenInclude(bu => bu.Upgrade)
                .SingleOrDefaultAsync(b => b.Id == upgradeVm.BuildingId);
            Upgrade upgrade = await _db.Upgrades.SingleOrDefaultAsync(u => u.Id == id);
            ApplicationUser user = await _userManager.GetUserAsync(User);

            // Check whether record exists
            if (upgrade == null) {
                return NotFound();
            }

            // Check whether this upgrade affects capacity, and if so affects capacity more than accepted
            if (building.Capacity + upgrade.CapacityModifier > building.Type.MaxCapacity) {
                ModelState.SetModelValue(nameof(upgradeVm.BuildingId), "Your building is already max upgraded!");
            }

            // Check whether this upgrade is stackable, and if it is not, if it is already applied
            if (!upgrade.IsStackable && building.BuildingUpgrades.Any(au => au.UpgradeId == id)) {
                ModelState.SetModelValue(nameof(upgradeVm.BuildingId), "You already upgraded this building with that upgrade!");
            }

            // If the modelstate is invalid, show view with errors displayed
            if (!ModelState.IsValid) {
                IEnumerable<Building> buildings = _db.Buildings
                    .Where(b => b.UserId == user.Id)
                    .Include(b => b.Type);
                return View(new StoreBuildingUpgradeViewModel {
                    Buildings = _mapper.Map<IEnumerable<Building>, IEnumerable<BuildingViewModel>>(buildings),
                    Upgrade = _mapper.Map<Upgrade, UpgradeViewModel>(upgrade)
                });
            }

            // Check for requests that should not be possible
            if (building.UserId != user.Id) {
                return Unauthorized();
            }
            if (user.Currency < upgrade.Price) {
                return BadRequest();
            }

            // At this point all should be well, set up the model and update database
            BuildingUpgrade buildingUpgrade = _mapper.Map<StoreBuildingUpgradeViewModel, BuildingUpgrade>(upgradeVm);
            buildingUpgrade.UpgradeId = id;
            buildingUpgrade.Id = 0;
            _db.Users.Attach(user);
            user.Currency -= upgrade.Price;
            _db.Entry(user).Property(x => x.Currency).IsModified = true;
            _db.Add(buildingUpgrade);

            _db.Add(new Notification {
                Message = $"You upgraded your building!",
                Title = "New upgrade!",
                Image = "upgrades/" + upgrade.SpritePath,
                Link = "/building/" + upgradeVm.BuildingId,
                UserId = user.Id
            });

            await _db.SaveChangesAsync();
            return RedirectToAction("Details", "Building", new { id = upgradeVm.BuildingId });
        }

        [HttpGet]
        public IActionResult AnimalSelect() {
            return View(_mapper.Map<IEnumerable<AnimalType>, IEnumerable<AnimalTypeViewModel>>(_db.AnimalTypes));
        }
    }
}