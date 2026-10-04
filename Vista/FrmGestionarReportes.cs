using Controladora;
using Controladora.Abm;
using Modelo;
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
using Vista.Comun;

namespace Vista
{
    /// <summary>
    /// Centro de Reportes. La pantalla no calcula nada: arma <see cref="ParametrosReporte"/>, llama a
    /// <see cref="ReporteService"/> (todo agregado en SQL, asíncrono y cancelable) y dibuja el
    /// <see cref="ResultadoReporte"/> de forma genérica: tarjetas de KPI, grilla ordenable con formatos y gráficos.
    /// Cambiar cualquier parámetro regenera el reporte automáticamente (con una pequeña espera para no
    /// consultar en cada clic); F5 o "Generar" lo fuerzan.
    /// </summary>
    public partial class FrmGestionarReportes : Form
    {
        #region Catálogo de reportes

        /// <summary>Qué parámetros usa cada reporte (los demás filtros se ocultan).</summary>
        private sealed record DefinicionReporte(TipoReporte Tipo, string Nombre, bool Fechas = true, bool Agrupacion = false,
            bool MedioPago = false, bool Categoria = false, bool Proveedor = false, bool Top = false, bool Criterio = false,
            bool ConsumidorFinal = false)
        {
            public override string ToString() => Nombre;
        }

        private static readonly DefinicionReporte[] Reportes =
        {
            new(TipoReporte.VentasPorPeriodo, "Ventas y facturación por período", Agrupacion: true, MedioPago: true),
            new(TipoReporte.VentasPorMedioPago, "Ventas por medio de pago"),
            new(TipoReporte.RankingProductos, "Ranking de productos más vendidos", MedioPago: true, Categoria: true, Top: true, Criterio: true),
            new(TipoReporte.StockSinMovimiento, "Stock sin movimiento (baja rotación)", Categoria: true),
            new(TipoReporte.ValorizacionStock, "Valorización del stock actual", Fechas: false, Categoria: true),
            new(TipoReporte.MejoresClientes, "Mejores clientes", MedioPago: true, Top: true, ConsumidorFinal: true),
            new(TipoReporte.ComprasPorProveedor, "Compras por proveedor", Proveedor: true),
            new(TipoReporte.OrdenesReposicion, "Órdenes de reposición emitidas", Proveedor: true),
        };

        private enum Rango { Hoy, EstaSemana, EsteMes, MesAnterior, Ultimos30Dias, AnioActual, Personalizado }

        private sealed record OpcionRango(Rango Valor, string Texto)
        {
            public override string ToString() => Texto;
        }

        private static readonly OpcionDTO Todos = new(0, "Todos");

        // Colores de las tarjetas de KPI (se aplican al dibujar, después del tema general).
        private static readonly Color[] ColoresKpi =
        {
            Color.FromArgb(41, 128, 185), Color.FromArgb(39, 174, 96), Color.FromArgb(142, 68, 173), Color.FromArgb(211, 84, 0),
        };

        #endregion

        private readonly ReporteService servicio = ReporteService.Instancia;
        private readonly FormsPlot[] graficos = new FormsPlot[3];
        private readonly System.Windows.Forms.Timer timerRegenerar = new() { Interval = 400 };
        private readonly CancellationTokenSource ctsFormulario = new();
        private CancellationTokenSource? ctsReporte;
        private ResultadoReporte? ultimo;
        private bool inicializando = true;
        private bool fijandoRango;
        private bool ocupado;

        private (Panel Tarjeta, Label Titulo, Label Valor, Label Detalle)[] Tarjetas => new[]
        {
            (panelKpi1, lblKpiTitulo1, lblKpiValor1, lblKpiDetalle1),
            (panelKpi2, lblKpiTitulo2, lblKpiValor2, lblKpiDetalle2),
            (panelKpi3, lblKpiTitulo3, lblKpiValor3, lblKpiDetalle3),
            (panelKpi4, lblKpiTitulo4, lblKpiValor4, lblKpiDetalle4),
        };

        public FrmGestionarReportes()
        {
            InitializeComponent();
            ConfigurarGrilla();
            ConfigurarGraficos();
            ConfigurarParametros();

            btnGenerar.Click += async (_, _) => await GenerarAsync();
            btnLimpiar.Click += (_, _) => LimpiarFiltros();
            btnExcel.Click += async (_, _) => await ExportarAsync(pdf: false);
            btnPdf.Click += async (_, _) => await ExportarAsync(pdf: true);
            btnSalir.Click += (_, _) => Salir();
            KeyDown += async (_, e) => { if (e.KeyCode == Keys.F5) { e.Handled = true; await GenerarAsync(); } };
            Resize += (_, _) => CentrarPanelCargando();

            FormClosing += (_, _) => { timerRegenerar.Stop(); ctsFormulario.Cancel(); };
            Disposed += (_, _) => { timerRegenerar.Dispose(); ctsFormulario.Dispose(); };
        }

