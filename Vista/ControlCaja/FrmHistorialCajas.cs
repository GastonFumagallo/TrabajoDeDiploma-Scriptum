using Controladora;
using Modelo;
using Servicios;
using Vista.Comun;

namespace Vista.ControlCaja
{
    /// <summary>
    /// Historial de cajas cerradas (sección embebida en el menú, se abre desde Ventas). Filtra por período de apertura
    /// y por cajero, muestra el arqueo guardado al cerrar y resalta los faltantes. Al seleccionar una caja, el panel
    /// derecho desglosa lo recaudado por medio de pago y las observaciones del cierre.
    /// Sólo habla con <see cref="ICajaService"/>: nada de DbContext ni entidades en la vista.
    ///
    /// Atajos: Enter en los filtros o F5 = buscar · Ctrl+RePág/AvPág = cambiar de página.
    /// </summary>
    public partial class FrmHistorialCajas : Form
    {
        private const int TamañoPagina = 50;

        private static readonly UsuarioCajaDTO TodosLosUsuarios = new(0, "Todos");
        private static readonly Color FondoFaltante = Color.FromArgb(255, 228, 228);
        private static readonly Color TextoFaltante = Color.FromArgb(170, 0, 0);
        private static readonly Color TextoSobrante = Color.FromArgb(150, 95, 0);
        private static readonly Color MetricaFaltante = Color.FromArgb(255, 175, 175);   // legible sobre el panel oscuro

        private readonly ICajaService servicio = CajaService.Instancia;
        private readonly BindingSource bsCajas = new();
        private readonly BindingSource bsMedios = new();
        private readonly System.Windows.Forms.Timer timerDetalle = new() { Interval = 150 };

        private int paginaActual = 1;
        private int totalPaginas = 1;
        private Font? fuenteFaltante;   // una sola instancia (una Font por celda pierde recursos GDI)

        // Cada carga cancela la anterior; al cerrar la sección se cancela todo lo pendiente.
        private readonly CancellationTokenSource ctsFormulario = new();
        private CancellationTokenSource? ctsCarga;
        private CancellationTokenSource? ctsDetalle;

        public FrmHistorialCajas()
        {
            InitializeComponent();
            ConfigurarGrillas();
            RestablecerFiltros();

            btnFiltrar.Click += async (_, _) => await BuscarDesdePrimeraPaginaAsync();
            btnLimpiarFiltros.Click += async (_, _) => { RestablecerFiltros(); await BuscarDesdePrimeraPaginaAsync(); };
            btnVolver.Click += (_, _) => Volver();
            btnPaginaAnterior.Click += async (_, _) => await CambiarPaginaAsync(-1);
            btnPaginaSiguiente.Click += async (_, _) => await CambiarPaginaAsync(+1);
            timerDetalle.Tick += async (_, _) => { timerDetalle.Stop(); await CargarDetalleAsync(); };

            KeyPreview = true;
            KeyDown += FrmHistorialCajas_KeyDown;
            Load += async (_, _) => await InicializarAsync();
            FormClosing += (_, _) => { timerDetalle.Stop(); ctsFormulario.Cancel(); };
            Disposed += (_, _) =>
            {
                timerDetalle.Dispose();
                fuenteFaltante?.Dispose();
                ctsCarga?.Dispose();
                ctsDetalle?.Dispose();
                ctsFormulario.Dispose();
            };
        }

        #region Configuración

