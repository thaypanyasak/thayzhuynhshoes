using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _1721001086_PanyasakKhamkeuth_Week8.Migrations
{
    /// <inheritdoc />
    public partial class _2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ShippingFee",
                table: "HoaDon",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TaxAmount",
                table: "HoaDon",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingFee",
                table: "HoaDon");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "HoaDon");
        }
    }
}
