using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class Initial11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductType",
                table: "Products",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BraceletSize",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BraceletSize2",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BraceletSize",
                table: "CartItems",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "BraceletSize2",
                table: "CartItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductType",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "BraceletSize",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "BraceletSize2",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "BraceletSize2",
                table: "CartItems");

            migrationBuilder.AlterColumn<int>(
                name: "BraceletSize",
                table: "CartItems",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
