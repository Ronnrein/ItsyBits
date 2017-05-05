﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ItsyBits.Helpers;

namespace ItsyBits.Models {

    /// <summary>
    /// Temagotchi animal
    /// </summary>
    public class Animal {

        /// <summary>
        /// Id of animal
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of animal
        /// </summary>
        [Required(ErrorMessage = "Please enter its name")]
        public string Name { get; set; }

        /// <summary>
        /// Date the animal was created
        /// </summary>
        [ScaffoldColumn(false)]
        public DateTime Created { get; set; }

        /// <summary>
        /// Last time animal was fed
        /// </summary>
        [ScaffoldColumn(false)]
        public DateTime LastFeed { get; set; }

        /// <summary>
        /// Last time animal slept
        /// </summary>
        [ScaffoldColumn(false)]
        public DateTime LastSleep { get; set; }

        /// <summary>
        /// Last time animal was pet
        /// </summary>
        [ScaffoldColumn(false)]
        public DateTime LastPet { get; set; }

        /// <summary>
        /// Level of animal
        /// </summary>
        [ScaffoldColumn(false)]
        public int Level { get; set; }

        /// <summary>
        /// Whether animal is male or female
        /// </summary>
        public bool Male { get; set; }

        /// <summary>
        /// Type of animal
        /// </summary>
        public AnimalType Type { get; set; }

        /// <summary>
        /// Time of death
        /// </summary>
        [ScaffoldColumn(false)]
        public DateTime? DeathTime { get; set; }

        /// <summary>
        /// Id of type of animal
        /// </summary>
        [ForeignKey("Type")]
        [DisplayName("Type")]
        public int TypeId { get; set; }

        /// <summary>
        /// Building animal lives in
        /// </summary>
        [ScaffoldColumn(false)]
        public Building Building { get; set; }

        /// <summary>
        /// Id of building of animal
        /// </summary>
        [ForeignKey("Building")]
        [DisplayName("Building")]
        public int BuildingId { get; set; }

        /// <summary>
        /// Junction for animal upgrades
        /// </summary>
        [ScaffoldColumn(false)]
        public ICollection<AnimalUpgrade> AnimalUpgrades { get; set; }

        /// <summary>
        /// Upgrades for animal
        /// </summary>
        [NotMapped]
        public IEnumerable<Upgrade> Upgrades => AnimalUpgrades?.Select(au => au.Upgrade) ?? Enumerable.Empty<Upgrade>();

        /// <summary>
        /// The percentage of how well fed the animal is
        /// </summary>
        public int FeedPercentage => Type == null ? 0 : CalculateStatPercentage(LastFeed, Type.FeedTime);

        /// <summary>
        /// The percentage of how rested the animal is
        /// </summary>
        public int SleepPercentage => Type == null ? 0 : CalculateStatPercentage(LastSleep, Type.SleepTime);

        /// <summary>
        /// The percentage of how loved the animal is
        /// </summary>
        public int PetPercentage => Type == null ? 0 : CalculateStatPercentage(LastPet, Type.PetTime);

        /// <summary>
        /// The percentage avarage of all stats
        /// </summary>
        public int HappinessPercentage => (int) new[] {FeedPercentage, SleepPercentage, PetPercentage}.Average();

        /// <summary>
        /// Whether the animal is alive or not
        /// </summary>
        public bool IsAlive => Type != null && (FeedPercentage > 20 && SleepPercentage > 20);

        /// <summary>
        /// Readable age of animal
        /// </summary>
        public string Age => Created.ReadableAge();

        /// <summary>
        /// Constructor of animal
        /// </summary>
        public Animal() {
            LastFeed = DateTime.Now;
            LastPet = DateTime.Now;
            LastSleep = DateTime.Now;
            Created = DateTime.Now;
            Level = 1;
        }

        /// <summary>
        /// Gets status text for animal
        /// </summary>
        /// <returns>Current status of animal</returns>
        public string GetStatusText() {
            if (DeathTime != null) {
                return "I ran away!";
            }
            if(new[] {FeedPercentage, SleepPercentage, PetPercentage}.All(s => s >= 80)) {
                return "I'm happy ^_^";
            }
            if(HappinessPercentage >= 39) {
                return "I could need some attention";
            }
            if(HappinessPercentage > 20) {
                return "I don't feel so good";
            }
            return "I feel sick, don't you love me anymore?";
        }

        /// <summary>
        /// Calculate the percentage of the stat of the animal
        /// </summary>
        /// <param name="lastTime">Last time the action was taken</param>
        /// <param name="duration">The duration before action must be taken again</param>
        /// <returns>Percentage of stat</returns>
        private static int CalculateStatPercentage(DateTime lastTime, TimeSpan duration) {
            return (100 - (int) ((DateTime.Now - lastTime).TotalSeconds / duration.TotalSeconds * 100)).Clamp(0, 100);
        }
    }
}