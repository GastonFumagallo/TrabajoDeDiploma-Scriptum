using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modelo.Migrations
{
    /// <inheritdoc />
    public partial class PrecioLibro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Precio",
                table: "ProveedoresLibros",
                newName: "PL_PrecioCompra");

            migrationBuilder.AddColumn<decimal>(
                name: "LIB_PrecioVenta",
                table: "Libros",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LIB_PrecioVenta",
                table: "Libros");

            migrationBuilder.RenameColumn(
                name: "PL_PrecioCompra",
                table: "ProveedoresLibros",
                newName: "Precio");
        }
    }
}
