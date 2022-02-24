using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class ComponentExample : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExampleImageDescription",
                table: "Components",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExampleImageUrl",
                table: "Components",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExampleImageDescription",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "ExampleImageUrl",
                table: "Components");
        }
    }
}
