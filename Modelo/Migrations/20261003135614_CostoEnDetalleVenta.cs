using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modelo.Migrations
{
    /// <inheritdoc />
    public partial class CostoEnDetalleVenta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DV_CostoUnitario",
                table: "DetallesVenta",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            // Agregado a mano: las ventas anteriores no guardaban el costo. Se completa con el costo actual de cada
            // libro (aproximación) para que el CMV y la ganancia del historial no queden en cero.
            migrationBuilder.Sql(@"
UPDATE dv SET DV_CostoUnitario = l.LIB_PrecioCosto
FROM DetallesVenta dv JOIN Libros l ON l.LIB_ID = dv.LIB_ID;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DV_CostoUnitario",
                table: "DetallesVenta");
        }
    }
}
