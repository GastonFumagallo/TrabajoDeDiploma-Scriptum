namespace Vista
{
    partial class FrmOrdenReposicion
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
            panelCabecera = new Panel();
            lblNumero = new Label();
            lblEstadoOrden = new Label();
            lblFecha = new Label();
            lblUsuario = new Label();
            lblProveedorTitulo = new Label();
            cbProveedor = new ComboBox();
            lblContacto = new Label();
            panelCarga = new Panel();
            lblBuscarTitulo = new Label();
            txtBuscar = new TextBox();
            lblCantidadTitulo = new Label();
            numCantidad = new NumericUpDown();
            lblCostoTitulo = new Label();
            numCosto = new NumericUpDown();
            btnAgregar = new Button();
            chkSoloProveedor = new CheckBox();
            lblSeleccion = new Label();
            dgvItems = new DataGridView();
            colLibroId = new DataGridViewTextBoxColumn();
            colCodigo = new DataGridViewTextBoxColumn();
            colTitulo = new DataGridViewTextBoxColumn();
            colStock = new DataGridViewTextBoxColumn();
            colOptimo = new DataGridViewTextBoxColumn();
            colCantidad = new DataGridViewTextBoxColumn();
            colCosto = new DataGridViewTextBoxColumn();
            colSubtotal = new DataGridViewTextBoxColumn();
            colRecibido = new DataGridViewTextBoxColumn();
            colCostoRecibido = new DataGridViewTextBoxColumn();
            colQuitar = new DataGridViewButtonColumn();
            panelPie = new Panel();
            lblObservacionesTitulo = new Label();
            txtObservaciones = new TextBox();
            lblInfoEstado = new Label();
            chkActualizarCostos = new CheckBox();
            lblTotalTitulo = new Label();
            lblTotal = new Label();
            btnGuardar = new Button();
            btnEmitir = new Button();
            btnRecibir = new Button();
            btnCerrar = new Button();
            lstSugerencias = new ListBox();
            panelCabecera.SuspendLayout();
            panelCarga.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numCantidad).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCosto).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvItems).BeginInit();
            panelPie.SuspendLayout();
            SuspendLayout();
            //
            // panelCabecera
            //
            panelCabecera.Controls.Add(lblNumero);
            panelCabecera.Controls.Add(lblEstadoOrden);
            panelCabecera.Controls.Add(lblFecha);
            panelCabecera.Controls.Add(lblUsuario);
            panelCabecera.Controls.Add(lblProveedorTitulo);
            panelCabecera.Controls.Add(cbProveedor);
            panelCabecera.Controls.Add(lblContacto);
            panelCabecera.Dock = DockStyle.Top;
            panelCabecera.Location = new Point(0, 0);
            panelCabecera.Name = "panelCabecera";
            panelCabecera.Size = new Size(1200, 116);
            panelCabecera.TabIndex = 0;
            //
            // lblNumero
            //
            lblNumero.AutoSize = true;
            lblNumero.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblNumero.Location = new Point(12, 8);
            lblNumero.Name = "lblNumero";
            lblNumero.Size = new Size(335, 37);
            lblNumero.TabIndex = 0;
            lblNumero.Tag = "BLANCO";
            lblNumero.Text = "Nueva orden de reposición";
            //
            // lblEstadoOrden
            //
            lblEstadoOrden.AutoSize = true;
            lblEstadoOrden.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblEstadoOrden.Location = new Point(14, 52);
            lblEstadoOrden.Name = "lblEstadoOrden";
            lblEstadoOrden.Size = new Size(64, 23);
            lblEstadoOrden.TabIndex = 1;
            lblEstadoOrden.Tag = "BLANCO";
            lblEstadoOrden.Text = "Estado:";
            //
            // lblFecha
            //
            lblFecha.AutoSize = true;
            lblFecha.Location = new Point(14, 84);
            lblFecha.Name = "lblFecha";
            lblFecha.Size = new Size(50, 20);
            lblFecha.TabIndex = 2;
            lblFecha.Tag = "BLANCO";
            lblFecha.Text = "Fecha:";
            //
            // lblUsuario
            //
            lblUsuario.AutoSize = true;
            lblUsuario.Location = new Point(250, 84);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(85, 20);
            lblUsuario.TabIndex = 3;
            lblUsuario.Tag = "BLANCO";
            lblUsuario.Text = "Solicitante:";
            //
            // lblProveedorTitulo
            //
            lblProveedorTitulo.AutoSize = true;
            lblProveedorTitulo.Location = new Point(560, 8);
            lblProveedorTitulo.Name = "lblProveedorTitulo";
            lblProveedorTitulo.Size = new Size(77, 20);
            lblProveedorTitulo.TabIndex = 4;
            lblProveedorTitulo.Tag = "BLANCO";
            lblProveedorTitulo.Text = "Proveedor";
            //
            // cbProveedor
            //
            cbProveedor.DropDownStyle = ComboBoxStyle.DropDownList;
            cbProveedor.Font = new Font("Segoe UI", 11F);
            cbProveedor.Location = new Point(560, 32);
            cbProveedor.Name = "cbProveedor";
            cbProveedor.Size = new Size(380, 33);
            cbProveedor.TabIndex = 0;
            //
            // lblContacto
            //
            lblContacto.AutoEllipsis = true;
            lblContacto.Location = new Point(560, 72);
            lblContacto.Name = "lblContacto";
            lblContacto.Size = new Size(628, 40);
            lblContacto.TabIndex = 6;
            lblContacto.Tag = "BLANCO";
            //
            // panelCarga
            //
            panelCarga.Controls.Add(lblBuscarTitulo);
            panelCarga.Controls.Add(txtBuscar);
            panelCarga.Controls.Add(lblCantidadTitulo);
            panelCarga.Controls.Add(numCantidad);
            panelCarga.Controls.Add(lblCostoTitulo);
            panelCarga.Controls.Add(numCosto);
            panelCarga.Controls.Add(btnAgregar);
            panelCarga.Controls.Add(chkSoloProveedor);
            panelCarga.Controls.Add(lblSeleccion);
            panelCarga.Dock = DockStyle.Top;
            panelCarga.Location = new Point(0, 116);
            panelCarga.Name = "panelCarga";
            panelCarga.Size = new Size(1200, 96);
            panelCarga.TabIndex = 1;
            //
            // lblBuscarTitulo
            //
            lblBuscarTitulo.AutoSize = true;
            lblBuscarTitulo.Location = new Point(12, 6);
            lblBuscarTitulo.Name = "lblBuscarTitulo";
            lblBuscarTitulo.Size = new Size(312, 20);
            lblBuscarTitulo.TabIndex = 0;
            lblBuscarTitulo.Tag = "BLANCO";
            lblBuscarTitulo.Text = "Agregar libro: ISBN, título, autor o editorial";
            //
            // txtBuscar
            //
            txtBuscar.Font = new Font("Segoe UI", 11F);
            txtBuscar.Location = new Point(12, 30);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(480, 32);
            txtBuscar.TabIndex = 0;
            //
            // lblCantidadTitulo
            //
            lblCantidadTitulo.AutoSize = true;
            lblCantidadTitulo.Location = new Point(504, 6);
            lblCantidadTitulo.Name = "lblCantidadTitulo";
            lblCantidadTitulo.Size = new Size(69, 20);
            lblCantidadTitulo.TabIndex = 2;
            lblCantidadTitulo.Tag = "BLANCO";
            lblCantidadTitulo.Text = "Cantidad";
            //
            // numCantidad
            //
            numCantidad.Font = new Font("Segoe UI", 11F);
            numCantidad.Location = new Point(504, 30);
            numCantidad.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numCantidad.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numCantidad.Name = "numCantidad";
            numCantidad.Size = new Size(90, 32);
            numCantidad.TabIndex = 1;
            numCantidad.TextAlign = HorizontalAlignment.Right;
            numCantidad.Value = new decimal(new int[] { 1, 0, 0, 0 });
            //
            // lblCostoTitulo
            //
            lblCostoTitulo.AutoSize = true;
            lblCostoTitulo.Location = new Point(604, 6);
            lblCostoTitulo.Name = "lblCostoTitulo";
            lblCostoTitulo.Size = new Size(84, 20);
            lblCostoTitulo.TabIndex = 4;
            lblCostoTitulo.Tag = "BLANCO";
            lblCostoTitulo.Text = "Costo unit.";
            //
            // numCosto
            //
            numCosto.DecimalPlaces = 2;
            numCosto.Font = new Font("Segoe UI", 11F);
            numCosto.Location = new Point(604, 30);
            numCosto.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            numCosto.Name = "numCosto";
            numCosto.Size = new Size(130, 32);
            numCosto.TabIndex = 2;
            numCosto.TextAlign = HorizontalAlignment.Right;
            numCosto.ThousandsSeparator = true;
            //
            // btnAgregar
            //
            btnAgregar.Location = new Point(744, 27);
            btnAgregar.Name = "btnAgregar";
            btnAgregar.Size = new Size(140, 38);
            btnAgregar.TabIndex = 3;
            btnAgregar.Text = "Agregar (Enter)";
            btnAgregar.UseVisualStyleBackColor = true;
            //
            // chkSoloProveedor
            //
            chkSoloProveedor.AutoSize = true;
            chkSoloProveedor.Location = new Point(898, 34);
            chkSoloProveedor.Name = "chkSoloProveedor";
            chkSoloProveedor.Size = new Size(234, 24);
            chkSoloProveedor.TabIndex = 4;
            chkSoloProveedor.Text = "Sólo libros de este proveedor";
            chkSoloProveedor.UseVisualStyleBackColor = true;
            //
            // lblSeleccion
            //
            lblSeleccion.AutoEllipsis = true;
            lblSeleccion.Location = new Point(12, 68);
            lblSeleccion.Name = "lblSeleccion";
            lblSeleccion.Size = new Size(1170, 24);
            lblSeleccion.TabIndex = 8;
            lblSeleccion.Tag = "BLANCO";
            //
            // dgvItems
            //
            dgvItems.AllowUserToAddRows = false;
            dgvItems.AllowUserToDeleteRows = false;
            dgvItems.AllowUserToResizeRows = false;
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvItems.Columns.AddRange(new DataGridViewColumn[] { colLibroId, colCodigo, colTitulo, colStock, colOptimo, colCantidad, colCosto, colSubtotal, colRecibido, colCostoRecibido, colQuitar });
            dgvItems.Dock = DockStyle.Fill;
            dgvItems.Location = new Point(0, 212);
            dgvItems.MultiSelect = false;
            dgvItems.Name = "dgvItems";
            dgvItems.RowHeadersVisible = false;
            dgvItems.RowHeadersWidth = 51;
            dgvItems.RowTemplate.Height = 30;
            dgvItems.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvItems.Size = new Size(1200, 378);
            dgvItems.TabIndex = 2;
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
            // colCodigo
            //
            colCodigo.DataPropertyName = "Codigo";
            colCodigo.FillWeight = 90F;
            colCodigo.HeaderText = "Código / ISBN";
            colCodigo.MinimumWidth = 6;
            colCodigo.Name = "colCodigo";
            colCodigo.ReadOnly = true;
            //
            // colTitulo
            //
            colTitulo.DataPropertyName = "Titulo";
            colTitulo.FillWeight = 220F;
            colTitulo.HeaderText = "Libro";
            colTitulo.MinimumWidth = 6;
            colTitulo.Name = "colTitulo";
            colTitulo.ReadOnly = true;
            //
            // colStock
            //
            colStock.DataPropertyName = "StockActual";
            colStock.FillWeight = 55F;
            colStock.HeaderText = "Stock actual";
            colStock.MinimumWidth = 6;
            colStock.Name = "colStock";
            colStock.ReadOnly = true;
            //
            // colOptimo
            //
            colOptimo.DataPropertyName = "StockOptimo";
            colOptimo.FillWeight = 55F;
            colOptimo.HeaderText = "Óptimo";
            colOptimo.MinimumWidth = 6;
            colOptimo.Name = "colOptimo";
            colOptimo.ReadOnly = true;
            //
            // colCantidad
            //
            colCantidad.DataPropertyName = "CantidadPedida";
            colCantidad.FillWeight = 65F;
            colCantidad.HeaderText = "A pedir";
            colCantidad.MinimumWidth = 6;
            colCantidad.Name = "colCantidad";
            //
            // colCosto
            //
            colCosto.DataPropertyName = "CostoUnitario";
            colCosto.FillWeight = 75F;
            colCosto.HeaderText = "Costo unit.";
            colCosto.MinimumWidth = 6;
            colCosto.Name = "colCosto";
            //
            // colSubtotal
            //
            colSubtotal.DataPropertyName = "Subtotal";
            colSubtotal.FillWeight = 85F;
            colSubtotal.HeaderText = "Subtotal";
            colSubtotal.MinimumWidth = 6;
            colSubtotal.Name = "colSubtotal";
            colSubtotal.ReadOnly = true;
            //
            // colRecibido
            //
            colRecibido.DataPropertyName = "CantidadRecibida";
            colRecibido.FillWeight = 65F;
            colRecibido.HeaderText = "Recibido";
            colRecibido.MinimumWidth = 6;
            colRecibido.Name = "colRecibido";
            colRecibido.Visible = false;
            //
            // colCostoRecibido
            //
            colCostoRecibido.DataPropertyName = "CostoRecibido";
            colCostoRecibido.FillWeight = 75F;
            colCostoRecibido.HeaderText = "Costo real";
            colCostoRecibido.MinimumWidth = 6;
            colCostoRecibido.Name = "colCostoRecibido";
            colCostoRecibido.Visible = false;
            //
            // colQuitar
            //
            colQuitar.FillWeight = 45F;
            colQuitar.HeaderText = "";
            colQuitar.MinimumWidth = 6;
            colQuitar.Name = "colQuitar";
            colQuitar.Text = "Quitar";
            colQuitar.ToolTipText = "Quitar el ítem (Supr)";
            colQuitar.UseColumnTextForButtonValue = true;
            //
            // panelPie
            //
            panelPie.Controls.Add(lblObservacionesTitulo);
            panelPie.Controls.Add(txtObservaciones);
            panelPie.Controls.Add(lblInfoEstado);
            panelPie.Controls.Add(chkActualizarCostos);
            panelPie.Controls.Add(lblTotalTitulo);
            panelPie.Controls.Add(lblTotal);
            panelPie.Controls.Add(btnGuardar);
            panelPie.Controls.Add(btnEmitir);
            panelPie.Controls.Add(btnRecibir);
            panelPie.Controls.Add(btnCerrar);
            panelPie.Dock = DockStyle.Bottom;
            panelPie.Location = new Point(0, 590);
            panelPie.Name = "panelPie";
            panelPie.Size = new Size(1200, 170);
            panelPie.TabIndex = 3;
            //
            // lblObservacionesTitulo
            //
            lblObservacionesTitulo.AutoSize = true;
            lblObservacionesTitulo.Location = new Point(12, 6);
            lblObservacionesTitulo.Name = "lblObservacionesTitulo";
            lblObservacionesTitulo.Size = new Size(296, 20);
            lblObservacionesTitulo.TabIndex = 0;
            lblObservacionesTitulo.Tag = "BLANCO";
            lblObservacionesTitulo.Text = "Observaciones / condiciones de entrega";
            //
            // txtObservaciones
            //
            txtObservaciones.Location = new Point(12, 30);
            txtObservaciones.MaxLength = 500;
            txtObservaciones.Multiline = true;
            txtObservaciones.Name = "txtObservaciones";
            txtObservaciones.ScrollBars = ScrollBars.Vertical;
            txtObservaciones.Size = new Size(560, 80);
            txtObservaciones.TabIndex = 0;
            //
            // lblInfoEstado
            //
            lblInfoEstado.AutoEllipsis = true;
            lblInfoEstado.Location = new Point(12, 120);
            lblInfoEstado.Name = "lblInfoEstado";
            lblInfoEstado.Size = new Size(560, 44);
            lblInfoEstado.TabIndex = 2;
            lblInfoEstado.Tag = "BLANCO";
            //
            // chkActualizarCostos
            //
            chkActualizarCostos.AutoSize = true;
            chkActualizarCostos.Checked = true;
            chkActualizarCostos.CheckState = CheckState.Checked;
            chkActualizarCostos.Location = new Point(590, 76);
            chkActualizarCostos.Name = "chkActualizarCostos";
            chkActualizarCostos.Size = new Size(419, 24);
            chkActualizarCostos.TabIndex = 1;
            chkActualizarCostos.Text = "Actualizar el costo del libro y el precio del proveedor si varió";
            chkActualizarCostos.UseVisualStyleBackColor = true;
            chkActualizarCostos.Visible = false;
            //
            // lblTotalTitulo
            //
            lblTotalTitulo.AutoSize = true;
            lblTotalTitulo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTotalTitulo.Location = new Point(590, 22);
            lblTotalTitulo.Name = "lblTotalTitulo";
            lblTotalTitulo.Size = new Size(144, 28);
            lblTotalTitulo.TabIndex = 4;
            lblTotalTitulo.Tag = "BLANCO";
            lblTotalTitulo.Text = "Total estimado";
            //
            // lblTotal
            //
            lblTotal.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTotal.Location = new Point(860, 12);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(328, 46);
            lblTotal.TabIndex = 5;
            lblTotal.Tag = "BLANCO";
            lblTotal.Text = "$0,00";
            lblTotal.TextAlign = ContentAlignment.MiddleRight;
            //
            // btnGuardar
            //
            btnGuardar.Location = new Point(590, 116);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(150, 46);
            btnGuardar.TabIndex = 2;
            btnGuardar.Text = "Guardar borrador";
            btnGuardar.UseVisualStyleBackColor = true;
            //
            // btnEmitir
            //
            btnEmitir.Location = new Point(746, 116);
            btnEmitir.Name = "btnEmitir";
            btnEmitir.Size = new Size(150, 46);
            btnEmitir.TabIndex = 3;
            btnEmitir.Text = "Emitir al proveedor";
            btnEmitir.UseVisualStyleBackColor = true;
            //
            // btnRecibir
            //
            btnRecibir.Location = new Point(902, 116);
            btnRecibir.Name = "btnRecibir";
            btnRecibir.Size = new Size(170, 46);
            btnRecibir.TabIndex = 4;
            btnRecibir.Text = "Registrar recepción";
            btnRecibir.UseVisualStyleBackColor = true;
            //
            // btnCerrar
            //
            btnCerrar.Location = new Point(1078, 116);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(110, 46);
            btnCerrar.TabIndex = 5;
            btnCerrar.Text = "Cerrar";
            btnCerrar.UseVisualStyleBackColor = true;
            //
            // lstSugerencias
            //
            lstSugerencias.Font = new Font("Segoe UI", 10F);
            lstSugerencias.IntegralHeight = false;
            lstSugerencias.Location = new Point(12, 180);
            lstSugerencias.Name = "lstSugerencias";
            lstSugerencias.Size = new Size(760, 32);
            lstSugerencias.TabIndex = 9;
            lstSugerencias.TabStop = false;
            lstSugerencias.Visible = false;
            //
            // FrmOrdenReposicion
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 760);
            Controls.Add(lstSugerencias);
            Controls.Add(dgvItems);
            Controls.Add(panelPie);
            Controls.Add(panelCarga);
            Controls.Add(panelCabecera);
            MinimumSize = new Size(1100, 700);
            Name = "FrmOrdenReposicion";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Orden de reposición";
            Load += FrmOrdenReposicion_Load;
            panelCabecera.ResumeLayout(false);
            panelCabecera.PerformLayout();
            panelCarga.ResumeLayout(false);
            panelCarga.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numCantidad).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCosto).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvItems).EndInit();
            panelPie.ResumeLayout(false);
            panelPie.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelCabecera;
        private Label lblNumero;
        private Label lblEstadoOrden;
        private Label lblFecha;
        private Label lblUsuario;
        private Label lblProveedorTitulo;
        private ComboBox cbProveedor;
        private Label lblContacto;
        private Panel panelCarga;
        private Label lblBuscarTitulo;
        private TextBox txtBuscar;
        private Label lblCantidadTitulo;
        private NumericUpDown numCantidad;
        private Label lblCostoTitulo;
        private NumericUpDown numCosto;
        private Button btnAgregar;
        private CheckBox chkSoloProveedor;
        private Label lblSeleccion;
        private DataGridView dgvItems;
        private DataGridViewTextBoxColumn colLibroId;
        private DataGridViewTextBoxColumn colCodigo;
        private DataGridViewTextBoxColumn colTitulo;
        private DataGridViewTextBoxColumn colStock;
        private DataGridViewTextBoxColumn colOptimo;
        private DataGridViewTextBoxColumn colCantidad;
        private DataGridViewTextBoxColumn colCosto;
        private DataGridViewTextBoxColumn colSubtotal;
        private DataGridViewTextBoxColumn colRecibido;
        private DataGridViewTextBoxColumn colCostoRecibido;
        private DataGridViewButtonColumn colQuitar;
        private Panel panelPie;
        private Label lblObservacionesTitulo;
        private TextBox txtObservaciones;
        private Label lblInfoEstado;
        private CheckBox chkActualizarCostos;
        private Label lblTotalTitulo;
        private Label lblTotal;
        private Button btnGuardar;
        private Button btnEmitir;
        private Button btnRecibir;
        private Button btnCerrar;
        private ListBox lstSugerencias;
    }
}
