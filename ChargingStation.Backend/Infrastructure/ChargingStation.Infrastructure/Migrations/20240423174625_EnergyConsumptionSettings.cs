using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChargingStation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnergyConsumptionSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnergyLimit",
                table: "Depots");

            migrationBuilder.CreateTable(
                name: "DepotEnergyConsumptionSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepotEnergyLimit = table.Column<double>(type: "float", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepotEnergyConsumptionSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepotEnergyConsumptionSettings_Depot",
                        column: x => x.DepotId,
                        principalTable: "Depots",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChargePointEnergyConsumptionSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChargePointId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepotEnergyConsumptionSettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChargePointEnergyLimit = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargePointEnergyConsumptionSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChargePointEnergyConsumptionSettings_ChargePoint",
                        column: x => x.ChargePointId,
                        principalTable: "ChargePoints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChargePointsLimits_DepotEnergyConsumptionSettings",
                        column: x => x.DepotEnergyConsumptionSettingsId,
                        principalTable: "DepotEnergyConsumptionSettings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EnergyConsumptionIntervalSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepotEnergyConsumptionSettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EnergyLimit = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnergyConsumptionIntervalSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Intervals_DepotEnergyConsumptionSettings",
                        column: x => x.DepotEnergyConsumptionSettingsId,
                        principalTable: "DepotEnergyConsumptionSettings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChargePointEnergyConsumptionSettings_ChargePointId",
                table: "ChargePointEnergyConsumptionSettings",
                column: "ChargePointId");

            migrationBuilder.CreateIndex(
                name: "IX_ChargePointEnergyConsumptionSettings_DepotEnergyConsumptionSettingsId",
                table: "ChargePointEnergyConsumptionSettings",
                column: "DepotEnergyConsumptionSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_DepotEnergyConsumptionSettings_DepotId",
                table: "DepotEnergyConsumptionSettings",
                column: "DepotId");

            migrationBuilder.CreateIndex(
                name: "IX_EnergyConsumptionIntervalSettings_DepotEnergyConsumptionSettingsId",
                table: "EnergyConsumptionIntervalSettings",
                column: "DepotEnergyConsumptionSettingsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChargePointEnergyConsumptionSettings");

            migrationBuilder.DropTable(
                name: "EnergyConsumptionIntervalSettings");

            migrationBuilder.DropTable(
                name: "DepotEnergyConsumptionSettings");

            migrationBuilder.AddColumn<double>(
                name: "EnergyLimit",
                table: "Depots",
                type: "float",
                nullable: true);
        }
    }
}
