using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace elenora.Migrations
{
    public partial class ComponentFamily : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ComponentFamilyId",
                table: "Components",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ComponentFamily",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentFamily", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Components_ComponentFamilyId",
                table: "Components",
                column: "ComponentFamilyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Components_ComponentFamily_ComponentFamilyId",
                table: "Components",
                column: "ComponentFamilyId",
                principalTable: "ComponentFamily",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Components_ComponentFamily_ComponentFamilyId",
                table: "Components");

            migrationBuilder.DropTable(
                name: "ComponentFamily");

            migrationBuilder.DropIndex(
                name: "IX_Components_ComponentFamilyId",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "ComponentFamilyId",
                table: "Components");
        }
    }
}
