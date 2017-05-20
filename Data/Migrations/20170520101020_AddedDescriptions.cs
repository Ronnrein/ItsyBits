using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ItsyBits.Migrations
{
    public partial class AddedDescriptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "BuildingTypes",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AnimalTypes",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "BuildingTypes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AnimalTypes");
        }
    }
}
