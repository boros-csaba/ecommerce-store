using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class RemoveColorCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorCode",
                table: "Products");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColorCode",
                table: "Products",
                type: "text",
                nullable: true);
        }
    }
}
