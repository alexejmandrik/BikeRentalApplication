using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeRentalApplication.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingStatusToBikeBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BookingStatus",
                table: "BikeBookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingStatus",
                table: "BikeBookings");
        }
    }
}
