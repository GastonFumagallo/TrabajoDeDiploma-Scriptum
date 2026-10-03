using Modelo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Vista.Inventario
{
    /// <summary>
    /// Ajuste manual de stock: ingreso, egreso o conteo físico, con motivo obligatorio y vista previa del
    /// stock resultante. No persiste nada: devuelve la solicitud y la persiste <c>InventarioService</c>.
    /// </summary>
    internal sealed class DlgAjusteStock : Form
    {
        private static readonly Dictionary<TipoAjusteStock, string[]> Motivos = new()
        {
            [TipoAjusteStock.Ingreso] = new[] { "Devolución de cliente", "Mercadería encontrada", "Ingreso sin orden de compra", "Otro" },
            [TipoAjusteStock.Egreso] = new[] { "Rotura", "Deterioro / Vencimiento", "Pérdida / Robo", "Uso interno / Muestra", "Otro" },
            [TipoAjusteStock.ConteoFisico] = new[] { "Conteo físico", "Inventario periódico", "Otro" },
        };

        private readonly ProductoInventarioDTO producto;
        private readonly RadioButton rbIngreso = new() { Text = "Ingreso", AutoSize = true, Checked = true };
        private readonly RadioButton rbEgreso = new() { Text = "Egreso", AutoSize = true };
        private readonly RadioButton rbConteo = new() { Text = "Conteo físico (fijar stock)", AutoSize = true };
        private readonly Label lblCantidad = new() { AutoSize = true };
        private readonly NumericUpDown numCantidad = new() { Maximum = 100_000, Width = 120, TextAlign = HorizontalAlignment.Right };
        private readonly ComboBox cbMotivo = new() { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly TextBox txtObservacion = new() { Multiline = true, Width = 420, Height = 60, MaxLength = 250 };
        private readonly Label lblResultado = new() { AutoSize = true, Font = new Font("Segoe UI", 11F, FontStyle.Bold) };
        private readonly Button btnAceptar = new() { Text = "Registrar ajuste", Width = 150, Height = 36, DialogResult = DialogResult.OK };
        private readonly Button btnCancelar = new() { Text = "Cancelar", Width = 110, Height = 36, DialogResult = DialogResult.Cancel };

        public AjusteStockSolicitud? Resultado { get; private set; }

        public DlgAjusteStock(ProductoInventarioDTO producto)
        {
            this.producto = producto;
            Text = "Ajuste de stock";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = MaximizeBox = ShowInTaskbar = false;
            ClientSize = new Size(460, 400);
            AcceptButton = btnAceptar;
            CancelButton = btnCancelar;

            var lblLibro = new Label
            {
                Text = $"{producto.Titulo}{(string.IsNullOrEmpty(producto.Codigo) ? "" : $"  ({producto.Codigo})")}",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                AutoEllipsis = true,
                Location = new Point(16, 12),
                Size = new Size(428, 26),
            };
            var lblStock = new Label { Text = $"Stock actual: {producto.Stock}", AutoSize = true, Location = new Point(16, 42) };

            rbIngreso.Location = new Point(16, 74);
            rbEgreso.Location = new Point(110, 74);
            rbConteo.Location = new Point(200, 74);
            lblCantidad.Location = new Point(16, 112);
            numCantidad.Location = new Point(150, 108);
            var lblMotivo = new Label { Text = "Motivo:", AutoSize = true, Location = new Point(16, 152) };
            cbMotivo.Location = new Point(150, 148);
            var lblObs = new Label { Text = "Observación:", AutoSize = true, Location = new Point(16, 188) };
            txtObservacion.Location = new Point(16, 212);
            lblResultado.Location = new Point(16, 290);
            btnAceptar.Location = new Point(176, 344);
            btnCancelar.Location = new Point(334, 344);

            Controls.AddRange(new Control[]
            {
                lblLibro, lblStock, rbIngreso, rbEgreso, rbConteo, lblCantidad, numCantidad,
                lblMotivo, cbMotivo, lblObs, txtObservacion, lblResultado, btnAceptar, btnCancelar,
            });

            foreach (var rb in new[] { rbIngreso, rbEgreso, rbConteo })
                rb.CheckedChanged += (_, _) => { if (rb.Checked) CambiarTipo(); };
            numCantidad.ValueChanged += (_, _) => Actualizar();
            cbMotivo.SelectedIndexChanged += (_, _) => Actualizar();
            txtObservacion.TextChanged += (_, _) => Actualizar();
            btnAceptar.Click += (_, _) => Resultado = ArmarSolicitud();

            CambiarTipo();
        }

        private TipoAjusteStock Tipo =>
            rbEgreso.Checked ? TipoAjusteStock.Egreso : rbConteo.Checked ? TipoAjusteStock.ConteoFisico : TipoAjusteStock.Ingreso;

        private void CambiarTipo()
        {
            cbMotivo.Items.Clear();
            cbMotivo.Items.AddRange(Motivos[Tipo]);
            cbMotivo.SelectedIndex = 0;

            if (Tipo == TipoAjusteStock.ConteoFisico)
            {
                lblCantidad.Text = "Stock contado:";
                numCantidad.Minimum = 0;
                numCantidad.Value = Math.Max(0, producto.Stock);
            }
            else
            {
                lblCantidad.Text = Tipo == TipoAjusteStock.Ingreso ? "Unidades a ingresar:" : "Unidades a egresar:";
                numCantidad.Minimum = 1;
                numCantidad.Value = 1;
            }
            Actualizar();
            numCantidad.Focus();
            numCantidad.Select(0, numCantidad.Text.Length);
        }

        private int StockResultante => Tipo switch
        {
            TipoAjusteStock.Ingreso => producto.Stock + (int)numCantidad.Value,
            TipoAjusteStock.Egreso => producto.Stock - (int)numCantidad.Value,
            _ => (int)numCantidad.Value,
        };

        /// <summary>Vista previa y validación en vivo: el botón sólo se habilita si el ajuste es válido.</summary>
        private void Actualizar()
        {
            int resultante = StockResultante;
            bool requiereObservacion = cbMotivo.Text == "Otro";
            bool sinCambios = Tipo == TipoAjusteStock.ConteoFisico && resultante == producto.Stock;

            lblResultado.Text = resultante < 0
                ? $"Stock resultante: {resultante} (no alcanza el stock)"
                : $"Stock resultante: {producto.Stock} → {resultante}" + (sinCambios ? "  (sin diferencias)" : "");
            lblResultado.ForeColor = resultante < 0 ? Color.Firebrick : SystemColors.ControlText;

            btnAceptar.Enabled = resultante >= 0
                                 && cbMotivo.SelectedIndex >= 0
                                 && (!requiereObservacion || txtObservacion.Text.Trim().Length >= 5);
        }

        private AjusteStockSolicitud ArmarSolicitud() => new()
        {
            LibroId = producto.LibroId,
            Tipo = Tipo,
            Cantidad = (int)numCantidad.Value,
            Motivo = cbMotivo.Text,
            Observacion = string.IsNullOrWhiteSpace(txtObservacion.Text) ? null : txtObservacion.Text.Trim(),
        };
    }

    /// <summary>Edición de mínimo, punto de reposición y stock óptimo de un libro, con validación en vivo.</summary>
    internal sealed class DlgParametrosStock : Form
    {
        private readonly NumericUpDown numMinimo = Numerico();
        private readonly NumericUpDown numPunto = Numerico();
        private readonly NumericUpDown numOptimo = Numerico();
        private readonly Label lblError = new() { AutoSize = true, ForeColor = Color.Firebrick, MaximumSize = new Size(380, 0) };
        private readonly Button btnAceptar = new() { Text = "Guardar", Width = 110, Height = 34, DialogResult = DialogResult.OK };

        public ParametrosStockSolicitud? Resultado { get; private set; }

        public DlgParametrosStock(ProductoInventarioDTO producto)
        {
            Text = "Parámetros de reposición";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = MaximizeBox = ShowInTaskbar = false;
            ClientSize = new Size(410, 260);
            AcceptButton = btnAceptar;

            var btnCancelar = new Button { Text = "Cancelar", Width = 110, Height = 34, DialogResult = DialogResult.Cancel, Location = new Point(284, 212) };
            CancelButton = btnCancelar;

            numMinimo.Value = producto.StockMinimo;
            numPunto.Value = producto.PuntoReposicion;
            numOptimo.Value = producto.StockOptimo;

            Controls.Add(new Label { Text = producto.Titulo, Font = new Font("Segoe UI", 10F, FontStyle.Bold), AutoEllipsis = true, Location = new Point(16, 12), Size = new Size(380, 24) });
            Controls.Add(new Label { Text = $"Stock actual: {producto.Stock}", AutoSize = true, Location = new Point(16, 40) });
            AgregarFila("Stock mínimo (crítico):", numMinimo, 72);
            AgregarFila("Punto de reposición:", numPunto, 108);
            AgregarFila("Stock óptimo (objetivo):", numOptimo, 144);
            lblError.Location = new Point(16, 180);
            btnAceptar.Location = new Point(168, 212);
            Controls.AddRange(new Control[] { lblError, btnAceptar, btnCancelar });

            foreach (var n in new[] { numMinimo, numPunto, numOptimo })
                n.ValueChanged += (_, _) => Validar();
            btnAceptar.Click += (_, _) => Resultado = new ParametrosStockSolicitud
            {
                LibroId = producto.LibroId,
                StockMinimo = (int)numMinimo.Value,
                PuntoReposicion = (int)numPunto.Value,
                StockOptimo = (int)numOptimo.Value,
            };
            Validar();
        }

        private static NumericUpDown Numerico() => new() { Maximum = 100_000, Width = 100, TextAlign = HorizontalAlignment.Right };

        private void AgregarFila(string texto, NumericUpDown control, int y)
        {
            Controls.Add(new Label { Text = texto, AutoSize = true, Location = new Point(16, y + 3) });
            control.Location = new Point(230, y);
            Controls.Add(control);
        }

        private void Validar()
        {
            string? error = numPunto.Value < numMinimo.Value ? "El punto de reposición no puede ser menor que el mínimo."
                : numOptimo.Value < numPunto.Value ? "El stock óptimo no puede ser menor que el punto de reposición."
                : numOptimo.Value <= 0 ? "El stock óptimo debe ser mayor a 0."
                : null;
            lblError.Text = error ?? string.Empty;
            btnAceptar.Enabled = error == null;
        }
    }

    /// <summary>Grilla de sólo lectura en un diálogo (historial de movimientos, proveedores de un libro...).</summary>
    internal sealed class DlgGrilla : Form
    {
        public DlgGrilla(string titulo, object datos, Action<DataGridView>? configurar = null, Size? tamaño = null)
        {
            Text = titulo;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = ShowInTaskbar = false;
            ClientSize = tamaño ?? new Size(980, 460);

            var grilla = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = datos,
            };
            var btnCerrar = new Button { Text = "Cerrar", Dock = DockStyle.Bottom, Height = 36, DialogResult = DialogResult.Cancel };
            CancelButton = btnCerrar;

            Controls.Add(grilla);
            Controls.Add(btnCerrar);
            grilla.DataBindingComplete += (_, _) => configurar?.Invoke(grilla);
        }
    }

    /// <summary>Pide un texto obligatorio (ej. motivo de cancelación). Devuelve null si se cancela.</summary>
    internal static class DlgTexto
    {
        public static string? Pedir(IWin32Window owner, string titulo, string consigna, int minimo = 5, int maximo = 250)
        {
            using var dlg = new Form
            {
                Text = titulo,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false,
                ClientSize = new Size(460, 210),
                ShowInTaskbar = false,
            };
            var lbl = new Label { Text = consigna, Location = new Point(12, 12), AutoSize = true };
            var txt = new TextBox { Location = new Point(12, 40), Size = new Size(436, 100), Multiline = true, MaxLength = maximo };
            var btnOk = new Button { Text = "Aceptar", Location = new Point(262, 160), Size = new Size(90, 34), DialogResult = DialogResult.OK, Enabled = false };
            var btnCancel = new Button { Text = "Cancelar", Location = new Point(358, 160), Size = new Size(90, 34), DialogResult = DialogResult.Cancel };
            txt.TextChanged += (_, _) => btnOk.Enabled = txt.Text.Trim().Length >= minimo;
            dlg.Controls.AddRange(new Control[] { lbl, txt, btnOk, btnCancel });
            dlg.AcceptButton = btnOk;
            dlg.CancelButton = btnCancel;

            return dlg.ShowDialog(owner) == DialogResult.OK ? txt.Text.Trim() : null;
        }
    }
}
