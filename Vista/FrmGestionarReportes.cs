using Controladora;
using Modelo;
using Modelo.Seguridad;
using ScottPlot;
using ScottPlot.WinForms;
using Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static Modelo.Reportes;
namespace Vista
{
    public partial class FrmGestionarReportes : Form
    {
        private FormsPlot plotIngresos;
        private FormsPlot plotLibrosMasVendidos;
        private FormsPlot plotVentasPorGenero;
        private List<ReporteIngresos> cacheIngresos;
        private List<ReporteLibroMasVendido> cacheLibrosMasVendidos;
        private List<ReporteVentasPorGenero> cacheVentasPorGenero;

        public FrmGestionarReportes()
        {
            InitializeComponent();
            InicializarGraficos();
        }
        private void InicializarGraficos()
        {
            plotIngresos = new FormsPlot() { Dock = DockStyle.Fill };
            panelIngresos.Controls.Add(plotIngresos);

            plotLibrosMasVendidos = new FormsPlot() { Dock = DockStyle.Fill };
            panelLibrosVendidos.Controls.Add(plotLibrosMasVendidos);

            plotVentasPorGenero = new FormsPlot() { Dock = DockStyle.Fill };
            panelGeneros.Controls.Add(plotVentasPorGenero);
        }

        private void FrmGestionarReportes_Load(object sender, EventArgs e)
        {
            dtpDesde.Value = DateTime.Today.AddMonths(-1);
            dtpHasta.Value = DateTime.Today;
            AplicarSeguridad();

        }
        private void AplicarSeguridad()
        {
            if (btnGenerar != null)
                btnGenerar.Visible = PermisoService.Instancia.TienePermiso("GenerarReportes");

            if (btnExportar != null)
                btnExportar.Visible = PermisoService.Instancia.TienePermiso("ExportarReportes");
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime desde = dtpDesde.Value.Date;
                DateTime hasta = dtpHasta.Value.Date.AddDays(1).AddSeconds(-1);
                generarReporteIngresos(desde, hasta);
                generarReporteLibrosMasVendidos(desde, hasta);
                generarReporteGeneros(desde, hasta);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar reportes: " + ex.Message);
            }
        }

        private void generarReporteGeneros(DateTime desde, DateTime hasta)
        {
            cacheVentasPorGenero = ControladoraReportes.Instancia.ObtenerVentasPorGenero(desde, hasta);
            dgvGeneros.DataSource = null;
            dgvGeneros.DataSource = cacheVentasPorGenero;
            DibujarGraficoVentasPorGenero();
        }
        private void generarReporteLibrosMasVendidos(DateTime desde, DateTime hasta)
        {
            cacheLibrosMasVendidos = ControladoraReportes.Instancia.ObtenerLibrosMasVendidos(desde, hasta);
            dgvLibrosVendidos.DataSource = null;
            dgvLibrosVendidos.DataSource = cacheLibrosMasVendidos;
            dgvLibrosVendidos.Columns["IngresoGenerado"].DefaultCellStyle.Format = "N2";
            DibujarGraficoLibrosMasVendidos();
        }
        private void generarReporteIngresos(DateTime desde, DateTime hasta)
        {
            cacheIngresos = ControladoraReportes.Instancia.ObtenerIngresos(desde, hasta);
            dgvIngresos.DataSource = null;
            dgvIngresos.DataSource = cacheIngresos;
            dgvIngresos.Columns["TotalIngresos"].DefaultCellStyle.Format = "N2";
            DibujarGraficoIngresos();
        }

