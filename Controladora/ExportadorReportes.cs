using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Modelo;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Controladora
{
    /// <summary>
    /// Exporta cualquier <see cref="ResultadoReporte"/> a Excel (.xlsx real, con formatos numéricos, tabla filtrable
    /// y hoja de gráficos) o a PDF imprimible (KPIs, gráficos y detalle). Trabaja sobre los datos ya procesados,
    /// no sobre la grilla: lo exportado es exactamente lo que calculó el servicio, con tipos numéricos reales.
    /// </summary>
    public static class ExportadorReportes
    {
        private static readonly ConcurrentDictionary<(Type, string), PropertyInfo?> Propiedades = new();
        private static CultureInfo Cultura => CultureInfo.CurrentCulture;   // la que fija CulturaRegional al iniciar

        #region Excel

        /// <param name="graficos">Imágenes PNG de los gráficos (opcional): se insertan en una hoja aparte.</param>
        public static void ExportarExcel(ResultadoReporte r, string ruta, IReadOnlyList<byte[]>? graficos = null)
        {
            using var libro = new XLWorkbook();

            // Hoja 1: resumen (título, filtros y KPIs).
            var resumen = libro.Worksheets.Add("Resumen");
            resumen.Cell(1, 1).Value = r.Titulo;
            resumen.Cell(1, 1).Style.Font.SetBold().Font.SetFontSize(16);
            resumen.Cell(2, 1).Value = r.Descripcion;
            resumen.Cell(3, 1).Value = $"Generado: {r.Generado:dd/MM/yyyy HH:mm}";
            resumen.Range(2, 1, 3, 1).Style.Font.SetFontColor(XLColor.Gray);

            resumen.Cell(5, 1).Value = "Indicador";
            resumen.Cell(5, 2).Value = "Valor";
            resumen.Cell(5, 3).Value = "Detalle";
            resumen.Range(5, 1, 5, 3).Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.FromHtml("#34495E")).Font.SetFontColor(XLColor.White);
            int fila = 6;
            foreach (var kpi in r.Kpis)
            {
                resumen.Cell(fila, 1).Value = kpi.Titulo;
                resumen.Cell(fila, 2).Value = kpi.Valor;
                resumen.Cell(fila, 2).Style.NumberFormat.Format = FormatoExcel(kpi.Formato);
                resumen.Cell(fila, 3).Value = kpi.Detalle ?? string.Empty;
                fila++;
            }
            resumen.Columns().AdjustToContents();

            // Hoja 2: detalle como tabla de Excel (encabezados con filtro, filas alternadas, panel inmovilizado).
            var detalle = libro.Worksheets.Add("Detalle");
            for (int c = 0; c < r.Columnas.Count; c++)
                detalle.Cell(1, c + 1).Value = r.Columnas[c].Encabezado;

            int f = 2;
            foreach (var item in r.Filas)
            {
                for (int c = 0; c < r.Columnas.Count; c++)
                {
                    var celda = detalle.Cell(f, c + 1);
                    object? valor = ObtenerValor(item!, r.Columnas[c].Propiedad);
                    celda.Value = valor switch
                    {
                        null => Blank.Value,
                        decimal d => d,
                        int i => i,
                        double d => d,
                        DateTime dt => dt,
                        _ => valor.ToString(),
                    };
                    if (r.Columnas[c].Formato != FormatoValor.Texto)
                        celda.Style.NumberFormat.Format = FormatoExcel(r.Columnas[c].Formato);
                }
                f++;
            }

            if (r.Filas.Count > 0)
            {
                var tabla = detalle.Range(1, 1, f - 1, r.Columnas.Count).CreateTable("Detalle");
                tabla.Theme = XLTableTheme.TableStyleMedium2;
                tabla.ShowTotalsRow = false;
            }
            detalle.SheetView.FreezeRows(1);
            detalle.Columns().AdjustToContents(1, Math.Min(f, 500));

            // Hoja 3: gráficos como imágenes (lo que se vio en pantalla).
            if (graficos is { Count: > 0 })
            {
                var hoja = libro.Worksheets.Add("Gráficos");
                int filaImagen = 1;
                foreach (var png in graficos)
                {
                    using var stream = new MemoryStream(png);
                    hoja.AddPicture(stream).MoveTo(hoja.Cell(filaImagen, 1));
                    filaImagen += 22;
                }
            }

            libro.SaveAs(ruta);
        }

        private static string FormatoExcel(FormatoValor formato) => formato switch
        {
            FormatoValor.Moneda => "\"$\" #,##0.00",
            FormatoValor.Entero => "#,##0",
            FormatoValor.Decimal => "#,##0.0",
            FormatoValor.Porcentaje => "0.0%",
            FormatoValor.Fecha => "dd/mm/yyyy",
            _ => "@",
        };

        #endregion

        #region PDF

        /// <param name="graficos">Imágenes PNG de los gráficos (opcional), se ubican de a dos por fila.</param>
        public static void ExportarPdf(ResultadoReporte r, string ruta, IReadOnlyList<byte[]>? graficos = null)
        {
            var fuenteTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 15);
            var fuenteSub = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.DARK_GRAY);
            var fuenteKpiTitulo = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.WHITE);
            var fuenteKpiValor = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 13, BaseColor.WHITE);
            var fuenteHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8, BaseColor.WHITE);
            var fuenteCelda = FontFactory.GetFont(FontFactory.HELVETICA, 8);
            var azul = new BaseColor(52, 73, 94);

            using var stream = new FileStream(ruta, FileMode.Create, FileAccess.Write);
            var doc = new Document(PageSize.A4.Rotate(), 24, 24, 24, 24);
            var writer = PdfWriter.GetInstance(doc, stream);
            writer.PageEvent = new PiePagina($"Librería SCRIPTUM · {r.Titulo}");
            doc.Open();
            try
            {
                doc.Add(new Paragraph(r.Titulo, fuenteTitulo));
                doc.Add(new Paragraph($"{r.Descripcion}   ·   Generado: {r.Generado:dd/MM/yyyy HH:mm}\n\n", fuenteSub));

                // Tarjetas de KPIs.
                if (r.Kpis.Count > 0)
                {
                    var kpis = new PdfPTable(r.Kpis.Count) { WidthPercentage = 100, SpacingAfter = 12 };
                    foreach (var k in r.Kpis)
                    {
                        var celda = new PdfPCell { BackgroundColor = azul, Padding = 8, BorderColor = BaseColor.WHITE, BorderWidth = 3 };
                        celda.AddElement(new Paragraph(k.Titulo.ToUpper(), fuenteKpiTitulo));
                        celda.AddElement(new Paragraph(FormatearValor(k.Valor, k.Formato), fuenteKpiValor));
                        if (k.Detalle != null) celda.AddElement(new Paragraph(k.Detalle, fuenteKpiTitulo));
                        kpis.AddCell(celda);
                    }
                    doc.Add(kpis);
                }

                // Gráficos, de a dos por fila.
                if (graficos is { Count: > 0 })
                {
                    var tablaGraficos = new PdfPTable(Math.Min(2, graficos.Count)) { WidthPercentage = 100, SpacingAfter = 12 };
                    foreach (var png in graficos)
                    {
                        var imagen = iTextSharp.text.Image.GetInstance(png);
                        tablaGraficos.AddCell(new PdfPCell(imagen, fit: true) { Border = Rectangle.NO_BORDER, Padding = 4 });
                    }
                    if (graficos.Count > 2 && graficos.Count % 2 == 1)
                        tablaGraficos.AddCell(new PdfPCell { Border = Rectangle.NO_BORDER });
                    doc.Add(tablaGraficos);
                }

                // Detalle.
                var tabla = new PdfPTable(r.Columnas.Count) { WidthPercentage = 100, HeaderRows = 1 };
                tabla.SetWidths(r.Columnas.Select(c => c.Peso).ToArray());
                foreach (var c in r.Columnas)
                    tabla.AddCell(new PdfPCell(new Phrase(c.Encabezado, fuenteHeader)) { BackgroundColor = azul, Padding = 4 });

                bool alternar = false;
                foreach (var item in r.Filas)
                {
                    var fondo = alternar ? new BaseColor(242, 245, 248) : BaseColor.WHITE;
                    foreach (var c in r.Columnas)
                    {
                        object? valor = ObtenerValor(item!, c.Propiedad);
                        tabla.AddCell(new PdfPCell(new Phrase(FormatearValor(valor, c.Formato), fuenteCelda))
                        {
                            BackgroundColor = fondo,
                            Padding = 3,
                            HorizontalAlignment = c.Formato is FormatoValor.Texto or FormatoValor.Fecha ? Element.ALIGN_LEFT : Element.ALIGN_RIGHT,
                        });
                    }
                    alternar = !alternar;
                }
                if (r.Filas.Count == 0)
                    tabla.AddCell(new PdfPCell(new Phrase("Sin datos para los filtros elegidos.", fuenteCelda)) { Colspan = r.Columnas.Count, Padding = 6 });
                doc.Add(tabla);
            }
            finally
            {
                doc.Close();
            }
        }

        /// <summary>Numera las páginas del PDF.</summary>
        private sealed class PiePagina : PdfPageEventHelper
        {
            private readonly string texto;
            public PiePagina(string texto) => this.texto = texto;

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                var fuente = FontFactory.GetFont(FontFactory.HELVETICA, 7, BaseColor.GRAY);
                ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_RIGHT,
                    new Phrase($"{texto} · Página {writer.PageNumber}", fuente),
                    document.PageSize.Width - document.RightMargin, document.BottomMargin - 12, 0);
            }
        }

        #endregion

        #region Valores

        /// <summary>Lee una propiedad del DTO por nombre (reflexión cacheada).</summary>
        public static object? ObtenerValor(object fila, string propiedad)
        {
            var info = Propiedades.GetOrAdd((fila.GetType(), propiedad), k => k.Item1.GetProperty(k.Item2));
            return info?.GetValue(fila);
        }

        /// <summary>Formato de texto común a pantalla, PDF y tarjetas.</summary>
        public static string FormatearValor(object? valor, FormatoValor formato) => valor switch
        {
            null => string.Empty,
            DateTime dt => dt.ToString("dd/MM/yyyy", Cultura),
            decimal or double or int or long => formato switch
            {
                FormatoValor.Moneda => Convert.ToDecimal(valor).ToString("C2", Cultura),
                FormatoValor.Entero => Convert.ToDecimal(valor).ToString("N0", Cultura),
                FormatoValor.Decimal => Convert.ToDecimal(valor).ToString("N1", Cultura),
                FormatoValor.Porcentaje => Convert.ToDecimal(valor).ToString("P1", Cultura),
                _ => Convert.ToString(valor, Cultura) ?? string.Empty,
            },
            _ => valor.ToString() ?? string.Empty,
        };

        #endregion
    }
}
