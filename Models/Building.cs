﻿using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ItsyBits.Models {

    /// <summary>
    /// Farm buildings containing animals
    /// </summary>
    public class Building {

        /// <summary>
        /// Id of building
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of building
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// User of building
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// Id of user of building
        /// </summary>
        [ForeignKey("User")]
        [DisplayName("User")]
        public string UserId { get; set; }

        /// <summary>
        /// Type of building
        /// </summary>
        public BuildingType Type { get; set; }

        /// <summary>
        /// Id of type of building
        /// </summary>
        [ForeignKey("Type")]
        [DisplayName("Type")]
        public int TypeId { get; set; }

        /// <summary>
        /// Plot of building
        /// </summary>
        public Plot Plot { get; set; }

        /// <summary>
        /// Id of plot of building
        /// </summary>
        [ForeignKey("Plot")]
        [DisplayName("Plot")]
        public int PlotId { get; set; }

        /// <summary>
        /// Animals living in building
        /// </summary>
        public ICollection<Animal> Animals { get; set; }

        /// <summary>
        /// Junction for building upgrades
        /// </summary>
        public ICollection<BuildingUpgrade> BuildingUpgrades { get; set; }

        /// <summary>
        /// Upgrades for building
        /// </summary>
        [NotMapped]
        public IEnumerable<Upgrade> Upgrades => BuildingUpgrades?.Select(au => au.Upgrade) ?? Enumerable.Empty<Upgrade>();

        /// <summary>
        /// Avarage happiness of all animals in building
        /// </summary>
        public int HappinessPercentage => Animals == null || Animals.Count == 0 ? 0 : (int) Animals.Average(a => a.HappinessPercentage);

        /// <summary>
        /// Total capacity of building
        /// </summary>
        public int Capacity => Type == null || BuildingUpgrades == null ? 0 : Type.Capacity + Upgrades.Sum(u => u.CapacityModifier);

        /// <summary>
        /// Reward from the animals in this building
        /// </summary>
        public int Reward => Animals?.Sum(a => a.GetReward()) ?? 0;

        /// <summary>
        /// Gets status text for building
        /// </summary>
        /// <returns>Current status of building</returns>
        public string GetStatusText() {
            if(Animals.Count == 0) {
                return "You have no animals";
            }
            if(HappinessPercentage >= 76) {
                return "Your animals seem happy";
            }
            if(HappinessPercentage >= 31) {
                return "Your pets need some attention";
            }
            return "Help us!";
        }
    }
}