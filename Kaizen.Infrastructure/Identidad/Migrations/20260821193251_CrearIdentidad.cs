using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kaizen.Infrastructure.Identidad.Migrations
{
    /// <inheritdoc />
    public partial class CrearIdentidad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NombreNormalizado = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    SelloConcurrencia = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DebeCambiarClave = table.Column<bool>(type: "bit", nullable: false),
                    NombreUsuario = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NombreUsuarioNormalizado = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Correo = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CorreoNormalizado = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CorreoConfirmado = table.Column<bool>(type: "bit", nullable: false),
                    HashClave = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelloSeguridad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelloConcurrencia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TelefonoConfirmado = table.Column<bool>(type: "bit", nullable: false),
                    DobleFactorHabilitado = table.Column<bool>(type: "bit", nullable: false),
                    FinBloqueo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    BloqueoHabilitado = table.Column<bool>(type: "bit", nullable: false),
                    IntentosFallidos = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeclaracionRol",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RolId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Valor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeclaracionRol", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeclaracionRol_Rol_RolId",
                        column: x => x.RolId,
                        principalTable: "Rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeclaracionUsuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Valor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeclaracionUsuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeclaracionUsuario_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InicioSesionExterno",
                columns: table => new
                {
                    Proveedor = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaveProveedor = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NombreProveedor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InicioSesionExterno", x => new { x.Proveedor, x.ClaveProveedor });
                    table.ForeignKey(
                        name: "FK_InicioSesionExterno_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TokenUsuario",
                columns: table => new
                {
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Proveedor = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenUsuario", x => new { x.UsuarioId, x.Proveedor, x.Nombre });
                    table.ForeignKey(
                        name: "FK_TokenUsuario_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioRol",
                columns: table => new
                {
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RolId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioRol", x => new { x.UsuarioId, x.RolId });
                    table.ForeignKey(
                        name: "FK_UsuarioRol_Rol_RolId",
                        column: x => x.RolId,
                        principalTable: "Rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioRol_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Usuario",
                columns: new[] { "Id", "IntentosFallidos", "SelloConcurrencia", "DebeCambiarClave", "Correo", "CorreoConfirmado", "BloqueoHabilitado", "FinBloqueo", "CorreoNormalizado", "NombreUsuarioNormalizado", "HashClave", "Telefono", "TelefonoConfirmado", "SelloSeguridad", "DobleFactorHabilitado", "NombreUsuario" },
                values: new object[] { "00000000-0000-0000-0000-000000000001", 0, "CUENTA-INICIAL-KAIZEN", true, "agustin@kaizen.local", true, false, null, "AGUSTIN@KAIZEN.LOCAL", "AGUSTIN@KAIZEN.LOCAL", null, null, false, "CUENTA-INICIAL-KAIZEN", false, "agustin@kaizen.local" });

            migrationBuilder.CreateIndex(
                name: "IX_DeclaracionRol_RolId",
                table: "DeclaracionRol",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_DeclaracionUsuario_UsuarioId",
                table: "DeclaracionUsuario",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_InicioSesionExterno_UsuarioId",
                table: "InicioSesionExterno",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Rol",
                column: "NombreNormalizado",
                unique: true,
                filter: "[NombreNormalizado] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Usuario",
                column: "CorreoNormalizado");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Usuario",
                column: "NombreUsuarioNormalizado",
                unique: true,
                filter: "[NombreUsuarioNormalizado] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRol_RolId",
                table: "UsuarioRol",
                column: "RolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeclaracionRol");

            migrationBuilder.DropTable(
                name: "DeclaracionUsuario");

            migrationBuilder.DropTable(
                name: "InicioSesionExterno");

            migrationBuilder.DropTable(
                name: "TokenUsuario");

            migrationBuilder.DropTable(
                name: "UsuarioRol");

            migrationBuilder.DropTable(
                name: "Rol");

            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}
