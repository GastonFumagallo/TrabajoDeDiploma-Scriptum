using Controladora.Seguridad;
using Modelo;
using Servicios;
using Vista.Comun;

namespace Vista.Seguridad
{
    /// <summary>
    /// Cambio de clave de la propia cuenta: exige la clave actual y valida la fortaleza.
    /// En modo obligatorio (clave temporal, primer ingreso o blanqueo) se abre desde el login con la clave
    /// recién ingresada: no se puede saltear y, si se cancela, no se entra al sistema.
    /// </summary>
    public partial class FrmCambiarClave : Form
    {
        private readonly IUsuarioService servicio = UsuarioService.Instancia;
        private readonly int usuarioId;
        private readonly string nombreUsuario;
        private readonly ErrorProvider errorProvider;
        private readonly ValidadorFormulario validador;
        private bool guardando;

        /// <param name="claveTemporal">Modo obligatorio: la clave con la que se acaba de iniciar sesión.</param>
        public FrmCambiarClave(int usuarioId, string nombreUsuario, string? claveTemporal = null)
        {
            InitializeComponent();
            this.usuarioId = usuarioId;
            this.nombreUsuario = nombreUsuario;
            StartPosition = FormStartPosition.CenterParent;
            AcceptButton = btnAceptar;

            errorProvider = new ErrorProvider(this) { BlinkStyle = ErrorBlinkStyle.NeverBlink };
            validador = new ValidadorFormulario(errorProvider)
                .Mapear("ClaveActual", txtClaveActual)
                .Mapear("ClaveNueva", txtClaveNueva);
            Disposed += (_, _) => errorProvider.Dispose();

            lblInfo.Text = $"Mínimo {HasherClaves.LargoMinimo} caracteres y no puede contener tu usuario. " +
                           "Mejor una frase fácil de recordar que una palabra corta con símbolos.";
            if (claveTemporal != null)
            {
                Text = "Cambiá tu clave temporal";
                txtClaveActual.Text = claveTemporal;
                txtClaveActual.Enabled = false;
                lblInfo.Text = "Tu clave es temporal: elegí una personal para continuar. " + lblInfo.Text;
            }
        }

        private async void btnAceptar_Click(object sender, EventArgs e)
        {
            if (guardando) return;

            validador.Limpiar();
            validador.Requerido(txtClaveActual, "La clave actual");
            validador.Regla(txtClaveNueva, HasherClaves.ValidarFortaleza(txtClaveNueva.Text, nombreUsuario) is not null,
                HasherClaves.ValidarFortaleza(txtClaveNueva.Text, nombreUsuario) ?? "");
            validador.Regla(txtConfirmar, txtConfirmar.Text != txtClaveNueva.Text, "Las claves no coinciden.");
            validador.EnfocarPrimero();
            if (!validador.EsValido) return;

            guardando = true;
            btnAceptar.Enabled = false;
            UseWaitCursor = true;
            try
            {
                // El hash es costoso a propósito (PBKDF2): se calcula fuera del hilo de la interfaz.
                string actual = txtClaveActual.Text, nueva = txtClaveNueva.Text;
                await Task.Run(() => servicio.CambiarClaveAsync(usuarioId, actual, nueva));
                MessageBox.Show(this, "Clave modificada con éxito.", "Cambiar clave", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
            }
            catch (ValidacionException ex)
            {
                if (validador.MostrarErrores(ex) is string resto)
                    MessageBox.Show(this, resto, "Revise los datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo cambiar la clave.");
            }
            finally
            {
                guardando = false;
                btnAceptar.Enabled = true;
                UseWaitCursor = false;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
