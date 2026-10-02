using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modelo.Migrations
{
    /// <inheritdoc />
    public partial class CorregirClavesForaneas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallesVenta_Libros_DV_LibroLIB_ID",
                table: "DetallesVenta");

            migrationBuilder.DropForeignKey(
                name: "FK_DetallesVenta_Ventas_DV_VentaVEN_ID",
                table: "DetallesVenta");

            migrationBuilder.DropForeignKey(
                name: "FK_Libros_Generos_LIB_GeneroGEN_ID",
                table: "Libros");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Clientes_VEN_ClienteCLI_ID",
                table: "Ventas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_MetodosPago_VEN_MetodoPagoMP_ID",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Ventas_VEN_ClienteCLI_ID",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Ventas_VEN_MetodoPagoMP_ID",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Libros_LIB_GeneroGEN_ID",
                table: "Libros");

            migrationBuilder.DropIndex(
                name: "IX_DetallesVenta_DV_LibroLIB_ID",
                table: "DetallesVenta");

            migrationBuilder.DropIndex(
                name: "IX_DetallesVenta_DV_VentaVEN_ID",
                table: "DetallesVenta");

            // Agregado a mano: las relaciones reales estaban en las columnas sombra y las propiedades
            // CLI_ID, MP_ID, GEN_ID, LIB_ID y VEN_ID quedaron en 0. Se copian antes de borrar las sombra
            // para no perder datos y para que las nuevas FK no fallen.
            migrationBuilder.Sql("UPDATE Ventas SET CLI_ID = VEN_ClienteCLI_ID, MP_ID = VEN_MetodoPagoMP_ID;");
            migrationBuilder.Sql("UPDATE Libros SET GEN_ID = LIB_GeneroGEN_ID;");
            migrationBuilder.Sql("UPDATE DetallesVenta SET LIB_ID = DV_LibroLIB_ID, VEN_ID = DV_VentaVEN_ID;");

            migrationBuilder.DropColumn(
                name: "VEN_ClienteCLI_ID",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "VEN_MetodoPagoMP_ID",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "LIB_GeneroGEN_ID",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "DV_LibroLIB_ID",
                table: "DetallesVenta");

            migrationBuilder.DropColumn(
                name: "DV_VentaVEN_ID",
                table: "DetallesVenta");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_CLI_ID",
                table: "Ventas",
                column: "CLI_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_MP_ID",
                table: "Ventas",
                column: "MP_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Libros_GEN_ID",
                table: "Libros",
                column: "GEN_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVenta_LIB_ID",
                table: "DetallesVenta",
                column: "LIB_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVenta_VEN_ID",
                table: "DetallesVenta",
                column: "VEN_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesVenta_Libros_LIB_ID",
                table: "DetallesVenta",
                column: "LIB_ID",
                principalTable: "Libros",
                principalColumn: "LIB_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesVenta_Ventas_VEN_ID",
                table: "DetallesVenta",
                column: "VEN_ID",
                principalTable: "Ventas",
                principalColumn: "VEN_ID",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallesVenta_Libros_LIB_ID",
                table: "DetallesVenta");

            migrationBuilder.DropForeignKey(
                name: "FK_DetallesVenta_Ventas_VEN_ID",
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

            migrationBuilder.DropIndex(
                name: "IX_Ventas_CLI_ID",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Ventas_MP_ID",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Libros_GEN_ID",
                table: "Libros");

            migrationBuilder.DropIndex(
                name: "IX_DetallesVenta_LIB_ID",
                table: "DetallesVenta");

            migrationBuilder.DropIndex(
                name: "IX_DetallesVenta_VEN_ID",
                table: "DetallesVenta");

            migrationBuilder.AddColumn<int>(
                name: "VEN_ClienteCLI_ID",
                table: "Ventas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VEN_MetodoPagoMP_ID",
                table: "Ventas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LIB_GeneroGEN_ID",
                table: "Libros",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DV_LibroLIB_ID",
                table: "DetallesVenta",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DV_VentaVEN_ID",
                table: "DetallesVenta",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Agregado a mano: restaura los vínculos en las columnas sombra antes de recrear sus FK.
            migrationBuilder.Sql("UPDATE Ventas SET VEN_ClienteCLI_ID = CLI_ID, VEN_MetodoPagoMP_ID = MP_ID;");
            migrationBuilder.Sql("UPDATE Libros SET LIB_GeneroGEN_ID = GEN_ID;");
            migrationBuilder.Sql("UPDATE DetallesVenta SET DV_LibroLIB_ID = LIB_ID, DV_VentaVEN_ID = VEN_ID;");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_VEN_ClienteCLI_ID",
                table: "Ventas",
                column: "VEN_ClienteCLI_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_VEN_MetodoPagoMP_ID",
                table: "Ventas",
                column: "VEN_MetodoPagoMP_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Libros_LIB_GeneroGEN_ID",
                table: "Libros",
                column: "LIB_GeneroGEN_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVenta_DV_LibroLIB_ID",
                table: "DetallesVenta",
                column: "DV_LibroLIB_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVenta_DV_VentaVEN_ID",
                table: "DetallesVenta",
                column: "DV_VentaVEN_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesVenta_Libros_DV_LibroLIB_ID",
                table: "DetallesVenta",
                column: "DV_LibroLIB_ID",
                principalTable: "Libros",
                principalColumn: "LIB_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesVenta_Ventas_DV_VentaVEN_ID",
                table: "DetallesVenta",
                column: "DV_VentaVEN_ID",
                principalTable: "Ventas",
                principalColumn: "VEN_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Libros_Generos_LIB_GeneroGEN_ID",
                table: "Libros",
                column: "LIB_GeneroGEN_ID",
                principalTable: "Generos",
                principalColumn: "GEN_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Clientes_VEN_ClienteCLI_ID",
                table: "Ventas",
                column: "VEN_ClienteCLI_ID",
                principalTable: "Clientes",
                principalColumn: "CLI_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_MetodosPago_VEN_MetodoPagoMP_ID",
                table: "Ventas",
                column: "VEN_MetodoPagoMP_ID",
                principalTable: "MetodosPago",
                principalColumn: "MP_ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
