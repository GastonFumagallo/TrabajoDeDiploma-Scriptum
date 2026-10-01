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
    public partial class FrmRealizarVenta : Form
    {
        private List<LibroVentaDTO> librosVenta;
        private ClienteDTO clienteActual;
        private MetodoPago metodoPagoSeleccionado;
        public FrmRealizarVenta()
        {
            InitializeComponent();
            actualizarGrillaLibros();
            actualizarGrillaFactura();
            actualizarGrillaClientes();
            librosVenta = new List<LibroVentaDTO>();
            clienteActual = new ClienteDTO();
            clienteActual = null;
            metodoPagoSeleccionado = new MetodoPago();
            metodoPagoSeleccionado = null;
        }

        void actualizarGrillaClientes()
        {
            dgvClientes.DataSource = null;
            dgvClientes.DataSource = ControladoraClientes.Instancia.ObtenerClientesGrid();
            dgvClientes.Columns["CLIDTO_ID"].Visible = false;
        }

        void actualizarGrillaFactura()
        {
            dgvVentas.DataSource = null;
            dgvVentas.DataSource = librosVenta;
            if (dgvVentas.Columns.Contains("Precio"))
            {
                dgvVentas.Columns["Precio"].DefaultCellStyle.Format = "N2";
            }
            if (dgvVentas.Columns.Contains("LVDTO_ID"))
            {
                dgvVentas.Columns["LVDTO_ID"].Visible = false;
            }
        }
        void actualizarGrillaLibros()
        {
            dgvLibros.DataSource = null;
            dgvLibros.DataSource = ControladoraLibros.Instancia.ObtenerLibrosGrid();
            dgvLibros.Columns["LIBDTO_ID"].Visible = false;
            dgvLibros.Columns["Descripcion"].Visible = false;
            dgvLibros.Columns["AñoPublicacion"].Visible = false;
            dgvLibros.Columns["Genero"].Visible = false;
        }

        private void dgvLibros_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var libro1 = (LibroDTO)dgvLibros.Rows[e.RowIndex].DataBoundItem;
                lblTitulo.Text = libro1.Titulo;
                lblPrecio.Text = libro1.Precio.ToString();
            }
        }

        private void btnAgregarLibro_Click(object sender, EventArgs e)
        {
            if (dgvLibros.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un libro.");
                return;
            }

            if (numCantidad.Value == 0)
            {
                MessageBox.Show("Por favor, el valor de la cantidad debe ser distinto de 0.", "Valor inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var libroSeleccionado = (LibroDTO)dgvLibros.CurrentRow.DataBoundItem;

            int idLibroSeleccionado = libroSeleccionado.LIBDTO_ID;

            if (LibroYaAgregado(idLibroSeleccionado))
            {
                MessageBox.Show("Ese libro ya fue agregado a la venta.", "Libro duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int cantidadSolicitada = (int)numCantidad.Value;

            if (cantidadSolicitada > libroSeleccionado.Stock)
            {
                MessageBox.Show($"No hay suficiente stock de {libroSeleccionado.Titulo}. " + $"Disponible: {libroSeleccionado.Stock}, solicitado: {cantidadSolicitada}");
                return;
            }

            var libroVenta = new LibroVentaDTO
            {
                LVDTO_ID = libroSeleccionado.LIBDTO_ID,
                Titulo = libroSeleccionado.Titulo,
                Editorial = libroSeleccionado.Editorial,
                Autor = libroSeleccionado.Autor,
                Precio = libroSeleccionado.Precio,
                Cantidad = (int)numCantidad.Value,
            };
            librosVenta.Add(libroVenta);
            lblPasoTres.Visible = true;
            lblTotal.Text = "Total:";
            lblTotalNumero.Text = librosVenta.Sum(p => p.Precio * p.Cantidad).ToString("N2");
            numCantidad.Value = 0;
            dgvVentas.Visible = true;
            btnEliminarLibro.Visible = true;
            btnGenerarVenta.Visible = true;
            actualizarGrillaFactura();
        }
        private bool LibroYaAgregado(int idLibro)
        {
            foreach (DataGridViewRow fila in dgvVentas.Rows)
            {
                if (fila.IsNewRow) continue;

                LibroVentaDTO libro = (LibroVentaDTO)fila.DataBoundItem;

                if (libro.LVDTO_ID != null && libro.LVDTO_ID == idLibro)
                {
                    return true;
                }
            }
            return false;
        }
        private void btnEliminarLibro_Click(object sender, EventArgs e)
        {
            if (dgvVentas.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un libro de la venta para eliminar.", "Sin selección", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var libroSeleccionado = (LibroVentaDTO)dgvVentas.CurrentRow.DataBoundItem;

            var confirmacion = MessageBox.Show($"¿Desea eliminar el libro '{libroSeleccionado.Titulo}' de la venta?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                librosVenta.Remove(libroSeleccionado);

                lblTotalNumero.Text = librosVenta.Sum(p => p.Precio * p.Cantidad).ToString("N2");

                if (librosVenta.Count == 0)
                {
                    lblPasoTres.Visible = false;
                    sinLibrosVenta();
                }

                actualizarGrillaFactura();
            }

        }

        private void btnGenerarVenta_Click(object sender, EventArgs e)
        {
            if (librosVenta.Count == 0)
            {
                MessageBox.Show("Debe agregar al menos un libro a la venta.", "Venta sin productos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var frmConcretarVenta = new FrmConcretarVenta())
            {
                frmConcretarVenta.PrecioTotal = lblTotalNumero.Text;
                if (frmConcretarVenta.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                var cliente = frmConcretarVenta.ClienteSeleccionado;
                clienteActual = cliente;
                var metodoPago = frmConcretarVenta.MetodoPagoSeleccionado;
                metodoPagoSeleccionado = metodoPago;
            }
            var venta = FacadeVentas.Instancia.RealizarVenta(clienteActual, librosVenta, metodoPagoSeleccionado);
            if (venta != null)
            {
                librosVenta.Clear();
                actualizarGrillaFactura();
                actualizarGrillaLibros();
                clienteActual = null;
                metodoPagoSeleccionado = null;
                lblTotal.Text = "";
                lblTotalNumero.Text = "";
                MessageBox.Show("Venta creada con sus detalles.");
            }


        }

        private void btnMetodosPago_Click(object sender, EventArgs e)
        {
            FrmMetodosPago metodosPago = new FrmMetodosPago();
            metodosPago.ShowDialog();
        }




        private void FrmRealizarVenta_Load(object sender, EventArgs e)
        {
            sinLibrosVenta();
            modoInicio();
        }

        void modoInicio()
        {
            gbSeleccionarLibros.Enabled = false;
            btnSelectOtroCliente.Visible = false;
        }

        void sinLibrosVenta()
        {
            lblPasoTres.Visible = false;
            lblTotal.Text = "";
            lblTotalNumero.Text = "";
            dgvVentas.Visible = false;
            btnEliminarLibro.Visible = false;
            btnGenerarVenta.Visible = false;
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFiltrarLibro.Text))
            {
                MessageBox.Show("Ingrese un título de libro para filtrar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string filtro = txtFiltrarLibro.Text.Trim().ToLower();
            if (filtro != null && !filtro.Equals(""))
            {
                var librosFiltrados = ControladoraLibros.Instancia.BuscarLibros(filtro);
                dgvLibros.DataSource = librosFiltrados;
            }
            else
            {
                actualizarGrillaLibros();
            }
        }

        private void btnBorrarFiltros_Click(object sender, EventArgs e)
        {
            actualizarGrillaLibros();
            txtFiltrarLibro.Text = string.Empty;
        }

        private void txtFiltrar_TextChanged(object sender, EventArgs e)
        {
            string filtro = txtFiltrarLibro.Text.Trim().ToLower();
            var librosFiltrados = ControladoraLibros.Instancia.BuscarLibros(filtro);
            dgvLibros.DataSource = librosFiltrados;
        }

        private void btnVolverGestionarVEN_Click(object sender, EventArgs e)
        {
            FrmMenu principal = Application.OpenForms["FrmMenu"] as FrmMenu;

            if (principal != null)
            {
                principal.MostrarGestionarVentas();
            }

            this.Close();
        }

        private void btnSeleccionarCliente_Click(object sender, EventArgs e)
        {
            if (dgvClientes.Rows.Count > 0)
            {
                ClienteDTO cliente = (ClienteDTO)dgvClientes.CurrentRow.DataBoundItem;
                var confirmacion = MessageBox.Show($"¿Está seguro que desea seleccionar al cliente '{cliente.Nombre}'?", "Confirmar selección", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirmacion == DialogResult.Yes)
                {
                    clienteActual = cliente;
                    gbSeleccionarCliente.Enabled = false;
                    lblClienteSeleccionado.Visible = true;
                    lblNombreCliente.Text = cliente.Nombre;
                    lblNombreCliente.Visible = true;
                    btnSelectOtroCliente.Visible = true;
                    gbSeleccionarLibros.Enabled = true;
                }
            }
        }

        private void btnSelectOtroCliente_Click(object sender, EventArgs e)
        {
            clienteActual = null;
            gbSeleccionarCliente.Enabled = true;
            lblClienteSeleccionado.Visible = false;
            lblNombreCliente.Visible = false;
            lblNombreCliente.Text = "";
        }
    }
}
