using System;
using System.Threading.Tasks;
using ItsyBits.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using ItsyBits.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ItsyBits.Areas.Admin.Controllers {

    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class UserController : Controller {

        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager) {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index() {
            return View(_userManager.Users);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id) {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user == null) {
                return NotFound();
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationUserVm userVm) {
            if (!ModelState.IsValid) {
                return View(userVm);
            }
            ApplicationUser user = new ApplicationUser();
            user.UserName = userVm.UserName;
            user.NormalizedUserName = user.UserName.ToUpper();
            user.Email = userVm.Email;
            user.NormalizedEmail = user.Email.ToUpper();
            PasswordHasher<ApplicationUser> hasher = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = hasher.HashPassword(user, userVm.NewPassword);
            user.EmailConfirmed = userVm.EmailConfirmed;
            user.LockoutEnabled = userVm.LockoutEnabled;
            user.LockoutEnd = userVm.LockoutEnd;
            user.PhoneNumber = userVm.PhoneNumber;
            user.PhoneNumberConfirmed = userVm.PhoneNumberConfirmed;
            user.TwoFactorEnabled = userVm.TwoFactorEnabled;
            user.Currency = userVm.Currency;
            await _userManager.CreateAsync(user);
            return RedirectToAction("Details", new { id = user.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id) {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user == null) {
                return NotFound();
            }
            return View(new ApplicationUserVm {
                UserName =  user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                LockoutEnd = user.LockoutEnd,
                LockoutEnabled = user.LockoutEnabled,
                Currency = user.Currency
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUserVm userVm) {
            if (id != userVm.Id) {
                return NotFound();
            }
            if (!ModelState.IsValid) {
                return View(userVm);
            }
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            user.UserName = userVm.UserName;
            user.NormalizedUserName = user.UserName.ToUpper();
            user.Email = userVm.Email;
            user.NormalizedEmail = user.Email.ToUpper();
            if (!String.IsNullOrEmpty(userVm.NewPassword)) {
                PasswordHasher<ApplicationUser> hasher = new PasswordHasher<ApplicationUser>();
                user.PasswordHash = hasher.HashPassword(user, userVm.NewPassword);
            }
            user.EmailConfirmed = userVm.EmailConfirmed;
            user.LockoutEnabled = userVm.LockoutEnabled;
            user.LockoutEnd = userVm.LockoutEnd;
            user.PhoneNumber = userVm.PhoneNumber;
            user.PhoneNumberConfirmed = userVm.PhoneNumberConfirmed;
            user.TwoFactorEnabled = userVm.TwoFactorEnabled;
            user.Currency = userVm.Currency;
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Details", new { id = user.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id) {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user == null) {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id) {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }
    }
}