        private void ConfigurarGrillas()
        {
            GrillaHelper.ConfigurarListado(dgvCajas);
            GrillaHelper.Columna(dgvCajas, nameof(CajaHistorialDTO.Numero), "N°", peso: 45, derecha: true);
            GrillaHelper.Columna(dgvCajas, nameof(CajaHistorialDTO.FechaApertura), "Apertura", peso: 95, formato: "dd/MM/yyyy HH:mm");
            GrillaHelper.Columna(dgvCajas, nameof(CajaHistorialDTO.UsuarioApertura), "Abrió", peso: 70);
            GrillaHelper.Columna(dgvCajas, nameof(CajaHistorialDTO.FechaCierre), "Cierre", peso: 95, formato: "dd/MM/yyyy HH:mm");
            GrillaHelper.Columna(dgvCajas, nameof(CajaHistorialDTO.UsuarioCierre), "Cerró", peso: 70);
            GrillaHelper.Columna(dgvCajas, nameof(CajaHistorialDTO.MontoInicial), "Monto inicial", peso: 80, formato: "C2", derecha: true);
            GrillaHelper.Columna(dgvCajas, nameof(CajaHistorialDTO.CantidadVentas), "Ventas", peso: 50, derecha: true);
            GrillaHelper.Columna(dgvCajas, nameof(CajaHistorialDTO.TotalVentas), "Total ventas", peso: 90, formato: "C2", derecha: true);
            GrillaHelper.Columna(dgvCajas, nameof(CajaHistorialDTO.EfectivoEsperado), "Efectivo esperado", peso: 90, formato: "C2", derecha: true);
            GrillaHelper.Columna(dgvCajas, nameof(CajaHistorialDTO.EfectivoContado), "Efectivo contado", peso: 90, formato: "C2", derecha: true);
            GrillaHelper.Columna(dgvCajas, nameof(CajaHistorialDTO.Diferencia), "Diferencia", peso: 85, formato: "C2", derecha: true);
            GrillaHelper.Columna(dgvCajas, nameof(CajaHistorialDTO.Resultado), "Resultado", peso: 120);
            dgvCajas.DataSource = bsCajas;

            // Faltante: fila en rojo suave y la diferencia en rojo y negrita. Sobrante: la diferencia en ámbar.
            dgvCajas.CellFormatting += (_, e) =>
            {
                if (e.RowIndex < 0 || e.CellStyle == null || dgvCajas.Rows[e.RowIndex].DataBoundItem is not CajaHistorialDTO caja)
                    return;
                bool columnaDiferencia = dgvCajas.Columns[e.ColumnIndex].Name is nameof(CajaHistorialDTO.Diferencia)
                                                                              or nameof(CajaHistorialDTO.Resultado);
                if (caja.Diferencia < 0)
                {
                    e.CellStyle.BackColor = FondoFaltante;
                    if (columnaDiferencia)
                    {
                        e.CellStyle.ForeColor = TextoFaltante;
                        e.CellStyle.Font = fuenteFaltante ??= new Font(dgvCajas.Font, FontStyle.Bold);
                    }
                }
                else if (caja.Diferencia > 0 && columnaDiferencia)
                {
                    e.CellStyle.ForeColor = TextoSobrante;
                }
            };
            dgvCajas.CellToolTipTextNeeded += (_, e) =>
            {
                if (e.RowIndex >= 0 && dgvCajas.Rows[e.RowIndex].DataBoundItem is CajaHistorialDTO { Observaciones: { } obs })
                    e.ToolTipText = obs;
            };
            dgvCajas.SelectionChanged += (_, _) => ProgramarDetalle();

            GrillaHelper.ConfigurarListado(dgvMedios);
            GrillaHelper.Columna(dgvMedios, nameof(TotalMedioCajaDTO.MedioPago), "Medio de pago", peso: 150);
            GrillaHelper.Columna(dgvMedios, nameof(TotalMedioCajaDTO.Cantidad), "Ventas", peso: 60, derecha: true);
            GrillaHelper.Columna(dgvMedios, nameof(TotalMedioCajaDTO.Total), "Total", peso: 100, formato: "C2", derecha: true);
            dgvMedios.DataSource = bsMedios;
        }

        private async Task InicializarAsync()
        {
            // El menú ya verificó el acceso a Ventas; el historial exige además su propio permiso (y el servicio lo
            // vuelve a exigir). No se navega desde acá: Load corre dentro del Navegar que abre esta sección.
            if (!PermisoService.Instancia.TienePermiso(CajaService.PermisoHistorial))
            {
                panelFiltros.Controls.OfType<Control>().Where(c => c != btnVolver).ToList().ForEach(c => c.Enabled = false);
                btnPaginaAnterior.Enabled = btnPaginaSiguiente.Enabled = false;
                lblResumen.Text = "No tiene permiso para ver el historial de cajas.";
                return;
            }

            try
            {
                var usuarios = await servicio.ObtenerUsuariosConCajasAsync(ctsFormulario.Token);
                cbUsuario.DataSource = new[] { TodosLosUsuarios }.Concat(usuarios).ToList();
            }
            catch (Exception cancelada) when (ManejadorErrores.EsCancelacion(cancelada, ctsFormulario.Token)) { return; }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudieron cargar los cajeros.");
                cbUsuario.DataSource = new List<UsuarioCajaDTO> { TodosLosUsuarios };
            }

