using Controladora;
using Modelo;
using Modelo.Seguridad;
using Vista.Comun;

namespace Vista.ControlCaja
{
    /// <summary>
    /// Apertura de caja: registra el fondo inicial en efectivo. Devuelve <see cref="DialogResult.OK"/> y el Id de la
    /// caja en <see cref="CajaId"/>. Las reglas (permiso, una sola caja abierta, importe válido) las aplica
    /// <see cref="CajaService"/>; acá sólo se da feedback inmediato.
    /// </summary>
    public partial class FrmAperturaCaja : Form
    {
        private readonly ICajaService servicio = CajaService.Instancia;
        private readonly ValidadorFormulario validador;
        private bool guardando;

        public int? CajaId { get; private set; }

        public FrmAperturaCaja()
        {
            InitializeComponent();
            validador = new ValidadorFormulario(errorProvider).Mapear("MontoInicial", numMonto);
            lblInfo.Text = $"Apertura: {DateTime.Now:dd/MM/yyyy HH:mm}\nResponsable: {Sesion.Instancia.Usuario?.USU_Nombre ?? "—"}";
            numMonto.Enter += (_, _) => numMonto.Select(0, numMonto.Text.Length);
            numMonto.ValueChanged += (_, _) => validador.Limpiar(numMonto);
            btnAbrir.Click += async (_, _) => await AbrirAsync();
            Shown += (_, _) => numMonto.Focus();
        }

        private async Task AbrirAsync()
        {
            if (guardando) return;
            _ = numMonto.Value;   // leer Value confirma lo tipeado aunque todavía no haya salido del control

            if (numMonto.Value == 0 &&
                MessageBox.Show(this, "¿Abrir la caja sin fondo inicial ($ 0)?", Text,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            guardando = true;
            btnAbrir.Enabled = false;
            UseWaitCursor = true;
            try
            {
                CajaId = await servicio.AbrirCajaAsync(numMonto.Value);
                DialogResult = DialogResult.OK;
            }
            catch (ValidacionException ex)
            {
                if (validador.MostrarErrores(ex) is string resto)
                    MessageBox.Show(this, resto, "Revise los datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo abrir la caja.");
            }
            finally
            {
                guardando = false;
                btnAbrir.Enabled = true;
                UseWaitCursor = false;
            }
        }
    }
}
