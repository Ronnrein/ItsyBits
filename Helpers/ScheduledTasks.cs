using System;
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
                int reward = 0;
                foreach (Animal animal in user.Animals) {
                    if (!animal.IsAlive) {
                        continue;
                    }
                    reward += (int)(20f * (animal.PetPercentage / 100f));
                }
                user.Currency += reward;
                _db.Update(user);
                if (reward > 0) {
                    _db.Add(new Notification {
                        Message = $"You got {reward} coins from your daily harvest!",
                        Title = "Coins!",
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