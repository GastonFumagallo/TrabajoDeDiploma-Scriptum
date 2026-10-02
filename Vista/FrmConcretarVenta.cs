using Controladora;
using Controladora.MetodoPagoStrategy;
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
    /// <summary>
    /// Diálogo de cobro. No persiste nada: recibe cliente y subtotal, deja elegir el método de pago,
    /// muestra el total final (con recargo/descuento de la estrategia) y devuelve la elección
    /// mediante DialogResult.OK + propiedades públicas. Quien lo abre es el que guarda la venta.
    /// </summary>
    public partial class FrmConcretarVenta : Form
    {
        public ClienteDTO? ClienteSeleccionado { get; private set; }
        public MetodoPago? MetodoPagoSeleccionado { get; private set; }

        /// <summary>Suma de precio × cantidad, antes de aplicar la estrategia del método de pago.</summary>
        public decimal Subtotal { get; }

        /// <summary>Total a cobrar con el método de pago elegido (informativo; el definitivo lo recalcula el back-end).</summary>
        public decimal TotalFinal { get; private set; }

        private readonly bool clienteFijo;

        // Requerido por el diseñador.
        public FrmConcretarVenta()
        {
            InitializeComponent();
        }

        /// <param name="cliente">Si viene informado, se omite el paso de selección de cliente.</param>
        public FrmConcretarVenta(ClienteDTO? cliente, decimal subtotal) : this()
        {
            Subtotal = subtotal;
            ClienteSeleccionado = cliente;
            clienteFijo = cliente != null;

            if (!clienteFijo)
                actualizarGrilla();
            llenarCBMetodosPago();
            cbMetodoPago.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMetodoPago.SelectedIndexChanged += (_, _) => actualizarTotal();
            AcceptButton = btnGenerarVenta;
            CancelButton = btnCancelar;
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
            int? seleccionado = cbMetodoPago.SelectedValue as int?;
            cbMetodoPago.DataSource = ControladoraMetodosPago.Instancia.obtenerMetodosPago();
            cbMetodoPago.DisplayMember = "MP_Nombre";
            cbMetodoPago.ValueMember = "MP_ID";
            if (seleccionado != null)
                cbMetodoPago.SelectedValue = seleccionado;
            actualizarTotal();
        }
        void actualizarGrilla()
        {
            dgvClientes.DataSource = null;
            dgvClientes.DataSource = ControladoraClientes.Instancia.ObtenerClientesGrid();
            dgvClientes.Columns["CLIDTO_ID"].Visible = false;
        }

        /// <summary>Recalcula el total según el método de pago elegido para que el cajero vea lo que realmente cobra.</summary>
        void actualizarTotal()
        {
            var metodo = cbMetodoPago.SelectedItem as MetodoPago;
            TotalFinal = metodo == null
                ? Subtotal
                : Math.Round(MetodoPagoStrategyFactory.Obtener(metodo).CalcularTotal(Subtotal), 2);
            lblPrecioNumero.Text = TotalFinal.ToString("N2");
        }

        private void FrmConcretarVenta_Load(object sender, EventArgs e)
        {
            actualizarTotal();

            if (clienteFijo && ClienteSeleccionado != null)
            {
                // El cliente ya se eligió en FrmRealizarVenta: ocultamos el paso de selección.
                foreach (Control c in new Control[] { dgvClientes, btnSeleccionarCliente, lblFiltrar, txtFiltrar, btnFiltrar, btnBorrarFiltros, lblPasoTres })
                    c.Visible = false;
                lblClienteSeleccionado.Visible = true;
                lblNombreSeleccionado.Visible = true;
                lblNombreSeleccionado.Text = ClienteSeleccionado.Nombre;
                conPasoCuatro();
                cbMetodoPago.Focus();
            }
            else
            {
                lblClienteSeleccionado.Visible = false;
                lblNombreSeleccionado.Visible = false;
            }
        }

        private void btnSeleccionarCliente_Click(object sender, EventArgs e)
        {
            if (dgvClientes.CurrentRow?.DataBoundItem is not ClienteDTO cliente)
                return;

            var confirmacion = MessageBox.Show($"¿Está seguro que desea seleccionar al cliente '{cliente.Nombre}'?", "Confirmar selección", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion == DialogResult.Yes)
            {
                ClienteSeleccionado = cliente;
                lblClienteSeleccionado.Visible = true;
                lblNombreSeleccionado.Visible = true;
                lblNombreSeleccionado.Text = ClienteSeleccionado.Nombre;
                conPasoCuatro();
            }
        }

        private void btnGenerarVenta_Click(object sender, EventArgs e)
        {
            if (ClienteSeleccionado == null)
            {
                MessageBox.Show("Debe seleccionar un cliente para concretar la venta.", "Cliente no seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbMetodoPago.SelectedItem is not MetodoPago metodo)
            {
                MessageBox.Show("Debe seleccionar un método de pago para concretar la venta.", "Método de pago no seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            actualizarTotal();
            string mensaje = $"Cliente: {ClienteSeleccionado.Nombre}\n\nMétodo de pago: {metodo.MP_Nombre}\n\n" +
                             $"Subtotal: ${Subtotal:N2}\nTotal a cobrar: ${TotalFinal:N2}\n\n¿Desea confirmar la venta?";
            var confirmacion = MessageBox.Show(mensaje, "Confirmar venta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                MetodoPagoSeleccionado = metodo;
                DialogResult = DialogResult.OK;
            }
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFiltrar.Text))
            {
                MessageBox.Show("Ingrese un número de documento para filtrar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtFiltrar.Text, out _))
            {
                MessageBox.Show("El DNI debe contener solo números.", "Formato Incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            dgvClientes.DataSource = ControladoraClientes.Instancia.BuscarCliente(txtFiltrar.Text.Trim());
        }

        private void btnBorrarFiltros_Click(object sender, EventArgs e)
        {
            actualizarGrilla();
            txtFiltrar.Text = string.Empty;
        }

        private void txtFiltrar_TextChanged(object sender, EventArgs e)
        {
            if (clienteFijo) return;
            dgvClientes.DataSource = ControladoraClientes.Instancia.BuscarCliente(txtFiltrar.Text.Trim());
        }

        private void btnMetodosPago_Click(object sender, EventArgs e)
        {
            using var frmMetodosPago = new FrmMetodosPago();
            frmMetodosPago.ShowDialog(this);
            // Si se dio de alta un método nuevo, que aparezca sin reabrir el diálogo.
            llenarCBMetodosPago();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
