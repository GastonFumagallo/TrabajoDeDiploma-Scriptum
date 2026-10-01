using Controladora;
using Modelo.Seguridad;
using Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Vista.Seguridad;
using Vista.Theme;

namespace Vista
{
    public partial class FrmMenu : Form
    {
        public FrmMenu()
        {
            InitializeComponent();
        }

     

        private void btnClientes_Click(object sender, EventArgs e)
        {
            if(PermisoService.Instancia.PuedeAccederFormulario("FrmGestionarClientes"))
            {
                AbrirFormulario<FrmGestionarClientes>();
            }
        }

        private void btnLibros_Click(object sender, EventArgs e)
        {
            if (PermisoService.Instancia.PuedeAccederFormulario("FrmGestionarLibros"))
            {
                AbrirFormulario<FrmGestionarLibros>();
            }
        }

        private void btnProveedores_Click(object sender, EventArgs e)
        {
            if (PermisoService.Instancia.PuedeAccederFormulario("FrmGestionarProveedores"))
            {
                AbrirFormulario<FrmGestionarProveedores>();
            }
        }

        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            if (PermisoService.Instancia.PuedeAccederFormulario("FrmGestionarUsuarios"))
            {
                AbrirFormulario<FrmGestionarUsuarios>();
            }
        }

        private void btnVentas_Click(object sender, EventArgs e)
        {
            if (PermisoService.Instancia.PuedeAccederFormulario("FrmGestionarVentas"))
            {
                AbrirFormulario<FrmGestionarVentas>();
            }
        }

        private void btnGrupos_Click(object sender, EventArgs e)
        {
            if (PermisoService.Instancia.PuedeAccederFormulario("FrmGestionarGrupos"))
            {
                AbrirFormulario<FrmGestionarGrupos>();
            }
        }
        private string GetNombreUsuarioParaSaludo()
        {
            try
            {
                var usuario = Sesion.Instancia.Usuario;
                if (usuario != null)
                {
                    var persona = usuario.USU_Persona;
                    if (persona != null)
                    {
                        var nom = string.IsNullOrWhiteSpace(persona.PER_Nombre) ? usuario.USU_Nombre : persona.PER_Nombre;
                        return (nom).Trim();
                    }

                    return usuario.USU_Nombre ?? "Usuario";
                }

                return "Usuario";
            }
            catch
            {
                return "Usuario";
            }
        }
        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            DialogResult resul = MessageBox.Show("¿Desea salir del sistema?", "Atencion", MessageBoxButtons.YesNo);

