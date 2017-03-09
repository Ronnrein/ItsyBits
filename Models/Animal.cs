using System;
using System.Collections.Generic;
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
        /// Type of animal
        /// </summary>
        public AnimalType Type { get; set; }

        /// <summary>
        /// Id of type of animal
        /// </summary>
        [ForeignKey("Type")]
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
        public int BuildingId { get; set; }

        /// <summary>
        /// Junction for animal upgrades
        /// </summary>
        [ScaffoldColumn(false)]
        public ICollection<AnimalUpgrade> AnimalUpgrades { get; set; }

        /// <summary>
        /// The percentage of how well fed the animal is
        /// </summary>
        public int FeedPercentage => CalculateStatPercentage(LastFeed, Type.FeedTime);

        /// <summary>
        /// The percentage of how rested the animal is
        /// </summary>
        public int SleepPercentage => CalculateStatPercentage(LastSleep, Type.SleepTime);

        /// <summary>
        /// The percentage of how loved the animal is
        /// </summary>
        public int PetPercentage => CalculateStatPercentage(LastPet, Type.PetTime);

        /// <summary>
        /// The percentage avarage of all stats
        /// </summary>
        public int HappinessPercentage => (int) new[] {FeedPercentage, SleepPercentage, PetPercentage}.Average();

        /// <summary>
        /// Upgrades of animal
        /// </summary>
        [NotMapped]
        public IEnumerable<Upgrade> Upgrades => AnimalUpgrades.Select(au => au.Upgrade);

        /// <summary>
        /// Constructor of animal
        /// </summary>
        public Animal() {
            LastFeed = DateTime.Now;
            LastPet = DateTime.Now;
            LastSleep = DateTime.Now;
            Level = 1;
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