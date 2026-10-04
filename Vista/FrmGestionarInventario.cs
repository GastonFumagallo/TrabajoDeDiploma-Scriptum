using Controladora;
using Modelo;
using ScottPlot;
using ScottPlot.WinForms;
using Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Vista.Inventario;

namespace Vista
{
    /// <summary>
    /// Centro de control del inventario.
    /// Pestaña Existencias: grilla con alertas por color, filtros (texto, categoría, proveedor, estado),
    /// indicadores, ajustes auditados, parámetros de reposición, historial y exportación.
    /// Pestaña Órdenes: listado de órdenes de reposición con alta, edición, recepción y cancelación.
    ///
    /// Toda la lógica de datos está en <see cref="InventarioService"/> y <see cref="OrdenReposicionService"/>.
    /// </summary>
    public partial class FrmGestionarInventario : Form
    {
        // Colores de alerta (fila tenue + celda "Estado" como badge).
        private static readonly System.Drawing.Color FondoAgotado = System.Drawing.Color.FromArgb(250, 210, 210);
        private static readonly System.Drawing.Color FondoCritico = System.Drawing.Color.FromArgb(252, 228, 200);
        private static readonly System.Drawing.Color FondoReponer = System.Drawing.Color.FromArgb(252, 246, 205);
        private static readonly System.Drawing.Color BadgeAgotado = System.Drawing.Color.FromArgb(200, 50, 50);
        private static readonly System.Drawing.Color BadgeCritico = System.Drawing.Color.FromArgb(220, 110, 20);
        private static readonly System.Drawing.Color BadgeReponer = System.Drawing.Color.FromArgb(190, 160, 20);
        private static readonly System.Drawing.Color BadgeNormal = System.Drawing.Color.FromArgb(40, 140, 70);

        private static readonly OpcionDTO Todos = new(0, "Todos");

        private readonly BindingSource bsInventario = new();
        private readonly BindingSource bsOrdenes = new();
        private readonly System.Windows.Forms.Timer timerFiltro = new() { Interval = 300 };
        private readonly CancellationTokenSource ctsFormulario = new();
        private CancellationTokenSource? ctsCarga;
        private FormsPlot? grafico;
        private Font? fuenteBadge;
        private bool inicializando = true;
        private bool ocupado;

        public FrmGestionarInventario()
        {
            InitializeComponent();
            ConfigurarGrillaInventario();
            ConfigurarGrillaOrdenes();
            ConfigurarFiltros();
            ConfigurarAcciones();

            grafico = new FormsPlot { Dock = DockStyle.Fill };
            panelGrafico.Controls.Add(grafico);

            FormClosing += (_, _) => { timerFiltro.Stop(); ctsFormulario.Cancel(); };
            Disposed += (_, _) => { timerFiltro.Dispose(); fuenteBadge?.Dispose(); ctsFormulario.Dispose(); };
        }

        private static string Usuario => PermisoService.Instancia.UsuarioActual?.USU_Nombre ?? "desconocido";

        #region Configuración

        private static void DobleBuffer(DataGridView dgv) =>
            typeof(DataGridView).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(dgv, true);

