using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ItsyBits.Data.Migrations
{
    public partial class AddedMaxCapacity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxCapacity",
                table: "BuildingTypes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "BuildingTypes",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxCapacity",
                table: "BuildingTypes");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "BuildingTypes");
        }
    }
}
