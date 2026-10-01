using Controladora;
using Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Vista.Seguridad
{
    public partial class FrmRecuperarContraseña : Form
    {
        public FrmRecuperarContraseña()
        {
            InitializeComponent();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (txtEmail.Text == string.Empty || txtUsuario.Text == string.Empty)
            {
                MessageBox.Show("Debe completar todos los campos");
            }
            else
            {
                string claveNueva = ServiciosUsuario.GenerarPassword();
                var ok = ControladoraUsuarios.Instancia.RecuperarClave(txtUsuario.Text, txtEmail.Text, claveNueva);
                if (ok)
                {
                    MessageBox.Show("La nueva clave sera enviada a su E-mail registrado");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Usuario o email incorrectos");
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
