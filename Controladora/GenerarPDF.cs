using iTextSharp.text;
using iTextSharp.text.pdf;
using Modelo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora
{
    public class GenerarPDF
    {
        static Font _fontTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
        static Font _fontHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
        static Font _fontCelda = FontFactory.GetFont(FontFactory.HELVETICA, 9);

        public static void ExportarReporteCompleto(
            string rutaArchivo,
            DateTime fechaDesde,
            DateTime fechaHasta,

            byte[] imgIngresos,
            List<Reportes.ReporteIngresos> datosIngresos,

            byte[] imgLibrosMasVendidos,
            List<Reportes.ReporteLibroMasVendido> datosLibros,

            byte[] imgVentasPorGenero,
            List<Reportes.ReporteVentasPorGenero> datosGeneros
        )
        {
            Document doc = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20);

            try
            {
                PdfWriter.GetInstance(doc, new FileStream(rutaArchivo, FileMode.Create));

                doc.Open();

                doc.Add(new Paragraph("REPORTE DE LIBRERÍA", _fontTitulo)
                {
                    Alignment = Element.ALIGN_CENTER
                });

                doc.Add(new Paragraph(
                    $"Período: {fechaDesde:dd/MM/yyyy} al {fechaHasta:dd/MM/yyyy}",
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11))
                {
                    Alignment = Element.ALIGN_CENTER
                });

                doc.Add(new Paragraph(
                    $"Fecha de emisión: {DateTime.Now:dd/MM/yyyy HH:mm}\n\n"));

                AgregarSeccion(
                    doc,
                    "1. Ingresos por Mes",
                    imgIngresos,
                    CrearTablaIngresos(datosIngresos));

                doc.Add(new Paragraph("\n"));

                AgregarSeccion(
                    doc,
                    "2. Libros Más Vendidos",
                    imgLibrosMasVendidos,
                    CrearTablaLibrosMasVendidos(datosLibros));

                doc.Add(new Paragraph("\n"));

                AgregarSeccion(
                    doc,
                    "3. Ventas por Género",
                    imgVentasPorGenero,
                    CrearTablaVentasPorGenero(datosGeneros));

                doc.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al generar PDF: " + ex.Message);
            }
        }

        // Método Mágico: Crea una tabla invisible de 2 columnas para alinear Gráfico y Datos
        private static void AgregarSeccion(Document doc, string titulo, byte[] imagenBytes, PdfPTable tablaDatos)
        {
            // Subtítulo
            doc.Add(new Paragraph(titulo, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));
            doc.Add(new Paragraph("\n"));

            // Tabla Contenedora (Layout)
            PdfPTable layout = new PdfPTable(2);
            layout.WidthPercentage = 100;
            layout.SetWidths(new float[] { 60f, 40f }); // 60% Gráfico, 40% Tabla

            // COLUMNA 1: IMAGEN
            if (imagenBytes != null && imagenBytes.Length > 0)
            {
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imagenBytes);
                img.ScaleToFit(450f, 280f);
                PdfPCell celdaImg = new PdfPCell(img);
                celdaImg.Border = Rectangle.NO_BORDER;
                celdaImg.HorizontalAlignment = Element.ALIGN_CENTER;
                celdaImg.VerticalAlignment = Element.ALIGN_MIDDLE;
                layout.AddCell(celdaImg);
            }
            else
            {
                layout.AddCell(new PdfPCell(new Phrase("Sin Gráfico")) { Border = Rectangle.NO_BORDER });
            }

            // COLUMNA 2: TABLA DE DATOS
            PdfPCell celdaTabla = new PdfPCell(tablaDatos);
            celdaTabla.Border = Rectangle.NO_BORDER;
            celdaTabla.PaddingLeft = 10f;
            layout.AddCell(celdaTabla);

            doc.Add(layout);
        }

        // HELPERS PARA CREAR TABLAS

       

        private static PdfPTable CrearTablaIngresos(List<Reportes.ReporteIngresos> datos)
        {
            
                PdfPTable t = new PdfPTable(2);

                t.WidthPercentage = 100;

                AgregarHeader(t, "Mes");
                AgregarHeader(t, "Ingresos");

                foreach (var d in datos)
                {
                    AgregarCelda(t, d.MesNumero.ToString());
                    AgregarCelda(t, $"${d.TotalIngresos:N0}");
                }

                return t;
            
        }

        private static PdfPTable CrearTablaLibrosMasVendidos(List<Reportes.ReporteLibroMasVendido> datos)
        {
            PdfPTable t = new PdfPTable(2);

            t.WidthPercentage = 100;

            AgregarHeader(t, "Libro");
            AgregarHeader(t, "Cantidad");

            foreach (var d in datos)
            {
                AgregarCelda(t, d.Titulo);
                AgregarCelda(t, d.CantidadVendida.ToString());
            }

            return t;
        }

        private static PdfPTable CrearTablaVentasPorGenero(List<Reportes.ReporteVentasPorGenero> datos)
        {
            PdfPTable t = new PdfPTable(2);

            t.WidthPercentage = 100;

            AgregarHeader(t, "Género");
            AgregarHeader(t, "Ventas");

            foreach (var d in datos)
            {
                AgregarCelda(t, d.Genero);
                AgregarCelda(t, d.CantidadVendida.ToString());
            }

            return t;
        }


        private static void AgregarHeader(PdfPTable t, string texto)
        {
            PdfPCell c = new PdfPCell(new Phrase(texto, _fontHeader));
            c.BackgroundColor = new BaseColor(50, 50, 50);
            c.HorizontalAlignment = Element.ALIGN_CENTER;
            c.Padding = 5;
            t.AddCell(c);
        }

        private static void AgregarCelda(PdfPTable t, string texto)
        {
            PdfPCell c = new PdfPCell(new Phrase(texto, _fontCelda));
            c.HorizontalAlignment = Element.ALIGN_CENTER;
            c.Padding = 5;
            t.AddCell(c);
        }
    }
}
