using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kaizen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AsociarMetasAUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "Meta",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "00000000-0000-0000-0000-000000000001");

            migrationBuilder.CreateIndex(
                name: "IX_Meta_UsuarioId",
                table: "Meta",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Meta_Usuario_UsuarioId",
                table: "Meta",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meta_Usuario_UsuarioId",
                table: "Meta");

            migrationBuilder.DropIndex(
                name: "IX_Meta_UsuarioId",
                table: "Meta");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Meta");
        }
    }
}