        private void ConfigurarGrillaInventario()
        {
            DobleBuffer(dgvInventario);
            dgvInventario.MultiSelect = true;   // se pueden elegir varios libros para una orden
            dgvInventario.DataSource = bsInventario;

            dgvInventario.DataBindingComplete += (_, _) =>
            {
                Ocultar(dgvInventario, "ProveedorHabitualId", "PrecioVenta", "Estado", "ValorCosto");
                Columna(dgvInventario, "LibroId", "ID", ancho: 45);
                Columna(dgvInventario, "Codigo", "Código / ISBN", ancho: 110);
                Columna(dgvInventario, "Titulo", "Título", ancho: 220);
                Columna(dgvInventario, "Categoria", "Categoría");
                Columna(dgvInventario, "ProveedorHabitual", "Proveedor habitual", ancho: 140);
                Columna(dgvInventario, "Stock", "Stock", numero: true, ancho: 60);
                Columna(dgvInventario, "StockMinimo", "Mínimo", numero: true, ancho: 60);
                Columna(dgvInventario, "PuntoReposicion", "Pto. repos.", numero: true, ancho: 70);
                Columna(dgvInventario, "StockOptimo", "Óptimo", numero: true, ancho: 60);
                Columna(dgvInventario, "EnPedido", "En pedido", numero: true, ancho: 65);
                Columna(dgvInventario, "CantidadSugerida", "A pedir", numero: true, ancho: 60);
                Columna(dgvInventario, "PrecioCosto", "Costo", numero: true, formato: "N2", ancho: 80);
                Columna(dgvInventario, "EstadoTexto", "Estado", ancho: 90);
                if (dgvInventario.Columns["EstadoTexto"] is DataGridViewColumn estado)
                    estado.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            };

            // Formato condicional: fila tenue según el estado y la celda Estado como badge de color fuerte.
            dgvInventario.CellFormatting += (_, e) =>
            {
                if (e.RowIndex < 0 || e.CellStyle == null || dgvInventario.Rows[e.RowIndex].DataBoundItem is not ProductoInventarioDTO p)
                    return;

                var fondo = p.Estado switch
                {
                    EstadoStock.Agotado => FondoAgotado,
                    EstadoStock.Critico => FondoCritico,
                    EstadoStock.AReponer => FondoReponer,
                    _ => System.Drawing.Color.White,
                };
                e.CellStyle.BackColor = fondo;
                e.CellStyle.ForeColor = System.Drawing.Color.Black;

                if (dgvInventario.Columns[e.ColumnIndex].Name == "EstadoTexto")
                {
                    e.CellStyle.BackColor = p.Estado switch
                    {
                        EstadoStock.Agotado => BadgeAgotado,
                        EstadoStock.Critico => BadgeCritico,
                        EstadoStock.AReponer => BadgeReponer,
                        _ => BadgeNormal,
                    };
                    e.CellStyle.ForeColor = System.Drawing.Color.White;
                    e.CellStyle.Font = fuenteBadge ??= new Font(dgvInventario.Font, System.Drawing.FontStyle.Bold);
                    e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
                }
            };
            dgvInventario.CellToolTipTextNeeded += (_, e) =>
            {
                if (e.RowIndex >= 0 && dgvInventario.Rows[e.RowIndex].DataBoundItem is ProductoInventarioDTO p && p.EnPedido > 0)
                    e.ToolTipText = $"Ya hay {p.EnPedido} unidad(es) pedidas en órdenes activas.";
            };
            dgvInventario.SelectionChanged += (_, _) => ActualizarAcciones();
            dgvInventario.CellDoubleClick += async (_, e) => { if (e.RowIndex >= 0) await AjustarStockAsync(); };
        }

        private void ConfigurarGrillaOrdenes()
        {
            DobleBuffer(dgvOrdenes);
            dgvOrdenes.DataSource = bsOrdenes;
            dgvOrdenes.DataBindingComplete += (_, _) =>
            {
                Ocultar(dgvOrdenes, "OrdenId", "EstadoCodigo");
                Columna(dgvOrdenes, "Numero", "N° orden", ancho: 90);
                Columna(dgvOrdenes, "Fecha", "Emisión", formato: "dd/MM/yyyy HH:mm", ancho: 120);
                Columna(dgvOrdenes, "Unidades", "Unidades", numero: true);
                Columna(dgvOrdenes, "Items", "Ítems", numero: true);
                Columna(dgvOrdenes, "TotalEstimado", "Total estimado", numero: true, formato: "N2");
                Columna(dgvOrdenes, "Usuario", "Solicitante");
                Columna(dgvOrdenes, "FechaRecepcion", "Recepción", formato: "dd/MM/yyyy HH:mm");
            };
            dgvOrdenes.CellFormatting += (_, e) =>
            {
                if (e.RowIndex < 0 || e.CellStyle == null || dgvOrdenes.Rows[e.RowIndex].DataBoundItem is not OrdenReposicionDTO o) return;
                e.CellStyle.ForeColor = o.EstadoCodigo switch
                {
                    EstadoOrden.Cancelada => System.Drawing.Color.Gray,
                    EstadoOrden.Recibida => System.Drawing.Color.DarkGreen,
                    EstadoOrden.Solicitada => System.Drawing.Color.DarkBlue,
                    _ => System.Drawing.Color.Black,
                };
            };
            dgvOrdenes.SelectionChanged += (_, _) => ActualizarAcciones();
            dgvOrdenes.CellDoubleClick += async (_, e) => { if (e.RowIndex >= 0) await AbrirOrdenAsync(iniciarRecepcion: false); };
        }

