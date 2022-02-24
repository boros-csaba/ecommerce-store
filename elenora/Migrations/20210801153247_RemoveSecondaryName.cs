using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class RemoveSecondaryName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecondaryName",
                table: "Products");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecondaryName",
                table: "Products",
                type: "text",
                nullable: true);
        }
    }
}
