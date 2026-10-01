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
            dgvLibros = new DataGridView();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            numCantidad = new NumericUpDown();
            lblTitulo = new Label();
            lblPrecio = new Label();
            btnAgregarLibro = new Button();
            lblTotalNumero = new Label();
            lblTotal = new Label();
            btnEliminarLibro = new Button();
            btnGenerarVenta = new Button();
            dgvVentas = new DataGridView();
            lblFiltrar = new Label();
            btnFiltrarLibro = new Button();
            txtFiltrarLibro = new TextBox();
            btnBorrarFiltrosLibros = new Button();
            panelSuperior = new Panel();
            btnVolverGestionarVEN = new Button();
            lblPasoDos = new Label();
            lblPasoTres = new Label();
            btnFiltrarCliente = new Button();
            txtFiltrarCliente = new TextBox();
            btnBorrarFiltrosCliente = new Button();
            btnSeleccionarCliente = new Button();
            dgvClientes = new DataGridView();
            gbSeleccionarCliente = new GroupBox();
            label4 = new Label();
            lblPasoUno = new Label();
            lblClienteSeleccionado = new Label();
            lblNombreCliente = new Label();
            btnSelectOtroCliente = new Button();
            gbSeleccionarLibros = new GroupBox();
            ((System.ComponentModel.ISupportInitialize)dgvLibros).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCantidad).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvVentas).BeginInit();
            panelSuperior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvClientes).BeginInit();
            gbSeleccionarCliente.SuspendLayout();
            gbSeleccionarLibros.SuspendLayout();
            SuspendLayout();
            // 
            // dgvLibros
            // 
            dgvLibros.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLibros.Location = new Point(19, 165);
            dgvLibros.Name = "dgvLibros";
            dgvLibros.ReadOnly = true;
            dgvLibros.RowHeadersWidth = 51;
            dgvLibros.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLibros.Size = new Size(411, 217);
            dgvLibros.TabIndex = 0;
            dgvLibros.CellClick += dgvLibros_CellClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(436, 187);
            label1.Name = "label1";
            label1.Size = new Size(50, 20);
            label1.TabIndex = 2;
            label1.Tag = "BLANCO";
            label1.Text = "Título:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(436, 244);
            label2.Name = "label2";
            label2.Size = new Size(53, 20);
            label2.TabIndex = 3;
            label2.Tag = "BLANCO";
            label2.Text = "Precio:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(436, 298);
            label3.Name = "label3";
            label3.Size = new Size(72, 20);
            label3.TabIndex = 4;
            label3.Tag = "BLANCO";
            label3.Text = "Cantidad:";
            // 
            // numCantidad
            // 
            numCantidad.Location = new Point(549, 291);
            numCantidad.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numCantidad.Name = "numCantidad";
            numCantidad.Size = new Size(150, 27);
            numCantidad.TabIndex = 5;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Location = new Point(549, 187);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(0, 20);
            lblTitulo.TabIndex = 6;
            lblTitulo.Tag = "BLANCO";
            // 
            // lblPrecio
            // 
            lblPrecio.AutoSize = true;
            lblPrecio.Location = new Point(549, 244);
            lblPrecio.Name = "lblPrecio";
            lblPrecio.Size = new Size(0, 20);
            lblPrecio.TabIndex = 7;
            lblPrecio.Tag = "BLANCO";
            // 
            // btnAgregarLibro
            // 
            btnAgregarLibro.BackColor = SystemColors.Control;
            btnAgregarLibro.Location = new Point(503, 353);
            btnAgregarLibro.Name = "btnAgregarLibro";
            btnAgregarLibro.Size = new Size(120, 68);
            btnAgregarLibro.TabIndex = 8;
            btnAgregarLibro.Text = "Agregar libro a la factura";
            btnAgregarLibro.UseVisualStyleBackColor = false;
            btnAgregarLibro.Click += btnAgregarLibro_Click;
            // 
            // lblTotalNumero
            // 
            lblTotalNumero.AutoSize = true;
            lblTotalNumero.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTotalNumero.Location = new Point(628, 828);
            lblTotalNumero.Name = "lblTotalNumero";
            lblTotalNumero.Size = new Size(137, 41);
            lblTotalNumero.TabIndex = 10;
            lblTotalNumero.Text = "4000000";
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTotal.Location = new Point(521, 828);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(96, 41);
            lblTotal.TabIndex = 9;
            lblTotal.Text = "Total:";
            // 
            // btnEliminarLibro
            // 
            btnEliminarLibro.BackColor = SystemColors.Control;
            btnEliminarLibro.Location = new Point(521, 718);
            btnEliminarLibro.Name = "btnEliminarLibro";
            btnEliminarLibro.Size = new Size(131, 68);
            btnEliminarLibro.TabIndex = 11;
            btnEliminarLibro.Text = "Eliminar libro de la venta";
            btnEliminarLibro.UseVisualStyleBackColor = false;
            btnEliminarLibro.Click += btnEliminarLibro_Click;
            // 
            // btnGenerarVenta
            // 
            btnGenerarVenta.BackColor = SystemColors.Control;
            btnGenerarVenta.Location = new Point(521, 906);
            btnGenerarVenta.Name = "btnGenerarVenta";
            btnGenerarVenta.Size = new Size(120, 68);
            btnGenerarVenta.TabIndex = 12;
            btnGenerarVenta.Text = "Generar venta";
            btnGenerarVenta.UseVisualStyleBackColor = false;
            btnGenerarVenta.Click += btnGenerarVenta_Click;
            // 
            // dgvVentas
            // 
            dgvVentas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvVentas.Location = new Point(18, 718);
            dgvVentas.Name = "dgvVentas";
            dgvVentas.ReadOnly = true;
            dgvVentas.RowHeadersWidth = 51;
            dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvVentas.Size = new Size(472, 266);
            dgvVentas.TabIndex = 13;
            // 
            // lblFiltrar
            // 
            lblFiltrar.AutoSize = true;
            lblFiltrar.Location = new Point(21, 77);
            lblFiltrar.Name = "lblFiltrar";
            lblFiltrar.Size = new Size(169, 20);
            lblFiltrar.TabIndex = 17;
            lblFiltrar.Tag = "BLANCO";
            lblFiltrar.Text = "Filtrar por titulo de libro";
            // 
            // btnFiltrarLibro
            // 
            btnFiltrarLibro.BackColor = SystemColors.Control;
            btnFiltrarLibro.Location = new Point(317, 85);
            btnFiltrarLibro.Name = "btnFiltrarLibro";
            btnFiltrarLibro.Size = new Size(139, 48);
            btnFiltrarLibro.TabIndex = 16;
            btnFiltrarLibro.Text = "Filtrar";
            btnFiltrarLibro.UseVisualStyleBackColor = false;
            btnFiltrarLibro.Click += btnFiltrar_Click;
            // 
            // txtFiltrarLibro
            // 
            txtFiltrarLibro.Location = new Point(21, 106);
            txtFiltrarLibro.Name = "txtFiltrarLibro";
            txtFiltrarLibro.Size = new Size(280, 27);
            txtFiltrarLibro.TabIndex = 15;
            txtFiltrarLibro.TextChanged += txtFiltrar_TextChanged;
            // 
            // btnBorrarFiltrosLibros
            // 
            btnBorrarFiltrosLibros.BackColor = SystemColors.Control;
            btnBorrarFiltrosLibros.Location = new Point(462, 85);
            btnBorrarFiltrosLibros.Name = "btnBorrarFiltrosLibros";
            btnBorrarFiltrosLibros.Size = new Size(139, 48);
            btnBorrarFiltrosLibros.TabIndex = 14;
            btnBorrarFiltrosLibros.Text = "Borrar Filtros";
            btnBorrarFiltrosLibros.UseVisualStyleBackColor = false;
            btnBorrarFiltrosLibros.Click += btnBorrarFiltros_Click;
            // 
            // panelSuperior
            // 
            panelSuperior.Controls.Add(btnVolverGestionarVEN);
            panelSuperior.Dock = DockStyle.Top;
            panelSuperior.Location = new Point(0, 0);
            panelSuperior.Name = "panelSuperior";
            panelSuperior.Size = new Size(1488, 81);
            panelSuperior.TabIndex = 25;
            // 
            // btnVolverGestionarVEN
            // 
            btnVolverGestionarVEN.Anchor = AnchorStyles.Right;
            btnVolverGestionarVEN.Location = new Point(1251, 12);
            btnVolverGestionarVEN.Name = "btnVolverGestionarVEN";
            btnVolverGestionarVEN.Size = new Size(225, 60);
            btnVolverGestionarVEN.TabIndex = 8;
            btnVolverGestionarVEN.Text = "Volver al gestionar ventas";
            btnVolverGestionarVEN.UseVisualStyleBackColor = true;
            btnVolverGestionarVEN.Click += btnVolverGestionarVEN_Click;
            // 
            // lblPasoDos
            // 
            lblPasoDos.AutoSize = true;
            lblPasoDos.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblPasoDos.Location = new Point(6, 23);
            lblPasoDos.Name = "lblPasoDos";
            lblPasoDos.Size = new Size(304, 41);
            lblPasoDos.TabIndex = 26;
            lblPasoDos.Tag = "BLANCO";
            lblPasoDos.Text = "2. Seleccionar Libros";
            // 
            // lblPasoTres
            // 
            lblPasoTres.AutoSize = true;
            lblPasoTres.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblPasoTres.Location = new Point(14, 631);
            lblPasoTres.Name = "lblPasoTres";
            lblPasoTres.Size = new Size(812, 41);
            lblPasoTres.TabIndex = 27;
            lblPasoTres.Text = "2. Cuando la venta este lista, clickear en \"Generar Venta\"";
            // 
            // btnFiltrarCliente
            // 
            btnFiltrarCliente.BackColor = SystemColors.Control;
            btnFiltrarCliente.Location = new Point(242, 95);
            btnFiltrarCliente.Name = "btnFiltrarCliente";
            btnFiltrarCliente.Size = new Size(120, 67);
            btnFiltrarCliente.TabIndex = 43;
            btnFiltrarCliente.Text = "Filtrar";
            btnFiltrarCliente.UseVisualStyleBackColor = false;
            // 
            // txtFiltrarCliente
            // 
            txtFiltrarCliente.Location = new Point(6, 135);
            txtFiltrarCliente.Name = "txtFiltrarCliente";
            txtFiltrarCliente.Size = new Size(230, 27);
            txtFiltrarCliente.TabIndex = 42;
            // 
            // btnBorrarFiltrosCliente
            // 
            btnBorrarFiltrosCliente.BackColor = SystemColors.Control;
            btnBorrarFiltrosCliente.Location = new Point(368, 95);
            btnBorrarFiltrosCliente.Name = "btnBorrarFiltrosCliente";
            btnBorrarFiltrosCliente.Size = new Size(120, 67);
            btnBorrarFiltrosCliente.TabIndex = 41;
            btnBorrarFiltrosCliente.Text = "Borrar Filtros";
            btnBorrarFiltrosCliente.UseVisualStyleBackColor = false;
            // 
            // btnSeleccionarCliente
            // 
            btnSeleccionarCliente.BackColor = SystemColors.Control;
            btnSeleccionarCliente.Location = new Point(509, 229);
            btnSeleccionarCliente.Name = "btnSeleccionarCliente";
            btnSeleccionarCliente.Size = new Size(155, 68);
            btnSeleccionarCliente.TabIndex = 40;
            btnSeleccionarCliente.Text = "Seleccionar cliente";
            btnSeleccionarCliente.UseVisualStyleBackColor = false;
            btnSeleccionarCliente.Click += btnSeleccionarCliente_Click;
            // 
            // dgvClientes
            // 
            dgvClientes.BackgroundColor = SystemColors.Control;
            dgvClientes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvClientes.Location = new Point(0, 176);
            dgvClientes.Name = "dgvClientes";
            dgvClientes.ReadOnly = true;
            dgvClientes.RowHeadersWidth = 51;
            dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClientes.Size = new Size(472, 217);
            dgvClientes.TabIndex = 39;
            // 
            // gbSeleccionarCliente
            // 
            gbSeleccionarCliente.Controls.Add(label4);
            gbSeleccionarCliente.Controls.Add(lblPasoUno);
            gbSeleccionarCliente.Controls.Add(dgvClientes);
            gbSeleccionarCliente.Controls.Add(btnSeleccionarCliente);
            gbSeleccionarCliente.Controls.Add(btnBorrarFiltrosCliente);
            gbSeleccionarCliente.Controls.Add(btnFiltrarCliente);
            gbSeleccionarCliente.Controls.Add(txtFiltrarCliente);
            gbSeleccionarCliente.Location = new Point(12, 97);
            gbSeleccionarCliente.Name = "gbSeleccionarCliente";
            gbSeleccionarCliente.Size = new Size(700, 444);
            gbSeleccionarCliente.TabIndex = 46;
            gbSeleccionarCliente.TabStop = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 105);
            label4.Name = "label4";
            label4.Size = new Size(230, 20);
            label4.TabIndex = 49;
            label4.Tag = "BLANCO";
            label4.Text = "Filtrar por número de documento";
            // 
            // lblPasoUno
            // 
            lblPasoUno.AutoSize = true;
            lblPasoUno.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPasoUno.ForeColor = SystemColors.ControlText;
            lblPasoUno.Location = new Point(6, 33);
            lblPasoUno.Name = "lblPasoUno";
            lblPasoUno.Size = new Size(317, 41);
            lblPasoUno.TabIndex = 48;
            lblPasoUno.Tag = "BLANCO";
            lblPasoUno.Text = "1. Seleccionar Cliente";
            // 
            // lblClienteSeleccionado
            // 
            lblClienteSeleccionado.AutoSize = true;
            lblClienteSeleccionado.Location = new Point(14, 561);
            lblClienteSeleccionado.Name = "lblClienteSeleccionado";
            lblClienteSeleccionado.Size = new Size(149, 20);
            lblClienteSeleccionado.TabIndex = 46;
            lblClienteSeleccionado.Text = "Cliente seleccionado:";
            // 
            // lblNombreCliente
            // 
            lblNombreCliente.AutoSize = true;
            lblNombreCliente.Location = new Point(173, 561);
            lblNombreCliente.Name = "lblNombreCliente";
            lblNombreCliente.Size = new Size(77, 20);
            lblNombreCliente.TabIndex = 47;
            lblNombreCliente.Text = "Juan Perez";
            // 
            // btnSelectOtroCliente
            // 
            btnSelectOtroCliente.BackColor = SystemColors.Control;
            btnSelectOtroCliente.Location = new Point(271, 547);
            btnSelectOtroCliente.Name = "btnSelectOtroCliente";
            btnSelectOtroCliente.Size = new Size(229, 45);
            btnSelectOtroCliente.TabIndex = 48;
            btnSelectOtroCliente.Text = "Seleccionar otro cliente";
            btnSelectOtroCliente.UseVisualStyleBackColor = false;
            btnSelectOtroCliente.Click += btnSelectOtroCliente_Click;
            // 
            // gbSeleccionarLibros
            // 
            gbSeleccionarLibros.Controls.Add(lblPasoDos);
            gbSeleccionarLibros.Controls.Add(btnAgregarLibro);
            gbSeleccionarLibros.Controls.Add(label1);
            gbSeleccionarLibros.Controls.Add(dgvLibros);
            gbSeleccionarLibros.Controls.Add(label2);
            gbSeleccionarLibros.Controls.Add(txtFiltrarLibro);
            gbSeleccionarLibros.Controls.Add(label3);
            gbSeleccionarLibros.Controls.Add(numCantidad);
            gbSeleccionarLibros.Controls.Add(lblFiltrar);
            gbSeleccionarLibros.Controls.Add(btnFiltrarLibro);
            gbSeleccionarLibros.Controls.Add(btnBorrarFiltrosLibros);
            gbSeleccionarLibros.Controls.Add(lblTitulo);
            gbSeleccionarLibros.Controls.Add(lblPrecio);
            gbSeleccionarLibros.Location = new Point(730, 97);
            gbSeleccionarLibros.Name = "gbSeleccionarLibros";
            gbSeleccionarLibros.Size = new Size(746, 444);
            gbSeleccionarLibros.TabIndex = 49;
            gbSeleccionarLibros.TabStop = false;
            // 
            // FrmRealizarVenta
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1488, 1055);
            Controls.Add(gbSeleccionarLibros);
            Controls.Add(btnSelectOtroCliente);
            Controls.Add(lblNombreCliente);
            Controls.Add(lblClienteSeleccionado);
            Controls.Add(lblPasoTres);
            Controls.Add(panelSuperior);
            Controls.Add(dgvVentas);
            Controls.Add(btnGenerarVenta);
            Controls.Add(btnEliminarLibro);
            Controls.Add(lblTotalNumero);
            Controls.Add(lblTotal);
            Controls.Add(gbSeleccionarCliente);
            Name = "FrmRealizarVenta";
            Load += FrmRealizarVenta_Load;
            ((System.ComponentModel.ISupportInitialize)dgvLibros).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCantidad).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvVentas).EndInit();
            panelSuperior.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvClientes).EndInit();
            gbSeleccionarCliente.ResumeLayout(false);
            gbSeleccionarCliente.PerformLayout();
            gbSeleccionarLibros.ResumeLayout(false);
            gbSeleccionarLibros.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvLibros;
        private Label label1;
        private Label label2;
        private Label label3;
        private NumericUpDown numCantidad;
        private Label lblTitulo;
        private Label lblPrecio;
        private Button btnAgregarLibro;
        private Label lblTotalNumero;
        private Label lblTotal;
        private Button btnEliminarLibro;
        private Button btnGenerarVenta;
        private DataGridView dgvVentas;
        private Label lblFiltrar;
        private Button btnFiltrarLibro;
        private TextBox txtFiltrarLibro;
        private Button btnBorrarFiltrosLibros;
        private Panel panelSuperior;
        private Button btnVolverGestionarVEN;
        private Label lblPasoDos;
        private Label lblPasoTres;
        private Button btnFiltrarCliente;
        private TextBox txtFiltrarCliente;
        private Button btnBorrarFiltrosCliente;
        private Button btnSeleccionarCliente;
        private DataGridView dgvClientes;
        private GroupBox gbSeleccionarCliente;
        private Label lblClienteSeleccionado;
        private Label lblNombreCliente;
        private Button btnSelectOtroCliente;
        private Label lblPasoUno;
        private Label label4;
        private GroupBox gbSeleccionarLibros;
    }
}