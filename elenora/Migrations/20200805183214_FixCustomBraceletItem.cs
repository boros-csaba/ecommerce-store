using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class FixCustomBraceletItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomBraceletCartItem_BraceletSize2",
                table: "CartItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomBraceletCartItem_BraceletSize2",
                table: "CartItems",
                type: "integer",
                nullable: true);
        }
    }
}
