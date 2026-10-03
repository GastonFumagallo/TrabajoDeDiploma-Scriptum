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
    /// Gestión de clientes. Misma estructura que <see cref="FrmGestionarProveedores"/> (comportamiento común en
    /// <see cref="ListadoAbm{TListado}"/>). "Consumidor Final" es un cliente de sistema: aparece primero,
    /// en azul, y no se puede modificar ni dar de baja.
    /// </summary>
    public partial class FrmGestionarClientes : Form
    {
        private const string TodasLasLocalidades = "Todas";

        private readonly ClienteService servicio = ClienteService.Instancia;
        private readonly ListadoAbm<ClienteListadoDTO> listado;

        public FrmGestionarClientes()
        {
            InitializeComponent();
            ConfigurarGrilla();

            listado = new ListadoAbm<ClienteListadoDTO>(this, new()
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
                Entidad = "cliente",
                EntidadPlural = "clientes",
                Id = c => c.Id,
                Activo = c => c.Activo,
                Descripcion = c => c.Nombre,
                Protegido = c => c.EsConsumidorFinal,
                Cargar = (texto, estado, ct) => servicio.ObtenerTodosAsync(new FiltroClientes
                {
                    Texto = texto,
                    Estado = estado,
                    Localidad = cbFiltroExtra.SelectedItem as string is { } l && l != TodasLasLocalidades ? l : null,
                }, ct),
                AbrirEdicion = AbrirEdicion,
                CambiarEstadoServicio = servicio.CambiarEstadoAsync,
                AdvertenciaBaja = c => c.Ventas > 0 ? $"tiene {c.Ventas} venta(s) registradas; se conservan en el historial." : null,
            });

            // El cliente de sistema se distingue visualmente.
            dgvListado.CellFormatting += (_, e) =>
            {
                if (e.RowIndex >= 0 && e.CellStyle != null && dgvListado.Rows[e.RowIndex].DataBoundItem is ClienteListadoDTO { EsConsumidorFinal: true })
                    e.CellStyle.ForeColor = Color.SteelBlue;
            };

            cbFiltroExtra.SelectedIndexChanged += async (_, _) => await listado.CargarAsync();
            btnLimpiarFiltros.Click += async (_, _) =>
            {
                if (cbFiltroExtra.Items.Count > 0) cbFiltroExtra.SelectedIndex = 0;
                await listado.LimpiarFiltrosAsync();
            };
            btnSalir.Click += (_, _) => Salir();
        }

        private void ConfigurarGrilla()
        {
            GrillaHelper.ConfigurarListado(dgvListado);
            GrillaHelper.Columna(dgvListado, nameof(ClienteListadoDTO.Id), "ID", peso: 40, derecha: true);
            GrillaHelper.Columna(dgvListado, nameof(ClienteListadoDTO.DocumentoFormateado), "Documento", peso: 120);
            GrillaHelper.Columna(dgvListado, nameof(ClienteListadoDTO.Nombre), "Nombre / Razón social", peso: 200);
            GrillaHelper.Columna(dgvListado, nameof(ClienteListadoDTO.Telefono), "Teléfono", peso: 100);
            GrillaHelper.Columna(dgvListado, nameof(ClienteListadoDTO.Email), "Email", peso: 170);
            GrillaHelper.Columna(dgvListado, nameof(ClienteListadoDTO.Localidad), "Localidad", peso: 110);
            GrillaHelper.Columna(dgvListado, nameof(ClienteListadoDTO.LimiteCredito), "Límite crédito", peso: 80, formato: "N2", derecha: true);
            GrillaHelper.Columna(dgvListado, nameof(ClienteListadoDTO.Ventas), "Ventas", peso: 50, derecha: true);
            GrillaHelper.Columna(dgvListado, nameof(ClienteListadoDTO.Estado), "Estado", peso: 60);
        }

        private async void FrmGestionarClientes_Load(object sender, EventArgs e)
        {
            btnNuevo.Visible = PermisoService.Instancia.TienePermiso("AgregarCliente");
            btnEditar.Visible = PermisoService.Instancia.TienePermiso("ModificarCliente");
            btnCambiarEstado.Visible = PermisoService.Instancia.TienePermiso("EliminarCliente");

            await CargarLocalidadesAsync();
            await listado.IniciarAsync();
        }

        private async Task CargarLocalidadesAsync()
        {
            try
            {
                string? actual = cbFiltroExtra.SelectedItem as string;
                var localidades = await servicio.ObtenerLocalidadesAsync(listado.Token);
                cbFiltroExtra.Items.Clear();
                cbFiltroExtra.Items.Add(TodasLasLocalidades);
                cbFiltroExtra.Items.AddRange(localidades.ToArray());
                cbFiltroExtra.SelectedItem = actual != null && localidades.Contains(actual) ? actual : TodasLasLocalidades;
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudieron cargar las localidades.");
            }
        }

        /// <summary>Abre el modal único (null = alta). Devuelve el id guardado, o null si se canceló.</summary>
        private int? AbrirEdicion(int? id)
        {
            using var frm = new FrmEditarCliente(id);
            if (frm.ShowDialog(this) != DialogResult.OK) return null;
            _ = CargarLocalidadesAsync();   // pudo agregarse una localidad nueva
            return frm.ClienteId;
        }

        private void Salir()
        {
            if (Application.OpenForms["FrmMenu"] is FrmMenu principal)
                principal.MostrarInicio();
            Close();
        }
    }
}
