using Controladora;
using Controladora.Seguridad;
using Modelo.Seguridad;
using Servicios;
using Vista.Comun;
using Vista.Seguridad;
using static System.Collections.Specialized.BitVector32;

namespace Vista
{
    public partial class FrmIniciarSesión : Form
    {
        public FrmIniciarSesión()
        {
            InitializeComponent();

        }
        private bool ingresando;

        private async void btnAceptar_Click(object sender, EventArgs e)
        {
            if (ingresando) return;
            string usuario = txtUsuario.Text.Trim(), clave = txtClave.Text;
            if (usuario.Length == 0 || usuario == "USUARIO" || clave.Length == 0 || (clave == "CONTRASEÑA" && !txtClave.UseSystemPasswordChar))
            {
                MessageBox.Show(this, "Debe completar todos los campos", "Iniciar sesión", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ingresando = true;
            btnAceptar.Enabled = false;
            UseWaitCursor = true;
            try
            {
                // El hash es costoso a propósito (PBKDF2): se verifica fuera del hilo de la interfaz.
                var resultado = await Task.Run(() => UsuarioService.Instancia.AutenticarAsync(usuario, clave));
                switch (resultado.Estado)
                {
                    case EstadoLogin.Invalido:
                        MessageBox.Show(this, "Usuario o clave incorrecta.", "Iniciar sesión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    case EstadoLogin.Bloqueado:
                        MessageBox.Show(this,
                            $"La cuenta está bloqueada por {UsuarioService.IntentosMaximos} intentos fallidos hasta las {resultado.BloqueadoHasta:HH:mm}.\n\n" +
                            "Esperá o pedile a un administrador que la desbloquee.",
                            "Cuenta bloqueada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    case EstadoLogin.Inactivo:
                        MessageBox.Show(this, "La cuenta está desactivada. Consultá con un administrador.", "Cuenta desactivada",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                }

                // Clave temporal (alta o blanqueo): hay que cambiarla antes de entrar.
                if (resultado.DebeCambiarClave)
                {
                    UseWaitCursor = false;
                    using var cambio = new FrmCambiarClave(resultado.UsuarioId, usuario, clave);
                    if (cambio.ShowDialog(this) != DialogResult.OK) return;
                }

                var cuenta = await UsuarioService.Instancia.ObtenerUsuarioSesionAsync(resultado.UsuarioId)
                    ?? throw new InvalidOperationException("La cuenta ya no existe.");
                ControladoraSesiones.Instancia.RegistrarLogin(cuenta);
                Sesion.Instancia.Usuario = cuenta;
                PermisoService.Instancia.IniciarSesion(cuenta);

                UseWaitCursor = false;
                Form form = new FrmMenu();
                this.Hide();
                form.ShowDialog();
                txtUsuario.Text = "USUARIO";
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo iniciar sesión.");
            }
            finally
            {
                ingresando = false;
                btnAceptar.Enabled = true;
                UseWaitCursor = false;
                txtClave.Text = "CONTRASEÑA";
                txtClave.UseSystemPasswordChar = false;
            }
        }

        private void lblRecuperarContraseña_Click(object sender, EventArgs e)
        {
            var form = new FrmRecuperarContraseña();
            this.Hide();
            form.ShowDialog();
            this.Show();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtClave_Enter(object sender, EventArgs e)
        {
            if (txtClave.Text == "CONTRASEÑA")
            {
                txtClave.Text = "";
                txtClave.UseSystemPasswordChar = true;
            }
        }

        private void txtUsuario_Leave(object sender, EventArgs e)
        {
            if (txtUsuario.Text == "")
            {
                txtUsuario.Text = "USUARIO";
                txtUsuario.ForeColor = Color.Black;
            }
        }

        private void txtUsuario_Enter(object sender, EventArgs e)
        {
            if (txtUsuario.Text == "USUARIO")
            {
                txtUsuario.Text = "";
            }
        }

        public void LimpiarCredenciales()
        {
            try
            {
                txtUsuario.Text = "USUARIO";
                txtUsuario.ForeColor = Color.Black;

                txtClave.Text = "CONTRASEÑA";
                txtClave.ForeColor = Color.Black;
                txtClave.UseSystemPasswordChar = false;

            }
            catch { }
        }

        private void pbCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void txtClave_Leave(object sender, EventArgs e)
        {
            if (txtClave.Text == "")
            {
                txtClave.Text = "CONTRASEÑA";
                txtClave.ForeColor = Color.Black;
                txtClave.UseSystemPasswordChar = false;
            }
        }

        private void FrmIniciarSesión_Load(object sender, EventArgs e)
        {
            txtUsuario.Multiline = true;
            txtUsuario.Height = 35;
        }
    }
}
