using System.Linq;
using System.Threading.Tasks;
using ItsyBits.Data;
using ItsyBits.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItsyBits.ViewComponents {
    public class UserSummaryViewComponent : ViewComponent {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserSummaryViewComponent(ApplicationDbContext db, UserManager<ApplicationUser> userManager) {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync() {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            return View(_db.Users
                .Include(u => u.Buildings)
                .ThenInclude(b => b.Animals)
                .ThenInclude(a => a.Type)
                .Include(u => u.Buildings)
                .ThenInclude(b => b.Type)
                .Include(u => u.Buildings)
                .ThenInclude(b => b.BuildingUpgrades)
                .ThenInclude(b => b.Upgrade)
                .Include(u => u.Notifications)
                .Single(u => u.Id == user.Id)
            );
        }
    }
}