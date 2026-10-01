namespace Vista.Seguridad
{
    partial class FrmGestionarUsuarios
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
            btnAgregar = new Button();
            btnResetearClave = new Button();
            btnModificar = new Button();
            btnEliminar = new Button();
            btnVolver = new Button();
            gbFiltrar = new GroupBox();
            btnFiltrar = new Button();
            cbEstados = new ComboBox();
            label3 = new Label();
            label1 = new Label();
            cbGrupos = new ComboBox();
            label2 = new Label();
            txtNombre = new TextBox();
            dgvUsuarios = new DataGridView();
            panel1.SuspendLayout();
            gbFiltrar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUsuarios).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Control;
            panel1.Controls.Add(btnAgregar);
            panel1.Controls.Add(btnResetearClave);
            panel1.Controls.Add(btnModificar);
            panel1.Controls.Add(btnEliminar);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new Size(1210, 61);
            panel1.TabIndex = 0;
            // 
            // btnAgregar
            // 
            btnAgregar.BackColor = SystemColors.Control;
            btnAgregar.Location = new Point(5, 4);
            btnAgregar.Margin = new Padding(5, 4, 5, 4);
            btnAgregar.Name = "btnAgregar";
            btnAgregar.Size = new Size(120, 47);
            btnAgregar.TabIndex = 10;
            btnAgregar.Text = "Agregar";
            btnAgregar.UseVisualStyleBackColor = false;
            btnAgregar.Click += btnAgregar_Click;
            // 
            // btnResetearClave
            // 
            btnResetearClave.BackColor = SystemColors.Control;
            btnResetearClave.Location = new Point(437, 4);
            btnResetearClave.Margin = new Padding(5, 4, 5, 4);
            btnResetearClave.Name = "btnResetearClave";
            btnResetearClave.Size = new Size(162, 47);
            btnResetearClave.TabIndex = 14;
            btnResetearClave.Text = "Resetear clave";
            btnResetearClave.UseVisualStyleBackColor = false;
            btnResetearClave.Click += btnResetear_Click;
            // 
            // btnModificar
            // 
            btnModificar.BackColor = SystemColors.Control;
            btnModificar.Location = new Point(148, 4);
            btnModificar.Margin = new Padding(5, 4, 5, 4);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new Size(120, 47);
            btnModificar.TabIndex = 11;
            btnModificar.Text = "Modificar";
            btnModificar.UseVisualStyleBackColor = false;
            btnModificar.Click += btnModificar_Click;
            // 
            // btnEliminar
            // 
            btnEliminar.BackColor = SystemColors.Control;
            btnEliminar.Location = new Point(295, 4);
            btnEliminar.Margin = new Padding(5, 4, 5, 4);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(120, 47);
            btnEliminar.TabIndex = 12;
            btnEliminar.Text = "Eliminar";
            btnEliminar.UseVisualStyleBackColor = false;
            btnEliminar.Click += btnEliminar_Click;
            // 
            // btnVolver
            // 
            btnVolver.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnVolver.BackColor = SystemColors.Control;
            btnVolver.Location = new Point(1073, 642);
            btnVolver.Margin = new Padding(5, 4, 5, 4);
            btnVolver.Name = "btnVolver";
            btnVolver.Size = new Size(120, 47);
            btnVolver.TabIndex = 13;
            btnVolver.Text = "Volver";
            btnVolver.UseVisualStyleBackColor = false;
            btnVolver.Click += btnVolver_Click;
            // 
            // gbFiltrar
            // 
            gbFiltrar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbFiltrar.Controls.Add(btnFiltrar);
            gbFiltrar.Controls.Add(cbEstados);
            gbFiltrar.Controls.Add(label3);
            gbFiltrar.Controls.Add(label1);
            gbFiltrar.Controls.Add(cbGrupos);
            gbFiltrar.Controls.Add(label2);
            gbFiltrar.Controls.Add(txtNombre);
            gbFiltrar.Location = new Point(12, 84);
            gbFiltrar.Margin = new Padding(5, 4, 5, 4);
            gbFiltrar.Name = "gbFiltrar";
            gbFiltrar.Padding = new Padding(5, 4, 5, 4);
            gbFiltrar.Size = new Size(1181, 104);
            gbFiltrar.TabIndex = 16;
            gbFiltrar.TabStop = false;
            gbFiltrar.Text = "Filtrar";
            // 
            // btnFiltrar
            // 
            btnFiltrar.Location = new Point(926, 36);
            btnFiltrar.Margin = new Padding(5, 4, 5, 4);
            btnFiltrar.Name = "btnFiltrar";
            btnFiltrar.Size = new Size(120, 47);
            btnFiltrar.TabIndex = 9;
            btnFiltrar.Text = "Buscar";
            btnFiltrar.UseVisualStyleBackColor = true;
            btnFiltrar.Click += btnFiltrar_Click;
            // 
            // cbEstados
            // 
            cbEstados.FormattingEnabled = true;
            cbEstados.Location = new Point(719, 46);
            cbEstados.Margin = new Padding(5, 4, 5, 4);
            cbEstados.Name = "cbEstados";
            cbEstados.Size = new Size(181, 28);
            cbEstados.TabIndex = 8;
            cbEstados.SelectedIndexChanged += cbEstados_SelectedIndexChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(652, 54);
            label3.Margin = new Padding(5, 0, 5, 0);
            label3.Name = "label3";
            label3.Size = new Size(57, 20);
            label3.TabIndex = 7;
            label3.Tag = "BLANCO";
            label3.Text = "Estado:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(388, 55);
            label1.Margin = new Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(53, 20);
            label1.TabIndex = 6;
            label1.Tag = "BLANCO";
            label1.Text = "Grupo:";
            // 
            // cbGrupos
            // 
            cbGrupos.FormattingEnabled = true;
            cbGrupos.Location = new Point(451, 48);
            cbGrupos.Margin = new Padding(5, 4, 5, 4);
            cbGrupos.Name = "cbGrupos";
            cbGrupos.Size = new Size(181, 28);
            cbGrupos.TabIndex = 5;
            cbGrupos.SelectedIndexChanged += cbGrupos_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(10, 54);
            label2.Margin = new Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new Size(67, 20);
            label2.TabIndex = 3;
            label2.Tag = "BLANCO";
            label2.Text = "Nombre:";
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(87, 48);
            txtNombre.Margin = new Padding(5, 4, 5, 4);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(291, 27);
            txtNombre.TabIndex = 1;
            txtNombre.TextChanged += txtNombre_TextChanged;
            // 
            // dgvUsuarios
            // 
            dgvUsuarios.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsuarios.BackgroundColor = SystemColors.Control;
            dgvUsuarios.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvUsuarios.Location = new Point(12, 210);
            dgvUsuarios.Name = "dgvUsuarios";
            dgvUsuarios.ReadOnly = true;
            dgvUsuarios.RowHeadersWidth = 51;
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.Size = new Size(1181, 425);
            dgvUsuarios.TabIndex = 10;
            // 
            // FrmGestionarUsuarios
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1210, 693);
            ControlBox = false;
            Controls.Add(dgvUsuarios);
            Controls.Add(gbFiltrar);
            Controls.Add(btnVolver);
            Controls.Add(panel1);
            Name = "FrmGestionarUsuarios";
            Text = "USUARIOS";
            panel1.ResumeLayout(false);
            gbFiltrar.ResumeLayout(false);
            gbFiltrar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUsuarios).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button btnVolver;
        private Button btnEliminar;
        private Button btnModificar;
        private Button btnAgregar;
        private Button btnResetearClave;
        private GroupBox gbFiltrar;
        private TextBox txtNombre;
        private Label label2;
        private ComboBox cbEstados;
        private Label label3;
        private Label label1;
        private ComboBox cbGrupos;
        private Button btnFiltrar;
        private DataGridView dgvUsuarios;
    }
}