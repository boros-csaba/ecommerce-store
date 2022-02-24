using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class CouponGetOneFree : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GetOneFreeMinimumQuantity",
                table: "Coupons",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GetOneFreeMinimumQuantity",
                table: "Coupons");
        }
    }
}
