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
    public partial class FrmModificarCliente : Form
    {
        private ClienteDTO clienteExistente = null;
        public FrmModificarCliente()
        {
            InitializeComponent();
        }

        public void CargarCliente(ClienteDTO cliente)
        {
            clienteExistente = cliente;

            if (cliente != null)
            {
                txtDocumento.Text = cliente.DNI.ToString();
                txtNombreyA.Text = cliente.Nombre;
                txtMail.Text = cliente.Email;
                txtTelefono.Text = cliente.Telefono;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Cliente clienteAM = ControladoraClientes.Instancia.BuscarClienteIndividual(clienteExistente);
            clienteAM.CLI_Persona.PER_DNI = Convert.ToInt32(txtDocumento.Text);
            clienteAM.CLI_Persona.PER_Nombre = txtNombreyA.Text;
            clienteAM.CLI_Persona.PER_Mail = txtMail.Text;
            clienteAM.CLI_Persona.PER_Telefono = txtTelefono.Text;
            ControladoraClientes.Instancia.ModificarCliente(clienteAM);
            MessageBox.Show("Cliente modificado correctamente");
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
