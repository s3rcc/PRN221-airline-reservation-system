using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObjects.Migrations
{
    /// <inheritdoc />
    public partial class FLightFix2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "availableVipSeat",
                table: "Flights",
                newName: "AvailableVipSeat");

            migrationBuilder.RenameColumn(
                name: "availableNormalSeat",
                table: "Flights",
                newName: "AvailableNormalSeat");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvailableVipSeat",
                table: "Flights",
                newName: "availableVipSeat");

            migrationBuilder.RenameColumn(
                name: "AvailableNormalSeat",
                table: "Flights",
                newName: "availableNormalSeat");
        }
    }
}
