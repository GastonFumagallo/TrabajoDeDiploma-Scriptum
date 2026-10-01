using Controladora;
using Modelo;
using Modelo.Seguridad;
using Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Vista.Theme;

namespace Vista
{
    public partial class FrmGestionarClientes : Form
    {
        public FrmGestionarClientes()
        {
            InitializeComponent();
            CargarClientes();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            FrmAgregarCliente vistaCliente = new FrmAgregarCliente();
            vistaCliente.ShowDialog();
            CargarClientes();
        }

        public void CargarClientes()
        {
            var listaClientes = ControladoraClientes.Instancia.ObtenerClientesGrid();
            dgvClientes.DataSource = null;
            dgvClientes.DataSource = listaClientes;
            dgvClientes.Columns["CLIDTO_ID"].Visible = false;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            FrmMenu principal = Application.OpenForms["FrmMenu"] as FrmMenu;

            if (principal != null)
            {
                principal.MostrarInicio();
            }

            this.Close();

        }
        private void AplicarSeguridad()
        {
            btnAgregar.Visible = PermisoService.Instancia.TienePermiso("AgregarCliente");
            btnModificar.Visible = PermisoService.Instancia.TienePermiso("ModificarCliente");
            btnEliminar.Visible = PermisoService.Instancia.TienePermiso("EliminarCliente");
        }
        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccioná un cliente para modificar.");
                return;
            }
            if (dgvClientes.CurrentRow != null)
            {
                ClienteDTO clienteSeleccionado = (ClienteDTO)dgvClientes.CurrentRow.DataBoundItem;
                FrmModificarCliente modificarCliente = new FrmModificarCliente();
                modificarCliente.CargarCliente(clienteSeleccionado);
                modificarCliente.ShowDialog();
                CargarClientes();
            }
            else
            {
                MessageBox.Show("Seleccioná un cliente para modificar.");
                return;
            }

        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvClientes.CurrentRow != null)
                {
                    var clienteSeleccionado = (ClienteDTO)dgvClientes.CurrentRow.DataBoundItem;
                    var cliente1 = ControladoraClientes.Instancia.BuscarClienteIndividual(clienteSeleccionado);
                    var confirmacion = MessageBox.Show($"¿Está seguro que desea eliminar el cliente '{clienteSeleccionado.Nombre}'?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirmacion == DialogResult.Yes)
                    {
                        var mensaje = ControladoraClientes.Instancia.EliminarCliente(cliente1);
                        MessageBox.Show(mensaje);
                        CargarClientes();
                    }
                }
                else
                {
                    MessageBox.Show("Seleccioná un cliente para eliminar.");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBorrarFiltros_Click(object sender, EventArgs e)
        {
            CargarClientes();
            txtFiltrar.Text = string.Empty;
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFiltrar.Text))
            {
                MessageBox.Show("Ingrese un número de documento para filtrar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtFiltrar.Text, out int dni))
            {
                MessageBox.Show("El DNI debe contener solo números.", "Formato Incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string filtro = txtFiltrar.Text.Trim().ToLower();
            if (filtro != null && !filtro.Equals(""))
            {
                var clientesFiltrados = ControladoraClientes.Instancia.BuscarCliente(filtro);
                dgvClientes.DataSource = clientesFiltrados;
            }
            else
            {
                CargarClientes();
            }


        }

        private void txtFiltrar_TextChanged(object sender, EventArgs e)
        {
            string filtro = txtFiltrar.Text.Trim().ToLower();

            var clientesFiltrados = ControladoraClientes.Instancia.BuscarCliente(filtro);
            dgvClientes.DataSource = clientesFiltrados;
        }

        private void FrmGestionarClientes_Load(object sender, EventArgs e)
        {
            AplicarSeguridad();
        }
    }
}
