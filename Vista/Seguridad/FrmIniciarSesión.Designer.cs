namespace Vista
{
    partial class FrmIniciarSesión
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmIniciarSesión));
            panel1 = new Panel();
            pbCerrar = new PictureBox();
            pictureBox1 = new PictureBox();
            txtUsuario = new TextBox();
            txtClave = new TextBox();
            label1 = new Label();
            lblRecuperarContraseña = new Label();
            btnAceptar = new Button();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbCerrar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Control;
            panel1.Controls.Add(pbCerrar);
            panel1.Controls.Add(pictureBox1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(415, 178);
            panel1.TabIndex = 0;
            // 
            // pbCerrar
            // 
            pbCerrar.Image = (Image)resources.GetObject("pbCerrar.Image");
            pbCerrar.Location = new Point(383, 12);
            pbCerrar.Name = "pbCerrar";
            pbCerrar.Size = new Size(20, 18);
            pbCerrar.SizeMode = PictureBoxSizeMode.Zoom;
            pbCerrar.TabIndex = 9;
            pbCerrar.TabStop = false;
            pbCerrar.Click += pbCerrar_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(415, 178);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // txtUsuario
            // 
            txtUsuario.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtUsuario.BackColor = Color.FromArgb(226, 232, 240);
            txtUsuario.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtUsuario.ForeColor = Color.FromArgb(45, 55, 72);
            txtUsuario.Location = new Point(34, 259);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(339, 34);
            txtUsuario.TabIndex = 1;
            txtUsuario.Text = "USUARIO";
            txtUsuario.TextAlign = HorizontalAlignment.Center;
            txtUsuario.Enter += txtUsuario_Enter;
            txtUsuario.Leave += txtUsuario_Leave;
            // 
            // txtClave
            // 
            txtClave.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtClave.BackColor = Color.FromArgb(226, 232, 240);
            txtClave.Font = new Font("Segoe UI", 12F);
            txtClave.ForeColor = Color.FromArgb(45, 55, 72);
            txtClave.Location = new Point(34, 321);
            txtClave.Name = "txtClave";
            txtClave.Size = new Size(339, 34);
            txtClave.TabIndex = 2;
            txtClave.Text = "CONTRASEÑA";
            txtClave.TextAlign = HorizontalAlignment.Center;
            txtClave.Enter += txtClave_Enter;
            txtClave.Leave += txtClave_Leave;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 24F);
            label1.ForeColor = SystemColors.Control;
            label1.Location = new Point(144, 181);
            label1.Name = "label1";
            label1.Size = new Size(139, 54);
            label1.TabIndex = 3;
            label1.Text = "LOGIN";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblRecuperarContraseña
            // 
            lblRecuperarContraseña.AutoSize = true;
            lblRecuperarContraseña.Font = new Font("Segoe UI", 10.2F, FontStyle.Underline, GraphicsUnit.Point, 0);
            lblRecuperarContraseña.ForeColor = Color.FromArgb(230, 213, 184);
            lblRecuperarContraseña.Location = new Point(121, 431);
            lblRecuperarContraseña.Name = "lblRecuperarContraseña";
            lblRecuperarContraseña.Size = new Size(171, 23);
            lblRecuperarContraseña.TabIndex = 5;
            lblRecuperarContraseña.Text = "Olvidé mi contraseña";
            lblRecuperarContraseña.Click += lblRecuperarContraseña_Click;
            // 
            // btnAceptar
            // 
            btnAceptar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnAceptar.BackColor = Color.FromArgb(230, 213, 184);
            btnAceptar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAceptar.ForeColor = Color.FromArgb(26, 47, 76);
            btnAceptar.Location = new Point(34, 377);
            btnAceptar.Name = "btnAceptar";
            btnAceptar.Size = new Size(339, 40);
            btnAceptar.TabIndex = 6;
            btnAceptar.Text = "INICIAR SESIÓN";
            btnAceptar.UseVisualStyleBackColor = false;
            btnAceptar.Click += btnAceptar_Click;
            // 
            // FrmIniciarSesión
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(76, 99, 120);
            ClientSize = new Size(415, 474);
            ControlBox = false;
            Controls.Add(btnAceptar);
            Controls.Add(lblRecuperarContraseña);
            Controls.Add(label1);
            Controls.Add(txtClave);
            Controls.Add(txtUsuario);
            Controls.Add(panel1);
            ForeColor = Color.FromArgb(26, 47, 76);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FrmIniciarSesión";
            StartPosition = FormStartPosition.CenterScreen;
            Load += FrmIniciarSesión_Load;
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbCerrar).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private TextBox txtUsuario;
        private TextBox txtClave;
        private Label label1;
        private Label lblRecuperarContraseña;
        private Button btnAceptar;
        private PictureBox pictureBox1;
        private PictureBox pbCerrar;
    }
}