        private static void Ocultar(DataGridView dgv, params string[] columnas)
        {
            foreach (var c in columnas)
                if (dgv.Columns[c] is DataGridViewColumn col) col.Visible = false;
        }

        private static void Columna(DataGridView dgv, string nombre, string encabezado, bool numero = false, string? formato = null, int? ancho = null)
        {
            if (dgv.Columns[nombre] is not DataGridViewColumn col) return;
            col.HeaderText = encabezado;
            if (formato != null) col.DefaultCellStyle.Format = formato;
            if (numero) col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            if (ancho is int a) col.FillWeight = a;
        }

        private void ConfigurarFiltros()
        {
            cbEstado.Items.AddRange(new object[] { "Todos", "Requieren reposición", "Solo bajo stock mínimo", "Agotados" });
            cbEstado.SelectedIndex = 0;
            cbEstadoOrden.Items.AddRange(new object[] { "Activas", "Todas", "Pendientes (borrador)", "Solicitadas", "Recibidas", "Canceladas" });
            cbEstadoOrden.SelectedIndex = 0;

            txtBuscar.TextChanged += (_, _) => { if (!inicializando) { timerFiltro.Stop(); timerFiltro.Start(); } };
            timerFiltro.Tick += async (_, _) => { timerFiltro.Stop(); await CargarInventarioAsync(); };
            cbCategoria.SelectedIndexChanged += async (_, _) => await CargarInventarioAsync();
            cbProveedor.SelectedIndexChanged += async (_, _) => await CargarInventarioAsync();
            cbEstado.SelectedIndexChanged += async (_, _) => await CargarInventarioAsync();
            cbEstadoOrden.SelectedIndexChanged += async (_, _) => await CargarOrdenesAsync();
            cbProveedorOrden.SelectedIndexChanged += async (_, _) => await CargarOrdenesAsync();
            btnLimpiarFiltros.Click += async (_, _) =>
            {
                inicializando = true;
                txtBuscar.Clear();
                cbCategoria.SelectedIndex = cbProveedor.SelectedIndex = cbEstado.SelectedIndex = 0;
                inicializando = false;
                await CargarInventarioAsync();
            };
        }

        private void ConfigurarAcciones()
        {
            btnSalir.Click += (_, _) => Salir();
            btnAjustarStock.Click += async (_, _) => await AjustarStockAsync();
            btnParametros.Click += async (_, _) => await EditarParametrosAsync();
            btnHistorial.Click += async (_, _) => await VerHistorialAsync();
            btnVerProveedores.Click += async (_, _) => await VerProveedoresAsync();
            btnOrdenReposicion.Click += async (_, _) => await GenerarOrdenAsync();
            btnExportarExcel.Click += async (_, _) => await ExportarAsync(pdf: false);
            btnExportarPdf.Click += async (_, _) => await ExportarAsync(pdf: true);

            btnNuevaOrden.Click += async (_, _) => await AbrirFormularioOrdenAsync(new FrmOrdenReposicion());
            btnAbrirOrden.Click += async (_, _) => await AbrirOrdenAsync(iniciarRecepcion: false);
            btnRegistrarRecepcion.Click += async (_, _) => await AbrirOrdenAsync(iniciarRecepcion: true);
            btnCancelarOrden.Click += async (_, _) => await CancelarOrdenAsync();
        }

