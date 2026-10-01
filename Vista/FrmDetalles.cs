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
    public partial class FrmDetalles : Form
    {
        public FrmDetalles()
        {
            InitializeComponent();
        }

        public void cargarDetalles(Venta ventaSeleccionada)
        {
            var detalleFacturas = ControladoraDetallesVenta.Instancia.BuscarDetalles(ventaSeleccionada.VEN_ID);
            dgvDetalles.DataSource = null;
            dgvDetalles.DataSource = detalleFacturas;
            dgvDetalles.Columns["DVDTO_ID"].Visible = false;
            dgvDetalles.Columns["Subtotal"].DefaultCellStyle.Format = "N2";
            dgvDetalles.Columns["PrecioUnitario"].DefaultCellStyle.Format = "N2";
        }
    }


}