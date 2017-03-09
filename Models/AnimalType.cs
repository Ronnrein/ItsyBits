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
        [Required(ErrorMessage = "Please enter animal name")]
        public string Name { get; set; }

        /// <summary>
        /// Time between each feeding
        /// </summary>
        [DisplayName("Time between feedings")]
        public TimeSpan FeedTime { get; set; }

        /// <summary>
        /// Time between each sleep
        /// </summary>
        [DisplayName("Time between sleep")]
        public TimeSpan SleepTime { get; set; }

        /// <summary>
        /// Time between each pet
        /// </summary>
        [DisplayName("Time between pettings")]
        public TimeSpan PetTime { get; set; }

        /// <summary>
        /// How much the level of animal is multiplied by
        /// </summary>
        [Required(ErrorMessage = "Please set multiplier")]
        [Range(1, 10, ErrorMessage = "Multiplier must be between 1 and 10")]
        public float LevelMultiplier { get; set; }

        /// <summary>
        /// Path to spriteset of animal
        /// </summary>
        [Required(ErrorMessage = "Please enter path of sprite")]
        public string SpritePath { get; set; }
    }
}