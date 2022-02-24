using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class ComponentImageQuality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImagesQuality",
                table: "Components",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "Components",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagesQuality",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "Components");
        }
    }
}
