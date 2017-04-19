using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ItsyBits.Data.Migrations
{
    public partial class UpdatedUpgrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CapacityModifier",
                table: "Upgrades",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ForAnimal",
                table: "Upgrades",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ForBuilding",
                table: "Upgrades",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SpritePath",
                table: "Upgrades",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CapacityModifier",
                table: "Upgrades");

            migrationBuilder.DropColumn(
                name: "ForAnimal",
                table: "Upgrades");

            migrationBuilder.DropColumn(
                name: "ForBuilding",
                table: "Upgrades");

            migrationBuilder.DropColumn(
                name: "SpritePath",
                table: "Upgrades");
        }
    }
}
