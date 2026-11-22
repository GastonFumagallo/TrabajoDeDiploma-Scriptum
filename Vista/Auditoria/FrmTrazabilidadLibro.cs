using Controladora;
using Modelo;
using Vista.Comun;

namespace Vista.Auditoria
{
    /// <summary>
    /// Trazabilidad completa de un libro: su estado original, su estado actual y todas sus transformaciones
    /// (cambios de datos y movimientos de stock), con usuario, fecha y valores anteriores y nuevos.
    /// </summary>
    public sealed class FrmTrazabilidadLibro : Form
    {
        private readonly AuditoriaService servicio = AuditoriaService.Instancia;
        private readonly int? libroInicial;

        private readonly TextBox txtBuscar = new() { Width = 300, PlaceholderText = "Título, autor o ISBN" };
        private readonly Button btnBuscar = new() { Text = "Buscar", AutoSize = true, MinimumSize = new Size(0, 30) };
        private readonly ComboBox cbLibro = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 480 };
        private readonly Label lblTitulo = new() { Dock = DockStyle.Top, Height = 34, Font = new Font("Segoe UI", 12F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(8, 0, 0, 0) };
        private readonly Label lblAclaracion = new() { Dock = DockStyle.Top, Height = 24, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(8, 0, 0, 0) };
        private readonly DataGridView dgvEstado = new() { Dock = DockStyle.Fill };
        private readonly DataGridView dgvHistorial = new() { Dock = DockStyle.Fill };
        private readonly Button btnImprimir = new() { Text = "Imprimir historial", AutoSize = true, MinimumSize = new Size(0, 32) };
        private readonly Button btnCerrar = new() { Text = "Cerrar", AutoSize = true, Height = 32, DialogResult = DialogResult.Cancel };
        private readonly CancellationTokenSource cts = new();
        private Font? fuenteModificado;

        public FrmTrazabilidadLibro(int? libroId)
        {
            libroInicial = libroId;
            Text = "Trazabilidad del libro";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1150, 780);
            MinimumSize = new Size(900, 600);
            CancelButton = btnCerrar;
            Armar();

            btnBuscar.Click += async (_, _) => await BuscarLibrosAsync(null);
            txtBuscar.KeyDown += async (_, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; await BuscarLibrosAsync(null); } };
            cbLibro.SelectedIndexChanged += async (_, _) => { if (cbLibro.SelectedItem is LibroAuditableDTO l) await CargarAsync(l.Id); };
            btnImprimir.Click += (_, _) => ImpresorGrilla.Previsualizar(this, dgvHistorial, "Trazabilidad — " + lblTitulo.Text, lblAclaracion.Text);
            Load += async (_, _) => await BuscarLibrosAsync(libroInicial);
            FormClosing += (_, _) => cts.Cancel();
            Disposed += (_, _) => { cts.Dispose(); fuenteModificado?.Dispose(); };
        }

        private void Armar()
        {
            var busqueda = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(8, 8, 8, 6), WrapContents = false };
            busqueda.Controls.AddRange(new Control[]
            {
                new Label { Text = "Buscar", AutoSize = true, Margin = new Padding(0, 6, 4, 0) }, txtBuscar, btnBuscar,
                new Label { Text = "Libro", AutoSize = true, Margin = new Padding(16, 6, 4, 0) }, cbLibro,
            });

            GrillaHelper.ConfigurarListado(dgvEstado);
            GrillaHelper.Columna(dgvEstado, nameof(EstadoCampoDTO.Campo), "Campo", 90);
            GrillaHelper.Columna(dgvEstado, nameof(EstadoCampoDTO.Original), "Estado original", 170);
            GrillaHelper.Columna(dgvEstado, nameof(EstadoCampoDTO.Actual), "Estado actual", 170);
            GrillaHelper.Columna(dgvEstado, nameof(EstadoCampoDTO.Cambios), "Cambios", 50, derecha: true);
            dgvEstado.CellFormatting += (_, e) =>
            {
                if (e.RowIndex >= 0 && e.CellStyle != null && dgvEstado.Rows[e.RowIndex].DataBoundItem is EstadoCampoDTO { Modificado: true })
                    e.CellStyle.Font = fuenteModificado ??= new Font(dgvEstado.Font, FontStyle.Bold);
            };

            GrillaHelper.ConfigurarListado(dgvHistorial);
            GrillaHelper.Columna(dgvHistorial, nameof(CambioLibroDTO.Fecha), "Fecha", 100, "dd/MM/yyyy HH:mm:ss");
            GrillaHelper.Columna(dgvHistorial, nameof(CambioLibroDTO.Usuario), "Usuario", 70);
            GrillaHelper.Columna(dgvHistorial, nameof(CambioLibroDTO.Operacion), "Operación", 110);
            GrillaHelper.Columna(dgvHistorial, nameof(CambioLibroDTO.Campo), "Campo", 80);
            GrillaHelper.Columna(dgvHistorial, nameof(CambioLibroDTO.ValorAnterior), "Valor anterior", 140);
            GrillaHelper.Columna(dgvHistorial, nameof(CambioLibroDTO.ValorNuevo), "Valor nuevo", 140);
            GrillaHelper.Columna(dgvHistorial, nameof(CambioLibroDTO.Referencia), "Referencia", 80);

            var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 300 };
            split.Panel1.Controls.Add(dgvEstado);
            split.Panel1.Controls.Add(new Label { Text = "Estado original y actual", Dock = DockStyle.Top, Height = 24, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) });
            split.Panel2.Controls.Add(dgvHistorial);
            split.Panel2.Controls.Add(new Label { Text = "Transformaciones (de la más reciente a la más antigua)", Dock = DockStyle.Top, Height = 24, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) });

            var botones = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(8, 6, 8, 6), FlowDirection = FlowDirection.RightToLeft };
            botones.Controls.AddRange(new Control[] { btnCerrar, btnImprimir });

            Controls.Add(split);
            Controls.Add(lblAclaracion);
            Controls.Add(lblTitulo);
            Controls.Add(busqueda);
            Controls.Add(botones);
        }

        private async Task BuscarLibrosAsync(int? seleccionar)
        {
            try
            {
                var libros = await servicio.BuscarLibrosAsync(txtBuscar.Text, cts.Token);
                if (seleccionar is int id && libros.All(l => l.Id != id))
                {
                    var traza = await servicio.ObtenerTrazabilidadAsync(id, cts.Token);
                    if (traza != null) libros.Insert(0, new LibroAuditableDTO { Id = id, Texto = traza.Titulo });
                }
                cbLibro.DataSource = libros;
                if (seleccionar is int sel)
                    cbLibro.SelectedItem = libros.FirstOrDefault(l => l.Id == sel);
                if (libros.Count == 0)
                {
                    lblTitulo.Text = "No se encontraron libros.";
                    dgvEstado.DataSource = null; dgvHistorial.DataSource = null;
                }
            }
            catch (Exception c) when (ManejadorErrores.EsCancelacion(c, cts.Token)) { }
            catch (Exception ex) { ManejadorErrores.Mostrar(this, ex, "No se pudieron buscar los libros."); }
        }

        private async Task CargarAsync(int libroId)
        {
            UseWaitCursor = true;
            try
            {
                var t = await servicio.ObtenerTrazabilidadAsync(libroId, cts.Token);
                if (t == null) { lblTitulo.Text = "El libro ya no existe."; return; }
                lblTitulo.Text = t.Titulo + (t.ISBN.Length > 0 ? " — ISBN " + t.ISBN : "");
                lblAclaracion.Text = (t.AltaRegistrada
                        ? "Estado original: valores registrados en el alta del libro."
                        : "Estado original reconstruido: el libro se cargó antes de activar la auditoría; se toma el valor anterior al primer cambio de cada campo.")
                    + $" · {t.Historial.Count} transformaciones.";
                dgvEstado.DataSource = t.Estado;
                dgvHistorial.DataSource = t.Historial;
            }
            catch (Exception c) when (ManejadorErrores.EsCancelacion(c, cts.Token)) { }
            catch (Exception ex) { ManejadorErrores.Mostrar(this, ex, "No se pudo cargar la trazabilidad del libro."); }
            finally { UseWaitCursor = false; }
        }
    }
}
