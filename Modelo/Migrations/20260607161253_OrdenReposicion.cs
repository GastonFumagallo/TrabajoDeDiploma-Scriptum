using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modelo.Migrations
{
    /// <inheritdoc />
    public partial class OrdenReposicion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrdenesReposicion",
                columns: table => new
                {
                    OR_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OR_LIB_ID = table.Column<int>(type: "int", nullable: false),
                    OR_PROV_ID = table.Column<int>(type: "int", nullable: false),
                    OR_Cantidad = table.Column<int>(type: "int", nullable: false),
                    OR_Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OR_Estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesReposicion", x => x.OR_ID);
                    table.ForeignKey(
                        name: "FK_OrdenesReposicion_Libros_OR_LIB_ID",
                        column: x => x.OR_LIB_ID,
                        principalTable: "Libros",
                        principalColumn: "LIB_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesReposicion_Proveedores_OR_PROV_ID",
                        column: x => x.OR_PROV_ID,
                        principalTable: "Proveedores",
                        principalColumn: "PROV_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesReposicion_OR_LIB_ID",
                table: "OrdenesReposicion",
                column: "OR_LIB_ID");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesReposicion_OR_PROV_ID",
                table: "OrdenesReposicion",
                column: "OR_PROV_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdenesReposicion");
        }
    }
}
