namespace Vista
{
    partial class FrmGestionarInventario
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
            checkReposicion = new CheckBox();
            panelSuperior = new Panel();
            btnOrdenReposicion = new Button();
            btnVerProveedores = new Button();
            lblTotalLibros = new Label();
            lblDisponibles = new Label();
            lblReposicion = new Label();
            panelInventario = new Panel();
            btnSalir = new Button();
            dgvOrdenes = new DataGridView();
            btnRegistrarRecepcion = new Button();
            label1 = new Label();
            btnCancelarOrden = new Button();
            label2 = new Label();
            label3 = new Label();
            dgvOrdenesHistoricas = new DataGridView();
            dgvProveedoresLibro = new DataGridView();
            lblTituloProveedores = new Label();
            lblOrdenesActivas = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvLibros).BeginInit();
            panelSuperior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvOrdenes).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvOrdenesHistoricas).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvProveedoresLibro).BeginInit();
            SuspendLayout();
            // 
            // dgvLibros
            // 
            dgvLibros.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLibros.BackgroundColor = SystemColors.MenuBar;
            dgvLibros.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLibros.Location = new Point(12, 170);
            dgvLibros.Name = "dgvLibros";
            dgvLibros.ReadOnly = true;
            dgvLibros.RowHeadersWidth = 51;
            dgvLibros.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLibros.Size = new Size(686, 340);
            dgvLibros.TabIndex = 3;
            dgvLibros.CellClick += dgvLibros_CellClick;
            dgvLibros.CellFormatting += dgvLibros_CellFormatting;
            dgvLibros.DataBindingComplete += dgvLibros_DataBindingComplete;
            // 
            // checkReposicion
            // 
            checkReposicion.AutoSize = true;
            checkReposicion.BackColor = Color.Transparent;
            checkReposicion.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkReposicion.ForeColor = SystemColors.Control;
            checkReposicion.Location = new Point(12, 29);
            checkReposicion.Name = "checkReposicion";
            checkReposicion.Size = new Size(427, 27);
            checkReposicion.TabIndex = 7;
            checkReposicion.Tag = "CheckInventario";
            checkReposicion.Text = "Mostrar únicamente libros que requieren reposición";
            checkReposicion.UseVisualStyleBackColor = false;
            checkReposicion.CheckedChanged += checkReposicion_CheckedChanged;
            // 
            // panelSuperior
            // 
            panelSuperior.Controls.Add(btnOrdenReposicion);
            panelSuperior.Controls.Add(checkReposicion);
            panelSuperior.Controls.Add(btnVerProveedores);
            panelSuperior.Dock = DockStyle.Top;
            panelSuperior.Location = new Point(0, 0);
            panelSuperior.Name = "panelSuperior";
            panelSuperior.Size = new Size(1525, 91);
            panelSuperior.TabIndex = 8;
            // 
            // btnOrdenReposicion
            // 
            btnOrdenReposicion.Anchor = AnchorStyles.Right;
            btnOrdenReposicion.Location = new Point(1385, 12);
            btnOrdenReposicion.Name = "btnOrdenReposicion";
            btnOrdenReposicion.Size = new Size(128, 64);
            btnOrdenReposicion.TabIndex = 9;
            btnOrdenReposicion.Text = "Generar orden de reposicion";
            btnOrdenReposicion.UseVisualStyleBackColor = true;
            btnOrdenReposicion.Click += btnOrdenReposicion_Click;
            // 
            // btnVerProveedores
            // 
            btnVerProveedores.Location = new Point(464, 10);
            btnVerProveedores.Name = "btnVerProveedores";
            btnVerProveedores.Size = new Size(119, 64);
            btnVerProveedores.TabIndex = 8;
            btnVerProveedores.Text = "Ver Proveedores";
            btnVerProveedores.UseVisualStyleBackColor = true;
            btnVerProveedores.Click += btnVerProveedores_Click;
            // 
            // lblTotalLibros
            // 
            lblTotalLibros.AutoSize = true;
            lblTotalLibros.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalLibros.Location = new Point(12, 109);
            lblTotalLibros.Name = "lblTotalLibros";
            lblTotalLibros.Size = new Size(210, 38);
            lblTotalLibros.TabIndex = 9;
            lblTotalLibros.Text = "Total de libros:";
            // 
            // lblDisponibles
            // 
            lblDisponibles.AutoSize = true;
            lblDisponibles.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDisponibles.Location = new Point(336, 109);
            lblDisponibles.Name = "lblDisponibles";
            lblDisponibles.Size = new Size(185, 38);
            lblDisponibles.TabIndex = 10;
            lblDisponibles.Text = "Disponibles: ";
            // 
            // lblReposicion
            // 
            lblReposicion.AutoSize = true;
            lblReposicion.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblReposicion.Location = new Point(615, 109);
            lblReposicion.Name = "lblReposicion";
            lblReposicion.Size = new Size(347, 38);
            lblReposicion.TabIndex = 11;
            lblReposicion.Text = "⚠️ Requieren reposición:";
            // 
            // panelInventario
            // 
            panelInventario.Location = new Point(1146, 170);
            panelInventario.Name = "panelInventario";
            panelInventario.Size = new Size(341, 340);
            panelInventario.TabIndex = 12;
            // 
            // btnSalir
            // 
            btnSalir.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSalir.Location = new Point(1385, 979);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(128, 64);
            btnSalir.TabIndex = 10;
            btnSalir.Text = "Volver";
            btnSalir.UseVisualStyleBackColor = true;
            btnSalir.Click += btnSalir_Click;
            // 
            // dgvOrdenes
            // 
            dgvOrdenes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvOrdenes.BackgroundColor = SystemColors.MenuBar;
            dgvOrdenes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvOrdenes.Location = new Point(636, 645);
            dgvOrdenes.Name = "dgvOrdenes";
            dgvOrdenes.ReadOnly = true;
            dgvOrdenes.RowHeadersWidth = 51;
            dgvOrdenes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOrdenes.Size = new Size(557, 386);
            dgvOrdenes.TabIndex = 13;
            // 
            // btnRegistrarRecepcion
            // 
            btnRegistrarRecepcion.Location = new Point(1243, 698);
            btnRegistrarRecepcion.Name = "btnRegistrarRecepcion";
            btnRegistrarRecepcion.Size = new Size(157, 91);
            btnRegistrarRecepcion.TabIndex = 14;
            btnRegistrarRecepcion.Text = "Registrar recepcion";
            btnRegistrarRecepcion.UseVisualStyleBackColor = true;
            btnRegistrarRecepcion.Click += btnRegistrarRecepcion_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(455, 538);
            label1.Name = "label1";
            label1.Size = new Size(316, 38);
            label1.TabIndex = 15;
            label1.Text = "Ordenes de reposición:";
            // 
            // btnCancelarOrden
            // 
            btnCancelarOrden.Location = new Point(1243, 819);
            btnCancelarOrden.Name = "btnCancelarOrden";
            btnCancelarOrden.Size = new Size(157, 91);
            btnCancelarOrden.TabIndex = 16;
            btnCancelarOrden.Text = "Cancelar Orden";
            btnCancelarOrden.UseVisualStyleBackColor = true;
            btnCancelarOrden.Click += btnCancelarOrden_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(857, 595);
            label2.Name = "label2";
            label2.Size = new Size(119, 38);
            label2.TabIndex = 17;
            label2.Text = "Activas:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(209, 595);
            label3.Name = "label3";
            label3.Size = new Size(153, 38);
            label3.TabIndex = 19;
            label3.Text = "Historicas:";
            // 
            // dgvOrdenesHistoricas
            // 
            dgvOrdenesHistoricas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvOrdenesHistoricas.BackgroundColor = SystemColors.MenuBar;
            dgvOrdenesHistoricas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvOrdenesHistoricas.Location = new Point(27, 645);
            dgvOrdenesHistoricas.Name = "dgvOrdenesHistoricas";
            dgvOrdenesHistoricas.ReadOnly = true;
            dgvOrdenesHistoricas.RowHeadersWidth = 51;
            dgvOrdenesHistoricas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOrdenesHistoricas.Size = new Size(556, 386);
            dgvOrdenesHistoricas.TabIndex = 18;
            // 
            // dgvProveedoresLibro
            // 
            dgvProveedoresLibro.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProveedoresLibro.BackgroundColor = SystemColors.MenuBar;
            dgvProveedoresLibro.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProveedoresLibro.Location = new Point(717, 211);
            dgvProveedoresLibro.Name = "dgvProveedoresLibro";
            dgvProveedoresLibro.ReadOnly = true;
            dgvProveedoresLibro.RowHeadersWidth = 51;
            dgvProveedoresLibro.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProveedoresLibro.Size = new Size(372, 272);
            dgvProveedoresLibro.TabIndex = 20;
            // 
            // lblTituloProveedores
            // 
            lblTituloProveedores.AutoSize = true;
            lblTituloProveedores.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTituloProveedores.Location = new Point(815, 170);
            lblTituloProveedores.Name = "lblTituloProveedores";
            lblTituloProveedores.Size = new Size(187, 38);
            lblTituloProveedores.TabIndex = 21;
            lblTituloProveedores.Text = "Proveedores:";
            // 
            // lblOrdenesActivas
            // 
            lblOrdenesActivas.AutoSize = true;
            lblOrdenesActivas.Font = new Font("Segoe UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblOrdenesActivas.Location = new Point(727, 724);
            lblOrdenesActivas.Name = "lblOrdenesActivas";
            lblOrdenesActivas.Size = new Size(399, 31);
            lblOrdenesActivas.TabIndex = 22;
            lblOrdenesActivas.Text = "No hay ordenes de reposicion activas.";
            // 
            // FrmGestionarInventario
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1525, 1055);
            Controls.Add(lblTituloProveedores);
            Controls.Add(dgvProveedoresLibro);
            Controls.Add(label3);
            Controls.Add(dgvOrdenesHistoricas);
            Controls.Add(label2);
            Controls.Add(btnCancelarOrden);
            Controls.Add(label1);
            Controls.Add(btnRegistrarRecepcion);
            Controls.Add(dgvOrdenes);
            Controls.Add(btnSalir);
            Controls.Add(panelInventario);
            Controls.Add(lblReposicion);
            Controls.Add(lblDisponibles);
            Controls.Add(lblTotalLibros);
            Controls.Add(panelSuperior);
            Controls.Add(dgvLibros);
            Controls.Add(lblOrdenesActivas);
            Name = "FrmGestionarInventario";
            Text = "INVENTARIO";
            Load += FrmGestionarInventario_Load;
            ((System.ComponentModel.ISupportInitialize)dgvLibros).EndInit();
            panelSuperior.ResumeLayout(false);
            panelSuperior.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvOrdenes).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvOrdenesHistoricas).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvProveedoresLibro).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvLibros;
        private CheckBox checkReposicion;
        private Panel panelSuperior;
        private Button btnVerProveedores;
        private Label lblTotalLibros;
        private Label lblDisponibles;
        private Label lblReposicion;
        private Panel panelInventario;
        private Button btnOrdenReposicion;
        private Button btnSalir;
        private DataGridView dgvOrdenes;
        private Button btnRegistrarRecepcion;
        private Label label1;
        private Button btnCancelarOrden;
        private Label label2;
        private Label label3;
        private DataGridView dgvOrdenesHistoricas;
        private DataGridView dgvProveedoresLibro;
        private Label lblTituloProveedores;
        private Label lblOrdenesActivas;
    }
}