            await BuscarDesdePrimeraPaginaAsync();
        }

        #endregion

        #region Filtros y carga

        /// <summary>Por defecto, el mes en curso y todos los cajeros.</summary>
        private void RestablecerFiltros()
        {
            dtpDesde.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpHasta.Value = DateTime.Today;
            if (cbUsuario.Items.Count > 0) cbUsuario.SelectedIndex = 0;
        }

        private FiltroHistorialCajas? ConstruirFiltro(out string? error)
        {
            error = null;
            if (dtpDesde.Value.Date > dtpHasta.Value.Date)
            {
                error = "El período es inválido: \"desde\" es posterior a \"hasta\".";
                return null;
            }
            return new FiltroHistorialCajas
            {
                Desde = dtpDesde.Value.Date,
                Hasta = dtpHasta.Value.Date,
                UsuarioId = cbUsuario.SelectedItem is UsuarioCajaDTO { Id: > 0 } u ? u.Id : null,
            };
        }

        private async Task BuscarDesdePrimeraPaginaAsync()
        {
            paginaActual = 1;
            await CargarAsync();
        }

        private async Task CambiarPaginaAsync(int delta)
        {
            int nueva = Math.Clamp(paginaActual + delta, 1, totalPaginas);
            if (nueva == paginaActual) return;
            paginaActual = nueva;
            await CargarAsync();
        }

        private async Task CargarAsync()
        {
            var filtro = ConstruirFiltro(out string? error);
            if (filtro == null)
            {
                lblResumen.Text = error;
                return;
            }

            ctsCarga?.Cancel();
            ctsCarga?.Dispose();
            ctsCarga = CancellationTokenSource.CreateLinkedTokenSource(ctsFormulario.Token);
            var token = ctsCarga.Token;

            try
            {
                UseWaitCursor = true;
                var pagina = await servicio.BuscarHistorialAsync(filtro, paginaActual, TamañoPagina, token);
                if (token.IsCancellationRequested) return;

                totalPaginas = Math.Max(1, (int)Math.Ceiling(pagina.TotalRegistros / (double)TamañoPagina));
                if (paginaActual > totalPaginas)
                {
                    paginaActual = totalPaginas;   // el filtro achicó el resultado
                    await CargarAsync();
                    return;
                }

                bsCajas.DataSource = pagina.Cajas.ToList();
                MostrarMetricas(pagina.Metricas);
                lblPagina.Text = $"Página {paginaActual} de {totalPaginas}";
                lblResumen.Text = pagina.TotalRegistros == 0
                    ? "No hay cajas cerradas con los filtros aplicados."
                    : $"{pagina.TotalRegistros} caja(s) cerrada(s) · {pagina.Metricas.CajasConFaltante} con faltante.";
                btnPaginaAnterior.Enabled = paginaActual > 1;
                btnPaginaSiguiente.Enabled = paginaActual < totalPaginas;

                if (pagina.TotalRegistros == 0)
                    LimpiarDetalle();
            }
            catch (Exception cancelada) when (ManejadorErrores.EsCancelacion(cancelada, token))
            {
                // Hubo una búsqueda más nueva o se está cerrando la sección.
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo cargar el historial de cajas.");
            }
            finally
            {
                if (!token.IsCancellationRequested)
                    UseWaitCursor = false;
            }
        }

        private void MostrarMetricas(MetricasHistorialCajas m)
        {
            lblCantidad.Text = m.CantidadCajas.ToString("N0");
            lblRecaudado.Text = m.TotalRecaudado.ToString("C2");
            lblDiferencia.Text = m.DiferenciaNeta.ToString("C2");
            lblDiferencia.ForeColor = m.DiferenciaNeta < 0 ? MetricaFaltante : Color.White;
            lblFaltantes.Text = m.CajasConFaltante == 0
                ? "Sin faltantes"
                : $"{m.TotalFaltantes:C2} en {m.CajasConFaltante} caja(s)";
            lblFaltantes.ForeColor = m.CajasConFaltante > 0 ? MetricaFaltante : Color.White;
        }

