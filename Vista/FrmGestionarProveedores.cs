using Controladora;
using Modelo;
using Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Vista
{
    public partial class FrmGestionarProveedores : Form
    {
        public FrmGestionarProveedores()
        {
            InitializeComponent();
            CargarProveedores();
        }

        public void CargarProveedores()
        {
            var listaProveedores = ControladoraProveedores.Instancia.ObtenerProveedoresGrid();
            dgvProveedores.DataSource = null;
            dgvProveedores.DataSource = listaProveedores;
            dgvProveedores.Columns["PROVDTO_ID"].Visible = false;
        }


        private void btnAgregar_Click(object sender, EventArgs e)
        {
            FrmAMProveedor agregarProveedor = new FrmAMProveedor();
            agregarProveedor.ShowDialog();
            CargarProveedores();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvProveedores.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccioná un proveedor para modificar.");
                return;
            }

            if (dgvProveedores.CurrentRow != null)
            {
                ProveedorDTO proveedorSeleccionado = (ProveedorDTO)dgvProveedores.CurrentRow.DataBoundItem;
                FrmAMProveedor modificarProveedor = new FrmAMProveedor();
                modificarProveedor.CargarCliente(proveedorSeleccionado);
                modificarProveedor.ShowDialog();
                CargarProveedores();
            }
            else
            {
                MessageBox.Show("Seleccioná un proveedor para modificar.");
                return;
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {

            try
            {
                if (dgvProveedores.CurrentRow != null)
                {
                    var proveedorSeleccionado = (ProveedorDTO)dgvProveedores.CurrentRow.DataBoundItem;
                    var proveedor1 = ControladoraProveedores.Instancia.BuscarProveedorIndividual(proveedorSeleccionado);
                    var confirmacion = MessageBox.Show($"¿Está seguro que desea eliminar el proveedorx '{proveedorSeleccionado.Nombre}'?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirmacion == DialogResult.Yes)
                    {
                        var mensaje = ControladoraProveedores.Instancia.EliminarProveedor(proveedor1);
                        MessageBox.Show(mensaje);
                        CargarProveedores();
                    }
                }
                else
                {
                    MessageBox.Show("Seleccioná un proveedor para eliminar.");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar proveedor: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                var proveedoresFiltrados = ControladoraProveedores.Instancia.BuscarProveedor(filtro);
                dgvProveedores.DataSource = proveedoresFiltrados;
            }
            else
            {
                CargarProveedores();
            }
        }

        private void txtFiltrar_TextChanged(object sender, EventArgs e)
        {
            string filtro = txtFiltrar.Text.Trim().ToLower();
            var proveedoresFiltrados = ControladoraProveedores.Instancia.BuscarProveedor(filtro);
            dgvProveedores.DataSource = proveedoresFiltrados;
        }



        private void btnBorrarFiltros_Click(object sender, EventArgs e)
        {
            CargarProveedores();
            txtFiltrar.Text = string.Empty;
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
            btnAgregar.Visible = PermisoService.Instancia.TienePermiso("AgregarProveedor");
            btnModificar.Visible = PermisoService.Instancia.TienePermiso("ModificarProveedor");
            btnEliminar.Visible = PermisoService.Instancia.TienePermiso("EliminarProveedor");
        }

        private void FrmGestionarProveedores_Load(object sender, EventArgs e)
        {
            AplicarSeguridad();
        }
    }
}
