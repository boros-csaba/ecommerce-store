using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace elenora.Migrations
{
    public partial class CustomBracelet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CartItems",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "BeadTypeId",
                table: "CartItems",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomBraceletComponents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CartItemId = table.Column<int>(nullable: false),
                    ComponentId = table.Column<int>(nullable: false),
                    Position = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomBraceletComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomBraceletComponents_CartItems_CartItemId",
                        column: x => x.CartItemId,
                        principalTable: "CartItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomBraceletComponents_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_BeadTypeId",
                table: "CartItems",
                column: "BeadTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomBraceletComponents_CartItemId",
                table: "CustomBraceletComponents",
                column: "CartItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomBraceletComponents_ComponentId",
                table: "CustomBraceletComponents",
                column: "ComponentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Components_BeadTypeId",
                table: "CartItems",
                column: "BeadTypeId",
                principalTable: "Components",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Components_BeadTypeId",
                table: "CartItems");

            migrationBuilder.DropTable(
                name: "CustomBraceletComponents");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_BeadTypeId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "BeadTypeId",
                table: "CartItems");
        }
    }
}
