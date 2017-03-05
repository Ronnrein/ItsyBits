using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ItsyBits.Models {

    /// <summary>
    /// Farm buildings containing animals
    /// </summary>
    public class Building {

        /// <summary>
        /// Id of building
        /// </summary>
        [Key]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        /// <summary>
        /// Name of building
        /// </summary>
        [Required(ErrorMessage = "Please enter name of building")]
        public string Name { get; set; }

        /// <summary>
        /// User of building
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// Type of building
        /// </summary>
        public BuildingType Type { get; set; }

        /// <summary>
        /// Animals living in building
        /// </summary>
        public ICollection<Animal> Animals { get; set; }

        /// <summary>
        /// Junction for building upgrades
        /// </summary>
        public ICollection<BuildingUpgrade> BuildingUpgrades { get; set; }

        /// <summary>
        /// Upgrades of building
        /// </summary>
        [NotMapped]
        public ICollection<Upgrade> Upgrades {
            get { return (ICollection<Upgrade>) BuildingUpgrades.Select(bu => bu.Upgrade); }
        }
    }
}