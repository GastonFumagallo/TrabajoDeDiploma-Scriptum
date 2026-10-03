namespace Vista
{
    partial class FrmGestionarVentas
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
            panelAcciones = new Panel();
            btnRealizarVenta = new Button();
            btnVerDetalles = new Button();
            btnVerTicket = new Button();
            btnAnularVenta = new Button();
            btnExportar = new Button();
            btnSalir = new Button();
            panelFiltros = new Panel();
            chkFechas = new CheckBox();
            dtpDesde = new DateTimePicker();
            dtpHasta = new DateTimePicker();
            lblClienteTitulo = new Label();
            txtFiltrar = new TextBox();
            lblComprobanteTitulo = new Label();
            txtComprobante = new TextBox();
            lblEstadoTitulo = new Label();
            cbEstado = new ComboBox();
            lblMetodoTitulo = new Label();
            cbMetodoPago = new ComboBox();
            btnBorrarFiltros = new Button();
            panelMetricas = new Panel();
            lblRecaudadoTitulo = new Label();
            lblRecaudado = new Label();
            lblCantidadTitulo = new Label();
            lblCantidad = new Label();
            lblPromedioTitulo = new Label();
            lblPromedio = new Label();
            lblAnuladasTitulo = new Label();
            lblAnuladas = new Label();
            dgvVentas = new DataGridView();
            panelPie = new Panel();
            lblResumen = new Label();
            btnPaginaAnterior = new Button();
            lblPagina = new Label();
            btnPaginaSiguiente = new Button();
            panelAcciones.SuspendLayout();
            panelFiltros.SuspendLayout();
            panelMetricas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvVentas).BeginInit();
            panelPie.SuspendLayout();
            SuspendLayout();
            //
            // panelAcciones
            //
            panelAcciones.Controls.Add(btnRealizarVenta);
            panelAcciones.Controls.Add(btnVerDetalles);
            panelAcciones.Controls.Add(btnVerTicket);
            panelAcciones.Controls.Add(btnAnularVenta);
            panelAcciones.Controls.Add(btnExportar);
            panelAcciones.Controls.Add(btnSalir);
            panelAcciones.Dock = DockStyle.Top;
            panelAcciones.Location = new Point(0, 0);
            panelAcciones.Name = "panelAcciones";
            panelAcciones.Size = new Size(1427, 80);
            panelAcciones.TabIndex = 0;
            //
            // btnRealizarVenta
            //
            btnRealizarVenta.Location = new Point(12, 6);
            btnRealizarVenta.Name = "btnRealizarVenta";
            btnRealizarVenta.Size = new Size(120, 68);
            btnRealizarVenta.TabIndex = 0;
            btnRealizarVenta.Text = "Nueva venta";
            btnRealizarVenta.UseVisualStyleBackColor = true;
            //
            // btnVerDetalles
            //
            btnVerDetalles.Location = new Point(138, 6);
            btnVerDetalles.Name = "btnVerDetalles";
            btnVerDetalles.Size = new Size(120, 68);
            btnVerDetalles.TabIndex = 1;
            btnVerDetalles.Text = "Ver detalle";
            btnVerDetalles.UseVisualStyleBackColor = true;
            //
            // btnVerTicket
            //
            btnVerTicket.Location = new Point(264, 6);
            btnVerTicket.Name = "btnVerTicket";
            btnVerTicket.Size = new Size(120, 68);
            btnVerTicket.TabIndex = 2;
            btnVerTicket.Text = "Reimprimir comprobante";
            btnVerTicket.UseVisualStyleBackColor = true;
            //
            // btnAnularVenta
            //
            btnAnularVenta.Location = new Point(390, 6);
            btnAnularVenta.Name = "btnAnularVenta";
            btnAnularVenta.Size = new Size(120, 68);
            btnAnularVenta.TabIndex = 3;
            btnAnularVenta.Text = "Anular venta";
            btnAnularVenta.UseVisualStyleBackColor = true;
            //
            // btnExportar
            //
            btnExportar.Location = new Point(516, 6);
            btnExportar.Name = "btnExportar";
            btnExportar.Size = new Size(120, 68);
            btnExportar.TabIndex = 4;
            btnExportar.Text = "Exportar listado";
            btnExportar.UseVisualStyleBackColor = true;
            //
            // btnSalir
            //
            btnSalir.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSalir.Location = new Point(1295, 6);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(120, 68);
            btnSalir.TabIndex = 5;
            btnSalir.Text = "Salir";
            btnSalir.UseVisualStyleBackColor = true;
            //
            // panelFiltros
            //
            panelFiltros.Controls.Add(chkFechas);
            panelFiltros.Controls.Add(dtpDesde);
            panelFiltros.Controls.Add(dtpHasta);
            panelFiltros.Controls.Add(lblClienteTitulo);
            panelFiltros.Controls.Add(txtFiltrar);
            panelFiltros.Controls.Add(lblComprobanteTitulo);
            panelFiltros.Controls.Add(txtComprobante);
            panelFiltros.Controls.Add(lblEstadoTitulo);
            panelFiltros.Controls.Add(cbEstado);
            panelFiltros.Controls.Add(lblMetodoTitulo);
            panelFiltros.Controls.Add(cbMetodoPago);
            panelFiltros.Controls.Add(btnBorrarFiltros);
            panelFiltros.Dock = DockStyle.Top;
            panelFiltros.Location = new Point(0, 80);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new Size(1427, 76);
            panelFiltros.TabIndex = 1;
            //
            // chkFechas
            //
            chkFechas.AutoSize = true;
            chkFechas.Checked = true;
            chkFechas.CheckState = CheckState.Checked;
            chkFechas.Location = new Point(12, 8);
            chkFechas.Name = "chkFechas";
            chkFechas.Size = new Size(143, 24);
            chkFechas.TabIndex = 0;
            chkFechas.Text = "Período (desde / hasta)";
            chkFechas.UseVisualStyleBackColor = true;
            //
            // dtpDesde
            //
            dtpDesde.Format = DateTimePickerFormat.Short;
            dtpDesde.Location = new Point(12, 38);
            dtpDesde.Name = "dtpDesde";
            dtpDesde.Size = new Size(130, 27);
            dtpDesde.TabIndex = 1;
            //
            // dtpHasta
            //
            dtpHasta.Format = DateTimePickerFormat.Short;
            dtpHasta.Location = new Point(148, 38);
            dtpHasta.Name = "dtpHasta";
            dtpHasta.Size = new Size(130, 27);
            dtpHasta.TabIndex = 2;
            //
            // lblClienteTitulo
            //
            lblClienteTitulo.AutoSize = true;
            lblClienteTitulo.Location = new Point(296, 12);
            lblClienteTitulo.Name = "lblClienteTitulo";
            lblClienteTitulo.Size = new Size(97, 20);
            lblClienteTitulo.TabIndex = 3;
            lblClienteTitulo.Tag = "BLANCO";
            lblClienteTitulo.Text = "Cliente o DNI";
            //
            // txtFiltrar
            //
            txtFiltrar.Location = new Point(296, 38);
            txtFiltrar.Name = "txtFiltrar";
            txtFiltrar.Size = new Size(220, 27);
            txtFiltrar.TabIndex = 3;
            //
            // lblComprobanteTitulo
            //
            lblComprobanteTitulo.AutoSize = true;
            lblComprobanteTitulo.Location = new Point(530, 12);
            lblComprobanteTitulo.Name = "lblComprobanteTitulo";
            lblComprobanteTitulo.Size = new Size(118, 20);
            lblComprobanteTitulo.TabIndex = 5;
            lblComprobanteTitulo.Tag = "BLANCO";
            lblComprobanteTitulo.Text = "N° comprobante";
            //
            // txtComprobante
            //
            txtComprobante.Location = new Point(530, 38);
            txtComprobante.Name = "txtComprobante";
            txtComprobante.Size = new Size(140, 27);
            txtComprobante.TabIndex = 4;
            //
            // lblEstadoTitulo
            //
            lblEstadoTitulo.AutoSize = true;
            lblEstadoTitulo.Location = new Point(684, 12);
            lblEstadoTitulo.Name = "lblEstadoTitulo";
            lblEstadoTitulo.Size = new Size(54, 20);
            lblEstadoTitulo.TabIndex = 7;
            lblEstadoTitulo.Tag = "BLANCO";
            lblEstadoTitulo.Text = "Estado";
            //
            // cbEstado
            //
            cbEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEstado.Location = new Point(684, 37);
            cbEstado.Name = "cbEstado";
            cbEstado.Size = new Size(150, 28);
            cbEstado.TabIndex = 5;
            //
            // lblMetodoTitulo
            //
            lblMetodoTitulo.AutoSize = true;
            lblMetodoTitulo.Location = new Point(848, 12);
            lblMetodoTitulo.Name = "lblMetodoTitulo";
            lblMetodoTitulo.Size = new Size(108, 20);
            lblMetodoTitulo.TabIndex = 9;
            lblMetodoTitulo.Tag = "BLANCO";
            lblMetodoTitulo.Text = "Medio de pago";
            //
            // cbMetodoPago
            //
            cbMetodoPago.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMetodoPago.Location = new Point(848, 37);
            cbMetodoPago.Name = "cbMetodoPago";
            cbMetodoPago.Size = new Size(190, 28);
            cbMetodoPago.TabIndex = 6;
            //
            // btnBorrarFiltros
            //
            btnBorrarFiltros.Location = new Point(1052, 30);
            btnBorrarFiltros.Name = "btnBorrarFiltros";
            btnBorrarFiltros.Size = new Size(140, 38);
            btnBorrarFiltros.TabIndex = 7;
            btnBorrarFiltros.Text = "Limpiar filtros";
            btnBorrarFiltros.UseVisualStyleBackColor = true;
            //
            // panelMetricas
            //
            panelMetricas.Controls.Add(lblRecaudadoTitulo);
            panelMetricas.Controls.Add(lblRecaudado);
            panelMetricas.Controls.Add(lblCantidadTitulo);
            panelMetricas.Controls.Add(lblCantidad);
            panelMetricas.Controls.Add(lblPromedioTitulo);
            panelMetricas.Controls.Add(lblPromedio);
            panelMetricas.Controls.Add(lblAnuladasTitulo);
            panelMetricas.Controls.Add(lblAnuladas);
            panelMetricas.Dock = DockStyle.Top;
            panelMetricas.Location = new Point(0, 156);
            panelMetricas.Name = "panelMetricas";
            panelMetricas.Size = new Size(1427, 72);
            panelMetricas.TabIndex = 2;
            //
            // lblRecaudadoTitulo
            //
            lblRecaudadoTitulo.AutoSize = true;
            lblRecaudadoTitulo.Location = new Point(12, 6);
            lblRecaudadoTitulo.Name = "lblRecaudadoTitulo";
            lblRecaudadoTitulo.Size = new Size(114, 20);
            lblRecaudadoTitulo.TabIndex = 0;
            lblRecaudadoTitulo.Tag = "BLANCO";
            lblRecaudadoTitulo.Text = "Total recaudado";
            //
            // lblRecaudado
            //
            lblRecaudado.AutoSize = true;
            lblRecaudado.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblRecaudado.Location = new Point(12, 28);
            lblRecaudado.Name = "lblRecaudado";
            lblRecaudado.Size = new Size(76, 37);
            lblRecaudado.TabIndex = 1;
            lblRecaudado.Tag = "BLANCO";
            lblRecaudado.Text = "$0,00";
            //
            // lblCantidadTitulo
            //
            lblCantidadTitulo.AutoSize = true;
            lblCantidadTitulo.Location = new Point(320, 6);
            lblCantidadTitulo.Name = "lblCantidadTitulo";
            lblCantidadTitulo.Size = new Size(129, 20);
            lblCantidadTitulo.TabIndex = 2;
            lblCantidadTitulo.Tag = "BLANCO";
            lblCantidadTitulo.Text = "Ventas completadas";
            //
            // lblCantidad
            //
            lblCantidad.AutoSize = true;
            lblCantidad.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblCantidad.Location = new Point(320, 28);
            lblCantidad.Name = "lblCantidad";
            lblCantidad.Size = new Size(31, 37);
            lblCantidad.TabIndex = 3;
            lblCantidad.Tag = "BLANCO";
            lblCantidad.Text = "0";
            //
            // lblPromedioTitulo
            //
            lblPromedioTitulo.AutoSize = true;
            lblPromedioTitulo.Location = new Point(560, 6);
            lblPromedioTitulo.Name = "lblPromedioTitulo";
            lblPromedioTitulo.Size = new Size(114, 20);
            lblPromedioTitulo.TabIndex = 4;
            lblPromedioTitulo.Tag = "BLANCO";
            lblPromedioTitulo.Text = "Ticket promedio";
            //
            // lblPromedio
            //
            lblPromedio.AutoSize = true;
            lblPromedio.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblPromedio.Location = new Point(560, 28);
            lblPromedio.Name = "lblPromedio";
            lblPromedio.Size = new Size(76, 37);
            lblPromedio.TabIndex = 5;
            lblPromedio.Tag = "BLANCO";
            lblPromedio.Text = "$0,00";
            //
            // lblAnuladasTitulo
            //
            lblAnuladasTitulo.AutoSize = true;
            lblAnuladasTitulo.Location = new Point(820, 6);
            lblAnuladasTitulo.Name = "lblAnuladasTitulo";
            lblAnuladasTitulo.Size = new Size(70, 20);
            lblAnuladasTitulo.TabIndex = 6;
            lblAnuladasTitulo.Tag = "BLANCO";
            lblAnuladasTitulo.Text = "Anuladas";
            //
            // lblAnuladas
            //
            lblAnuladas.AutoSize = true;
            lblAnuladas.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblAnuladas.Location = new Point(820, 28);
            lblAnuladas.Name = "lblAnuladas";
            lblAnuladas.Size = new Size(31, 37);
            lblAnuladas.TabIndex = 7;
            lblAnuladas.Tag = "BLANCO";
            lblAnuladas.Text = "0";
            //
            // dgvVentas
            //
            dgvVentas.AllowUserToAddRows = false;
            dgvVentas.AllowUserToDeleteRows = false;
            dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVentas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvVentas.Dock = DockStyle.Fill;
            dgvVentas.Location = new Point(0, 228);
            dgvVentas.MultiSelect = false;
            dgvVentas.Name = "dgvVentas";
            dgvVentas.ReadOnly = true;
            dgvVentas.RowHeadersVisible = false;
            dgvVentas.RowHeadersWidth = 51;
            dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvVentas.Size = new Size(1427, 477);
            dgvVentas.TabIndex = 3;
            //
            // panelPie
            //
            panelPie.Controls.Add(lblResumen);
            panelPie.Controls.Add(btnPaginaAnterior);
            panelPie.Controls.Add(lblPagina);
            panelPie.Controls.Add(btnPaginaSiguiente);
            panelPie.Dock = DockStyle.Bottom;
            panelPie.Location = new Point(0, 705);
            panelPie.Name = "panelPie";
            panelPie.Size = new Size(1427, 48);
            panelPie.TabIndex = 4;
            //
            // lblResumen
            //
            lblResumen.AutoSize = true;
            lblResumen.Location = new Point(12, 14);
            lblResumen.Name = "lblResumen";
            lblResumen.Size = new Size(0, 20);
            lblResumen.TabIndex = 0;
            lblResumen.Tag = "BLANCO";
            //
            // btnPaginaAnterior
            //
            btnPaginaAnterior.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPaginaAnterior.Location = new Point(1123, 7);
            btnPaginaAnterior.Name = "btnPaginaAnterior";
            btnPaginaAnterior.Size = new Size(60, 34);
            btnPaginaAnterior.TabIndex = 1;
            btnPaginaAnterior.Text = "◀";
            btnPaginaAnterior.UseVisualStyleBackColor = true;
            //
            // lblPagina
            //
            lblPagina.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblPagina.Location = new Point(1189, 7);
            lblPagina.Name = "lblPagina";
            lblPagina.Size = new Size(160, 34);
            lblPagina.TabIndex = 2;
            lblPagina.Tag = "BLANCO";
            lblPagina.TextAlign = ContentAlignment.MiddleCenter;
            //
            // btnPaginaSiguiente
            //
            btnPaginaSiguiente.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPaginaSiguiente.Location = new Point(1355, 7);
            btnPaginaSiguiente.Name = "btnPaginaSiguiente";
            btnPaginaSiguiente.Size = new Size(60, 34);
            btnPaginaSiguiente.TabIndex = 3;
            btnPaginaSiguiente.Text = "▶";
            btnPaginaSiguiente.UseVisualStyleBackColor = true;
            //
            // FrmGestionarVentas
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1427, 753);
            ControlBox = false;
            Controls.Add(dgvVentas);
            Controls.Add(panelPie);
            Controls.Add(panelMetricas);
            Controls.Add(panelFiltros);
            Controls.Add(panelAcciones);
            Name = "FrmGestionarVentas";
            Text = "VENTAS";
            Load += FrmGestionarVentas_Load;
            panelAcciones.ResumeLayout(false);
            panelFiltros.ResumeLayout(false);
            panelFiltros.PerformLayout();
            panelMetricas.ResumeLayout(false);
            panelMetricas.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvVentas).EndInit();
            panelPie.ResumeLayout(false);
            panelPie.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelAcciones;
        private Button btnRealizarVenta;
        private Button btnVerDetalles;
        private Button btnVerTicket;
        private Button btnAnularVenta;
        private Button btnExportar;
        private Button btnSalir;
        private Panel panelFiltros;
        private CheckBox chkFechas;
        private DateTimePicker dtpDesde;
        private DateTimePicker dtpHasta;
        private Label lblClienteTitulo;
        private TextBox txtFiltrar;
        private Label lblComprobanteTitulo;
        private TextBox txtComprobante;
        private Label lblEstadoTitulo;
        private ComboBox cbEstado;
        private Label lblMetodoTitulo;
        private ComboBox cbMetodoPago;
        private Button btnBorrarFiltros;
        private Panel panelMetricas;
        private Label lblRecaudadoTitulo;
        private Label lblRecaudado;
        private Label lblCantidadTitulo;
        private Label lblCantidad;
        private Label lblPromedioTitulo;
        private Label lblPromedio;
        private Label lblAnuladasTitulo;
        private Label lblAnuladas;
        private DataGridView dgvVentas;
        private Panel panelPie;
        private Label lblResumen;
        private Button btnPaginaAnterior;
        private Label lblPagina;
        private Button btnPaginaSiguiente;
    }
}
