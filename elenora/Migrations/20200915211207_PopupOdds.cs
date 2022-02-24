using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class PopupOdds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Odds",
                table: "Popups",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Odds",
                table: "Popups");
        }
    }
}
