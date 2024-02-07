using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChargingStation.Infrastructure.Migrations;

/// <inheritdoc />
public partial class ChargePoints : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ChargePoints",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DepotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OcppProtocol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                RegistrationStatus = table.Column<int>(type: "int", nullable: false),
                ChargePointVendor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ChargePointModel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ChargeBoxSerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                FirmwareVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                FirmwareUpdateTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                Iccid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Imsi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                MeterType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                MeterSerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DiagnosticsTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                LastHeartbeat = table.Column<DateTime>(type: "datetime2", nullable: true),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ChargePoints", x => x.Id);
                table.ForeignKey(
                    name: "FK_ChargePoints_Depots_DepotId",
                    column: x => x.DepotId,
                    principalTable: "Depots",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ChargePoints_DepotId",
            table: "ChargePoints",
            column: "DepotId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ChargePoints");
    }
}