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
    public partial class FrmAMProveedor : Form
    {
        private ProveedorDTO proveedorExistente = null;

        public FrmAMProveedor()
        {
            InitializeComponent();
        }
        public void CargarCliente(ProveedorDTO proveedor)
        {
            lblIngresarDatos.Text = "Modifique los datos del Proveedor y haga click en \"Guardar\"";
            proveedorExistente = proveedor;

            if (proveedor.PROVDTO_ID != 0)
            {
                txtDocumento.Text = proveedor.DNI.ToString();
                txtNombreyA.Text = proveedor.Nombre;
                txtEmpresa.Text = proveedor.Empresa;
                txtMail.Text = proveedor.Email;
                txtTelefono.Text = proveedor.Telefono;
            }
        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreyA.Text) || string.IsNullOrWhiteSpace(txtTelefono.Text) || string.IsNullOrWhiteSpace(txtEmpresa.Text) || string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                MessageBox.Show("Por favor, completa todos los campos.");
                return;
            }

            if (!int.TryParse(txtDocumento.Text, out int documento))
            {
                MessageBox.Show("Por favor, ingresa un número válido para el documento.");
                return;
            }
            if (proveedorExistente == null)
            {
                Persona nuevaPersona = new Persona();

                nuevaPersona.PER_Nombre = txtNombreyA.Text;
                nuevaPersona.PER_DNI = Convert.ToInt32(txtDocumento.Text);
                nuevaPersona.PER_Telefono = txtTelefono.Text;
                nuevaPersona.PER_Mail = txtMail.Text;


                Proveedor nuevoProveedor = new Proveedor();
                nuevoProveedor.PER_ID = nuevaPersona.PER_ID;
                nuevoProveedor.PROV_Empresa = txtEmpresa.Text;
                nuevoProveedor.PER_Proveedor = nuevaPersona;

                ControladoraProveedores.Instancia.AgregarProveedor(nuevoProveedor);
                MessageBox.Show("Proveedor agregado correctamente");
            }
            else
            {
                Proveedor proveedorM = ControladoraProveedores.Instancia.BuscarProveedorIndividual(proveedorExistente);
                proveedorM.PER_Proveedor.PER_DNI = Convert.ToInt32(txtDocumento.Text);
                proveedorM.PER_Proveedor.PER_Nombre = txtNombreyA.Text;
                proveedorM.PER_Proveedor.PER_Mail = txtMail.Text;
                proveedorM.PER_Proveedor.PER_Telefono = txtTelefono.Text;
                proveedorM.PROV_Empresa = txtEmpresa.Text;
                ControladoraProveedores.Instancia.ModificarProveedor(proveedorM);
                MessageBox.Show("Proveedor modificado correctamente");
                this.Close();
            }

            this.Close();
        }


    }
}

