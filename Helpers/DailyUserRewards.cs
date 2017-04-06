using System;
using System.Collections.Generic;
using System.Linq;
using ItsyBits.Data;
using ItsyBits.Models;
using Microsoft.EntityFrameworkCore;

namespace ItsyBits.Helpers {
    public class DailyUserRewards {

        private readonly ApplicationDbContext _db;

        public DailyUserRewards(ApplicationDbContext db) {
            _db = db;
        }

        public void Reward() {
            IEnumerable<ApplicationUser> users = _db.Users
                .Include(u => u.Buildings)
                .ThenInclude(b => b.Animals)
                .ThenInclude(a => a.Type);
            foreach (ApplicationUser user in users) {
                Console.WriteLine(user.UserName+" - "+user.HappinessPercentage+"%");
            }
        }

    }
}