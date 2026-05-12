using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetOS.Migrations
{
    /// <inheritdoc />
    public partial class RenameAlertTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_T_PET_PetId",
                table: "Alerts");

            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_T_VACCINE_VaccineId",
                table: "Alerts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Alerts",
                table: "Alerts");

            migrationBuilder.RenameTable(
                name: "Alerts",
                newName: "T_ALERT");

            migrationBuilder.RenameIndex(
                name: "IX_Alerts_VaccineId",
                table: "T_ALERT",
                newName: "IX_T_ALERT_VaccineId");

            migrationBuilder.RenameIndex(
                name: "IX_Alerts_PetId",
                table: "T_ALERT",
                newName: "IX_T_ALERT_PetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_ALERT",
                table: "T_ALERT",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_T_ALERT_T_PET_PetId",
                table: "T_ALERT",
                column: "PetId",
                principalTable: "T_PET",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_T_ALERT_T_VACCINE_VaccineId",
                table: "T_ALERT",
                column: "VaccineId",
                principalTable: "T_VACCINE",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_ALERT_T_PET_PetId",
                table: "T_ALERT");

            migrationBuilder.DropForeignKey(
                name: "FK_T_ALERT_T_VACCINE_VaccineId",
                table: "T_ALERT");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_ALERT",
                table: "T_ALERT");

            migrationBuilder.RenameTable(
                name: "T_ALERT",
                newName: "Alerts");

            migrationBuilder.RenameIndex(
                name: "IX_T_ALERT_VaccineId",
                table: "Alerts",
                newName: "IX_Alerts_VaccineId");

            migrationBuilder.RenameIndex(
                name: "IX_T_ALERT_PetId",
                table: "Alerts",
                newName: "IX_Alerts_PetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Alerts",
                table: "Alerts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_T_PET_PetId",
                table: "Alerts",
                column: "PetId",
                principalTable: "T_PET",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_T_VACCINE_VaccineId",
                table: "Alerts",
                column: "VaccineId",
                principalTable: "T_VACCINE",
                principalColumn: "Id");
        }
    }
}
