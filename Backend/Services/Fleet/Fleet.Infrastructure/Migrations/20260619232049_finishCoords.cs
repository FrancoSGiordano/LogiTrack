using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fleet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class finishCoords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "FinishLat",
                table: "Trips",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FinishLon",
                table: "Trips",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinishLat",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "FinishLon",
                table: "Trips");
        }
    }
}
