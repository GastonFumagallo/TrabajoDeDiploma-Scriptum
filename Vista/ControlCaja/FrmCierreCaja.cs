using Controladora;
using Modelo;
using Vista.Comun;

namespace Vista.ControlCaja
{
    /// <summary>
    /// Cierre de caja con arqueo: muestra lo recaudado por medio de pago y el efectivo esperado, pide el efectivo
    /// contado y calcula la diferencia en vivo. El cierre lo hace <see cref="CajaService"/>, que vuelve a totalizar
    /// con la caja bloqueada (si entró una venta mientras esta pantalla estaba abierta, el resultado la incluye).
    /// </summary>
    public partial class FrmCierreCaja : Form
    {
        private readonly ICajaService servicio = CajaService.Instancia;
        private readonly ValidadorFormulario validador;
        private readonly BindingSource bsTotales = new();
        private readonly int cajaId;
        private ArqueoCajaDTO? arqueo;
        private bool guardando;

        /// <summary>Arqueo definitivo (válido cuando el diálogo devuelve OK).</summary>
        public ArqueoCajaDTO? Resultado { get; private set; }

        public FrmCierreCaja(int cajaId)
        {
            InitializeComponent();
            this.cajaId = cajaId;

            validador = new ValidadorFormulario(errorProvider)
                .Mapear("EfectivoContado", numContado)
                .Mapear("Observaciones", txtObservaciones);

            GrillaHelper.ConfigurarListado(dgvTotales);
            GrillaHelper.Columna(dgvTotales, nameof(TotalMedioCajaDTO.MedioPago), "Medio de pago", peso: 160);
            GrillaHelper.Columna(dgvTotales, nameof(TotalMedioCajaDTO.Cantidad), "Ventas", peso: 60, derecha: true);
            GrillaHelper.Columna(dgvTotales, nameof(TotalMedioCajaDTO.Total), "Total", peso: 100, formato: "C2", derecha: true);
            dgvTotales.DataSource = bsTotales;

            numContado.Enter += (_, _) => numContado.Select(0, numContado.Text.Length);
            numContado.ValueChanged += (_, _) => { validador.Limpiar(numContado); ActualizarDiferencia(); };
            numContado.TextChanged += (_, _) => ActualizarDiferencia();   // en vivo, mientras se tipea
            txtObservaciones.TextChanged += (_, _) => validador.Limpiar(txtObservaciones);
            btnActualizar.Click += async (_, _) => await CargarAsync();
            btnCerrarCaja.Click += async (_, _) => await CerrarAsync();
            Load += async (_, _) => await CargarAsync();
        }

        private async Task CargarAsync()
        {
            try
            {
                UseWaitCursor = true;
                arqueo = await servicio.ObtenerArqueoAsync(cajaId);
                lblEncabezado.Text = $"Caja #{arqueo.CajaId}";
                lblApertura.Text = $"Abierta el {arqueo.FechaApertura:dd/MM/yyyy HH:mm} por {arqueo.UsuarioApertura} · " +
                                   $"monto inicial {arqueo.MontoInicial:C2}";
                bsTotales.DataSource = arqueo.TotalesPorMedio.ToList();
                lblCantidad.Text = arqueo.CantidadVentas.ToString("N0");
                lblTotalVentas.Text = arqueo.TotalVentas.ToString("C2");
                lblEsperado.Text = arqueo.EfectivoEsperado.ToString("C2");
                ActualizarDiferencia();
                numContado.Focus();
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo calcular el arqueo de la caja.");
                if (arqueo == null) DialogResult = DialogResult.Cancel;
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        /// <summary>Lo tipeado aunque todavía no se haya confirmado en el control (para la diferencia en vivo).</summary>
        private decimal ContadoTipeado =>
            decimal.TryParse(numContado.Text, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.CurrentCulture, out var valor)
                ? Math.Clamp(valor, numContado.Minimum, numContado.Maximum)
                : numContado.Value;

        private decimal? Diferencia => arqueo == null ? null : ContadoTipeado - arqueo.EfectivoEsperado;

        private void ActualizarDiferencia()
        {
            if (Diferencia is not decimal diferencia) return;
            lblDiferencia.Text = ArqueoCajaDTO.DescribirDiferencia(diferencia);
            lblDiferencia.ForeColor = diferencia switch
            {
                > 0 => Color.DarkGoldenrod,
                < 0 => Color.Firebrick,
                _ => Color.DarkGreen,
            };
        }

        private async Task CerrarAsync()
        {
            if (guardando || arqueo == null) return;
            _ = numContado.Value;   // confirma lo tipeado

            validador.Limpiar();
            var diferencia = Diferencia!.Value;
            validador.Regla(txtObservaciones, diferencia != 0 && string.IsNullOrWhiteSpace(txtObservaciones.Text),
                $"{ArqueoCajaDTO.DescribirDiferencia(diferencia)}: explicá la diferencia en las observaciones.");
            validador.EnfocarPrimero();
            if (!validador.EsValido) return;

            if (MessageBox.Show(this,
                    $"¿Cerrar la caja #{arqueo.CajaId}?\n\n" +
                    $"Efectivo esperado: {arqueo.EfectivoEsperado:C2}\n" +
                    $"Efectivo contado: {numContado.Value:C2}\n" +
                    $"{ArqueoCajaDTO.DescribirDiferencia(diferencia)}\n\n" +
                    "Después de cerrarla no se pueden registrar ventas hasta abrir otra caja.",
                    "Cerrar caja", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            guardando = true;
            btnCerrarCaja.Enabled = btnActualizar.Enabled = false;
            UseWaitCursor = true;
            try
            {
                Resultado = await servicio.CerrarCajaAsync(cajaId, numContado.Value, txtObservaciones.Text);
                DialogResult = DialogResult.OK;
            }
            catch (ValidacionException ex)
            {
                // Ej.: entró una venta en efectivo mientras se contaba y ahora hay una diferencia sin explicar.
                await CargarAsync();
                if (validador.MostrarErrores(ex) is string resto)
                    MessageBox.Show(this, resto, "Revise los datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo cerrar la caja.");
            }
            finally
            {
                guardando = false;
                btnCerrarCaja.Enabled = btnActualizar.Enabled = true;
                UseWaitCursor = false;
            }
        }

        /// <summary>Texto del resumen para mostrar después del cierre.</summary>
        public static string Resumir(ArqueoCajaDTO r) =>
            $"Caja #{r.CajaId} cerrada{(r.FechaCierre is DateTime f ? $" el {f:dd/MM/yyyy HH:mm}" : "")}.\n\n" +
            $"Ventas: {r.CantidadVentas:N0} · Total vendido: {r.TotalVentas:C2}\n" +
            string.Concat(r.TotalesPorMedio.Select(t => $"   {t.MedioPago}: {t.Total:C2} ({t.Cantidad})\n")) +
            $"\nEfectivo esperado: {r.EfectivoEsperado:C2}\n" +
            $"Efectivo contado: {r.Contado:C2}\n" +
            (r.Diferencia is decimal d ? ArqueoCajaDTO.DescribirDiferencia(d) : "");
    }
}
