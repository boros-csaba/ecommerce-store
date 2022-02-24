using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class ComponentInBraceletDesigner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SelectabeInBraceletDesigner",
                table: "Components",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoldOutInBraceletDesigner",
                table: "Components",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectabeInBraceletDesigner",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "SoldOutInBraceletDesigner",
                table: "Components");
        }
    }
}
