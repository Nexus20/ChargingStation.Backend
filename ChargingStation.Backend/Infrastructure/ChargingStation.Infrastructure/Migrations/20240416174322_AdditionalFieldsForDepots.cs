using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChargingStation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalFieldsForDepots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Depots",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Depots",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Depots",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Depots");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Depots");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Depots");
        }
    }
}
