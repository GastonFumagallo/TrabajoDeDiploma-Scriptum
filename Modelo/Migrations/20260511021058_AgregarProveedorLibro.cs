using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modelo.Migrations
{
    /// <inheritdoc />
    public partial class AgregarProveedorLibro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProveedorLibro_Libros_LIB_ID",
                table: "ProveedorLibro");

            migrationBuilder.DropForeignKey(
                name: "FK_ProveedorLibro_Proveedores_PROV_ID",
                table: "ProveedorLibro");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProveedorLibro",
                table: "ProveedorLibro");

            migrationBuilder.RenameTable(
                name: "ProveedorLibro",
                newName: "ProveedoresLibros");

            migrationBuilder.RenameIndex(
                name: "IX_ProveedorLibro_LIB_ID",
                table: "ProveedoresLibros",
                newName: "IX_ProveedoresLibros_LIB_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProveedoresLibros",
                table: "ProveedoresLibros",
                columns: new[] { "PROV_ID", "LIB_ID" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProveedoresLibros_Libros_LIB_ID",
                table: "ProveedoresLibros",
                column: "LIB_ID",
                principalTable: "Libros",
                principalColumn: "LIB_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProveedoresLibros_Proveedores_PROV_ID",
                table: "ProveedoresLibros",
                column: "PROV_ID",
                principalTable: "Proveedores",
                principalColumn: "PROV_ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProveedoresLibros_Libros_LIB_ID",
                table: "ProveedoresLibros");

            migrationBuilder.DropForeignKey(
                name: "FK_ProveedoresLibros_Proveedores_PROV_ID",
                table: "ProveedoresLibros");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProveedoresLibros",
                table: "ProveedoresLibros");

            migrationBuilder.RenameTable(
                name: "ProveedoresLibros",
                newName: "ProveedorLibro");

            migrationBuilder.RenameIndex(
                name: "IX_ProveedoresLibros_LIB_ID",
                table: "ProveedorLibro",
                newName: "IX_ProveedorLibro_LIB_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProveedorLibro",
                table: "ProveedorLibro",
                columns: new[] { "PROV_ID", "LIB_ID" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProveedorLibro_Libros_LIB_ID",
                table: "ProveedorLibro",
                column: "LIB_ID",
                principalTable: "Libros",
                principalColumn: "LIB_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProveedorLibro_Proveedores_PROV_ID",
                table: "ProveedorLibro",
                column: "PROV_ID",
                principalTable: "Proveedores",
                principalColumn: "PROV_ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
