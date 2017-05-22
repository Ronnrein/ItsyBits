using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ItsyBits.Models {

    /// <summary>
    /// Represents a plot of land to place a building on
    /// </summary>
    public class Plot {
        
        /// <summary>
        /// Id of plot
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Description of plot
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// X-position of plot on the canvas
        /// </summary>
        public int PositionX { get; set; }

        /// <summary>
        /// Y-position of plot on the canvas
        /// </summary>
        public int PositionY { get; set; }

    }
}