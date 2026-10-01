namespace Vista.Seguridad
{
    partial class FrmGestionarGrupos
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
            panel1 = new Panel();
            btnEliminar = new Button();
            btnAgregar = new Button();
            btnModificar = new Button();
            groupBox1 = new GroupBox();
            btnFiltrar = new Button();
            CBestados = new ComboBox();
            label3 = new Label();
            label2 = new Label();
            lblFiltrar = new Label();
            txtNombre = new TextBox();
            dgvGrupos = new DataGridView();
            btnVolver = new Button();
            panel1.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvGrupos).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Control;
            panel1.Controls.Add(btnEliminar);
            panel1.Controls.Add(btnAgregar);
            panel1.Controls.Add(btnModificar);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1060, 76);
            panel1.TabIndex = 0;
            // 
            // btnEliminar
            // 
            btnEliminar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnEliminar.BackColor = SystemColors.Control;
            btnEliminar.Location = new Point(304, 13);
            btnEliminar.Margin = new Padding(5, 4, 5, 4);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(120, 47);
            btnEliminar.TabIndex = 8;
            btnEliminar.Text = "Eliminar";
            btnEliminar.UseVisualStyleBackColor = false;
            btnEliminar.Click += btnEliminar_Click;
            // 
            // btnAgregar
            // 
            btnAgregar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAgregar.BackColor = SystemColors.Control;
            btnAgregar.Location = new Point(14, 13);
            btnAgregar.Margin = new Padding(5, 4, 5, 4);
            btnAgregar.Name = "btnAgregar";
            btnAgregar.Size = new Size(120, 47);
            btnAgregar.TabIndex = 6;
            btnAgregar.Text = "Agregar";
            btnAgregar.UseVisualStyleBackColor = false;
            btnAgregar.Click += btnAgregar_Click;
            // 
            // btnModificar
            // 
            btnModificar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnModificar.BackColor = SystemColors.Control;
            btnModificar.Location = new Point(157, 13);
            btnModificar.Margin = new Padding(5, 4, 5, 4);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new Size(120, 47);
            btnModificar.TabIndex = 7;
            btnModificar.Text = "Modificar";
            btnModificar.UseVisualStyleBackColor = false;
            btnModificar.Click += btnModificar_Click;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(btnFiltrar);
            groupBox1.Controls.Add(CBestados);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(lblFiltrar);
            groupBox1.Controls.Add(txtNombre);
            groupBox1.Location = new Point(16, 94);
            groupBox1.Margin = new Padding(5, 4, 5, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(5, 4, 5, 4);
            groupBox1.Size = new Size(1033, 104);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Filtrar";
            // 
            // btnFiltrar
            // 
            btnFiltrar.Location = new Point(887, 26);
            btnFiltrar.Margin = new Padding(5, 4, 5, 4);
            btnFiltrar.Name = "btnFiltrar";
            btnFiltrar.Size = new Size(120, 47);
            btnFiltrar.TabIndex = 5;
            btnFiltrar.Text = "Buscar";
            btnFiltrar.UseVisualStyleBackColor = true;
            btnFiltrar.Click += btnFiltrar_Click;
            // 
            // CBestados
            // 
            CBestados.FormattingEnabled = true;
            CBestados.Location = new Point(665, 36);
            CBestados.Margin = new Padding(5, 4, 5, 4);
            CBestados.Name = "CBestados";
            CBestados.Size = new Size(181, 28);
            CBestados.TabIndex = 4;
            CBestados.SelectedIndexChanged += CBestados_SelectedIndexChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(592, 44);
            label3.Margin = new Padding(5, 0, 5, 0);
            label3.Name = "label3";
            label3.Size = new Size(57, 20);
            label3.TabIndex = 3;
            label3.Tag = "BLANCO";
            label3.Text = "Estado:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(9, 44);
            label2.Margin = new Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new Size(67, 20);
            label2.TabIndex = 2;
            label2.Tag = "BLANCO";
            label2.Text = "Nombre:";
            // 
            // lblFiltrar
            // 
            lblFiltrar.AutoSize = true;
            lblFiltrar.Location = new Point(0, 0);
            lblFiltrar.Name = "lblFiltrar";
            lblFiltrar.Size = new Size(0, 20);
            lblFiltrar.TabIndex = 1;
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(90, 39);
            txtNombre.Margin = new Padding(5, 4, 5, 4);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(492, 27);
            txtNombre.TabIndex = 0;
            txtNombre.TextChanged += txtNombre_TextChanged;
            // 
            // dgvGrupos
            // 
            dgvGrupos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvGrupos.BackgroundColor = SystemColors.Control;
            dgvGrupos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvGrupos.Location = new Point(16, 237);
            dgvGrupos.Margin = new Padding(5, 4, 5, 4);
            dgvGrupos.MultiSelect = false;
            dgvGrupos.Name = "dgvGrupos";
            dgvGrupos.ReadOnly = true;
            dgvGrupos.RowHeadersWidth = 51;
            dgvGrupos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvGrupos.Size = new Size(1033, 425);
            dgvGrupos.TabIndex = 2;
            // 
            // btnVolver
            // 
            btnVolver.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnVolver.BackColor = SystemColors.Control;
            btnVolver.Location = new Point(929, 672);
            btnVolver.Margin = new Padding(5, 4, 5, 4);
            btnVolver.Name = "btnVolver";
            btnVolver.Size = new Size(120, 47);
            btnVolver.TabIndex = 9;
            btnVolver.Text = "Volver";
            btnVolver.UseVisualStyleBackColor = false;
            btnVolver.Click += btnVolver_Click;
            // 
            // FrmGestionarGrupos
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(1060, 732);
            ControlBox = false;
            Controls.Add(btnVolver);
            Controls.Add(dgvGrupos);
            Controls.Add(groupBox1);
            Controls.Add(panel1);
            Name = "FrmGestionarGrupos";
            Text = "GRUPOS";
            Load += FrmGestionarGrupos_Load;
            panel1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvGrupos).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private GroupBox groupBox1;
        private TextBox txtNombre;
        private Button btnFiltrar;
        private ComboBox CBestados;
        private Label label3;
        private Label label2;
        private Label lblFiltrar;
        private DataGridView dgvGrupos;
        private Button btnAgregar;
        private Button btnModificar;
        private Button btnEliminar;
        private Button btnVolver;
    }
}