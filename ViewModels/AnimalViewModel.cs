using System.Collections.Generic;

namespace ItsyBits.ViewModels {
    public class AnimalViewModel {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Age { get; set; }
        public bool Male { get; set; }
        public int Reward { get; set; }
        public int HappinessPercentage { get; set; }
        public int FeedPercentage { get; set; }
        public int SleepPercentage { get; set; }
        public int PetPercentage { get; set; }
        public string StatusText { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string SpritePath { get; set; }
        public BuildingViewModel Building { get; set; }
        public IEnumerable<UpgradeViewModel> Upgrades { get; set; }

    }
}
