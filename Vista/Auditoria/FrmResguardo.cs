using Controladora;
using Modelo;
using Servicios;
using Vista.Comun;

namespace Vista.Auditoria
{
    /// <summary>
    /// Resguardo y restauración de la base de datos (sección embebida en el menú). Permite hacer un resguardo
    /// completo verificado, verificar un resguardo existente y restaurar la base desde uno de ellos. Antes de
    /// restaurar se hace un resguardo automático del estado actual, y después la aplicación se reinicia.
    /// Las carpetas y los archivos son del equipo donde corre SQL Server.
    /// </summary>
    public sealed class FrmResguardo : Form
    {
        private readonly ResguardoService servicio = ResguardoService.Instancia;

        private readonly TextBox txtCarpeta = new() { Width = 480 };
        private readonly TextBox txtDescripcion = new() { Width = 300, PlaceholderText = "Opcional (ej. antes de actualizar precios)" };
        private readonly Button btnExaminar = new() { Text = "…", Width = 34, Height = 28 };
        private readonly Button btnResguardar = new() { Text = "Realizar resguardo", AutoSize = true, MinimumSize = new Size(0, 34) };
        private readonly Button btnVerificar = new() { Text = "Verificar seleccionado", AutoSize = true, MinimumSize = new Size(0, 34) };
        private readonly Button btnVerificarArchivo = new() { Text = "Verificar otro archivo…", AutoSize = true, MinimumSize = new Size(0, 34) };
        private readonly Button btnRestaurar = new() { Text = "Restaurar seleccionado…", AutoSize = true, MinimumSize = new Size(0, 34) };
        private readonly Button btnActualizar = new() { Text = "Actualizar lista", AutoSize = true, MinimumSize = new Size(0, 34) };
        private readonly Button btnVolver = new() { Text = "Volver", AutoSize = true, MinimumSize = new Size(0, 34) };
        private readonly DataGridView dgv = new() { Dock = DockStyle.Fill };
        private readonly Label lblEstado = new() { Dock = DockStyle.Bottom, Height = 30, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(8, 0, 0, 0) };
        private readonly Label lblInfo = new() { Dock = DockStyle.Top, Height = 44, Padding = new Padding(8, 4, 8, 0) };
        private readonly CancellationTokenSource cts = new();
        private bool ocupado;

