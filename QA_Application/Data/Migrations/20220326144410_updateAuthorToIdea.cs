using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QA_Application.Data.Migrations
{
    public partial class updateAuthorToIdea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Ideas",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Ideas_AuthorId",
                table: "Ideas",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ideas_AspNetUsers_AuthorId",
                table: "Ideas",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ideas_AspNetUsers_AuthorId",
                table: "Ideas");

            migrationBuilder.DropIndex(
                name: "IX_Ideas_AuthorId",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Ideas");
        }
    }
}
