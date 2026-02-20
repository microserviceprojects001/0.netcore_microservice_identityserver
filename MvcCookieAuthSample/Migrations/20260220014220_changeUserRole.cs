using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcCookieAuthSample.Migrations
{
    /// <inheritdoc />
    public partial class changeUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NickName1",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "role1",
                table: "AspNetRoles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NickName1",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "role1",
                table: "AspNetRoles");
        }
    }
}
