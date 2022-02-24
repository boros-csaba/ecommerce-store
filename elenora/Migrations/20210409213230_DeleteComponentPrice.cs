using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class DeleteComponentPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Components");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Components",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
