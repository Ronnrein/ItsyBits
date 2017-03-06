using System;
using ItsyBits.Data;
using ItsyBits.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ItsyBits.Helpers {

    /// <summary>
    /// Service type to seed database
    /// </summary>
    public class DatabaseSeeder {

        /// <summary>
        /// Database context
        /// </summary>
        private ApplicationDbContext context;

        /// <summary>
        /// Database seeder constructor
        /// </summary>
        /// <param name="context">Database context</param>
        public DatabaseSeeder(ApplicationDbContext context) {
            this.context = context;
        }

        /// <summary>
        /// Seeds database with dummy data
        /// </summary>
        public async void SeedData() {

            // Generate dummy data
            // Roles
            RoleStore<IdentityRole> roleStore = new RoleStore<IdentityRole>(context);
            await roleStore.CreateAsync(new IdentityRole{Name = "Administrator", NormalizedName = "ADMINISTRATOR"});
            await roleStore.CreateAsync(new IdentityRole{Name = "Player", NormalizedName = "PLAYER"});

            // Users
            ApplicationUser admin = new ApplicationUser {
                UserName = "admin@admin.admin",
                Email = "admin@admin.admin",
                NormalizedUserName = "ADMIN@ADMIN.ADMIN",
                NormalizedEmail = "ADMIN@ADMIN.ADMIN",
                SecurityStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
                LockoutEnabled = false
            };
            ApplicationUser player = new ApplicationUser {
                UserName = "player@player.player",
                Email = "player@player.player",
                NormalizedUserName = "PLAYER@PLAYER.PLAYER",
                NormalizedEmail = "PLAYER@PLAYER.PLAYER",
                SecurityStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
                LockoutEnabled = false
            };
            PasswordHasher<ApplicationUser> hasher = new PasswordHasher<ApplicationUser>();
            admin.PasswordHash = hasher.HashPassword(admin, "admin");
            player.PasswordHash = hasher.HashPassword(player, "player");
            UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(context);
            await userStore.CreateAsync(admin);
            await userStore.CreateAsync(player);

            // Assign user roles
            await userStore.AddToRoleAsync(admin, "ADMINISTRATOR");
            await userStore.AddToRoleAsync(player, "PLAYER");

            // Building types
            BuildingType shack = new BuildingType {
                Name = "Shack",
                Capacity = 3,
                SpritePath = "shack"
            };
            BuildingType coop = new BuildingType {
                Name = "Coop",
                Capacity = 5,
                SpritePath = "coop"
            };
            context.BuildingTypes.Add(shack);
            context.BuildingTypes.Add(coop);

            // Buildings
            Building playerShack = new Building {
                Name = "Players shack",
                Type = shack,
                User = player
            };
            Building playerCoop = new Building {
                Name = "Players coop",
                Type = coop,
                User = player
            };
            context.Buildings.Add(playerShack);
            context.Buildings.Add(playerCoop);

            // Animal types
            AnimalType ant = new AnimalType {
                Name = "Ant",
                FeedTime = new TimeSpan(4, 0, 0),
                SleepTime = new TimeSpan(12, 0, 0),
                PetTime = new TimeSpan(8, 0, 0),
                LevelMultiplier = 1f,
                SpritePath = "ant"
            };
            AnimalType bear = new AnimalType {
                Name = "Bear",
                FeedTime = new TimeSpan(8, 0, 0),
                SleepTime = new TimeSpan(24, 0, 0),
                PetTime = new TimeSpan(4, 0, 0),
                LevelMultiplier = 1.1f,
                SpritePath = "bear"
            };
            context.AnimalTypes.Add(ant);
            context.AnimalTypes.Add(bear);

            // Animals
            Animal playerAnt = new Animal {
                Name = "Peter",
                Type = ant,
                Building = playerShack,
            };
            Animal playerBear = new Animal {
                Name = "Winnie",
                Type = bear,
                Building = playerCoop,
            };
            context.Animals.Add(playerAnt);
            context.Animals.Add(playerBear);

            // Upgrades
            Upgrade comfyBlanket = new Upgrade {
                Name = "Comfy blanket",
                Description = "Comfy blanket that decreases the need to be pet",
                PetModifier = 1.5f
            };
            Upgrade heater = new Upgrade {
                Name = "Heater",
                Description = "Heater that makes animals sleep better, decreasing need for sleep",
                SleepModifier = 1.4f
            };
            context.Upgrades.Add(comfyBlanket);
            context.Upgrades.Add(heater);

            // Upgrade junctions
            context.AnimalUpgrades.Add(new AnimalUpgrade {
                Upgrade = comfyBlanket,
                Animal = playerAnt
            });
            context.BuildingUpgrades.Add(new BuildingUpgrade {
                Upgrade = heater,
                Building = playerCoop
            });

            context.SaveChangesAsync();
        }

    }
}