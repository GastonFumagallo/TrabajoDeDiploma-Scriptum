using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modelo.Migrations
{
    /// <summary>
    /// Seguridad de cuentas de usuario. Revisada a mano (ver docs/GUIA-ABM.md): EF proponía renombrar la FK sombra
    /// USU_PersonaPER_ID a USU_Version, lo que habría perdido el vínculo de cada usuario con su Persona.
    /// </summary>
    public partial class GestionUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditoriaSesiones_Usuarios_AS_UsuarioUSU_ID",
                table: "AuditoriaSesiones");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Personas_USU_PersonaPER_ID",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_USU_PersonaPER_ID",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_AuditoriaSesiones_AS_UsuarioUSU_ID",
                table: "AuditoriaSesiones");

            // Las relaciones reales estaban en columnas sombra; PER_ID y AS_USU_ID tenían valores sin sentido
            // (AS_USU_ID guardaba el PER_ID del usuario). Se copian antes de borrar las sombra.
            migrationBuilder.Sql("UPDATE Usuarios SET PER_ID = USU_PersonaPER_ID;");
            migrationBuilder.Sql("UPDATE AuditoriaSesiones SET AS_USU_ID = AS_UsuarioUSU_ID;");

            migrationBuilder.DropColumn(
                name: "AS_UsuarioUSU_ID",
                table: "AuditoriaSesiones");

            migrationBuilder.DropColumn(
                name: "USU_PersonaPER_ID",
                table: "Usuarios");

            // El formato PBKDF2 ("pbkdf2-sha256$600000$sal$hash") ocupa ~90 caracteres.
            migrationBuilder.AlterColumn<string>(
                name: "USU_Clave",
                table: "Usuarios",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AddColumn<int>(
                name: "USU_Version",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "USU_BloqueadoHasta",
                table: "Usuarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "USU_CodigoRecuperacion",
                table: "Usuarios",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "USU_CodigoVence",
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

            migrationBuilder.AddColumn<DateTime>(
                name: "USU_UltimoAcceso",
                table: "Usuarios",
                type: "datetime2",
                nullable: true);

            // Las claves existentes son SHA-256 sin sal y, en general, temporales de 5 dígitos que nunca se
            // obligó a cambiar: se pide cambiarlas en el próximo ingreso (el hash se migra a PBKDF2 en ese login).
            // EXEC: la columna se agregó en esta misma migración y el script puede compilar todo el lote junto.
            migrationBuilder.Sql("EXEC(N'UPDATE Usuarios SET USU_DebeCambiarClave = 1 WHERE LEN(USU_Clave) = 64;');");

            // Estados de usuario de sistema (Estado_Usuario.Activo / Inactivo). Hasta ahora sólo existía "Activo".
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM Estados_Usuarios WHERE EST_USU_Nombre = N'Activo')
    INSERT INTO Estados_Usuarios (EST_USU_Nombre) VALUES (N'Activo');
IF NOT EXISTS (SELECT 1 FROM Estados_Usuarios WHERE EST_USU_Nombre = N'Inactivo')
    INSERT INTO Estados_Usuarios (EST_USU_Nombre) VALUES (N'Inactivo');
UPDATE Usuarios SET EST_USU_ID = (SELECT EST_USU_ID FROM Estados_Usuarios WHERE EST_USU_Nombre = N'Activo') WHERE EST_USU_ID IS NULL;");

            // Usuario y email únicos: sin espacios sobrantes y sin duplicados antes de los índices. El de menor ID
            // conserva el valor; los demás quedan marcados para corregirlos desde la gestión de usuarios.
            migrationBuilder.Sql("UPDATE Usuarios SET USU_Nombre = LTRIM(RTRIM(USU_Nombre)), USU_Mail = LTRIM(RTRIM(USU_Mail));");
            migrationBuilder.Sql(@"
UPDATE u
SET USU_Nombre = LEFT(u.USU_Nombre, 60 - LEN('_' + CAST(u.USU_ID AS nvarchar(10)))) + '_' + CAST(u.USU_ID AS nvarchar(10))
FROM Usuarios u
WHERE EXISTS (SELECT 1 FROM Usuarios o WHERE o.USU_Nombre = u.USU_Nombre AND o.USU_ID < u.USU_ID);");
            migrationBuilder.Sql(@"
UPDATE u
SET USU_Mail = LEFT('duplicado-' + CAST(u.USU_ID AS nvarchar(10)) + '-' + u.USU_Mail, 60)
FROM Usuarios u
WHERE EXISTS (SELECT 1 FROM Usuarios o WHERE o.USU_Mail = u.USU_Mail AND o.USU_ID < u.USU_ID);");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_PER_ID",
                table: "Usuarios",
                column: "PER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_USU_Mail",
                table: "Usuarios",
                column: "USU_Mail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_USU_Nombre",
                table: "Usuarios",
                column: "USU_Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaSesiones_AS_USU_ID",
                table: "AuditoriaSesiones",
                column: "AS_USU_ID");

            // Restrict (antes Cascade): borrar un usuario se llevaba su historial de sesiones.
            migrationBuilder.AddForeignKey(
                name: "FK_AuditoriaSesiones_Usuarios_AS_USU_ID",
                table: "AuditoriaSesiones",
                column: "AS_USU_ID",
                principalTable: "Usuarios",
                principalColumn: "USU_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Personas_PER_ID",
                table: "Usuarios",
                column: "PER_ID",
                principalTable: "Personas",
                principalColumn: "PER_ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditoriaSesiones_Usuarios_AS_USU_ID",
                table: "AuditoriaSesiones");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Personas_PER_ID",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_PER_ID",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_USU_Mail",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_USU_Nombre",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_AuditoriaSesiones_AS_USU_ID",
                table: "AuditoriaSesiones");

            migrationBuilder.DropColumn(name: "USU_BloqueadoHasta", table: "Usuarios");
            migrationBuilder.DropColumn(name: "USU_CodigoRecuperacion", table: "Usuarios");
            migrationBuilder.DropColumn(name: "USU_CodigoVence", table: "Usuarios");
            migrationBuilder.DropColumn(name: "USU_DebeCambiarClave", table: "Usuarios");
            migrationBuilder.DropColumn(name: "USU_IntentosFallidos", table: "Usuarios");
            migrationBuilder.DropColumn(name: "USU_UltimoAcceso", table: "Usuarios");
            migrationBuilder.DropColumn(name: "USU_Version", table: "Usuarios");

            // USU_Clave queda en nvarchar(200): los hashes PBKDF2 no entran en 64 caracteres y el código anterior
            // no los puede verificar. Después de volver atrás hay que blanquear las claves de esos usuarios.

            migrationBuilder.AddColumn<int>(
                name: "USU_PersonaPER_ID",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AS_UsuarioUSU_ID",
                table: "AuditoriaSesiones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE Usuarios SET USU_PersonaPER_ID = PER_ID;");
            migrationBuilder.Sql("UPDATE AuditoriaSesiones SET AS_UsuarioUSU_ID = AS_USU_ID;");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_USU_PersonaPER_ID",
                table: "Usuarios",
                column: "USU_PersonaPER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaSesiones_AS_UsuarioUSU_ID",
                table: "AuditoriaSesiones",
                column: "AS_UsuarioUSU_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditoriaSesiones_Usuarios_AS_UsuarioUSU_ID",
                table: "AuditoriaSesiones",
                column: "AS_UsuarioUSU_ID",
                principalTable: "Usuarios",
                principalColumn: "USU_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Personas_USU_PersonaPER_ID",
                table: "Usuarios",
                column: "USU_PersonaPER_ID",
                principalTable: "Personas",
                principalColumn: "PER_ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
