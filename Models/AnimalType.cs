using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ItsyBits.Models {

    /// <summary>
    /// The definition of animal types
    /// </summary>
    public class AnimalType {

        /// <summary>
        /// Id of animal type
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of animal type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of animal type
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Time before animal needs food
        /// </summary>
        public TimeSpan FeedTime { get; set; }

        /// <summary>
        /// Time before animal needs to sleep
        /// </summary>
        public TimeSpan SleepTime { get; set; }

        /// <summary>
        /// Time before animal needs to be pet
        /// </summary>
        public TimeSpan PetTime { get; set; }

        /// <summary>
        /// How muuch stronger animal gets each level
        /// </summary>
        public float LevelMultiplier { get; set; }

        /// <summary>
        /// Path to spriteset of animal
        /// </summary>
        public string SpritePath { get; set; }

        /// <summary>
        /// Price of animal
        /// </summary>
        public int Price { get; set; }
    }
}