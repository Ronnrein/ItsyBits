using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ItsyBits.Models.ViewModels.Store {
    public class StoreAnimalUpgradeViewModel {

        [Required(ErrorMessage = "Please select animal to upgrade")]
        [Display(Name = "Which animal would you like to upgrade?")]
        public int? AnimalId { get; set; }

        public IEnumerable<Animal> Animals { get; set; }
        public Upgrade Upgrade { get; set; }

    }
}