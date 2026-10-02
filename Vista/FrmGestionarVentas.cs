using Controladora;
using Modelo;
using Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Vista
{
    /// <summary>
    /// Listado y gestión de ventas: filtros combinables (cliente/DNI, fechas, estado) resueltos en SQL,
    /// paginación, detalle, ticket (reimpresión), exportación a CSV y anulación con reposición de stock.
    ///
    /// Atajos: Enter o doble clic = ver detalle · F5 = refrescar · Ctrl+RePág/AvPág = cambiar de página.
    /// </summary>
    public partial class FrmGestionarVentas : Form
    {
        private const int TamañoPagina = 50;

        private readonly BindingSource bsVentas = new();
        private readonly System.Windows.Forms.Timer timerFiltro = new() { Interval = 300 };

        private int paginaActual = 1;
        private int totalPaginas = 1;
        private bool ocupado;
        private Font? fuenteAnulada;  // se crea una sola vez (crear una Font por celda pierde recursos GDI)

        // Cada carga cancela la anterior: si el usuario tipea rápido sólo se muestra el último resultado.
        private CancellationTokenSource? ctsCarga;
        private readonly CancellationTokenSource ctsFormulario = new();

        public FrmGestionarVentas()
        {
            InitializeComponent();

            ConfigurarGrilla();

            cbEstado.Items.AddRange(new object[] { "Todas las ventas", "Sólo vigentes", "Sólo anuladas" });
            cbEstado.SelectedIndex = 0;
            cbEstado.SelectedIndexChanged += (_, _) => RecargarDesdePrimeraPagina();

            dtpDesde.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpHasta.Value = DateTime.Today;
            dtpDesde.ValueChanged += (_, _) => ProgramarRecarga();
            dtpHasta.ValueChanged += (_, _) => ProgramarRecarga();

            timerFiltro.Tick += (_, _) => { timerFiltro.Stop(); RecargarDesdePrimeraPagina(); };

            btnAnularVenta.Click += btnAnularVenta_Click;
            btnExportar.Click += btnExportar_Click;
            btnPaginaAnterior.Click += (_, _) => CambiarPagina(-1);
            btnPaginaSiguiente.Click += (_, _) => CambiarPagina(+1);

            KeyPreview = true;
            KeyDown += FrmGestionarVentas_KeyDown;
            FormClosing += (_, _) => { timerFiltro.Stop(); ctsFormulario.Cancel(); };
            Disposed += (_, _) => { timerFiltro.Dispose(); fuenteAnulada?.Dispose(); ctsFormulario.Dispose(); };
        }

        #region Configuración

        private void ConfigurarGrilla()
        {
            typeof(DataGridView)
                .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)?
                .SetValue(dgvVentas, true);

            dgvVentas.AllowUserToAddRows = false;
            dgvVentas.AllowUserToDeleteRows = false;
            dgvVentas.MultiSelect = false;
            dgvVentas.RowHeadersVisible = false;
            dgvVentas.DataSource = bsVentas;

            dgvVentas.DataBindingComplete += (_, _) =>
            {
                Ocultar("VENDTO_ID", "Anulada", "MotivoAnulacion");
                Configurar("Fecha", "Fecha", "dd/MM/yyyy HH:mm");
                Configurar("TotalVenta", "Monto total de la venta", "N2");
                Configurar("MetodoPago", "Método de pago");
                if (dgvVentas.Columns["TotalVenta"] is DataGridViewColumn total)
                    total.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            };

            // Las anuladas se ven tachadas y en gris para distinguirlas de un vistazo.
            dgvVentas.CellFormatting += (_, e) =>
            {
                if (e.RowIndex >= 0 && e.CellStyle != null && dgvVentas.Rows[e.RowIndex].DataBoundItem is VentaDTO { Anulada: true })
                {
                    e.CellStyle.ForeColor = Color.Gray;
                    e.CellStyle.Font = fuenteAnulada ??= new Font(dgvVentas.Font, FontStyle.Strikeout);
                }
            };
            dgvVentas.CellToolTipTextNeeded += (_, e) =>
            {
                if (e.RowIndex >= 0 && dgvVentas.Rows[e.RowIndex].DataBoundItem is VentaDTO { Anulada: true } v)
                    e.ToolTipText = $"Anulada: {v.MotivoAnulacion}";
            };

            dgvVentas.SelectionChanged += (_, _) => ActualizarAcciones();
            dgvVentas.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) btnVerDetalles_Click(this, EventArgs.Empty); };
            dgvVentas.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    btnVerDetalles_Click(this, EventArgs.Empty);
                }
            };

            void Ocultar(params string[] columnas)
            {
                foreach (var c in columnas)
                    if (dgvVentas.Columns[c] is DataGridViewColumn col) col.Visible = false;
            }
            void Configurar(string columna, string encabezado, string? formato = null)
            {
                if (dgvVentas.Columns[columna] is not DataGridViewColumn col) return;
                col.HeaderText = encabezado;
                if (formato != null) col.DefaultCellStyle.Format = formato;
            }
        }

        private async void FrmGestionarVentas_Load(object sender, EventArgs e)
        {
            gbFiltrarFecha.Visible = false;
            AplicarSeguridad();
            await CargarVentasAsync();
        }

        private void AplicarSeguridad()
        {
            btnVerTicket.Visible = PermisoService.Instancia.TienePermiso("VerTickets");
            btnRealizarVenta.Visible = PermisoService.Instancia.TienePermiso("RealizarVenta");
            btnVerDetalles.Visible = PermisoService.Instancia.TienePermiso("VerDetalles");
            btnAnularVenta.Visible = PermisoService.Instancia.TienePermiso("AnularVenta");
            btnExportar.Visible = PermisoService.Instancia.TienePermiso("ExportarVentas");
        }

        #endregion

        #region Filtros y carga

        private FiltroVentas? ConstruirFiltro()
        {
            var filtro = new FiltroVentas
            {
                Cliente = txtFiltrar.Text.Trim(),
                Estado = (EstadoVentaFiltro)Math.Max(0, cbEstado.SelectedIndex),
            };

            if (checkFiltrarPorFecha.Checked)
            {
                if (dtpDesde.Value.Date > dtpHasta.Value.Date)
                    return null;  // rango inválido: no se consulta
                filtro.Desde = dtpDesde.Value.Date;
                filtro.Hasta = dtpHasta.Value.Date;
            }
            return filtro;
        }

        private void ProgramarRecarga()
        {
            timerFiltro.Stop();
            timerFiltro.Start();
        }

        private async void RecargarDesdePrimeraPagina()
        {
            paginaActual = 1;
            await CargarVentasAsync();
        }

        private async void CambiarPagina(int delta)
        {
            int nueva = Math.Clamp(paginaActual + delta, 1, totalPaginas);
            if (nueva == paginaActual) return;
            paginaActual = nueva;
            await CargarVentasAsync();
        }

        /// <param name="seleccionarVentaId">Si se indica, se re-selecciona esa venta tras recargar (ej. después de anular).</param>
        private async Task CargarVentasAsync(int? seleccionarVentaId = null)
        {
            var filtro = ConstruirFiltro();
            if (filtro == null)
            {
                lblResumen.Text = "El rango de fechas es inválido: \"Desde\" es posterior a \"Hasta\".";
                return;
            }

            ctsCarga?.Cancel();
            ctsCarga = CancellationTokenSource.CreateLinkedTokenSource(ctsFormulario.Token);
            var token = ctsCarga.Token;

            try
            {
                UseWaitCursor = true;
                var pagina = await ControladoraVentas.Instancia.BuscarVentasAsync(filtro, paginaActual, TamañoPagina, token);
                if (token.IsCancellationRequested) return;

                totalPaginas = Math.Max(1, (int)Math.Ceiling(pagina.TotalRegistros / (double)TamañoPagina));
                if (paginaActual > totalPaginas)
                {
                    // Puede pasar si cambió el filtro o se anularon ventas con el filtro "sólo vigentes".
                    paginaActual = totalPaginas;
                    await CargarVentasAsync(seleccionarVentaId);
                    return;
                }

                bsVentas.DataSource = pagina.Ventas;
                lblPagina.Text = $"Página {paginaActual} de {totalPaginas}";
                lblResumen.Text = pagina.TotalRegistros == 0
                    ? "No se encontraron ventas con los filtros aplicados."
                    : $"{pagina.TotalRegistros} venta(s) · Total facturado (vigentes): ${pagina.TotalFacturado:N2}";

                if (seleccionarVentaId is int id)
                    SeleccionarVenta(id);
            }
            catch (OperationCanceledException)
            {
                // Hubo una carga más nueva o se está cerrando el formulario.
            }
            catch (Exception ex)
            {
                MostrarError("No se pudieron cargar las ventas.", ex);
            }
            finally
            {
                if (!token.IsCancellationRequested)
                {
                    UseWaitCursor = false;
                    ActualizarAcciones();
                }
            }
        }

        private void SeleccionarVenta(int ventaId)
        {
            foreach (DataGridViewRow fila in dgvVentas.Rows)
            {
                if (fila.DataBoundItem is VentaDTO v && v.VENDTO_ID == ventaId)
                {
                    dgvVentas.CurrentCell = fila.Cells.Cast<DataGridViewCell>().First(c => c.Visible);
                    return;
                }
            }
        }

        private void txtFiltrar_TextChanged(object sender, EventArgs e) => ProgramarRecarga();

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            timerFiltro.Stop();
            RecargarDesdePrimeraPagina();
        }

        private void btnBorrarFiltros_Click(object sender, EventArgs e)
        {
            txtFiltrar.Text = "";
            cbEstado.SelectedIndex = 0;
            timerFiltro.Stop();
            RecargarDesdePrimeraPagina();
            txtFiltrar.Focus();
        }

        private void checkFiltrarPorFecha_CheckedChanged(object sender, EventArgs e)
        {
            gbFiltrarFecha.Visible = checkFiltrarPorFecha.Checked;
            RecargarDesdePrimeraPagina();
        }

        private void btnFiltrarFecha_Click(object sender, EventArgs e)
        {
            if (dtpDesde.Value.Date > dtpHasta.Value.Date)
            {
                MessageBox.Show("La fecha \"Desde\" no puede ser posterior a la fecha \"Hasta\".", "Rango inválido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            timerFiltro.Stop();
            RecargarDesdePrimeraPagina();
        }

        private void btnBorrarFiltrosFecha_Click(object sender, EventArgs e)
        {
            dtpDesde.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpHasta.Value = DateTime.Today;
            checkFiltrarPorFecha.Checked = false;  // dispara la recarga
        }

        #endregion

        #region Acciones sobre la venta seleccionada

        private VentaDTO? VentaSeleccionada => dgvVentas.CurrentRow?.DataBoundItem as VentaDTO;

        /// <summary>Habilita cada acción sólo cuando tiene sentido para la fila seleccionada.</summary>
        private void ActualizarAcciones()
        {
            var venta = VentaSeleccionada;
            bool hay = venta != null && !ocupado;
            btnVerDetalles.Enabled = hay;
            btnVerTicket.Enabled = hay;
            btnAnularVenta.Enabled = hay && !venta!.Anulada;
            btnExportar.Enabled = !ocupado && bsVentas.Count > 0;
            btnPaginaAnterior.Enabled = !ocupado && paginaActual > 1;
            btnPaginaSiguiente.Enabled = !ocupado && paginaActual < totalPaginas;
        }

        private void btnVerDetalles_Click(object sender, EventArgs e)
        {
            if (!btnVerDetalles.Visible || VentaSeleccionada is not VentaDTO venta)
            {
                if (btnVerDetalles.Visible) AvisarSinSeleccion();
                return;
            }

            try
            {
                using var detalles = new FrmDetalles();
                detalles.cargarDetalles(new Venta { VEN_ID = venta.VENDTO_ID });
                detalles.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MostrarError("No se pudo abrir el detalle de la venta.", ex);
            }
        }

        /// <summary>Muestra el ticket, desde donde se puede reimprimir. Si la venta está anulada, el ticket lo indica.</summary>
        private async void btnVerTicket_Click(object sender, EventArgs e)
        {
            if (VentaSeleccionada is not VentaDTO venta)
            {
                AvisarSinSeleccion();
                return;
            }

            try
            {
                UseWaitCursor = true;
                var ticket = await ControladoraVentas.Instancia.GenerarTicketAsync(venta.VENDTO_ID, ctsFormulario.Token);
                UseWaitCursor = false;
                if (ticket == null)
                {
                    MessageBox.Show("La venta ya no existe.", "Ticket", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                using var frmTicket = new FrmTickets(ticket);
                frmTicket.ShowDialog(this);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                MostrarError("No se pudo generar el ticket.", ex);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private async void btnAnularVenta_Click(object? sender, EventArgs e)
        {
            if (ocupado) return;
            if (VentaSeleccionada is not VentaDTO venta)
            {
                AvisarSinSeleccion();
                return;
            }
            if (venta.Anulada)
            {
                MessageBox.Show("La venta ya está anulada.", "Anular venta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Paso 1: motivo obligatorio (queda registrado junto con el usuario y la fecha).
            string? motivo = PedirMotivoAnulacion(venta);
            if (motivo == null) return;

            // Paso 2: confirmación explícita con las consecuencias, con "No" como opción por defecto.
            var confirmacion = MessageBox.Show(
                $"Se va a ANULAR la venta N° {venta.VENDTO_ID}:\n\n" +
                $"   Cliente: {venta.Cliente}\n" +
                $"   Fecha: {venta.Fecha:dd/MM/yyyy HH:mm}\n" +
                $"   Total: ${venta.TotalVenta:N2}\n" +
                $"   Motivo: {motivo}\n\n" +
                "• Las unidades vendidas vuelven al stock.\n" +
                "• La venta deja de contar en los reportes.\n" +
                "• Esta acción no se puede deshacer.\n\n" +
                "¿Confirma la anulación?",
                "Confirmar anulación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (confirmacion != DialogResult.Yes) return;

            SetOcupado(true);
            try
            {
                string usuario = PermisoService.Instancia.UsuarioActual?.USU_Nombre ?? "desconocido";
                var resultado = await FacadeVentas.Instancia.AnularVentaAsync(venta.VENDTO_ID, motivo, usuario, ctsFormulario.Token);

                if (resultado.Exito)
                    MessageBox.Show($"La venta N° {venta.VENDTO_ID} fue anulada y el stock fue repuesto.", "Venta anulada",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(resultado.Mensaje, "No se pudo anular", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                await CargarVentasAsync(venta.VENDTO_ID);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                MostrarError("No se pudo anular la venta. No se realizó ningún cambio.", ex);
            }
            finally
            {
                SetOcupado(false);
            }
        }

        /// <summary>Diálogo mínimo para pedir el motivo. Devuelve null si el usuario cancela.</summary>
        private string? PedirMotivoAnulacion(VentaDTO venta)
        {
            using var dlg = new Form
            {
                Text = $"Anular venta N° {venta.VENDTO_ID}",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false,
                ClientSize = new Size(460, 210),
                ShowInTaskbar = false,
            };
            var lbl = new Label { Text = "Motivo de la anulación (obligatorio):", Location = new Point(12, 12), AutoSize = true };
            var txt = new TextBox { Location = new Point(12, 40), Size = new Size(436, 100), Multiline = true, MaxLength = 250 };
            var btnOk = new Button { Text = "Continuar", Location = new Point(262, 160), Size = new Size(90, 34), DialogResult = DialogResult.OK, Enabled = false };
            var btnCancel = new Button { Text = "Cancelar", Location = new Point(358, 160), Size = new Size(90, 34), DialogResult = DialogResult.Cancel };
            txt.TextChanged += (_, _) => btnOk.Enabled = txt.Text.Trim().Length >= 5;
            dlg.Controls.AddRange(new Control[] { lbl, txt, btnOk, btnCancel });
            dlg.AcceptButton = btnOk;
            dlg.CancelButton = btnCancel;

            return dlg.ShowDialog(this) == DialogResult.OK ? txt.Text.Trim() : null;
        }

        /// <summary>Exporta a CSV todas las ventas que cumplen el filtro actual (no sólo la página visible).</summary>
        private async void btnExportar_Click(object? sender, EventArgs e)
        {
            var filtro = ConstruirFiltro();
            if (ocupado || filtro == null) return;

            using var dialogo = new SaveFileDialog
            {
                Filter = "Archivo CSV (Excel)|*.csv",
                FileName = $"Ventas_{DateTime.Now:yyyyMMdd_HHmm}.csv",
            };
            if (dialogo.ShowDialog(this) != DialogResult.OK) return;

            SetOcupado(true);
            try
            {
                var todas = await ControladoraVentas.Instancia.BuscarVentasAsync(filtro, 1, null, ctsFormulario.Token);

                var sb = new StringBuilder();
                sb.AppendLine("N° venta;Fecha;Cliente;Método de pago;Total;Estado;Motivo anulación");
                foreach (var v in todas.Ventas)
                {
                    sb.AppendLine(string.Join(";",
                        v.VENDTO_ID,
                        v.Fecha.ToString("dd/MM/yyyy HH:mm"),
                        Csv(v.Cliente),
                        Csv(v.MetodoPago),
                        v.TotalVenta.ToString("0.00"),
                        v.Estado,
                        Csv(v.MotivoAnulacion)));
                }

                // UTF-8 con BOM y separador ';' para que Excel en español lo abra bien con doble clic.
                await File.WriteAllTextAsync(dialogo.FileName, sb.ToString(), new UTF8Encoding(true), ctsFormulario.Token);
                MessageBox.Show($"Se exportaron {todas.Ventas.Count} venta(s).", "Exportación completa",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                MostrarError("No se pudo exportar el listado.", ex);
            }
            finally
            {
                SetOcupado(false);
            }

            static string Csv(string? valor) =>
                string.IsNullOrEmpty(valor) ? "" : "\"" + valor.Replace("\"", "\"\"") + "\"";
        }

        #endregion

        #region Navegación y utilidades

        private void btnRealizarVenta_Click(object sender, EventArgs e)
        {
            if (TopLevelControl is FrmMenu principal)
                principal.AbrirFormularioPanel(new FrmRealizarVenta());
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["FrmMenu"] is FrmMenu principal)
                principal.MostrarInicio();

            Close();
        }

        private async void FrmGestionarVentas_KeyDown(object? sender, KeyEventArgs e)
        {
            if (ocupado) return;
            switch (e.KeyCode)
            {
                case Keys.F5:
                    e.Handled = true;
                    await CargarVentasAsync(VentaSeleccionada?.VENDTO_ID);
                    break;
                case Keys.PageDown when e.Control:
                    e.Handled = true;
                    CambiarPagina(+1);
                    break;
                case Keys.PageUp when e.Control:
                    e.Handled = true;
                    CambiarPagina(-1);
                    break;
            }
        }

        /// <summary>Bloquea las acciones mientras se anula o exporta, para evitar operaciones duplicadas.</summary>
        private void SetOcupado(bool valor)
        {
            ocupado = valor;
            UseWaitCursor = valor;
            panelSuperior.Enabled = !valor;
            dgvVentas.Enabled = !valor;
            ActualizarAcciones();
        }

        private void AvisarSinSeleccion() =>
            MessageBox.Show("Seleccioná una venta.", "Sin selección", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void MostrarError(string mensaje, Exception ex) =>
            MessageBox.Show($"{mensaje}\n\nDetalle: {ex.GetBaseException().Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

        #endregion
    }
}
