using System.ComponentModel.DataAnnotations;

namespace ItsyBits.ViewModels {
    public class UserManageViewModel {

        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be max {1} characters long.")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

    }
}