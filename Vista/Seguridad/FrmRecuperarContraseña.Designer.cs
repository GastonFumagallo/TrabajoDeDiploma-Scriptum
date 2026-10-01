namespace Vista.Seguridad
{
    partial class FrmRecuperarContraseña
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmRecuperarContraseña));
            btnAceptar = new Button();
            lblRecuperarContraseña = new Label();
            label2 = new Label();
            label1 = new Label();
            txtEmail = new TextBox();
            txtUsuario = new TextBox();
            panelSuperior = new Panel();
            btnCancelar = new Button();
            pbLogo = new PictureBox();
            panelSuperior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbLogo).BeginInit();
            SuspendLayout();
            // 
            // btnAceptar
            // 
            btnAceptar.Location = new Point(462, 214);
            btnAceptar.Margin = new Padding(3, 4, 3, 4);
            btnAceptar.Name = "btnAceptar";
            btnAceptar.Size = new Size(101, 53);
            btnAceptar.TabIndex = 14;
            btnAceptar.Text = "Aceptar";
            btnAceptar.UseVisualStyleBackColor = true;
            btnAceptar.Click += btnAceptar_Click;
            // 
            // lblRecuperarContraseña
            // 
            lblRecuperarContraseña.AutoSize = true;
            lblRecuperarContraseña.Font = new Font("Consolas", 9.75F, FontStyle.Underline);
            lblRecuperarContraseña.ForeColor = Color.DarkBlue;
            lblRecuperarContraseña.Location = new Point(28, 214);
            lblRecuperarContraseña.Margin = new Padding(5, 0, 5, 0);
            lblRecuperarContraseña.Name = "lblRecuperarContraseña";
            lblRecuperarContraseña.Size = new Size(0, 20);
            lblRecuperarContraseña.TabIndex = 13;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(28, 166);
            label2.Margin = new Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new Size(63, 28);
            label2.TabIndex = 12;
            label2.Text = "Email:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(28, 79);
            label1.Margin = new Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(83, 28);
            label1.TabIndex = 11;
            label1.Text = "Usuario:";
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(28, 198);
            txtEmail.Margin = new Padding(5, 4, 5, 4);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(283, 27);
            txtEmail.TabIndex = 10;
            // 
            // txtUsuario
            // 
            txtUsuario.Location = new Point(28, 112);
            txtUsuario.Margin = new Padding(5, 4, 5, 4);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(283, 27);
            txtUsuario.TabIndex = 9;
            // 
            // panelSuperior
            // 
            panelSuperior.Controls.Add(btnCancelar);
            panelSuperior.Controls.Add(pbLogo);
            panelSuperior.Dock = DockStyle.Top;
            panelSuperior.Location = new Point(0, 0);
            panelSuperior.Name = "panelSuperior";
            panelSuperior.Size = new Size(575, 56);
            panelSuperior.TabIndex = 17;
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(469, 12);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(94, 29);
            btnCancelar.TabIndex = 18;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
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
            // FrmRecuperarContraseña
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(575, 271);
            ControlBox = false;
            Controls.Add(panelSuperior);
            Controls.Add(btnAceptar);
            Controls.Add(lblRecuperarContraseña);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtEmail);
            Controls.Add(txtUsuario);
            Name = "FrmRecuperarContraseña";
            panelSuperior.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnAceptar;
        private Label lblRecuperarContraseña;
        private Label label2;
        private Label label1;
        private TextBox txtEmail;
        private TextBox txtUsuario;
        private Panel panelSuperior;
        private PictureBox pbLogo;
        private Button btnCancelar;
    }
}