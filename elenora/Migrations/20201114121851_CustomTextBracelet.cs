using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class CustomTextBracelet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomTextOrderItem_BraceletSize",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomText",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomTextOrderItem_ProductId",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomTextBraceletCartItem_BraceletSize",
                table: "CartItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomText",
                table: "CartItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomTextBraceletCartItem_ProductId",
                table: "CartItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_CustomTextOrderItem_ProductId",
                table: "OrderItems",
                column: "CustomTextOrderItem_ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CustomTextBraceletCartItem_ProductId",
                table: "CartItems",
                column: "CustomTextBraceletCartItem_ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Products_CustomTextBraceletCartItem_ProductId",
                table: "CartItems",
                column: "CustomTextBraceletCartItem_ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Products_CustomTextOrderItem_ProductId",
                table: "OrderItems",
                column: "CustomTextOrderItem_ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Products_CustomTextBraceletCartItem_ProductId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Products_CustomTextOrderItem_ProductId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_CustomTextOrderItem_ProductId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_CustomTextBraceletCartItem_ProductId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "CustomTextOrderItem_BraceletSize",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CustomText",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CustomTextOrderItem_ProductId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CustomTextBraceletCartItem_BraceletSize",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "CustomText",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "CustomTextBraceletCartItem_ProductId",
                table: "CartItems");
        }
    }
}
