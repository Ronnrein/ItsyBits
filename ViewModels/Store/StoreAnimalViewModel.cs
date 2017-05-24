using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ItsyBits.ViewModels {
    public class StoreAnimalViewModel {

        [Required(ErrorMessage = "Please enter the animals name")]
        [Display(Name = "What should this animal be called?")]
        [StringLength(20, ErrorMessage = "The name must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string Name { get; set; }

        [Required(ErrorMessage = "You have to select a building for the animal")]
        [Display(Name = "Where should this animal live?")]
        public int? BuildingId { get; set; }

        public IEnumerable<BuildingViewModel> Buildings { get; set; }
        public AnimalTypeViewModel AnimalType { get; set; }

    }
}