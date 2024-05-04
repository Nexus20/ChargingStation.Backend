using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChargingStation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeZoneTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TimeZoneId",
                table: "Depots",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TimeZoneId1",
                table: "Depots",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TimeZones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    BaseUtcOffset = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    IanaId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    WindowsId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeZones", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Depots_TimeZoneId",
                table: "Depots",
                column: "TimeZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Depots_TimeZoneId1",
                table: "Depots",
                column: "TimeZoneId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Depots_TimeZones_TimeZoneId",
                table: "Depots",
                column: "TimeZoneId",
                principalTable: "TimeZones",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Depots_TimeZones_TimeZoneId1",
                table: "Depots",
                column: "TimeZoneId1",
                principalTable: "TimeZones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Depots_TimeZones_TimeZoneId",
                table: "Depots");

            migrationBuilder.DropForeignKey(
                name: "FK_Depots_TimeZones_TimeZoneId1",
                table: "Depots");

            migrationBuilder.DropTable(
                name: "TimeZones");

            migrationBuilder.DropIndex(
                name: "IX_Depots_TimeZoneId",
                table: "Depots");

            migrationBuilder.DropIndex(
                name: "IX_Depots_TimeZoneId1",
                table: "Depots");

            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                table: "Depots");

            migrationBuilder.DropColumn(
                name: "TimeZoneId1",
                table: "Depots");
        }
    }
}
