using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

namespace Vista.Comun
{
    /// <summary>
    /// Imprime una grilla tal como se ve (columnas visibles, en su orden y formato), en hoja apaisada,
    /// con encabezado repetido en cada página y numeración. Muestra vista previa antes de imprimir.
    /// </summary>
    internal static class ImpresorGrilla
    {
        public static void Previsualizar(IWin32Window owner, DataGridView dgv, string titulo, string? subtitulo = null)
        {
            var columnas = dgv.Columns.Cast<DataGridViewColumn>()
                .Where(c => c.Visible && c is DataGridViewTextBoxColumn)
                .OrderBy(c => c.DisplayIndex)
                .ToList();
            var filas = dgv.Rows.Cast<DataGridViewRow>()
                .Select(r => columnas.Select(c => r.Cells[c.Index].FormattedValue?.ToString() ?? string.Empty).ToArray())
                .ToList();
            var alineadoDerecha = columnas.Select(c => c.DefaultCellStyle.Alignment is DataGridViewContentAlignment.MiddleRight).ToArray();
            float totalPeso = columnas.Sum(c => c.Width);

            using var fuenteTitulo = new Font("Segoe UI", 14F, FontStyle.Bold);
            using var fuenteEncabezado = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            using var fuenteCelda = new Font("Segoe UI", 8.5F);
            using var lapizLinea = new Pen(Color.Gainsboro);

            int filaActual = 0, pagina = 0;
            using var documento = new PrintDocument { DocumentName = titulo };
            documento.DefaultPageSettings.Landscape = true;
            documento.DefaultPageSettings.Margins = new Margins(40, 40, 40, 40);
            documento.BeginPrint += (_, _) => { filaActual = 0; pagina = 0; };
            documento.PrintPage += (_, e) =>
            {
                var g = e.Graphics!;
                var area = e.MarginBounds;
                float y = area.Top;
                pagina++;

                g.DrawString(titulo, fuenteTitulo, Brushes.Black, area.Left, y);
                y += fuenteTitulo.GetHeight(g) + 2;
                g.DrawString($"{subtitulo}   ·   Emitido: {DateTime.Now:dd/MM/yyyy HH:mm}   ·   Página {pagina}",
                    fuenteCelda, Brushes.DimGray, area.Left, y);
                y += fuenteCelda.GetHeight(g) + 10;

                // Anchos proporcionales al ancho actual de cada columna en pantalla.
                float[] anchos = columnas.Select(c => area.Width * c.Width / totalPeso).ToArray();
                float alto = fuenteCelda.GetHeight(g) + 6;

                DibujarFila(g, columnas.Select(c => c.HeaderText).ToArray(), fuenteEncabezado, area.Left, y, anchos, alto, alineadoDerecha, fondo: Brushes.Gainsboro);
                y += alto;

                while (filaActual < filas.Count && y + alto <= area.Bottom)
                {
                    DibujarFila(g, filas[filaActual], fuenteCelda, area.Left, y, anchos, alto, alineadoDerecha, fondo: null);
                    g.DrawLine(lapizLinea, area.Left, y + alto, area.Right, y + alto);
                    y += alto;
                    filaActual++;
                }
                e.HasMorePages = filaActual < filas.Count;
            };

            using var vista = new PrintPreviewDialog { Document = documento, Width = 1100, Height = 750, ShowIcon = false };
            vista.ShowDialog(owner);
        }

        private static void DibujarFila(Graphics g, string[] textos, Font fuente, float x, float y, float[] anchos, float alto,
            bool[] derecha, Brush? fondo)
        {
            if (fondo != null) g.FillRectangle(fondo, x, y, anchos.Sum(), alto);
            for (int i = 0; i < textos.Length; i++)
            {
                var celda = new RectangleF(x + 3, y + 3, anchos[i] - 6, alto - 3);
                using var formato = new StringFormat
                {
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap,
                    Alignment = derecha[i] ? StringAlignment.Far : StringAlignment.Near,
                };
                g.DrawString(textos[i], fuente, Brushes.Black, celda, formato);
                x += anchos[i];
            }
        }
    }
}
