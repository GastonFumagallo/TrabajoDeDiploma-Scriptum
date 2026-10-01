using Controladora;
using Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Vista
{
    public partial class FrmMetodosPago : Form
    {
        public FrmMetodosPago()
        {
            InitializeComponent();
            llenarCombo();
            checkActivo.Enabled = false;
        }

        void llenarCombo()
        {
            cbMetodos.DataSource = ControladoraMetodosPago.Instancia.obtenerMetodosPago();
            cbMetodos.DisplayMember = "MP_Nombre";
            cbMetodos.ValueMember = "MP_ID";
        }


        private void btnGuardarMetodo_Click(object sender, EventArgs e)
        {
            MetodoPago metodoDePago = new MetodoPago();
            metodoDePago.MP_Estado = true;
            if (txtMetodoNuevo.Text != string.Empty)
            {
                metodoDePago.MP_Nombre = txtMetodoNuevo.Text;
                var ok = ControladoraMetodosPago.Instancia.AgregarMetodo(metodoDePago);
                if (ok)
                {
                    MessageBox.Show("El metodo de pago fue cargado correctamente");
                    txtMetodoNuevo.Text = string.Empty;
                }
                else
                {
                    MessageBox.Show("El metodo de pago ya existe en el sistema");
                }
            }
            llenarCombo();
        }

        private void btnActivar_Click(object sender, EventArgs e)
        {
            if (cbMetodos.SelectedItem != null)
            {
                var metodoPago = cbMetodos.SelectedItem as MetodoPago;
                if (metodoPago.MP_Estado)
                {
                    MessageBox.Show("El metodo de pago ya se encuentra activo");
                    return;
                }
                else
                {
                    metodoPago.MP_Estado = true;
                    var ok = ControladoraMetodosPago.Instancia.ModificarMetodo(metodoPago);
                    if (ok)
                    {
                        MessageBox.Show("El metodo de pago fue activado de manera exitosa");
                        checkActivo.Checked = metodoPago.MP_Estado;
                    }
                    else
                    {
                        MessageBox.Show("El metodo de pago no fue encontrado");
                    }
                }

            }
        }

        private void btnDarDeBaja_Click(object sender, EventArgs e)
        {
            if (cbMetodos.SelectedItem != null)
            {
                var metodoPago = cbMetodos.SelectedItem as MetodoPago;
                if (!metodoPago.MP_Estado)
                {
                    MessageBox.Show("El metodo de pago ya se encuentra dado de baja");
                    return;
                }
                else
                {
                    metodoPago.MP_Estado = false;
                    var ok = ControladoraMetodosPago.Instancia.ModificarMetodo(metodoPago);
                    if (ok)
                    {
                        MessageBox.Show("El metodo de pago fue dado de baja de manera exitosa");
                        checkActivo.Checked = metodoPago.MP_Estado;
                    }
                    else
                    {
                        MessageBox.Show("El metodo de pago no fue encontrado");
                    }
                }

            }
        }

        private void cbMetodos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMetodos.SelectedItem != null)
            {
                var metodoPago = cbMetodos.SelectedItem as MetodoPago;
                checkActivo.Checked = metodoPago.MP_Estado;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
