using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace elenora.Migrations
{
    public partial class RemoveHoroscopeBracelets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_HoroscopeBracelets_HoroscopeBraceletId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_HoroscopeBracelets_HoroscopeBraceletId",
                table: "OrderItems");

            migrationBuilder.DropTable(
                name: "HoroscopeBracelets");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_HoroscopeBraceletId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_HoroscopeBraceletId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "OriginalPrice",
                table: "Products");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OriginalPrice",
                table: "Products",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HoroscopeBraceletOrderItem_BraceletSize",
                table: "OrderItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HoroscopeBraceletId",
                table: "OrderItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HoroscopeBraceletCartItem_BraceletSize",
                table: "CartItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HoroscopeBraceletId",
                table: "CartItems",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HoroscopeBracelets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    BeadId = table.Column<int>(type: "integer", nullable: false),
                    HoroscopeId = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoroscopeBracelets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoroscopeBracelets_Components_BeadId",
                        column: x => x.BeadId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoroscopeBracelets_Horoscopes_HoroscopeId",
                        column: x => x.HoroscopeId,
                        principalTable: "Horoscopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_HoroscopeBraceletId",
                table: "OrderItems",
                column: "HoroscopeBraceletId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_HoroscopeBraceletId",
                table: "CartItems",
                column: "HoroscopeBraceletId");

            migrationBuilder.CreateIndex(
                name: "IX_HoroscopeBracelets_BeadId",
                table: "HoroscopeBracelets",
                column: "BeadId");

            migrationBuilder.CreateIndex(
                name: "IX_HoroscopeBracelets_HoroscopeId",
                table: "HoroscopeBracelets",
                column: "HoroscopeId");

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
    }
}
