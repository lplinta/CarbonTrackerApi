using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarbonTrackerApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EDIFICIOS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    ENDERECO = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    TIPOEDIFICIO = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    LATITUDE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    LONGITUDE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EDIFICIOS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FATORESEMISSAO",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TIPOENERGIA = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    UNIDADEEMISSAO = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    VALORFATOR = table.Column<decimal>(type: "decimal(10,6)", nullable: false),
                    REGIAO = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    ANOVIGENCIA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FATORESEMISSAO", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MEDIDORESENERGIA",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NUMEROSERIE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    TIPOMEDIDOR = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    LOCALIZACAO = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    EDIFICIOID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MEDIDORESENERGIA", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MEDIDORESENERGIA_EDIFICIOS_EDIFICIOID",
                        column: x => x.EDIFICIOID,
                        principalTable: "EDIFICIOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "METASCARBONO",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ANOMETA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    REDUCAOPERCENTUAL = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    ANOBASE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DATACRIACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    EDIFICIOID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_METASCARBONO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_METASCARBONO_EDIFICIOS_EDIFICIOID",
                        column: x => x.EDIFICIOID,
                        principalTable: "EDIFICIOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MEDICOESENERGIA",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    CONSUMOVALOR = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    UNIDADEMEDIDA = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    TIMESTAMP = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    MEDIDORENERGIAID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MEDICOESENERGIA", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MEDICOESENERGIA_MEDIDORESENERGIA_MEDIDORENERGIAID",
                        column: x => x.MEDIDORENERGIAID,
                        principalTable: "MEDIDORESENERGIA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MEDICOESENERGIA_MEDIDORENERGIAID",
                table: "MEDICOESENERGIA",
                column: "MEDIDORENERGIAID");

            migrationBuilder.CreateIndex(
                name: "IX_MEDIDORESENERGIA_EDIFICIOID",
                table: "MEDIDORESENERGIA",
                column: "EDIFICIOID");

            migrationBuilder.CreateIndex(
                name: "IX_MEDIDORESENERGIA_NUMEROSERIE",
                table: "MEDIDORESENERGIA",
                column: "NUMEROSERIE",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_METASCARBONO_EDIFICIOID",
                table: "METASCARBONO",
                column: "EDIFICIOID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FATORESEMISSAO");

            migrationBuilder.DropTable(
                name: "MEDICOESENERGIA");

            migrationBuilder.DropTable(
                name: "METASCARBONO");

            migrationBuilder.DropTable(
                name: "MEDIDORESENERGIA");

            migrationBuilder.DropTable(
                name: "EDIFICIOS");
        }
    }
}
