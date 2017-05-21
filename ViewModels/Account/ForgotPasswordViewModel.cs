using System.ComponentModel.DataAnnotations;

namespace ItsyBits.ViewModels {
    public class ForgotPasswordViewModel {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
