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
                name: "PETS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    Especie = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false),
                    Raca = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: true),
                    DataNascimento = table.Column<string>(type: "NVARCHAR2(10)", nullable: true),
                    ResponsavelNome = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    ResponsavelTelefone = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PETS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ROTINAS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PetId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Titulo = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(400)", maxLength: 400, nullable: true),
                    Horario = table.Column<string>(type: "NVARCHAR2(48)", nullable: false),
                    Tipo = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Frequencia = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    Ativa = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROTINAS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ROTINAS_PETS_PetId",
                        column: x => x.PetId,
                        principalTable: "PETS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VACINAS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PetId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Nome = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    DataAplicacao = table.Column<string>(type: "NVARCHAR2(10)", nullable: false),
                    ProximaDose = table.Column<string>(type: "NVARCHAR2(10)", nullable: true),
                    Observacoes = table.Column<string>(type: "NVARCHAR2(400)", maxLength: 400, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VACINAS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VACINAS_PETS_PetId",
                        column: x => x.PetId,
                        principalTable: "PETS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ALERTAS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PetId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    RotinaId = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Titulo = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    Mensagem = table.Column<string>(type: "NVARCHAR2(600)", maxLength: 600, nullable: false),
                    DataAlerta = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Status = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ALERTAS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ALERTAS_PETS_PetId",
                        column: x => x.PetId,
                        principalTable: "PETS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ALERTAS_ROTINAS_RotinaId",
                        column: x => x.RotinaId,
                        principalTable: "ROTINAS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ALERTAS_PetId",
                table: "ALERTAS",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_ALERTAS_RotinaId",
                table: "ALERTAS",
                column: "RotinaId");

            migrationBuilder.CreateIndex(
                name: "IX_ROTINAS_PetId",
                table: "ROTINAS",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_VACINAS_PetId",
                table: "VACINAS",
                column: "PetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ALERTAS");

            migrationBuilder.DropTable(
                name: "VACINAS");

            migrationBuilder.DropTable(
                name: "ROTINAS");

            migrationBuilder.DropTable(
                name: "PETS");
        }
    }
}
