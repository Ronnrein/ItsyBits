using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ItsyBits.Data;
using ItsyBits.Models;
using Microsoft.AspNetCore.Authorization;

namespace ItsyBits.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class PlotController : Controller {

        private readonly ApplicationDbContext _db;

        public PlotController(ApplicationDbContext db) {
            _db = db;
        }

        [HttpGet]
        public IActionResult Index() {
            return View(_db.Plots);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id) {
            Plot plot = await _db.Plots
                .SingleOrDefaultAsync(m => m.Id == id);
            if (plot == null) {
                return NotFound();
            }
            return View(plot);
        }

        [HttpGet]
        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Plot plot) {
            if (!ModelState.IsValid) {
                return View(plot);
            }
            _db.Add(plot);
            await _db.SaveChangesAsync();
            return RedirectToAction("Details", new { id = plot.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id) {
            Plot plot = await _db.Plots.SingleOrDefaultAsync(m => m.Id == id);
            if (plot == null) {
                return NotFound();
            }
            return View(plot);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Plot plot) {
            if (id != plot.Id) {
                return NotFound();
            }
            if (!ModelState.IsValid) {
                return View(plot);
            }
            try {
                _db.Update(plot);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!_db.Plots.Any(e => e.Id == id)) {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction("Details", new { id = plot.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id) {
            Plot plot = await _db.Plots.SingleOrDefaultAsync(m => m.Id == id);
            if (plot == null) {
                return NotFound();
            }
            return View(plot);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            Plot plot = await _db.Plots.SingleOrDefaultAsync(m => m.Id == id);
            _db.Plots.Remove(plot);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
