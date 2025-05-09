using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cutify.Migrations
{
    /// <inheritdoc />
    public partial class NewUpdateReservationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReservationDateTime",
                table: "Reservations",
                newName: "ReservationTime");

            migrationBuilder.RenameColumn(
                name: "CustomerName",
                table: "Reservations",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "Barber",
                table: "Reservations",
                newName: "BarberFullName");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTime",
                table: "Reservations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "SoftDelete",
                table: "Reservations",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "SoftDelete",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "ReservationTime",
                table: "Reservations",
                newName: "ReservationDateTime");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Reservations",
                newName: "CustomerName");

            migrationBuilder.RenameColumn(
                name: "BarberFullName",
                table: "Reservations",
                newName: "Barber");
        }
    }
}
