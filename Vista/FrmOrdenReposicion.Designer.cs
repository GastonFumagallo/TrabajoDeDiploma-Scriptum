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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmOrdenReposicion));
            dgvLibrosBajoStock = new DataGridView();
            dgvProveedores = new DataGridView();
            panelSuperior = new Panel();
            pbLogo = new PictureBox();
            btnCancelar = new Button();
            lblTituloLibro = new Label();
            lblTituloProveedor = new Label();
            btnSeleccionarProveedor = new Button();
            lblLibro = new Label();
            lblProveedor = new Label();
            lblCantidad = new Label();
            numCantidad = new NumericUpDown();
            lblLibroSeleccionado = new Label();
            lblProveedorSeleccionado = new Label();
            btnSeleccionarLibro = new Button();
            lblPrecioNumero = new Label();
            lblPrecio = new Label();
            btnGenerarOrden = new Button();
            btnReiniciarLyP = new Button();
            dgvOrdenesReposicion = new DataGridView();
            lblTituloOrden = new Label();
            lblSeleccion = new Label();
            lblNoHayOrdenes = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvLibrosBajoStock).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvProveedores).BeginInit();
            panelSuperior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCantidad).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvOrdenesReposicion).BeginInit();
            SuspendLayout();
            // 
            // dgvLibrosBajoStock
            // 
            dgvLibrosBajoStock.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLibrosBajoStock.BackgroundColor = SystemColors.MenuBar;
            dgvLibrosBajoStock.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLibrosBajoStock.Location = new Point(12, 118);
            dgvLibrosBajoStock.Name = "dgvLibrosBajoStock";
            dgvLibrosBajoStock.ReadOnly = true;
            dgvLibrosBajoStock.RowHeadersWidth = 51;
            dgvLibrosBajoStock.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLibrosBajoStock.Size = new Size(427, 216);
            dgvLibrosBajoStock.TabIndex = 5;
            // 
            // dgvProveedores
            // 
            dgvProveedores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProveedores.BackgroundColor = SystemColors.MenuBar;
            dgvProveedores.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProveedores.Location = new Point(538, 118);
            dgvProveedores.Name = "dgvProveedores";
            dgvProveedores.ReadOnly = true;
            dgvProveedores.RowHeadersWidth = 51;
            dgvProveedores.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProveedores.Size = new Size(416, 216);
            dgvProveedores.TabIndex = 6;
            // 
            // panelSuperior
            // 
            panelSuperior.BackColor = SystemColors.Control;
            panelSuperior.Controls.Add(pbLogo);
            panelSuperior.Controls.Add(btnCancelar);
            panelSuperior.Dock = DockStyle.Top;
            panelSuperior.Location = new Point(0, 0);
            panelSuperior.Name = "panelSuperior";
            panelSuperior.Size = new Size(1017, 61);
            panelSuperior.TabIndex = 12;
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
            btnCancelar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancelar.Location = new Point(911, 12);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(94, 29);
            btnCancelar.TabIndex = 8;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // lblTituloLibro
            // 
            lblTituloLibro.AutoSize = true;
            lblTituloLibro.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTituloLibro.Location = new Point(158, 67);
            lblTituloLibro.Name = "lblTituloLibro";
            lblTituloLibro.Size = new Size(93, 31);
            lblTituloLibro.TabIndex = 13;
            lblTituloLibro.Text = "LIBROS";
            // 
            // lblTituloProveedor
            // 
            lblTituloProveedor.AutoSize = true;
            lblTituloProveedor.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTituloProveedor.Location = new Point(657, 67);
            lblTituloProveedor.Name = "lblTituloProveedor";
            lblTituloProveedor.Size = new Size(173, 31);
            lblTituloProveedor.TabIndex = 14;
            lblTituloProveedor.Text = "PROVEEDORES";
            // 
            // btnSeleccionarProveedor
            // 
            btnSeleccionarProveedor.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSeleccionarProveedor.Location = new Point(649, 340);
            btnSeleccionarProveedor.Name = "btnSeleccionarProveedor";
            btnSeleccionarProveedor.Size = new Size(219, 47);
            btnSeleccionarProveedor.TabIndex = 11;
            btnSeleccionarProveedor.Text = "Seleccionar Proveedor";
            btnSeleccionarProveedor.UseVisualStyleBackColor = true;
            btnSeleccionarProveedor.Click += btnSeleccionarLyP_Click;
            // 
            // lblLibro
            // 
            lblLibro.AutoSize = true;
            lblLibro.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLibro.Location = new Point(328, 501);
            lblLibro.Name = "lblLibro";
            lblLibro.Size = new Size(49, 20);
            lblLibro.TabIndex = 15;
            lblLibro.Text = "Libro:";
            // 
            // lblProveedor
            // 
            lblProveedor.AutoSize = true;
            lblProveedor.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblProveedor.Location = new Point(557, 501);
            lblProveedor.Name = "lblProveedor";
            lblProveedor.Size = new Size(86, 20);
            lblProveedor.TabIndex = 16;
            lblProveedor.Text = "Proveedor:";
            // 
            // lblCantidad
            // 
            lblCantidad.AutoSize = true;
            lblCantidad.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCantidad.Location = new Point(557, 545);
            lblCantidad.Name = "lblCantidad";
            lblCantidad.Size = new Size(75, 20);
            lblCantidad.TabIndex = 17;
            lblCantidad.Text = "Cantidad:";
            // 
            // numCantidad
            // 
            numCantidad.Location = new Point(661, 538);
            numCantidad.Name = "numCantidad";
            numCantidad.Size = new Size(150, 27);
            numCantidad.TabIndex = 18;
            // 
            // lblLibroSeleccionado
            // 
            lblLibroSeleccionado.AutoSize = true;
            lblLibroSeleccionado.Location = new Point(389, 501);
            lblLibroSeleccionado.Name = "lblLibroSeleccionado";
            lblLibroSeleccionado.Size = new Size(149, 20);
            lblLibroSeleccionado.TabIndex = 19;
            lblLibroSeleccionado.Text = "lblLibroSeleccionado";
            // 
            // lblProveedorSeleccionado
            // 
            lblProveedorSeleccionado.AutoSize = true;
            lblProveedorSeleccionado.Location = new Point(661, 501);
            lblProveedorSeleccionado.Name = "lblProveedorSeleccionado";
            lblProveedorSeleccionado.Size = new Size(166, 20);
            lblProveedorSeleccionado.TabIndex = 20;
            lblProveedorSeleccionado.Text = "ProveedorSeleccionado";
            // 
            // btnSeleccionarLibro
            // 
            btnSeleccionarLibro.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSeleccionarLibro.Location = new Point(118, 340);
            btnSeleccionarLibro.Name = "btnSeleccionarLibro";
            btnSeleccionarLibro.Size = new Size(201, 47);
            btnSeleccionarLibro.TabIndex = 21;
            btnSeleccionarLibro.Text = "Seleccionar Libro";
            btnSeleccionarLibro.UseVisualStyleBackColor = true;
            btnSeleccionarLibro.Click += btnSeleccionarLibro_Click;
            // 
            // lblPrecioNumero
            // 
            lblPrecioNumero.AutoSize = true;
            lblPrecioNumero.Location = new Point(398, 545);
            lblPrecioNumero.Name = "lblPrecioNumero";
            lblPrecioNumero.Size = new Size(65, 20);
            lblPrecioNumero.TabIndex = 23;
            lblPrecioNumero.Text = "4434343";
            // 
            // lblPrecio
            // 
            lblPrecio.AutoSize = true;
            lblPrecio.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPrecio.Location = new Point(328, 545);
            lblPrecio.Name = "lblPrecio";
            lblPrecio.Size = new Size(56, 20);
            lblPrecio.TabIndex = 22;
            lblPrecio.Text = "Precio:";
            // 
            // btnGenerarOrden
            // 
            btnGenerarOrden.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGenerarOrden.Location = new Point(405, 598);
            btnGenerarOrden.Name = "btnGenerarOrden";
            btnGenerarOrden.Size = new Size(261, 47);
            btnGenerarOrden.TabIndex = 24;
            btnGenerarOrden.Text = "Generar orden de reposición";
            btnGenerarOrden.UseVisualStyleBackColor = true;
            btnGenerarOrden.Click += btnGenerarOrden_Click;
            // 
            // btnReiniciarLyP
            // 
            btnReiniciarLyP.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnReiniciarLyP.Location = new Point(380, 409);
            btnReiniciarLyP.Name = "btnReiniciarLyP";
            btnReiniciarLyP.Size = new Size(261, 47);
            btnReiniciarLyP.TabIndex = 25;
            btnReiniciarLyP.Text = "Reiniciar Libros y Proveedores";
            btnReiniciarLyP.UseVisualStyleBackColor = true;
            btnReiniciarLyP.Click += btnReiniciarLyP_Click;
            // 
            // dgvOrdenesReposicion
            // 
            dgvOrdenesReposicion.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvOrdenesReposicion.BackgroundColor = SystemColors.MenuBar;
            dgvOrdenesReposicion.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvOrdenesReposicion.Location = new Point(24, 707);
            dgvOrdenesReposicion.Name = "dgvOrdenesReposicion";
            dgvOrdenesReposicion.ReadOnly = true;
            dgvOrdenesReposicion.RowHeadersWidth = 51;
            dgvOrdenesReposicion.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOrdenesReposicion.Size = new Size(942, 231);
            dgvOrdenesReposicion.TabIndex = 26;
            // 
            // lblTituloOrden
            // 
            lblTituloOrden.AutoSize = true;
            lblTituloOrden.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTituloOrden.Location = new Point(364, 662);
            lblTituloOrden.Name = "lblTituloOrden";
            lblTituloOrden.Size = new Size(293, 31);
            lblTituloOrden.TabIndex = 27;
            lblTituloOrden.Text = "ORDENES DE REPOSICION";
            // 
            // lblSeleccion
            // 
            lblSeleccion.AutoSize = true;
            lblSeleccion.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSeleccion.Location = new Point(36, 516);
            lblSeleccion.Name = "lblSeleccion";
            lblSeleccion.Size = new Size(269, 31);
            lblSeleccion.TabIndex = 28;
            lblSeleccion.Text = "SELECCIÓN REALIZADA:";
            // 
            // lblNoHayOrdenes
            // 
            lblNoHayOrdenes.AutoSize = true;
            lblNoHayOrdenes.Font = new Font("Segoe UI Semibold", 13.8F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            lblNoHayOrdenes.Location = new Point(376, 779);
            lblNoHayOrdenes.Name = "lblNoHayOrdenes";
            lblNoHayOrdenes.Size = new Size(256, 31);
            lblNoHayOrdenes.TabIndex = 29;
            lblNoHayOrdenes.Text = "No hay ordenes activas";
            // 
            // FrmOrdenReposicion
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1017, 970);
            Controls.Add(lblSeleccion);
            Controls.Add(lblTituloOrden);
            Controls.Add(dgvOrdenesReposicion);
            Controls.Add(btnReiniciarLyP);
            Controls.Add(btnGenerarOrden);
            Controls.Add(lblPrecioNumero);
            Controls.Add(lblPrecio);
            Controls.Add(btnSeleccionarLibro);
            Controls.Add(lblProveedorSeleccionado);
            Controls.Add(lblLibroSeleccionado);
            Controls.Add(numCantidad);
            Controls.Add(lblCantidad);
            Controls.Add(lblProveedor);
            Controls.Add(lblLibro);
            Controls.Add(btnSeleccionarProveedor);
            Controls.Add(lblTituloProveedor);
            Controls.Add(lblTituloLibro);
            Controls.Add(panelSuperior);
            Controls.Add(dgvProveedores);
            Controls.Add(dgvLibrosBajoStock);
            Controls.Add(lblNoHayOrdenes);
            Name = "FrmOrdenReposicion";
            Load += FrmOrdenReposicion_Load;
            ((System.ComponentModel.ISupportInitialize)dgvLibrosBajoStock).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvProveedores).EndInit();
            panelSuperior.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCantidad).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvOrdenesReposicion).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvLibrosBajoStock;
        private DataGridView dgvProveedores;
        private Panel panelSuperior;
        private PictureBox pbLogo;
        private Button btnCancelar;
        private Label lblTituloLibro;
        private Label lblTituloProveedor;
        private Button btnSeleccionarProveedor;
        private Label lblLibro;
        private Label lblProveedor;
        private Label lblCantidad;
        private NumericUpDown numCantidad;
        private Label lblLibroSeleccionado;
        private Label lblProveedorSeleccionado;
        private Button btnSeleccionarLibro;
        private Label lblPrecioNumero;
        private Label lblPrecio;
        private Button btnGenerarOrden;
        private Button btnReiniciarLyP;
        private DataGridView dgvOrdenesReposicion;
        private Label lblTituloOrden;
        private Label lblSeleccion;
        private Label lblNoHayOrdenes;
    }
}