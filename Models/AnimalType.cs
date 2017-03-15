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
        /// Time before animal needs food
        /// </summary>
        [Display(Name = "Feed time", Description = "Time before animal needs food")]
        public TimeSpan FeedTime { get; set; }

        /// <summary>
        /// Time before animal needs to sleep
        /// </summary>
        [Display(Name = "Sleep time", Description = "Time before animal needs to sleep")]
        public TimeSpan SleepTime { get; set; }

        /// <summary>
        /// Time before animal needs to be pet
        /// </summary>
        [Display(Name = "Pet time", Description = "Time before animal needs to be pet")]
        public TimeSpan PetTime { get; set; }

        /// <summary>
        /// How muuch stronger animal gets each level
        /// </summary>
        [Required(ErrorMessage = "Please set multiplier")]
        [Range(1.0, 10.0, ErrorMessage = "Multiplier must be between 1 and 10")]
        [Display(Name = "Level multiplier", Description = "How muuch stronger animal gets each level")]
        public float LevelMultiplier { get; set; }

        /// <summary>
        /// Path to spriteset of animal
        /// </summary>
        [Required(ErrorMessage = "Please enter path of sprite")]
        [Display(Name = "Sprite path", Description = "Path to spriteset")]
        public string SpritePath { get; set; }
    }
}