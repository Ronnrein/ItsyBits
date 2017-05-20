﻿using System.ComponentModel.DataAnnotations;

namespace ItsyBits.Models {

    /// <summary>
    /// The definition of building types
    /// </summary>
    public class BuildingType {

        /// <summary>
        /// Id of building type
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of building type
        /// </summary>
        [Required(ErrorMessage = "Please enter a name of the building type")]
        public string Name { get; set; }

        /// <summary>
        /// Description of building type
        /// </summary>
        [Required(ErrorMessage = "Please enter building description")]
        public string Description { get; set; }

        /// <summary>
        /// Path to spriteset of building
        /// </summary>
        [Required(ErrorMessage = "Please enter path to spriteset")]
        [Display(Name = "Sprite path", Description = "Path to spriteset of building")]
        public string SpritePath { get; set; }

        /// <summary>
        /// The amount of animals this building can hold
        /// </summary>
        [Required(ErrorMessage = "Please enter amount of animals this building can hold")]
        [Range(1, 100, ErrorMessage = "Amount must be between 1 and 100")]
        [Display(Description = "The amount of animals this building can hold")]
        public int Capacity { get; set; }

        /// <summary>
        /// The max amount with upgrades this building can hold
        /// </summary>
        [Required(ErrorMessage = "Please enter max amount of animals this building can hold")]
        [Range(1, 100, ErrorMessage = "Amount must be between 1 and 100")]
        [Display(Name = "Max capacity", Description = "The max amount with upgrades this building can hold")]
        public int MaxCapacity { get; set; }

        /// <summary>
        /// Price of building
        /// </summary>
        [Required(ErrorMessage = "Please enter price of building")]
        [Display(Description = "Price of building")]
        public int Price { get; set; }
    }
}