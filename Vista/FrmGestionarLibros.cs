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
    /// Gestión de libros. Misma estructura que Proveedores y Clientes: el comportamiento de listado
    /// (búsqueda reactiva, estado, F2/F3/F4, baja lógica, exportar e imprimir) lo aporta <see cref="ListadoAbm{TListado}"/>.
    /// </summary>
    public partial class FrmGestionarLibros : Form
    {
        private static readonly OpcionDTO TodosLosGeneros = new(0, "Todos");

        private readonly LibroService servicio = LibroService.Instancia;
        private readonly ListadoAbm<LibroListadoDTO> listado;

        public FrmGestionarLibros()
        {
            InitializeComponent();
            ConfigurarGrilla();

            listado = new ListadoAbm<LibroListadoDTO>(this, new()
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
                Entidad = "libro",
                EntidadPlural = "libros",
                Id = l => l.Id,
                Activo = l => l.Activo,
                Descripcion = l => l.Titulo,
                Cargar = (texto, estado, ct) => servicio.ObtenerTodosAsync(new FiltroLibros
                {
                    Texto = texto,
                    Estado = estado,
                    GeneroId = cbFiltroExtra.SelectedItem is OpcionDTO { Id: > 0 } g ? g.Id : null,
                }, ct),
                AbrirEdicion = AbrirEdicion,
                CambiarEstadoServicio = servicio.CambiarEstadoAsync,
                AdvertenciaBaja = l => l.Stock > 0 ? $"todavía tiene {l.Stock} unidad(es) en stock; no se podrán vender." : null,
            });

            // Margen no positivo en rojo: alerta de precio mal cargado.
            dgvListado.CellFormatting += (_, e) =>
            {
                if (e.RowIndex >= 0 && e.CellStyle != null
                    && dgvListado.Columns[e.ColumnIndex].Name == nameof(LibroListadoDTO.Margen)
                    && dgvListado.Rows[e.RowIndex].DataBoundItem is LibroListadoDTO { Activo: true, Margen: <= 0 })
                    e.CellStyle.ForeColor = Color.Firebrick;
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
            GrillaHelper.Columna(dgvListado, nameof(LibroListadoDTO.Id), "ID", peso: 40, derecha: true);
            GrillaHelper.Columna(dgvListado, nameof(LibroListadoDTO.ISBN), "ISBN", peso: 100);
            GrillaHelper.Columna(dgvListado, nameof(LibroListadoDTO.Titulo), "Título", peso: 230);
            GrillaHelper.Columna(dgvListado, nameof(LibroListadoDTO.Autor), "Autor", peso: 140);
            GrillaHelper.Columna(dgvListado, nameof(LibroListadoDTO.Editorial), "Editorial", peso: 110);
            GrillaHelper.Columna(dgvListado, nameof(LibroListadoDTO.Genero), "Género", peso: 90);
            GrillaHelper.Columna(dgvListado, nameof(LibroListadoDTO.PrecioCosto), "Costo", peso: 70, formato: "N2", derecha: true);
            GrillaHelper.Columna(dgvListado, nameof(LibroListadoDTO.PrecioVenta), "Venta", peso: 70, formato: "N2", derecha: true);
            GrillaHelper.Columna(dgvListado, nameof(LibroListadoDTO.Margen), "Margen %", peso: 60, formato: "N1", derecha: true);
            GrillaHelper.Columna(dgvListado, nameof(LibroListadoDTO.Stock), "Stock", peso: 50, derecha: true);
            GrillaHelper.Columna(dgvListado, nameof(LibroListadoDTO.StockMinimo), "Mínimo", peso: 50, derecha: true);
            GrillaHelper.Columna(dgvListado, nameof(LibroListadoDTO.Proveedores), "Prov.", peso: 40, derecha: true);
            GrillaHelper.Columna(dgvListado, nameof(LibroListadoDTO.Estado), "Estado", peso: 60);
        }

        private async void FrmGestionarLibros_Load(object sender, EventArgs e)
        {
            btnNuevo.Visible = PermisoService.Instancia.TienePermiso("AgregarLibro");
            btnEditar.Visible = PermisoService.Instancia.TienePermiso("ModificarLibro");
            btnCambiarEstado.Visible = PermisoService.Instancia.TienePermiso("EliminarLibro");

            try
            {
                var generos = await servicio.ObtenerGenerosAsync(listado.Token);
                cbFiltroExtra.DataSource = new[] { TodosLosGeneros }.Concat(generos).ToList();
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudieron cargar los géneros.");
            }

            await listado.IniciarAsync();
        }

        /// <summary>Abre el modal único (null = alta). Devuelve el id guardado, o null si se canceló.</summary>
        private int? AbrirEdicion(int? id)
        {
            using var frm = new FrmEditarLibro(id);
            return frm.ShowDialog(this) == DialogResult.OK ? frm.LibroId : null;
        }

        private void Salir()
        {
            if (TopLevelControl is FrmMenu principal)
                principal.MostrarInicio();   // cierra y libera esta sección
            else
                Close();
        }
    }
}
