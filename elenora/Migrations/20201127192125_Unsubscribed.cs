using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class Unsubscribed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Unsubscribed",
                table: "EmailHistories",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unsubscribed",
                table: "EmailHistories");
        }
    }
}
