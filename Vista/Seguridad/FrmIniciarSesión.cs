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
            if (txtClave.Text == string.Empty || txtUsuario.Text == string.Empty)
            {
                MessageBox.Show("Debe completar todos los campos");
            }
            else
            {
                var Usuario = ControladoraUsuarios.Instancia.IniciarSesion(txtUsuario.Text, txtClave.Text);
                if (Usuario != null)
                {
                    try
                    {
                        var accionesGrupo = Usuario.Grupos.Where(g => g.Estado_Grupo.EST_GRU_ID == 1)
                                           .SelectMany(g => g.Acciones);
                        var accionesDirectas = Usuario.Acciones;

                        var todasLasAccionesObj = accionesGrupo.Concat(accionesDirectas).ToList();

                        var nombresPermisos = todasLasAccionesObj.Select(a => a.ACC_Nombre).Distinct().ToList();

                        var nombresFormularios = todasLasAccionesObj
                                                    .Where(a => a.Formulario != null)
                                                    .Select(a => a.Formulario.FORM_Nombre)
                                                    .Distinct()
                                                    .ToList();

                        var sesion = ControladoraSesiones.Instancia.RegistrarLogin(Usuario);
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
                    catch (Exception)
                    {

                        throw;
                    }
                }
                else
                {
                    MessageBox.Show("Usuario o clave incorrecta");
                }
            }
            txtClave.Text = "CONTRASEÑA";
            txtClave.UseSystemPasswordChar = false;
            txtUsuario.Text = "USUARIO";
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
