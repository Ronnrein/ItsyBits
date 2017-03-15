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
        /// The multiplier for how much longer between each feeding
        /// </summary>
        [Required(ErrorMessage = "Please enter feed modifier")]
        [Display(Name = "Food modifier", Description = "The multiplier for how much longer between each feeding")]
        public float FeedModifier { get; set; }

        /// <summary>
        /// The multiplier for how much longer between each sleep
        /// </summary>
        [Required(ErrorMessage = "Please enter sleep modifier")]
        [Display(Name = "Sleep modifier", Description = "The multiplier for how much longer between each sleep")]
        public float SleepModifier { get; set; }

        /// <summary>
        /// The multiplier for how much longer between each pet
        /// </summary>
        [Required(ErrorMessage = "Please enter pet modifier")]
        [Display(Name = "Per modifier", Description = "The multiplier for how much longer between each pet")]
        public float PetModifier { get; set; }

        /// <summary>
        /// The static method to call under namespace ItsyBits.Helpers.UpgradeMethods
        /// </summary>
        [Display(Description = "The static method to call under namespace ItsyBits.Helpers.UpgradeMethods")]
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