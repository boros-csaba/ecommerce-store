using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class Referrer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Referrer",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Referrer",
                table: "ActionLogs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Referrer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Referrer",
                table: "ActionLogs");
        }
    }
}