        public FrmResguardo()
        {
            Text = "RESGUARDO Y RESTAURACIÓN";
            Name = nameof(FrmResguardo);
            ClientSize = new Size(1100, 700);
            Armar();

            btnExaminar.Click += (_, _) => ElegirCarpeta();
            btnResguardar.Click += async (_, _) => await ResguardarAsync();
            btnVerificar.Click += async (_, _) => { if (Seleccionado is { } r) await VerificarAsync(r.Archivo); };
            btnVerificarArchivo.Click += async (_, _) => await VerificarArchivoAsync();
            btnRestaurar.Click += async (_, _) => await RestaurarAsync();
            btnActualizar.Click += async (_, _) => await CargarHistorialAsync();
            btnVolver.Click += (_, _) => Volver();
            Load += async (_, _) => await InicializarAsync();
            FormClosing += (_, e) =>
            {
                if (ocupado)
                {
                    e.Cancel = true;
                    MessageBox.Show(this, "Espere a que termine la operación en curso.", "Resguardo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                cts.Cancel();
            };
            Disposed += (_, _) => cts.Dispose();
        }

        private ResguardoDTO? Seleccionado => dgv.CurrentRow?.DataBoundItem as ResguardoDTO;

        private void Armar()
        {
            lblInfo.Text = $"Base de datos: {ResguardoService.NombreBase}. Los resguardos son completos y se verifican al terminar. " +
                           "La carpeta debe existir en el equipo del servidor SQL y el servicio de SQL Server debe poder escribir en ella.";

            var nuevo = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(8, 8, 8, 6), WrapContents = false };
            nuevo.Controls.AddRange(new Control[]
            {
                new Label { Text = "Carpeta", AutoSize = true, Margin = new Padding(0, 7, 4, 0) }, txtCarpeta, btnExaminar,
                new Label { Text = "Descripción", AutoSize = true, Margin = new Padding(12, 7, 4, 0) }, txtDescripcion, btnResguardar,
            });
            btnResguardar.Margin = new Padding(12, 0, 0, 0);

            GrillaHelper.ConfigurarListado(dgv);
            GrillaHelper.Columna(dgv, nameof(ResguardoDTO.Fecha), "Fecha", 95, "dd/MM/yyyy HH:mm:ss");
            GrillaHelper.Columna(dgv, nameof(ResguardoDTO.Tipo), "Tipo", 60);
            GrillaHelper.Columna(dgv, nameof(ResguardoDTO.Usuario), "Usuario", 70);
            GrillaHelper.Columna(dgv, nameof(ResguardoDTO.TamanioMB), "Tamaño (MB)", 60, "N2", derecha: true);
            GrillaHelper.Columna(dgv, nameof(ResguardoDTO.Archivo), "Archivo", 260);
            GrillaHelper.Columna(dgv, nameof(ResguardoDTO.Descripcion), "Descripción", 170);
            dgv.CellFormatting += (_, e) =>
            {
                if (e.RowIndex >= 0 && e.CellStyle != null && dgv.Rows[e.RowIndex].DataBoundItem is ResguardoDTO { ArchivoDisponible: false })
                    e.CellStyle.ForeColor = Color.Gray;
            };
            dgv.CellToolTipTextNeeded += (_, e) =>
            {
                if (e.RowIndex >= 0 && dgv.Rows[e.RowIndex].DataBoundItem is ResguardoDTO { ArchivoDisponible: false })
                    e.ToolTipText = "El archivo ya no está en esa ubicación (se movió o se eliminó).";
            };

            var acciones = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(8, 6, 8, 6) };
            acciones.Controls.AddRange(new Control[] { btnVerificar, btnVerificarArchivo, btnRestaurar, btnActualizar, btnVolver });

            var titulo = new Label { Text = "Resguardos realizados", Dock = DockStyle.Top, Height = 26, Padding = new Padding(8, 4, 0, 0), Font = new Font("Segoe UI", 10F, FontStyle.Bold) };

            Controls.Add(dgv);
            Controls.Add(titulo);
            Controls.Add(nuevo);
            Controls.Add(lblInfo);
            Controls.Add(lblEstado);
            Controls.Add(acciones);
        }

        private async Task InicializarAsync()
        {
            bool realizar = PermisoService.Instancia.TienePermiso(ResguardoService.PermisoRealizar);
            bool restaurar = PermisoService.Instancia.TienePermiso(ResguardoService.PermisoRestaurar);
            btnRestaurar.Visible = restaurar;
            if (!realizar)
            {
                foreach (var c in new Control[] { txtCarpeta, txtDescripcion, btnExaminar, btnResguardar, btnVerificar, btnVerificarArchivo, btnActualizar, btnRestaurar })
                    c.Enabled = false;
                lblEstado.Text = "No tiene permiso para realizar resguardos.";
                return;
            }
            try
            {
                txtCarpeta.Text = await servicio.ObtenerCarpetaPredeterminadaAsync(cts.Token);
            }
            catch (Exception c) when (ManejadorErrores.EsCancelacion(c, cts.Token)) { return; }
            catch (Exception ex) { ManejadorErrores.Mostrar(this, ex, "No se pudo conectar con el servidor de base de datos."); }
            await CargarHistorialAsync();
        }

        private async Task CargarHistorialAsync()
        {
            try
            {
                var lista = await servicio.ObtenerHistorialAsync(100, cts.Token);
                dgv.DataSource = lista;
                var ultimo = lista.FirstOrDefault();
                lblEstado.Text = ultimo == null
                    ? "Todavía no se realizó ningún resguardo de esta base."
                    : $"Último resguardo: {ultimo.Fecha:dd/MM/yyyy HH:mm} ({(DateTime.Now - ultimo.Fecha).TotalDays:0} días) · {lista.Count} resguardos registrados.";
                if (ultimo != null && (DateTime.Now - ultimo.Fecha).TotalDays > 1)
                    lblEstado.Text += " Atención: pasó más de un día desde el último resguardo.";
            }
            catch (Exception c) when (ManejadorErrores.EsCancelacion(c, cts.Token)) { }
            catch (Exception ex) { ManejadorErrores.Mostrar(this, ex, "No se pudo leer el historial de resguardos."); }
        }

