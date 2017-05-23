using System.Threading.Tasks;
using ItsyBits.Data;
using ItsyBits.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ItsyBits.ViewComponents {
    public class CurrencyViewComponent : ViewComponent {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public CurrencyViewComponent(ApplicationDbContext db, UserManager<ApplicationUser> userManager) {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync() {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            return View(user.Currency);
        }
    }
}