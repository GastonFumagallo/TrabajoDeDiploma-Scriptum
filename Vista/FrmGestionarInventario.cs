using Controladora;
using Modelo;
using ScottPlot;
using ScottPlot.WinForms;
using Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static Modelo.Reportes;

namespace Vista
{
    public partial class FrmGestionarInventario : Form
    {
        private FormsPlot plotInventario;

        public FrmGestionarInventario()
        {
            InitializeComponent();
            cargarLibros();
            cargarOrdenesActivas();
            cargarOrdenesHistoricas();
            ocultarProveedores();
            InicializarGraficos();
        }
        private void cargarOrdenesHistoricas()
        {
            dgvOrdenesHistoricas.DataSource = null;
            dgvOrdenesHistoricas.DataSource = ControladoraOrdenesReposicion.Instancia.ObtenerOrdenesHistoricas();
            dgvOrdenesHistoricas.Columns["ORDTO_ID"].Visible = false;
        }
        private void cargarOrdenesActivas()
        {
            dgvOrdenes.DataSource = null;
            var ordenesActivas = ControladoraOrdenesReposicion.Instancia.ObtenerOrdenesGrid();
            if( ordenesActivas.Count == 0)
            {
                dgvOrdenes.Visible = false;
                lblOrdenesActivas.Visible = true;
                btnCancelarOrden.Visible = false;
                btnRegistrarRecepcion.Visible = false;
            }
            else
            {
                btnRegistrarRecepcion.Visible = true;
                btnCancelarOrden.Visible = true;
                dgvOrdenes.Visible = true;
                dgvOrdenes.DataSource = ordenesActivas;
                dgvOrdenes.Columns["ORDTO_ID"].Visible = false;
                lblOrdenesActivas.Visible = false;
            }
            
        }
        private void InicializarGraficos()
        {
            plotInventario = new FormsPlot() { Dock = DockStyle.Fill };
            panelInventario.Controls.Add(plotInventario);
        }
        public void cargarLibros()
        {
            dgvLibros.DataSource = null;
            dgvLibros.DataSource = ControladoraLibros.Instancia.ObtenerInventario();
            dgvLibros.Columns["LINVDTO_ID"].Visible = false;
            ActualizarIndicadores();
        }
        private void ActualizarIndicadores()
        {
            var inventario = (List<LibroInventarioDTO>)dgvLibros.DataSource;

            if (inventario == null)
                return;

            int totalLibros = inventario.Count;

            int disponibles = inventario.Count(x => x.Estado == "Disponible");

            int reposicion = inventario.Count(x =>
                x.Estado == "Stock Bajo" ||
                x.Estado == "Sin Stock");

            lblTotalLibros.Text = $"Total libros: {totalLibros}";
            lblDisponibles.Text = $"Disponibles: {disponibles}";
            lblReposicion.Text = $"⚠ Requieren reposición: {reposicion}";
        }
        private void dgvLibros_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvLibros.Rows[e.RowIndex].DataBoundItem is LibroInventarioDTO libro)
            {
                if (libro.Estado == "Sin Stock")
                    dgvLibros.Rows[e.RowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;

                else if (libro.Estado == "Stock Bajo")
                    dgvLibros.Rows[e.RowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.Khaki;
                else if (libro.Estado == "Disponible")
                {
                    dgvLibros.Rows[e.RowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(198, 239, 206);
                }
            }
        }

        private void ocultarProveedores()
        {
            dgvProveedoresLibro.Visible = false;
            lblTituloProveedores.Visible = false;
        }
        public void cargarLibrosBajoStock()
        {
            dgvLibros.DataSource = null;
            dgvLibros.DataSource = ControladoraLibros.Instancia.ObtenerBajoStock();
            dgvLibros.Columns["LINVDTO_ID"].Visible = false;
        }

        private void dgvLibros_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dgvLibros.ClearSelection();
            dgvLibros.CurrentCell = null;
        }


        private void checkReposicion_CheckedChanged(object sender, EventArgs e)
        {
            if (checkReposicion.Checked)
            {
                cargarLibrosBajoStock();
            }
            else
            {
                cargarLibros();
            }
        }

        private void btnVerProveedores_Click(object sender, EventArgs e)
        {
            if (dgvLibros.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un libro.");
                return;
            }
            LibroInventarioDTO libro = (LibroInventarioDTO)dgvLibros.CurrentRow.DataBoundItem;
            dgvProveedoresLibro.DataSource = null;
            var listaProveedores = ControladoraProveedores.Instancia.obtenerProveedoresInventario(libro.LINVDTO_ID);
            if (listaProveedores.Count == 0)
            {
                MessageBox.Show("No existen proveedores asociados.");
                return;
            }
            dgvProveedoresLibro.Visible = true;
            lblTituloProveedores.Visible = true;
            dgvProveedoresLibro.DataSource = listaProveedores;
        }
        private void DibujarGraficoInventario()
        {
            var cacheLibrosInventario = ControladoraLibros.Instancia.ObtenerInventario();
            plotInventario.Plot.Clear();

            int disponibles = cacheLibrosInventario.Count(x => x.Estado == "Disponible");
            int stockBajo = cacheLibrosInventario.Count(x => x.Estado == "Stock Bajo");
            int sinStock = cacheLibrosInventario.Count(x => x.Estado == "Sin Stock");

            List<PieSlice> porciones = new()
            {
                new PieSlice
                {
                    Value = disponibles,
                    Label = "Disponible",
                    FillColor = ScottPlot.Color.FromHex("#A8D5BA"),
                    LegendText = $"Disponible: {disponibles}"
                },
                new PieSlice
                {
                    Value = stockBajo,
                    Label = "Stock Bajo",
                    FillColor = ScottPlot.Color.FromHex("#E6D97A"),
                    LegendText = $"Stock Bajo: {stockBajo}"
                },
                new PieSlice
                {
                    Value = sinStock,
                    Label = "Sin Stock",
                    FillColor = ScottPlot.Color.FromHex("#F28C8C"),
                    LegendText = $"Sin Stock: {sinStock}"
                }
            };

            var pie = plotInventario.Plot.Add.Pie(porciones);

            pie.SliceLabelDistance = 1.3;

            plotInventario.Plot.Title("Estado del Inventario");

            // Ocultar ejes
            plotInventario.Plot.Grid.MajorLineColor = ScottPlot.Colors.Transparent;
            plotInventario.Plot.Axes.Bottom.TickGenerator =
                new ScottPlot.TickGenerators.EmptyTickGenerator();

            plotInventario.Plot.Axes.Left.TickGenerator =
                new ScottPlot.TickGenerators.EmptyTickGenerator();

            plotInventario.Refresh();
        }
        private void FrmGestionarInventario_Load(object sender, EventArgs e)
        {
            cargarLibros();
            DibujarGraficoInventario();
            AplicarSeguridad();
            cargarOrdenesActivas();
        }
        private void AplicarSeguridad()
        {
            btnVerProveedores.Visible = PermisoService.Instancia.TienePermiso("VerProveedores");
            btnOrdenReposicion.Visible = PermisoService.Instancia.TienePermiso("GenerarOrdenReposicion");
            btnRegistrarRecepcion.Visible = PermisoService.Instancia.TienePermiso("RegistrarRecepcionOrden");
            btnCancelarOrden.Visible = PermisoService.Instancia.TienePermiso("CancelarOrden");
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

        private void btnOrdenReposicion_Click(object sender, EventArgs e)
        {
            var librosBajoStock = ControladoraLibros.Instancia.ObtenerBajoStock();
            FrmOrdenReposicion frmOrdenReposicion = new FrmOrdenReposicion();
            frmOrdenReposicion.actualizarGrillaLibros(librosBajoStock);
            frmOrdenReposicion.ShowDialog();
            cargarOrdenesActivas();
        }

        private void btnRegistrarRecepcion_Click(object sender, EventArgs e)
        {
            if (dgvOrdenes.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una orden de reposición.");
                return;
            }
            OrdenReposicionDTO orden1 = (OrdenReposicionDTO)dgvOrdenes.CurrentRow.DataBoundItem;
            OrdenReposicion ordenSeleccionada = ControladoraOrdenesReposicion.Instancia.buscarOrdenIndividual(orden1);
            ordenSeleccionada.OR_Estado = "RECIBIDA";
            ordenSeleccionada.OR_Libro.LIB_Stock += ordenSeleccionada.OR_Cantidad;
            ControladoraLibros.Instancia.modificarLibroOrden(ordenSeleccionada.OR_Libro);
            ControladoraOrdenesReposicion.Instancia.ModificarOrdenReposicion(ordenSeleccionada);
            MessageBox.Show("Recepción registrada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            cargarLibros();
            cargarOrdenesActivas();
            cargarOrdenesHistoricas();
        }

        private void btnCancelarOrden_Click(object sender, EventArgs e)
        {
            if (dgvOrdenes.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una orden de reposición.");
                return;
            }
            OrdenReposicionDTO orden1 = (OrdenReposicionDTO)dgvOrdenes.CurrentRow.DataBoundItem;
            OrdenReposicion ordenSeleccionada = ControladoraOrdenesReposicion.Instancia.buscarOrdenIndividual(orden1);
            ordenSeleccionada.OR_Estado = "CANCELADA";
            ControladoraOrdenesReposicion.Instancia.ModificarOrdenReposicion(ordenSeleccionada);
            MessageBox.Show("Orden cancelada correctamente.", "Orden cancelada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            cargarOrdenesActivas();
            cargarOrdenesHistoricas();
        }

        private void dgvLibros_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvProveedoresLibro.Visible == true)
            {
                if (dgvLibros.CurrentRow == null)
                {
                    MessageBox.Show("Seleccione un libro.");
                    return;
                }
                LibroInventarioDTO libro = (LibroInventarioDTO)dgvLibros.CurrentRow.DataBoundItem;
                dgvProveedoresLibro.DataSource = null;
                var listaProveedores = ControladoraProveedores.Instancia.obtenerProveedoresInventario(libro.LINVDTO_ID);
                if (listaProveedores.Count == 0)
                {
                    dgvProveedoresLibro.Visible = false;
                    lblTituloProveedores.Visible = false;
                    MessageBox.Show("No existen proveedores asociados.");
                    return;
                }
                dgvProveedoresLibro.DataSource = listaProveedores;
                lblTituloProveedores.Visible = true;
            }
        }
    }
}
