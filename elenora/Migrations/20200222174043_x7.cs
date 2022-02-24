using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class x7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ListOrder",
                table: "Products",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListOrder",
                table: "Products");
        }
    }
}
