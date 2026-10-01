namespace Vista
{
    partial class FrmGestionarClientes
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
            panelSuperior = new Panel();
            lblFiltrar = new Label();
            btnFiltrar = new Button();
            txtFiltrar = new TextBox();
            btnBorrarFiltros = new Button();
            btnEliminar = new Button();
            btnModificar = new Button();
            btnAgregar = new Button();
            dgvClientes = new DataGridView();
            btnSalir = new Button();
            panelSuperior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvClientes).BeginInit();
            SuspendLayout();
            // 
            // panelSuperior
            // 
            panelSuperior.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelSuperior.BackColor = SystemColors.ControlLightLight;
            panelSuperior.Controls.Add(lblFiltrar);
            panelSuperior.Controls.Add(btnFiltrar);
            panelSuperior.Controls.Add(txtFiltrar);
            panelSuperior.Controls.Add(btnBorrarFiltros);
            panelSuperior.Controls.Add(btnEliminar);
            panelSuperior.Controls.Add(btnModificar);
            panelSuperior.Controls.Add(btnAgregar);
            panelSuperior.Location = new Point(0, 0);
            panelSuperior.Name = "panelSuperior";
            panelSuperior.Size = new Size(1427, 82);
            panelSuperior.TabIndex = 0;
            // 
            // lblFiltrar
            // 
            lblFiltrar.Anchor = AnchorStyles.Right;
            lblFiltrar.AutoSize = true;
            lblFiltrar.Location = new Point(1048, 9);
            lblFiltrar.Name = "lblFiltrar";
            lblFiltrar.Size = new Size(230, 20);
            lblFiltrar.TabIndex = 6;
            lblFiltrar.Tag = "BLANCO";
            lblFiltrar.Text = "Filtrar por número de documento";
            // 
            // btnFiltrar
            // 
            btnFiltrar.Anchor = AnchorStyles.Right;
            btnFiltrar.BackColor = SystemColors.Control;
            btnFiltrar.Location = new Point(1284, 3);
            btnFiltrar.Name = "btnFiltrar";
            btnFiltrar.Size = new Size(120, 68);
            btnFiltrar.TabIndex = 5;
            btnFiltrar.Text = "Filtrar";
            btnFiltrar.UseVisualStyleBackColor = false;
            btnFiltrar.Click += btnFiltrar_Click;
            // 
            // txtFiltrar
            // 
            txtFiltrar.Anchor = AnchorStyles.Right;
            txtFiltrar.Location = new Point(1048, 44);
            txtFiltrar.Name = "txtFiltrar";
            txtFiltrar.Size = new Size(230, 27);
            txtFiltrar.TabIndex = 4;
            txtFiltrar.TextChanged += txtFiltrar_TextChanged;
            // 
            // btnBorrarFiltros
            // 
            btnBorrarFiltros.Anchor = AnchorStyles.Right;
            btnBorrarFiltros.BackColor = SystemColors.Control;
            btnBorrarFiltros.Location = new Point(922, 3);
            btnBorrarFiltros.Name = "btnBorrarFiltros";
            btnBorrarFiltros.Size = new Size(120, 68);
            btnBorrarFiltros.TabIndex = 3;
            btnBorrarFiltros.Text = "Borrar Filtros";
            btnBorrarFiltros.UseVisualStyleBackColor = false;
            btnBorrarFiltros.Click += btnBorrarFiltros_Click;
            // 
            // btnEliminar
            // 
            btnEliminar.BackColor = SystemColors.Control;
            btnEliminar.Location = new Point(264, 3);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(120, 68);
            btnEliminar.TabIndex = 2;
            btnEliminar.Text = "Eliminar";
            btnEliminar.UseVisualStyleBackColor = false;
            btnEliminar.Click += btnEliminar_Click;
            // 
            // btnModificar
            // 
            btnModificar.BackColor = SystemColors.Control;
            btnModificar.Location = new Point(138, 3);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new Size(120, 68);
            btnModificar.TabIndex = 1;
            btnModificar.Text = "Modificar";
            btnModificar.UseVisualStyleBackColor = false;
            btnModificar.Click += btnModificar_Click;
            // 
            // btnAgregar
            // 
            btnAgregar.BackColor = SystemColors.Control;
            btnAgregar.Location = new Point(12, 3);
            btnAgregar.Name = "btnAgregar";
            btnAgregar.Size = new Size(120, 68);
            btnAgregar.TabIndex = 0;
            btnAgregar.Text = "Agregar";
            btnAgregar.UseVisualStyleBackColor = false;
            btnAgregar.Click += btnAgregar_Click;
            // 
            // dgvClientes
            // 
            dgvClientes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvClientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvClientes.BackgroundColor = SystemColors.Control;
            dgvClientes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvClientes.Location = new Point(12, 133);
            dgvClientes.Name = "dgvClientes";
            dgvClientes.ReadOnly = true;
            dgvClientes.RowHeadersWidth = 51;
            dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClientes.Size = new Size(1403, 530);
            dgvClientes.TabIndex = 1;
            // 
            // btnSalir
            // 
            btnSalir.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSalir.BackColor = SystemColors.Control;
            btnSalir.Location = new Point(1295, 702);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(120, 68);
            btnSalir.TabIndex = 8;
            btnSalir.Text = "Salir";
            btnSalir.UseVisualStyleBackColor = false;
            btnSalir.Click += btnSalir_Click;
            // 
            // FrmGestionarClientes
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(1427, 782);
            ControlBox = false;
            Controls.Add(btnSalir);
            Controls.Add(dgvClientes);
            Controls.Add(panelSuperior);
            Name = "FrmGestionarClientes";
            Text = "CLIENTES";
            Load += FrmGestionarClientes_Load;
            panelSuperior.ResumeLayout(false);
            panelSuperior.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvClientes).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelSuperior;
        private Button btnAgregar;
        private Label lblFiltrar;
        private Button btnFiltrar;
        private TextBox txtFiltrar;
        private Button btnBorrarFiltros;
        private Button btnEliminar;
        private Button btnModificar;
        private DataGridView dgvClientes;
        private Button btnSalir;
    }
}