        private void DibujarGraficoIngresos()
        {
            plotIngresos.Plot.Clear();

            double[] valores = cacheIngresos
                .Select(i => (double)i.TotalIngresos)
                .ToArray();

            string[] etiquetas = cacheIngresos
            .Select(x => $"{x.MesNumero:D2}/{x.Año}")
            .ToArray();

            var bars = plotIngresos.Plot.Add.Bars(valores);

            plotIngresos.Plot.Title("Ingresos por Mes");
            plotIngresos.Plot.Axes.Bottom.TickGenerator =
                new ScottPlot.TickGenerators.NumericManual(
                    Enumerable.Range(0, etiquetas.Length)
                              .Select(i => (double)i)
                              .ToArray(),
                    etiquetas);

            plotIngresos.Plot.Axes.AutoScale();

            plotIngresos.Refresh();
        }
        private void DibujarGraficoLibrosMasVendidos()
        {
            plotLibrosMasVendidos.Plot.Clear();

            double[] valores = cacheLibrosMasVendidos
                .Select(x => (double)x.CantidadVendida)
                .ToArray();

            string[] etiquetas = cacheLibrosMasVendidos
                .Select(x => x.Titulo.Length > 20
                    ? x.Titulo.Substring(0, 20) + "..."
                    : x.Titulo)
                .ToArray();

            plotLibrosMasVendidos.Plot.Add.Bars(valores);

            plotLibrosMasVendidos.Plot.Title("Top 10 Libros Más Vendidos");

            plotLibrosMasVendidos.Plot.Axes.Bottom.TickGenerator =
                new ScottPlot.TickGenerators.NumericManual(
                    Enumerable.Range(0, etiquetas.Length)
                              .Select(i => (double)i)
                              .ToArray(),
                    etiquetas);

            plotLibrosMasVendidos.Plot.Axes.AutoScale();

            plotLibrosMasVendidos.Refresh();
        }

