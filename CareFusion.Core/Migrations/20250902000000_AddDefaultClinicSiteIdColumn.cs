using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareFusion.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultClinicSiteIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add DefaultClinicSiteId column to Users table
            migrationBuilder.AddColumn<int>(
                name: "DefaultClinicSiteId",
                table: "Users",
                type: "int",
                nullable: true);

            // Create foreign key constraint
            migrationBuilder.CreateIndex(
                name: "IX_Users_DefaultClinicSiteId",
                table: "Users",
                column: "DefaultClinicSiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_ClinicSites_DefaultClinicSiteId",
                table: "Users",
                column: "DefaultClinicSiteId",
                principalTable: "ClinicSites",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK_Users_ClinicSites_DefaultClinicSiteId",
                table: "Users");

            // Drop index
            migrationBuilder.DropIndex(
                name: "IX_Users_DefaultClinicSiteId",
                table: "Users");

            // Drop column
            migrationBuilder.DropColumn(
                name: "DefaultClinicSiteId",
                table: "Users");
        }
    }
}