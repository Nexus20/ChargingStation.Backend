using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChargingStation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedConnectorIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Connectors_ChargePointId",
                table: "Connectors");

            migrationBuilder.AddColumn<int>(
                name: "ConnectorId",
                table: "Connectors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Connectors_ChargePointId_ConnectorId",
                table: "Connectors",
                columns: new[] { "ChargePointId", "ConnectorId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Connectors_ChargePointId_ConnectorId",
                table: "Connectors");

            migrationBuilder.DropColumn(
                name: "ConnectorId",
                table: "Connectors");

            migrationBuilder.CreateIndex(
                name: "IX_Connectors_ChargePointId",
                table: "Connectors",
                column: "ChargePointId");
        }
    }
}
