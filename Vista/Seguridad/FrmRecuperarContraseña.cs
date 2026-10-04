using Controladora.Seguridad;
using Modelo;
using Servicios;
using Vista.Comun;

namespace Vista.Seguridad
{
    /// <summary>
    /// Recuperación de acceso en dos pasos:
    /// <list type="number">
    /// <item>Usuario y email → se envía un código de 6 dígitos con vencimiento. La clave actual NO cambia,
    /// así que saber el usuario y el email de otra persona no alcanza para dejarla afuera.</item>
    /// <item>Código + clave nueva → recién ahí cambia la clave.</item>
    /// </list>
    /// La respuesta del paso 1 es siempre la misma: no revela si el usuario o el email existen.
    /// </summary>
    public partial class FrmRecuperarContraseña : Form
    {
        private readonly IUsuarioService servicio = UsuarioService.Instancia;
        private readonly ErrorProvider errorProvider;
        private readonly ValidadorFormulario validador;
        private bool codigoEnviado;
        private bool trabajando;

        public FrmRecuperarContraseña()
        {
            InitializeComponent();
            AcceptButton = btnAceptar;
            btnAceptar.Text = "Enviar código";
            btnAceptar.Width = 130;
            btnAceptar.Left = ClientSize.Width - btnAceptar.Width - 12;

            errorProvider = new ErrorProvider(this) { BlinkStyle = ErrorBlinkStyle.NeverBlink };
            validador = new ValidadorFormulario(errorProvider)
                .Mapear("Codigo", txtCodigo)
                .Mapear("ClaveNueva", txtClaveNueva);
            Disposed += (_, _) => errorProvider.Dispose();
        }

        private async void btnAceptar_Click(object sender, EventArgs e)
        {
            if (trabajando) return;
            if (codigoEnviado) await CambiarClaveAsync();
            else await EnviarCodigoAsync();
        }

        private async Task EnviarCodigoAsync()
        {
            validador.Limpiar();
            validador.Requerido(txtUsuario, "El usuario");
            validador.Requerido(txtEmail, "El email");
            validador.EnfocarPrimero();
            if (!validador.EsValido) return;

            await EjecutarAsync(async () =>
            {
                string usuario = txtUsuario.Text, email = txtEmail.Text;
                await Task.Run(() => servicio.SolicitarCodigoRecuperacionAsync(usuario, email));
                MostrarPaso2();
            }, "No se pudo procesar el pedido.");
        }

        private void MostrarPaso2()
        {
            codigoEnviado = true;
            txtUsuario.ReadOnly = txtEmail.ReadOnly = true;
            lblInfo.Text = $"Si los datos son correctos, te enviamos un código por email (vence en {UsuarioService.MinutosVigenciaCodigo} minutos). " +
                           "Tu clave actual sigue funcionando hasta que lo uses.";
            foreach (var c in new Control[] { lblInfo, lblCodigo, txtCodigo, lblClaveNueva, txtClaveNueva, lblConfirmar, txtConfirmar })
                c.Visible = true;

            ClientSize = new Size(ClientSize.Width, 470);
            btnAceptar.Text = "Cambiar clave";
            btnAceptar.Location = new Point(ClientSize.Width - btnAceptar.Width - 12, 410);
            txtCodigo.Focus();
        }

        private async Task CambiarClaveAsync()
        {
            validador.Limpiar();
            validador.Regla(txtCodigo, txtCodigo.Text.Trim().Length != 6 || !txtCodigo.Text.Trim().All(char.IsDigit),
                "Ingresá el código de 6 dígitos que te llegó por email.");
            var fortaleza = HasherClaves.ValidarFortaleza(txtClaveNueva.Text, txtUsuario.Text);
            validador.Regla(txtClaveNueva, fortaleza != null, fortaleza ?? "");
            validador.Regla(txtConfirmar, txtConfirmar.Text != txtClaveNueva.Text, "Las claves no coinciden.");
            validador.EnfocarPrimero();
            if (!validador.EsValido) return;

            await EjecutarAsync(async () =>
            {
                string usuario = txtUsuario.Text, codigo = txtCodigo.Text, clave = txtClaveNueva.Text;
                await Task.Run(() => servicio.RecuperarClaveAsync(usuario, codigo, clave));
                MessageBox.Show(this, "Listo: ya podés iniciar sesión con tu clave nueva.", "Recuperar acceso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }, "No se pudo cambiar la clave.");
        }

        private async Task EjecutarAsync(Func<Task> accion, string contexto)
        {
            trabajando = true;
            btnAceptar.Enabled = false;
            UseWaitCursor = true;
            try
            {
                await accion();
            }
            catch (ValidacionException ex)
            {
                if (validador.MostrarErrores(ex) is string resto)
                    MessageBox.Show(this, resto, "Recuperar acceso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, contexto);
            }
            finally
            {
                trabajando = false;
                btnAceptar.Enabled = true;
                UseWaitCursor = false;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
