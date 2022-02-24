using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class SecondaryBead : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SecondaryBeadTypeId",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StyleType",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SecondaryBeadTypeId",
                table: "CartItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StyleType",
                table: "CartItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_SecondaryBeadTypeId",
                table: "OrderItems",
                column: "SecondaryBeadTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_SecondaryBeadTypeId",
                table: "CartItems",
                column: "SecondaryBeadTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Components_SecondaryBeadTypeId",
                table: "CartItems",
                column: "SecondaryBeadTypeId",
                principalTable: "Components",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Components_SecondaryBeadTypeId",
                table: "OrderItems",
                column: "SecondaryBeadTypeId",
                principalTable: "Components",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Components_SecondaryBeadTypeId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Components_SecondaryBeadTypeId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_SecondaryBeadTypeId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_SecondaryBeadTypeId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "SecondaryBeadTypeId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "StyleType",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "SecondaryBeadTypeId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "StyleType",
                table: "CartItems");
        }
    }
}
