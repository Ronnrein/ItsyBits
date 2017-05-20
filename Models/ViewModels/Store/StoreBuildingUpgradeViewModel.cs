using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ItsyBits.Models.ViewModels.Store {
    public class StoreBuildingUpgradeViewModel {

        [Required(ErrorMessage = "Please select building to upgrade")]
        [Display(Name = "Which building would you like to upgrade?")]
        public int? BuildingId { get; set; }

        public IEnumerable<Building> Buildings { get; set; }
        public Upgrade Upgrade { get; set; }

    }
}