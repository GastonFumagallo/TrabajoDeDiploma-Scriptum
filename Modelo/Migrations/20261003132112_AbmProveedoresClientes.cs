using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modelo.Migrations
{
    /// <summary>
    /// ABM de proveedores y clientes.
    ///
    /// IMPORTANTE (revisada a mano): EF generó un RenameColumn de las columnas FK sombra
    /// (PER_ProveedorPER_ID / CLI_PersonaPER_ID) a PROV_Version / CLI_Version por ser ambas int, lo que habría
    /// convertido los vínculos con Persona en "versiones" y perdido la relación. Se reemplazó por:
    /// copiar la FK sombra a PER_ID → crear las columnas de versión → borrar las columnas sombra.
    /// También se corrigieron los valores por defecto (Activo = true, TipoDocumento = 'DNI') y se agregó
    /// la migración de datos (documento de clientes, Consumidor Final, duplicados).
    /// </summary>
    public partial class AbmProveedoresClientes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ---- 1. FK reales hacia Persona: PER_ID pasa a ser la FK (antes EF usaba columnas sombra) ----
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Personas_CLI_PersonaPER_ID",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Proveedores_Personas_PER_ProveedorPER_ID",
                table: "Proveedores");

            migrationBuilder.DropIndex(
                name: "IX_Proveedores_PER_ProveedorPER_ID",
                table: "Proveedores");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_CLI_PersonaPER_ID",
                table: "Clientes");

            migrationBuilder.Sql("UPDATE Proveedores SET PER_ID = PER_ProveedorPER_ID;");
            migrationBuilder.Sql("UPDATE Clientes SET PER_ID = CLI_PersonaPER_ID;");

            migrationBuilder.DropColumn(
                name: "PER_ProveedorPER_ID",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "CLI_PersonaPER_ID",
                table: "Clientes");

            // ---- 2. Columnas nuevas de Proveedores ----
            migrationBuilder.AddColumn<int>(
                name: "PROV_Version",
                table: "Proveedores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "PROV_Activo",
                table: "Proveedores",
                type: "bit",
                nullable: false,
                defaultValue: true);   // los proveedores existentes quedan activos

            migrationBuilder.AddColumn<string>(
                name: "PROV_CUIT",
                table: "Proveedores",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PROV_CondicionFiscal",
                table: "Proveedores",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PROV_Direccion",
                table: "Proveedores",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PROV_Observaciones",
                table: "Proveedores",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            // ---- 3. Columnas nuevas de Clientes ----
            migrationBuilder.AddColumn<int>(
                name: "CLI_Version",
                table: "Clientes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "CLI_Activo",
                table: "Clientes",
                type: "bit",
                nullable: false,
                defaultValue: true);   // los clientes existentes quedan activos

            migrationBuilder.AddColumn<bool>(
                name: "CLI_ConsumidorFinal",
                table: "Clientes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CLI_Direccion",
                table: "Clientes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CLI_Documento",
                table: "Clientes",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CLI_LimiteCredito",
                table: "Clientes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CLI_Localidad",
                table: "Clientes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CLI_TipoDocumento",
                table: "Clientes",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: false,
                defaultValue: "DNI");

            // ---- 4. Migración de datos de clientes ----
            // El cliente de sistema: el de menor ID entre los "Consumidor Final" con DNI 0.
            migrationBuilder.Sql(@"
UPDATE Clientes SET CLI_ConsumidorFinal = 1
WHERE CLI_ID = (SELECT MIN(c.CLI_ID) FROM Clientes c
                JOIN Personas p ON p.PER_ID = c.PER_ID
                WHERE p.PER_DNI = 0 AND p.PER_Nombre = 'Consumidor Final');");

            // Documento = DNI de la persona (7-8 dígitos). Si el DNI está repetido se conserva en el de menor ID
            // y los demás quedan sin documento (se completan al editarlos, el ABM lo exige).
            migrationBuilder.Sql(@"
WITH Documentos AS (
    SELECT c.CLI_ID, CAST(p.PER_DNI AS nvarchar(11)) AS Documento,
           ROW_NUMBER() OVER (PARTITION BY p.PER_DNI ORDER BY c.CLI_ID) AS Orden
    FROM Clientes c JOIN Personas p ON p.PER_ID = c.PER_ID
    WHERE p.PER_DNI BETWEEN 1000000 AND 99999999 AND c.CLI_ConsumidorFinal = 0
)
UPDATE c SET CLI_Documento = d.Documento
FROM Clientes c JOIN Documentos d ON d.CLI_ID = c.CLI_ID
WHERE d.Orden = 1;");

            // ---- 5. Índices y FK ----
            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_PER_ID",
                table: "Proveedores",
                column: "PER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_PROV_CUIT",
                table: "Proveedores",
                column: "PROV_CUIT",
                unique: true,
                filter: "[PROV_CUIT] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_CLI_Documento",
                table: "Clientes",
                column: "CLI_Documento",
                unique: true,
                filter: "[CLI_Documento] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_PER_ID",
                table: "Clientes",
                column: "PER_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Personas_PER_ID",
                table: "Clientes",
                column: "PER_ID",
                principalTable: "Personas",
                principalColumn: "PER_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proveedores_Personas_PER_ID",
                table: "Proveedores",
                column: "PER_ID",
                principalTable: "Personas",
                principalColumn: "PER_ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Personas_PER_ID",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Proveedores_Personas_PER_ID",
                table: "Proveedores");

            migrationBuilder.DropIndex(
                name: "IX_Proveedores_PER_ID",
                table: "Proveedores");

            migrationBuilder.DropIndex(
                name: "IX_Proveedores_PROV_CUIT",
                table: "Proveedores");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_CLI_Documento",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_PER_ID",
                table: "Clientes");

            foreach (var columna in new[] { "PROV_Version", "PROV_Activo", "PROV_CUIT", "PROV_CondicionFiscal", "PROV_Direccion", "PROV_Observaciones" })
                migrationBuilder.DropColumn(name: columna, table: "Proveedores");

            foreach (var columna in new[] { "CLI_Version", "CLI_Activo", "CLI_ConsumidorFinal", "CLI_Direccion", "CLI_Documento", "CLI_LimiteCredito", "CLI_Localidad", "CLI_TipoDocumento" })
                migrationBuilder.DropColumn(name: columna, table: "Clientes");

            // Se reconstruyen las columnas sombra a partir de PER_ID.
            migrationBuilder.AddColumn<int>(
                name: "PER_ProveedorPER_ID",
                table: "Proveedores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CLI_PersonaPER_ID",
                table: "Clientes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE Proveedores SET PER_ProveedorPER_ID = PER_ID;");
            migrationBuilder.Sql("UPDATE Clientes SET CLI_PersonaPER_ID = PER_ID;");

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_PER_ProveedorPER_ID",
                table: "Proveedores",
                column: "PER_ProveedorPER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_CLI_PersonaPER_ID",
                table: "Clientes",
                column: "CLI_PersonaPER_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Personas_CLI_PersonaPER_ID",
                table: "Clientes",
                column: "CLI_PersonaPER_ID",
                principalTable: "Personas",
                principalColumn: "PER_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proveedores_Personas_PER_ProveedorPER_ID",
                table: "Proveedores",
                column: "PER_ProveedorPER_ID",
                principalTable: "Personas",
                principalColumn: "PER_ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
