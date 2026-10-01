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
    public partial class FrmGestionarLibros : Form
    {

        public FrmGestionarLibros()
        {
            InitializeComponent();
            cargarLibros();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            FrmAMLibros agregarLibros = new FrmAMLibros();
            agregarLibros.ShowDialog();
            cargarLibros();
        }

        public void cargarLibros()
        {
            dgvLibros.DataSource = null;
            dgvLibros.DataSource = ControladoraLibros.Instancia.ObtenerLibrosGrid();
            dgvLibros.Columns["LIBDTO_ID"].Visible = false;

        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvLibros.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccioná un libro para modificar.");
                return;
            }

            if (dgvLibros.CurrentRow != null)
            {
                LibroDTO libroSeleccionado = (LibroDTO)dgvLibros.CurrentRow.DataBoundItem;
                FrmAMLibros modificarLibro = new FrmAMLibros();
                modificarLibro.cargarLibro(libroSeleccionado);
                modificarLibro.ShowDialog();
                cargarLibros();
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
                if (dgvLibros.CurrentRow != null)
                {
                    var libroSeleccionado = (LibroDTO)dgvLibros.CurrentRow.DataBoundItem;
                    var libro1 = ControladoraLibros.Instancia.BuscarLibroIndividual(libroSeleccionado);
                    var confirmacion = MessageBox.Show($"¿Está seguro que desea eliminar el libro: '{libro1.LIB_Titulo}'?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirmacion == DialogResult.Yes)
                    {
                        var mensaje = ControladoraLibros.Instancia.EliminarLibro(libro1);
                        MessageBox.Show(mensaje);
                        cargarLibros();
                    }
                }
                else
                {
                    MessageBox.Show("Seleccioná un libro para eliminar.");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar libro: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtFiltrar_TextChanged(object sender, EventArgs e)
        {
            string filtro = txtFiltrar.Text.Trim().ToLower();
            var librosFiltrados = ControladoraLibros.Instancia.BuscarLibros(filtro);
            dgvLibros.DataSource = librosFiltrados;
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFiltrar.Text))
            {
                MessageBox.Show("Ingrese un título de libro para filtrar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string filtro = txtFiltrar.Text.Trim().ToLower();
            if (filtro != null && !filtro.Equals(""))
            {
                var librosFiltrados = ControladoraLibros.Instancia.BuscarLibros(filtro);
                dgvLibros.DataSource = librosFiltrados;
            }
            else
            {
                cargarLibros();
            }
        }

        private void btnBorrarFiltros_Click(object sender, EventArgs e)
        {
            cargarLibros();
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
            btnAgregar.Visible = PermisoService.Instancia.TienePermiso("AgregarLibro");
            btnModificar.Visible = PermisoService.Instancia.TienePermiso("ModificarLibro");
            btnEliminar.Visible = PermisoService.Instancia.TienePermiso("EliminarLibro");
        }

        private void FrmGestionarLibros_Load(object sender, EventArgs e)
        {
            AplicarSeguridad();
        }
    }
}
