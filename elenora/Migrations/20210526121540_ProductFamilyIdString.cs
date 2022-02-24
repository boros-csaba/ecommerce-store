using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class ProductFamilyIdString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowInMineralsList",
                table: "Components");

            migrationBuilder.AddColumn<string>(
                name: "IdString",
                table: "ComponentFamilies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "ComponentFamilies",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdString",
                table: "ComponentFamilies");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "ComponentFamilies");

            migrationBuilder.AddColumn<bool>(
                name: "ShowInMineralsList",
                table: "Components",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
