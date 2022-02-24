using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class PopupRemark : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PopupDisplayRemark",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PopupDisplayRemark",
                table: "Customers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PopupDisplayRemark",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PopupDisplayRemark",
                table: "Customers");
        }
    }
}
