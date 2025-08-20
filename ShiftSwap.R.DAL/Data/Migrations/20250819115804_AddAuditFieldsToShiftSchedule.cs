using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSwap.R.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFieldsToShiftSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ShiftSchedules",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "ShiftSchedules",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ShiftSchedules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "ShiftSchedules",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ShiftSchedules");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ShiftSchedules");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ShiftSchedules");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "ShiftSchedules");
        }
    }
}
