using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class HoroscopeBraceletPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "HoroscopeBracelets",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "HoroscopeBracelets");
        }
    }
}
