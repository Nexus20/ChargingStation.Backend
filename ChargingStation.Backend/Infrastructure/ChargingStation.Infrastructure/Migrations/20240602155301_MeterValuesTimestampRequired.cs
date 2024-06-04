using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChargingStation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MeterValuesTimestampRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OcppTags_ApplicationUserId",
                table: "OcppTags");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MeterValueTimestamp",
                table: "ConnectorMeterValue",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OcppTags_ApplicationUserId",
                table: "OcppTags",
                column: "ApplicationUserId",
                unique: true,
                filter: "[ApplicationUserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OcppTags_ApplicationUserId",
                table: "OcppTags");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MeterValueTimestamp",
                table: "ConnectorMeterValue",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_OcppTags_ApplicationUserId",
                table: "OcppTags",
                column: "ApplicationUserId",
                unique: true);
        }
    }
}
