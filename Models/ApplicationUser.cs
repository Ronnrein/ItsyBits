using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ItsyBits.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ItsyBits.Models {

    /// <summary>
    /// User of application
    /// </summary>
    public class ApplicationUser : IdentityUser {

        /// <summary>
        /// Buildings of user
        /// </summary>
        public ICollection<Building> Buildings { get; set; }

        /// <summary>
        /// Animals of user
        /// </summary>
        [NotMapped]
        public IEnumerable<Animal> Animals => Buildings?.SelectMany(b => b.Animals) ?? Enumerable.Empty<Animal>();

        /// <summary>
        /// Overall happiness of all users animals
        /// </summary>
        public int HappinessPercentage => Buildings == null || Buildings.Count == 0 ? 0 : (int) Buildings.Where(b => b.Animals.Count > 0).Average(a => a.HappinessPercentage);

        /// <summary>
        /// Rewards users with currency based on their game status
        /// </summary>
        /// <param name="users">Users to reward</param>
        public static void AwardDailyCurrency(IEnumerable<ApplicationUser> users) {
            Console.WriteLine("User "+users.First().UserName);
        }
    }
    
}
