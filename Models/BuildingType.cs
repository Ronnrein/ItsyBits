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
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        /// <summary>
        /// Name of building type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Path to spriteset of animal
        /// </summary>
        public string SpritePath { get; set; }

        /// <summary>
        /// Animal capacity of building type
        /// </summary>
        public int Capacity { get; set; }
    }
}