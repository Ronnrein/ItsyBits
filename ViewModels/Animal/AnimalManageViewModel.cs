using System.ComponentModel.DataAnnotations;

namespace ItsyBits.ViewModels {
    public class AnimalManageViewModel {

        [Required(ErrorMessage = "Please enter the animals name")]
        [Display(Name = "What should this animal be called?")]
        [StringLength(20, ErrorMessage = "The name must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string Name { get; set; }

        public string Id { get; set; }
        public string Delete { get; set; }
        public int Refund { get; set; }
        public string Description { get; set; }
        public string SpritePath { get; set; }

    }
}