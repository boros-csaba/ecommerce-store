using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace elenora.Migrations
{
    public partial class LastPaymentRequestEmailDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastPaymentRequestEmailDate",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPaymentRequestEmailDate",
                table: "Orders");
        }
    }
}
