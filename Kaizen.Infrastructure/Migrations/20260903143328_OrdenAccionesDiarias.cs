using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kaizen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrdenAccionesDiarias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Orden",
                table: "AccionProgramada",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Orden",
                table: "AccionProgramada");
        }
    }
}
