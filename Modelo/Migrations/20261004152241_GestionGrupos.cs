using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modelo.Migrations
{
    /// <inheritdoc />
    public partial class GestionGrupos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Nombres sin espacios sobrantes y sin duplicados antes del índice único: el de menor ID conserva
            // el nombre; los demás quedan como "Nombre (ID)" para renombrarlos desde la gestión de grupos.
            migrationBuilder.Sql("UPDATE Grupos SET GRU_Nombre = LTRIM(RTRIM(GRU_Nombre));");
            migrationBuilder.Sql(@"
UPDATE g
SET GRU_Nombre = LEFT(g.GRU_Nombre, 60 - LEN(' (' + CAST(g.GRU_ID AS nvarchar(10)) + ')')) + ' (' + CAST(g.GRU_ID AS nvarchar(10)) + ')'
FROM Grupos g
WHERE EXISTS (SELECT 1 FROM Grupos o WHERE o.GRU_Nombre = g.GRU_Nombre AND o.GRU_ID < g.GRU_ID);");

            // Estados de grupo de sistema (Estado_Grupo.Activo / Estado_Grupo.Inactivo). Hasta ahora sólo existía
            // "Activo" y no había forma de deshabilitar un grupo.
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM Estados_Grupos WHERE EST_GRU_Nombre = N'Activo')
    INSERT INTO Estados_Grupos (EST_GRU_Nombre) VALUES (N'Activo');
IF NOT EXISTS (SELECT 1 FROM Estados_Grupos WHERE EST_GRU_Nombre = N'Inactivo')
    INSERT INTO Estados_Grupos (EST_GRU_Nombre) VALUES (N'Inactivo');");

            migrationBuilder.CreateIndex(
                name: "IX_Grupos_GRU_Nombre",
                table: "Grupos",
                column: "GRU_Nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Grupos_GRU_Nombre",
                table: "Grupos");
        }
    }
}
