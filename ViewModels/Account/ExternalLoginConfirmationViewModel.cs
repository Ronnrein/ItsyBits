using System.ComponentModel.DataAnnotations;

namespace ItsyBits.ViewModels {
    public class ExternalLoginConfirmationViewModel {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
