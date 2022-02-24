using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class ComponentFamilyShortDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                table: "ComponentFamilies",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortDescription",
                table: "ComponentFamilies");
        }
    }
}
