using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChargingStation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixTimeZone2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Depot_TimeZone",
                table: "Depots");

            migrationBuilder.AddForeignKey(
                name: "FK_Depot_TimeZone",
                table: "Depots",
                column: "TimeZoneId",
                principalTable: "TimeZones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Depot_TimeZone",
                table: "Depots");

            migrationBuilder.AddForeignKey(
                name: "FK_Depot_TimeZone",
                table: "Depots",
                column: "TimeZoneId",
                principalTable: "TimeZones",
                principalColumn: "Id");
        }
    }
}
