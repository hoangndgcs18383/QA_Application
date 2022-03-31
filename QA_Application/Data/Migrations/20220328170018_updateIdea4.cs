using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QA_Application.Data.Migrations
{
    public partial class updateIdea4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdateDate",
                table: "Ideas",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdateDate",
                table: "Ideas");
        }
    }
}
