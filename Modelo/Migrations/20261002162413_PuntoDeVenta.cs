using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modelo.Migrations
{
    /// <inheritdoc />
    public partial class PuntoDeVenta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "VEN_AjusteMedioPago",
                table: "Ventas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VEN_Descuento",
                table: "Ventas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VEN_IVA",
                table: "Ventas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VEN_PorcentajeDescuento",
                table: "Ventas",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VEN_Subtotal",
                table: "Ventas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "VEN_Usuario",
                table: "Ventas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LIB_ISBN",
                table: "Libros",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PagosVenta",
                columns: table => new
                {
                    PAG_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VEN_ID = table.Column<int>(type: "int", nullable: false),
                    MP_ID = table.Column<int>(type: "int", nullable: false),
                    PAG_Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PAG_Recibido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PAG_Vuelto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PAG_Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosVenta", x => x.PAG_ID);
                    table.ForeignKey(
                        name: "FK_PagosVenta_MetodosPago_MP_ID",
                        column: x => x.MP_ID,
                        principalTable: "MetodosPago",
                        principalColumn: "MP_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PagosVenta_Ventas_VEN_ID",
                        column: x => x.VEN_ID,
                        principalTable: "Ventas",
                        principalColumn: "VEN_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Libros_LIB_ISBN",
                table: "Libros",
                column: "LIB_ISBN");

            migrationBuilder.CreateIndex(
                name: "IX_PagosVenta_MP_ID",
                table: "PagosVenta",
                column: "MP_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PagosVenta_VEN_ID",
                table: "PagosVenta",
                column: "VEN_ID");

            // Agregado a mano: completar el desglose y el pago de las ventas existentes para que
            // tickets, métricas y la regla Total = Subtotal - Descuento + Ajuste valgan también para el historial.
            migrationBuilder.Sql(@"
UPDATE v SET
    VEN_Subtotal = ISNULL(d.Subtotal, v.VEN_Total),
    VEN_AjusteMedioPago = v.VEN_Total - ISNULL(d.Subtotal, v.VEN_Total)
FROM Ventas v
OUTER APPLY (SELECT SUM(dv.DV_Cantidad * dv.DV_PrecioUnitario) AS Subtotal
             FROM DetallesVenta dv WHERE dv.VEN_ID = v.VEN_ID) d;");

            migrationBuilder.Sql(@"
INSERT INTO PagosVenta (VEN_ID, MP_ID, PAG_Monto, PAG_Recibido, PAG_Vuelto, PAG_Fecha)
SELECT v.VEN_ID, v.MP_ID, v.VEN_Total, v.VEN_Total, 0, v.VEN_Fecha
FROM Ventas v
WHERE NOT EXISTS (SELECT 1 FROM PagosVenta p WHERE p.VEN_ID = v.VEN_ID);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PagosVenta");

            migrationBuilder.DropIndex(
                name: "IX_Libros_LIB_ISBN",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "VEN_AjusteMedioPago",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "VEN_Descuento",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "VEN_IVA",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "VEN_PorcentajeDescuento",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "VEN_Subtotal",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "VEN_Usuario",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "LIB_ISBN",
                table: "Libros");
        }
    }
}
