using System.ComponentModel.DataAnnotations;

namespace ItsyBits.Models.ViewModels.Store {
    public class StoreBuildingViewModel {

        [Required(ErrorMessage = "Please enter the name of the building")]
        [Display(Name = "What should this building be called?")]
        [StringLength(10, ErrorMessage = "The name must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string Name { get; set; }

        [Required(ErrorMessage = "You have to select a plot for this building")]
        [Display(Name = "Where should this building be placed?")]
        public int? PlotId { get; set; }

    }
}
