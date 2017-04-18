using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ItsyBits.Data;
using ItsyBits.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ItsyBits.Controllers {

    [Authorize]
    public class UserController : Controller {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(ApplicationDbContext db, UserManager<ApplicationUser> userManager) {
            _db = db;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> ClearNotifications() {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            IEnumerable<Notification> notifications = _db.Notifications
                .Where(n => n.UserId == user.Id)
                .OrderByDescending(n => n.Created);
            for (int i = 20; i < notifications.Count(); i++) {
                _db.Notifications.Remove(notifications.ElementAt(i));
            }
            foreach (Notification notification in notifications.Where(n => !n.Read)) {
                notification.Read = true;
                _db.Update(notification);
            }
            await _db.SaveChangesAsync();
            await _db.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}