using System.Collections.Generic;

namespace ItsyBits.ViewModels {
    public class AnimalDetailsViewModel {
        
        public AnimalViewModel Animal { get; set; }
        public IEnumerable<UpgradeViewModel> AvailableUpgrades { get; set; }

    }
}