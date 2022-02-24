using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace elenora.Migrations
{
    public partial class Faq : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Faqs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(nullable: true),
                    Answer = table.Column<string>(nullable: true),
                    FaqPageOrder = table.Column<int>(nullable: false),
                    FaqPageOpenCount = table.Column<int>(nullable: false),
                    BraceletDesignerOrder = table.Column<int>(nullable: false),
                    BraceletDesignerOpenCount = table.Column<int>(nullable: false),
                    ProductDetailsOrder = table.Column<int>(nullable: false),
                    ProductDetailsOpenCount = table.Column<int>(nullable: false),
                    CartPageOrder = table.Column<int>(nullable: false),
                    CartPageOpenCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faqs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Faqs");
        }
    }
}
