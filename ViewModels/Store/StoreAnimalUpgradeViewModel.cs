using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ItsyBits.ViewModels {
    public class StoreAnimalUpgradeViewModel {

        [Required(ErrorMessage = "Please select animal to upgrade")]
        [Display(Name = "Which animal would you like to upgrade?")]
        public int? AnimalId { get; set; }

        public IEnumerable<AnimalViewModel> Animals { get; set; }
        public UpgradeViewModel Upgrade { get; set; }

    }
}