using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class StringBraceletCartItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BraceletType",
                table: "CartItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FlapColor1",
                table: "CartItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FlapColor2",
                table: "CartItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KnotColor",
                table: "CartItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StringColor1",
                table: "CartItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StringColor2",
                table: "CartItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StringColor3",
                table: "CartItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BraceletType",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "FlapColor1",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "FlapColor2",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "KnotColor",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "StringColor1",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "StringColor2",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "StringColor3",
                table: "CartItems");
        }
    }
}
