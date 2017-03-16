﻿using System;
using System.Linq;
using System.Threading.Tasks;
using ItsyBits.Data;
using ItsyBits.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItsyBits.Controllers {

    [Authorize]
    public class AnimalController : Controller {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public AnimalController(ApplicationDbContext db, UserManager<ApplicationUser> userManager) {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index() {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            return View(_db.Buildings
                .Include(b => b.Animals)
                .ThenInclude(a => a.Type)
                .Where(b => b.UserId == user.Id)
                .SelectMany(b => b.Animals)
            );
        }
        
        [HttpGet]
        public async Task<IActionResult> Details(int id) {
            Animal animal = await _db.Animals
                .Include(a => a.Building)
                .Include(a => a.Type)
                .SingleOrDefaultAsync(a => a.Id == id);
            if (animal == null) {
                return NotFound();
            }
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user.Id != animal.Building.UserId) {
                return Unauthorized();
            }
            return View(animal);
        }

        [HttpPost]
        public async Task<IActionResult> Feed(int id) {
            Animal animal = await _db.Animals
                .Include(a => a.Building)
                .SingleOrDefaultAsync(a => a.Id == id);
            if (animal == null) {
                return NotFound();
            }
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user.Id != animal.Building.UserId) {
                return Unauthorized();
            }
            animal.LastFeed = DateTime.Now;
            _db.Update(animal);
            await _db.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> Sleep(int id) {
            Animal animal = await _db.Animals
                .Include(a => a.Building)
                .SingleOrDefaultAsync(a => a.Id == id);
            if (animal == null) {
                return NotFound();
            }
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user.Id != animal.Building.UserId) {
                return Unauthorized();
            }
            animal.LastSleep = DateTime.Now;
            _db.Update(animal);
            _db.SaveChanges();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> Pet(int id) {
            Animal animal = await _db.Animals
                .Include(a => a.Building)
                .SingleOrDefaultAsync(a => a.Id == id);
            if (animal == null) {
                return NotFound();
            }
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user.Id != animal.Building.UserId) {
                return Unauthorized();
            }
            animal.LastPet = DateTime.Now;
            _db.Update(animal);
            _db.SaveChanges();
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
