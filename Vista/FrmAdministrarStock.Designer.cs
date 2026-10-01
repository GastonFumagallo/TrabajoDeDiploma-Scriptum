namespace Vista
{
    partial class FrmAdministrarStock
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAdministrarStock));
            panelSuperior = new Panel();
            pbLogo = new PictureBox();
            btnCancelar = new Button();
            txtFiltrar = new TextBox();
            btnBuscar = new Button();
            dgvLibros = new DataGridView();
            lblUnidades = new Label();
            numStockLibro = new NumericUpDown();
            btnGuardar = new Button();
            panelSuperior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvLibros).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numStockLibro).BeginInit();
            SuspendLayout();
            // 
            // panelSuperior
            // 
            panelSuperior.BackColor = SystemColors.Control;
            panelSuperior.Controls.Add(pbLogo);
            panelSuperior.Controls.Add(btnCancelar);
            panelSuperior.Dock = DockStyle.Top;
            panelSuperior.Location = new Point(0, 0);
            panelSuperior.Name = "panelSuperior";
            panelSuperior.Size = new Size(649, 61);
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
            btnCancelar.Location = new Point(393, 12);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(94, 29);
            btnCancelar.TabIndex = 8;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // txtFiltrar
            // 
            txtFiltrar.Location = new Point(12, 84);
            txtFiltrar.Name = "txtFiltrar";
            txtFiltrar.Size = new Size(351, 27);
            txtFiltrar.TabIndex = 13;
            txtFiltrar.TextChanged += txtFiltrar_TextChanged;
            // 
            // btnBuscar
            // 
            btnBuscar.Location = new Point(393, 82);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(94, 29);
            btnBuscar.TabIndex = 11;
            btnBuscar.Text = "Buscar";
            btnBuscar.UseVisualStyleBackColor = true;
            btnBuscar.Click += btnBuscar_Click;
            // 
            // dgvLibros
            // 
            dgvLibros.BackgroundColor = SystemColors.Control;
            dgvLibros.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLibros.Location = new Point(12, 129);
            dgvLibros.MultiSelect = false;
            dgvLibros.Name = "dgvLibros";
            dgvLibros.ReadOnly = true;
            dgvLibros.RowHeadersWidth = 51;
            dgvLibros.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLibros.Size = new Size(475, 234);
            dgvLibros.TabIndex = 14;
            dgvLibros.CellClick += dgvLibros_CellClick;
            // 
            // lblUnidades
            // 
            lblUnidades.AutoSize = true;
            lblUnidades.Font = new Font("Segoe UI", 12F);
            lblUnidades.Location = new Point(12, 390);
            lblUnidades.Name = "lblUnidades";
            lblUnidades.Size = new Size(281, 28);
            lblUnidades.TabIndex = 15;
            lblUnidades.Text = "Unidades del libro disponibles:";
            // 
            // numStockLibro
            // 
            numStockLibro.Location = new Point(307, 391);
            numStockLibro.Name = "numStockLibro";
            numStockLibro.Size = new Size(180, 27);
            numStockLibro.TabIndex = 16;
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(234, 442);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(94, 61);
            btnGuardar.TabIndex = 17;
            btnGuardar.Text = "Modificar Stock";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // FrmAdministrarStock
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(649, 515);
            Controls.Add(btnGuardar);
            Controls.Add(numStockLibro);
            Controls.Add(lblUnidades);
            Controls.Add(dgvLibros);
            Controls.Add(btnBuscar);
            Controls.Add(txtFiltrar);
            Controls.Add(panelSuperior);
            Name = "FrmAdministrarStock";
            panelSuperior.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvLibros).EndInit();
            ((System.ComponentModel.ISupportInitialize)numStockLibro).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panelSuperior;
        private PictureBox pbLogo;
        private Button btnCancelar;
        private TextBox txtFiltrar;
        private Button btnBuscar;
        private DataGridView dgvLibros;
        private Label lblUnidades;
        private NumericUpDown numStockLibro;
        private Button btnGuardar;
    }
}