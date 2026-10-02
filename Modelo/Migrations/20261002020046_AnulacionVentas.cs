using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modelo.Migrations
{
    /// <inheritdoc />
    public partial class AnulacionVentas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "VEN_Anulada",
                table: "Ventas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "VEN_FechaAnulacion",
                table: "Ventas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VEN_MotivoAnulacion",
                table: "Ventas",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VEN_UsuarioAnulacion",
                table: "Ventas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VEN_Anulada",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "VEN_FechaAnulacion",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "VEN_MotivoAnulacion",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "VEN_UsuarioAnulacion",
                table: "Ventas");
        }
    }
}
