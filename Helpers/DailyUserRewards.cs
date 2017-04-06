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
    public class DailyUserRewards {

        private readonly ApplicationDbContext _db;

        public DailyUserRewards(ApplicationDbContext db) {
            _db = db;
        }

        /// <summary>
        /// Reward users based on their animals happiness
        /// </summary>
        public void Reward() {
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
                _db.SaveChanges();
            }
        }

    }
}