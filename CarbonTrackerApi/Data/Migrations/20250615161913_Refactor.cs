using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarbonTrackerApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class Refactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VALORFATOR",
                table: "FATORESEMISSAO",
                newName: "VALOREMISSAO");

            migrationBuilder.RenameColumn(
                name: "TIPOENERGIA",
                table: "FATORESEMISSAO",
                newName: "TIPOMEDIDOR");

            migrationBuilder.RenameColumn(
                name: "ANOVIGENCIA",
                table: "FATORESEMISSAO",
                newName: "DATAINICIO");

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAFIM",
                table: "FATORESEMISSAO",
                type: "TIMESTAMP(7)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DATAFIM",
                table: "FATORESEMISSAO");

            migrationBuilder.RenameColumn(
                name: "VALOREMISSAO",
                table: "FATORESEMISSAO",
                newName: "VALORFATOR");

            migrationBuilder.RenameColumn(
                name: "TIPOMEDIDOR",
                table: "FATORESEMISSAO",
                newName: "TIPOENERGIA");

            migrationBuilder.RenameColumn(
                name: "DATAINICIO",
                table: "FATORESEMISSAO",
                newName: "ANOVIGENCIA");
        }
    }
}
