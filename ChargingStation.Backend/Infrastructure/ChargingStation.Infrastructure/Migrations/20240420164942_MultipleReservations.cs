using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChargingStation.Infrastructure.Migrations;

/// <inheritdoc />
public partial class MultipleReservations : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Reservations_ConnectorId",
            table: "Reservations");

        migrationBuilder.CreateIndex(
            name: "IX_Reservations_ConnectorId",
            table: "Reservations",
            column: "ConnectorId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Reservations_ConnectorId",
            table: "Reservations");

        migrationBuilder.CreateIndex(
            name: "IX_Reservations_ConnectorId",
            table: "Reservations",
            column: "ConnectorId",
            unique: true,
            filter: "[ConnectorId] IS NOT NULL");
    }
}