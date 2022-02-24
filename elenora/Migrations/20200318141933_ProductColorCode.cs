using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class ProductColorCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColorCode",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryImage",
                table: "Products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorCode",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SecondaryImage",
                table: "Products");
        }
    }
}
