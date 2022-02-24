using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace elenora.Migrations
{
    public partial class PopupStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PopupActionExecutedCount",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PopupCouponUsed",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PopupDisplayedCount",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PopupId",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PopupLastDisplayed",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PopupActionExecutedCount",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PopupDisplayedCount",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PopupId",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PopupLastDisplayed",
                table: "Customers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Popups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    CouponId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Popups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Popups_Coupons_CouponId",
                        column: x => x.CouponId,
                        principalTable: "Coupons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PopupId",
                table: "Orders",
                column: "PopupId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PopupId",
                table: "Customers",
                column: "PopupId");

            migrationBuilder.CreateIndex(
                name: "IX_Popups_CouponId",
                table: "Popups",
                column: "CouponId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Popups_PopupId",
                table: "Customers",
                column: "PopupId",
                principalTable: "Popups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Popups_PopupId",
                table: "Orders",
                column: "PopupId",
                principalTable: "Popups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Popups_PopupId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Popups_PopupId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Popups");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PopupId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Customers_PopupId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PopupActionExecutedCount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PopupCouponUsed",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PopupDisplayedCount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PopupId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PopupLastDisplayed",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PopupActionExecutedCount",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PopupDisplayedCount",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PopupId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PopupLastDisplayed",
                table: "Customers");
        }
    }
}
