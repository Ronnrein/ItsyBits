using System;
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
        /// Gets age of animal in formatted string
        /// </summary>
        /// <returns>Formatted string of animal age</returns>
        public string GetAge() {
            TimeSpan span = DateTime.Now - Created;
            if(span.TotalDays >= 365) {
                return ((int)Math.Floor(span.TotalDays/365)) + " years";
            }
            if(span.TotalDays >= 31) {
                return ((int)Math.Floor(span.TotalDays/31)) + " months";
            }
            if(span.TotalDays >= 7) {
                return ((int)Math.Floor(span.TotalDays/7)) + " weeks";
            }
            if(span.TotalDays >= 1) {
                return span.Days + " days";
            }
            if(span.TotalHours >= 1) {
                return span.Hours + " hours";
            }
            if(span.TotalMinutes >= 1) {
                return span.Minutes + " minutes";
            }
            return span.Seconds + " seconds";
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