using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarbonTrackerApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class EdificioCidadeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CIDADE",
                table: "EDIFICIOS",
                type: "NVARCHAR2(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CIDADE",
                table: "EDIFICIOS");
        }
    }
}
