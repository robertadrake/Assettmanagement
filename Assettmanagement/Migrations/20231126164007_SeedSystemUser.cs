using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assettmanagement.Migrations
{
    /// <inheritdoc />
    public partial class SeedSystemUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FirstName", "IsAdministrator", "LastName", "PasswordHash" },
                values: new object[] { 1, "system@home.com", "System", true, "System", "65e84be33532fb784c48129675f9eff3a682b27168c0ea744b2cf58ee02337c5" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
