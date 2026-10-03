using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modelo.Migrations
{
    /// <inheritdoc />
    public partial class InventarioYReposicion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesReposicion_Libros_OR_LIB_ID",
                table: "OrdenesReposicion");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesReposicion_OR_LIB_ID",
                table: "OrdenesReposicion");

            // OR_Cantidad y OR_LIB_ID se eliminan al final, después de copiar sus datos a DetallesOrdenReposicion.

            migrationBuilder.AlterColumn<string>(
                name: "OR_Estado",
                table: "OrdenesReposicion",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "OR_FechaCancelacion",
                table: "OrdenesReposicion",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OR_FechaRecepcion",
                table: "OrdenesReposicion",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OR_FechaSolicitud",
                table: "OrdenesReposicion",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OR_MotivoCancelacion",
                table: "OrdenesReposicion",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OR_Observaciones",
                table: "OrdenesReposicion",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OR_Usuario",
                table: "OrdenesReposicion",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OR_UsuarioCancelacion",
                table: "OrdenesReposicion",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OR_UsuarioRecepcion",
                table: "OrdenesReposicion",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LIB_PrecioCosto",
                table: "Libros",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "LIB_PuntoReposicion",
                table: "Libros",
                type: "int",
                nullable: false,
                defaultValue: 5);  // valor inicial para los libros existentes

            migrationBuilder.AddColumn<int>(
                name: "LIB_StockMinimo",
                table: "Libros",
                type: "int",
                nullable: false,
                defaultValue: 2);  // valor inicial para los libros existentes

            migrationBuilder.AddColumn<int>(
                name: "LIB_StockOptimo",
                table: "Libros",
                type: "int",
                nullable: false,
                defaultValue: 10);  // valor inicial para los libros existentes

            migrationBuilder.CreateTable(
                name: "DetallesOrdenReposicion",
                columns: table => new
                {
                    DOR_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OR_ID = table.Column<int>(type: "int", nullable: false),
                    LIB_ID = table.Column<int>(type: "int", nullable: false),
                    DOR_CantidadPedida = table.Column<int>(type: "int", nullable: false),
                    DOR_CostoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DOR_CantidadRecibida = table.Column<int>(type: "int", nullable: true),
                    DOR_CostoRecibido = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesOrdenReposicion", x => x.DOR_ID);
                    table.ForeignKey(
                        name: "FK_DetallesOrdenReposicion_Libros_LIB_ID",
                        column: x => x.LIB_ID,
                        principalTable: "Libros",
                        principalColumn: "LIB_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetallesOrdenReposicion_OrdenesReposicion_OR_ID",
                        column: x => x.OR_ID,
                        principalTable: "OrdenesReposicion",
                        principalColumn: "OR_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosStock",
                columns: table => new
                {
                    MOV_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LIB_ID = table.Column<int>(type: "int", nullable: false),
                    MOV_Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MOV_Tipo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MOV_Cantidad = table.Column<int>(type: "int", nullable: false),
                    MOV_StockAnterior = table.Column<int>(type: "int", nullable: false),
                    MOV_StockResultante = table.Column<int>(type: "int", nullable: false),
                    MOV_Motivo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MOV_Observacion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    MOV_Usuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MOV_Referencia = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosStock", x => x.MOV_ID);
                    table.ForeignKey(
                        name: "FK_MovimientosStock_Libros_LIB_ID",
                        column: x => x.LIB_ID,
                        principalTable: "Libros",
                        principalColumn: "LIB_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesReposicion_OR_Estado",
                table: "OrdenesReposicion",
                column: "OR_Estado");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesOrdenReposicion_LIB_ID",
                table: "DetallesOrdenReposicion",
                column: "LIB_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesOrdenReposicion_OR_ID",
                table: "DetallesOrdenReposicion",
                column: "OR_ID");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosStock_LIB_ID_MOV_Fecha",
                table: "MovimientosStock",
                columns: new[] { "LIB_ID", "MOV_Fecha" });

            // ---- Agregado a mano: migración de datos ----

            // 1) Cada orden vieja tenía un único libro: pasa a ser una orden con un ítem.
            //    El costo se toma del precio pactado con ese proveedor (si existe).
            migrationBuilder.Sql(@"
INSERT INTO DetallesOrdenReposicion (OR_ID, LIB_ID, DOR_CantidadPedida, DOR_CostoUnitario, DOR_CantidadRecibida, DOR_CostoRecibido)
SELECT o.OR_ID, o.OR_LIB_ID, o.OR_Cantidad,
       ISNULL(pl.PL_PrecioCompra, 0),
       CASE WHEN o.OR_Estado = 'RECIBIDA' THEN o.OR_Cantidad END,
       CASE WHEN o.OR_Estado = 'RECIBIDA' THEN ISNULL(pl.PL_PrecioCompra, 0) END
FROM OrdenesReposicion o
LEFT JOIN ProveedoresLibros pl ON pl.PROV_ID = o.OR_PROV_ID AND pl.LIB_ID = o.OR_LIB_ID;");

            // 2) Estados: ENVIADA pasa a llamarse SOLICITADA.
            migrationBuilder.Sql("UPDATE OrdenesReposicion SET OR_Estado = 'SOLICITADA', OR_FechaSolicitud = OR_Fecha WHERE OR_Estado = 'ENVIADA';");
            migrationBuilder.Sql("UPDATE OrdenesReposicion SET OR_FechaRecepcion = OR_Fecha WHERE OR_Estado = 'RECIBIDA';");

            // 3) Último costo conocido de cada libro: el menor precio pactado con sus proveedores.
            migrationBuilder.Sql(@"
UPDATE l SET LIB_PrecioCosto = p.Minimo
FROM Libros l
JOIN (SELECT LIB_ID, MIN(PL_PrecioCompra) AS Minimo FROM ProveedoresLibros GROUP BY LIB_ID) p ON p.LIB_ID = l.LIB_ID;");

            // 4) Saldo inicial del historial: así la suma de movimientos de cada libro coincide con su stock.
            migrationBuilder.Sql(@"
INSERT INTO MovimientosStock (LIB_ID, MOV_Fecha, MOV_Tipo, MOV_Cantidad, MOV_StockAnterior, MOV_StockResultante, MOV_Motivo, MOV_Usuario)
SELECT LIB_ID, SYSDATETIME(), 'ALTA_INICIAL', LIB_Stock, 0, LIB_Stock, 'Saldo inicial al habilitar el historial', 'sistema'
FROM Libros WHERE LIB_Stock > 0;");

            // 5) Recién ahora se eliminan las columnas viejas de la orden.
            migrationBuilder.DropColumn(
                name: "OR_Cantidad",
                table: "OrdenesReposicion");

            migrationBuilder.DropColumn(
                name: "OR_LIB_ID",
                table: "OrdenesReposicion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Agregado a mano: se reconstruye el modelo viejo (un libro por orden) a partir del primer ítem.
            migrationBuilder.AddColumn<int>(
                name: "OR_Cantidad",
                table: "OrdenesReposicion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OR_LIB_ID",
                table: "OrdenesReposicion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
UPDATE o SET OR_LIB_ID = d.LIB_ID, OR_Cantidad = d.DOR_CantidadPedida
FROM OrdenesReposicion o
CROSS APPLY (SELECT TOP 1 LIB_ID, DOR_CantidadPedida FROM DetallesOrdenReposicion WHERE OR_ID = o.OR_ID ORDER BY DOR_ID) d;");
            migrationBuilder.Sql("DELETE FROM OrdenesReposicion WHERE OR_LIB_ID = 0;");
            migrationBuilder.Sql("UPDATE OrdenesReposicion SET OR_Estado = 'ENVIADA' WHERE OR_Estado = 'SOLICITADA';");

            migrationBuilder.DropTable(
                name: "DetallesOrdenReposicion");

            migrationBuilder.DropTable(
                name: "MovimientosStock");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesReposicion_OR_Estado",
                table: "OrdenesReposicion");

            migrationBuilder.DropColumn(
                name: "OR_FechaCancelacion",
                table: "OrdenesReposicion");

            migrationBuilder.DropColumn(
                name: "OR_FechaRecepcion",
                table: "OrdenesReposicion");

            migrationBuilder.DropColumn(
                name: "OR_FechaSolicitud",
                table: "OrdenesReposicion");

            migrationBuilder.DropColumn(
                name: "OR_MotivoCancelacion",
                table: "OrdenesReposicion");

            migrationBuilder.DropColumn(
                name: "OR_Observaciones",
                table: "OrdenesReposicion");

            migrationBuilder.DropColumn(
                name: "OR_Usuario",
                table: "OrdenesReposicion");

            migrationBuilder.DropColumn(
                name: "OR_UsuarioCancelacion",
                table: "OrdenesReposicion");

            migrationBuilder.DropColumn(
                name: "OR_UsuarioRecepcion",
                table: "OrdenesReposicion");

            migrationBuilder.DropColumn(
                name: "LIB_PrecioCosto",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "LIB_PuntoReposicion",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "LIB_StockMinimo",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "LIB_StockOptimo",
                table: "Libros");

            migrationBuilder.AlterColumn<string>(
                name: "OR_Estado",
                table: "OrdenesReposicion",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesReposicion_OR_LIB_ID",
                table: "OrdenesReposicion",
                column: "OR_LIB_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesReposicion_Libros_OR_LIB_ID",
                table: "OrdenesReposicion",
                column: "OR_LIB_ID",
                principalTable: "Libros",
                principalColumn: "LIB_ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
