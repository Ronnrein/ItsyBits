using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ItsyBits.Data;
using ItsyBits.Models;
using ItsyBits.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItsyBits.ViewComponents {
    public class UserSummaryViewComponent : ViewComponent {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserSummaryViewComponent(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IMapper mapper) {
            _db = db;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync() {
            ApplicationUser user = await _db.Users
                .Include(u => u.Buildings)
                .ThenInclude(b => b.Animals)
                .ThenInclude(a => a.Type)
                .Include(u => u.Buildings)
                .ThenInclude(b => b.Type)
                .Include(u => u.Buildings)
                .ThenInclude(b => b.BuildingUpgrades)
                .ThenInclude(b => b.Upgrade)
                .Include(u => u.Notifications)
                .SingleOrDefaultAsync(u => u.Id == _userManager.GetUserId(HttpContext.User));
            return View(_mapper.Map<ApplicationUser, UserViewModel>(user));
        }
    }
}