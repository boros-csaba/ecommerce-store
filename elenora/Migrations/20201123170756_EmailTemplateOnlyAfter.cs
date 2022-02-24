using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class EmailTemplateOnlyAfter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailTemplates_EmailTemplates_OnlyAfterEmailId",
                table: "EmailTemplates");

            migrationBuilder.AlterColumn<int>(
                name: "OnlyAfterEmailId",
                table: "EmailTemplates",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailTemplates_EmailTemplates_OnlyAfterEmailId",
                table: "EmailTemplates",
                column: "OnlyAfterEmailId",
                principalTable: "EmailTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailTemplates_EmailTemplates_OnlyAfterEmailId",
                table: "EmailTemplates");

            migrationBuilder.AlterColumn<int>(
                name: "OnlyAfterEmailId",
                table: "EmailTemplates",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailTemplates_EmailTemplates_OnlyAfterEmailId",
                table: "EmailTemplates",
                column: "OnlyAfterEmailId",
                principalTable: "EmailTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
