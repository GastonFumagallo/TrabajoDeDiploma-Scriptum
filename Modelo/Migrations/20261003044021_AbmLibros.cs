using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modelo.Migrations
{
    /// <inheritdoc />
    public partial class AbmLibros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Libros_LIB_ISBN",
                table: "Libros");

            migrationBuilder.AddColumn<bool>(
                name: "LIB_Activo",
                table: "Libros",
                type: "bit",
                nullable: false,
                defaultValue: true);   // corregido a mano: los libros existentes quedan activos

            migrationBuilder.AddColumn<int>(
                name: "LIB_Version",
                table: "Libros",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Agregado a mano: el índice único fallaría si ya hay ISBN repetidos. Se conserva el ISBN en el libro
            // de menor ID y en los demás se deja vacío (se pueden revisar luego desde el ABM).
            migrationBuilder.Sql(@"
WITH Repetidos AS (
    SELECT LIB_ID, ROW_NUMBER() OVER (PARTITION BY LIB_ISBN ORDER BY LIB_ID) AS Orden
    FROM Libros WHERE LIB_ISBN IS NOT NULL
)
UPDATE Libros SET LIB_ISBN = NULL
WHERE LIB_ID IN (SELECT LIB_ID FROM Repetidos WHERE Orden > 1);");

            migrationBuilder.CreateIndex(
                name: "IX_Libros_LIB_ISBN",
                table: "Libros",
                column: "LIB_ISBN",
                unique: true,
                filter: "[LIB_ISBN] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Libros_LIB_ISBN",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "LIB_Activo",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "LIB_Version",
                table: "Libros");

            migrationBuilder.CreateIndex(
                name: "IX_Libros_LIB_ISBN",
                table: "Libros",
                column: "LIB_ISBN");
        }
    }
}
