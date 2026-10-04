using Modelo;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Vista.Comun
{
    /// <summary>
    /// Dibuja un <see cref="GraficoReporte"/> con ScottPlot (líneas, barras, barras horizontales o torta/dona).
    /// Se usa tanto para la pantalla como para generar las imágenes que van a Excel y PDF, así se ven igual.
    /// </summary>
    internal static class GraficosReporte
    {
        private static readonly IPalette Paleta = new ScottPlot.Palettes.Category10();
        private static CultureInfo Cultura => CultureInfo.CurrentCulture;   // la que fija CulturaRegional al iniciar

        public static void Dibujar(Plot plot, GraficoReporte g)
        {
            plot.Clear();
            plot.Legend.ManualItems.Clear();
            plot.Title(g.Titulo, size: 14);

            if (g.Etiquetas.Length == 0 || g.Series.All(s => s.Valores.All(v => v == 0)))
            {
                plot.Add.Annotation("Sin datos para el período", Alignment.MiddleCenter);
                plot.Axes.Frameless();
                plot.HideGrid();
                return;
            }

            switch (g.Tipo)
            {
                case TipoGrafico.Lineas: Lineas(plot, g); break;
                case TipoGrafico.Barras: Barras(plot, g, horizontal: false); break;
                case TipoGrafico.BarrasHorizontales: Barras(plot, g, horizontal: true); break;
                case TipoGrafico.Torta: Torta(plot, g); break;
            }
        }

        /// <summary>Imagen PNG del gráfico (para exportar).</summary>
        public static byte[] Imagen(GraficoReporte g, int ancho = 900, int alto = 480)
        {
            var plot = new Plot();
            Dibujar(plot, g);
            return plot.GetImage(ancho, alto).GetImageBytes();
        }

        private static string Formatear(double valor, FormatoValor formato) => formato switch
        {
            FormatoValor.Moneda => valor.ToString("C0", Cultura),
            FormatoValor.Porcentaje => valor.ToString("P0", Cultura),
            _ => valor.ToString("N0", Cultura),
        };

        private static void Lineas(Plot plot, GraficoReporte g)
        {
            double[] xs = Enumerable.Range(0, g.Etiquetas.Length).Select(i => (double)i).ToArray();
            for (int s = 0; s < g.Series.Count; s++)
            {
                var serie = g.Series[s];
                var linea = plot.Add.Scatter(xs, serie.Valores);
                linea.Color = Paleta.GetColor(s);
                linea.LineWidth = 2;
                linea.MarkerSize = g.Etiquetas.Length > 40 ? 0 : 5;
                linea.LegendText = serie.Nombre;
            }
            EtiquetasEjeX(plot, g.Etiquetas);
            FormatoEje(plot.Axes.Left, g.FormatoValores);
            if (g.Series.Count > 1) plot.ShowLegend(Alignment.UpperLeft);
            plot.Axes.Margins(bottom: 0);
        }

        private static void Barras(Plot plot, GraficoReporte g, bool horizontal)
        {
            int series = g.Series.Count;
            double ancho = 0.8 / series;
            var barras = new List<Bar>();
            int n = g.Etiquetas.Length;

            for (int s = 0; s < series; s++)
            {
                var color = Paleta.GetColor(s);
                for (int i = 0; i < n; i++)
                {
                    // En horizontal se invierte el orden: el N° 1 del ranking queda arriba.
                    double posicion = (horizontal ? n - 1 - i : i) - 0.4 + ancho * (s + 0.5);
                    barras.Add(new Bar
                    {
                        Position = posicion,
                        Value = g.Series[s].Valores[i],
                        Size = ancho * 0.9,
                        FillColor = color,
                        Label = series == 1 && n <= 20 ? Formatear(g.Series[s].Valores[i], g.FormatoValores) : string.Empty,
                    });
                }
                if (series > 1)
                    plot.Legend.ManualItems.Add(new LegendItem { LabelText = g.Series[s].Nombre, FillColor = color });
            }

            var barPlot = plot.Add.Bars(barras);
            barPlot.Horizontal = horizontal;
            barPlot.ValueLabelStyle.FontSize = 10;

            var posiciones = Enumerable.Range(0, n).Select(i => (double)i).ToArray();
            var etiquetas = horizontal ? g.Etiquetas.Reverse().ToArray() : g.Etiquetas;
            if (horizontal)
            {
                plot.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericManual(posiciones, etiquetas);
                FormatoEje(plot.Axes.Bottom, g.FormatoValores);
                plot.Axes.Margins(left: 0);
            }
            else
            {
                EtiquetasEjeX(plot, etiquetas);
                FormatoEje(plot.Axes.Left, g.FormatoValores);
                plot.Axes.Margins(bottom: 0);
            }
            if (series > 1) plot.ShowLegend(Alignment.UpperRight);
        }

        private static void Torta(Plot plot, GraficoReporte g)
        {
            var valores = g.Series[0].Valores;
            double total = valores.Sum();
            var porciones = new List<PieSlice>();
            for (int i = 0; i < g.Etiquetas.Length; i++)
            {
                if (valores[i] <= 0) continue;
                porciones.Add(new PieSlice
                {
                    Value = valores[i],
                    FillColor = Paleta.GetColor(i),
                    Label = (valores[i] / total).ToString("P0", Cultura),
                    LegendText = $"{g.Etiquetas[i]}: {Formatear(valores[i], g.FormatoValores)}",
                });
            }

            var pie = plot.Add.Pie(porciones);
            pie.DonutFraction = 0.5;
            pie.SliceLabelDistance = 0.75;
            plot.ShowLegend(Alignment.MiddleRight);
            plot.Axes.Frameless();
            plot.HideGrid();
        }

        /// <summary>Etiquetas del eje X; si son muchas se muestra una cada k para que no se encimen.</summary>
        private static void EtiquetasEjeX(Plot plot, string[] etiquetas)
        {
            int paso = Math.Max(1, (int)Math.Ceiling(etiquetas.Length / 15.0));
            var indices = Enumerable.Range(0, etiquetas.Length).Where(i => i % paso == 0).ToArray();
            plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
                indices.Select(i => (double)i).ToArray(),
                indices.Select(i => etiquetas[i]).ToArray());
            plot.Axes.Bottom.TickLabelStyle.Rotation = etiquetas.Length > 8 ? -35 : 0;
            plot.Axes.Bottom.TickLabelStyle.Alignment = etiquetas.Length > 8 ? Alignment.MiddleRight : Alignment.UpperCenter;
        }

        private static void FormatoEje(IAxis eje, FormatoValor formato)
        {
            eje.TickGenerator = new ScottPlot.TickGenerators.NumericAutomatic
            {
                LabelFormatter = v => Formatear(v, formato),
            };
        }
    }
}
