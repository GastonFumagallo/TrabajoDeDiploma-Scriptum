using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modelo.Migrations
{
    /// <summary>
    /// Gestión de usuarios. Revisada a mano (ver docs/GUIA-ABM.md): EF proponía renombrar la FK sombra
    /// USU_PersonaPER_ID a USU_Version, lo que habría perdido el vínculo de cada usuario con su Persona.
    /// Los UPDATE que usan columnas creadas en esta misma migración van en EXEC(...): el script de EF puede
    /// compilar varias sentencias en un mismo lote.
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

            // Las relaciones reales estaban en columnas sombra: PER_ID podía valer 0 y AS_USU_ID se llenaba aparte.
            // Se copian antes de borrar las sombra.
            migrationBuilder.Sql("UPDATE Usuarios SET PER_ID = USU_PersonaPER_ID;");
            migrationBuilder.Sql("UPDATE AuditoriaSesiones SET AS_USU_ID = AS_UsuarioUSU_ID;");

            migrationBuilder.DropColumn(
                name: "AS_UsuarioUSU_ID",
                table: "AuditoriaSesiones");

            migrationBuilder.DropColumn(
                name: "USU_PersonaPER_ID",
                table: "Usuarios");

            migrationBuilder.AddColumn<int>(
                name: "USU_Version",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "USU_UltimoAcceso",
                table: "Usuarios",
                type: "datetime2",
                nullable: true);

            // Email único: sin espacios sobrantes y sin duplicados antes del índice. El de menor ID conserva el
            // email; los demás quedan marcados para corregirlos desde la gestión de usuarios.
            migrationBuilder.Sql("UPDATE Usuarios SET USU_Mail = LTRIM(RTRIM(USU_Mail));");
            migrationBuilder.Sql(@"
UPDATE u
SET USU_Mail = LEFT('duplicado-' + CAST(u.USU_ID AS nvarchar(10)) + '-' + u.USU_Mail, 60)
FROM Usuarios u
WHERE EXISTS (SELECT 1 FROM Usuarios o WHERE o.USU_Mail = u.USU_Mail AND o.USU_ID < u.USU_ID);");

            // Estados de usuario de sistema (Estado_Usuario.Activo / Inactivo).
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM Estados_Usuarios WHERE EST_USU_Nombre = N'Activo')
    INSERT INTO Estados_Usuarios (EST_USU_Nombre) VALUES (N'Activo');
IF NOT EXISTS (SELECT 1 FROM Estados_Usuarios WHERE EST_USU_Nombre = N'Inactivo')
    INSERT INTO Estados_Usuarios (EST_USU_Nombre) VALUES (N'Inactivo');");

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
                name: "IX_AuditoriaSesiones_AS_USU_ID",
                table: "AuditoriaSesiones",
                column: "AS_USU_ID");

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
                name: "IX_AuditoriaSesiones_AS_USU_ID",
                table: "AuditoriaSesiones");

            migrationBuilder.DropColumn(
                name: "USU_UltimoAcceso",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "USU_Version",
                table: "Usuarios");

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

            migrationBuilder.Sql("EXEC(N'UPDATE Usuarios SET USU_PersonaPER_ID = PER_ID;');");
            migrationBuilder.Sql("EXEC(N'UPDATE AuditoriaSesiones SET AS_UsuarioUSU_ID = AS_USU_ID;');");

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
                onDelete: ReferentialAction.Restrict);

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
