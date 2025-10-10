using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdsetManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVehicleImageToUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "VehicleImages");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "VehicleImages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "VehicleImages");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "VehicleImages",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
