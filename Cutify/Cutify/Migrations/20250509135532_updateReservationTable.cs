using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cutify.Migrations
{
    /// <inheritdoc />
    public partial class updateReservationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Users_BarberId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_BarberId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "DateTime",
                table: "Reservations",
                newName: "ReservationDateTime");

            migrationBuilder.AddColumn<string>(
                name: "Barber",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Barber",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "ReservationDateTime",
                table: "Reservations",
                newName: "DateTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Reservations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_BarberId",
                table: "Reservations",
                column: "BarberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Users_BarberId",
                table: "Reservations",
                column: "BarberId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
