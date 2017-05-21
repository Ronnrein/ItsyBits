using System.Collections.Generic;

namespace ItsyBits.ViewModels {
    public class BuildingDetailsViewModel {
        
        public BuildingViewModel Building { get; set; }
        public IEnumerable<UpgradeViewModel> Upgrades { get; set; }

    }
}