        private async void FrmGestionarInventario_Load(object sender, EventArgs e)
        {
            AplicarSeguridad();
            try
            {
                UseWaitCursor = true;
                var generos = await InventarioService.Instancia.ObtenerGenerosAsync(ctsFormulario.Token);
                var proveedores = await OrdenReposicionService.Instancia.ObtenerProveedoresAsync(incluirInactivos: true, ctsFormulario.Token);
                var opcionesProveedor = new[] { Todos }.Concat(proveedores.Select(p => new OpcionDTO(p.ProveedorId, p.ToString()))).ToList();

                cbCategoria.DataSource = new[] { Todos }.Concat(generos).ToList();
                cbProveedor.DataSource = opcionesProveedor;
                cbProveedorOrden.DataSource = opcionesProveedor.ToList();
            }
            catch (OperationCanceledException) { return; }
            catch (Exception ex)
            {
                MostrarError("No se pudieron cargar los filtros.", ex);
            }
            finally
            {
                UseWaitCursor = false;
            }

            inicializando = false;
            await Task.WhenAll(CargarInventarioAsync(), CargarOrdenesAsync());
        }

        private void AplicarSeguridad()
        {
            var permisos = PermisoService.Instancia;
            btnVerProveedores.Visible = permisos.TienePermiso("VerProveedores");
            btnOrdenReposicion.Visible = btnNuevaOrden.Visible = permisos.TienePermiso("GenerarOrdenReposicion");
            btnRegistrarRecepcion.Visible = permisos.TienePermiso("RegistrarRecepcionOrden");
            btnCancelarOrden.Visible = permisos.TienePermiso("CancelarOrden");
            btnAjustarStock.Visible = btnParametros.Visible = permisos.TienePermiso("AjustarStock");
            btnExportarExcel.Visible = btnExportarPdf.Visible = permisos.TienePermiso("ExportarInventario");
        }

        #endregion

        #region Carga de datos

        private FiltroInventario ConstruirFiltro() => new()
        {
            Texto = txtBuscar.Text.Trim(),
            GeneroId = cbCategoria.SelectedItem is OpcionDTO { Id: > 0 } g ? g.Id : null,
            ProveedorId = cbProveedor.SelectedItem is OpcionDTO { Id: > 0 } p ? p.Id : null,
            Estado = (FiltroEstadoStock)Math.Max(0, cbEstado.SelectedIndex),
        };

        private async Task CargarInventarioAsync(IEnumerable<int>? reseleccionar = null)
        {
            if (inicializando) return;

            ctsCarga?.Cancel();
            ctsCarga = CancellationTokenSource.CreateLinkedTokenSource(ctsFormulario.Token);
            var token = ctsCarga.Token;
            var seleccion = (reseleccionar ?? LibrosSeleccionados.Select(p => p.LibroId)).ToHashSet();

            try
            {
                UseWaitCursor = true;
                var productosTask = InventarioService.Instancia.ObtenerInventarioAsync(ConstruirFiltro(), token);
                var resumenTask = InventarioService.Instancia.ObtenerResumenAsync(token);
                await Task.WhenAll(productosTask, resumenTask);
                if (token.IsCancellationRequested) return;

                var productos = productosTask.Result;
                bsInventario.DataSource = productos;
                MostrarResumen(resumenTask.Result);
                lblResumenInventario.Text =
                    $"{productos.Count} producto(s) mostrados · Valor a costo de lo filtrado: ${productos.Sum(p => p.ValorCosto):N2} · " +
                    "Seleccioná varios con Ctrl/Shift para generar una orden con esos libros.";

                dgvInventario.ClearSelection();
                foreach (DataGridViewRow fila in dgvInventario.Rows)
                    if (fila.DataBoundItem is ProductoInventarioDTO p && seleccion.Contains(p.LibroId))
                        fila.Selected = true;
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                MostrarError("No se pudo cargar el inventario.", ex);
            }
            finally
            {
                if (!token.IsCancellationRequested)
                {
                    UseWaitCursor = false;
                    ActualizarAcciones();
                }
            }
        }

        private void MostrarResumen(ResumenInventarioDTO r)
        {
            lblIndTotal.Text = $"Productos\n{r.Total:N0}";
            lblIndNormales.Text = $"Stock normal\n{r.Normales:N0}";
            lblIndReponer.Text = $"A reponer\n{r.AReponer:N0}";
            lblIndCriticos.Text = $"Críticos (≤ mínimo)\n{r.Criticos:N0}";
            lblIndAgotados.Text = $"Sin stock\n{r.Agotados:N0}";
            lblIndValor.Text = $"Valor a costo\n${r.ValorCosto:N2}";
            DibujarGrafico(r);
        }

