using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class CartItemBase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomBraceletCartItem_BraceletSize",
                table: "CartItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomBraceletCartItem_BraceletSize2",
                table: "CartItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomBraceletCartItem_BraceletSize",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "CustomBraceletCartItem_BraceletSize2",
                table: "CartItems");
        }
    }
}
