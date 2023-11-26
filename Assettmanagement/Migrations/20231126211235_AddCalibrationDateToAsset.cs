using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assettmanagement.Migrations
{
    /// <inheritdoc />
    public partial class AddCalibrationDateToAsset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CalibrationDate",
                table: "Assets",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalibrationDate",
                table: "Assets");
        }
    }
}
