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
    public partial class FrmOrdenReposicion : Form
    {
        private LibroInventarioDTO libroSeleccionado;
        private ProveedorLibroDTO proveedorSeleccionado;

        public FrmOrdenReposicion()
        {
            InitializeComponent();
        }

        public void actualizarGrillaLibros(List<LibroInventarioDTO> librosInventario)
        {
            dgvLibrosBajoStock.DataSource = null;
            dgvLibrosBajoStock.DataSource = librosInventario;
            dgvLibrosBajoStock.Columns["LINVDTO_ID"].Visible = false;
        }
        public void actualizarGrillaOrdenes()
        {
            dgvOrdenesReposicion.DataSource = null;
            var ordenesActivas = ControladoraOrdenesReposicion.Instancia.ObtenerOrdenesGrid();
            if (ordenesActivas.Count == 0)
            {
                lblNoHayOrdenes.Visible = true;
                dgvOrdenesReposicion.Visible = false;
            }
            else
            {
                lblNoHayOrdenes.Visible = false;
                dgvOrdenesReposicion.Visible = true;
                dgvOrdenesReposicion.DataSource = ordenesActivas;
                dgvOrdenesReposicion.Columns["ORDTO_ID"].Visible = false;
            }
        }


        private void btnSeleccionarLyP_Click(object sender, EventArgs e)
        {
            if (dgvProveedores.CurrentRow == null)
            {
                MessageBox.Show("Seleccioná un proveedor para generar la orden de reposición.");
                return;
            }
            ProveedorLibroDTO proveedor1 = (ProveedorLibroDTO)dgvProveedores.CurrentRow.DataBoundItem;
            proveedorSeleccionado = proveedor1;
            dgvProveedores.Enabled = false;
            dgvProveedores.Visible = false;
            lblProveedor.Visible = true;
            lblTituloProveedor.Visible = false;
            lblProveedorSeleccionado.Visible = true;
            lblProveedorSeleccionado.Text = $"{proveedor1.Proveedor}";
            lblPrecio.Visible = true;
            lblPrecioNumero.Visible = true;
            lblPrecioNumero.Text = $"{proveedor1.Precio}";
            lblCantidad.Visible = true;
            numCantidad.Visible = true;
            btnGenerarOrden.Visible = true;
            btnSeleccionarProveedor.Visible = false;
            btnReiniciarLyP.Visible = true;
        }

        private void btnSeleccionarLibro_Click(object sender, EventArgs e)
        {
            if (dgvLibrosBajoStock.CurrentRow == null)
            {
                MessageBox.Show("Seleccioná un libro para generar la orden de reposición.");
                return;
            }
            LibroInventarioDTO libro1 = (LibroInventarioDTO)dgvLibrosBajoStock.CurrentRow.DataBoundItem;
            libroSeleccionado = libro1;
            if (ControladoraOrdenesReposicion.Instancia.ExisteOrdenActiva(libro1.LINVDTO_ID))
            {
                MessageBox.Show("Ya existe una orden de reposición activa para este libro.", "Orden existente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            lblSeleccion.Visible = true;
            lblLibroSeleccionado.Visible = true;
            lblLibro.Visible = true;
            lblTituloLibro.Visible = false;
            lblLibroSeleccionado.Text = $"{libro1.Titulo}";
            dgvLibrosBajoStock.Enabled = false;
            dgvLibrosBajoStock.Visible = false;
            btnSeleccionarLibro.Visible = false;
            var proveedoresLibro = ControladoraProveedores.Instancia.ObtenerProveedoresLibros(libro1.LINVDTO_ID);
            if (proveedoresLibro.Count == 0)
            {
                MessageBox.Show("No se encontraron proveedores para este libro.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dgvProveedores.Visible = false;
                return;
            }
            lblTituloProveedor.Visible = true;
            dgvProveedores.DataSource = null;
            dgvProveedores.DataSource = proveedoresLibro;
            dgvProveedores.Columns["PLDTO_PROVID"].Visible = false;
            dgvProveedores.Visible = true;
            btnSeleccionarProveedor.Visible = true;

        }

        private void FrmOrdenReposicion_Load(object sender, EventArgs e)
        {
            ocultarLabels();
            actualizarGrillaOrdenes();
        }

        private void ocultarLabels()
        {
            lblSeleccion.Visible = false;
            lblCantidad.Visible = false;
            lblLibroSeleccionado.Visible = false;
            lblProveedorSeleccionado.Visible = false;
            lblLibro.Visible = false;
            lblPrecio.Visible = false;
            lblPrecioNumero.Visible = false;
            lblProveedor.Visible = false;
            numCantidad.Visible = false;
            btnGenerarOrden.Visible = false;
            btnSeleccionarProveedor.Visible = false;
            btnReiniciarLyP.Visible = false;
            dgvProveedores.Visible = false;
            lblTituloProveedor.Visible = false;
        }

        private void mostrarGrids()
        {
            dgvLibrosBajoStock.Enabled = true;
            dgvLibrosBajoStock.Visible = true;
            dgvProveedores.Enabled = true;
            dgvProveedores.Visible = true;
            lblTituloProveedor.Visible = true;
            lblTituloLibro.Visible = true;
            btnSeleccionarLibro.Visible = true;
            numCantidad.Value = 0;
        }

        private void btnReiniciarLyP_Click(object sender, EventArgs e)
        {
            mostrarGrids();
            ocultarLabels();
            libroSeleccionado = null;
            proveedorSeleccionado = null;
        }

        private void btnGenerarOrden_Click(object sender, EventArgs e)
        {
            if (libroSeleccionado == null || proveedorSeleccionado == null)
            {
                MessageBox.Show("Debe seleccionar un libro y un proveedor para generar la orden de reposición.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Libro libroOrden = ControladoraLibros.Instancia.ObtenerLibroPorId(libroSeleccionado.LINVDTO_ID);
            Proveedor proveedorOrden = ControladoraProveedores.Instancia.ObtenerProveedorPorId(proveedorSeleccionado.PLDTO_PROVID);
            int cantidadLibros = (int)numCantidad.Value;
            OrdenReposicion orden = new OrdenReposicion()
            {
                OR_Libro = libroOrden,
                OR_Proveedor = proveedorOrden,
                OR_Cantidad = cantidadLibros,
                OR_Fecha = DateTime.Now,
                OR_Estado = "PENDIENTE"
            };

            ControladoraOrdenesReposicion.Instancia.AgregarOrdenReposicion(orden);

            bool ok = ServiciosOrdenReposicion.EnviarSolicitudReposicion(proveedorOrden, libroOrden, cantidadLibros);

            if (ok)
            {
                orden.OR_Estado = "ENVIADA";
                ControladoraOrdenesReposicion.Instancia.ModificarOrdenReposicion(orden);
                MessageBox.Show("Orden generada y correo enviado.", "Reposición", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("La orden fue creada pero el correo no pudo enviarse.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            actualizarGrillaOrdenes();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
