using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetOS.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_PET",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Species = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Breed = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Gender = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    Weight = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PET", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_ROUTINE_RECORD",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PetId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    Type = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    Date = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Notes = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_ROUTINE_RECORD", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_ROUTINE_RECORD_T_PET_PetId",
                        column: x => x.PetId,
                        principalTable: "T_PET",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_VACCINE",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PetId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Manufacturer = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    ApplicationDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    NextDueDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Dose = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    CreateAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_VACCINE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_VACCINE_T_PET_PetId",
                        column: x => x.PetId,
                        principalTable: "T_PET",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PetId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    VaccineId = table.Column<long>(type: "NUMBER(19)", nullable: true),
                    Message = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    AlertDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    IsRead = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alerts_T_PET_PetId",
                        column: x => x.PetId,
                        principalTable: "T_PET",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Alerts_T_VACCINE_VaccineId",
                        column: x => x.VaccineId,
                        principalTable: "T_VACCINE",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_PetId",
                table: "Alerts",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_VaccineId",
                table: "Alerts",
                column: "VaccineId");

            migrationBuilder.CreateIndex(
                name: "IX_T_ROUTINE_RECORD_PetId",
                table: "T_ROUTINE_RECORD",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_T_VACCINE_PetId",
                table: "T_VACCINE",
                column: "PetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "T_ROUTINE_RECORD");

            migrationBuilder.DropTable(
                name: "T_VACCINE");

            migrationBuilder.DropTable(
                name: "T_PET");
        }
    }
}
