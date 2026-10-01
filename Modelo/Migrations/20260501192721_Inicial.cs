using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modelo.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Estados_Grupos",
                columns: table => new
                {
                    EST_GRU_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EST_GRU_Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estados_Grupos", x => x.EST_GRU_ID);
                });

            migrationBuilder.CreateTable(
                name: "Estados_Usuarios",
                columns: table => new
                {
                    EST_USU_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EST_USU_Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estados_Usuarios", x => x.EST_USU_ID);
                });

            migrationBuilder.CreateTable(
                name: "Generos",
                columns: table => new
                {
                    GEN_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GEN_Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GEN_Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Generos", x => x.GEN_ID);
                });

            migrationBuilder.CreateTable(
                name: "MetodosPago",
                columns: table => new
                {
                    MP_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MP_Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MP_Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetodosPago", x => x.MP_ID);
                });

            migrationBuilder.CreateTable(
                name: "Modulos",
                columns: table => new
                {
                    MOD_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MOD_Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modulos", x => x.MOD_ID);
                });

            migrationBuilder.CreateTable(
                name: "Personas",
                columns: table => new
                {
                    PER_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PER_Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    PER_Mail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PER_Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PER_DNI = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personas", x => x.PER_ID);
                });

            migrationBuilder.CreateTable(
                name: "Grupos",
                columns: table => new
                {
                    GRU_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GRU_Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    GRU_Descripcion = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    EST_GRU_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grupos", x => x.GRU_ID);
                    table.ForeignKey(
                        name: "FK_Grupos_Estados_Grupos_EST_GRU_ID",
                        column: x => x.EST_GRU_ID,
                        principalTable: "Estados_Grupos",
                        principalColumn: "EST_GRU_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Libros",
                columns: table => new
                {
                    LIB_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LIB_Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LIB_Autor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LIB_Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LIB_Editorial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LIB_AñoPublicacion = table.Column<int>(type: "int", nullable: false),
                    GEN_ID = table.Column<int>(type: "int", nullable: false),
                    LIB_GeneroGEN_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Libros", x => x.LIB_ID);
                    table.ForeignKey(
                        name: "FK_Libros_Generos_LIB_GeneroGEN_ID",
                        column: x => x.LIB_GeneroGEN_ID,
                        principalTable: "Generos",
                        principalColumn: "GEN_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Formularios",
                columns: table => new
                {
                    FORM_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FORM_Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    MOD_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formularios", x => x.FORM_ID);
                    table.ForeignKey(
                        name: "FK_Formularios_Modulos_MOD_ID",
                        column: x => x.MOD_ID,
                        principalTable: "Modulos",
                        principalColumn: "MOD_ID");
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    CLI_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PER_ID = table.Column<int>(type: "int", nullable: false),
                    CLI_PersonaPER_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.CLI_ID);
                    table.ForeignKey(
                        name: "FK_Clientes_Personas_CLI_PersonaPER_ID",
                        column: x => x.CLI_PersonaPER_ID,
                        principalTable: "Personas",
                        principalColumn: "PER_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    PROV_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PER_ID = table.Column<int>(type: "int", nullable: false),
                    PER_ProveedorPER_ID = table.Column<int>(type: "int", nullable: false),
                    PROV_Empresa = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.PROV_ID);
                    table.ForeignKey(
                        name: "FK_Proveedores_Personas_PER_ProveedorPER_ID",
                        column: x => x.PER_ProveedorPER_ID,
                        principalTable: "Personas",
                        principalColumn: "PER_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    USU_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    USU_Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    USU_Clave = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    USU_Mail = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    EST_USU_ID = table.Column<int>(type: "int", nullable: true),
                    PER_ID = table.Column<int>(type: "int", nullable: false),
                    USU_PersonaPER_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.USU_ID);
                    table.ForeignKey(
                        name: "FK_Usuarios_Estados_Usuarios_EST_USU_ID",
                        column: x => x.EST_USU_ID,
                        principalTable: "Estados_Usuarios",
                        principalColumn: "EST_USU_ID");
                    table.ForeignKey(
                        name: "FK_Usuarios_Personas_USU_PersonaPER_ID",
                        column: x => x.USU_PersonaPER_ID,
                        principalTable: "Personas",
                        principalColumn: "PER_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Acciones",
                columns: table => new
                {
                    ACC_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ACC_Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    FORM_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acciones", x => x.ACC_ID);
                    table.ForeignKey(
                        name: "FK_Acciones_Formularios_FORM_ID",
                        column: x => x.FORM_ID,
                        principalTable: "Formularios",
                        principalColumn: "FORM_ID");
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    VEN_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VEN_Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CLI_ID = table.Column<int>(type: "int", nullable: false),
                    VEN_ClienteCLI_ID = table.Column<int>(type: "int", nullable: false),
                    VEN_Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MP_ID = table.Column<int>(type: "int", nullable: false),
                    VEN_MetodoPagoMP_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.VEN_ID);
                    table.ForeignKey(
                        name: "FK_Ventas_Clientes_VEN_ClienteCLI_ID",
                        column: x => x.VEN_ClienteCLI_ID,
                        principalTable: "Clientes",
                        principalColumn: "CLI_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ventas_MetodosPago_VEN_MetodoPagoMP_ID",
                        column: x => x.VEN_MetodoPagoMP_ID,
                        principalTable: "MetodosPago",
                        principalColumn: "MP_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProveedorLibro",
                columns: table => new
                {
                    PROV_ID = table.Column<int>(type: "int", nullable: false),
                    LIB_ID = table.Column<int>(type: "int", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProveedorLibro", x => new { x.PROV_ID, x.LIB_ID });
                    table.ForeignKey(
                        name: "FK_ProveedorLibro_Libros_LIB_ID",
                        column: x => x.LIB_ID,
                        principalTable: "Libros",
                        principalColumn: "LIB_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProveedorLibro_Proveedores_PROV_ID",
                        column: x => x.PROV_ID,
                        principalTable: "Proveedores",
                        principalColumn: "PROV_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditoriaSesiones",
                columns: table => new
                {
                    AS_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AS_USU_ID = table.Column<int>(type: "int", nullable: false),
                    AS_UsuarioUSU_ID = table.Column<int>(type: "int", nullable: false),
                    AS_FechaHoraLogin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AS_FechaHoraLogout = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AS_TipoLogout = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AS_SesionActiva = table.Column<bool>(type: "bit", nullable: false),
                    AS_TiempoSesion = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditoriaSesiones", x => x.AS_ID);
                    table.ForeignKey(
                        name: "FK_AuditoriaSesiones_Usuarios_AS_UsuarioUSU_ID",
                        column: x => x.AS_UsuarioUSU_ID,
                        principalTable: "Usuarios",
                        principalColumn: "USU_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrupoUsuario",
                columns: table => new
                {
                    GruposGRU_ID = table.Column<int>(type: "int", nullable: false),
                    UsuariosUSU_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoUsuario", x => new { x.GruposGRU_ID, x.UsuariosUSU_ID });
                    table.ForeignKey(
                        name: "FK_GrupoUsuario_Grupos_GruposGRU_ID",
                        column: x => x.GruposGRU_ID,
                        principalTable: "Grupos",
                        principalColumn: "GRU_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GrupoUsuario_Usuarios_UsuariosUSU_ID",
                        column: x => x.UsuariosUSU_ID,
                        principalTable: "Usuarios",
                        principalColumn: "USU_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccionGrupo",
                columns: table => new
                {
                    AccionesACC_ID = table.Column<int>(type: "int", nullable: false),
                    GruposGRU_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccionGrupo", x => new { x.AccionesACC_ID, x.GruposGRU_ID });
                    table.ForeignKey(
                        name: "FK_AccionGrupo_Acciones_AccionesACC_ID",
                        column: x => x.AccionesACC_ID,
                        principalTable: "Acciones",
                        principalColumn: "ACC_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccionGrupo_Grupos_GruposGRU_ID",
                        column: x => x.GruposGRU_ID,
                        principalTable: "Grupos",
                        principalColumn: "GRU_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccionUsuario",
                columns: table => new
                {
                    AccionesACC_ID = table.Column<int>(type: "int", nullable: false),
                    UsuariosUSU_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccionUsuario", x => new { x.AccionesACC_ID, x.UsuariosUSU_ID });
                    table.ForeignKey(
                        name: "FK_AccionUsuario_Acciones_AccionesACC_ID",
                        column: x => x.AccionesACC_ID,
                        principalTable: "Acciones",
                        principalColumn: "ACC_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccionUsuario_Usuarios_UsuariosUSU_ID",
                        column: x => x.UsuariosUSU_ID,
                        principalTable: "Usuarios",
                        principalColumn: "USU_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesVenta",
                columns: table => new
                {
                    DV_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VEN_ID = table.Column<int>(type: "int", nullable: false),
                    DV_VentaVEN_ID = table.Column<int>(type: "int", nullable: false),
                    LIB_ID = table.Column<int>(type: "int", nullable: false),
                    DV_LibroLIB_ID = table.Column<int>(type: "int", nullable: false),
                    DV_Cantidad = table.Column<int>(type: "int", nullable: false),
                    DV_PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesVenta", x => x.DV_ID);
                    table.ForeignKey(
                        name: "FK_DetallesVenta_Libros_DV_LibroLIB_ID",
                        column: x => x.DV_LibroLIB_ID,
                        principalTable: "Libros",
                        principalColumn: "LIB_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesVenta_Ventas_DV_VentaVEN_ID",
                        column: x => x.DV_VentaVEN_ID,
                        principalTable: "Ventas",
                        principalColumn: "VEN_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Acciones_FORM_ID",
                table: "Acciones",
                column: "FORM_ID");

            migrationBuilder.CreateIndex(
                name: "IX_AccionGrupo_GruposGRU_ID",
                table: "AccionGrupo",
                column: "GruposGRU_ID");

            migrationBuilder.CreateIndex(
                name: "IX_AccionUsuario_UsuariosUSU_ID",
                table: "AccionUsuario",
                column: "UsuariosUSU_ID");

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaSesiones_AS_UsuarioUSU_ID",
                table: "AuditoriaSesiones",
                column: "AS_UsuarioUSU_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_CLI_PersonaPER_ID",
                table: "Clientes",
                column: "CLI_PersonaPER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVenta_DV_LibroLIB_ID",
                table: "DetallesVenta",
                column: "DV_LibroLIB_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVenta_DV_VentaVEN_ID",
                table: "DetallesVenta",
                column: "DV_VentaVEN_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Formularios_MOD_ID",
                table: "Formularios",
                column: "MOD_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Grupos_EST_GRU_ID",
                table: "Grupos",
                column: "EST_GRU_ID");

            migrationBuilder.CreateIndex(
                name: "IX_GrupoUsuario_UsuariosUSU_ID",
                table: "GrupoUsuario",
                column: "UsuariosUSU_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Libros_LIB_GeneroGEN_ID",
                table: "Libros",
                column: "LIB_GeneroGEN_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_PER_ProveedorPER_ID",
                table: "Proveedores",
                column: "PER_ProveedorPER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorLibro_LIB_ID",
                table: "ProveedorLibro",
                column: "LIB_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_EST_USU_ID",
                table: "Usuarios",
                column: "EST_USU_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_USU_PersonaPER_ID",
                table: "Usuarios",
                column: "USU_PersonaPER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_VEN_ClienteCLI_ID",
                table: "Ventas",
                column: "VEN_ClienteCLI_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_VEN_MetodoPagoMP_ID",
                table: "Ventas",
                column: "VEN_MetodoPagoMP_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccionGrupo");

            migrationBuilder.DropTable(
                name: "AccionUsuario");

            migrationBuilder.DropTable(
                name: "AuditoriaSesiones");

            migrationBuilder.DropTable(
                name: "DetallesVenta");

            migrationBuilder.DropTable(
                name: "GrupoUsuario");

            migrationBuilder.DropTable(
                name: "ProveedorLibro");

            migrationBuilder.DropTable(
                name: "Acciones");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.DropTable(
                name: "Grupos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Libros");

            migrationBuilder.DropTable(
                name: "Proveedores");

            migrationBuilder.DropTable(
                name: "Formularios");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "MetodosPago");

            migrationBuilder.DropTable(
                name: "Estados_Grupos");

            migrationBuilder.DropTable(
                name: "Estados_Usuarios");

            migrationBuilder.DropTable(
                name: "Generos");

            migrationBuilder.DropTable(
                name: "Modulos");

            migrationBuilder.DropTable(
                name: "Personas");
        }
    }
}
