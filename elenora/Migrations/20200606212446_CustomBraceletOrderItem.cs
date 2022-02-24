using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace elenora.Migrations
{
    public partial class CustomBraceletOrderItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_ProductSets_ProductSetId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomBraceletComponents_CartItems_CartItemId",
                table: "CustomBraceletComponents");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_ProductSets_ProductSetId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductSets_ProductSetId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "ProductSets");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProductSetId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ProductSetId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_ProductSetId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "ProductSetId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductSetId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ProductSetId",
                table: "CartItems");

            migrationBuilder.AddColumn<int>(
                name: "BeadTypeId",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomBraceletOrderItem_BraceletSize",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "OrderItems",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "CartItemId",
                table: "CustomBraceletComponents",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "OrderItemId",
                table: "CustomBraceletComponents",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_BeadTypeId",
                table: "OrderItems",
                column: "BeadTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomBraceletComponents_OrderItemId",
                table: "CustomBraceletComponents",
                column: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomBraceletComponents_CartItems_CartItemId",
                table: "CustomBraceletComponents",
                column: "CartItemId",
                principalTable: "CartItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomBraceletComponents_OrderItems_OrderItemId",
                table: "CustomBraceletComponents",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Components_BeadTypeId",
                table: "OrderItems",
                column: "BeadTypeId",
                principalTable: "Components",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomBraceletComponents_CartItems_CartItemId",
                table: "CustomBraceletComponents");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomBraceletComponents_OrderItems_OrderItemId",
                table: "CustomBraceletComponents");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Components_BeadTypeId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_BeadTypeId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_CustomBraceletComponents_OrderItemId",
                table: "CustomBraceletComponents");

            migrationBuilder.DropColumn(
                name: "BeadTypeId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CustomBraceletOrderItem_BraceletSize",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "OrderItemId",
                table: "CustomBraceletComponents");

            migrationBuilder.AddColumn<int>(
                name: "ProductSetId",
                table: "Products",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductSetId",
                table: "OrderItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CartItemId",
                table: "CustomBraceletComponents",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductSetId",
                table: "CartItems",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductSetId",
                table: "Products",
                column: "ProductSetId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductSetId",
                table: "OrderItems",
                column: "ProductSetId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductSetId",
                table: "CartItems",
                column: "ProductSetId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_ProductSets_ProductSetId",
                table: "CartItems",
                column: "ProductSetId",
                principalTable: "ProductSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomBraceletComponents_CartItems_CartItemId",
                table: "CustomBraceletComponents",
                column: "CartItemId",
                principalTable: "CartItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ProductSets_ProductSetId",
                table: "OrderItems",
                column: "ProductSetId",
                principalTable: "ProductSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductSets_ProductSetId",
                table: "Products",
                column: "ProductSetId",
                principalTable: "ProductSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
