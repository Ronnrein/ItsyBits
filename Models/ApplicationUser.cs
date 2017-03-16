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
        /// Get animals of user
        /// </summary>
        /// <returns>Animals of user</returns>
        public IEnumerable<Animal> GetAnimals() {
            return Buildings.SelectMany(b => b.Animals);
        }
    }
    
}
