namespace Vista
{
    partial class FrmRealizarVenta
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            panelEncabezado = new Panel();
            lblTituloPOS = new Label();
            lblFecha = new Label();
            lblComprobante = new Label();
            lblUsuario = new Label();
            lblClienteTitulo = new Label();
            cbCliente = new ComboBox();
            btnConsumidorFinal = new Button();
            btnNuevoCliente = new Button();
            btnVolver = new Button();
            panelCarga = new Panel();
            lblBuscarTitulo = new Label();
            txtBuscar = new TextBox();
            lblCantidadTitulo = new Label();
            numCantidad = new NumericUpDown();
            btnAgregar = new Button();
            lblSeleccion = new Label();
            lblEstado = new Label();
            dgvCarrito = new DataGridView();
            colLibroId = new DataGridViewTextBoxColumn();
            colProducto = new DataGridViewTextBoxColumn();
            colPrecio = new DataGridViewTextBoxColumn();
            colCantidad = new DataGridViewTextBoxColumn();
            colSubtotal = new DataGridViewTextBoxColumn();
            colMenos = new DataGridViewButtonColumn();
            colMas = new DataGridViewButtonColumn();
            colQuitar = new DataGridViewButtonColumn();
            panelCierre = new Panel();
            lblCierreTitulo = new Label();
            lblSubtotalTitulo = new Label();
            lblSubtotal = new Label();
            lblDescuentoTitulo = new Label();
            numDescuento = new NumericUpDown();
            lblDescuento = new Label();
            lblAjusteTitulo = new Label();
            lblAjuste = new Label();
            lblIVATitulo = new Label();
            lblIVA = new Label();
            lblTotalTitulo = new Label();
            lblTotal = new Label();
            lblMedioPagoTitulo = new Label();
            cbMetodoPago = new ComboBox();
            btnMediosPago = new Button();
            lblRecibidoTitulo = new Label();
            numMontoRecibido = new NumericUpDown();
            lblVueltoTitulo = new Label();
            lblVuelto = new Label();
            btnRegistrar = new Button();
            btnImprimir = new Button();
            btnCancelar = new Button();
            lblUltimaVenta = new Label();
            lblAtajos = new Label();
            lstSugerencias = new ListBox();
            toolTip = new ToolTip(components);
            panelEncabezado.SuspendLayout();
            panelCarga.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numCantidad).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvCarrito).BeginInit();
            panelCierre.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numDescuento).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMontoRecibido).BeginInit();
            SuspendLayout();
            //
            // panelEncabezado
            //
            panelEncabezado.Controls.Add(lblTituloPOS);
            panelEncabezado.Controls.Add(lblFecha);
            panelEncabezado.Controls.Add(lblComprobante);
            panelEncabezado.Controls.Add(lblUsuario);
            panelEncabezado.Controls.Add(lblClienteTitulo);
            panelEncabezado.Controls.Add(cbCliente);
            panelEncabezado.Controls.Add(btnConsumidorFinal);
            panelEncabezado.Controls.Add(btnNuevoCliente);
            panelEncabezado.Controls.Add(btnVolver);
            panelEncabezado.Dock = DockStyle.Top;
            panelEncabezado.Location = new Point(0, 0);
            panelEncabezado.Name = "panelEncabezado";
            panelEncabezado.Size = new Size(1427, 84);
            panelEncabezado.TabIndex = 0;
            //
            // lblTituloPOS
            //
            lblTituloPOS.AutoSize = true;
            lblTituloPOS.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTituloPOS.Location = new Point(12, 6);
            lblTituloPOS.Name = "lblTituloPOS";
            lblTituloPOS.Size = new Size(166, 37);
            lblTituloPOS.TabIndex = 0;
            lblTituloPOS.Tag = "BLANCO";
            lblTituloPOS.Text = "Nueva venta";
            //
            // lblFecha
            //
            lblFecha.AutoSize = true;
            lblFecha.Location = new Point(14, 50);
            lblFecha.Name = "lblFecha";
            lblFecha.Size = new Size(50, 20);
            lblFecha.TabIndex = 1;
            lblFecha.Tag = "BLANCO";
            lblFecha.Text = "Fecha:";
            //
            // lblComprobante
            //
            lblComprobante.AutoSize = true;
            lblComprobante.Location = new Point(230, 50);
            lblComprobante.Name = "lblComprobante";
            lblComprobante.Size = new Size(103, 20);
            lblComprobante.TabIndex = 2;
            lblComprobante.Tag = "BLANCO";
            lblComprobante.Text = "Comprobante:";
            //
            // lblUsuario
            //
            lblUsuario.AutoSize = true;
            lblUsuario.Location = new Point(560, 50);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(62, 20);
            lblUsuario.TabIndex = 3;
            lblUsuario.Tag = "BLANCO";
            lblUsuario.Text = "Usuario:";
            //
            // lblClienteTitulo
            //
            lblClienteTitulo.AutoSize = true;
            lblClienteTitulo.Location = new Point(760, 8);
            lblClienteTitulo.Name = "lblClienteTitulo";
            lblClienteTitulo.Size = new Size(91, 20);
            lblClienteTitulo.TabIndex = 4;
            lblClienteTitulo.Tag = "BLANCO";
            lblClienteTitulo.Text = "Cliente (F3):";
            //
            // cbCliente
            //
            cbCliente.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cbCliente.AutoCompleteSource = AutoCompleteSource.ListItems;
            cbCliente.Font = new Font("Segoe UI", 11F);
            cbCliente.Location = new Point(760, 34);
            cbCliente.Name = "cbCliente";
            cbCliente.Size = new Size(330, 33);
            cbCliente.TabIndex = 5;
            //
            // btnConsumidorFinal
            //
            btnConsumidorFinal.Location = new Point(1096, 33);
            btnConsumidorFinal.Name = "btnConsumidorFinal";
            btnConsumidorFinal.Size = new Size(140, 36);
            btnConsumidorFinal.TabIndex = 6;
            btnConsumidorFinal.Text = "Consumidor final";
            btnConsumidorFinal.UseVisualStyleBackColor = true;
            //
            // btnNuevoCliente
            //
            btnNuevoCliente.Location = new Point(1242, 33);
            btnNuevoCliente.Name = "btnNuevoCliente";
            btnNuevoCliente.Size = new Size(44, 36);
            btnNuevoCliente.TabIndex = 7;
            btnNuevoCliente.Text = "+";
            btnNuevoCliente.UseVisualStyleBackColor = true;
            //
            // btnVolver
            //
            btnVolver.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnVolver.Location = new Point(1305, 12);
            btnVolver.Name = "btnVolver";
            btnVolver.Size = new Size(110, 60);
            btnVolver.TabIndex = 8;
            btnVolver.Text = "Volver";
            btnVolver.UseVisualStyleBackColor = true;
            //
            // panelCarga
            //
            panelCarga.Controls.Add(lblBuscarTitulo);
            panelCarga.Controls.Add(txtBuscar);
            panelCarga.Controls.Add(lblCantidadTitulo);
            panelCarga.Controls.Add(numCantidad);
            panelCarga.Controls.Add(btnAgregar);
            panelCarga.Controls.Add(lblSeleccion);
            panelCarga.Controls.Add(lblEstado);
            panelCarga.Dock = DockStyle.Top;
            panelCarga.Location = new Point(0, 84);
            panelCarga.Name = "panelCarga";
            panelCarga.Size = new Size(1047, 104);
            panelCarga.TabIndex = 1;
            //
            // lblBuscarTitulo
            //
            lblBuscarTitulo.AutoSize = true;
            lblBuscarTitulo.Location = new Point(12, 6);
            lblBuscarTitulo.Name = "lblBuscarTitulo";
            lblBuscarTitulo.Size = new Size(428, 20);
            lblBuscarTitulo.TabIndex = 0;
            lblBuscarTitulo.Tag = "BLANCO";
            lblBuscarTitulo.Text = "Buscar libro (F2): código de barras / ISBN, título, autor o editorial";
            //
            // txtBuscar
            //
            txtBuscar.Font = new Font("Segoe UI", 12F);
            txtBuscar.Location = new Point(12, 30);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(560, 34);
            txtBuscar.TabIndex = 0;
            //
            // lblCantidadTitulo
            //
            lblCantidadTitulo.AutoSize = true;
            lblCantidadTitulo.Location = new Point(584, 6);
            lblCantidadTitulo.Name = "lblCantidadTitulo";
            lblCantidadTitulo.Size = new Size(69, 20);
            lblCantidadTitulo.TabIndex = 2;
            lblCantidadTitulo.Tag = "BLANCO";
            lblCantidadTitulo.Text = "Cantidad";
            //
            // numCantidad
            //
            numCantidad.Font = new Font("Segoe UI", 12F);
            numCantidad.Location = new Point(584, 30);
            numCantidad.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numCantidad.Name = "numCantidad";
            numCantidad.Size = new Size(90, 34);
            numCantidad.TabIndex = 1;
            numCantidad.TextAlign = HorizontalAlignment.Right;
            numCantidad.Value = new decimal(new int[] { 1, 0, 0, 0 });
            //
            // btnAgregar
            //
            btnAgregar.Location = new Point(684, 28);
            btnAgregar.Name = "btnAgregar";
            btnAgregar.Size = new Size(140, 38);
            btnAgregar.TabIndex = 2;
            btnAgregar.Text = "Agregar (Enter)";
            btnAgregar.UseVisualStyleBackColor = true;
            //
            // lblSeleccion
            //
            lblSeleccion.AutoEllipsis = true;
            lblSeleccion.Location = new Point(12, 72);
            lblSeleccion.Name = "lblSeleccion";
            lblSeleccion.Size = new Size(560, 24);
            lblSeleccion.TabIndex = 5;
            lblSeleccion.Tag = "BLANCO";
            //
            // lblEstado
            //
            lblEstado.AutoEllipsis = true;
            lblEstado.Location = new Point(584, 72);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(450, 24);
            lblEstado.TabIndex = 6;
            lblEstado.Tag = "BLANCO";
            //
            // dgvCarrito
            //
            dgvCarrito.AllowUserToAddRows = false;
            dgvCarrito.AllowUserToDeleteRows = false;
            dgvCarrito.AllowUserToResizeRows = false;
            dgvCarrito.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCarrito.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCarrito.Columns.AddRange(new DataGridViewColumn[] { colLibroId, colProducto, colPrecio, colCantidad, colSubtotal, colMenos, colMas, colQuitar });
            dgvCarrito.Dock = DockStyle.Fill;
            dgvCarrito.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            dgvCarrito.Location = new Point(0, 188);
            dgvCarrito.MultiSelect = false;
            dgvCarrito.Name = "dgvCarrito";
            dgvCarrito.RowHeadersVisible = false;
            dgvCarrito.RowHeadersWidth = 51;
            dgvCarrito.RowTemplate.Height = 32;
            dgvCarrito.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCarrito.Size = new Size(1047, 537);
            dgvCarrito.TabIndex = 3;
            //
            // colLibroId
            //
            colLibroId.DataPropertyName = "LibroId";
            colLibroId.FillWeight = 40F;
            colLibroId.HeaderText = "ID";
            colLibroId.MinimumWidth = 6;
            colLibroId.Name = "colLibroId";
            colLibroId.ReadOnly = true;
            //
            // colProducto
            //
            colProducto.DataPropertyName = "Producto";
            colProducto.FillWeight = 260F;
            colProducto.HeaderText = "Producto";
            colProducto.MinimumWidth = 6;
            colProducto.Name = "colProducto";
            colProducto.ReadOnly = true;
            //
            // colPrecio
            //
            colPrecio.DataPropertyName = "PrecioUnitario";
            colPrecio.FillWeight = 90F;
            colPrecio.HeaderText = "Precio unit.";
            colPrecio.MinimumWidth = 6;
            colPrecio.Name = "colPrecio";
            colPrecio.ReadOnly = true;
            //
            // colCantidad
            //
            colCantidad.DataPropertyName = "Cantidad";
            colCantidad.FillWeight = 70F;
            colCantidad.HeaderText = "Cantidad";
            colCantidad.MinimumWidth = 6;
            colCantidad.Name = "colCantidad";
            //
            // colSubtotal
            //
            colSubtotal.DataPropertyName = "Subtotal";
            colSubtotal.FillWeight = 100F;
            colSubtotal.HeaderText = "Subtotal";
            colSubtotal.MinimumWidth = 6;
            colSubtotal.Name = "colSubtotal";
            colSubtotal.ReadOnly = true;
            //
            // colMenos
            //
            colMenos.FillWeight = 30F;
            colMenos.HeaderText = "";
            colMenos.MinimumWidth = 6;
            colMenos.Name = "colMenos";
            colMenos.Text = "−";
            colMenos.ToolTipText = "Restar una unidad (−)";
            colMenos.UseColumnTextForButtonValue = true;
            //
            // colMas
            //
            colMas.FillWeight = 30F;
            colMas.HeaderText = "";
            colMas.MinimumWidth = 6;
            colMas.Name = "colMas";
            colMas.Text = "+";
            colMas.ToolTipText = "Sumar una unidad (+)";
            colMas.UseColumnTextForButtonValue = true;
            //
            // colQuitar
            //
            colQuitar.FillWeight = 40F;
            colQuitar.HeaderText = "";
            colQuitar.MinimumWidth = 6;
            colQuitar.Name = "colQuitar";
            colQuitar.Text = "Quitar";
            colQuitar.ToolTipText = "Quitar la línea (Supr)";
            colQuitar.UseColumnTextForButtonValue = true;
            //
            // panelCierre
            //
            panelCierre.Controls.Add(lblCierreTitulo);
            panelCierre.Controls.Add(lblSubtotalTitulo);
            panelCierre.Controls.Add(lblSubtotal);
            panelCierre.Controls.Add(lblDescuentoTitulo);
            panelCierre.Controls.Add(numDescuento);
            panelCierre.Controls.Add(lblDescuento);
            panelCierre.Controls.Add(lblAjusteTitulo);
            panelCierre.Controls.Add(lblAjuste);
            panelCierre.Controls.Add(lblIVATitulo);
            panelCierre.Controls.Add(lblIVA);
            panelCierre.Controls.Add(lblTotalTitulo);
            panelCierre.Controls.Add(lblTotal);
            panelCierre.Controls.Add(lblMedioPagoTitulo);
            panelCierre.Controls.Add(cbMetodoPago);
            panelCierre.Controls.Add(btnMediosPago);
            panelCierre.Controls.Add(lblRecibidoTitulo);
            panelCierre.Controls.Add(numMontoRecibido);
            panelCierre.Controls.Add(lblVueltoTitulo);
            panelCierre.Controls.Add(lblVuelto);
            panelCierre.Controls.Add(btnRegistrar);
            panelCierre.Controls.Add(btnImprimir);
            panelCierre.Controls.Add(btnCancelar);
            panelCierre.Controls.Add(lblUltimaVenta);
            panelCierre.Dock = DockStyle.Right;
            panelCierre.Location = new Point(1047, 84);
            panelCierre.Name = "panelCierre";
            panelCierre.Size = new Size(380, 641);
            panelCierre.TabIndex = 2;
            //
            // lblCierreTitulo
            //
            lblCierreTitulo.AutoSize = true;
            lblCierreTitulo.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblCierreTitulo.Location = new Point(12, 8);
            lblCierreTitulo.Name = "lblCierreTitulo";
            lblCierreTitulo.Size = new Size(169, 30);
            lblCierreTitulo.TabIndex = 0;
            lblCierreTitulo.Tag = "BLANCO";
            lblCierreTitulo.Text = "Cierre de venta";
            //
            // lblSubtotalTitulo
            //
            lblSubtotalTitulo.AutoSize = true;
            lblSubtotalTitulo.Location = new Point(12, 52);
            lblSubtotalTitulo.Name = "lblSubtotalTitulo";
            lblSubtotalTitulo.Size = new Size(65, 20);
            lblSubtotalTitulo.TabIndex = 1;
            lblSubtotalTitulo.Tag = "BLANCO";
            lblSubtotalTitulo.Text = "Subtotal";
            //
            // lblSubtotal
            //
            lblSubtotal.Location = new Point(198, 48);
            lblSubtotal.Name = "lblSubtotal";
            lblSubtotal.Size = new Size(170, 28);
            lblSubtotal.TabIndex = 2;
            lblSubtotal.Tag = "BLANCO";
            lblSubtotal.Text = "$0,00";
            lblSubtotal.TextAlign = ContentAlignment.MiddleRight;
            //
            // lblDescuentoTitulo
            //
            lblDescuentoTitulo.AutoSize = true;
            lblDescuentoTitulo.Location = new Point(12, 88);
            lblDescuentoTitulo.Name = "lblDescuentoTitulo";
            lblDescuentoTitulo.Size = new Size(96, 20);
            lblDescuentoTitulo.TabIndex = 3;
            lblDescuentoTitulo.Tag = "BLANCO";
            lblDescuentoTitulo.Text = "Descuento %";
            //
            // numDescuento
            //
            numDescuento.DecimalPlaces = 2;
            numDescuento.Location = new Point(118, 85);
            numDescuento.Name = "numDescuento";
            numDescuento.Size = new Size(80, 27);
            numDescuento.TabIndex = 10;
            numDescuento.TextAlign = HorizontalAlignment.Right;
            //
            // lblDescuento
            //
            lblDescuento.Location = new Point(198, 84);
            lblDescuento.Name = "lblDescuento";
            lblDescuento.Size = new Size(170, 28);
            lblDescuento.TabIndex = 5;
            lblDescuento.Tag = "BLANCO";
            lblDescuento.Text = "-$0,00";
            lblDescuento.TextAlign = ContentAlignment.MiddleRight;
            //
            // lblAjusteTitulo
            //
            lblAjusteTitulo.AutoSize = true;
            lblAjusteTitulo.Location = new Point(12, 124);
            lblAjusteTitulo.Name = "lblAjusteTitulo";
            lblAjusteTitulo.Size = new Size(176, 20);
            lblAjusteTitulo.TabIndex = 6;
            lblAjusteTitulo.Tag = "BLANCO";
            lblAjusteTitulo.Text = "Recargo/desc. medio pago";
            //
            // lblAjuste
            //
            lblAjuste.Location = new Point(198, 120);
            lblAjuste.Name = "lblAjuste";
            lblAjuste.Size = new Size(170, 28);
            lblAjuste.TabIndex = 7;
            lblAjuste.Tag = "BLANCO";
            lblAjuste.Text = "$0,00";
            lblAjuste.TextAlign = ContentAlignment.MiddleRight;
            //
            // lblIVATitulo
            //
            lblIVATitulo.AutoSize = true;
            lblIVATitulo.Location = new Point(12, 160);
            lblIVATitulo.Name = "lblIVATitulo";
            lblIVATitulo.Size = new Size(86, 20);
            lblIVATitulo.TabIndex = 8;
            lblIVATitulo.Tag = "BLANCO";
            lblIVATitulo.Text = "IVA incluido";
            //
            // lblIVA
            //
            lblIVA.Location = new Point(198, 156);
            lblIVA.Name = "lblIVA";
            lblIVA.Size = new Size(170, 28);
            lblIVA.TabIndex = 9;
            lblIVA.Tag = "BLANCO";
            lblIVA.Text = "$0,00";
            lblIVA.TextAlign = ContentAlignment.MiddleRight;
            //
            // lblTotalTitulo
            //
            lblTotalTitulo.AutoSize = true;
            lblTotalTitulo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTotalTitulo.Location = new Point(12, 204);
            lblTotalTitulo.Name = "lblTotalTitulo";
            lblTotalTitulo.Size = new Size(88, 37);
            lblTotalTitulo.TabIndex = 10;
            lblTotalTitulo.Tag = "BLANCO";
            lblTotalTitulo.Text = "TOTAL";
            //
            // lblTotal
            //
            lblTotal.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblTotal.Location = new Point(118, 196);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(250, 50);
            lblTotal.TabIndex = 11;
            lblTotal.Tag = "BLANCO";
            lblTotal.Text = "$0,00";
            lblTotal.TextAlign = ContentAlignment.MiddleRight;
            //
            // lblMedioPagoTitulo
            //
            lblMedioPagoTitulo.AutoSize = true;
            lblMedioPagoTitulo.Location = new Point(12, 262);
            lblMedioPagoTitulo.Name = "lblMedioPagoTitulo";
            lblMedioPagoTitulo.Size = new Size(142, 20);
            lblMedioPagoTitulo.TabIndex = 12;
            lblMedioPagoTitulo.Tag = "BLANCO";
            lblMedioPagoTitulo.Text = "Medio de pago (F4)";
            //
            // cbMetodoPago
            //
            cbMetodoPago.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMetodoPago.Font = new Font("Segoe UI", 11F);
            cbMetodoPago.Location = new Point(12, 286);
            cbMetodoPago.Name = "cbMetodoPago";
            cbMetodoPago.Size = new Size(306, 33);
            cbMetodoPago.TabIndex = 11;
            //
            // btnMediosPago
            //
            btnMediosPago.Location = new Point(324, 285);
            btnMediosPago.Name = "btnMediosPago";
            btnMediosPago.Size = new Size(44, 35);
            btnMediosPago.TabIndex = 12;
            btnMediosPago.Text = "…";
            btnMediosPago.UseVisualStyleBackColor = true;
            //
            // lblRecibidoTitulo
            //
            lblRecibidoTitulo.AutoSize = true;
            lblRecibidoTitulo.Location = new Point(12, 334);
            lblRecibidoTitulo.Name = "lblRecibidoTitulo";
            lblRecibidoTitulo.Size = new Size(149, 20);
            lblRecibidoTitulo.TabIndex = 14;
            lblRecibidoTitulo.Tag = "BLANCO";
            lblRecibidoTitulo.Text = "Monto recibido (F6)";
            //
            // numMontoRecibido
            //
            numMontoRecibido.DecimalPlaces = 2;
            numMontoRecibido.Font = new Font("Segoe UI", 14F);
            numMontoRecibido.Location = new Point(12, 358);
            numMontoRecibido.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            numMontoRecibido.Name = "numMontoRecibido";
            numMontoRecibido.Size = new Size(356, 39);
            numMontoRecibido.TabIndex = 13;
            numMontoRecibido.TextAlign = HorizontalAlignment.Right;
            numMontoRecibido.ThousandsSeparator = true;
            //
            // lblVueltoTitulo
            //
            lblVueltoTitulo.AutoSize = true;
            lblVueltoTitulo.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblVueltoTitulo.Location = new Point(12, 418);
            lblVueltoTitulo.Name = "lblVueltoTitulo";
            lblVueltoTitulo.Size = new Size(79, 30);
            lblVueltoTitulo.TabIndex = 16;
            lblVueltoTitulo.Tag = "BLANCO";
            lblVueltoTitulo.Text = "Vuelto";
            //
            // lblVuelto
            //
            lblVuelto.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblVuelto.Location = new Point(118, 410);
            lblVuelto.Name = "lblVuelto";
            lblVuelto.Size = new Size(250, 44);
            lblVuelto.TabIndex = 17;
            lblVuelto.Tag = "BLANCO";
            lblVuelto.Text = "$0,00";
            lblVuelto.TextAlign = ContentAlignment.MiddleRight;
            //
            // btnRegistrar
            //
            btnRegistrar.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnRegistrar.Location = new Point(12, 468);
            btnRegistrar.Name = "btnRegistrar";
            btnRegistrar.Size = new Size(356, 58);
            btnRegistrar.TabIndex = 14;
            btnRegistrar.Text = "Registrar venta (F5)";
            btnRegistrar.UseVisualStyleBackColor = true;
            //
            // btnImprimir
            //
            btnImprimir.Location = new Point(12, 534);
            btnImprimir.Name = "btnImprimir";
            btnImprimir.Size = new Size(175, 46);
            btnImprimir.TabIndex = 15;
            btnImprimir.Text = "Imprimir ticket (F8)";
            btnImprimir.UseVisualStyleBackColor = true;
            //
            // btnCancelar
            //
            btnCancelar.Location = new Point(193, 534);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(175, 46);
            btnCancelar.TabIndex = 16;
            btnCancelar.Text = "Cancelar / Limpiar (Esc)";
            btnCancelar.UseVisualStyleBackColor = true;
            //
            // lblUltimaVenta
            //
            lblUltimaVenta.Location = new Point(12, 588);
            lblUltimaVenta.Name = "lblUltimaVenta";
            lblUltimaVenta.Size = new Size(356, 48);
            lblUltimaVenta.TabIndex = 21;
            lblUltimaVenta.Tag = "BLANCO";
            //
            // lblAtajos
            //
            lblAtajos.Dock = DockStyle.Bottom;
            lblAtajos.Location = new Point(0, 725);
            lblAtajos.Name = "lblAtajos";
            lblAtajos.Padding = new Padding(8, 0, 0, 0);
            lblAtajos.Size = new Size(1427, 28);
            lblAtajos.TabIndex = 4;
            lblAtajos.Text = "F2 Buscar · F3 Cliente · F4 Medio de pago · F6 Monto recibido · F5 Registrar · F8 Ticket · Supr Quitar · +/− Cantidad · Esc Cancelar";
            lblAtajos.TextAlign = ContentAlignment.MiddleLeft;
            //
            // lstSugerencias
            //
            lstSugerencias.Font = new Font("Segoe UI", 11F);
            lstSugerencias.IntegralHeight = false;
            lstSugerencias.Location = new Point(12, 152);
            lstSugerencias.Name = "lstSugerencias";
            lstSugerencias.Size = new Size(760, 240);
            lstSugerencias.TabIndex = 9;
            lstSugerencias.TabStop = false;
            lstSugerencias.Visible = false;
            //
            // FrmRealizarVenta
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1427, 753);
            ControlBox = false;
            Controls.Add(lstSugerencias);
            Controls.Add(dgvCarrito);
            Controls.Add(panelCarga);
            Controls.Add(panelCierre);
            Controls.Add(lblAtajos);
            Controls.Add(panelEncabezado);
            Name = "FrmRealizarVenta";
            Text = "Realizar venta";
            Load += FrmRealizarVenta_Load;
            panelEncabezado.ResumeLayout(false);
            panelEncabezado.PerformLayout();
            panelCarga.ResumeLayout(false);
            panelCarga.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numCantidad).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvCarrito).EndInit();
            panelCierre.ResumeLayout(false);
            panelCierre.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numDescuento).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMontoRecibido).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelEncabezado;
        private Label lblTituloPOS;
        private Label lblFecha;
        private Label lblComprobante;
        private Label lblUsuario;
        private Label lblClienteTitulo;
        private ComboBox cbCliente;
        private Button btnConsumidorFinal;
        private Button btnNuevoCliente;
        private Button btnVolver;
        private Panel panelCarga;
        private Label lblBuscarTitulo;
        private TextBox txtBuscar;
        private Label lblCantidadTitulo;
        private NumericUpDown numCantidad;
        private Button btnAgregar;
        private Label lblSeleccion;
        private Label lblEstado;
        private DataGridView dgvCarrito;
        private DataGridViewTextBoxColumn colLibroId;
        private DataGridViewTextBoxColumn colProducto;
        private DataGridViewTextBoxColumn colPrecio;
        private DataGridViewTextBoxColumn colCantidad;
        private DataGridViewTextBoxColumn colSubtotal;
        private DataGridViewButtonColumn colMenos;
        private DataGridViewButtonColumn colMas;
        private DataGridViewButtonColumn colQuitar;
        private Panel panelCierre;
        private Label lblCierreTitulo;
        private Label lblSubtotalTitulo;
        private Label lblSubtotal;
        private Label lblDescuentoTitulo;
        private NumericUpDown numDescuento;
        private Label lblDescuento;
        private Label lblAjusteTitulo;
        private Label lblAjuste;
        private Label lblIVATitulo;
        private Label lblIVA;
        private Label lblTotalTitulo;
        private Label lblTotal;
        private Label lblMedioPagoTitulo;
        private ComboBox cbMetodoPago;
        private Button btnMediosPago;
        private Label lblRecibidoTitulo;
        private NumericUpDown numMontoRecibido;
        private Label lblVueltoTitulo;
        private Label lblVuelto;
        private Button btnRegistrar;
        private Button btnImprimir;
        private Button btnCancelar;
        private Label lblUltimaVenta;
        private Label lblAtajos;
        private ListBox lstSugerencias;
        private ToolTip toolTip;
    }
}
