using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareFusion.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddClinicSiteIdToPatients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClinicSiteId",
                table: "Patients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_ClinicSiteId",
                table: "Patients",
                column: "ClinicSiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_ClinicSites_ClinicSiteId",
                table: "Patients",
                column: "ClinicSiteId",
                principalTable: "ClinicSites",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_ClinicSites_ClinicSiteId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_ClinicSiteId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ClinicSiteId",
                table: "Patients");
        }
    }
}