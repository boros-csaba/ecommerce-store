using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class PromotionMinOrderValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MinOrderValue",
                table: "Promotions",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinOrderValue",
                table: "Promotions");
        }
    }
}
