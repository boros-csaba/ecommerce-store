using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace elenora.Migrations
{
    public partial class ComplementaryProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExtraBraceletForBeads");

            migrationBuilder.CreateTable(
                name: "ComplementaryProducts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplementaryProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BeadComplementaryProducts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ComponentId = table.Column<int>(nullable: false),
                    ComplementaryProductId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeadComplementaryProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BeadComplementaryProducts_ComplementaryProducts_Complementa~",
                        column: x => x.ComplementaryProductId,
                        principalTable: "ComplementaryProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BeadComplementaryProducts_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItemComplementaryProducts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CartItemId = table.Column<int>(nullable: false),
                    ComplementaryProductId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItemComplementaryProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItemComplementaryProducts_CartItems_CartItemId",
                        column: x => x.CartItemId,
                        principalTable: "CartItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItemComplementaryProducts_ComplementaryProducts_Complem~",
                        column: x => x.ComplementaryProductId,
                        principalTable: "ComplementaryProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemComplementaryProducts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderItemId = table.Column<int>(nullable: false),
                    ComplementaryProductId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemComplementaryProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemComplementaryProducts_ComplementaryProducts_Comple~",
                        column: x => x.ComplementaryProductId,
                        principalTable: "ComplementaryProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItemComplementaryProducts_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BeadComplementaryProducts_ComplementaryProductId",
                table: "BeadComplementaryProducts",
                column: "ComplementaryProductId");

            migrationBuilder.CreateIndex(
                name: "IX_BeadComplementaryProducts_ComponentId",
                table: "BeadComplementaryProducts",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItemComplementaryProducts_CartItemId",
                table: "CartItemComplementaryProducts",
                column: "CartItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItemComplementaryProducts_ComplementaryProductId",
                table: "CartItemComplementaryProducts",
                column: "ComplementaryProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemComplementaryProducts_ComplementaryProductId",
                table: "OrderItemComplementaryProducts",
                column: "ComplementaryProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemComplementaryProducts_OrderItemId",
                table: "OrderItemComplementaryProducts",
                column: "OrderItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BeadComplementaryProducts");

            migrationBuilder.DropTable(
                name: "CartItemComplementaryProducts");

            migrationBuilder.DropTable(
                name: "OrderItemComplementaryProducts");

            migrationBuilder.DropTable(
                name: "ComplementaryProducts");

            migrationBuilder.CreateTable(
                name: "ExtraBraceletForBeads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ComponentId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraBraceletForBeads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtraBraceletForBeads_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExtraBraceletForBeads_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExtraBraceletForBeads_ComponentId",
                table: "ExtraBraceletForBeads",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtraBraceletForBeads_ProductId",
                table: "ExtraBraceletForBeads",
                column: "ProductId");
        }
    }
}
