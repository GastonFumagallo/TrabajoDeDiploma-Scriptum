using iTextSharp.text;
using iTextSharp.text.pdf;
using Modelo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora
{
    /// <summary>Exporta el reporte de existencias (lo filtrado en pantalla) a CSV (Excel) o PDF.</summary>
    public static class ExportadorInventario
    {
        private static readonly string[] Encabezados =
            { "Código", "Título", "Autor", "Categoría", "Proveedor habitual", "Stock", "Mínimo", "Pto. reposición", "Óptimo", "En pedido", "Costo", "Valor", "Estado" };

        /// <summary>CSV con ';' y UTF-8 con BOM para que Excel en español lo abra correctamente con doble clic.</summary>
        public static async Task ExportarCsvAsync(string ruta, IReadOnlyList<ProductoInventarioDTO> productos, CancellationToken ct = default)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(";", Encabezados));
            foreach (var p in productos)
            {
                sb.AppendLine(string.Join(";",
                    Csv(p.Codigo), Csv(p.Titulo), Csv(p.Autor), Csv(p.Categoria), Csv(p.ProveedorHabitual),
                    p.Stock, p.StockMinimo, p.PuntoReposicion, p.StockOptimo, p.EnPedido,
                    p.PrecioCosto.ToString("0.00"), p.ValorCosto.ToString("0.00"), p.EstadoTexto));
            }
            sb.AppendLine();
            sb.AppendLine($"Productos;{productos.Count}");
            sb.AppendLine($"Valor total a costo;{productos.Sum(p => p.ValorCosto):0.00}");

            await File.WriteAllTextAsync(ruta, sb.ToString(), new UTF8Encoding(true), ct);

            static string Csv(string? valor) =>
                string.IsNullOrEmpty(valor) ? "" : "\"" + valor.Replace("\"", "\"\"") + "\"";
        }

        public static void ExportarPdf(string ruta, IReadOnlyList<ProductoInventarioDTO> productos, string descripcionFiltro)
        {
            var fuenteTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
            var fuenteSub = FontFactory.GetFont(FontFactory.HELVETICA, 9);
            var fuenteHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8, BaseColor.WHITE);
            var fuenteCelda = FontFactory.GetFont(FontFactory.HELVETICA, 8);

            using var stream = new FileStream(ruta, FileMode.Create, FileAccess.Write);
            var doc = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20);
            PdfWriter.GetInstance(doc, stream);
            doc.Open();
            try
            {
                doc.Add(new Paragraph("REPORTE DE EXISTENCIAS - LIBRERÍA SCRIPTUM", fuenteTitulo) { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph($"Emitido: {DateTime.Now:dd/MM/yyyy HH:mm}   ·   Filtro: {descripcionFiltro}", fuenteSub) { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph(
                    $"Productos: {productos.Count}   ·   Sin stock: {productos.Count(p => p.Estado == EstadoStock.Agotado)}   ·   " +
                    $"Críticos: {productos.Count(p => p.Estado == EstadoStock.Critico)}   ·   A reponer: {productos.Count(p => p.Estado == EstadoStock.AReponer)}   ·   " +
                    $"Valor a costo: ${productos.Sum(p => p.ValorCosto):N2}\n\n", fuenteSub) { Alignment = Element.ALIGN_CENTER });

                var tabla = new PdfPTable(Encabezados.Length) { WidthPercentage = 100, HeaderRows = 1 };
                tabla.SetWidths(new float[] { 9, 22, 13, 9, 13, 5, 5, 6, 5, 6, 7, 8, 7 });

                foreach (var h in Encabezados)
                    tabla.AddCell(new PdfPCell(new Phrase(h, fuenteHeader)) { BackgroundColor = new BaseColor(52, 73, 94), Padding = 4 });

                foreach (var p in productos)
                {
                    BaseColor fondo = p.Estado switch
                    {
                        EstadoStock.Agotado => new BaseColor(242, 140, 140),
                        EstadoStock.Critico => new BaseColor(247, 200, 140),
                        EstadoStock.AReponer => new BaseColor(240, 230, 140),
                        _ => BaseColor.WHITE,
                    };
                    void Celda(string? texto, bool numero = false) =>
                        tabla.AddCell(new PdfPCell(new Phrase(texto ?? "", fuenteCelda))
                        {
                            BackgroundColor = fondo,
                            Padding = 3,
                            HorizontalAlignment = numero ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT,
                        });

                    Celda(p.Codigo); Celda(p.Titulo); Celda(p.Autor); Celda(p.Categoria); Celda(p.ProveedorHabitual);
                    Celda(p.Stock.ToString(), true); Celda(p.StockMinimo.ToString(), true); Celda(p.PuntoReposicion.ToString(), true);
                    Celda(p.StockOptimo.ToString(), true); Celda(p.EnPedido.ToString(), true);
                    Celda(p.PrecioCosto.ToString("N2"), true); Celda(p.ValorCosto.ToString("N2"), true); Celda(p.EstadoTexto);
                }

                doc.Add(tabla);
            }
            finally
            {
                doc.Close();
            }
        }
    }
}
