using Controladora;
using Modelo.Seguridad;
using Servicios;
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
        private void btnAceptar_Click(object sender, EventArgs e)
        {
            // Los textos de ayuda ("USUARIO", "CONTRASEÑA") no son credenciales: no deben sumar intentos fallidos.
            if (txtClave.Text is "" or "CONTRASEÑA" || txtUsuario.Text is "" or "USUARIO")
            {
                MessageBox.Show("Debe completar todos los campos");
            }
            else
            {
                string nombre = txtUsuario.Text.Trim();
                string clave = txtClave.Text;
                LimpiarCredenciales();   // la clave no queda en pantalla mientras la sesión está abierta

                ResultadoLogin resultado;
                Cursor = Cursors.WaitCursor;   // verificar el hash tarda a propósito (factor de trabajo)
                try { resultado = ControladoraUsuarios.Instancia.IniciarSesion(nombre, clave); }
                finally { Cursor = Cursors.Default; }

                if (!resultado.Exitoso)
                {
                    MessageBox.Show(this, resultado.Error, "Iniciar sesión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var Usuario = resultado.Usuario!;

                // Clave temporal (alta, reseteo o recuperación): no se entra sin elegir una propia.
                if (resultado.DebeCambiarClave)
                {
                    using var cambio = new FrmCambiarClave(Usuario.USU_ID, obligatorio: true);
                    if (cambio.ShowDialog(this) != DialogResult.OK)
                        return;
                }

                var accionesGrupo = Usuario.Grupos.Where(g => g.EstaActivo)
                                   .SelectMany(g => g.Acciones);
                var accionesDirectas = Usuario.Acciones;

                var todasLasAccionesObj = accionesGrupo.Concat(accionesDirectas).ToList();

                var nombresPermisos = todasLasAccionesObj.Select(a => a.ACC_Nombre).Distinct().ToList();

                var nombresFormularios = todasLasAccionesObj
                                            .Where(a => a.Formulario != null)
                                            .Select(a => a.Formulario.FORM_Nombre)
                                            .Distinct()
                                            .ToList();

                ControladoraSesiones.Instancia.RegistrarLogin(Usuario);
                Sesion.Instancia.Usuario = Usuario;
                PermisoService.Instancia.CargarPermisos(nombresPermisos, nombresFormularios);
                PermisoService.Instancia.UsuarioActual = Usuario;
                // ShowDialog no libera el formulario al cerrarse: sin using, cada logout dejaba
                // vivo el menú entero en memoria.
                bool salir;
                using (var menu = new FrmMenu())
                {
                    Hide();
                    menu.ShowDialog();
                    salir = menu.SalirDeLaAplicacion;
                }

                if (salir)
                {
                    Close();   // es el formulario principal: termina la aplicación
                    return;
                }

                LimpiarCredenciales();
                Show();
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
