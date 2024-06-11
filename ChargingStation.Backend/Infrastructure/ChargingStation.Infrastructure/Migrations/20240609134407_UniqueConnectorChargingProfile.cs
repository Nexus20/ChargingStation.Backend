using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChargingStation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UniqueConnectorChargingProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ConnectorChargingProfile_ConnectorId",
                table: "ConnectorChargingProfile");

            migrationBuilder.CreateIndex(
                name: "IX_ConnectorChargingProfile_ConnectorId_ChargingProfileId",
                table: "ConnectorChargingProfile",
                columns: new[] { "ConnectorId", "ChargingProfileId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ConnectorChargingProfile_ConnectorId_ChargingProfileId",
                table: "ConnectorChargingProfile");

            migrationBuilder.CreateIndex(
                name: "IX_ConnectorChargingProfile_ConnectorId",
                table: "ConnectorChargingProfile",
                column: "ConnectorId");
        }
    }
}
