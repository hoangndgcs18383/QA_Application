﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QA_Application.Migrations
{
    public partial class update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Thumbs_Ideas_IdeaId",
                table: "Thumbs");

            migrationBuilder.DropColumn(
                name: "Count",
                table: "Thumbs");

            migrationBuilder.DropColumn(
                name: "isThumb",
                table: "Thumbs");

            migrationBuilder.AlterColumn<int>(
                name: "IdeaId",
                table: "Thumbs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Thumbs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "toggle",
                table: "Thumbs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "CountThumb",
                table: "Ideas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Thumbs_AuthorId",
                table: "Thumbs",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Thumbs_AspNetUsers_AuthorId",
                table: "Thumbs",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Thumbs_Ideas_IdeaId",
                table: "Thumbs",
                column: "IdeaId",
                principalTable: "Ideas",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Thumbs_AspNetUsers_AuthorId",
                table: "Thumbs");

            migrationBuilder.DropForeignKey(
                name: "FK_Thumbs_Ideas_IdeaId",
                table: "Thumbs");

            migrationBuilder.DropIndex(
                name: "IX_Thumbs_AuthorId",
                table: "Thumbs");

            migrationBuilder.DropColumn(
                name: "toggle",
                table: "Thumbs");

            migrationBuilder.DropColumn(
                name: "CountThumb",
                table: "Ideas");

            migrationBuilder.AlterColumn<int>(
                name: "IdeaId",
                table: "Thumbs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Thumbs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "Thumbs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "isThumb",
                table: "Thumbs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Thumbs_Ideas_IdeaId",
                table: "Thumbs",
                column: "IdeaId",
                principalTable: "Ideas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
