using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class RemoveSoldOutInBraceletDesigner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoldOutInBraceletDesigner",
                table: "Components");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SoldOutInBraceletDesigner",
                table: "Components",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
