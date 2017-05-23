using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ItsyBits.Data;
using ItsyBits.Models;
using ItsyBits.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ItsyBits.Controllers {

    [Authorize]
    public class UserController : Controller {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public UserController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IConfiguration config,
            IMapper mapper
        ) {
            _db = db;
            _userManager = userManager;
            _config = config;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Notifications() {
            IEnumerable<Notification> notifications = _db.Notifications
                .Where(n => n.UserId == _userManager.GetUserId(User))
                .OrderByDescending(n => n.Created);

            // Go through notifications and keep the overall notification count down
            List<Notification> viewNotifications = notifications.ToList();
            for (int i = int.Parse(_config["MaxNotifications"]); i < notifications.Count(); i++) {
                Notification notification = notifications.ElementAt(i);
                _db.Notifications.Remove(notification);
                viewNotifications.Remove(notification);
            }

            // Set all notifications to read
            foreach (Notification notification in viewNotifications.Where(n => !n.IsRead)) {
                notification.IsRead = true;
                _db.Update(notification);
            }

            await _db.SaveChangesAsync();
            return View(_mapper.Map<IEnumerable<Notification>, IEnumerable<NotificationViewModel>>(viewNotifications));
        }
    }
}