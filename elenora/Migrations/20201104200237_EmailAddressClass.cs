using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace elenora.Migrations
{
    public partial class EmailAddressClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn( 
                name: "EmailAddress",
                table: "EmailLogs",
                newName: "Email");

            migrationBuilder.AddColumn<int>(
                name: "EmailAddressId",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmailAddressId",
                table: "EmailLogs",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmailAddressId",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmailAddressId",
                table: "BraceletPreviewRequests",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmailAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Address = table.Column<string>(nullable: true),
                    AddedDate = table.Column<DateTime>(nullable: false),
                    Source = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailAddresses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_EmailAddressId",
                table: "Orders",
                column: "EmailAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_EmailAddressId",
                table: "EmailLogs",
                column: "EmailAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_EmailAddressId",
                table: "Customers",
                column: "EmailAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_BraceletPreviewRequests_EmailAddressId",
                table: "BraceletPreviewRequests",
                column: "EmailAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_BraceletPreviewRequests_EmailAddresses_EmailAddressId",
                table: "BraceletPreviewRequests",
                column: "EmailAddressId",
                principalTable: "EmailAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_EmailAddresses_EmailAddressId",
                table: "Customers",
                column: "EmailAddressId",
                principalTable: "EmailAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailLogs_EmailAddresses_EmailAddressId",
                table: "EmailLogs",
                column: "EmailAddressId",
                principalTable: "EmailAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_EmailAddresses_EmailAddressId",
                table: "Orders",
                column: "EmailAddressId",
                principalTable: "EmailAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BraceletPreviewRequests_EmailAddresses_EmailAddressId",
                table: "BraceletPreviewRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_EmailAddresses_EmailAddressId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailLogs_EmailAddresses_EmailAddressId",
                table: "EmailLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_EmailAddresses_EmailAddressId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "EmailAddresses");

            migrationBuilder.DropIndex(
                name: "IX_Orders_EmailAddressId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_EmailLogs_EmailAddressId",
                table: "EmailLogs");

            migrationBuilder.DropIndex(
                name: "IX_Customers_EmailAddressId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_BraceletPreviewRequests_EmailAddressId",
                table: "BraceletPreviewRequests");

            migrationBuilder.DropColumn(
                name: "EmailAddressId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                newName: "EmailAddress",
                table: "EmailLogs",
                name: "Email");

            migrationBuilder.DropColumn(
                name: "EmailAddressId",
                table: "EmailLogs");

            migrationBuilder.DropColumn(
                name: "EmailAddressId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "EmailAddressId",
                table: "BraceletPreviewRequests");

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "EmailLogs",
                type: "text",
                nullable: true);
        }
    }
}
