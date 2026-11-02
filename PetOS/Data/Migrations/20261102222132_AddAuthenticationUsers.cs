using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetOS.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthenticationUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_PETOS_USER",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(160)", maxLength: 160, nullable: false),
                    PasswordHash = table.Column<string>(type: "NVARCHAR2(512)", maxLength: 512, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PETOS_USER", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_PETOS_USER_Email",
                table: "T_PETOS_USER",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_PETOS_USER");
        }
    }
}
