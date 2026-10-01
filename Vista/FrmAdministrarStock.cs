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
    public partial class FrmAdministrarStock : Form
    {
        public FrmAdministrarStock()
        {
            InitializeComponent();
            actualizarGrilla();
        }

        void actualizarGrilla()
        {
            dgvLibros.DataSource = null;
            dgvLibros.DataSource = ControladoraLibros.Instancia.ObtenerLibrosGrid();
            dgvLibros.Columns["LIBDTO_ID"].Visible = false;
            dgvLibros.Columns["Descripcion"].Visible = false;
            dgvLibros.Columns["Precio"].Visible = false;
            dgvLibros.Columns["Autor"].Visible = false;
            dgvLibros.Columns["AñoPublicacion"].Visible = false;
            dgvLibros.Columns["Genero"].Visible = false;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFiltrar.Text))
            {
                MessageBox.Show("Por favor, complete el campo con el nombre antes de buscar", "Campo incompleto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                actualizarGrilla();
                return;
            }
            string busqueda = txtFiltrar.Text.Trim();
            if (busqueda != null && !busqueda.Equals(""))
            {
                buscarLibro(busqueda);
            }
            else
            {
                actualizarGrilla();
            }
        }
        void buscarLibro(string busqueda)
        {
            var librosFiltrados = ControladoraLibros.Instancia.BuscarLibros(busqueda);
            dgvLibros.DataSource = librosFiltrados;

        }

        private void dgvLibros_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var libro = (LibroDTO)dgvLibros.Rows[e.RowIndex].DataBoundItem;

                numStockLibro.Value = libro.Stock;

            }
        }

        private void txtFiltrar_TextChanged(object sender, EventArgs e)
        {
            string filtro = txtFiltrar.Text.Trim().ToLower();
            buscarLibro(filtro);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {

            try
            {
                if (dgvLibros.Rows.Count > 0)
                {
                    var libroSeleccionado = (LibroDTO)dgvLibros.CurrentRow.DataBoundItem;

                    if (libroSeleccionado != null)
                    {
                        var mensaje = ControladoraLibros.Instancia.ModificarStock(libroSeleccionado, (int)numStockLibro.Value);
                        MessageBox.Show(mensaje);
                        actualizarGrilla();
                    }
                    else
                    {
                        MessageBox.Show("El producto no se ha podido modificar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al modificar producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void LimpiarCampos()
        {
            numStockLibro.Value = 0;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
