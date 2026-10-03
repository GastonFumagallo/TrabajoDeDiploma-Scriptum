using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Vista.Comun
{
    /// <summary>
    /// Configuración estándar de grillas de ABM: columnas explícitas (nada de autogeneradas desordenadas),
    /// doble buffer, selección de fila completa y sólo lectura.
    /// </summary>
    internal static class GrillaHelper
    {
        public static void ConfigurarListado(DataGridView dgv, bool multiSeleccion = false)
        {
            typeof(DataGridView).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(dgv, true);
            dgv.AutoGenerateColumns = false;
            dgv.Columns.Clear();
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.RowHeadersVisible = false;
            dgv.MultiSelect = multiSeleccion;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        /// <summary>Agrega una columna enlazada a una propiedad del DTO.</summary>
        /// <param name="peso">Ancho relativo (FillWeight).</param>
        /// <param name="formato">Formato .NET, ej. "N2" o "dd/MM/yyyy".</param>
        public static DataGridViewTextBoxColumn Columna(DataGridView dgv, string propiedad, string encabezado,
            float peso = 100, string? formato = null, bool derecha = false, bool visible = true)
        {
            var col = new DataGridViewTextBoxColumn
            {
                Name = propiedad,
                DataPropertyName = propiedad,
                HeaderText = encabezado,
                FillWeight = peso,
                MinimumWidth = 40,
                Visible = visible,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };
            if (formato != null) col.DefaultCellStyle.Format = formato;
            if (derecha) col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns.Add(col);
            return col;
        }

        /// <summary>Re-selecciona una fila por id tras recargar la grilla (para no perder el lugar del usuario).</summary>
        public static void Seleccionar<T>(DataGridView dgv, Func<T, bool> criterio)
        {
            foreach (DataGridViewRow fila in dgv.Rows)
            {
                if (fila.DataBoundItem is T item && criterio(item))
                {
                    var celda = fila.Cells.Cast<DataGridViewCell>().FirstOrDefault(c => c.Visible);
                    if (celda != null) dgv.CurrentCell = celda;
                    return;
                }
            }
        }
    }

    /// <summary>Exporta cualquier grilla a CSV (Excel): columnas visibles, en el orden y formato que se ven.</summary>
    internal static class ExportadorGrilla
    {
        public static async Task<int> ExportarCsvAsync(DataGridView dgv, string ruta, CancellationToken ct = default)
        {
            var columnas = dgv.Columns.Cast<DataGridViewColumn>()
                .Where(c => c.Visible && c is DataGridViewTextBoxColumn)
                .OrderBy(c => c.DisplayIndex)
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine(string.Join(";", columnas.Select(c => Csv(c.HeaderText))));
            foreach (DataGridViewRow fila in dgv.Rows)
                sb.AppendLine(string.Join(";", columnas.Select(c => Csv(fila.Cells[c.Index].FormattedValue?.ToString()))));

            // UTF-8 con BOM y ';' para que Excel en español lo abra bien con doble clic.
            await File.WriteAllTextAsync(ruta, sb.ToString(), new UTF8Encoding(true), ct);
            return dgv.Rows.Count;

            static string Csv(string? valor) =>
                string.IsNullOrEmpty(valor) ? "" : "\"" + valor.Replace("\"", "\"\"") + "\"";
        }
    }
}
