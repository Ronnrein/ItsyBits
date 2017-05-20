using System.Collections.Generic;

namespace ItsyBits.Models.ViewModels.Store {
    public class StoreIndexViewModel {
        
        public IEnumerable<AnimalType> AnimalTypes { get; set; }
        public IEnumerable<BuildingType> BuildingTypes { get; set; }
        public IEnumerable<Upgrade> Upgrades { get; set; }

    }
}