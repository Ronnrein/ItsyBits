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
        public string Name { get; set; }

        /// <summary>
        /// Description of upgrade
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The multiplier for how much longer between each feeding
        /// </summary>
        public float FeedModifier { get; set; }

        /// <summary>
        /// The multiplier for how much longer between each sleep
        /// </summary>
        public float SleepModifier { get; set; }

        /// <summary>
        /// The multiplier for how much longer between each pet
        /// </summary>
        public float PetModifier { get; set; }

        /// <summary>
        /// How much extra capacity this upgrade brings
        /// </summary>
        public int CapacityModifier { get; set; }

        /// <summary>
        /// The static method to call under namespace ItsyBits.Helpers.UpgradeMethods
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Whether or not the upgrade works for buildings
        /// </summary>
        public bool ForBuilding { get; set; }

        /// <summary>
        /// Whether or not the upgrade works for animals
        /// </summary>
        public bool ForAnimal { get; set; }

        /// <summary>
        /// Whether the upgrade is stackable
        /// </summary>
        public bool IsStackable { get; set; }

        /// <summary>
        /// The path to the sprite for upgrade
        /// </summary>
        public string SpritePath { get; set; }

        /// <summary>
        /// Price of upgrade
        /// </summary>
        public int Price { get; set; }

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