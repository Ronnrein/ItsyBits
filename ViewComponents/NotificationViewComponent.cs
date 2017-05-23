using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ItsyBits.Data;
using ItsyBits.Models;
using ItsyBits.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ItsyBits.ViewComponents {
    public class NotificationViewComponent : ViewComponent {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public NotificationViewComponent(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IMapper mapper) {
            _db = db;
            _userManager = userManager;
            _mapper = mapper;
        }

        public IViewComponentResult Invoke() {
            IEnumerable<Notification> notifications = _db.Notifications.Where(n => n.UserId == _userManager.GetUserId(HttpContext.User));
            return View(_mapper.Map<IEnumerable<Notification>, IEnumerable<NotificationViewModel>>(notifications));
        }
    }
}