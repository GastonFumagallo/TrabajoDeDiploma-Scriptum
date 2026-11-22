using Controladora;
using Modelo;
using Servicios;
using Vista.Comun;

namespace Vista.Auditoria
{
    /// <summary>
    /// Reportes de auditoría (sección embebida en el menú): trazabilidad de libros, inicios y cierres de sesión y
    /// eventos de seguridad. Filtra por período y usuario; cada pestaña agrega su propio filtro.
    /// Sólo habla con <see cref="AuditoriaService"/>.
    ///
    /// Atajos: F5 = buscar · Enter en una fila de libros = ver su trazabilidad completa.
    /// </summary>
    public sealed class FrmAuditoria : Form
    {
        private const string Todos = "(Todos)";

        private readonly AuditoriaService servicio = AuditoriaService.Instancia;

        private readonly DateTimePicker dtpDesde = new() { Format = DateTimePickerFormat.Short, Width = 120 };
        private readonly DateTimePicker dtpHasta = new() { Format = DateTimePickerFormat.Short, Width = 120 };
        private readonly ComboBox cbUsuario = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 160 };
        private readonly ComboBox cbOperacion = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 190 };
        private readonly Label lblOperacion = new() { Text = "Operación", AutoSize = true, Margin = new Padding(12, 8, 4, 0) };
        private readonly TextBox txtLibro = new() { Width = 200, PlaceholderText = "Título o ISBN" };
        private readonly Label lblLibro = new() { Text = "Libro", AutoSize = true, Margin = new Padding(12, 8, 4, 0) };
        private readonly Button btnBuscar = new() { Text = "Buscar (F5)", AutoSize = true, MinimumSize = new Size(0, 32) };
        private readonly Button btnTrazabilidad = new() { Text = "Trazabilidad del libro…", AutoSize = true, MinimumSize = new Size(0, 32) };
        private readonly Button btnExportar = new() { Text = "Exportar a Excel", AutoSize = true, MinimumSize = new Size(0, 32) };
        private readonly Button btnImprimir = new() { Text = "Imprimir", AutoSize = true, MinimumSize = new Size(0, 32) };
        private readonly Button btnVolver = new() { Text = "Volver", AutoSize = true, MinimumSize = new Size(0, 32) };

        private readonly TabControl tabs = new() { Dock = DockStyle.Fill };
        private readonly DataGridView dgvLibros = new() { Dock = DockStyle.Fill };
        private readonly DataGridView dgvSesiones = new() { Dock = DockStyle.Fill };
        private readonly DataGridView dgvEventos = new() { Dock = DockStyle.Fill };
        private readonly Label lblResumen = new() { Dock = DockStyle.Bottom, Height = 28, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(8, 0, 0, 0) };

        private List<string> tiposEvento = new();
        private readonly CancellationTokenSource ctsFormulario = new();
        private CancellationTokenSource? ctsCarga;

        public FrmAuditoria()
        {
            Text = "AUDITORÍA";
            Name = nameof(FrmAuditoria);
            ClientSize = new Size(1100, 700);
            ArmarPantalla();
            ConfigurarGrillas();

            dtpDesde.Value = DateTime.Today.AddDays(-30);
            dtpHasta.Value = DateTime.Today;

            btnBuscar.Click += async (_, _) => await BuscarAsync();
            btnExportar.Click += async (_, _) => await ExportarAsync();
            btnImprimir.Click += (_, _) => ImpresorGrilla.Previsualizar(this, GrillaActual, "Auditoría — " + tabs.SelectedTab!.Text, DescribirFiltros());
            btnTrazabilidad.Click += (_, _) => AbrirTrazabilidad();
            btnVolver.Click += (_, _) => Volver();
            tabs.SelectedIndexChanged += async (_, _) => { AjustarFiltrosPorPestaña(); await BuscarAsync(); };
            dgvLibros.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) AbrirTrazabilidad(); };
            txtLibro.KeyDown += async (_, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; await BuscarAsync(); } };

            KeyPreview = true;
            KeyDown += async (_, e) =>
            {
                if (e.KeyCode == Keys.F5) { e.Handled = true; await BuscarAsync(); }
                else if (e.KeyCode == Keys.Enter && ActiveControl == dgvLibros) { e.Handled = true; AbrirTrazabilidad(); }
            };
            Load += async (_, _) => await InicializarAsync();
            FormClosing += (_, _) => ctsFormulario.Cancel();
            Disposed += (_, _) => { ctsCarga?.Dispose(); ctsFormulario.Dispose(); };
        }

        #region Armado de la pantalla

        private void ArmarPantalla()
        {
            var filtros = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(8, 8, 8, 6), WrapContents = false };
            filtros.Controls.AddRange(new Control[]
            {
                Etiqueta("Desde"), dtpDesde, Etiqueta("Hasta"), dtpHasta, Etiqueta("Usuario"), cbUsuario,
                lblOperacion, cbOperacion, lblLibro, txtLibro, btnBuscar,
            });
            btnBuscar.Margin = new Padding(12, 0, 0, 0);

            var acciones = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(8, 6, 8, 6), FlowDirection = FlowDirection.LeftToRight };
            acciones.Controls.AddRange(new Control[] { btnTrazabilidad, btnExportar, btnImprimir, btnVolver });

            tabs.TabPages.Add(Pestaña("Trazabilidad de libros", dgvLibros));
            tabs.TabPages.Add(Pestaña("Inicios y cierres de sesión", dgvSesiones));
            tabs.TabPages.Add(Pestaña("Eventos de seguridad", dgvEventos));

            Controls.Add(tabs);
            Controls.Add(lblResumen);
            Controls.Add(acciones);
            Controls.Add(filtros);
        }

        private static Label Etiqueta(string texto) => new() { Text = texto, AutoSize = true, Margin = new Padding(12, 8, 4, 0) };

        private static TabPage Pestaña(string titulo, Control contenido)
        {
            var p = new TabPage(titulo) { Padding = new Padding(4) };
            p.Controls.Add(contenido);
            return p;
        }

        private void ConfigurarGrillas()
        {
            GrillaHelper.ConfigurarListado(dgvLibros);
            GrillaHelper.Columna(dgvLibros, nameof(CambioLibroDTO.Fecha), "Fecha", 95, "dd/MM/yyyy HH:mm");
            GrillaHelper.Columna(dgvLibros, nameof(CambioLibroDTO.Usuario), "Usuario", 70);
            GrillaHelper.Columna(dgvLibros, nameof(CambioLibroDTO.Titulo), "Libro", 150);
            GrillaHelper.Columna(dgvLibros, nameof(CambioLibroDTO.ISBN), "ISBN", 85);
            GrillaHelper.Columna(dgvLibros, nameof(CambioLibroDTO.Operacion), "Operación", 90);
            GrillaHelper.Columna(dgvLibros, nameof(CambioLibroDTO.Campo), "Campo", 90);
            GrillaHelper.Columna(dgvLibros, nameof(CambioLibroDTO.ValorAnterior), "Valor anterior", 130);
            GrillaHelper.Columna(dgvLibros, nameof(CambioLibroDTO.ValorNuevo), "Valor nuevo", 130);
            GrillaHelper.Columna(dgvLibros, nameof(CambioLibroDTO.Referencia), "Referencia", 70);

            GrillaHelper.ConfigurarListado(dgvSesiones);
            GrillaHelper.Columna(dgvSesiones, nameof(SesionAuditoriaDTO.Usuario), "Usuario", 90);
            GrillaHelper.Columna(dgvSesiones, nameof(SesionAuditoriaDTO.Inicio), "Inicio de sesión", 110, "dd/MM/yyyy HH:mm:ss");
            GrillaHelper.Columna(dgvSesiones, nameof(SesionAuditoriaDTO.Fin), "Cierre de sesión", 110, "dd/MM/yyyy HH:mm:ss");
            GrillaHelper.Columna(dgvSesiones, nameof(SesionAuditoriaDTO.Duracion), "Duración", 80);
            GrillaHelper.Columna(dgvSesiones, nameof(SesionAuditoriaDTO.Cierre), "Tipo de cierre", 120);

            GrillaHelper.ConfigurarListado(dgvEventos);
            GrillaHelper.Columna(dgvEventos, nameof(EventoSeguridadDTO.Fecha), "Fecha", 100, "dd/MM/yyyy HH:mm:ss");
            GrillaHelper.Columna(dgvEventos, nameof(EventoSeguridadDTO.Evento), "Evento", 110);
            GrillaHelper.Columna(dgvEventos, nameof(EventoSeguridadDTO.Usuario), "Usuario", 80);
            GrillaHelper.Columna(dgvEventos, nameof(EventoSeguridadDTO.Equipo), "Equipo", 80);
            GrillaHelper.Columna(dgvEventos, nameof(EventoSeguridadDTO.Detalle), "Detalle", 260);
        }

        private DataGridView GrillaActual => tabs.SelectedIndex switch { 1 => dgvSesiones, 2 => dgvEventos, _ => dgvLibros };

        private void AjustarFiltrosPorPestaña()
        {
            int i = tabs.SelectedIndex;
            lblOperacion.Visible = cbOperacion.Visible = i != 1;
            lblLibro.Visible = txtLibro.Visible = btnTrazabilidad.Visible = i == 0;
            lblOperacion.Text = i == 2 ? "Evento" : "Operación";
            var opciones = new List<string> { Todos };
            opciones.AddRange(i == 2 ? tiposEvento : AuditoriaService.Operaciones);
            cbOperacion.DataSource = opciones;
        }

        #endregion

        #region Carga

        private async Task InicializarAsync()
        {
            if (!PermisoService.Instancia.TienePermiso(AuditoriaService.PermisoVer))
            {
                btnBuscar.Parent!.Enabled = false;
                tabs.Enabled = false;
                foreach (Control c in btnVolver.Parent!.Controls) c.Enabled = c == btnVolver;
                lblResumen.Text = "No tiene permiso para ver la auditoría.";
                return;
            }
            btnExportar.Visible = PermisoService.Instancia.TienePermiso(AuditoriaService.PermisoExportar);

            try
            {
                var usuarios = await servicio.ObtenerUsuariosAsync(ctsFormulario.Token);
                cbUsuario.DataSource = new[] { Todos }.Concat(usuarios).ToList();
                tiposEvento = await servicio.ObtenerTiposEventoAsync(ctsFormulario.Token);

                // Sugerencias del filtro de libro: títulos e ISBN que empiezan con lo escrito.
                var sugerencias = new AutoCompleteStringCollection();
                sugerencias.AddRange((await servicio.ObtenerSugerenciasLibrosAsync(ctsFormulario.Token)).ToArray());
                txtLibro.AutoCompleteCustomSource = sugerencias;
                txtLibro.AutoCompleteSource = AutoCompleteSource.CustomSource;
                txtLibro.AutoCompleteMode = AutoCompleteMode.Suggest;
            }
            catch (Exception cancelada) when (ManejadorErrores.EsCancelacion(cancelada, ctsFormulario.Token)) { return; }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudieron cargar los filtros de auditoría.");
            }
            AjustarFiltrosPorPestaña();
            await BuscarAsync();
        }

        private FiltroAuditoria ArmarFiltro() => new()
        {
            Desde = dtpDesde.Value.Date,
            Hasta = dtpHasta.Value.Date,
            Usuario = cbUsuario.SelectedItem is string u && u != Todos ? u : null,
            Operacion = cbOperacion.Visible && cbOperacion.SelectedItem is string o && o != Todos ? o : null,
            Texto = tabs.SelectedIndex == 0 ? txtLibro.Text : null,
        };

        private async Task BuscarAsync()
        {
            if (!PermisoService.Instancia.TienePermiso(AuditoriaService.PermisoVer)) return;
            if (dtpDesde.Value.Date > dtpHasta.Value.Date)
            {
                lblResumen.Text = "La fecha \"desde\" no puede ser posterior a \"hasta\".";
                return;
            }

            ctsCarga?.Cancel();
            ctsCarga?.Dispose();
            ctsCarga = CancellationTokenSource.CreateLinkedTokenSource(ctsFormulario.Token);
            var ct = ctsCarga.Token;
            var filtro = ArmarFiltro();
            UseWaitCursor = true;
            try
            {
                int cantidad;
                switch (tabs.SelectedIndex)
                {
                    case 1:
                        var sesiones = await servicio.ObtenerSesionesAsync(filtro, ct);
                        dgvSesiones.DataSource = sesiones;
                        cantidad = sesiones.Count;
                        lblResumen.Text = $"{cantidad} sesiones · {sesiones.Count(s => s.Activa)} activas · " +
                                          $"{sesiones.Count(s => s.Cierre.Contains("Inactividad", StringComparison.OrdinalIgnoreCase) || s.Cierre.Contains("forzado", StringComparison.OrdinalIgnoreCase))} cerradas por inactividad o en forma forzada";
                        break;
                    case 2:
                        var eventos = await servicio.ObtenerEventosAsync(filtro, ct);
                        dgvEventos.DataSource = eventos;
                        cantidad = eventos.Count;
                        lblResumen.Text = $"{cantidad} eventos · {eventos.Count(e => e.Evento == BitacoraSeguridad.LoginFallido)} inicios de sesión fallidos · " +
                                          $"{eventos.Count(e => e.Evento == BitacoraSeguridad.UsuarioBloqueado)} bloqueos";
                        break;
                    default:
                        var cambios = await servicio.ObtenerCambiosLibrosAsync(filtro, ct);
                        dgvLibros.DataSource = cambios;
                        cantidad = cambios.Count;
                        lblResumen.Text = $"{cantidad} cambios en {cambios.Select(c => c.LibroId).Distinct().Count()} libros · " +
                                          $"{cambios.Count(c => c.Campo.StartsWith("Precio"))} cambios de precio";
                        break;
                }
                if (cantidad >= filtro.Maximo)
                    lblResumen.Text += $" · se muestran los {filtro.Maximo} más recientes: acote el período";
            }
            catch (Exception cancelada) when (ManejadorErrores.EsCancelacion(cancelada, ct)) { }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo cargar la auditoría.");
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        #endregion

        #region Acciones

        private void AbrirTrazabilidad()
        {
            int? libroId = dgvLibros.CurrentRow?.DataBoundItem is CambioLibroDTO c ? c.LibroId : null;
            using var dialogo = new FrmTrazabilidadLibro(libroId);
            dialogo.ShowDialog(this);
        }

        private async Task ExportarAsync()
        {
            var grilla = GrillaActual;
            if (grilla.Rows.Count == 0)
            {
                MessageBox.Show(this, "No hay datos para exportar.", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using var dlg = new SaveFileDialog
            {
                Filter = "Excel (CSV)|*.csv",
                FileName = $"Auditoria_{tabs.SelectedTab!.Text.Replace(' ', '_')}_{DateTime.Now:yyyyMMdd_HHmm}.csv",
            };
            if (dlg.ShowDialog(this) != DialogResult.OK) return;
            try
            {
                int filas = await ExportadorGrilla.ExportarCsvAsync(grilla, dlg.FileName, ctsFormulario.Token);
                MessageBox.Show(this, $"Se exportaron {filas} registros.", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo exportar la auditoría.");
            }
        }

        private string DescribirFiltros()
        {
            var f = ArmarFiltro();
            return $"Período {f.Desde:dd/MM/yyyy} a {f.Hasta:dd/MM/yyyy}" +
                   (f.Usuario != null ? $" · Usuario: {f.Usuario}" : "") +
                   (f.Operacion != null ? $" · {lblOperacion.Text}: {f.Operacion}" : "") +
                   (!string.IsNullOrWhiteSpace(f.Texto) ? $" · Libro: {f.Texto}" : "");
        }

        private void Volver()
        {
            if (TopLevelControl is FrmMenu principal)
                principal.MostrarInicio();
            else
                Close();
        }

        #endregion
    }
}
