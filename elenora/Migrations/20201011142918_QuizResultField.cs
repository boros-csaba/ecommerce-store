using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class QuizResultField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Result",
                table: "QuizResults",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Result",
                table: "QuizResults");
        }
    }
}
