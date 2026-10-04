using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modelo.Migrations
{
    /// <inheritdoc />
    public partial class SeguridadCredenciales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditoriaSesiones_Usuarios_AS_UsuarioUSU_ID",
                table: "AuditoriaSesiones");

            migrationBuilder.AlterColumn<string>(
                name: "USU_Clave",
                table: "Usuarios",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AddColumn<DateTime>(
                name: "USU_BloqueadoHasta",
                table: "Usuarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "USU_ClaveTemporal",
                table: "Usuarios",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "USU_ClaveTemporalVence",
                table: "Usuarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "USU_DebeCambiarClave",
                table: "Usuarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "USU_IntentosFallidos",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AuditoriaSeguridad",
                columns: table => new
                {
                    AUD_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AUD_Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AUD_Usuario = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    AUD_Evento = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    AUD_Detalle = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    AUD_Equipo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditoriaSeguridad", x => x.AUD_ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_USU_Nombre",
                table: "Usuarios",
                column: "USU_Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaSeguridad_AUD_Fecha",
                table: "AuditoriaSeguridad",
                column: "AUD_Fecha");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditoriaSesiones_Usuarios_AS_UsuarioUSU_ID",
                table: "AuditoriaSesiones",
                column: "AS_UsuarioUSU_ID",
                principalTable: "Usuarios",
                principalColumn: "USU_ID",
                onDelete: ReferentialAction.Restrict);

            // Baja lógica de usuarios: el login y DarDeBajaUsuario usan el estado "Inactivo", que no existía.
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM Estados_Usuarios WHERE EST_USU_Nombre = 'Inactivo')
    INSERT INTO Estados_Usuarios (EST_USU_Nombre) VALUES ('Inactivo');");

            // AS_USU_ID guardaba el ID de la persona en lugar del ID del usuario: se corrige con la FK real.
            migrationBuilder.Sql("UPDATE AuditoriaSesiones SET AS_USU_ID = AS_UsuarioUSU_ID;");

            // Las claves existentes (SHA-256 sin salt) siguen siendo válidas y se convierten a PBKDF2
            // automáticamente en el próximo login de cada usuario (ver HasherClaves).
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditoriaSesiones_Usuarios_AS_UsuarioUSU_ID",
                table: "AuditoriaSesiones");

            migrationBuilder.DropTable(
                name: "AuditoriaSeguridad");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_USU_Nombre",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "USU_BloqueadoHasta",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "USU_ClaveTemporal",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "USU_ClaveTemporalVence",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "USU_DebeCambiarClave",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "USU_IntentosFallidos",
                table: "Usuarios");

            migrationBuilder.AlterColumn<string>(
                name: "USU_Clave",
                table: "Usuarios",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditoriaSesiones_Usuarios_AS_UsuarioUSU_ID",
                table: "AuditoriaSesiones",
                column: "AS_UsuarioUSU_ID",
                principalTable: "Usuarios",
                principalColumn: "USU_ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
