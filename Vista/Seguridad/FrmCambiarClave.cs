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

namespace Vista.Seguridad
{
    public partial class FrmCambiarClave : Form
    {
        public FrmCambiarClave()
        {
            InitializeComponent();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            var clave = txtClaveActual.Text;
            clave = ServiciosUsuario.EncriptarClave(clave);
            if (!ValidarCampos())
            {
                MessageBox.Show("Debe completar todos los campos");
                return;
            }
            if (Sesion.Instancia.Usuario.USU_Clave == clave)
            {
                if (txtClaveNueva.Text == txtConfirmar.Text)
                {
                    var claveNueva = ServiciosUsuario.EncriptarClave(txtClaveNueva.Text);
                    Sesion.Instancia.Usuario.USU_Clave = claveNueva;
                    var ok = ControladoraUsuarios.Instancia.ModificarUsuario(Sesion.Instancia.Usuario);
                    if (ok)
                    {
                        MessageBox.Show("Clave modificada con éxito");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("La accion no se pudo realizar");
                    }
                }
                else
                {
                    MessageBox.Show("Las claves no coinciden");
                }
            }
            else
            {
                MessageBox.Show("Clave incorrecta");
            }
        }

        bool ValidarCampos()
        {
            if (txtClaveActual.Text == string.Empty || txtClaveNueva.Text == string.Empty || txtConfirmar.Text == string.Empty)
            { return false; }
            return true;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