        #region Configuración

        private void ConfigurarGrilla()
        {
            typeof(DataGridView).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(dgvReporte, true);
            dgvReporte.AutoGenerateColumns = false;
            dgvReporte.ReadOnly = true;
            dgvReporte.AllowUserToAddRows = false;
            dgvReporte.AllowUserToDeleteRows = false;
            dgvReporte.AllowUserToResizeRows = false;
            dgvReporte.RowHeadersVisible = false;
            dgvReporte.MultiSelect = false;
            dgvReporte.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReporte.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvReporte.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
        }

        private void ConfigurarGraficos()
        {
            for (int i = 0; i < graficos.Length; i++)
                graficos[i] = new FormsPlot { Dock = DockStyle.Fill, Margin = new Padding(6) };

            // El primero ocupa todo el ancho de la fila superior (evolución/ranking), los otros dos la inferior.
            tlpGraficos.Controls.Add(graficos[0], 0, 0);
            tlpGraficos.SetColumnSpan(graficos[0], 2);
            tlpGraficos.Controls.Add(graficos[1], 0, 1);
            tlpGraficos.Controls.Add(graficos[2], 1, 1);
        }

        private void ConfigurarParametros()
        {
            cbTipoReporte.Items.AddRange(Reportes);
            cbRango.Items.AddRange(new object[]
            {
                new OpcionRango(Rango.Hoy, "Hoy"),
                new OpcionRango(Rango.EstaSemana, "Esta semana"),
                new OpcionRango(Rango.EsteMes, "Este mes"),
                new OpcionRango(Rango.MesAnterior, "Mes anterior"),
                new OpcionRango(Rango.Ultimos30Dias, "Últimos 30 días"),
                new OpcionRango(Rango.AnioActual, "Año actual"),
                new OpcionRango(Rango.Personalizado, "Personalizado"),
            });
            cbAgrupacion.Items.AddRange(new object[] { "Diaria", "Semanal", "Mensual" });
            cbTop.Items.AddRange(new object[] { "Top 10", "Top 20", "Top 50" });
            cbCriterio.Items.AddRange(new object[] { "Unidades vendidas", "Recaudación" });

            cbTipoReporte.SelectedIndex = 0;
            cbRango.SelectedIndex = 2;   // Este mes
            cbAgrupacion.SelectedIndex = 0;
            cbTop.SelectedIndex = 0;
            cbCriterio.SelectedIndex = 0;
            AplicarRango(Rango.EsteMes);

            cbTipoReporte.SelectedIndexChanged += (_, _) => { ActualizarFiltrosVisibles(); ProgramarRegeneracion(); };
            cbRango.SelectedIndexChanged += (_, _) =>
            {
                if (cbRango.SelectedItem is OpcionRango r && r.Valor != Rango.Personalizado) AplicarRango(r.Valor);
                ProgramarRegeneracion();
            };
            // Tocar una fecha a mano pasa el rango a "Personalizado".
            EventHandler fechaManual = (_, _) =>
            {
                if (!fijandoRango) SeleccionarRango(Rango.Personalizado);
                ProgramarRegeneracion();
            };
            dtpDesde.ValueChanged += fechaManual;
            dtpHasta.ValueChanged += fechaManual;

            foreach (var combo in new[] { cbAgrupacion, cbMedioPago, cbCategoria, cbProveedor, cbTop, cbCriterio })
                combo.SelectedIndexChanged += (_, _) => ProgramarRegeneracion();
            chkExcluirConsumidorFinal.CheckedChanged += (_, _) => ProgramarRegeneracion();

            timerRegenerar.Tick += async (_, _) => { timerRegenerar.Stop(); await GenerarAsync(); };
            ActualizarFiltrosVisibles();
        }

