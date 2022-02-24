using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace elenora.Migrations
{
    public partial class QuizResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuizResults",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<int>(nullable: false),
                    QuizName = table.Column<string>(nullable: true),
                    Answer1 = table.Column<string>(nullable: true),
                    Answer1Date = table.Column<DateTime>(nullable: true),
                    Answer2 = table.Column<string>(nullable: true),
                    Answer2Date = table.Column<DateTime>(nullable: true),
                    Answer3 = table.Column<string>(nullable: true),
                    Answer3Date = table.Column<DateTime>(nullable: true),
                    Answer4 = table.Column<string>(nullable: true),
                    Answer4Date = table.Column<DateTime>(nullable: true),
                    Answer5 = table.Column<string>(nullable: true),
                    Answer5Date = table.Column<DateTime>(nullable: true),
                    Answer6 = table.Column<string>(nullable: true),
                    Answer6Date = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizResults_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizResults_CustomerId",
                table: "QuizResults",
                column: "CustomerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizResults");
        }
    }
}