        private void DibujarGrafico(ResumenInventarioDTO r)
        {
            if (grafico == null) return;
            grafico.Plot.Clear();

            var porciones = new List<PieSlice>
            {
                new() { Value = r.Normales, Label = "Normal", FillColor = ScottPlot.Color.FromHex("#A8D5BA"), LegendText = $"Normal: {r.Normales}" },
                new() { Value = r.AReponer, Label = "A reponer", FillColor = ScottPlot.Color.FromHex("#E6D97A"), LegendText = $"A reponer: {r.AReponer}" },
                new() { Value = r.Criticos, Label = "Crítico", FillColor = ScottPlot.Color.FromHex("#F2B279"), LegendText = $"Crítico: {r.Criticos}" },
                new() { Value = r.Agotados, Label = "Sin stock", FillColor = ScottPlot.Color.FromHex("#F28C8C"), LegendText = $"Sin stock: {r.Agotados}" },
            }.Where(p => p.Value > 0).ToList();

            if (porciones.Count > 0)
            {
                var pie = grafico.Plot.Add.Pie(porciones);
                pie.SliceLabelDistance = 1.3;
            }
            grafico.Plot.Title("Estado del inventario");
            grafico.Plot.ShowLegend();
            grafico.Plot.Grid.MajorLineColor = ScottPlot.Colors.Transparent;
            grafico.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.EmptyTickGenerator();
            grafico.Plot.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.EmptyTickGenerator();
            grafico.Refresh();
        }

        private async Task CargarOrdenesAsync(int? seleccionarOrdenId = null)
        {
            if (inicializando) return;
            var filtro = new FiltroOrdenes
            {
                Estado = cbEstadoOrden.SelectedIndex switch
                {
                    0 => "ACTIVAS",
                    2 => EstadoOrden.Pendiente,
                    3 => EstadoOrden.Solicitada,
                    4 => EstadoOrden.Recibida,
                    5 => EstadoOrden.Cancelada,
                    _ => null,
                },
                ProveedorId = cbProveedorOrden.SelectedItem is OpcionDTO { Id: > 0 } p ? p.Id : null,
            };

            try
            {
                var ordenes = await OrdenReposicionService.Instancia.ObtenerOrdenesAsync(filtro, ctsFormulario.Token);
                bsOrdenes.DataSource = ordenes;
                tabOrdenes.Text = $"Órdenes de reposición ({ordenes.Count(o => EstadoOrden.EsActiva(o.EstadoCodigo))} activas)";

                if (seleccionarOrdenId is int id)
                    foreach (DataGridViewRow fila in dgvOrdenes.Rows)
                        if (fila.DataBoundItem is OrdenReposicionDTO o && o.OrdenId == id)
                            dgvOrdenes.CurrentCell = fila.Cells.Cast<DataGridViewCell>().First(c => c.Visible);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                MostrarError("No se pudieron cargar las órdenes de reposición.", ex);
            }
            finally
            {
                ActualizarAcciones();
            }
        }

        #endregion

        #region Acciones de inventario

        private List<ProductoInventarioDTO> LibrosSeleccionados =>
            dgvInventario.SelectedRows.Cast<DataGridViewRow>()
                .Select(r => r.DataBoundItem).OfType<ProductoInventarioDTO>().ToList();

        private ProductoInventarioDTO? LibroActual =>
            dgvInventario.CurrentRow?.DataBoundItem as ProductoInventarioDTO ?? LibrosSeleccionados.FirstOrDefault();

        private OrdenReposicionDTO? OrdenActual => dgvOrdenes.CurrentRow?.DataBoundItem as OrdenReposicionDTO;

        private void ActualizarAcciones()
        {
            bool hayLibro = LibroActual != null && !ocupado;
            btnAjustarStock.Enabled = btnParametros.Enabled = btnHistorial.Enabled = btnVerProveedores.Enabled = hayLibro;
            btnOrdenReposicion.Enabled = btnExportarExcel.Enabled = btnExportarPdf.Enabled = !ocupado;
            btnOrdenReposicion.Text = LibrosSeleccionados.Count > 1
                ? $"Generar orden ({LibrosSeleccionados.Count} libros)"
                : "Generar orden de reposición";

            var orden = OrdenActual;
            bool activa = orden != null && EstadoOrden.EsActiva(orden.EstadoCodigo) && !ocupado;
            btnAbrirOrden.Enabled = orden != null && !ocupado;
            btnRegistrarRecepcion.Enabled = activa;
            btnCancelarOrden.Enabled = activa;
        }

