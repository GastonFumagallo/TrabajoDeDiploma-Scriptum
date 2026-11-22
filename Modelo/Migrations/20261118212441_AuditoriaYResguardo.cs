using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modelo.Migrations
{
    /// <inheritdoc />
    public partial class AuditoriaYResguardo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditoriaLibros",
                columns: table => new
                {
                    AUL_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LIB_ID = table.Column<int>(type: "int", nullable: false),
                    AUL_Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AUL_Usuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AUL_Operacion = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    AUL_Campo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    AUL_ValorAnterior = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AUL_ValorNuevo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AUL_Lote = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AUL_Referencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditoriaLibros", x => x.AUL_ID);
                    table.ForeignKey(
                        name: "FK_AuditoriaLibros_Libros_LIB_ID",
                        column: x => x.LIB_ID,
                        principalTable: "Libros",
                        principalColumn: "LIB_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaLibros_AUL_Fecha",
                table: "AuditoriaLibros",
                column: "AUL_Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaLibros_LIB_ID_AUL_Fecha",
                table: "AuditoriaLibros",
                columns: new[] { "LIB_ID", "AUL_Fecha" });

            // Formularios y permisos de auditoría y de resguardo (módulo de seguridad). Sólo datos.
            // No se asignan a ningún grupo: los habilita un administrador desde Grupos (el grupo Administrador ya tiene todo).
            migrationBuilder.Sql(@"
DECLARE @mod int = (SELECT TOP 1 MOD_ID FROM Modulos WHERE MOD_Nombre = N'ModuloSeguridad');
IF NOT EXISTS (SELECT 1 FROM Formularios WHERE FORM_Nombre = N'FrmAuditoria')
    INSERT INTO Formularios (FORM_Nombre, MOD_ID) VALUES (N'FrmAuditoria', @mod);
IF NOT EXISTS (SELECT 1 FROM Formularios WHERE FORM_Nombre = N'FrmResguardo')
    INSERT INTO Formularios (FORM_Nombre, MOD_ID) VALUES (N'FrmResguardo', @mod);
DECLARE @aud int = (SELECT TOP 1 FORM_ID FROM Formularios WHERE FORM_Nombre = N'FrmAuditoria');
DECLARE @res int = (SELECT TOP 1 FORM_ID FROM Formularios WHERE FORM_Nombre = N'FrmResguardo');
IF NOT EXISTS (SELECT 1 FROM Acciones WHERE ACC_Nombre = N'VerAuditoria')
    INSERT INTO Acciones (ACC_Nombre, FORM_ID) VALUES (N'VerAuditoria', @aud);
IF NOT EXISTS (SELECT 1 FROM Acciones WHERE ACC_Nombre = N'ExportarAuditoria')
    INSERT INTO Acciones (ACC_Nombre, FORM_ID) VALUES (N'ExportarAuditoria', @aud);
IF NOT EXISTS (SELECT 1 FROM Acciones WHERE ACC_Nombre = N'RealizarResguardo')
    INSERT INTO Acciones (ACC_Nombre, FORM_ID) VALUES (N'RealizarResguardo', @res);
IF NOT EXISTS (SELECT 1 FROM Acciones WHERE ACC_Nombre = N'RestaurarResguardo')
    INSERT INTO Acciones (ACC_Nombre, FORM_ID) VALUES (N'RestaurarResguardo', @res);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM AccionGrupo WHERE AccionesACC_ID IN (SELECT ACC_ID FROM Acciones WHERE ACC_Nombre IN (N'VerAuditoria', N'ExportarAuditoria', N'RealizarResguardo', N'RestaurarResguardo'));
DELETE FROM AccionUsuario WHERE AccionesACC_ID IN (SELECT ACC_ID FROM Acciones WHERE ACC_Nombre IN (N'VerAuditoria', N'ExportarAuditoria', N'RealizarResguardo', N'RestaurarResguardo'));
DELETE FROM Acciones WHERE ACC_Nombre IN (N'VerAuditoria', N'ExportarAuditoria', N'RealizarResguardo', N'RestaurarResguardo');
DELETE FROM Formularios WHERE FORM_Nombre IN (N'FrmAuditoria', N'FrmResguardo');");

            migrationBuilder.DropTable(
                name: "AuditoriaLibros");
        }
    }
}
