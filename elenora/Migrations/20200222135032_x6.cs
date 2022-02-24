using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class x6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CookieId",
                table: "ActionLogs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CookieId",
                table: "ActionLogs");
        }
    }
}
