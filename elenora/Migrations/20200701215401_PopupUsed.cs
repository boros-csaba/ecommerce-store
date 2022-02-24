using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class PopupUsed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PopupCouponUsed",
                table: "Orders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PopupCouponUsed",
                table: "Orders",
                type: "boolean",
                nullable: true);
        }
    }
}