        private void ElegirCarpeta()
        {
            using var dlg = new FolderBrowserDialog { SelectedPath = txtCarpeta.Text, Description = "Carpeta del servidor donde guardar el resguardo" };
            if (dlg.ShowDialog(this) == DialogResult.OK) txtCarpeta.Text = dlg.SelectedPath;
        }

        private async Task Ocupado(string mensaje, Func<Task> accion)
        {
            ocupado = true;
            UseWaitCursor = true;
            Enabled = false;
            lblEstado.Text = mensaje;
            try { await accion(); }
            finally
            {
                Enabled = true;
                UseWaitCursor = false;
                ocupado = false;
            }
        }

        private async Task ResguardarAsync()
        {
            if (string.IsNullOrWhiteSpace(txtCarpeta.Text))
            {
                MessageBox.Show(this, "Indique la carpeta donde guardar el resguardo.", "Resguardo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            await Ocupado("Realizando el resguardo… puede demorar unos minutos.", async () =>
            {
                try
                {
                    var (r, v) = await servicio.RealizarAsync(txtCarpeta.Text, txtDescripcion.Text, cts.Token);
                    MessageBox.Show(this,
                        $"Resguardo realizado:\n{r.Archivo}\n\n{v.Mensaje}",
                        "Resguardo", MessageBoxButtons.OK, v.Valido ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                    txtDescripcion.Clear();
                }
                catch (Exception ex) { ManejadorErrores.Mostrar(this, ex, "No se pudo realizar el resguardo."); }
            });
            await CargarHistorialAsync();
        }

        private async Task VerificarAsync(string ruta)
        {
            await Ocupado("Verificando el resguardo…", async () =>
            {
                try
                {
                    var v = await servicio.VerificarAsync(ruta, cts.Token);
                    BitacoraSeguridad.Registrar(BitacoraSeguridad.ResguardoVerificado, $"{ruta} · {(v.Valido ? "válido" : "inválido")}");
                    MessageBox.Show(this, v.Mensaje, "Verificación", MessageBoxButtons.OK, v.Valido ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                }
                catch (Exception ex) { ManejadorErrores.Mostrar(this, ex, "No se pudo verificar el resguardo."); }
            });
            lblEstado.Text = "";
        }

        private async Task VerificarArchivoAsync()
        {
            using var dlg = new OpenFileDialog { Filter = "Resguardos de SQL Server (*.bak)|*.bak", InitialDirectory = txtCarpeta.Text };
            if (dlg.ShowDialog(this) == DialogResult.OK) await VerificarAsync(dlg.FileName);
        }

        private async Task RestaurarAsync()
        {
            if (Seleccionado is not { } r)
            {
                MessageBox.Show(this, "Seleccione el resguardo a restaurar.", "Restaurar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!r.ArchivoDisponible)
            {
                MessageBox.Show(this, "El archivo de ese resguardo ya no está disponible en el servidor.", "Restaurar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var confirma = MessageBox.Show(this,
                $"Se va a restaurar la base \"{ResguardoService.NombreBase}\" al estado del {r.Fecha:dd/MM/yyyy HH:mm}.\n\n" +
                "• Se perderán todas las operaciones registradas después de esa fecha (ventas, cajas, cambios de stock…).\n" +
                "• Se desconectará a los demás usuarios.\n" +
                "• Antes se hará un resguardo automático del estado actual.\n" +
                "• Al terminar, la aplicación se reiniciará.\n\n¿Restaurar la base?",
                "Restaurar base de datos", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (confirma != DialogResult.Yes) return;

            ResultadoOperacion? resultado = null;
            await Ocupado("Restaurando la base… no cierre la aplicación.", async () =>
            {
                try { resultado = await servicio.RestaurarAsync(r.Archivo, txtCarpeta.Text, CancellationToken.None); }
                catch (Exception ex) { ManejadorErrores.Mostrar(this, ex, "No se pudo restaurar la base."); }
            });
            if (resultado == null) return;
            if (!resultado.Exito)
            {
                MessageBox.Show(this, resultado.Mensaje, "Restaurar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show(this, resultado.Mensaje + "\n\nLa aplicación se va a reiniciar.", "Restauración completa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Application.Restart();
            Environment.Exit(0);
        }

        private void Volver()
        {
            if (TopLevelControl is FrmMenu principal)
                principal.MostrarInicio();
            else
                Close();
        }
    }
}
