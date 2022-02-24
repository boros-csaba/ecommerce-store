using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class StringBraceletOrderItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BraceletType",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FlapColor1",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FlapColor2",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KnotColor",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StringColor1",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StringColor2",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StringColor3",
                table: "OrderItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BraceletType",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "FlapColor1",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "FlapColor2",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "KnotColor",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "StringColor1",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "StringColor2",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "StringColor3",
                table: "OrderItems");
        }
    }
}
