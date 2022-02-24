using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class EmailUnsubscribe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Unsubscribed",
                table: "EmailAddresses",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unsubscribed",
                table: "EmailAddresses");
        }
    }
}
