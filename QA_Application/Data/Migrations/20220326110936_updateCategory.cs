using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QA_Application.Data.Migrations
{
    public partial class updateCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FinalDeadline",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstDeadline",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalDeadline",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "FirstDeadline",
                table: "Categories");
        }
    }
}