        private async Task AjustarStockAsync()
        {
            if (!btnAjustarStock.Visible || LibroActual is not ProductoInventarioDTO libro) return;

            using var dlg = new DlgAjusteStock(libro);
            if (dlg.ShowDialog(this) != DialogResult.OK || dlg.Resultado == null) return;

            var solicitud = new AjusteStockSolicitud
            {
                LibroId = dlg.Resultado.LibroId,
                Tipo = dlg.Resultado.Tipo,
                Cantidad = dlg.Resultado.Cantidad,
                Motivo = dlg.Resultado.Motivo,
                Observacion = dlg.Resultado.Observacion,
                Usuario = Usuario,
            };

            await EjecutarAsync(async () =>
            {
                var r = await InventarioService.Instancia.AjustarStockAsync(solicitud, ctsFormulario.Token);
                MostrarResultado(r, "Ajuste de stock");
                if (r.Exito) await CargarInventarioAsync(new[] { libro.LibroId });
            }, "No se pudo registrar el ajuste. No se modificó el stock.");
        }

        private async Task EditarParametrosAsync()
        {
            if (LibroActual is not ProductoInventarioDTO libro) return;

            using var dlg = new DlgParametrosStock(libro);
            if (dlg.ShowDialog(this) != DialogResult.OK || dlg.Resultado == null) return;

            await EjecutarAsync(async () =>
            {
                var r = await InventarioService.Instancia.ActualizarParametrosAsync(dlg.Resultado, ctsFormulario.Token);
                if (!r.Exito) MostrarResultado(r, "Parámetros de reposición");
                else await CargarInventarioAsync(new[] { libro.LibroId });
            }, "No se pudieron guardar los parámetros.");
        }

        private async Task VerHistorialAsync()
        {
            if (LibroActual is not ProductoInventarioDTO libro) return;
            await EjecutarAsync(async () =>
            {
                var movimientos = await InventarioService.Instancia.ObtenerMovimientosAsync(libro.LibroId, ct: ctsFormulario.Token);
                using var dlg = new DlgGrilla($"Movimientos de stock — {libro.Titulo}", movimientos, g =>
                {
                    Columna(g, "Fecha", "Fecha", formato: "dd/MM/yyyy HH:mm", ancho: 110);
                    Columna(g, "Cantidad", "Cantidad", numero: true, ancho: 60);
                    Columna(g, "StockAnterior", "Antes", numero: true, ancho: 55);
                    Columna(g, "StockResultante", "Después", numero: true, ancho: 55);
                    Columna(g, "Observacion", "Observación", ancho: 160);
                    Columna(g, "Referencia", "Comprobante", ancho: 90);
                });
                dlg.ShowDialog(this);
            }, "No se pudo cargar el historial.");
        }

