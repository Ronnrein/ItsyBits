using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ItsyBits.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ItsyBits.Data {
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {

        public virtual DbSet<Animal> Animals { get; set; }
        public virtual DbSet<AnimalType> AnimalTypes { get; set; }
        public virtual DbSet<AnimalUpgrade> AnimalUpgrades { get; set; }
        public virtual DbSet<Building> Buildings { get; set; }
        public virtual DbSet<BuildingType> BuildingTypes { get; set; }
        public virtual DbSet<BuildingUpgrade> BuildingUpgrades { get; set; }
        public virtual DbSet<Upgrade> Upgrades { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options) {}
    }
}
