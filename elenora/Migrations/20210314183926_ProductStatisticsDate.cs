using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class ProductStatisticsDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Week",
                table: "ProductStatistics");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "ProductStatistics",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "ProductStatistics");

            migrationBuilder.AddColumn<int>(
                name: "Week",
                table: "ProductStatistics",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
