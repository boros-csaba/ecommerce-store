using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class GiftBraceletOrderItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PromotionType",
                table: "OrderItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PromotionType",
                table: "OrderItems");
        }
    }
}
