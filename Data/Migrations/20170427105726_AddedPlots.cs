using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ItsyBits.Migrations
{
    public partial class AddedPlots : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlotId",
                table: "Buildings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Plots",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(nullable: true),
                    PositionX = table.Column<int>(nullable: false),
                    PositionY = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plots", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_PlotId",
                table: "Buildings",
                column: "PlotId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Plots");

            migrationBuilder.DropIndex(
                name: "IX_Buildings_PlotId",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "PlotId",
                table: "Buildings");
        }
    }
}
