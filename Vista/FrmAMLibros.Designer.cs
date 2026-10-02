namespace Vista
{
    partial class FrmAMLibros
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAMLibros));
            panelSuperior = new Panel();
            pbLogo = new PictureBox();
            btnCancelar = new Button();
            lblEmpresa = new Label();
            txtEditorial = new TextBox();
            lblTelefono = new Label();
            btnGuardar = new Button();
            txtDescripcion = new TextBox();
            txtAutor = new TextBox();
            txtTitulo = new TextBox();
            label4 = new Label();
            label2 = new Label();
            label1 = new Label();
            cbGeneros = new ComboBox();
            label3 = new Label();
            sqlCommand1 = new Microsoft.Data.SqlClient.SqlCommand();
            lblAgregarProveedor = new Label();
            lblAgregarGenero = new Label();
            btnAgregarGenero = new Button();
            label5 = new Label();
            numStock = new NumericUpDown();
            txtAñoPublicacion = new TextBox();
            numPrecio = new NumericUpDown();
            label6 = new Label();
            lblISBN = new Label();
            txtISBN = new TextBox();
            panelSuperior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numStock).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPrecio).BeginInit();
            SuspendLayout();
            // 
            // panelSuperior
            // 
            panelSuperior.BackColor = SystemColors.Control;
            panelSuperior.Controls.Add(pbLogo);
            panelSuperior.Controls.Add(btnCancelar);
            panelSuperior.Location = new Point(0, 0);
            panelSuperior.Name = "panelSuperior";
            panelSuperior.Size = new Size(655, 61);
            panelSuperior.TabIndex = 11;
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
            btnCancelar.Location = new Point(544, 12);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(94, 29);
            btnCancelar.TabIndex = 8;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // lblEmpresa
            // 
            lblEmpresa.AutoSize = true;
            lblEmpresa.Location = new Point(7, 312);
            lblEmpresa.Name = "lblEmpresa";
            lblEmpresa.Size = new Size(60, 20);
            lblEmpresa.TabIndex = 35;
            lblEmpresa.Text = "Género:";
            // 
            // txtEditorial
            // 
            txtEditorial.Location = new Point(176, 355);
            txtEditorial.Name = "txtEditorial";
            txtEditorial.Size = new Size(125, 27);
            txtEditorial.TabIndex = 34;
            // 
            // lblTelefono
            // 
            lblTelefono.AutoSize = true;
            lblTelefono.Location = new Point(7, 263);
            lblTelefono.Name = "lblTelefono";
            lblTelefono.Size = new Size(141, 20);
            lblTelefono.TabIndex = 33;
            lblTelefono.Text = "Año de publicación:";
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(544, 511);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(94, 29);
            btnGuardar.TabIndex = 32;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // txtDescripcion
            // 
            txtDescripcion.Location = new Point(182, 169);
            txtDescripcion.Multiline = true;
            txtDescripcion.Name = "txtDescripcion";
            txtDescripcion.Size = new Size(224, 73);
            txtDescripcion.TabIndex = 31;
            // 
            // txtAutor
            // 
            txtAutor.Location = new Point(182, 120);
            txtAutor.Name = "txtAutor";
            txtAutor.Size = new Size(125, 27);
            txtAutor.TabIndex = 30;
            // 
            // txtTitulo
            // 
            txtTitulo.Location = new Point(182, 77);
            txtTitulo.Name = "txtTitulo";
            txtTitulo.Size = new Size(125, 27);
            txtTitulo.TabIndex = 29;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(7, 172);
            label4.Name = "label4";
            label4.Size = new Size(90, 20);
            label4.TabIndex = 28;
            label4.Text = "Descripción:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(7, 127);
            label2.Name = "label2";
            label2.Size = new Size(49, 20);
            label2.TabIndex = 27;
            label2.Text = "Autor:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(7, 84);
            label1.Name = "label1";
            label1.Size = new Size(50, 20);
            label1.TabIndex = 26;
            label1.Text = "Titulo:";
            // 
            // cbGeneros
            // 
            cbGeneros.FormattingEnabled = true;
            cbGeneros.Location = new Point(176, 304);
            cbGeneros.Name = "cbGeneros";
            cbGeneros.Size = new Size(151, 28);
            cbGeneros.TabIndex = 38;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(7, 362);
            label3.Name = "label3";
            label3.Size = new Size(68, 20);
            label3.TabIndex = 39;
            label3.Text = "Editorial:";
            // 
            // sqlCommand1
            // 
            sqlCommand1.CommandTimeout = 30;
            sqlCommand1.EnableOptimizedParameterBinding = false;
            // 
            // lblAgregarProveedor
            // 
            lblAgregarProveedor.AutoSize = true;
            lblAgregarProveedor.Location = new Point(439, 212);
            lblAgregarProveedor.Name = "lblAgregarProveedor";
            lblAgregarProveedor.Size = new Size(0, 20);
            lblAgregarProveedor.TabIndex = 41;
            // 
            // lblAgregarGenero
            // 
            lblAgregarGenero.AutoSize = true;
            lblAgregarGenero.Location = new Point(333, 307);
            lblAgregarGenero.Name = "lblAgregarGenero";
            lblAgregarGenero.Size = new Size(125, 20);
            lblAgregarGenero.TabIndex = 43;
            lblAgregarGenero.Text = "¿Falta un género?";
            // 
            // btnAgregarGenero
            // 
            btnAgregarGenero.Location = new Point(482, 298);
            btnAgregarGenero.Name = "btnAgregarGenero";
            btnAgregarGenero.Size = new Size(159, 29);
            btnAgregarGenero.TabIndex = 45;
            btnAgregarGenero.Text = "Agregar genero";
            btnAgregarGenero.UseVisualStyleBackColor = true;
            btnAgregarGenero.Click += btnAgregarGenero_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(7, 409);
            label5.Name = "label5";
            label5.Size = new Size(48, 20);
            label5.TabIndex = 47;
            label5.Text = "Stock:";
            // 
            // numStock
            // 
            numStock.Location = new Point(176, 407);
            numStock.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numStock.Name = "numStock";
            numStock.Size = new Size(150, 27);
            numStock.TabIndex = 48;
            // 
            // txtAñoPublicacion
            // 
            txtAñoPublicacion.Location = new Point(182, 260);
            txtAñoPublicacion.Name = "txtAñoPublicacion";
            txtAñoPublicacion.Size = new Size(125, 27);
            txtAñoPublicacion.TabIndex = 49;
            // 
            // numPrecio
            // 
            numPrecio.DecimalPlaces = 2;
            numPrecio.Location = new Point(176, 455);
            numPrecio.Maximum = new decimal(new int[] { -727379968, 232, 0, 0 });
            numPrecio.Name = "numPrecio";
            numPrecio.Size = new Size(150, 27);
            numPrecio.TabIndex = 50;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(7, 462);
            label6.Name = "label6";
            label6.Size = new Size(50, 20);
            label6.TabIndex = 51;
            label6.Text = "Precio";
            // 
            // lblISBN
            // 
            lblISBN.AutoSize = true;
            lblISBN.Location = new Point(7, 509);
            lblISBN.Name = "lblISBN";
            lblISBN.Size = new Size(152, 20);
            lblISBN.TabIndex = 52;
            lblISBN.Text = "ISBN / Cód. barras";
            // 
            // txtISBN
            // 
            txtISBN.Location = new Point(176, 506);
            txtISBN.MaxLength = 20;
            txtISBN.Name = "txtISBN";
            txtISBN.Size = new Size(250, 27);
            txtISBN.TabIndex = 53;
            // 
            // FrmAMLibros
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(656, 552);
            Controls.Add(txtISBN);
            Controls.Add(lblISBN);
            Controls.Add(label6);
            Controls.Add(numPrecio);
            Controls.Add(txtAñoPublicacion);
            Controls.Add(numStock);
            Controls.Add(label5);
            Controls.Add(btnAgregarGenero);
            Controls.Add(lblAgregarGenero);
            Controls.Add(lblAgregarProveedor);
            Controls.Add(label3);
            Controls.Add(cbGeneros);
            Controls.Add(lblEmpresa);
            Controls.Add(txtEditorial);
            Controls.Add(lblTelefono);
            Controls.Add(btnGuardar);
            Controls.Add(txtDescripcion);
            Controls.Add(txtAutor);
            Controls.Add(txtTitulo);
            Controls.Add(label4);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(panelSuperior);
            Name = "FrmAMLibros";
            panelSuperior.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)numStock).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPrecio).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panelSuperior;
        private PictureBox pbLogo;
        private Button btnCancelar;
        private Label lblEmpresa;
        private TextBox txtEditorial;
        private Label lblTelefono;
        private Button btnGuardar;
        private TextBox txtDescripcion;
        private TextBox txtAutor;
        private TextBox txtTitulo;
        private Label label4;
        private Label label2;
        private Label label1;
        private ComboBox cbGeneros;
        private Label label3;
        private Microsoft.Data.SqlClient.SqlCommand sqlCommand1;
        private Label lblAgregarProveedor;
        private Label lblAgregarGenero;
        private Button btnAgregarGenero;
        private Label label5;
        private NumericUpDown numStock;
        private TextBox txtAñoPublicacion;
        private NumericUpDown numPrecio;
        private Label label6;
        private Label lblISBN;
        private TextBox txtISBN;
    }
}