using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class PopupCoupon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Popups_Coupons_CouponId",
                table: "Popups");

            migrationBuilder.DropIndex(
                name: "IX_Popups_CouponId",
                table: "Popups");

            migrationBuilder.DropColumn(
                name: "CouponId",
                table: "Popups");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CouponId",
                table: "Popups",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Popups_CouponId",
                table: "Popups",
                column: "CouponId");

            migrationBuilder.AddForeignKey(
                name: "FK_Popups_Coupons_CouponId",
                table: "Popups",
                column: "CouponId",
                principalTable: "Coupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
