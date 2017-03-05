using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ItsyBits.Models;

namespace ItsyBits.Data {

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
        public ApplicationDbContext(DbContextOptions options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);
            builder.Entity<BuildingUpgrade>().HasKey(b => new {b.BuildingId, b.UpgradeId});
            builder.Entity<AnimalUpgrade>().HasKey(a => new {a.AnimalId, a.UpgradeId});
            builder.Entity<BuildingUpgrade>()
                .HasOne(bu => bu.Building)
                .WithMany(b => b.BuildingUpgrades);
            builder.Entity<AnimalUpgrade>()
                .HasOne(au => au.Animal)
                .WithMany(a => a.AnimalUpgrades);
        }
    }
}
