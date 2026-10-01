using Controladora;
using Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Vista.Seguridad;

namespace Vista
{
    public partial class FrmAMLibros : Form
    {
        private LibroDTO libroExistente = null;

        public FrmAMLibros()
        {
            InitializeComponent();
            cargarCBGenero();
        }

        public void cargarLibro(LibroDTO libroSeleccionado)
        {
            libroExistente = libroSeleccionado ;

            if (libroSeleccionado.LIBDTO_ID != 0)
            {
                txtTitulo.Text = libroSeleccionado.Titulo;
                txtDescripcion.Text = libroSeleccionado.Descripcion;
                txtEditorial.Text = libroSeleccionado.Editorial;
                txtAutor.Text = libroSeleccionado.Autor;
                txtAñoPublicacion.Text = libroSeleccionado.AñoPublicacion.ToString();
                numStock.Value = libroSeleccionado.Stock;
                cbGeneros.SelectedValue = ControladoraGeneros.Instancia.obtenerIDporNombre(libroSeleccionado.Genero);
            }
        }
        void cargarCBGenero()
        {
            cbGeneros.DataSource = ControladoraGeneros.Instancia.obtenerGeneros();
            cbGeneros.DisplayMember = "GEN_Nombre";
            cbGeneros.ValueMember = "GEN_ID";
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if(libroExistente == null)
            {
                Libro nuevoLibro = new Libro();
                nuevoLibro.LIB_Titulo = txtTitulo.Text;
                nuevoLibro.LIB_Autor = txtAutor.Text;
                nuevoLibro.LIB_Descripcion = txtDescripcion.Text;
                nuevoLibro.LIB_Editorial = txtEditorial.Text;
                nuevoLibro.LIB_AñoPublicacion = int.Parse(txtAñoPublicacion.Text);
                nuevoLibro.LIB_PrecioVenta = numPrecio.Value;
                Genero generoLibro = ControladoraGeneros.Instancia.ObtenerGeneroPorId((int)cbGeneros.SelectedValue);
                nuevoLibro.LIB_Genero = generoLibro;
                nuevoLibro.LIB_Stock = (int)numStock.Value;
                List<ProveedorLibroDTO> proveedoresLibros = new List<ProveedorLibroDTO>();
                FrmSeleccionarProveedores seleccionarProveedores = new FrmSeleccionarProveedores();

                MessageBox.Show("Se abrira otro formulario para seleccionar proveedores y asi poder guardar el libro", "Seleccionar Proveedores", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (seleccionarProveedores.ShowDialog() == DialogResult.OK)
                {
                    proveedoresLibros = seleccionarProveedores.proveedoresLibros;
                }
                ControladoraLibros.Instancia.AgregarLibro(nuevoLibro, proveedoresLibros);
            }
            else
            {
                Libro libroM = ControladoraLibros.Instancia.BuscarLibroIndividual(libroExistente);
                libroM.LIB_Titulo = txtTitulo.Text;
                libroM.LIB_Autor = txtAutor.Text;
                libroM.LIB_Descripcion = txtDescripcion.Text;
                libroM.LIB_Editorial = txtEditorial.Text;
                libroM.LIB_AñoPublicacion = int.Parse(txtAñoPublicacion.Text);
                Genero generoLibro = ControladoraGeneros.Instancia.ObtenerGeneroPorId((int)cbGeneros.SelectedValue);
                libroM.LIB_Genero = generoLibro;
                libroM.LIB_Stock = (int)numStock.Value;
                libroM.LIB_PrecioVenta = numPrecio.Value;
                FrmSeleccionarProveedores seleccionarProveedores = new FrmSeleccionarProveedores();
                List<ProveedorLibroDTO> proveedoresLibros = new List<ProveedorLibroDTO>();
                seleccionarProveedores.cargarProveedoresLibrosExistentes(libroM);
                MessageBox.Show("Se abrira otro formulario para modificar proveedores y asi poder guardar el libro", "Modificar Proveedores", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (seleccionarProveedores.ShowDialog() == DialogResult.OK)
                {
                    proveedoresLibros = seleccionarProveedores.proveedoresLibros;
                }
                ControladoraLibros.Instancia.ModificarLibro(libroM, proveedoresLibros);
                MessageBox.Show("Libro modificado correctamente");
                this.Close();
            }
            
        }

        private void btnAgregarGenero_Click(object sender, EventArgs e)
        {
            FrmAgregarGenero agregarGenero = new FrmAgregarGenero();
            agregarGenero.ShowDialog();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
