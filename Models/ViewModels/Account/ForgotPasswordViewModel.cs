using System.ComponentModel.DataAnnotations;

namespace ItsyBits.Models.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
