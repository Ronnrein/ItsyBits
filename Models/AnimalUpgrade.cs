using System.ComponentModel.DataAnnotations.Schema;

namespace ItsyBits.Models {

    /// <summary>
    /// Junction model for animal upgrades
    /// </summary>
    public class AnimalUpgrade {

        /// <summary>
        /// Id of animal
        /// </summary>
        [ForeignKey("Animal")]
        public int AnimalId { get; set; }

        /// <summary>
        /// Animal of upgrade
        /// </summary>
        public Animal Animal { get; set; }

        /// <summary>
        /// Id of upgrade
        /// </summary>
        [ForeignKey("Upgrade")]
        public int UpgradeId { get; set; }

        /// <summary>
        /// Upgrade of animal
        /// </summary>
        public Upgrade Upgrade { get; set; }
    }
}