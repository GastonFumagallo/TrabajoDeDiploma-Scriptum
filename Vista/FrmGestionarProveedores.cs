using Controladora.Abm;
using Modelo;
using Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Vista.Comun;

namespace Vista
{
    /// <summary>
    /// Gestión de proveedores. Todo el comportamiento de listado (búsqueda reactiva, filtro de estado, atajos
    /// F2/F3/F4, alta/edición, baja lógica, exportar e imprimir) lo aporta <see cref="ListadoAbm{TListado}"/>;
    /// este formulario sólo define columnas, el filtro propio (condición fiscal) y qué servicio usar.
    /// </summary>
    public partial class FrmGestionarProveedores : Form
    {
        private const string TodasLasCondiciones = "Todas";

        private readonly IProveedorService servicio = ProveedorService.Instancia;
        private readonly ListadoAbm<ProveedorListadoDTO> listado;

        public FrmGestionarProveedores()
        {
            InitializeComponent();
            ConfigurarGrilla();

            listado = new ListadoAbm<ProveedorListadoDTO>(this, new()
            {
                Grilla = dgvListado,
                Buscar = txtBuscar,
                Estado = cbEstado,
                Resumen = lblResumen,
                Nuevo = btnNuevo,
                Editar = btnEditar,
                CambiarEstado = btnCambiarEstado,
                Exportar = btnExportar,
                Imprimir = btnImprimir,
                Entidad = "proveedor",
                EntidadPlural = "proveedores",
                Id = p => p.Id,
                Activo = p => p.Activo,
                Descripcion = p => p.RazonSocial,
                Cargar = (texto, estado, ct) => servicio.ObtenerTodosAsync(new FiltroProveedores
                {
                    Texto = texto,
                    Estado = estado,
                    CondicionFiscal = cbFiltroExtra.SelectedItem as string is { } c && c != TodasLasCondiciones ? c : null,
                }, ct),
                AbrirEdicion = AbrirEdicion,
                CambiarEstadoServicio = servicio.CambiarEstadoAsync,
                AdvertenciaBaja = p => p.OrdenesActivas > 0
                    ? $"tiene {p.OrdenesActivas} orden(es) de reposición activa(s); seguirán su curso, pero no se podrán crear nuevas."
                    : null,
            });

            cbFiltroExtra.Items.Add(TodasLasCondiciones);
            cbFiltroExtra.Items.AddRange(CondicionFiscal.Todas);
            cbFiltroExtra.SelectedIndex = 0;
            cbFiltroExtra.SelectedIndexChanged += async (_, _) => await listado.CargarAsync();
            btnLimpiarFiltros.Click += async (_, _) =>
            {
                cbFiltroExtra.SelectedIndex = 0;
                await listado.LimpiarFiltrosAsync();
            };
            btnSalir.Click += (_, _) => Salir();
        }

        private void ConfigurarGrilla()
        {
            GrillaHelper.ConfigurarListado(dgvListado);
            GrillaHelper.Columna(dgvListado, nameof(ProveedorListadoDTO.Id), "ID", peso: 40, derecha: true);
            GrillaHelper.Columna(dgvListado, nameof(ProveedorListadoDTO.CuitFormateado), "CUIT", peso: 100);
            GrillaHelper.Columna(dgvListado, nameof(ProveedorListadoDTO.RazonSocial), "Razón social", peso: 200);
            GrillaHelper.Columna(dgvListado, nameof(ProveedorListadoDTO.Contacto), "Contacto", peso: 140);
            GrillaHelper.Columna(dgvListado, nameof(ProveedorListadoDTO.Telefono), "Teléfono", peso: 100);
            GrillaHelper.Columna(dgvListado, nameof(ProveedorListadoDTO.Email), "Email", peso: 170);
            GrillaHelper.Columna(dgvListado, nameof(ProveedorListadoDTO.CondicionFiscal), "Condición fiscal", peso: 120);
            GrillaHelper.Columna(dgvListado, nameof(ProveedorListadoDTO.Libros), "Libros", peso: 50, derecha: true);
            GrillaHelper.Columna(dgvListado, nameof(ProveedorListadoDTO.OrdenesActivas), "Órdenes activas", peso: 70, derecha: true);
            GrillaHelper.Columna(dgvListado, nameof(ProveedorListadoDTO.Estado), "Estado", peso: 60);
        }

        private async void FrmGestionarProveedores_Load(object sender, EventArgs e)
        {
            btnNuevo.Visible = PermisoService.Instancia.TienePermiso("AgregarProveedor");
            btnEditar.Visible = PermisoService.Instancia.TienePermiso("ModificarProveedor");
            btnCambiarEstado.Visible = PermisoService.Instancia.TienePermiso("EliminarProveedor");
            await listado.IniciarAsync();
        }

        /// <summary>Abre el modal único (null = alta). Devuelve el id guardado, o null si se canceló.</summary>
        private int? AbrirEdicion(int? id)
        {
            using var frm = new FrmEditarProveedor(id);
            return frm.ShowDialog(this) == DialogResult.OK ? frm.ProveedorId : null;
        }

        private void Salir()
        {
            if (Application.OpenForms["FrmMenu"] is FrmMenu principal)
                principal.MostrarInicio();
            Close();
        }
    }
}
