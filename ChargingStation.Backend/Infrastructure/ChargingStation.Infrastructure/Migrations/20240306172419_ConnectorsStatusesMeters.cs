using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChargingStation.Infrastructure.Migrations;

/// <inheritdoc />
public partial class ConnectorsStatusesMeters : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "TagId",
            table: "OcppTags",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(450)");

        migrationBuilder.AlterColumn<string>(
            name: "ParentTagId",
            table: "OcppTags",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.CreateTable(
            name: "Connectors",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ChargePointId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ConnectorId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Connectors", x => x.Id);
                table.ForeignKey(
                    name: "FK_Connectors_ChargePoint",
                    column: x => x.ChargePointId,
                    principalTable: "ChargePoints",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "ConnectorStatus",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ConnectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CurrentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                StatusUpdatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                ErrorCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Info = table.Column<string>(type: "nvarchar(max)", nullable: true),
                VendorErrorCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                VendorId = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ConnectorStatus", x => x.Id);
                table.ForeignKey(
                    name: "FK_ConnectorStatuses_Connector",
                    column: x => x.ConnectorId,
                    principalTable: "Connectors",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "Transactions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TransactionId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Uid = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                StartTagId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                MeterStart = table.Column<double>(type: "float", nullable: false),
                StartResult = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                StopTagId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                StopTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                MeterStop = table.Column<double>(type: "float", nullable: true),
                StopReason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                ConnectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ChargePointId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Transactions", x => x.Id);
                table.ForeignKey(
                    name: "FK_Transactions_ChargePoints_ChargePointId",
                    column: x => x.ChargePointId,
                    principalTable: "ChargePoints",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Transactions_Connectors_ConnectorId",
                    column: x => x.ConnectorId,
                    principalTable: "Connectors",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ConnectorMeterValue",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ConnectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Format = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Measurand = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Phase = table.Column<string>(type: "nvarchar(max)", nullable: true),
                MeterValueTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ConnectorMeterValue", x => x.Id);
                table.ForeignKey(
                    name: "FK_ConnectorMeterValues_Connector",
                    column: x => x.ConnectorId,
                    principalTable: "Connectors",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_ConnectorMeterValues_Transaction",
                    column: x => x.TransactionId,
                    principalTable: "Transactions",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_ConnectorMeterValue_ConnectorId",
            table: "ConnectorMeterValue",
            column: "ConnectorId");

        migrationBuilder.CreateIndex(
            name: "IX_ConnectorMeterValue_TransactionId",
            table: "ConnectorMeterValue",
            column: "TransactionId");

        migrationBuilder.CreateIndex(
            name: "IX_Connectors_ChargePointId_ConnectorId",
            table: "Connectors",
            columns: new[] { "ChargePointId", "ConnectorId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ConnectorStatus_ConnectorId",
            table: "ConnectorStatus",
            column: "ConnectorId");

        migrationBuilder.CreateIndex(
            name: "IX_Transactions_ChargePointId",
            table: "Transactions",
            column: "ChargePointId");

        migrationBuilder.CreateIndex(
            name: "IX_Transactions_ConnectorId",
            table: "Transactions",
            column: "ConnectorId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ConnectorMeterValue");

        migrationBuilder.DropTable(
            name: "ConnectorStatus");

        migrationBuilder.DropTable(
            name: "Transactions");

        migrationBuilder.DropTable(
            name: "Connectors");

        migrationBuilder.AlterColumn<string>(
            name: "TagId",
            table: "OcppTags",
            type: "nvarchar(450)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50);

        migrationBuilder.AlterColumn<string>(
            name: "ParentTagId",
            table: "OcppTags",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);
    }
}