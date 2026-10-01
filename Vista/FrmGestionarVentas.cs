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
    public partial class FrmGestionarVentas : Form
    {
        public FrmGestionarVentas()
        {
            InitializeComponent();
            cargarVentas();
        }

        private void btnRealizarVenta_Click(object sender, EventArgs e)
        {
            FrmRealizarVenta realizarVenta = new FrmRealizarVenta();

            FrmMenu principal = (FrmMenu)this.TopLevelControl;
            principal.AbrirFormularioPanel(realizarVenta);
        }

        void cargarVentas()
        {
            dgvVentas.DataSource = null;
            dgvVentas.DataSource = ControladoraVentas.Instancia.obtenerVentasGrid();
            dgvVentas.Columns["VENDTO_ID"].Visible = false;
            dgvVentas.Columns["TotalVenta"].DefaultCellStyle.Format = "N2";
            dgvVentas.Columns["TotalVenta"].HeaderText = "Monto total de la venta";
            dgvVentas.Columns["MetodoPago"].HeaderText = "Método de pago";
        }

        private void btnVerDetalles_Click(object sender, EventArgs e)
        {
            if (dgvVentas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccioná una venta para ver sus detalles.");
                return;
            }

            if (dgvVentas.CurrentRow != null)
            {
                VentaDTO ventaGrid = (VentaDTO)dgvVentas.CurrentRow.DataBoundItem;
                FrmDetalles detalles = new FrmDetalles();
                Venta ventaSeleccionada = ControladoraVentas.Instancia.buscarVenta(ventaGrid);
                detalles.cargarDetalles(ventaSeleccionada);
                detalles.ShowDialog();
                cargarVentas();
            }
            else
            {
                MessageBox.Show("Seleccioná una venta para ver sus detalles.");
                return;
            }
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

        private void FrmGestionarVentas_Load(object sender, EventArgs e)
        {
            gbFiltrarFecha.Visible = false;
            AplicarSeguridad();
        }

        private void AplicarSeguridad()
        {
            btnVerTicket.Visible = PermisoService.Instancia.TienePermiso("VerTickets");
            btnRealizarVenta.Visible = PermisoService.Instancia.TienePermiso("RealizarVenta");
            btnVerDetalles.Visible = PermisoService.Instancia.TienePermiso("VerDetalles");
        }
        private void checkFiltrarPorFecha_CheckedChanged(object sender, EventArgs e)
        {
            if (checkFiltrarPorFecha.Checked)
            {
                gbFiltrarFecha.Visible = true;
            }
            else
            {
                gbFiltrarFecha.Visible = false;
                cargarVentas();
            }
        }

        private void btnFiltrarFecha_Click(object sender, EventArgs e)
        {
            DateTime desde = dtpDesde.Value.Date;
            DateTime hasta = dtpHasta.Value.Date.AddDays(1).AddTicks(-1);
            var resultado = ControladoraVentas.Instancia.filtrarVentasPorFecha(desde, hasta);
            dgvVentas.DataSource = resultado;

        }

        private void btnBorrarFiltrosFecha_Click(object sender, EventArgs e)
        {
            cargarVentas();
            dtpDesde.Value = new DateTime(DateTime.Now.Year, 1, 1);
            dtpHasta.Value = DateTime.Now;
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFiltrar.Text))
            {
                MessageBox.Show("Ingrese un nombre de cliente para filtrar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string filtro = txtFiltrar.Text.Trim().ToLower();
            if (filtro != null && !filtro.Equals(""))
            {
                var ventasFiltradas = ControladoraVentas.Instancia.buscarVentasPorCliente(filtro);
                dgvVentas.DataSource = ventasFiltradas;
                if (ventasFiltradas.Count == 0)
                {
                    MessageBox.Show("No se encontraron ventas para el cliente ingresado.", "Sin resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtFiltrar.Text = "";
                }
            }
            else
            {
                cargarVentas();
            }

        }

        private void txtFiltrar_TextChanged(object sender, EventArgs e)
        {
            string filtro = txtFiltrar.Text.Trim().ToLower();

            var ventasFiltradas = ControladoraVentas.Instancia.buscarVentasPorCliente(filtro);
            dgvVentas.DataSource = ventasFiltradas;
        }

        private void btnBorrarFiltros_Click(object sender, EventArgs e)
        {
            txtFiltrar.Text = "";
            cargarVentas();
        }

        private void btnVerTicket_Click(object sender, EventArgs e)
        {
            if (dgvVentas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccioná una venta para ver su ticket.");
                return;
            }

            if (dgvVentas.CurrentRow != null)
            {
                VentaDTO ventaGrid = (VentaDTO)dgvVentas.CurrentRow.DataBoundItem;
                Venta ventaSeleccionada = ControladoraVentas.Instancia.buscarVenta(ventaGrid);
                var ticket = ControladoraVentas.Instancia.GenerarTicket(ventaSeleccionada);
                FrmTickets tickets = new FrmTickets(ticket);
                tickets.ShowDialog();
                cargarVentas();
            }
            else
            {
                MessageBox.Show("Seleccioná una venta para ver su ticket.");
                return;
            }
        }
    }
}
