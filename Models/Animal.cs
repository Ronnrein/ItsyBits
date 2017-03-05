using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ItsyBits.Models {

    /// <summary>
    /// Temagotchi animal
    /// </summary>
    public class Animal {

        /// <summary>
        /// Id of animal
        /// </summary>
        [Key]
        [ScaffoldColumn(false)]
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
        /// Building animal lives in
        /// </summary>
        [ScaffoldColumn(false)]
        public Building Building { get; set; }

        /// <summary>
        /// Junction for animal upgrades
        /// </summary>
        [ScaffoldColumn(false)]
        public ICollection<AnimalUpgrade> AnimalUpgrades { get; set; }

        /// <summary>
        /// Upgrades of animal
        /// </summary>
        [NotMapped]
        public ICollection<Upgrade> Upgrades {
            get { return (ICollection<Upgrade>) AnimalUpgrades.Select(au => au.Upgrade); }
        }

        /// <summary>
        /// Constructor of animal
        /// </summary>
        public Animal() {
            LastFeed = DateTime.Now;
            LastPet = DateTime.Now;
            LastSleep = DateTime.Now;
            Level = 1;
        }
    }
}