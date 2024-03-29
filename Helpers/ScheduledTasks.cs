﻿using System;
using System.Collections.Generic;
using System.Linq;
using ItsyBits.Data;
using ItsyBits.Models;
using Microsoft.EntityFrameworkCore;

namespace ItsyBits.Helpers {

    /// <summary>
    /// Class to handle daily user rewards
    /// </summary>
    public class ScheduledTasks {

        private readonly ApplicationDbContext _db;

        public ScheduledTasks(ApplicationDbContext db) {
            _db = db;
        }

        /// <summary>
        /// Gets fired daily
        /// </summary>
        public void AwardUserCoins() {
            IEnumerable<ApplicationUser> users = _db.Users
                .Include(u => u.Buildings)
                .ThenInclude(b => b.Animals)
                .ThenInclude(a => a.Type);
            foreach (ApplicationUser user in users) {
                int reward = user.Animals.Sum(a => a.Reward);
                user.Currency += reward;
                _db.Update(user);
                if (reward > 0) {
                    _db.Add(new Notification {
                        Message = $"You got {reward} Bits from your daily caretaking!",
                        Title = "Money!",
                        Image = "misc/coins.jpg",
                        Link = "/store",
                        User = user
                    });
                }
            }
            _db.SaveChanges();
        }

        /// <summary>
        /// Gets fired hourly
        /// </summary>
        public void CheckAnimalHealth() {
            IEnumerable<ApplicationUser> users = _db.Users
                .Include(u => u.Buildings)
                .ThenInclude(b => b.Animals)
                .ThenInclude(a => a.Type);
            foreach (ApplicationUser user in users) {
                foreach (Animal animal in user.Animals.Where(a => a.DeathTime.HasValue && (DateTime.Now - a.DeathTime.Value).TotalDays >= 1)) {
                    _db.Animals.Remove(animal);
                    _db.Add(new Notification {
                        Message = $"{animal.Name} decided life was too tough in this town, and decided to move on!",
                        Title = "Animal is gone!",
                        Image = $"animals/{animal.Type.SpritePath}/portrait.png",
                        Link = "/store",
                        User = user
                    });
                }
                foreach (Animal animal in user.Animals.Where(a => !a.DeathTime.HasValue && !a.IsAlive)) {
                    animal.DeathTime = DateTime.Now;
                    _db.Add(new Notification {
                        Message = $"{animal.Name} has run away! You can catch it if you are quick, but in 1 days time it will be gone forever!",
                        Title = "Animal ran away!",
                        Image = $"animals/{animal.Type.SpritePath}/portrait.png",
                        Link = "/animal/recover/"+animal.Id,
                        User = user
                    });
                }
            }
            _db.SaveChanges();
        }

    }
}