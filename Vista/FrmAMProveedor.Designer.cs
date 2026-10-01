namespace Vista
{
    partial class FrmAMProveedor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAMProveedor));
            panelSuperior = new Panel();
            pbLogo = new PictureBox();
            btnCancelar = new Button();
            txtTelefono = new TextBox();
            lblTelefono = new Label();
            lblIngresarDatos = new Label();
            btnGuardar = new Button();
            txtMail = new TextBox();
            txtNombreyA = new TextBox();
            txtDocumento = new TextBox();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            txtEmpresa = new TextBox();
            lblEmpresa = new Label();
            panelSuperior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbLogo).BeginInit();
            SuspendLayout();
            // 
            // panelSuperior
            // 
            panelSuperior.Controls.Add(pbLogo);
            panelSuperior.Controls.Add(btnCancelar);
            panelSuperior.Location = new Point(0, 0);
            panelSuperior.Name = "panelSuperior";
            panelSuperior.Size = new Size(418, 61);
            panelSuperior.TabIndex = 10;
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
            btnCancelar.Location = new Point(312, 12);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(94, 29);
            btnCancelar.TabIndex = 8;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // txtTelefono
            // 
            txtTelefono.Location = new Point(187, 243);
            txtTelefono.Name = "txtTelefono";
            txtTelefono.Size = new Size(125, 27);
            txtTelefono.TabIndex = 23;
            // 
            // lblTelefono
            // 
            lblTelefono.AutoSize = true;
            lblTelefono.Location = new Point(12, 246);
            lblTelefono.Name = "lblTelefono";
            lblTelefono.Size = new Size(70, 20);
            lblTelefono.TabIndex = 22;
            lblTelefono.Text = "Telefono:";
            // 
            // lblIngresarDatos
            // 
            lblIngresarDatos.AutoSize = true;
            lblIngresarDatos.Location = new Point(12, 75);
            lblIngresarDatos.Name = "lblIngresarDatos";
            lblIngresarDatos.Size = new Size(388, 20);
            lblIngresarDatos.TabIndex = 21;
            lblIngresarDatos.Text = "Ingrese los datos del Proveedor y haga click en \"Guardar\"";
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(312, 327);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(94, 29);
            btnGuardar.TabIndex = 20;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // txtMail
            // 
            txtMail.Location = new Point(187, 200);
            txtMail.Name = "txtMail";
            txtMail.Size = new Size(125, 27);
            txtMail.TabIndex = 19;
            // 
            // txtNombreyA
            // 
            txtNombreyA.Location = new Point(187, 151);
            txtNombreyA.Name = "txtNombreyA";
            txtNombreyA.Size = new Size(125, 27);
            txtNombreyA.TabIndex = 18;
            // 
            // txtDocumento
            // 
            txtDocumento.Location = new Point(187, 108);
            txtDocumento.Name = "txtDocumento";
            txtDocumento.Size = new Size(125, 27);
            txtDocumento.TabIndex = 17;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 203);
            label4.Name = "label4";
            label4.Size = new Size(41, 20);
            label4.TabIndex = 16;
            label4.Text = "Mail:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 132);
            label3.Name = "label3";
            label3.Size = new Size(0, 20);
            label3.TabIndex = 15;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 158);
            label2.Name = "label2";
            label2.Size = new Size(137, 20);
            label2.TabIndex = 14;
            label2.Text = "Nombre y apellido:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 115);
            label1.Name = "label1";
            label1.Size = new Size(90, 20);
            label1.TabIndex = 13;
            label1.Text = "Documento:";
            // 
            // txtEmpresa
            // 
            txtEmpresa.Location = new Point(187, 292);
            txtEmpresa.Name = "txtEmpresa";
            txtEmpresa.Size = new Size(125, 27);
            txtEmpresa.TabIndex = 25;
            // 
            // lblEmpresa
            // 
            lblEmpresa.AutoSize = true;
            lblEmpresa.Location = new Point(12, 295);
            lblEmpresa.Name = "lblEmpresa";
            lblEmpresa.Size = new Size(69, 20);
            lblEmpresa.TabIndex = 24;
            lblEmpresa.Text = "Empresa:";
            // 
            // FrmAMProveedor
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(418, 359);
            Controls.Add(txtEmpresa);
            Controls.Add(lblEmpresa);
            Controls.Add(txtTelefono);
            Controls.Add(lblTelefono);
            Controls.Add(lblIngresarDatos);
            Controls.Add(btnGuardar);
            Controls.Add(txtMail);
            Controls.Add(txtNombreyA);
            Controls.Add(txtDocumento);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(panelSuperior);
            Name = "FrmAMProveedor";
            panelSuperior.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panelSuperior;
        private PictureBox pbLogo;
        private Button btnCancelar;
        private TextBox txtTelefono;
        private Label lblTelefono;
        private Label lblIngresarDatos;
        private Button btnGuardar;
        private TextBox txtMail;
        private TextBox txtNombreyA;
        private TextBox txtDocumento;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private TextBox txtEmpresa;
        private Label lblEmpresa;
    }
}