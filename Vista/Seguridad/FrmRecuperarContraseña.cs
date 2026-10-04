using Controladora;
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
                // El mismo mensaje exista o no el usuario: la pantalla no sirve para averiguar cuentas válidas.
                ControladoraUsuarios.Instancia.SolicitarRecuperacion(txtUsuario.Text.Trim(), txtEmail.Text.Trim());
                MessageBox.Show(this,
                    "Si el usuario y el email son correctos, vas a recibir una clave temporal válida por 30 minutos.\n" +
                    "Tu clave actual sigue funcionando hasta que uses la temporal.",
                    "Recuperar clave", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
