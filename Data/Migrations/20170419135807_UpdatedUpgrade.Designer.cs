using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ItsyBits.Data;

namespace ItsyBits.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20170419135807_UpdatedUpgrade")]
    partial class UpdatedUpgrade
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("ItsyBits.Models.Animal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BuildingId");

                    b.Property<DateTime>("Created");

                    b.Property<DateTime>("LastFeed");

                    b.Property<DateTime>("LastPet");

                    b.Property<DateTime>("LastSleep");

                    b.Property<int>("Level");

                    b.Property<bool>("Male");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("BuildingId");

                    b.HasIndex("TypeId");

                    b.ToTable("Animals");
                });

            modelBuilder.Entity("ItsyBits.Models.AnimalType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<TimeSpan>("FeedTime");

                    b.Property<float>("LevelMultiplier");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<TimeSpan>("PetTime");

                    b.Property<TimeSpan>("SleepTime");

                    b.Property<string>("SpritePath")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("AnimalTypes");
                });

            modelBuilder.Entity("ItsyBits.Models.AnimalUpgrade", b =>
                {
                    b.Property<int>("AnimalId");

                    b.Property<int>("UpgradeId");

                    b.HasKey("AnimalId", "UpgradeId");

                    b.HasIndex("UpgradeId");

                    b.ToTable("AnimalUpgrades");
                });

            modelBuilder.Entity("ItsyBits.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<int>("Currency");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("ItsyBits.Models.Building", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("TypeId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("TypeId");

                    b.HasIndex("UserId");

                    b.ToTable("Buildings");
                });

            modelBuilder.Entity("ItsyBits.Models.BuildingType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Capacity");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("SpritePath")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("BuildingTypes");
                });

            modelBuilder.Entity("ItsyBits.Models.BuildingUpgrade", b =>
                {
                    b.Property<int>("BuildingId");

                    b.Property<int>("UpgradeId");

                    b.HasKey("BuildingId", "UpgradeId");

                    b.HasIndex("UpgradeId");

                    b.ToTable("BuildingUpgrades");
                });

            modelBuilder.Entity("ItsyBits.Models.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("Image");

                    b.Property<string>("Link");

                    b.Property<string>("Message");

                    b.Property<bool>("Read");

                    b.Property<string>("Title");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("ItsyBits.Models.Upgrade", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AnimalId");

                    b.Property<int?>("BuildingId");

                    b.Property<int>("CapacityModifier");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<float>("FeedModifier");

                    b.Property<bool>("ForAnimal");

                    b.Property<bool>("ForBuilding");

                    b.Property<string>("Method");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<float>("PetModifier");

                    b.Property<float>("SleepModifier");

                    b.Property<string>("SpritePath");

                    b.HasKey("Id");

                    b.HasIndex("AnimalId");

                    b.HasIndex("BuildingId");

                    b.ToTable("Upgrades");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("ItsyBits.Models.Animal", b =>
                {
                    b.HasOne("ItsyBits.Models.Building", "Building")
                        .WithMany("Animals")
                        .HasForeignKey("BuildingId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ItsyBits.Models.AnimalType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ItsyBits.Models.AnimalUpgrade", b =>
                {
                    b.HasOne("ItsyBits.Models.Animal", "Animal")
                        .WithMany("AnimalUpgrades")
                        .HasForeignKey("AnimalId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ItsyBits.Models.Upgrade", "Upgrade")
                        .WithMany()
                        .HasForeignKey("UpgradeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ItsyBits.Models.Building", b =>
                {
                    b.HasOne("ItsyBits.Models.BuildingType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ItsyBits.Models.ApplicationUser", "User")
                        .WithMany("Buildings")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("ItsyBits.Models.BuildingUpgrade", b =>
                {
                    b.HasOne("ItsyBits.Models.Building", "Building")
                        .WithMany("BuildingUpgrades")
                        .HasForeignKey("BuildingId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ItsyBits.Models.Upgrade", "Upgrade")
                        .WithMany()
                        .HasForeignKey("UpgradeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ItsyBits.Models.Notification", b =>
                {
                    b.HasOne("ItsyBits.Models.ApplicationUser", "User")
                        .WithMany("Notifications")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("ItsyBits.Models.Upgrade", b =>
                {
                    b.HasOne("ItsyBits.Models.Animal")
                        .WithMany("Upgrades")
                        .HasForeignKey("AnimalId");

                    b.HasOne("ItsyBits.Models.Building")
                        .WithMany("Upgrades")
                        .HasForeignKey("BuildingId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("ItsyBits.Models.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("ItsyBits.Models.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ItsyBits.Models.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
