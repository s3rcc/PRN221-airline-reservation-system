using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessObjects.Migrations
{
    /// <inheritdoc />
    public partial class Seeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "30c13631-2048-4ed0-b288-fa46c81a153a", null, "admin", "admin" },
                    { "4eeb1113-7e30-4800-9ea8-11b0eca3d46a", null, "member", "member" },
                    { "d2906bd1-bee3-4fa4-8a9a-e0cd08f18fd4", null, "staff", "staff" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "30c13631-2048-4ed0-b288-fa46c81a153a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4eeb1113-7e30-4800-9ea8-11b0eca3d46a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d2906bd1-bee3-4fa4-8a9a-e0cd08f18fd4");
        }
    }
}
