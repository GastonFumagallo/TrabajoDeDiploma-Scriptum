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
            btnRealizarVenta = new Button();
            panelSuperior = new Panel();
            gbFiltrarFecha = new GroupBox();
            btnBorrarFiltrosFecha = new Button();
            btnFiltrarFecha = new Button();
            dtpHasta = new DateTimePicker();
            label1 = new Label();
            dtpDesde = new DateTimePicker();
            label2 = new Label();
            checkFiltrarPorFecha = new CheckBox();
            btnVerDetalles = new Button();
            lblFiltrar = new Label();
            btnFiltrar = new Button();
            txtFiltrar = new TextBox();
            btnBorrarFiltros = new Button();
            dgvVentas = new DataGridView();
            btnSalir = new Button();
            btnVerTicket = new Button();
            panelSuperior.SuspendLayout();
            gbFiltrarFecha.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvVentas).BeginInit();
            SuspendLayout();
            // 
            // btnRealizarVenta
            // 
            btnRealizarVenta.BackColor = SystemColors.Control;
            btnRealizarVenta.Location = new Point(12, 24);
            btnRealizarVenta.Name = "btnRealizarVenta";
            btnRealizarVenta.Size = new Size(120, 68);
            btnRealizarVenta.TabIndex = 0;
            btnRealizarVenta.Text = "Realizar Venta";
            btnRealizarVenta.UseVisualStyleBackColor = false;
            btnRealizarVenta.Click += btnRealizarVenta_Click;
            // 
            // panelSuperior
            // 
            panelSuperior.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelSuperior.BackColor = SystemColors.ControlLightLight;
            panelSuperior.Controls.Add(btnVerTicket);
            panelSuperior.Controls.Add(gbFiltrarFecha);
            panelSuperior.Controls.Add(checkFiltrarPorFecha);
            panelSuperior.Controls.Add(btnVerDetalles);
            panelSuperior.Controls.Add(lblFiltrar);
            panelSuperior.Controls.Add(btnRealizarVenta);
            panelSuperior.Controls.Add(btnFiltrar);
            panelSuperior.Controls.Add(txtFiltrar);
            panelSuperior.Controls.Add(btnBorrarFiltros);
            panelSuperior.Location = new Point(0, 0);
            panelSuperior.Name = "panelSuperior";
            panelSuperior.Size = new Size(1427, 139);
            panelSuperior.TabIndex = 2;
            // 
            // gbFiltrarFecha
            // 
            gbFiltrarFecha.Controls.Add(btnBorrarFiltrosFecha);
            gbFiltrarFecha.Controls.Add(btnFiltrarFecha);
            gbFiltrarFecha.Controls.Add(dtpHasta);
            gbFiltrarFecha.Controls.Add(label1);
            gbFiltrarFecha.Controls.Add(dtpDesde);
            gbFiltrarFecha.Controls.Add(label2);
            gbFiltrarFecha.Location = new Point(577, 12);
            gbFiltrarFecha.Name = "gbFiltrarFecha";
            gbFiltrarFecha.Size = new Size(435, 113);
            gbFiltrarFecha.TabIndex = 9;
            gbFiltrarFecha.TabStop = false;
            gbFiltrarFecha.Text = "Filtrar por fecha";
            // 
            // btnBorrarFiltrosFecha
            // 
            btnBorrarFiltrosFecha.BackColor = SystemColors.Control;
            btnBorrarFiltrosFecha.Location = new Point(244, 51);
            btnBorrarFiltrosFecha.Name = "btnBorrarFiltrosFecha";
            btnBorrarFiltrosFecha.Size = new Size(179, 56);
            btnBorrarFiltrosFecha.TabIndex = 15;
            btnBorrarFiltrosFecha.Text = "Borrar Filtros Fecha";
            btnBorrarFiltrosFecha.UseVisualStyleBackColor = false;
            btnBorrarFiltrosFecha.Click += btnBorrarFiltrosFecha_Click;
            // 
            // btnFiltrarFecha
            // 
            btnFiltrarFecha.BackColor = SystemColors.Control;
            btnFiltrarFecha.Location = new Point(244, 13);
            btnFiltrarFecha.Name = "btnFiltrarFecha";
            btnFiltrarFecha.Size = new Size(179, 32);
            btnFiltrarFecha.TabIndex = 10;
            btnFiltrarFecha.Text = "Filtrar";
            btnFiltrarFecha.UseVisualStyleBackColor = false;
            btnFiltrarFecha.Click += btnFiltrarFecha_Click;
            // 
            // dtpHasta
            // 
            dtpHasta.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dtpHasta.Format = DateTimePickerFormat.Short;
            dtpHasta.Location = new Point(88, 63);
            dtpHasta.Name = "dtpHasta";
            dtpHasta.Size = new Size(150, 34);
            dtpHasta.TabIndex = 14;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(7, 31);
            label1.Name = "label1";
            label1.Size = new Size(70, 28);
            label1.TabIndex = 11;
            label1.Tag = "BLANCO";
            label1.Text = "Desde:";
            // 
            // dtpDesde
            // 
            dtpDesde.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dtpDesde.Format = DateTimePickerFormat.Short;
            dtpDesde.Location = new Point(88, 23);
            dtpDesde.Name = "dtpDesde";
            dtpDesde.Size = new Size(150, 34);
            dtpDesde.TabIndex = 13;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(7, 68);
            label2.Name = "label2";
            label2.Size = new Size(65, 28);
            label2.TabIndex = 12;
            label2.Tag = "BLANCO";
            label2.Text = "Hasta:";
            // 
            // checkFiltrarPorFecha
            // 
            checkFiltrarPorFecha.AutoSize = true;
            checkFiltrarPorFecha.Location = new Point(435, 47);
            checkFiltrarPorFecha.Name = "checkFiltrarPorFecha";
            checkFiltrarPorFecha.Size = new Size(136, 24);
            checkFiltrarPorFecha.TabIndex = 8;
            checkFiltrarPorFecha.Text = "Filtrar por fecha";
            checkFiltrarPorFecha.UseVisualStyleBackColor = true;
            checkFiltrarPorFecha.CheckedChanged += checkFiltrarPorFecha_CheckedChanged;
            // 
            // btnVerDetalles
            // 
            btnVerDetalles.BackColor = SystemColors.Control;
            btnVerDetalles.Location = new Point(138, 24);
            btnVerDetalles.Name = "btnVerDetalles";
            btnVerDetalles.Size = new Size(120, 68);
            btnVerDetalles.TabIndex = 7;
            btnVerDetalles.Text = "Ver detalles";
            btnVerDetalles.UseVisualStyleBackColor = false;
            btnVerDetalles.Click += btnVerDetalles_Click;
            // 
            // lblFiltrar
            // 
            lblFiltrar.Anchor = AnchorStyles.Right;
            lblFiltrar.AutoSize = true;
            lblFiltrar.Location = new Point(1099, 35);
            lblFiltrar.Name = "lblFiltrar";
            lblFiltrar.Size = new Size(124, 20);
            lblFiltrar.TabIndex = 6;
            lblFiltrar.Tag = "BLANCO";
            lblFiltrar.Text = "Filtrar por Cliente";
            // 
            // btnFiltrar
            // 
            btnFiltrar.Anchor = AnchorStyles.Right;
            btnFiltrar.BackColor = SystemColors.Control;
            btnFiltrar.Location = new Point(1285, 10);
            btnFiltrar.Name = "btnFiltrar";
            btnFiltrar.Size = new Size(120, 59);
            btnFiltrar.TabIndex = 5;
            btnFiltrar.Text = "Filtrar";
            btnFiltrar.UseVisualStyleBackColor = false;
            btnFiltrar.Click += btnFiltrar_Click;
            // 
            // txtFiltrar
            // 
            txtFiltrar.Anchor = AnchorStyles.Right;
            txtFiltrar.Location = new Point(1049, 63);
            txtFiltrar.Name = "txtFiltrar";
            txtFiltrar.Size = new Size(230, 27);
            txtFiltrar.TabIndex = 4;
            txtFiltrar.TextChanged += txtFiltrar_TextChanged;
            // 
            // btnBorrarFiltros
            // 
            btnBorrarFiltros.Anchor = AnchorStyles.Right;
            btnBorrarFiltros.BackColor = SystemColors.Control;
            btnBorrarFiltros.Location = new Point(1285, 73);
            btnBorrarFiltros.Name = "btnBorrarFiltros";
            btnBorrarFiltros.Size = new Size(120, 61);
            btnBorrarFiltros.TabIndex = 3;
            btnBorrarFiltros.Text = "Borrar Filtros";
            btnBorrarFiltros.UseVisualStyleBackColor = false;
            btnBorrarFiltros.Click += btnBorrarFiltros_Click;
            // 
            // dgvVentas
            // 
            dgvVentas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVentas.BackgroundColor = SystemColors.MenuBar;
            dgvVentas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvVentas.Location = new Point(12, 156);
            dgvVentas.Name = "dgvVentas";
            dgvVentas.ReadOnly = true;
            dgvVentas.RowHeadersWidth = 51;
            dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvVentas.Size = new Size(1403, 511);
            dgvVentas.TabIndex = 3;
            // 
            // btnSalir
            // 
            btnSalir.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSalir.BackColor = SystemColors.Control;
            btnSalir.Location = new Point(1284, 673);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(120, 68);
            btnSalir.TabIndex = 10;
            btnSalir.Text = "Salir";
            btnSalir.UseVisualStyleBackColor = false;
            btnSalir.Click += btnSalir_Click;
            // 
            // btnVerTicket
            // 
            btnVerTicket.BackColor = SystemColors.Control;
            btnVerTicket.Location = new Point(264, 22);
            btnVerTicket.Name = "btnVerTicket";
            btnVerTicket.Size = new Size(120, 68);
            btnVerTicket.TabIndex = 10;
            btnVerTicket.Text = "Ver ticket";
            btnVerTicket.UseVisualStyleBackColor = false;
            btnVerTicket.Click += btnVerTicket_Click;
            // 
            // FrmGestionarVentas
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(1427, 753);
            ControlBox = false;
            Controls.Add(btnSalir);
            Controls.Add(dgvVentas);
            Controls.Add(panelSuperior);
            Name = "FrmGestionarVentas";
            Text = "VENTAS";
            Load += FrmGestionarVentas_Load;
            panelSuperior.ResumeLayout(false);
            panelSuperior.PerformLayout();
            gbFiltrarFecha.ResumeLayout(false);
            gbFiltrarFecha.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvVentas).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btnRealizarVenta;
        private Panel panelSuperior;
        private Label lblFiltrar;
        private Button btnFiltrar;
        private TextBox txtFiltrar;
        private Button btnBorrarFiltros;
        private DataGridView dgvVentas;
        private Button btnSalir;
        private Button btnVerDetalles;
        private CheckBox checkFiltrarPorFecha;
        private GroupBox gbFiltrarFecha;
        private DateTimePicker dtpHasta;
        private Label label1;
        private DateTimePicker dtpDesde;
        private Label label2;
        private Button btnFiltrarFecha;
        private Button btnBorrarFiltrosFecha;
        private Button btnVerTicket;
    }
}