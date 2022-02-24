using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class RemoveComponentDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Components");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Components",
                type: "text",
                nullable: true);
        }
    }
}