        private void DibujarGraficoVentasPorGenero()
        {
            plotVentasPorGenero.Plot.Clear();

            List<PieSlice> porciones = new();

            ScottPlot.Color[] colores =
            {
                ScottPlot.Color.FromHex("#5C6B73"),
                ScottPlot.Color.FromHex("#7B8C95"),
                ScottPlot.Color.FromHex("#A7B4BC"),
                ScottPlot.Color.FromHex("#D9D2B0"),
                ScottPlot.Color.FromHex("#C6A969"),
                ScottPlot.Color.FromHex("#8A817C")
            };

            int i = 0;

            foreach (var item in cacheVentasPorGenero)
            {
                porciones.Add(new PieSlice
                {
                    Value = item.CantidadVendida,
                    Label = item.Genero,
                    FillColor = colores[i % colores.Length],
                    LegendText = $"{item.Genero}: {item.CantidadVendida} libros"
                });

                i++;
            }

            var pie = plotVentasPorGenero.Plot.Add.Pie(porciones);

            pie.SliceLabelDistance = 1.3;

            plotVentasPorGenero.Plot.Title("Ventas por Género");

            plotVentasPorGenero.Plot.HideGrid();

            plotVentasPorGenero.Plot.Axes.Bottom.TickGenerator =
                new ScottPlot.TickGenerators.EmptyTickGenerator();

            plotVentasPorGenero.Plot.Axes.Left.TickGenerator =
                new ScottPlot.TickGenerators.EmptyTickGenerator();

            plotVentasPorGenero.Plot.Axes.AutoScale();

            plotVentasPorGenero.Refresh();
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {

            SaveFileDialog save = new SaveFileDialog();
            save.FileName = $"Reporte_Libreria_{DateTime.Now:yyyyMMdd}.pdf";
            save.Filter = "PDF Files|*.pdf";

            if (save.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DateTime fechaDesde = dtpDesde.Value.Date;
                    DateTime fechaHasta = dtpHasta.Value.Date.AddDays(1).AddSeconds(-1);

                    // DATOS
                    var ingresosPorMes = ControladoraReportes.Instancia.ObtenerIngresos(fechaDesde, fechaHasta);
                    var librosMasVendidos = ControladoraReportes.Instancia.ObtenerLibrosMasVendidos(fechaDesde, fechaHasta);
                    var ventasPorGenero = ControladoraReportes.Instancia.ObtenerVentasPorGenero(fechaDesde, fechaHasta);

                    // IMÁGENES DE LOS GRÁFICOS
                    byte[] imgIngresos = GenerarImagenIngresos(ingresosPorMes);
                    byte[] imgLibrosMasVendidos = GenerarImagenLibrosMasVendidos(librosMasVendidos);
                    byte[] imgVentasPorGenero = GenerarImagenVentasPorGenero(ventasPorGenero);

                    // EXPORTAR PDF
                    GenerarPDF.ExportarReporteCompleto(
                        save.FileName,
                        fechaDesde,
                        fechaHasta,

                        imgIngresos,
                        ingresosPorMes,

                        imgLibrosMasVendidos,
                        librosMasVendidos,

                        imgVentasPorGenero,
                        ventasPorGenero
                    ); 

                    MessageBox.Show("Reporte exportado exitosamente.");

                    try
                    {
                        System.Diagnostics.Process.Start("explorer.exe", save.FileName);
                    }
                    catch { }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private byte[] GenerarImagenIngresos(List<Reportes.ReporteIngresos> datos)
        {
            ScottPlot.Plot plot = new();

            List<Bar> barras = new();
            double[] posiciones = new double[datos.Count];
            string[] etiquetas = new string[datos.Count];

            int i = 0;

            foreach (var item in datos)
            {
                barras.Add(new Bar
                {
                    Position = i,
                    Value = (double)item.TotalIngresos,
                    FillColor = ScottPlot.Color.FromHex("#5C6B73"),
                    Label = $"${item.TotalIngresos:N0}"
                });

                posiciones[i] = i;
                etiquetas[i] = item.MesNumero.ToString();
                i++;
            }

            plot.Add.Bars(barras);

            plot.Axes.Bottom.TickGenerator =
                new ScottPlot.TickGenerators.NumericManual(posiciones, etiquetas);

            plot.Title("Ingresos por Mes");

            plot.Grid.MajorLineColor = ScottPlot.Colors.Transparent;
            plot.Axes.AutoScale();

            return plot.GetImage(600, 400).GetImageBytes();
        }

        private byte[] GenerarImagenLibrosMasVendidos(List<Reportes.ReporteLibroMasVendido> datos)
        {
            ScottPlot.Plot plot = new();

            if (datos.Count == 0)
                return plot.GetImage(600, 400).GetImageBytes();

            var datosDibujo = datos
                .OrderBy(x => x.CantidadVendida)
                .ToList();

            List<Bar> barras = new();

            double[] posiciones = new double[datosDibujo.Count];
            string[] etiquetas = new string[datosDibujo.Count];

            int i = 0;

            foreach (var item in datosDibujo)
            {
                barras.Add(new Bar
                {
                    Position = i,
                    Value = item.CantidadVendida,
                    FillColor = ScottPlot.Color.FromHex("#4F6D8A"),
                    Label = item.CantidadVendida.ToString()
                });

                posiciones[i] = i;
                etiquetas[i] = item.Titulo;
                i++;
            }

            var barPlot = plot.Add.Bars(barras);

            barPlot.Horizontal = true;

            plot.Axes.Left.TickGenerator =
                new ScottPlot.TickGenerators.NumericManual(posiciones, etiquetas);

            plot.Grid.MajorLineColor = ScottPlot.Colors.Transparent;

            plot.Title("Libros Más Vendidos");

            plot.Axes.AutoScale();

            return plot.GetImage(600, 400).GetImageBytes();
        }

        private byte[] GenerarImagenVentasPorGenero(List<Reportes.ReporteVentasPorGenero> datos)
        {
            ScottPlot.Plot plot = new();

            List<PieSlice> porciones = new();

            ScottPlot.Color[] colores =
            {
                ScottPlot.Color.FromHex("#5C6B73"),
                ScottPlot.Color.FromHex("#7B8C95"),
                ScottPlot.Color.FromHex("#A7B4BC"),
                ScottPlot.Color.FromHex("#D9D2B0"),
                ScottPlot.Color.FromHex("#C6A969"),
                ScottPlot.Color.FromHex("#8A817C")
            };

            int i = 0;

            foreach (var item in datos)
            {
                porciones.Add(new PieSlice
                {
                    Value = item.CantidadVendida,
                    Label = item.Genero,
                    FillColor = colores[i % colores.Length],
                    LegendText = $"{item.Genero}: {item.CantidadVendida} libros"
                });

                i++;
            }

            var pie = plot.Add.Pie(porciones);

            pie.SliceLabelDistance = 1.3;

            plot.Title("Ventas por Género");

            plot.HideGrid();

            plot.Axes.Bottom.TickGenerator =
                new ScottPlot.TickGenerators.EmptyTickGenerator();

            plot.Axes.Left.TickGenerator =
                new ScottPlot.TickGenerators.EmptyTickGenerator();

            plot.Axes.AutoScale();

            return plot.GetImage(600, 400).GetImageBytes();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            FrmMenu principal = Application.OpenForms["FrmMenu"] as FrmMenu;

            if (principal != null)
            {
                principal.MostrarInicio();
            }

            this.Close();
        }

        

    }

}
