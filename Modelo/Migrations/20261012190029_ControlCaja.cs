using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modelo.Migrations
{
    /// <inheritdoc />
    public partial class ControlCaja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CAJ_ID",
                table: "Ventas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Cajas",
                columns: table => new
                {
                    CAJ_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CAJ_Estado = table.Column<int>(type: "int", nullable: false),
                    CAJ_FechaApertura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CAJ_UsuarioAperturaId = table.Column<int>(type: "int", nullable: false),
                    CAJ_MontoInicial = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CAJ_FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CAJ_UsuarioCierreId = table.Column<int>(type: "int", nullable: true),
                    CAJ_CantidadVentas = table.Column<int>(type: "int", nullable: true),
                    CAJ_TotalVentas = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CAJ_EfectivoEsperado = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CAJ_EfectivoContado = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CAJ_Diferencia = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CAJ_Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cajas", x => x.CAJ_ID);
                    table.CheckConstraint("CK_Cajas_Cierre", "([CAJ_Estado] = 0 AND [CAJ_FechaCierre] IS NULL AND [CAJ_UsuarioCierreId] IS NULL) OR ([CAJ_Estado] = 1 AND [CAJ_FechaCierre] IS NOT NULL AND [CAJ_UsuarioCierreId] IS NOT NULL AND [CAJ_EfectivoContado] IS NOT NULL)");
                    table.CheckConstraint("CK_Cajas_EfectivoContado", "[CAJ_EfectivoContado] IS NULL OR [CAJ_EfectivoContado] >= 0");
                    table.CheckConstraint("CK_Cajas_MontoInicial", "[CAJ_MontoInicial] >= 0");
                    table.ForeignKey(
                        name: "FK_Cajas_Usuarios_CAJ_UsuarioAperturaId",
                        column: x => x.CAJ_UsuarioAperturaId,
                        principalTable: "Usuarios",
                        principalColumn: "USU_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cajas_Usuarios_CAJ_UsuarioCierreId",
                        column: x => x.CAJ_UsuarioCierreId,
                        principalTable: "Usuarios",
                        principalColumn: "USU_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CajaTotalesMedio",
                columns: table => new
                {
                    CTM_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CAJ_ID = table.Column<int>(type: "int", nullable: false),
                    MP_ID = table.Column<int>(type: "int", nullable: false),
                    CTM_MedioPago = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    CTM_EsEfectivo = table.Column<bool>(type: "bit", nullable: false),
                    CTM_Cantidad = table.Column<int>(type: "int", nullable: false),
                    CTM_Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CajaTotalesMedio", x => x.CTM_ID);
                    table.ForeignKey(
                        name: "FK_CajaTotalesMedio_Cajas_CAJ_ID",
                        column: x => x.CAJ_ID,
                        principalTable: "Cajas",
                        principalColumn: "CAJ_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CajaTotalesMedio_MetodosPago_MP_ID",
                        column: x => x.MP_ID,
                        principalTable: "MetodosPago",
                        principalColumn: "MP_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_CAJ_ID",
                table: "Ventas",
                column: "CAJ_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Cajas_CAJ_FechaApertura",
                table: "Cajas",
                column: "CAJ_FechaApertura");

            migrationBuilder.CreateIndex(
                name: "IX_Cajas_CAJ_UsuarioAperturaId",
                table: "Cajas",
                column: "CAJ_UsuarioAperturaId");

            migrationBuilder.CreateIndex(
                name: "IX_Cajas_CAJ_UsuarioCierreId",
                table: "Cajas",
                column: "CAJ_UsuarioCierreId");

            migrationBuilder.CreateIndex(
                name: "UX_Cajas_UnaAbierta",
                table: "Cajas",
                column: "CAJ_Estado",
                unique: true,
                filter: "[CAJ_Estado] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CajaTotalesMedio_CAJ_ID_MP_ID",
                table: "CajaTotalesMedio",
                columns: new[] { "CAJ_ID", "MP_ID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CajaTotalesMedio_MP_ID",
                table: "CajaTotalesMedio",
                column: "MP_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Cajas_CAJ_ID",
                table: "Ventas",
                column: "CAJ_ID",
                principalTable: "Cajas",
                principalColumn: "CAJ_ID",
                onDelete: ReferentialAction.Restrict);

            // Permisos nuevos, colgados del formulario de Ventas. Se les dan a los grupos que ya pueden vender,
            // para que después de actualizar los cajeros puedan abrir su caja (si no, nadie podría vender hasta
            // que un administrador la abra). El grupo Administrador los tiene igual por su acceso total.
            migrationBuilder.Sql(@"
DECLARE @form int = (SELECT TOP 1 FORM_ID FROM Formularios WHERE FORM_Nombre = N'FrmGestionarVentas');
IF NOT EXISTS (SELECT 1 FROM Acciones WHERE ACC_Nombre = N'AbrirCaja')
    INSERT INTO Acciones (ACC_Nombre, FORM_ID) VALUES (N'AbrirCaja', @form);
IF NOT EXISTS (SELECT 1 FROM Acciones WHERE ACC_Nombre = N'CerrarCaja')
    INSERT INTO Acciones (ACC_Nombre, FORM_ID) VALUES (N'CerrarCaja', @form);

INSERT INTO AccionGrupo (AccionesACC_ID, GruposGRU_ID)
SELECT nueva.ACC_ID, ag.GruposGRU_ID
FROM AccionGrupo ag
JOIN Acciones vender ON vender.ACC_ID = ag.AccionesACC_ID AND vender.ACC_Nombre = N'RealizarVenta'
CROSS JOIN Acciones nueva
WHERE nueva.ACC_Nombre IN (N'AbrirCaja', N'CerrarCaja')
  AND NOT EXISTS (SELECT 1 FROM AccionGrupo x WHERE x.AccionesACC_ID = nueva.ACC_ID AND x.GruposGRU_ID = ag.GruposGRU_ID);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM AccionGrupo WHERE AccionesACC_ID IN (SELECT ACC_ID FROM Acciones WHERE ACC_Nombre IN (N'AbrirCaja', N'CerrarCaja'));
DELETE FROM AccionUsuario WHERE AccionesACC_ID IN (SELECT ACC_ID FROM Acciones WHERE ACC_Nombre IN (N'AbrirCaja', N'CerrarCaja'));
DELETE FROM Acciones WHERE ACC_Nombre IN (N'AbrirCaja', N'CerrarCaja');");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Cajas_CAJ_ID",
                table: "Ventas");

            migrationBuilder.DropTable(
                name: "CajaTotalesMedio");

            migrationBuilder.DropTable(
                name: "Cajas");

            migrationBuilder.DropIndex(
                name: "IX_Ventas_CAJ_ID",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "CAJ_ID",
                table: "Ventas");
        }
    }
}
