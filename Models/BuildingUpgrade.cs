using System.ComponentModel.DataAnnotations.Schema;

namespace ItsyBits.Models {

    /// <summary>
    /// Junction model for building upgrades
    /// </summary>
    public class BuildingUpgrade {

        /// <summary>
        /// Id of building
        /// </summary>
        [ForeignKey("Building")]
        public int BuildingId { get; set; }

        /// <summary>
        /// Building of uprade
        /// </summary>
        public Building Building { get; set; }

        /// <summary>
        /// Id of upgrade
        /// </summary>
        [ForeignKey("Upgrade")]
        public int UpgradeId { get; set; }

        /// <summary>
        /// Upgrade of building
        /// </summary>
        public Upgrade Upgrade { get; set; }
    }
}