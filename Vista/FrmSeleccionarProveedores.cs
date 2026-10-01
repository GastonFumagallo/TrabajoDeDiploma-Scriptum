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
    public partial class FrmSeleccionarProveedores : Form
    {
        public List<ProveedorLibroDTO> proveedoresLibros = new List<ProveedorLibroDTO>();
        public FrmSeleccionarProveedores()
        {
            InitializeComponent();
            cargarProveedores();
            actualizarGrilla();
        }
        public void cargarProveedoresLibrosExistentes(Libro libro)
        {
            dgvProvAgregados.DataSource = null;
            var proveedoresLibrosSeleccionados = ControladoraProveedores.Instancia.ObtenerProveedoresLibros(libro.LIB_ID);
            dgvProvAgregados.DataSource = proveedoresLibrosSeleccionados;
            proveedoresLibros = proveedoresLibrosSeleccionados;
            dgvProvAgregados.Columns["PLDTO_PROVID"].Visible = false;
        }
        void actualizarGrilla()
        {
            dgvProvAgregados.DataSource = null;
            dgvProvAgregados.DataSource = proveedoresLibros;
            dgvProvAgregados.Columns["PLDTO_PROVID"].Visible = false;
        }
        void cargarProveedores()
        {
            cbProveedores.DataSource = ControladoraProveedores.Instancia.ObtenerProveedoresGrid();
            cbProveedores.DisplayMember = "Nombre";
            cbProveedores.ValueMember = "PROVDTO_ID";
        }

        private void btnAgregarProvPrecio_Click(object sender, EventArgs e)
        {
            if (cbProveedores.SelectedValue != null && numPrecio.Value > 0)
            {
                int proveedorSeleccionadoID = (int)cbProveedores.SelectedValue;
                Proveedor proveedorSeleccionado = ControladoraProveedores.Instancia.ObtenerProveedorPorId(proveedorSeleccionadoID);
                ProveedorLibroDTO proveedorLibro = new ProveedorLibroDTO();
                proveedorLibro.PLDTO_PROVID = proveedorSeleccionado.PER_Proveedor.PER_ID;
                proveedorLibro.Proveedor = proveedorSeleccionado.PER_Proveedor.PER_Nombre;
                proveedorLibro.Precio = numPrecio.Value;
                proveedoresLibros.Add(proveedorLibro);
                actualizarGrilla();
            }
            else
            {
                MessageBox.Show("Debe seleccionar un proveedor y asignarle un precio mayor a 0.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardarProvPrecio_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnEliminarProvPrecio_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvProvAgregados.CurrentRow != null)
                {
                    var proveedorLibroSeleccionado = (ProveedorLibroDTO)dgvProvAgregados.CurrentRow.DataBoundItem;
                    var confirmacion = MessageBox.Show($"¿Está seguro que desea eliminar el proveedorx '{proveedorLibroSeleccionado.Proveedor}'?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirmacion == DialogResult.Yes)
                    {
                        proveedoresLibros.Remove(proveedorLibroSeleccionado);
                        MessageBox.Show("Proveedor y precio eliminados correctamente");
                        actualizarGrilla();
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
                MessageBox.Show($"Error al eliminar proveedor y precio: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
