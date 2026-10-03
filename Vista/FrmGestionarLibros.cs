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
    /// Gestión de libros (plantilla de los ABM maestros):
    /// barra de acciones (Nuevo, Editar, Activar/Desactivar, Exportar), filtros en tiempo real resueltos en SQL,
    /// grilla con columnas explícitas y modal único <see cref="FrmEditarLibro"/> para alta y edición.
    /// Ningún acceso a datos acá: todo pasa por <see cref="LibroService"/>.
    ///
    /// Atajos: Ctrl+N nuevo · Enter / doble clic editar · Supr activar/desactivar · F5 refrescar.
    /// </summary>
    public partial class FrmGestionarLibros : Form
    {
        private static readonly OpcionDTO TodosLosGeneros = new(0, "Todos");

        private readonly LibroService servicio = LibroService.Instancia;
        private readonly BindingSource bsLibros = new();
        private readonly System.Windows.Forms.Timer timerFiltro = new() { Interval = 300 };
        private readonly CancellationTokenSource ctsFormulario = new();
        private CancellationTokenSource? ctsCarga;
        private Font? fuenteInactivo;
        private bool inicializando = true;

        public FrmGestionarLibros()
        {
            InitializeComponent();
            ConfigurarGrilla();
            ConfigurarFiltros();

            btnNuevo.Click += async (_, _) => await AbrirEdicionAsync(null);
            btnEditar.Click += async (_, _) => await EditarSeleccionadoAsync();
            btnCambiarEstado.Click += async (_, _) => await CambiarEstadoAsync();
            btnExportar.Click += async (_, _) => await ExportarAsync();
            btnSalir.Click += (_, _) => Salir();
            KeyDown += FrmGestionarLibros_KeyDown;

            FormClosing += (_, _) => { timerFiltro.Stop(); ctsFormulario.Cancel(); };
            Disposed += (_, _) => { timerFiltro.Dispose(); fuenteInactivo?.Dispose(); ctsFormulario.Dispose(); };
        }

        private LibroListadoDTO? Seleccionado => dgvLibros.CurrentRow?.DataBoundItem as LibroListadoDTO;

        #region Configuración

        private void ConfigurarGrilla()
        {
            GrillaHelper.ConfigurarListado(dgvLibros);
            GrillaHelper.Columna(dgvLibros, nameof(LibroListadoDTO.Id), "ID", peso: 40, derecha: true);
            GrillaHelper.Columna(dgvLibros, nameof(LibroListadoDTO.ISBN), "ISBN", peso: 100);
            GrillaHelper.Columna(dgvLibros, nameof(LibroListadoDTO.Titulo), "Título", peso: 230);
            GrillaHelper.Columna(dgvLibros, nameof(LibroListadoDTO.Autor), "Autor", peso: 140);
            GrillaHelper.Columna(dgvLibros, nameof(LibroListadoDTO.Editorial), "Editorial", peso: 110);
            GrillaHelper.Columna(dgvLibros, nameof(LibroListadoDTO.Genero), "Género", peso: 90);
            GrillaHelper.Columna(dgvLibros, nameof(LibroListadoDTO.PrecioCosto), "Costo", peso: 70, formato: "N2", derecha: true);
            GrillaHelper.Columna(dgvLibros, nameof(LibroListadoDTO.PrecioVenta), "Venta", peso: 70, formato: "N2", derecha: true);
            GrillaHelper.Columna(dgvLibros, nameof(LibroListadoDTO.Margen), "Margen %", peso: 60, formato: "N1", derecha: true);
            GrillaHelper.Columna(dgvLibros, nameof(LibroListadoDTO.Stock), "Stock", peso: 50, derecha: true);
            GrillaHelper.Columna(dgvLibros, nameof(LibroListadoDTO.StockMinimo), "Mínimo", peso: 50, derecha: true);
            GrillaHelper.Columna(dgvLibros, nameof(LibroListadoDTO.Proveedores), "Prov.", peso: 40, derecha: true);
            GrillaHelper.Columna(dgvLibros, nameof(LibroListadoDTO.Estado), "Estado", peso: 60);
            dgvLibros.DataSource = bsLibros;

            // Inactivos en gris y cursiva; margen no positivo en rojo (alerta de precio mal cargado).
            dgvLibros.CellFormatting += (_, e) =>
            {
                if (e.RowIndex < 0 || e.CellStyle == null || dgvLibros.Rows[e.RowIndex].DataBoundItem is not LibroListadoDTO l) return;
                if (!l.Activo)
                {
                    e.CellStyle.ForeColor = Color.Gray;
                    e.CellStyle.Font = fuenteInactivo ??= new Font(dgvLibros.Font, FontStyle.Italic);
                }
                else if (dgvLibros.Columns[e.ColumnIndex].Name == nameof(LibroListadoDTO.Margen) && l.Margen is <= 0)
                {
                    e.CellStyle.ForeColor = Color.Firebrick;
                }
            };

            dgvLibros.SelectionChanged += (_, _) => ActualizarAcciones();
            dgvLibros.CellDoubleClick += async (_, e) => { if (e.RowIndex >= 0) await EditarSeleccionadoAsync(); };
        }

        private void ConfigurarFiltros()
        {
            cbEstado.Items.AddRange(new object[] { "Activos", "Inactivos", "Todos" });
            cbEstado.SelectedIndex = 0;

            // Búsqueda reactiva: espera 300 ms sin tipear y consulta (la anterior se cancela).
            txtBuscar.TextChanged += (_, _) => { timerFiltro.Stop(); timerFiltro.Start(); };
            timerFiltro.Tick += async (_, _) => { timerFiltro.Stop(); await CargarAsync(); };
            txtBuscar.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Down && dgvLibros.Rows.Count > 0) { e.SuppressKeyPress = true; dgvLibros.Focus(); }
            };
            cbGenero.SelectedIndexChanged += async (_, _) => await CargarAsync();
            cbEstado.SelectedIndexChanged += async (_, _) => await CargarAsync();
            btnLimpiarFiltros.Click += async (_, _) =>
            {
                inicializando = true;
                txtBuscar.Clear();
                cbGenero.SelectedIndex = 0;
                cbEstado.SelectedIndex = 0;
                inicializando = false;
                await CargarAsync();
                txtBuscar.Focus();
            };
        }

        private async void FrmGestionarLibros_Load(object sender, EventArgs e)
        {
            AplicarSeguridad();
            try
            {
                var generos = await servicio.ObtenerGenerosAsync(ctsFormulario.Token);
                cbGenero.DataSource = new[] { TodosLosGeneros }.Concat(generos).ToList();
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudieron cargar los géneros.");
            }

            inicializando = false;
            await CargarAsync();
            txtBuscar.Focus();
        }

        private void AplicarSeguridad()
        {
            btnNuevo.Visible = PermisoService.Instancia.TienePermiso("AgregarLibro");
            btnEditar.Visible = PermisoService.Instancia.TienePermiso("ModificarLibro");
            btnCambiarEstado.Visible = PermisoService.Instancia.TienePermiso("EliminarLibro");
        }

        #endregion

        #region Carga

        private FiltroLibros ConstruirFiltro() => new()
        {
            Texto = txtBuscar.Text,
            GeneroId = cbGenero.SelectedItem is OpcionDTO { Id: > 0 } g ? g.Id : null,
            Estado = (FiltroEstadoActivo)Math.Max(0, cbEstado.SelectedIndex),
        };

        /// <summary>Recarga la grilla con el filtro actual, conservando (o eligiendo) la fila seleccionada.</summary>
        private async Task CargarAsync(int? seleccionarId = null)
        {
            if (inicializando) return;

            ctsCarga?.Cancel();
            ctsCarga = CancellationTokenSource.CreateLinkedTokenSource(ctsFormulario.Token);
            var token = ctsCarga.Token;
            int? idActual = seleccionarId ?? Seleccionado?.Id;

            try
            {
                UseWaitCursor = true;
                var libros = await servicio.ObtenerTodosAsync(ConstruirFiltro(), token);
                if (token.IsCancellationRequested) return;

                bsLibros.DataSource = libros;
                lblResumen.Text = libros.Count == 0
                    ? "No hay libros que coincidan con los filtros."
                    : $"{libros.Count} libro(s) · Doble clic o Enter para editar · Supr para activar/desactivar.";
                if (idActual is int id)
                    GrillaHelper.Seleccionar<LibroListadoDTO>(dgvLibros, l => l.Id == id);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo cargar el listado de libros.");
            }
            catch (OperationCanceledException) { }
            finally
            {
                if (!token.IsCancellationRequested)
                {
                    UseWaitCursor = false;
                    ActualizarAcciones();
                }
            }
        }

        private void ActualizarAcciones()
        {
            var libro = Seleccionado;
            btnEditar.Enabled = libro != null;
            btnCambiarEstado.Enabled = libro != null;
            btnCambiarEstado.Text = libro is { Activo: false } ? "Reactivar (Supr)" : "Desactivar (Supr)";
            btnExportar.Enabled = bsLibros.Count > 0;
        }

        #endregion

        #region Acciones

        private async Task EditarSeleccionadoAsync()
        {
            if (!btnEditar.Visible || Seleccionado is not LibroListadoDTO libro) return;
            await AbrirEdicionAsync(libro.Id);
        }

        /// <summary>Abre el modal de alta (null) o edición (id). Si se guardó, recarga y selecciona el libro.</summary>
        private async Task AbrirEdicionAsync(int? libroId)
        {
            if (libroId == null && !btnNuevo.Visible) return;

            using var frm = new FrmEditarLibro(libroId);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                // Si se reactivó un libro inactivo desde el alta, se muestra aunque el filtro sea "Activos".
                await CargarAsync(frm.LibroId);
            }
        }

        private async Task CambiarEstadoAsync()
        {
            if (!btnCambiarEstado.Visible || Seleccionado is not LibroListadoDTO libro) return;

            bool activar = !libro.Activo;
            string mensaje = activar
                ? $"¿Reactivar \"{libro.Titulo}\"?\nVolverá a estar disponible para la venta y la reposición."
                : $"¿Desactivar \"{libro.Titulo}\"?\n\n" +
                  "• No se podrá vender ni incluir en órdenes de reposición.\n" +
                  "• Se conserva todo su historial (ventas, órdenes, movimientos).\n" +
                  "• Se puede reactivar en cualquier momento." +
                  (libro.Stock > 0 ? $"\n\nAtención: todavía tiene {libro.Stock} unidad(es) en stock." : "");

            if (MessageBox.Show(this, mensaje, activar ? "Reactivar libro" : "Desactivar libro", MessageBoxButtons.YesNo,
                    activar ? MessageBoxIcon.Question : MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            try
            {
                await servicio.CambiarEstadoAsync(libro.Id, activar, ctsFormulario.Token);
                await CargarAsync(libro.Id);
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo cambiar el estado del libro.");
            }
        }

        private async Task ExportarAsync()
        {
            using var dialogo = new SaveFileDialog
            {
                Filter = "Archivo CSV (Excel)|*.csv",
                FileName = $"Libros_{DateTime.Now:yyyyMMdd_HHmm}.csv",
            };
            if (dialogo.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                int filas = await ExportadorGrilla.ExportarCsvAsync(dgvLibros, dialogo.FileName, ctsFormulario.Token);
                MessageBox.Show(this, $"Se exportaron {filas} libro(s).", "Exportación completa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (IOException ex)
            {
                MessageBox.Show(this, $"No se pudo escribir el archivo. ¿Está abierto en Excel?\n\n{ex.Message}", "Exportar",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo exportar el listado.");
            }
        }

        private async void FrmGestionarLibros_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.N) { e.Handled = e.SuppressKeyPress = true; await AbrirEdicionAsync(null); }
            else if (e.KeyCode == Keys.F5) { e.Handled = true; await CargarAsync(); }
            else if (dgvLibros.Focused && e.KeyCode == Keys.Enter) { e.Handled = e.SuppressKeyPress = true; await EditarSeleccionadoAsync(); }
            else if (dgvLibros.Focused && e.KeyCode == Keys.Delete) { e.Handled = true; await CambiarEstadoAsync(); }
        }

        private void Salir()
        {
            if (Application.OpenForms["FrmMenu"] is FrmMenu principal)
                principal.MostrarInicio();
            Close();
        }

        #endregion
    }
}
