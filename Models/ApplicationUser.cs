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
        /// The current amount of currency this user has
        /// </summary>
        public int Currency { get; set; }

        /// <summary>
        /// Buildings of user
        /// </summary>
        public ICollection<Building> Buildings { get; set; }

        /// <summary>
        /// Notifications of user
        /// </summary>
        public ICollection<Notification> Notifications { get; set; }

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
        /// Check which of the supplied plots are available for user
        /// </summary>
        /// <param name="plots">The plots to check</param>
        /// <returns>The available plots</returns>
        public IEnumerable<Plot> GetAvailablePlots(IEnumerable<Plot> plots) {
            return plots.Where(p => Buildings.All(b => b.PlotId != p.Id));
        }
    }
    
}
