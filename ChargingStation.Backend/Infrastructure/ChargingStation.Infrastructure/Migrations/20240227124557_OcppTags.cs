using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChargingStation.Infrastructure.Migrations;

/// <inheritdoc />
public partial class OcppTags : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "OcppTags",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TagId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ParentTagId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                Blocked = table.Column<bool>(type: "bit", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OcppTags", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_OcppTags_TagId",
            table: "OcppTags",
            column: "TagId",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "OcppTags");
    }
}