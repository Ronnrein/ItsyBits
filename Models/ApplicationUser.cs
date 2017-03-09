using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ItsyBits.Models {

    /// <summary>
    /// User of application
    /// </summary>
    public class ApplicationUser : IdentityUser {

        /// <summary>
        /// Buildings of user
        /// </summary>
        public ICollection<Building> Buildings { get; set; }

        /// <summary>
        /// Animals of user
        /// </summary>
        [NotMapped]
        public IEnumerable<Animal> Animals => Buildings.SelectMany(b => b.Animals);
    }
    
}
