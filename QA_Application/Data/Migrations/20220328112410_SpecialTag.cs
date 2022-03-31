using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QA_Application.Data.Migrations
{
    public partial class SpecialTag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "SpecialTags");

            migrationBuilder.AddColumn<string>(
                name: "SpecialTagName",
                table: "SpecialTags",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpecialTagName",
                table: "SpecialTags");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SpecialTags",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
