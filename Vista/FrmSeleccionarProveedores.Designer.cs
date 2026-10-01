namespace Vista
{
    partial class FrmSeleccionarProveedores
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSeleccionarProveedores));
            dgvProvAgregados = new DataGridView();
            panelSuperior = new Panel();
            pbLogo = new PictureBox();
            btnCancelar = new Button();
            cbProveedores = new ComboBox();
            numPrecio = new NumericUpDown();
            btnAgregarProvPrecio = new Button();
            label1 = new Label();
            lblPrecio = new Label();
            btnGuardarProvPrecio = new Button();
            btnEliminarProvPrecio = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvProvAgregados).BeginInit();
            panelSuperior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPrecio).BeginInit();
            SuspendLayout();
            // 
            // dgvProvAgregados
            // 
            dgvProvAgregados.BackgroundColor = SystemColors.Control;
            dgvProvAgregados.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProvAgregados.Location = new Point(5, 228);
            dgvProvAgregados.MultiSelect = false;
            dgvProvAgregados.Name = "dgvProvAgregados";
            dgvProvAgregados.ReadOnly = true;
            dgvProvAgregados.RowHeadersWidth = 51;
            dgvProvAgregados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProvAgregados.Size = new Size(300, 188);
            dgvProvAgregados.TabIndex = 0;
            // 
            // panelSuperior
            // 
            panelSuperior.BackColor = SystemColors.Control;
            panelSuperior.Controls.Add(pbLogo);
            panelSuperior.Controls.Add(btnCancelar);
            panelSuperior.Location = new Point(0, 0);
            panelSuperior.Name = "panelSuperior";
            panelSuperior.Size = new Size(462, 61);
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
            btnCancelar.Location = new Point(357, 12);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(94, 29);
            btnCancelar.TabIndex = 8;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            // 
            // cbProveedores
            // 
            cbProveedores.FormattingEnabled = true;
            cbProveedores.Location = new Point(154, 80);
            cbProveedores.Name = "cbProveedores";
            cbProveedores.Size = new Size(151, 28);
            cbProveedores.TabIndex = 13;
            // 
            // numPrecio
            // 
            numPrecio.DecimalPlaces = 2;
            numPrecio.Increment = new decimal(new int[] { 50, 0, 0, 131072 });
            numPrecio.Location = new Point(154, 135);
            numPrecio.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            numPrecio.Name = "numPrecio";
            numPrecio.Size = new Size(150, 27);
            numPrecio.TabIndex = 14;
            // 
            // btnAgregarProvPrecio
            // 
            btnAgregarProvPrecio.Location = new Point(41, 182);
            btnAgregarProvPrecio.Name = "btnAgregarProvPrecio";
            btnAgregarProvPrecio.Size = new Size(200, 29);
            btnAgregarProvPrecio.TabIndex = 15;
            btnAgregarProvPrecio.Text = "Agregar Proveedor y Precio";
            btnAgregarProvPrecio.UseVisualStyleBackColor = true;
            btnAgregarProvPrecio.Click += btnAgregarProvPrecio_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(5, 88);
            label1.Name = "label1";
            label1.Size = new Size(80, 20);
            label1.TabIndex = 16;
            label1.Text = "Proveedor:";
            // 
            // lblPrecio
            // 
            lblPrecio.AutoSize = true;
            lblPrecio.Location = new Point(5, 142);
            lblPrecio.Name = "lblPrecio";
            lblPrecio.Size = new Size(53, 20);
            lblPrecio.TabIndex = 17;
            lblPrecio.Text = "Precio:";
            // 
            // btnGuardarProvPrecio
            // 
            btnGuardarProvPrecio.Location = new Point(5, 453);
            btnGuardarProvPrecio.Name = "btnGuardarProvPrecio";
            btnGuardarProvPrecio.Size = new Size(333, 65);
            btnGuardarProvPrecio.TabIndex = 18;
            btnGuardarProvPrecio.Text = "Guardar proveedores y precios";
            btnGuardarProvPrecio.UseVisualStyleBackColor = true;
            btnGuardarProvPrecio.Click += btnGuardarProvPrecio_Click;
            // 
            // btnEliminarProvPrecio
            // 
            btnEliminarProvPrecio.Location = new Point(336, 283);
            btnEliminarProvPrecio.Name = "btnEliminarProvPrecio";
            btnEliminarProvPrecio.Size = new Size(115, 73);
            btnEliminarProvPrecio.TabIndex = 19;
            btnEliminarProvPrecio.Text = "Eliminar Proveedor y Precio";
            btnEliminarProvPrecio.UseVisualStyleBackColor = true;
            btnEliminarProvPrecio.Click += btnEliminarProvPrecio_Click;
            // 
            // FrmSeleccionarProveedores
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(463, 580);
            Controls.Add(btnEliminarProvPrecio);
            Controls.Add(btnGuardarProvPrecio);
            Controls.Add(lblPrecio);
            Controls.Add(label1);
            Controls.Add(btnAgregarProvPrecio);
            Controls.Add(numPrecio);
            Controls.Add(cbProveedores);
            Controls.Add(panelSuperior);
            Controls.Add(dgvProvAgregados);
            Name = "FrmSeleccionarProveedores";
            ((System.ComponentModel.ISupportInitialize)dgvProvAgregados).EndInit();
            panelSuperior.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPrecio).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvProvAgregados;
        private Panel panelSuperior;
        private PictureBox pbLogo;
        private Button btnCancelar;
        private ComboBox cbProveedores;
        private NumericUpDown numPrecio;
        private Button btnAgregarProvPrecio;
        private Label label1;
        private Label lblPrecio;
        private Button btnGuardarProvPrecio;
        private Button btnEliminarProvPrecio;
    }
}