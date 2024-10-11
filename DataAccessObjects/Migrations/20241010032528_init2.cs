using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObjects.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Locations_DestinationID",
                table: "Flights");

            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Locations_OriginID",
                table: "Flights");

            migrationBuilder.AddForeignKey(
                name: "FK_Flight_Destination_Location",
                table: "Flights",
                column: "DestinationID",
                principalTable: "Locations",
                principalColumn: "LocationID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Flight_Origin_Location",
                table: "Flights",
                column: "OriginID",
                principalTable: "Locations",
                principalColumn: "LocationID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flight_Destination_Location",
                table: "Flights");

            migrationBuilder.DropForeignKey(
                name: "FK_Flight_Origin_Location",
                table: "Flights");

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Locations_DestinationID",
                table: "Flights",
                column: "DestinationID",
                principalTable: "Locations",
                principalColumn: "LocationID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Locations_OriginID",
                table: "Flights",
                column: "OriginID",
                principalTable: "Locations",
                principalColumn: "LocationID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
