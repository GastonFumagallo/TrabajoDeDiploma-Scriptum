using Controladora;
using Modelo;
using Modelo.Seguridad;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Vista
{
    public partial class FrmAgregarCliente : Form
    {
        public FrmAgregarCliente()
        {
            InitializeComponent();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreyA.Text) || string.IsNullOrWhiteSpace(txtMail.Text) || string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                MessageBox.Show("Por favor, completa todos los campos.");
                return;
            }

            if (!int.TryParse(txtDocumento.Text, out int documento))
            {
                MessageBox.Show("Por favor, ingresa un número válido para el documento.");
                return;
            }

            Persona nuevaPersona = new Persona();

            nuevaPersona.PER_Nombre = txtNombreyA.Text;
            nuevaPersona.PER_DNI = Convert.ToInt32(txtDocumento.Text);
            nuevaPersona.PER_Mail = txtMail.Text;
            nuevaPersona.PER_Telefono = txtTelefono.Text;


            Cliente nuevoCliente = new Cliente();
            nuevoCliente.PER_ID = nuevaPersona.PER_ID;
            nuevoCliente.CLI_Persona = nuevaPersona;

            ControladoraClientes.Instancia.AgregarCliente(nuevoCliente);
            MessageBox.Show("Cliente agregado correctamente");

        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
