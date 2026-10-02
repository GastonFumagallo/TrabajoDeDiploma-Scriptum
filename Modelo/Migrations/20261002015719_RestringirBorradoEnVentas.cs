using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modelo.Migrations
{
    /// <inheritdoc />
    public partial class RestringirBorradoEnVentas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallesVenta_Libros_LIB_ID",
                table: "DetallesVenta");

            migrationBuilder.DropForeignKey(
                name: "FK_Libros_Generos_GEN_ID",
                table: "Libros");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Clientes_CLI_ID",
                table: "Ventas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_MetodosPago_MP_ID",
                table: "Ventas");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesVenta_Libros_LIB_ID",
                table: "DetallesVenta",
                column: "LIB_ID",
                principalTable: "Libros",
                principalColumn: "LIB_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Libros_Generos_GEN_ID",
                table: "Libros",
                column: "GEN_ID",
                principalTable: "Generos",
                principalColumn: "GEN_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Clientes_CLI_ID",
                table: "Ventas",
                column: "CLI_ID",
                principalTable: "Clientes",
                principalColumn: "CLI_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_MetodosPago_MP_ID",
                table: "Ventas",
                column: "MP_ID",
                principalTable: "MetodosPago",
                principalColumn: "MP_ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallesVenta_Libros_LIB_ID",
                table: "DetallesVenta");

            migrationBuilder.DropForeignKey(
                name: "FK_Libros_Generos_GEN_ID",
                table: "Libros");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Clientes_CLI_ID",
                table: "Ventas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_MetodosPago_MP_ID",
                table: "Ventas");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesVenta_Libros_LIB_ID",
                table: "DetallesVenta",
                column: "LIB_ID",
                principalTable: "Libros",
                principalColumn: "LIB_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Libros_Generos_GEN_ID",
                table: "Libros",
                column: "GEN_ID",
                principalTable: "Generos",
                principalColumn: "GEN_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Clientes_CLI_ID",
                table: "Ventas",
                column: "CLI_ID",
                principalTable: "Clientes",
                principalColumn: "CLI_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_MetodosPago_MP_ID",
                table: "Ventas",
                column: "MP_ID",
                principalTable: "MetodosPago",
                principalColumn: "MP_ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
