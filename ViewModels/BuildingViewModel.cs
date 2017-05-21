using System.Collections.Generic;

namespace ItsyBits.ViewModels {
    public class BuildingViewModel {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public int Reward { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string SpritePath { get; set; }
        public string StatusText { get; set; }
        public int HappinessPercentage { get; set; }
        public IEnumerable<AnimalViewModel> Animals { get; set; }
        public IEnumerable<UpgradeViewModel> Upgrades { get; set; }

    }
}