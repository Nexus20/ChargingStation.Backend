using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChargingStation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReservationsAndChargingProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChargingProfile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChargingProfileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StackLevel = table.Column<int>(type: "int", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecurrencyKind = table.Column<int>(type: "int", nullable: false),
                    ChargingProfilePurpose = table.Column<int>(type: "int", nullable: false),
                    ChargingProfileKind = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    StartSchedule = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SchedulingUnit = table.Column<int>(type: "int", nullable: false),
                    MinChargingRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargingProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReservationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChargePointId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConnectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReservationRequestId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CancellationRequestId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_ChargePoint",
                        column: x => x.ChargePointId,
                        principalTable: "ChargePoints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reservations_Connectors_ConnectorId",
                        column: x => x.ConnectorId,
                        principalTable: "Connectors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reservations_OcppTags_TagId",
                        column: x => x.TagId,
                        principalTable: "OcppTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChargingSchedulePeriod",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChargingProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartPeriod = table.Column<int>(type: "int", nullable: false),
                    Limit = table.Column<double>(type: "float", nullable: false),
                    NumberPhases = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargingSchedulePeriod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChargingSchedulePeriods_ChargingProfile",
                        column: x => x.ChargingProfileId,
                        principalTable: "ChargingProfile",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ConnectorChargingProfile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConnectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChargingProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectorChargingProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConnectorChargingProfiles_ChargingProfile",
                        column: x => x.ChargingProfileId,
                        principalTable: "ChargingProfile",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ConnectorChargingProfiles_Connector",
                        column: x => x.ConnectorId,
                        principalTable: "Connectors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChargingSchedulePeriod_ChargingProfileId",
                table: "ChargingSchedulePeriod",
                column: "ChargingProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ConnectorChargingProfile_ChargingProfileId",
                table: "ConnectorChargingProfile",
                column: "ChargingProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ConnectorChargingProfile_ConnectorId",
                table: "ConnectorChargingProfile",
                column: "ConnectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ChargePointId",
                table: "Reservations",
                column: "ChargePointId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ConnectorId",
                table: "Reservations",
                column: "ConnectorId",
                unique: true,
                filter: "[ConnectorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ReservationRequestId",
                table: "Reservations",
                column: "ReservationRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TagId",
                table: "Reservations",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TransactionId",
                table: "Reservations",
                column: "TransactionId",
                unique: true,
                filter: "[TransactionId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChargingSchedulePeriod");

            migrationBuilder.DropTable(
                name: "ConnectorChargingProfile");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "ChargingProfile");
        }
    }
}
