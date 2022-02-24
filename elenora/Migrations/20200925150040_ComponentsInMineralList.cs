using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class ComponentsInMineralList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowInMineralsList",
                table: "Components",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ArticlesDescription",
                table: "ComponentFamilies",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowInMineralsList",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "ArticlesDescription",
                table: "ComponentFamilies");
        }
    }
}
