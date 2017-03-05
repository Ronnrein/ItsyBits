using System.Collections.Generic;
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
    }
    
}
