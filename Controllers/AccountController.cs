using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ItsyBits.Data;
using ItsyBits.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ItsyBits.Models;
using ItsyBits.Services;
using ItsyBits.ViewModels;
using Microsoft.Extensions.Configuration;

namespace ItsyBits.Controllers {

    [Authorize]
    public class AccountController : Controller {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly string _externalCookieScheme;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<IdentityCookieOptions> identityCookieOptions,
            IEmailSender emailSender,
            ILoggerFactory loggerFactory,
            ApplicationDbContext db,
            IConfiguration config
        ) {
            _userManager = userManager;
            _signInManager = signInManager;
            _externalCookieScheme = identityCookieOptions.Value.ExternalCookieAuthenticationScheme;
            _emailSender = emailSender;
            _db = db;
            _config = config;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null) {

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.Authentication.SignOutAsync(_externalCookieScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null) {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid) {
                ApplicationUser user;

                // Check the input is a valid email address, if so get user from that
                if (new EmailAddressAttribute().IsValid(model.User)) {
                    user = await _userManager.FindByEmailAsync(model.User);
                    model.User = user.UserName;
                }
                else {
                    user = await _userManager.FindByNameAsync(model.User);
                }
                if (user != null && !user.EmailConfirmed) {
                    TempData.Put("Result", new Result("Email not confirmed!", "Check your email for an activation link to activate your account!", ResultStatus.Error));
                    return View(model);
                }
                var result = await _signInManager.PasswordSignInAsync(model.User, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded) {
                    return RedirectToLocal(returnUrl);
                }
                if (result.IsLockedOut) {
                    return View("Lockout");
                }
                TempData.Put("Result", new Result("Wrong login!", "Wrong username, email or password!", ResultStatus.Error));
                return View(model);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register() {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model) {
            if (ModelState.IsValid) {

                // Create user, grant currency and building
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, Currency = int.Parse(_config["StartingCoin"]) };
                var result = await _userManager.CreateAsync(user, model.Password);
                BuildingType type = _db.BuildingTypes.OrderBy(bt => bt.Capacity).First();
                _db.Add(new Building {
                    Type = type,
                    Name = model.UserName+"'s first building",
                    Plot = _db.Plots.First(),
                    UserId = user.Id
                });
                _db.Add(new Notification {
                    Message = $"Your farm already has a building, and you have enough money to buy your first pet!",
                    Title = "Welcome to ItsyBits!",
                    Image = $"buildings/{type.SpritePath}/portrait.png",
                    Link = "/store/animalselect",
                    UserId = user.Id
                });
                await _db.SaveChangesAsync();
                if (result.Succeeded) {

                    // Send account confirmation email
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    await _emailSender.SendEmailAsync(model.Email, "Confirm your account", $"Please confirm your account by clicking this link:<br /><a href='{callbackUrl}'>{callbackUrl}</a>");
                    TempData.Put("Result", new Result("Registered!", "Check your email for an activation link to activate your account!", ResultStatus.Success));
                    return RedirectToAction("Login");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            TempData.Put("Result", new Result("Logged out!", "You have logged out. Come back soon!", ResultStatus.Success));
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code) {
            Result resultMessage = new Result("Invalid confirmation!", "Cannot confirm activation", ResultStatus.Error);
            if (userId == null || code == null) {
                TempData.Put("Result", resultMessage);
                return View("Login");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) {
                TempData.Put("Result", resultMessage);
                return View("Login");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded) {
                TempData.Put("Result", new Result("Email confirmed!", "You may now log in using your username and password", ResultStatus.Success));
                return View("Login");
            }
            TempData.Put("Result", resultMessage);
            return View("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword() {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model) {
            if (ModelState.IsValid) {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user))) {

                    // If the user doesnt exist or is not confirmed, do not reveal this and simply display the same result as if it was
                    TempData.Put("Result", new Result("Password reset!", "Check your email for a reset link to reset your password!", ResultStatus.Success));
                    return View("Login");
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action(nameof(ResetPassword), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Reset Password", $"Please reset your password by clicking here:<br /><a href='{callbackUrl}'>{callbackUrl}</a>");
                TempData.Put("Result", new Result("Password reset!", "Check your email for a reset link to reset your password!", ResultStatus.Success));
                return View("Login");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation() {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null) {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model) {
            if (!ModelState.IsValid) {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) {

                // Don't reveal that the user does not exist
                TempData.Put("Result", new Result("Password reset!", "Check your email for a reset link to reset your password!", ResultStatus.Success));
                return RedirectToAction("Login");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded) {
                TempData.Put("Result", new Result("Password reset!", "Check your email for a reset link to reset your password!", ResultStatus.Success));
                return RedirectToAction("Login");
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation() {
            return View();
        }

        private void AddErrors(IdentityResult result) {
            foreach (var error in result.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl) {
            if (Url.IsLocalUrl(returnUrl)) {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

    }
}
