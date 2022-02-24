using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class HoroscopeBraceletCartItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HoroscopeBraceletOrderItem_BraceletSize",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HoroscopeBraceletId",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HoroscopeBraceletCartItem_BraceletSize",
                table: "CartItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HoroscopeBraceletId",
                table: "CartItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_HoroscopeBraceletId",
                table: "OrderItems",
                column: "HoroscopeBraceletId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_HoroscopeBraceletId",
                table: "CartItems",
                column: "HoroscopeBraceletId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_HoroscopeBracelets_HoroscopeBraceletId",
                table: "CartItems",
                column: "HoroscopeBraceletId",
                principalTable: "HoroscopeBracelets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_HoroscopeBracelets_HoroscopeBraceletId",
                table: "OrderItems",
                column: "HoroscopeBraceletId",
                principalTable: "HoroscopeBracelets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_HoroscopeBracelets_HoroscopeBraceletId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_HoroscopeBracelets_HoroscopeBraceletId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_HoroscopeBraceletId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_HoroscopeBraceletId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "HoroscopeBraceletOrderItem_BraceletSize",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "HoroscopeBraceletId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "HoroscopeBraceletCartItem_BraceletSize",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "HoroscopeBraceletId",
                table: "CartItems");
        }
    }
}