        #endregion

        #region Detalle

        // Espera un instante: recorrer la grilla con las flechas no dispara una consulta por cada fila.
        private void ProgramarDetalle()
        {
            timerDetalle.Stop();
            timerDetalle.Start();
        }

        private async Task CargarDetalleAsync()
        {
            if (dgvCajas.CurrentRow?.DataBoundItem is not CajaHistorialDTO seleccionada)
            {
                LimpiarDetalle();
                return;
            }

            ctsDetalle?.Cancel();
            ctsDetalle?.Dispose();
            ctsDetalle = CancellationTokenSource.CreateLinkedTokenSource(ctsFormulario.Token);
            var token = ctsDetalle.Token;

            try
            {
                var detalle = await servicio.ObtenerDetalleCajaAsync(seleccionada.Id, token);
                if (token.IsCancellationRequested) return;
                MostrarDetalle(detalle);
            }
            catch (Exception cancelada) when (ManejadorErrores.EsCancelacion(cancelada, token)) { }
            catch (Exception ex)
            {
                LimpiarDetalle();
                ManejadorErrores.Mostrar(this, ex, "No se pudo cargar el detalle de la caja.");
            }
        }

        private void MostrarDetalle(CajaDetalleDTO detalle)
        {
            var c = detalle.Caja;
            lblDetalleTitulo.Text = $"Caja {c.Numero}";
            lblDetalleInfo.Text =
                $"Apertura: {c.FechaApertura:dd/MM/yyyy HH:mm} · {c.UsuarioApertura} · fondo {c.MontoInicial:C2}\n" +
                $"Cierre: {c.FechaCierre:dd/MM/yyyy HH:mm} · {c.UsuarioCierre}\n" +
                $"Ventas: {c.CantidadVentas:N0} por {c.TotalVentas:C2}\n" +
                $"Efectivo esperado {c.EfectivoEsperado:C2} · contado {c.EfectivoContado:C2}\n" +
                c.Resultado;

            bsMedios.DataSource = detalle.TotalesPorMedio.ToList();
            txtObservaciones.Text = string.IsNullOrWhiteSpace(detalle.Observaciones) ? "(sin observaciones)" : detalle.Observaciones;
            lblAnuladasLuego.Text = detalle.VentasAnuladasLuego == 0
                ? (detalle.TotalesPorMedio.Count == 0 ? "La caja se cerró sin ventas." : "")
                : $"⚠ {detalle.VentasAnuladasLuego} venta(s) de esta caja se anularon después del cierre: " +
                  "el arqueo guardado no las descuenta.";
        }

        private void LimpiarDetalle()
        {
            lblDetalleTitulo.Text = "Detalle de la caja";
            lblDetalleInfo.Text = "Seleccioná una caja para ver el desglose por medio de pago.";
            bsMedios.DataSource = new List<TotalMedioCajaDTO>();
            txtObservaciones.Clear();
            lblAnuladasLuego.Text = "";
        }

        #endregion

        #region Navegación

        private async void FrmHistorialCajas_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                case Keys.Enter when ActiveControl == dtpDesde || ActiveControl == dtpHasta || ActiveControl == cbUsuario:
                    e.SuppressKeyPress = true;
                    await BuscarDesdePrimeraPaginaAsync();
                    break;
                case Keys.PageUp when e.Control:
                    e.SuppressKeyPress = true;
                    await CambiarPaginaAsync(-1);
                    break;
                case Keys.PageDown when e.Control:
                    e.SuppressKeyPress = true;
                    await CambiarPaginaAsync(+1);
                    break;
            }
        }

        private void Volver()
        {
            if (TopLevelControl is FrmMenu principal)
                principal.Navegar<FrmGestionarVentas>();   // cierra y libera esta sección
            else
                Close();
        }

        #endregion
    }
}