        private async void FrmGestionarReportes_Load(object sender, EventArgs e)
        {
            btnGenerar.Visible = PermisoService.Instancia.TienePermiso("GenerarReportes");
            btnExcel.Visible = btnPdf.Visible = PermisoService.Instancia.TienePermiso("ExportarReportes");
            btnExcel.Enabled = btnPdf.Enabled = false;
            CentrarPanelCargando();

            try
            {
                var medios = ControladoraMetodosPago.Instancia.ObtenerMetodosPagoAsync(soloActivos: false, ctsFormulario.Token);
                var categorias = LibroService.Instancia.ObtenerGenerosAsync(ctsFormulario.Token);
                var proveedores = OrdenReposicionService.Instancia.ObtenerProveedoresAsync(incluirInactivos: true, ctsFormulario.Token);
                await Task.WhenAll(medios, categorias, proveedores);

                cbMedioPago.DataSource = new[] { Todos }.Concat(medios.Result.Select(m => new OpcionDTO(m.MP_ID, m.MP_Nombre))).ToList();
                cbCategoria.DataSource = new[] { Todos }.Concat(categorias.Result).ToList();
                cbProveedor.DataSource = new[] { Todos }.Concat(proveedores.Result.Select(p => new OpcionDTO(p.ProveedorId, p.ToString()))).ToList();
            }
            catch (Exception cancelada) when (ManejadorErrores.EsCancelacion(cancelada, ctsFormulario.Token)) { return; }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudieron cargar los filtros.");
            }

            inicializando = false;
            if (btnGenerar.Visible) await GenerarAsync();
        }

        #endregion

        #region Parámetros

        private DefinicionReporte Actual => (DefinicionReporte)cbTipoReporte.SelectedItem!;

        /// <summary>Muestra sólo los filtros que usa el reporte elegido.</summary>
        private void ActualizarFiltrosVisibles()
        {
            var d = Actual;
            lblRango.Visible = cbRango.Visible = lblDesde.Visible = dtpDesde.Visible = lblHasta.Visible = dtpHasta.Visible = d.Fechas;
            lblAgrupacion.Visible = cbAgrupacion.Visible = d.Agrupacion;
            pnlFiltroMedio.Visible = d.MedioPago;
            pnlFiltroCategoria.Visible = d.Categoria;
            pnlFiltroProveedor.Visible = d.Proveedor;
            pnlFiltroTop.Visible = d.Top;
            pnlFiltroCriterio.Visible = d.Criterio;
            pnlFiltroConsumidor.Visible = d.ConsumidorFinal;
        }

        private void AplicarRango(Rango rango)
        {
            DateTime hoy = DateTime.Today;
            (DateTime desde, DateTime hasta) = rango switch
            {
                Rango.Hoy => (hoy, hoy),
                Rango.EstaSemana => (hoy.AddDays(-(((int)hoy.DayOfWeek + 6) % 7)), hoy),
                Rango.EsteMes => (new DateTime(hoy.Year, hoy.Month, 1), hoy),
                Rango.MesAnterior => (new DateTime(hoy.Year, hoy.Month, 1).AddMonths(-1), new DateTime(hoy.Year, hoy.Month, 1).AddDays(-1)),
                Rango.Ultimos30Dias => (hoy.AddDays(-29), hoy),
                Rango.AnioActual => (new DateTime(hoy.Year, 1, 1), hoy),
                _ => (dtpDesde.Value.Date, dtpHasta.Value.Date),
            };

            fijandoRango = true;
            dtpDesde.Value = desde;
            dtpHasta.Value = hasta;
            fijandoRango = false;

            // Agrupación sugerida según el largo del período (el usuario la puede cambiar).
            int dias = (hasta - desde).Days;
            cbAgrupacion.SelectedIndex = dias > 120 ? 2 : dias > 31 ? 1 : 0;
        }

        private void SeleccionarRango(Rango rango)
        {
            fijandoRango = true;
            cbRango.SelectedItem = cbRango.Items.Cast<OpcionRango>().First(r => r.Valor == rango);
            fijandoRango = false;
        }

        private void ProgramarRegeneracion()
        {
            if (inicializando || fijandoRango || !btnGenerar.Visible) return;
            timerRegenerar.Stop();
            timerRegenerar.Start();
        }