            if (resul == DialogResult.Yes)
            {
                try
                {
                    ControladoraSesiones.Instancia.RegistrarLogout();
                    PermisoService.Instancia.Logout();
                    Sesion.Instancia.Usuario = null;
                    PermisoService.Instancia.UsuarioActual = null;
                }
                catch { }

                try
                {
                    var login = Application.OpenForms.OfType<FrmIniciarSesión>().FirstOrDefault();
                    if (login != null)
                    {
                        login.Invoke((Action)(() =>
                        {
                            login.LimpiarCredenciales();
                            login.Show();
                            login.BringToFront();
                        }));
                    }
                    else
                    {
                        var nuevo = new FrmIniciarSesión();
                        nuevo.Show();
                    }
                }
                catch { }

                this.Close();
            }
        }

        
        public void AbrirFormulario<MiForm>() where MiForm : Form, new()
        {
            panelForm.Controls.Clear();
            MiForm formulario = new MiForm();

            formulario.TopLevel = false;
            formulario.FormBorderStyle = FormBorderStyle.None;
            formulario.Dock = DockStyle.Fill;

            panelForm.Controls.Add(formulario);

            formulario.Show();

            lblTitulo.Text = formulario.Text;
            lblTitulo.ForeColor = Color.White;
        }
        private void ReiniciarBotones()
        {
            btnInicio.BackColor = Color.FromArgb(30, 30, 30);
            btnClientes.BackColor = Color.FromArgb(30, 30, 30);
            btnLibros.BackColor = Color.FromArgb(30, 30, 30);
            btnProveedores.BackColor = Color.FromArgb(30, 30, 30);
            btnVentas.BackColor = Color.FromArgb(30, 30, 30);
            btnReportes.BackColor = Color.FromArgb(30, 30, 30);
            btnUsuarios.BackColor = Color.FromArgb(30, 30, 30);
            btnGrupos.BackColor = Color.FromArgb(30, 30, 30);
            btnGestionarInventario.BackColor = Color.FromArgb(30, 30, 30);

        }
        public void MostrarInicio()
        {
            AbrirFormulario<FrmHome>();
            FrmHome home = panelForm.Controls.OfType<FrmHome>().FirstOrDefault();

            if (home != null)
            {
                home.SetWelcomeText($"Bienvenido, {GetNombreUsuarioParaSaludo()}");
                
            }
            lblTitulo.Text = "INICIO";

            ReiniciarBotones();
            btnInicio.BackColor = Color.FromArgb(126, 146, 167);
        }
        public void MostrarGestionarVentas()
        {
            AbrirFormulario<FrmGestionarVentas>();
            FrmGestionarVentas ventas = panelForm.Controls.OfType<FrmGestionarVentas>().FirstOrDefault();
            lblTitulo.Text = "VENTAS";
            ReiniciarBotones();
            btnVentas.BackColor = Color.FromArgb(126, 146, 167);
        }

        public void AbrirFormularioPanel(Form formularioHijo)
        {
            if (panelForm.Controls.Count > 0)
                panelForm.Controls.RemoveAt(0);

            formularioHijo.TopLevel = false;
            formularioHijo.FormBorderStyle = FormBorderStyle.None;
            formularioHijo.Dock = DockStyle.Fill;
            panelForm.Controls.Add(formularioHijo);
            panelForm.Tag = formularioHijo;
            formularioHijo.Show();
            formularioHijo.BringToFront();

            if (lblTitulo != null) lblTitulo.Text = formularioHijo.Text;
        }

        private void btnInicio_Click(object sender, EventArgs e)
        {
            MostrarInicio();
        }

        private void FrmMenu_Load(object sender, EventArgs e)
        {
            AplicarSeguridad();
            panelForm.Visible = true;

            FuturisticTheme.ApplyToForm(this);

            if (lblTitulo != null) lblTitulo.ForeColor = Color.White;

            AbrirFormulario<FrmHome>();
            var homeOnLoad = panelForm.Controls.OfType<FrmHome>().FirstOrDefault();
            if (homeOnLoad != null)
                homeOnLoad.SetWelcomeText($"Bienvenido, {GetNombreUsuarioParaSaludo()}");
        }
        private void AplicarSeguridad()
        {
            btnClientes.Visible = PermisoService.Instancia.PuedeAccederFormulario("FrmGestionarClientes");
            btnLibros.Visible = PermisoService.Instancia.PuedeAccederFormulario("FrmGestionarLibros");
            btnProveedores.Visible = PermisoService.Instancia.PuedeAccederFormulario("FrmGestionarProveedores");
            btnVentas.Visible = PermisoService.Instancia.PuedeAccederFormulario("FrmGestionarVentas");
            btnReportes.Visible = PermisoService.Instancia.PuedeAccederFormulario("FrmGestionarReportes");
            btnUsuarios.Visible = PermisoService.Instancia.PuedeAccederFormulario("FrmGestionarUsuarios");
            btnGrupos.Visible = PermisoService.Instancia.PuedeAccederFormulario("FrmGestionarGrupos");
        }

        private void pbCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void btnReportes_Click(object sender, EventArgs e)
        {
            if (PermisoService.Instancia.PuedeAccederFormulario("FrmGestionarReportes"))
            {
                AbrirFormulario<FrmGestionarReportes>();
            }
        }

        private void btnGestionarInventario_Click(object sender, EventArgs e)
        {
            if (PermisoService.Instancia.PuedeAccederFormulario("FrmGestionarInventario"))
            {
                AbrirFormulario<FrmGestionarInventario>();
            }
        }
    }
}
