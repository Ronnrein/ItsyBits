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
        public string Name { get; set; }

        /// <summary>
        /// Date the animal was created
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Last time animal was fed
        /// </summary>
        public DateTime LastFeed { get; set; }

        /// <summary>
        /// Last time animal slept
        /// </summary>
        public DateTime LastSleep { get; set; }

        /// <summary>
        /// Last time animal was pet
        /// </summary>
        public DateTime LastPet { get; set; }

        /// <summary>
        /// Level of animal
        /// </summary>
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
        public DateTime? DeathTime { get; set; }

        /// <summary>
        /// Id of type of animal
        /// </summary>
        [ForeignKey("Type")]
        public int TypeId { get; set; }

        /// <summary>
        /// Building animal lives in
        /// </summary>
        public Building Building { get; set; }

        /// <summary>
        /// Id of building of animal
        /// </summary>
        [ForeignKey("Building")]
        public int BuildingId { get; set; }

        /// <summary>
        /// Junction for animal upgrades
        /// </summary>
        public ICollection<AnimalUpgrade> AnimalUpgrades { get; set; }

        /// <summary>
        /// Upgrades for animal
        /// </summary>
        [NotMapped]
        public IEnumerable<Upgrade> Upgrades => AnimalUpgrades?.Select(au => au.Upgrade) ?? Enumerable.Empty<Upgrade>();

        /// <summary>
        /// Whether the animal is alive or not
        /// </summary>
        public bool IsAlive => FeedPercentage > 20 && SleepPercentage > 20;

        /// <summary>
        /// Current reward amount of this pet
        /// </summary>
        public int Reward => IsAlive ? (int)(20f * (PetPercentage / 100f)) : 0;

        /// <summary>
        /// The percentage of how well fed the animal is
        /// </summary>
        [NotMapped]
        public int FeedPercentage {
            get { return Type == null ? 0 : CalculateStatPercentage(LastFeed, Type.FeedTime, FeedUpgradeFactor); }
            set { LastFeed = DateTime.Now - TimeSpan.FromTicks((long)(Type.FeedTime.Ticks * FeedUpgradeFactor * (100 - value) / 100)); }
        }

        /// <summary>
        /// The percentage of how rested the animal is
        /// </summary>
        [NotMapped]
        public int SleepPercentage {
            get { return Type == null ? 0 : CalculateStatPercentage(LastSleep, Type.SleepTime, SleepUpgradeFactor); }
            set { LastSleep = DateTime.Now - TimeSpan.FromTicks((long)(Type.FeedTime.Ticks * SleepUpgradeFactor * (100 - value) / 100)); }
            }

        /// <summary>
        /// The percentage of how loved the animal is
        /// </summary>
        [NotMapped]
        public int PetPercentage {
            get { return Type == null ? 0 : CalculateStatPercentage(LastPet, Type.PetTime, PetUpgradeFactor); }
            set { LastPet = DateTime.Now - TimeSpan.FromTicks((long)(Type.FeedTime.Ticks * PetUpgradeFactor * (100 - value) / 100)); }
        }

        /// <summary>
        /// The percentage avarage of all stats
        /// </summary>
        [NotMapped]
        public int HappinessPercentage {
            get { return (int) new[] {FeedPercentage, SleepPercentage, PetPercentage}.Average(); }
            set {
                DeathTime = null;
                FeedPercentage = value;
                SleepPercentage = value;
                PetPercentage = value;
            }
        }

        /// <summary>
        /// Status text for animal
        /// </summary>
        public string StatusText {
            get {
                if (DeathTime != null) {
                    return "I ran away!";
                }
                if (new[] { FeedPercentage, SleepPercentage, PetPercentage }.All(s => s >= 80)) {
                    return "I'm happy ^_^";
                }
                if (HappinessPercentage >= 39) {
                    return "I could need some attention";
                }
                if (HappinessPercentage > 20) {
                    return "I don't feel so good";
                }
                return "I feel sick, don't you love me anymore?";
            }
        }

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
        /// The upgrade factor for food
        /// </summary>
        private float FeedUpgradeFactor => Upgrades.Select(u => u.FeedModifier).Aggregate(1f, (x, y) => x - 1 + y) + Building?.Upgrades.Select(u => u.FeedModifier).Aggregate(1f, (x, y) => x - 1 + y) ?? 0;

        /// <summary>
        /// The upgrade factor for food
        /// </summary>
        private float SleepUpgradeFactor => Upgrades.Select(u => u.SleepModifier).Aggregate(1f, (x, y) => x - 1 + y) + Building?.Upgrades.Select(u => u.SleepModifier).Aggregate(1f, (x, y) => x - 1 + y) ?? 0;

        /// <summary>
        /// The upgrade factor for food
        /// </summary>
        private float PetUpgradeFactor => Upgrades.Select(u => u.PetModifier).Aggregate(1f, (x, y) => x - 1 + y) + Building?.Upgrades.Select(u => u.PetModifier).Aggregate(1f, (x, y) => x - 1 + y) ?? 0;

        /// <summary>
        /// Calculate the percentage of the stat of the animal
        /// </summary>
        /// <param name="lastTime">Last time the action was taken</param>
        /// <param name="duration">The duration before action must be taken again</param>
        /// <param name="factor">The factor to multiply the duration with</param>
        /// <returns>Percentage of stat</returns>
        private static int CalculateStatPercentage(DateTime lastTime, TimeSpan duration, float factor = 1) {
            duration = TimeSpan.FromTicks((long) (duration.Ticks * factor));
            return (100 - (int) ((DateTime.Now - lastTime).TotalSeconds / duration.TotalSeconds * 100)).Clamp(0, 100);
        }

    }
}