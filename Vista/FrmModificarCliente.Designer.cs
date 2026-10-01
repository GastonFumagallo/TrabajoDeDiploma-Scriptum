namespace Vista
{
    partial class FrmModificarCliente
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmModificarCliente));
            txtTelefono = new TextBox();
            lblTelefono = new Label();
            label5 = new Label();
            btnGuardar = new Button();
            txtMail = new TextBox();
            txtNombreyA = new TextBox();
            txtDocumento = new TextBox();
            label4 = new Label();
            label2 = new Label();
            label1 = new Label();
            panelSuperior = new Panel();
            pbLogo = new PictureBox();
            btnCancelar = new Button();
            panelSuperior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbLogo).BeginInit();
            SuspendLayout();
            // 
            // txtTelefono
            // 
            txtTelefono.Location = new Point(187, 250);
            txtTelefono.Name = "txtTelefono";
            txtTelefono.Size = new Size(245, 27);
            txtTelefono.TabIndex = 22;
            // 
            // lblTelefono
            // 
            lblTelefono.AutoSize = true;
            lblTelefono.Location = new Point(12, 257);
            lblTelefono.Name = "lblTelefono";
            lblTelefono.Size = new Size(70, 20);
            lblTelefono.TabIndex = 21;
            lblTelefono.Text = "Telefono:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 82);
            label5.Name = "label5";
            label5.Size = new Size(506, 20);
            label5.TabIndex = 20;
            label5.Text = "Ingrese los datos que desea modificar del Cliente y haga click en \"Guardar\"";
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(418, 297);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(94, 29);
            btnGuardar.TabIndex = 19;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // txtMail
            // 
            txtMail.Location = new Point(187, 207);
            txtMail.Name = "txtMail";
            txtMail.Size = new Size(245, 27);
            txtMail.TabIndex = 18;
            // 
            // txtNombreyA
            // 
            txtNombreyA.Location = new Point(187, 158);
            txtNombreyA.Name = "txtNombreyA";
            txtNombreyA.Size = new Size(245, 27);
            txtNombreyA.TabIndex = 17;
            // 
            // txtDocumento
            // 
            txtDocumento.Location = new Point(187, 115);
            txtDocumento.Name = "txtDocumento";
            txtDocumento.Size = new Size(245, 27);
            txtDocumento.TabIndex = 16;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 214);
            label4.Name = "label4";
            label4.Size = new Size(41, 20);
            label4.TabIndex = 15;
            label4.Text = "Mail:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 165);
            label2.Name = "label2";
            label2.Size = new Size(137, 20);
            label2.TabIndex = 14;
            label2.Text = "Nombre y apellido:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 122);
            label1.Name = "label1";
            label1.Size = new Size(90, 20);
            label1.TabIndex = 13;
            label1.Text = "Documento:";
            // 
            // panelSuperior
            // 
            panelSuperior.Controls.Add(pbLogo);
            panelSuperior.Controls.Add(btnCancelar);
            panelSuperior.Location = new Point(0, -1);
            panelSuperior.Name = "panelSuperior";
            panelSuperior.Size = new Size(518, 61);
            panelSuperior.TabIndex = 23;
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
            btnCancelar.Location = new Point(418, 13);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(94, 29);
            btnCancelar.TabIndex = 8;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // FrmModificarCliente
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(524, 338);
            Controls.Add(panelSuperior);
            Controls.Add(txtTelefono);
            Controls.Add(lblTelefono);
            Controls.Add(label5);
            Controls.Add(btnGuardar);
            Controls.Add(txtMail);
            Controls.Add(txtNombreyA);
            Controls.Add(txtDocumento);
            Controls.Add(label4);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "FrmModificarCliente";
            panelSuperior.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtTelefono;
        private Label lblTelefono;
        private Label label5;
        private Button btnGuardar;
        private TextBox txtMail;
        private TextBox txtNombreyA;
        private TextBox txtDocumento;
        private Label label4;
        private Label label2;
        private Label label1;
        private Panel panelSuperior;
        private PictureBox pbLogo;
        private Button btnCancelar;
    }
}