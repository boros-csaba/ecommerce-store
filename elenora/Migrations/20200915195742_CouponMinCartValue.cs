using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class CouponMinCartValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MinCartValue",
                table: "Coupons",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinCartValue",
                table: "Coupons");
        }
    }
}
