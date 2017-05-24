using System;
using System.Linq;
using System.Threading.Tasks;
using ItsyBits.Data;
using ItsyBits.Models;
using ItsyBits.Services;
using ItsyBits.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace ItsyBits.Controllers {
    public class HomeController : Controller {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;

        public HomeController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            IConfiguration config
        ) {
            _db = db;
            _userManager = userManager;
            _emailSender = emailSender;
            _config = config;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index() {
            ApplicationUser user = await _db.Users
                .Include(u => u.Buildings).ThenInclude(b => b.Animals)
                .SingleOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            // If the user has no animals, redirect to the select animal page
            if (!user.Animals.Any()) {
                return RedirectToAction("AnimalSelect", "Store");
            }
            return View();
        }

        [HttpGet]
        public IActionResult About() {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Contact() {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Contact(ContactViewModel contact) {
            ApplicationUser user = await _userManager.GetUserAsync(User);

            // Prepare mail object
            MimeMessage mail = new MimeMessage();
            mail.From.Add(new MailboxAddress(user.UserName, user.Email));
            mail.To.Add(new MailboxAddress(_config["ContactEmail"], _config["ContactEmail"]));
            mail.Subject = "Query from " + user.UserName;
            mail.Body = new TextPart("html") { Text = contact.Message.Replace(Environment.NewLine, "<br />") };
            await _emailSender.SendEmailAsync(mail);

            ViewData["Result"] = new Result("Request received!", "We will get back to you as soon as possible", ResultStatus.Success);
            return View();
        }

        [HttpGet]
        public IActionResult Error(int? id) {
            if (id == null) {
                return View("~/Views/Error/Error.cshtml");
            }
            switch (id) {
                case 404:
                    return View("~/Views/Error/NotFound.cshtml");
                case 403:
                    return View("~/Views/Error/Unauthorized.cshtml");
                default:
                    return View("~/Views/Error/Error.cshtml");
            }
            
        }

    }
}
