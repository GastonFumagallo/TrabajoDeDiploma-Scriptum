namespace Vista
{
    partial class FrmAgregarCliente
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAgregarCliente));
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            txtDocumento = new TextBox();
            txtNombreyA = new TextBox();
            btnGuardar = new Button();
            btnCancelar = new Button();
            panelSuperior = new Panel();
            pbLogo = new PictureBox();
            label5 = new Label();
            txtTelefono = new TextBox();
            txtMail = new TextBox();
            lblTelefono = new Label();
            label4 = new Label();
            panelSuperior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbLogo).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 117);
            label1.Name = "label1";
            label1.Size = new Size(90, 20);
            label1.TabIndex = 0;
            label1.Text = "Documento:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 160);
            label2.Name = "label2";
            label2.Size = new Size(137, 20);
            label2.TabIndex = 1;
            label2.Text = "Nombre y apellido:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 134);
            label3.Name = "label3";
            label3.Size = new Size(0, 20);
            label3.TabIndex = 2;
            // 
            // txtDocumento
            // 
            txtDocumento.Location = new Point(187, 110);
            txtDocumento.Name = "txtDocumento";
            txtDocumento.Size = new Size(125, 27);
            txtDocumento.TabIndex = 4;
            // 
            // txtNombreyA
            // 
            txtNombreyA.Location = new Point(187, 153);
            txtNombreyA.Name = "txtNombreyA";
            txtNombreyA.Size = new Size(125, 27);
            txtNombreyA.TabIndex = 5;
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(312, 294);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(94, 29);
            btnGuardar.TabIndex = 7;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
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
            // panelSuperior
            // 
            panelSuperior.Controls.Add(pbLogo);
            panelSuperior.Controls.Add(btnCancelar);
            panelSuperior.Location = new Point(0, 0);
            panelSuperior.Name = "panelSuperior";
            panelSuperior.Size = new Size(418, 61);
            panelSuperior.TabIndex = 9;
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
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 77);
            label5.Name = "label5";
            label5.Size = new Size(366, 20);
            label5.TabIndex = 10;
            label5.Text = "Ingrese los datos del Cliente y haga click en \"Guardar\"";
            // 
            // txtTelefono
            // 
            txtTelefono.Location = new Point(187, 245);
            txtTelefono.Name = "txtTelefono";
            txtTelefono.Size = new Size(125, 27);
            txtTelefono.TabIndex = 12;
            // 
            // txtMail
            // 
            txtMail.Location = new Point(187, 202);
            txtMail.Name = "txtMail";
            txtMail.Size = new Size(125, 27);
            txtMail.TabIndex = 6;
            // 
            // lblTelefono
            // 
            lblTelefono.AutoSize = true;
            lblTelefono.Location = new Point(12, 248);
            lblTelefono.Name = "lblTelefono";
            lblTelefono.Size = new Size(70, 20);
            lblTelefono.TabIndex = 11;
            lblTelefono.Text = "Telefono:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 205);
            label4.Name = "label4";
            label4.Size = new Size(41, 20);
            label4.TabIndex = 3;
            label4.Text = "Mail:";
            // 
            // FrmAgregarCliente
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(417, 330);
            Controls.Add(txtTelefono);
            Controls.Add(lblTelefono);
            Controls.Add(label5);
            Controls.Add(panelSuperior);
            Controls.Add(btnGuardar);
            Controls.Add(txtMail);
            Controls.Add(txtNombreyA);
            Controls.Add(txtDocumento);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "FrmAgregarCliente";
            panelSuperior.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox txtDocumento;
        private TextBox txtNombreyA;
        private Button btnGuardar;
        private Button btnCancelar;
        private Panel panelSuperior;
        private PictureBox pbLogo;
        private Label label5;
        private TextBox txtTelefono;
        private TextBox txtMail;
        private Label lblTelefono;
        private Label label4;
    }
}