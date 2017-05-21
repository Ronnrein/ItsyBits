﻿using System.Collections.Generic;

namespace ItsyBits.ViewModels {
    public class StoreIndexViewModel {
        
        public IEnumerable<AnimalTypeViewModel> AnimalTypes { get; set; }
        public IEnumerable<BuildingTypeViewModel> BuildingTypes { get; set; }
        public IEnumerable<UpgradeViewModel> Upgrades { get; set; }

    }
}