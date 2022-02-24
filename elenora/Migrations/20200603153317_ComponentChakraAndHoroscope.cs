using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace elenora.Migrations
{
    public partial class ComponentChakraAndHoroscope : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Components_ComponentFamily_ComponentFamilyId",
                table: "Components");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ComponentFamily",
                table: "ComponentFamily");

            migrationBuilder.RenameTable(
                name: "ComponentFamily",
                newName: "ComponentFamilies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ComponentFamilies",
                table: "ComponentFamilies",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Chakras",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdString = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chakras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Horoscopes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdString = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    DateRange = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Horoscopes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComponentFamilyChakras",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ComponentFamilyId = table.Column<int>(nullable: false),
                    ChakraId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentFamilyChakras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComponentFamilyChakras_Chakras_ChakraId",
                        column: x => x.ChakraId,
                        principalTable: "Chakras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComponentFamilyChakras_ComponentFamilies_ComponentFamilyId",
                        column: x => x.ComponentFamilyId,
                        principalTable: "ComponentFamilies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComponentFamilyHoroscopes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ComponentFamilyId = table.Column<int>(nullable: false),
                    HoroscopeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentFamilyHoroscopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComponentFamilyHoroscopes_ComponentFamilies_ComponentFamily~",
                        column: x => x.ComponentFamilyId,
                        principalTable: "ComponentFamilies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComponentFamilyHoroscopes_Horoscopes_HoroscopeId",
                        column: x => x.HoroscopeId,
                        principalTable: "Horoscopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComponentFamilyChakras_ChakraId",
                table: "ComponentFamilyChakras",
                column: "ChakraId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentFamilyChakras_ComponentFamilyId",
                table: "ComponentFamilyChakras",
                column: "ComponentFamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentFamilyHoroscopes_ComponentFamilyId",
                table: "ComponentFamilyHoroscopes",
                column: "ComponentFamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentFamilyHoroscopes_HoroscopeId",
                table: "ComponentFamilyHoroscopes",
                column: "HoroscopeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Components_ComponentFamilies_ComponentFamilyId",
                table: "Components",
                column: "ComponentFamilyId",
                principalTable: "ComponentFamilies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Components_ComponentFamilies_ComponentFamilyId",
                table: "Components");

            migrationBuilder.DropTable(
                name: "ComponentFamilyChakras");

            migrationBuilder.DropTable(
                name: "ComponentFamilyHoroscopes");

            migrationBuilder.DropTable(
                name: "Chakras");

            migrationBuilder.DropTable(
                name: "Horoscopes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ComponentFamilies",
                table: "ComponentFamilies");

            migrationBuilder.RenameTable(
                name: "ComponentFamilies",
                newName: "ComponentFamily");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ComponentFamily",
                table: "ComponentFamily",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Components_ComponentFamily_ComponentFamilyId",
                table: "Components",
                column: "ComponentFamilyId",
                principalTable: "ComponentFamily",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
