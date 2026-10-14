using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modelo.Migrations
{
    /// <summary>
    /// Permiso VerHistorialCajas (historial de cajas cerradas). Sólo datos: no cambia el esquema.
    /// No se asigna a ningún grupo: el historial muestra los faltantes de todos los cajeros, así que lo habilita
    /// un administrador desde Grupos. Los administradores ya lo tienen (el grupo Administrador tiene todos los permisos).
    /// </summary>
    public partial class PermisoHistorialCajas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE @form int = (SELECT TOP 1 FORM_ID FROM Formularios WHERE FORM_Nombre = N'FrmGestionarVentas');
IF NOT EXISTS (SELECT 1 FROM Acciones WHERE ACC_Nombre = N'VerHistorialCajas')
    INSERT INTO Acciones (ACC_Nombre, FORM_ID) VALUES (N'VerHistorialCajas', @form);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM AccionGrupo WHERE AccionesACC_ID IN (SELECT ACC_ID FROM Acciones WHERE ACC_Nombre = N'VerHistorialCajas');
DELETE FROM AccionUsuario WHERE AccionesACC_ID IN (SELECT ACC_ID FROM Acciones WHERE ACC_Nombre = N'VerHistorialCajas');
DELETE FROM Acciones WHERE ACC_Nombre = N'VerHistorialCajas';");
        }
    }
}
