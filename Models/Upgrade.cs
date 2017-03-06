using System.ComponentModel.DataAnnotations;

namespace ItsyBits.Models {

    /// <summary>
    /// Upgrade for animals and buildings
    /// </summary>
    public class Upgrade {

        /// <summary>
        /// Id of upgrade
        /// </summary>
        [Key]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        /// <summary>
        /// Name of upgrade
        /// </summary>
        [Required(ErrorMessage = "Please enter name of upgrade")]
        public string Name { get; set; }

        /// <summary>
        /// Description of upgrade
        /// </summary>
        [Required(ErrorMessage = "Please enter upgrade description")]
        public string Description { get; set; }

        /// <summary>
        /// Modifier affecting the time between feedings
        /// </summary>
        [Required(ErrorMessage = "Please enter feed modifier")]
        public float FeedModifier { get; set; }

        /// <summary>
        /// Modifier affecting the time between sleep
        /// </summary>
        [Required(ErrorMessage = "Please enter sleep modifier")]
        public float SleepModifier { get; set; }

        /// <summary>
        /// Modifier affecting the time between each pet
        /// </summary>
        [Required(ErrorMessage = "Please enter pet modifier")]
        public float PetModifier { get; set; }

        /// <summary>
        /// Static method to call under namespace ItsyBits.Helpers.UpgradeMethods
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Constructor of upgrade
        /// </summary>
        public Upgrade() {
            FeedModifier = 1;
            SleepModifier = 1;
            PetModifier = 1;
        }
    }
}