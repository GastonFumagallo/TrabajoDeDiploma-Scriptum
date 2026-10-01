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
    public partial class FrmConcretarVenta : Form
    {
        public ClienteDTO ClienteSeleccionado { get; private set; }
        public MetodoPago MetodoPagoSeleccionado { get; private set; }
        public string PrecioTotal;
        public FrmConcretarVenta()
        {
            InitializeComponent();
            actualizarGrilla();
            llenarCBMetodosPago();
            sinPasoCuatro();
        }
        
        void conPasoCuatro()
        {
            lblPasoCuatro.Visible = true;
            lblMP.Visible = true;
            btnGenerarVenta.Visible = true;
            btnMetodosPago.Visible = true;
        }
        void sinPasoCuatro()
        {
            lblPasoCuatro.Visible = false;
            lblMP.Visible = false;
            btnGenerarVenta.Visible = false;
            btnMetodosPago.Visible = false;
        }
        void llenarCBMetodosPago()
        {
            cbMetodoPago.DataSource = ControladoraMetodosPago.Instancia.obtenerMetodosPago();
            cbMetodoPago.DisplayMember = "MP_Nombre";
            cbMetodoPago.ValueMember = "MP_ID";
        }
        void actualizarGrilla()
        {
            dgvClientes.DataSource = null;
            dgvClientes.DataSource = ControladoraClientes.Instancia.ObtenerClientesGrid();
            dgvClientes.Columns["CLIDTO_ID"].Visible = false;
        }

        private void btnSeleccionarCliente_Click(object sender, EventArgs e)
        {
            if (dgvClientes.Rows.Count > 0)
            {
                ClienteDTO cliente = (ClienteDTO)dgvClientes.CurrentRow.DataBoundItem;
                var confirmacion = MessageBox.Show($"¿Está seguro que desea seleccionar al cliente '{cliente.Nombre}'?", "Confirmar selección", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirmacion == DialogResult.Yes)
                {
                    ClienteSeleccionado = cliente;
                    lblClienteSeleccionado.Visible = true;
                    lblNombreSeleccionado.Visible = true;
                    lblNombreSeleccionado.Text = ClienteSeleccionado.Nombre;
                }
            }
            conPasoCuatro();
        }

        private void FrmConcretarVenta_Load(object sender, EventArgs e)
        {
            lblPrecioNumero.Text = PrecioTotal;
            lblClienteSeleccionado.Visible = false;
            lblNombreSeleccionado.Visible = false;
        }

        private void btnGenerarVenta_Click(object sender, EventArgs e)
        {
            if (ClienteSeleccionado == null)
            {
                MessageBox.Show("Debe seleccionar un cliente para concretar la venta.", "Cliente no seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbMetodoPago.SelectedValue == null)
            {
                MessageBox.Show("Debe seleccionar un método de pago para concretar la venta.", "Método de pago no seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            decimal total = Convert.ToDecimal(lblPrecioNumero.Text);
            string mensaje = "Cliente: " + ClienteSeleccionado.Nombre + "\n\nMétodo de pago: " + cbMetodoPago.Text + "\n\n Total: $" + total.ToString("N2") + "\n\n¿Desea confirmar la venta?";
            var confirmacion = MessageBox.Show(mensaje, "Confirmar venta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                MetodoPagoSeleccionado = ControladoraMetodosPago.Instancia.obtenerMetodoPagoPorID((int)cbMetodoPago.SelectedValue);
                this.DialogResult = DialogResult.OK;
                this.Close();
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
                var clientesFiltrados = ControladoraClientes.Instancia.BuscarCliente(filtro);
                dgvClientes.DataSource = clientesFiltrados;
            }
            else
            {
                actualizarGrilla();
            }

        }

        private void btnBorrarFiltros_Click(object sender, EventArgs e)
        {
            actualizarGrilla();
            txtFiltrar.Text = string.Empty;
        }

        private void txtFiltrar_TextChanged(object sender, EventArgs e)
        {
            string filtro = txtFiltrar.Text.Trim().ToLower();

            var clientesFiltrados = ControladoraClientes.Instancia.BuscarCliente(filtro);
            dgvClientes.DataSource = clientesFiltrados;
        }

        private void btnMetodosPago_Click(object sender, EventArgs e)
        {
            FrmMetodosPago frmMetodosPago = new FrmMetodosPago();
            frmMetodosPago.ShowDialog();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
