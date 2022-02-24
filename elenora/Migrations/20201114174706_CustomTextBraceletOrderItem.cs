using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class CustomTextBraceletOrderItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Products_CustomTextOrderItem_ProductId",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "CustomTextOrderItem_ProductId",
                table: "OrderItems",
                newName: "CustomTextBraceletOrderItem_ProductId");

            migrationBuilder.RenameColumn(
                name: "CustomTextOrderItem_BraceletSize",
                table: "OrderItems",
                newName: "CustomTextBraceletOrderItem_BraceletSize");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_CustomTextOrderItem_ProductId",
                table: "OrderItems",
                newName: "IX_OrderItems_CustomTextBraceletOrderItem_ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Products_CustomTextBraceletOrderItem_ProductId",
                table: "OrderItems",
                column: "CustomTextBraceletOrderItem_ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Products_CustomTextBraceletOrderItem_ProductId",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "CustomTextBraceletOrderItem_ProductId",
                table: "OrderItems",
                newName: "CustomTextOrderItem_ProductId");

            migrationBuilder.RenameColumn(
                name: "CustomTextBraceletOrderItem_BraceletSize",
                table: "OrderItems",
                newName: "CustomTextOrderItem_BraceletSize");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_CustomTextBraceletOrderItem_ProductId",
                table: "OrderItems",
                newName: "IX_OrderItems_CustomTextOrderItem_ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Products_CustomTextOrderItem_ProductId",
                table: "OrderItems",
                column: "CustomTextOrderItem_ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