        private ParametrosReporte ArmarParametros()
        {
            var d = Actual;
            int? Id(ComboBox cb) => cb.SelectedItem is OpcionDTO { Id: > 0 } o ? o.Id : null;

            var filtros = new List<string>();
            if (d.MedioPago && Id(cbMedioPago) != null) filtros.Add($"Medio: {cbMedioPago.Text}");
            if (d.Categoria && Id(cbCategoria) != null) filtros.Add($"Categoría: {cbCategoria.Text}");
            if (d.Proveedor && Id(cbProveedor) != null) filtros.Add($"Proveedor: {cbProveedor.Text}");

            return new ParametrosReporte
            {
                Tipo = d.Tipo,
                Desde = dtpDesde.Value.Date,
                Hasta = dtpHasta.Value.Date,
                Agrupacion = (Agrupacion)Math.Max(0, cbAgrupacion.SelectedIndex),
                MetodoPagoId = d.MedioPago ? Id(cbMedioPago) : null,
                GeneroId = d.Categoria ? Id(cbCategoria) : null,
                ProveedorId = d.Proveedor ? Id(cbProveedor) : null,
                TopN = cbTop.SelectedIndex switch { 1 => 20, 2 => 50, _ => 10 },
                Criterio = cbCriterio.SelectedIndex == 1 ? CriterioRanking.Recaudacion : CriterioRanking.Unidades,
                ExcluirConsumidorFinal = chkExcluirConsumidorFinal.Checked,
                DescripcionFiltros = filtros.Count > 0 ? string.Join(" · ", filtros) : null,
            };
        }

        private void LimpiarFiltros()
        {
            inicializando = true;
            foreach (var combo in new[] { cbMedioPago, cbCategoria, cbProveedor })
                if (combo.Items.Count > 0) combo.SelectedIndex = 0;
            cbTop.SelectedIndex = 0;
            cbCriterio.SelectedIndex = 0;
            chkExcluirConsumidorFinal.Checked = true;
            SeleccionarRango(Rango.EsteMes);
            AplicarRango(Rango.EsteMes);
            inicializando = false;
            ProgramarRegeneracion();
        }

        #endregion

        #region Generación (asíncrona y cancelable)

