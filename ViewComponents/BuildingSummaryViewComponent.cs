using System.Linq;
using System.Threading.Tasks;
using ItsyBits.Data;
using ItsyBits.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItsyBits.ViewComponents {
    public class BuildingSummaryViewComponent : ViewComponent {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public BuildingSummaryViewComponent(ApplicationDbContext db, UserManager<ApplicationUser> userManager) {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync() {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            return View(_db.Buildings
                .Where(b => b.UserId == user.Id)
                .Include(b => b.Animals)
                .ThenInclude(a => a.Type)
            );
        }
    }
}