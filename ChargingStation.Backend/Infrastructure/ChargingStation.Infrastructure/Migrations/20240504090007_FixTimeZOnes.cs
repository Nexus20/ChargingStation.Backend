using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChargingStation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixTimeZOnes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Depots_TimeZones_TimeZoneId",
                table: "Depots");

            migrationBuilder.DropForeignKey(
                name: "FK_Depots_TimeZones_TimeZoneId1",
                table: "Depots");

            migrationBuilder.DropIndex(
                name: "IX_Depots_TimeZoneId1",
                table: "Depots");

            migrationBuilder.DropColumn(
                name: "TimeZoneId1",
                table: "Depots");

            migrationBuilder.AddForeignKey(
                name: "FK_Depot_TimeZone",
                table: "Depots",
                column: "TimeZoneId",
                principalTable: "TimeZones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Depot_TimeZone",
                table: "Depots");

            migrationBuilder.AddColumn<Guid>(
                name: "TimeZoneId1",
                table: "Depots",
                type: "uniqueidentifier",
                nullable: true);

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
    }
}