        private async Task GenerarAsync()
        {
            if (inicializando || !btnGenerar.Visible) return;
            timerRegenerar.Stop();

            if (dtpHasta.Value.Date < dtpDesde.Value.Date)
            {
                lblDescripcion.Text = "La fecha \"hasta\" no puede ser anterior a \"desde\".";
                lblDescripcion.ForeColor = Color.Firebrick;
                return;
            }

            // Si había un reporte en curso (el usuario cambió un filtro), se cancela: sólo se muestra el último pedido.
            ctsReporte?.Cancel();
            ctsReporte = CancellationTokenSource.CreateLinkedTokenSource(ctsFormulario.Token);
            var token = ctsReporte.Token;
            var parametros = ArmarParametros();

            MostrarCargando(true, "Generando reporte...");
            try
            {
                var resultado = await servicio.GenerarAsync(parametros, token);
                if (token.IsCancellationRequested) return;
                Mostrar(resultado);
            }
            catch (Exception cancelada) when (ManejadorErrores.EsCancelacion(cancelada, token)) { }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo generar el reporte.");
            }
            finally
            {
                if (!token.IsCancellationRequested) MostrarCargando(false);
            }
        }

        private void Mostrar(ResultadoReporte r)
        {
            ultimo = r;
            lblDescripcion.ForeColor = SystemColors.ControlText;
            lblDescripcion.Text = $"{r.Titulo}   ·   {r.Descripcion}   ·   {r.Filas.Count} fila(s)   ·   Generado {r.Generado:HH:mm:ss}";
            MostrarKpis(r.Kpis);
            MostrarGrilla(r);
            MostrarGraficos(r.Graficos);
            btnExcel.Enabled = btnPdf.Enabled = true;
        }

        /// <summary>Tarjetas de KPI: hasta 4; las que el reporte no usa se ocultan y el resto se reparte el ancho.</summary>
        private void MostrarKpis(List<KpiDTO> kpis)
        {
            var tarjetas = Tarjetas;
            tlpKpis.SuspendLayout();
            for (int i = 0; i < tarjetas.Length; i++)
            {
                var (tarjeta, titulo, valor, detalle) = tarjetas[i];
                bool visible = i < kpis.Count;
                tarjeta.Visible = visible;
                tlpKpis.ColumnStyles[i].Width = visible ? 100f / Math.Max(1, Math.Min(4, kpis.Count)) : 0;
                if (!visible) continue;

                tarjeta.BackColor = ColoresKpi[i];
                foreach (var l in new[] { titulo, valor, detalle }) { l.ForeColor = Color.White; l.BackColor = Color.Transparent; }
                titulo.Text = kpis[i].Titulo.ToUpper();
                valor.Text = ExportadorReportes.FormatearValor(kpis[i].Valor, kpis[i].Formato);
                detalle.Text = kpis[i].Detalle ?? string.Empty;
            }
            tlpKpis.ResumeLayout();
        }

        /// <summary>Columnas explícitas según el reporte, con formato estricto y ordenamiento por clic en el encabezado.</summary>
        private void MostrarGrilla(ResultadoReporte r)
        {
            dgvReporte.SuspendLayout();
            dgvReporte.DataSource = null;
            dgvReporte.Columns.Clear();

            foreach (var c in r.Columnas)
            {
                var columna = new DataGridViewTextBoxColumn
                {
                    Name = c.Propiedad,
                    DataPropertyName = c.Propiedad,
                    HeaderText = c.Encabezado,
                    FillWeight = c.Peso,
                    MinimumWidth = 40,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                };
                columna.DefaultCellStyle.Format = c.Formato switch
                {
                    FormatoValor.Moneda => "C2",
                    FormatoValor.Entero => "N0",
                    FormatoValor.Decimal => "N1",
                    FormatoValor.Porcentaje => "P1",
                    FormatoValor.Fecha => "dd/MM/yyyy",
                    _ => string.Empty,
                };
                if (c.Formato is FormatoValor.Moneda or FormatoValor.Entero or FormatoValor.Decimal or FormatoValor.Porcentaje)
                    columna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvReporte.Columns.Add(columna);
            }

            dgvReporte.DataSource = ListaOrdenable.Crear(r.TipoFila, r.Filas);
            dgvReporte.ResumeLayout();
        }

        private void MostrarGraficos(List<GraficoReporte> definiciones)
        {
            for (int i = 0; i < graficos.Length; i++)
            {
                bool visible = i < definiciones.Count;
                graficos[i].Visible = visible;
                if (visible)
                    GraficosReporte.Dibujar(graficos[i].Plot, definiciones[i]);
                graficos[i].Refresh();
            }

            // Con un solo gráfico, que ocupe toda la pestaña; con dos, uno arriba y otro abajo a todo el ancho.
            tlpGraficos.RowStyles[1].Height = definiciones.Count > 1 ? 50 : 0;
            tlpGraficos.SetColumnSpan(graficos[1], definiciones.Count == 2 ? 2 : 1);
        }

        private void MostrarCargando(bool visible, string texto = "")
        {
            ocupado = visible;
            lblCargando.Text = texto;
            panelCargando.Visible = visible;
            if (visible) { CentrarPanelCargando(); panelCargando.BringToFront(); }
            panelParametros.Enabled = !visible;
            UseWaitCursor = visible;
        }

        private void CentrarPanelCargando()
        {
            panelCargando.Location = new Point(
                Math.Max(0, (ClientSize.Width - panelCargando.Width) / 2),
                Math.Max(0, tabResultados.Top + (tabResultados.Height - panelCargando.Height) / 2));
        }

        #endregion

        #region Exportación

        private async Task ExportarAsync(bool pdf)
        {
            if (ultimo is not ResultadoReporte r || ocupado) return;

            using var dialogo = new SaveFileDialog
            {
                Filter = pdf ? "Documento PDF|*.pdf" : "Libro de Excel|*.xlsx",
                FileName = $"{Archivo(r.Titulo)}_{DateTime.Now:yyyyMMdd_HHmm}.{(pdf ? "pdf" : "xlsx")}",
            };
            if (dialogo.ShowDialog(this) != DialogResult.OK) return;

            MostrarCargando(true, pdf ? "Generando PDF..." : "Generando Excel...");
            try
            {
                // Imágenes de los gráficos y escritura del archivo fuera del hilo de UI.
                string ruta = dialogo.FileName;
                await Task.Run(() =>
                {
                    var imagenes = r.Graficos.Select(g => GraficosReporte.Imagen(g)).ToList();
                    if (pdf) ExportadorReportes.ExportarPdf(r, ruta, imagenes);
                    else ExportadorReportes.ExportarExcel(r, ruta, imagenes);
                });

                if (MessageBox.Show(this, $"Reporte exportado:\n{ruta}\n\n¿Abrirlo ahora?", "Exportación completa",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(ruta) { UseShellExecute = true });
            }
            catch (IOException ex)
            {
                MessageBox.Show(this, $"No se pudo escribir el archivo. ¿Está abierto en otro programa?\n\n{ex.Message}", "Exportar",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo exportar el reporte.");
            }
            finally
            {
                MostrarCargando(false);
            }

            static string Archivo(string titulo) =>
                new string(titulo.Select(c => char.IsLetterOrDigit(c) ? c : '_').ToArray()).Trim('_');
        }

        #endregion

        private void Salir()
        {
            if (TopLevelControl is FrmMenu principal)
                principal.MostrarInicio();   // cierra y libera esta sección
            else
                Close();
        }
    }
}
