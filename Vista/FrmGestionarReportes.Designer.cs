namespace Vista
{
    partial class FrmGestionarReportes
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
            dgvIngresos = new DataGridView();
            dtpHasta = new DateTimePicker();
            dtpDesde = new DateTimePicker();
            label2 = new Label();
            label1 = new Label();
            btnExportar = new Button();
            btnGenerar = new Button();
            dgvLibrosVendidos = new DataGridView();
            dgvGeneros = new DataGridView();
            panelIngresos = new Panel();
            panelLibrosVendidos = new Panel();
            panelGeneros = new Panel();
            panelSuperior = new Panel();
            btnSalir = new Button();
            sqlCommand1 = new Microsoft.Data.SqlClient.SqlCommand();
            ((System.ComponentModel.ISupportInitialize)dgvIngresos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvLibrosVendidos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvGeneros).BeginInit();
            panelSuperior.SuspendLayout();
            SuspendLayout();
            // 
            // dgvIngresos
            // 
            dgvIngresos.BackgroundColor = SystemColors.Control;
            dgvIngresos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvIngresos.Location = new Point(42, 141);
            dgvIngresos.MultiSelect = false;
            dgvIngresos.Name = "dgvIngresos";
            dgvIngresos.ReadOnly = true;
            dgvIngresos.RowHeadersWidth = 51;
            dgvIngresos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvIngresos.Size = new Size(499, 184);
            dgvIngresos.TabIndex = 0;
            // 
            // dtpHasta
            // 
            dtpHasta.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dtpHasta.Format = DateTimePickerFormat.Short;
            dtpHasta.Location = new Point(358, 41);
            dtpHasta.Name = "dtpHasta";
            dtpHasta.Size = new Size(150, 34);
            dtpHasta.TabIndex = 10;
            // 
            // dtpDesde
            // 
            dtpDesde.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dtpDesde.Format = DateTimePickerFormat.Short;
            dtpDesde.Location = new Point(358, 6);
            dtpDesde.Name = "dtpDesde";
            dtpDesde.Size = new Size(150, 34);
            dtpDesde.TabIndex = 9;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(277, 46);
            label2.Name = "label2";
            label2.Size = new Size(65, 28);
            label2.TabIndex = 8;
            label2.Tag = "BLANCO";
            label2.Text = "Hasta:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(277, 9);
            label1.Name = "label1";
            label1.Size = new Size(70, 28);
            label1.TabIndex = 7;
            label1.Tag = "BLANCO";
            label1.Text = "Desde:";
            // 
            // btnExportar
            // 
            btnExportar.BackColor = SystemColors.Control;
            btnExportar.Location = new Point(138, 3);
            btnExportar.Name = "btnExportar";
            btnExportar.Size = new Size(120, 68);
            btnExportar.TabIndex = 1;
            btnExportar.Text = "Exportar";
            btnExportar.UseVisualStyleBackColor = false;
            btnExportar.Click += btnExportar_Click;
            // 
            // btnGenerar
            // 
            btnGenerar.BackColor = SystemColors.Control;
            btnGenerar.Location = new Point(12, 3);
            btnGenerar.Name = "btnGenerar";
            btnGenerar.Size = new Size(120, 68);
            btnGenerar.TabIndex = 0;
            btnGenerar.Text = "Generar";
            btnGenerar.UseVisualStyleBackColor = false;
            btnGenerar.Click += btnGenerar_Click;
            // 
            // dgvLibrosVendidos
            // 
            dgvLibrosVendidos.BackgroundColor = SystemColors.Control;
            dgvLibrosVendidos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLibrosVendidos.Location = new Point(42, 461);
            dgvLibrosVendidos.MultiSelect = false;
            dgvLibrosVendidos.Name = "dgvLibrosVendidos";
            dgvLibrosVendidos.ReadOnly = true;
            dgvLibrosVendidos.RowHeadersWidth = 51;
            dgvLibrosVendidos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLibrosVendidos.Size = new Size(499, 184);
            dgvLibrosVendidos.TabIndex = 3;
            // 
            // dgvGeneros
            // 
            dgvGeneros.BackgroundColor = SystemColors.Control;
            dgvGeneros.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvGeneros.Location = new Point(42, 795);
            dgvGeneros.MultiSelect = false;
            dgvGeneros.Name = "dgvGeneros";
            dgvGeneros.ReadOnly = true;
            dgvGeneros.RowHeadersWidth = 51;
            dgvGeneros.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvGeneros.Size = new Size(499, 184);
            dgvGeneros.TabIndex = 4;
            // 
            // panelIngresos
            // 
            panelIngresos.Anchor = AnchorStyles.Right;
            panelIngresos.Location = new Point(777, 100);
            panelIngresos.Name = "panelIngresos";
            panelIngresos.Size = new Size(446, 259);
            panelIngresos.TabIndex = 6;
            // 
            // panelLibrosVendidos
            // 
            panelLibrosVendidos.Anchor = AnchorStyles.Right;
            panelLibrosVendidos.Location = new Point(777, 420);
            panelLibrosVendidos.Name = "panelLibrosVendidos";
            panelLibrosVendidos.Size = new Size(446, 259);
            panelLibrosVendidos.TabIndex = 7;
            // 
            // panelGeneros
            // 
            panelGeneros.Anchor = AnchorStyles.Right;
            panelGeneros.Location = new Point(777, 741);
            panelGeneros.Name = "panelGeneros";
            panelGeneros.Size = new Size(446, 259);
            panelGeneros.TabIndex = 7;
            // 
            // panelSuperior
            // 
            panelSuperior.Controls.Add(btnSalir);
            panelSuperior.Controls.Add(btnGenerar);
            panelSuperior.Controls.Add(dtpHasta);
            panelSuperior.Controls.Add(dtpDesde);
            panelSuperior.Controls.Add(btnExportar);
            panelSuperior.Controls.Add(label2);
            panelSuperior.Controls.Add(label1);
            panelSuperior.Dock = DockStyle.Top;
            panelSuperior.Location = new Point(0, 0);
            panelSuperior.Name = "panelSuperior";
            panelSuperior.Size = new Size(1253, 82);
            panelSuperior.TabIndex = 11;
            // 
            // btnSalir
            // 
            btnSalir.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSalir.BackColor = SystemColors.Control;
            btnSalir.Location = new Point(1121, 6);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(120, 68);
            btnSalir.TabIndex = 11;
            btnSalir.Text = "Salir";
            btnSalir.UseVisualStyleBackColor = false;
            btnSalir.Click += btnSalir_Click;
            // 
            // sqlCommand1
            // 
            sqlCommand1.CommandTimeout = 30;
            sqlCommand1.EnableOptimizedParameterBinding = false;
            // 
            // FrmGestionarReportes
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1253, 1010);
            Controls.Add(panelSuperior);
            Controls.Add(panelGeneros);
            Controls.Add(panelLibrosVendidos);
            Controls.Add(panelIngresos);
            Controls.Add(dgvGeneros);
            Controls.Add(dgvLibrosVendidos);
            Controls.Add(dgvIngresos);
            Name = "FrmGestionarReportes";
            Text = "REPORTES";
            Load += FrmGestionarReportes_Load;
            ((System.ComponentModel.ISupportInitialize)dgvIngresos).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvLibrosVendidos).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvGeneros).EndInit();
            panelSuperior.ResumeLayout(false);
            panelSuperior.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvIngresos;
        private DateTimePicker dtpDesde;
        private Label label2;
        private Label label1;
        private Button btnExportar;
        private Button btnGenerar;
        private DateTimePicker dtpHasta;
        private DataGridView dgvLibrosVendidos;
        private DataGridView dgvGeneros;
        private Panel panelIngresos;
        private Panel panelLibrosVendidos;
        private Panel panelGeneros;
        private Panel panelSuperior;
        private Button btnSalir;
        private Microsoft.Data.SqlClient.SqlCommand sqlCommand1;
    }
}