        private async Task VerProveedoresAsync()
        {
            if (LibroActual is not ProductoInventarioDTO libro) return;
            await EjecutarAsync(async () =>
            {
                var proveedores = await InventarioService.Instancia.ObtenerProveedoresDeLibroAsync(libro.LibroId, ctsFormulario.Token);
                if (proveedores.Count == 0)
                {
                    MessageBox.Show("El libro no tiene proveedores asociados.", "Proveedores", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                using var dlg = new DlgGrilla($"Proveedores — {libro.Titulo}", proveedores,
                    g => Columna(g, "PrecioCompra", "Precio de compra", numero: true, formato: "N2"), new Size(820, 300));
                dlg.ShowDialog(this);
            }, "No se pudieron cargar los proveedores.");
        }

        /// <summary>
        /// Genera la orden con los libros seleccionados o, si no hay selección, con todos los que requieren reposición.
        /// Si todos son del mismo proveedor habitual se abre la orden para revisarla; si son de varios,
        /// se crean borradores (uno por proveedor) que luego se revisan en la pestaña Órdenes.
        /// </summary>
        private async Task GenerarOrdenAsync()
        {
            var libros = LibrosSeleccionados;
            if (libros.Count <= 1)
            {
                // Sin selección múltiple: se proponen todos los que necesitan reposición y aún no están cubiertos.
                List<ProductoInventarioDTO> faltantes;
                try
                {
                    faltantes = (await InventarioService.Instancia.ObtenerInventarioAsync(
                            new FiltroInventario { Estado = FiltroEstadoStock.RequierenReposicion }, ctsFormulario.Token))
                        .Where(p => p.CantidadSugerida > 0).ToList();
                }
                catch (Exception ex)
                {
                    MostrarError("No se pudo calcular qué libros reponer.", ex);
                    return;
                }

                if (faltantes.Count == 0 && libros.Count == 0)
                {
                    MessageBox.Show("No hay libros que requieran reposición (o ya están cubiertos por órdenes activas).\n\n" +
                                    "Para pedir libros puntuales, seleccionalos en la grilla.", "Reposición",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (faltantes.Count > 0)
                {
                    var r = MessageBox.Show(
                        $"Hay {faltantes.Count} libro(s) en o bajo el punto de reposición sin pedido suficiente.\n\n" +
                        "Sí: generar con todos ellos.\nNo: generar sólo con el libro seleccionado.",
                        "Generar orden de reposición", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (r == DialogResult.Cancel) return;
                    if (r == DialogResult.Yes || libros.Count == 0) libros = faltantes;
                }
            }

            var sinProveedor = libros.Where(l => l.ProveedorHabitualId == null).ToList();
            var grupos = libros.Where(l => l.ProveedorHabitualId != null).GroupBy(l => l.ProveedorHabitualId!.Value).ToList();

            if (grupos.Count == 0)
            {
                // Ningún libro tiene proveedor: se abre una orden vacía de proveedor para elegirlo a mano.
                await AbrirFormularioOrdenAsync(new FrmOrdenReposicion(proveedorId: null, libroIds: libros.Select(l => l.LibroId).ToList()));
                return;
            }

            if (grupos.Count == 1 && sinProveedor.Count == 0)
            {
                await AbrirFormularioOrdenAsync(new FrmOrdenReposicion(proveedorId: grupos[0].Key, libroIds: grupos[0].Select(l => l.LibroId).ToList()));
                return;
            }

            var detalle = string.Join("\n", grupos.Select(g => $"• {g.First().ProveedorHabitual}: {g.Count()} libro(s)"));
            var aviso = sinProveedor.Count > 0
                ? $"\n\nSin proveedor asociado (no se incluyen): {string.Join(", ", sinProveedor.Take(5).Select(s => s.Titulo))}{(sinProveedor.Count > 5 ? "…" : "")}"
                : "";
            if (MessageBox.Show($"Se crearán {grupos.Count} órdenes en borrador, una por proveedor habitual:\n\n{detalle}{aviso}\n\n¿Continuar?",
                    "Generar órdenes de reposición", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            await EjecutarAsync(async () =>
            {
                var (creadas, _) = await OrdenReposicionService.Instancia.GenerarBorradoresAsync(
                    grupos.SelectMany(g => g).Select(l => l.LibroId), Usuario, ctsFormulario.Token);
                MessageBox.Show($"Se generaron {creadas.Count} orden(es) en borrador: {string.Join(", ", creadas.Select(OrdenReposicion.FormatearNumero))}.\n\n" +
                                "Revisalas y emitilas desde la pestaña Órdenes de reposición.", "Órdenes generadas",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                cbEstadoOrden.SelectedIndex = 0;
                tabControl.SelectedTab = tabOrdenes;
                await Task.WhenAll(CargarOrdenesAsync(creadas.FirstOrDefault()), CargarInventarioAsync());
            }, "No se pudieron generar las órdenes.");
        }

        private async Task ExportarAsync(bool pdf)
        {
            if (bsInventario.DataSource is not List<ProductoInventarioDTO> productos || productos.Count == 0)
            {
                MessageBox.Show("No hay productos para exportar con los filtros actuales.", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var dialogo = new SaveFileDialog
            {
                Filter = pdf ? "PDF|*.pdf" : "Archivo CSV (Excel)|*.csv",
                FileName = $"Existencias_{DateTime.Now:yyyyMMdd_HHmm}.{(pdf ? "pdf" : "csv")}",
            };
            if (dialogo.ShowDialog(this) != DialogResult.OK) return;

            string descripcionFiltro = string.Join(" · ", new[]
            {
                string.IsNullOrWhiteSpace(txtBuscar.Text) ? null : $"Texto \"{txtBuscar.Text.Trim()}\"",
                cbCategoria.SelectedIndex > 0 ? $"Categoría {cbCategoria.Text}" : null,
                cbProveedor.SelectedIndex > 0 ? $"Proveedor {cbProveedor.Text}" : null,
                cbEstado.SelectedIndex > 0 ? cbEstado.Text : null,
            }.Where(s => s != null)) is { Length: > 0 } f ? f : "Todos los productos";

            await EjecutarAsync(async () =>
            {
                if (pdf)
                    await Task.Run(() => ExportadorInventario.ExportarPdf(dialogo.FileName, productos, descripcionFiltro));
                else
                    await ExportadorInventario.ExportarCsvAsync(dialogo.FileName, productos, ctsFormulario.Token);
                MessageBox.Show($"Se exportaron {productos.Count} producto(s).", "Exportación completa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }, "No se pudo exportar. Verificá que el archivo no esté abierto en otro programa.");
        }

        #endregion

        #region Acciones de órdenes

        private async Task AbrirOrdenAsync(bool iniciarRecepcion)
        {
            if (OrdenActual is not OrdenReposicionDTO orden) return;
            await AbrirFormularioOrdenAsync(new FrmOrdenReposicion(orden.OrdenId, iniciarRecepcion));
        }

        /// <summary>Abre la orden como diálogo y, al cerrar, refresca órdenes e inventario (la recepción cambia stock).</summary>
        private async Task AbrirFormularioOrdenAsync(FrmOrdenReposicion frm)
        {
            using (frm)
                frm.ShowDialog(this);

            await Task.WhenAll(CargarOrdenesAsync(frm.OrdenId), CargarInventarioAsync());
        }

        private async Task CancelarOrdenAsync()
        {
            if (OrdenActual is not OrdenReposicionDTO orden) return;

            string? motivo = DlgTexto.Pedir(this, $"Cancelar {orden.Numero}", "Motivo de la cancelación (obligatorio):");
            if (motivo == null) return;
            if (MessageBox.Show($"¿Cancelar la orden {orden.Numero} a {orden.Proveedor}?\nEsta acción no se puede deshacer.",
                    "Cancelar orden", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            await EjecutarAsync(async () =>
            {
                var r = await OrdenReposicionService.Instancia.CancelarAsync(orden.OrdenId, motivo, Usuario, ctsFormulario.Token);
                MostrarResultado(r, "Cancelar orden");
                await Task.WhenAll(CargarOrdenesAsync(orden.OrdenId), CargarInventarioAsync());
            }, "No se pudo cancelar la orden.");
        }

        #endregion

        #region Utilidades

        /// <summary>Ejecuta una operación bloqueando la UI, con manejo uniforme de errores.</summary>
        private async Task EjecutarAsync(Func<Task> accion, string mensajeError)
        {
            if (ocupado) return;
            ocupado = true;
            UseWaitCursor = true;
            ActualizarAcciones();
            try
            {
                await accion();
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                MostrarError(mensajeError, ex);
            }
            finally
            {
                ocupado = false;
                UseWaitCursor = false;
                ActualizarAcciones();
            }
        }

        private void MostrarResultado(ResultadoOperacion r, string titulo) =>
            MessageBox.Show(string.IsNullOrEmpty(r.Mensaje) ? (r.Exito ? "Operación realizada." : "No se pudo completar la operación.") : r.Mensaje,
                titulo, MessageBoxButtons.OK, r.Exito ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

        private void MostrarError(string mensaje, Exception ex) =>
            MessageBox.Show($"{mensaje}\n\nDetalle técnico: {ex.GetBaseException().Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

        private void Salir()
        {
            if (TopLevelControl is FrmMenu principal)
                principal.MostrarInicio();   // cierra y libera esta sección
            else
                Close();
        }

        #endregion
    }
}
