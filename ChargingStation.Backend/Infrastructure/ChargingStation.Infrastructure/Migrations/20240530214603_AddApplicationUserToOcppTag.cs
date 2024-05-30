using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChargingStation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationUserToOcppTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_OcppTags_OcppTagId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_OcppTagId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OcppTagId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "OcppTags",
                type: "uniqueidentifier",
                nullable: true,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_OcppTags_ApplicationUserId",
                table: "OcppTags",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OcppTags_Users_ApplicationUserId",
                table: "OcppTags",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OcppTags_Users_ApplicationUserId",
                table: "OcppTags");

            migrationBuilder.DropIndex(
                name: "IX_OcppTags_ApplicationUserId",
                table: "OcppTags");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "OcppTags");

            migrationBuilder.AddColumn<Guid>(
                name: "OcppTagId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_OcppTagId",
                table: "Users",
                column: "OcppTagId",
                unique: true,
                filter: "[OcppTagId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_OcppTags_OcppTagId",
                table: "Users",
                column: "OcppTagId",
                principalTable: "OcppTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
