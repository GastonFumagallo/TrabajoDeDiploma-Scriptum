namespace Vista
{
    partial class FrmConcretarVenta
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmConcretarVenta));
            dgvClientes = new DataGridView();
            btnSeleccionarCliente = new Button();
            panelSuperior = new Panel();
            pbLogo = new PictureBox();
            btnCancelar = new Button();
            btnMetodosPago = new Button();
            cbMetodoPago = new ComboBox();
            lblMP = new Label();
            btnGenerarVenta = new Button();
            label2 = new Label();
            lblPrecioNumero = new Label();
            lblClienteSeleccionado = new Label();
            lblNombreSeleccionado = new Label();
            lblFiltrar = new Label();
            btnFiltrar = new Button();
            txtFiltrar = new TextBox();
            btnBorrarFiltros = new Button();
            lblPasoCuatro = new Label();
            lblPasoTres = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvClientes).BeginInit();
            panelSuperior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbLogo).BeginInit();
            SuspendLayout();
            // 
            // dgvClientes
            // 
            dgvClientes.BackgroundColor = SystemColors.Control;
            dgvClientes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvClientes.Location = new Point(12, 229);
            dgvClientes.Name = "dgvClientes";
            dgvClientes.ReadOnly = true;
            dgvClientes.RowHeadersWidth = 51;
            dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClientes.Size = new Size(472, 217);
            dgvClientes.TabIndex = 2;
            // 
            // btnSeleccionarCliente
            // 
            btnSeleccionarCliente.BackColor = SystemColors.Control;
            btnSeleccionarCliente.Location = new Point(526, 306);
            btnSeleccionarCliente.Name = "btnSeleccionarCliente";
            btnSeleccionarCliente.Size = new Size(155, 68);
            btnSeleccionarCliente.TabIndex = 19;
            btnSeleccionarCliente.Text = "Seleccionar cliente";
            btnSeleccionarCliente.UseVisualStyleBackColor = false;
            btnSeleccionarCliente.Click += btnSeleccionarCliente_Click;
            // 
            // panelSuperior
            // 
            panelSuperior.Controls.Add(pbLogo);
            panelSuperior.Controls.Add(btnCancelar);
            panelSuperior.Dock = DockStyle.Top;
            panelSuperior.Location = new Point(0, 0);
            panelSuperior.Name = "panelSuperior";
            panelSuperior.Size = new Size(886, 61);
            panelSuperior.TabIndex = 24;
            // 
            // pbLogo
            // 
            pbLogo.Image = (Image)resources.GetObject("pbLogo.Image");
            pbLogo.Location = new Point(0, -1);
            pbLogo.Name = "pbLogo";
            pbLogo.Size = new Size(125, 62);
            pbLogo.SizeMode = PictureBoxSizeMode.Zoom;
            pbLogo.TabIndex = 10;
            pbLogo.TabStop = false;
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(780, 12);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(94, 29);
            btnCancelar.TabIndex = 8;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // btnMetodosPago
            // 
            btnMetodosPago.BackColor = SystemColors.Control;
            btnMetodosPago.Location = new Point(598, 650);
            btnMetodosPago.Name = "btnMetodosPago";
            btnMetodosPago.Size = new Size(155, 68);
            btnMetodosPago.TabIndex = 26;
            btnMetodosPago.Text = "Gestionar Metodos de Pago";
            btnMetodosPago.UseVisualStyleBackColor = false;
            btnMetodosPago.Click += btnMetodosPago_Click;
            // 
            // cbMetodoPago
            // 
            cbMetodoPago.FormattingEnabled = true;
            cbMetodoPago.Location = new Point(234, 676);
            cbMetodoPago.Name = "cbMetodoPago";
            cbMetodoPago.Size = new Size(322, 28);
            cbMetodoPago.TabIndex = 25;
            // 
            // lblMP
            // 
            lblMP.AutoSize = true;
            lblMP.Location = new Point(84, 679);
            lblMP.Name = "lblMP";
            lblMP.Size = new Size(125, 20);
            lblMP.TabIndex = 27;
            lblMP.Text = "Metodo de pago:";
            // 
            // btnGenerarVenta
            // 
            btnGenerarVenta.BackColor = SystemColors.Control;
            btnGenerarVenta.Location = new Point(313, 717);
            btnGenerarVenta.Name = "btnGenerarVenta";
            btnGenerarVenta.Size = new Size(120, 68);
            btnGenerarVenta.TabIndex = 28;
            btnGenerarVenta.Text = "Generar Venta";
            btnGenerarVenta.UseVisualStyleBackColor = false;
            btnGenerarVenta.Click += btnGenerarVenta_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label2.Location = new Point(15, 514);
            label2.Name = "label2";
            label2.Size = new Size(76, 28);
            label2.TabIndex = 29;
            label2.Text = "Precio:";
            // 
            // lblPrecioNumero
            // 
            lblPrecioNumero.AutoSize = true;
            lblPrecioNumero.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblPrecioNumero.Location = new Point(113, 514);
            lblPrecioNumero.Name = "lblPrecioNumero";
            lblPrecioNumero.Size = new Size(60, 28);
            lblPrecioNumero.TabIndex = 30;
            lblPrecioNumero.Text = "3333";
            // 
            // lblClienteSeleccionado
            // 
            lblClienteSeleccionado.AutoSize = true;
            lblClienteSeleccionado.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblClienteSeleccionado.Location = new Point(12, 468);
            lblClienteSeleccionado.Name = "lblClienteSeleccionado";
            lblClienteSeleccionado.Size = new Size(211, 28);
            lblClienteSeleccionado.TabIndex = 31;
            lblClienteSeleccionado.Text = "Cliente seleccionado:";
            // 
            // lblNombreSeleccionado
            // 
            lblNombreSeleccionado.AutoSize = true;
            lblNombreSeleccionado.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblNombreSeleccionado.Location = new Point(229, 468);
            lblNombreSeleccionado.Name = "lblNombreSeleccionado";
            lblNombreSeleccionado.Size = new Size(99, 28);
            lblNombreSeleccionado.TabIndex = 32;
            lblNombreSeleccionado.Text = "El tongas";
            // 
            // lblFiltrar
            // 
            lblFiltrar.AutoSize = true;
            lblFiltrar.Location = new Point(14, 136);
            lblFiltrar.Name = "lblFiltrar";
            lblFiltrar.Size = new Size(230, 20);
            lblFiltrar.TabIndex = 36;
            lblFiltrar.Text = "Filtrar por número de documento";
            // 
            // btnFiltrar
            // 
            btnFiltrar.BackColor = SystemColors.Control;
            btnFiltrar.Location = new Point(250, 131);
            btnFiltrar.Name = "btnFiltrar";
            btnFiltrar.Size = new Size(120, 67);
            btnFiltrar.TabIndex = 35;
            btnFiltrar.Text = "Filtrar";
            btnFiltrar.UseVisualStyleBackColor = false;
            btnFiltrar.Click += btnFiltrar_Click;
            // 
            // txtFiltrar
            // 
            txtFiltrar.Location = new Point(14, 171);
            txtFiltrar.Name = "txtFiltrar";
            txtFiltrar.Size = new Size(230, 27);
            txtFiltrar.TabIndex = 34;
            txtFiltrar.TextChanged += txtFiltrar_TextChanged;
            // 
            // btnBorrarFiltros
            // 
            btnBorrarFiltros.BackColor = SystemColors.Control;
            btnBorrarFiltros.Location = new Point(376, 131);
            btnBorrarFiltros.Name = "btnBorrarFiltros";
            btnBorrarFiltros.Size = new Size(120, 67);
            btnBorrarFiltros.TabIndex = 33;
            btnBorrarFiltros.Text = "Borrar Filtros";
            btnBorrarFiltros.UseVisualStyleBackColor = false;
            btnBorrarFiltros.Click += btnBorrarFiltros_Click;
            // 
            // lblPasoCuatro
            // 
            lblPasoCuatro.AutoSize = true;
            lblPasoCuatro.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPasoCuatro.Location = new Point(14, 591);
            lblPasoCuatro.Name = "lblPasoCuatro";
            lblPasoCuatro.Size = new Size(869, 41);
            lblPasoCuatro.TabIndex = 37;
            lblPasoCuatro.Text = "4. Seleccionar método de pago y clickear en \"Generar Venta\"";
            // 
            // lblPasoTres
            // 
            lblPasoTres.AutoSize = true;
            lblPasoTres.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold);
            lblPasoTres.Location = new Point(12, 74);
            lblPasoTres.Name = "lblPasoTres";
            lblPasoTres.Size = new Size(296, 38);
            lblPasoTres.TabIndex = 38;
            lblPasoTres.Text = "3. Seleccionar Cliente";
            // 
            // FrmConcretarVenta
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(886, 820);
            Controls.Add(lblPasoTres);
            Controls.Add(lblPasoCuatro);
            Controls.Add(lblFiltrar);
            Controls.Add(btnFiltrar);
            Controls.Add(txtFiltrar);
            Controls.Add(btnBorrarFiltros);
            Controls.Add(lblNombreSeleccionado);
            Controls.Add(lblClienteSeleccionado);
            Controls.Add(lblPrecioNumero);
            Controls.Add(label2);
            Controls.Add(btnGenerarVenta);
            Controls.Add(lblMP);
            Controls.Add(btnMetodosPago);
            Controls.Add(cbMetodoPago);
            Controls.Add(panelSuperior);
            Controls.Add(btnSeleccionarCliente);
            Controls.Add(dgvClientes);
            Name = "FrmConcretarVenta";
            Load += FrmConcretarVenta_Load;
            ((System.ComponentModel.ISupportInitialize)dgvClientes).EndInit();
            panelSuperior.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvClientes;
        private Button btnSeleccionarCliente;
        private Panel panelSuperior;
        private PictureBox pbLogo;
        private Button btnCancelar;
        private Button btnMetodosPago;
        private ComboBox cbMetodoPago;
        private Label lblMP;
        private Button btnGenerarVenta;
        private Label label2;
        private Label lblPrecioNumero;
        private Label lblClienteSeleccionado;
        private Label lblNombreSeleccionado;
        private Label lblFiltrar;
        private Button btnFiltrar;
        private TextBox txtFiltrar;
        private Button btnBorrarFiltros;
        private Label lblPasoCuatro;
        private Label lblPasoTres;
    }
}