using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace elenora.Migrations
{
    public partial class HoroscopeBracelet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HoroscopeBracelets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HoroscopeId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    BeadId = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoroscopeBracelets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoroscopeBracelets_Components_BeadId",
                        column: x => x.BeadId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoroscopeBracelets_Horoscopes_HoroscopeId",
                        column: x => x.HoroscopeId,
                        principalTable: "Horoscopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoroscopeBracelets_BeadId",
                table: "HoroscopeBracelets",
                column: "BeadId");

            migrationBuilder.CreateIndex(
                name: "IX_HoroscopeBracelets_HoroscopeId",
                table: "HoroscopeBracelets",
                column: "HoroscopeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HoroscopeBracelets");
        }
    }
}
