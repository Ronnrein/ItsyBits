using System.Linq;
using System.Threading.Tasks;
using ItsyBits.Data;
using ItsyBits.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ItsyBits.ViewComponents {
    public class NotificationViewComponent : ViewComponent {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationViewComponent(ApplicationDbContext db, UserManager<ApplicationUser> userManager) {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync() {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            return View(_db.Notifications.Where(n => n.UserId == user.Id));
        }
    }
}