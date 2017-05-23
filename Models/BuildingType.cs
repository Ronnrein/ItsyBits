using System.ComponentModel.DataAnnotations;

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
        public string Name { get; set; }

        /// <summary>
        /// Description of building type
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Path to spriteset of building
        /// </summary>
        public string SpritePath { get; set; }

        /// <summary>
        /// The amount of animals this building can hold
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// The max amount with upgrades this building can hold
        /// </summary>
        public int MaxCapacity { get; set; }

        /// <summary>
        /// Price of building
        /// </summary>
        public int Price { get; set; }
    }
}