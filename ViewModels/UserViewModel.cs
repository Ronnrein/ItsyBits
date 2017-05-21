using System.Collections.Generic;

namespace ItsyBits.ViewModels {
    public class UserViewModel {

        public string UserName { get; set; }
        public int Reward { get; set; }
        public int HappinessPercentage { get; set; }
        public int Currency { get; set; }
        public IEnumerable<BuildingViewModel> Buildings { get; set; }
        public IEnumerable<AnimalViewModel> Animals { get; set; }

    }
}