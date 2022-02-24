using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class x79 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Coupons");

            migrationBuilder.AddColumn<int>(
                name: "UsageCount",
                table: "Coupons",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsageCount",
                table: "Coupons");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Coupons",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
