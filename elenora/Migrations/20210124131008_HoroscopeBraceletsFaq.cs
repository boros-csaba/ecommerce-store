using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class HoroscopeBraceletsFaq : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HoroscopeBraceletsOpenCount",
                table: "Faqs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HoroscopeBraceletsOrder",
                table: "Faqs",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoroscopeBraceletsOpenCount",
                table: "Faqs");

            migrationBuilder.DropColumn(
                name: "HoroscopeBraceletsOrder",
                table: "Faqs");
        }
    }
}
