using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assettmanagement.Migrations
{
    public partial class UpdateIdentitySetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserInfoId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserInfoId",
                table: "AspNetUsers",
                column: "UserInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Users_UserInfoId",
                table: "AspNetUsers",
                column: "UserInfoId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Users_UserInfoId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserInfoId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserInfoId",
                table: "AspNetUsers");
        }
    }
}
