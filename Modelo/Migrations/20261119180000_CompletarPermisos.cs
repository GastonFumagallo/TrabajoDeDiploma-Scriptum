using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modelo.Migrations
{
    /// <summary>
    /// Sólo datos. Agrega las acciones que el código ya consultaba pero no existían en la tabla (nadie las podía
    /// habilitar desde Grupos) y asigna TODAS las acciones al grupo Administrador. El Administrador ya tenía acceso
    /// total por nombre (PermisoService); así además lo refleja el árbol de permisos de Grupos.
    /// </summary>
    public partial class CompletarPermisos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE @ventas int = (SELECT TOP 1 FORM_ID FROM Formularios WHERE FORM_Nombre = N'FrmGestionarVentas');
DECLARE @inv int = (SELECT TOP 1 FORM_ID FROM Formularios WHERE FORM_Nombre = N'FrmGestionarInventario');
IF NOT EXISTS (SELECT 1 FROM Acciones WHERE ACC_Nombre = N'AnularVenta')        INSERT INTO Acciones (ACC_Nombre, FORM_ID) VALUES (N'AnularVenta', @ventas);
IF NOT EXISTS (SELECT 1 FROM Acciones WHERE ACC_Nombre = N'ExportarVentas')     INSERT INTO Acciones (ACC_Nombre, FORM_ID) VALUES (N'ExportarVentas', @ventas);
IF NOT EXISTS (SELECT 1 FROM Acciones WHERE ACC_Nombre = N'AplicarDescuento')   INSERT INTO Acciones (ACC_Nombre, FORM_ID) VALUES (N'AplicarDescuento', @ventas);
IF NOT EXISTS (SELECT 1 FROM Acciones WHERE ACC_Nombre = N'AjustarStock')       INSERT INTO Acciones (ACC_Nombre, FORM_ID) VALUES (N'AjustarStock', @inv);
IF NOT EXISTS (SELECT 1 FROM Acciones WHERE ACC_Nombre = N'ExportarInventario') INSERT INTO Acciones (ACC_Nombre, FORM_ID) VALUES (N'ExportarInventario', @inv);

DECLARE @admin int = (SELECT TOP 1 GRU_ID FROM Grupos WHERE GRU_Nombre = N'Administrador');
IF @admin IS NOT NULL
    INSERT INTO AccionGrupo (AccionesACC_ID, GruposGRU_ID)
    SELECT a.ACC_ID, @admin FROM Acciones a
    WHERE NOT EXISTS (SELECT 1 FROM AccionGrupo ag WHERE ag.AccionesACC_ID = a.ACC_ID AND ag.GruposGRU_ID = @admin);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Quita sólo las acciones que agregó esta migración (las asignaciones previas del Administrador quedan).
            migrationBuilder.Sql(@"
DELETE FROM AccionGrupo WHERE AccionesACC_ID IN (SELECT ACC_ID FROM Acciones WHERE ACC_Nombre IN (N'AnularVenta', N'ExportarVentas', N'AplicarDescuento', N'AjustarStock', N'ExportarInventario'));
DELETE FROM AccionUsuario WHERE AccionesACC_ID IN (SELECT ACC_ID FROM Acciones WHERE ACC_Nombre IN (N'AnularVenta', N'ExportarVentas', N'AplicarDescuento', N'AjustarStock', N'ExportarInventario'));
DELETE FROM Acciones WHERE ACC_Nombre IN (N'AnularVenta', N'ExportarVentas', N'AplicarDescuento', N'AjustarStock', N'ExportarInventario');");
        }
    }
}