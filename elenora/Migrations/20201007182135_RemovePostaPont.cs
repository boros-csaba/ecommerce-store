using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class RemovePostaPont : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostaPont",
                table: "Orders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostaPont",
                table: "Orders",
                type: "text",
                nullable: true);
        }
    }
}
