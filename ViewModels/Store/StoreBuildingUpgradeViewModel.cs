using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ItsyBits.ViewModels {
    public class StoreBuildingUpgradeViewModel {

        [Required(ErrorMessage = "Please select building to upgrade")]
        [Display(Name = "Which building would you like to upgrade?")]
        public int? BuildingId { get; set; }

        public IEnumerable<BuildingViewModel> Buildings { get; set; }
        public UpgradeViewModel Upgrade { get; set; }

    }
}