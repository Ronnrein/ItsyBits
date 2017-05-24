using System.Threading.Tasks;
using ItsyBits.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ItsyBits.ViewComponents {
    public class CurrencyViewComponent : ViewComponent {

        private readonly UserManager<ApplicationUser> _userManager;

        public CurrencyViewComponent(UserManager<ApplicationUser> userManager) {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync() {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            return View(user.Currency);
        }
    }
}