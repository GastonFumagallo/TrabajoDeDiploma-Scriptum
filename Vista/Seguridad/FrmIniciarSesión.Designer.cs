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
            lblLema = new Label();
            pictureBox1 = new PictureBox();
            pbCerrar = new PictureBox();
            txtUsuario = new TextBox();
            txtClave = new TextBox();
            label1 = new Label();
            lblSubtitulo = new Label();
            lblUsuarioTitulo = new Label();
            lblClaveTitulo = new Label();
            lblRecuperarContraseña = new Label();
            btnAceptar = new Button();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbCerrar).BeginInit();
            SuspendLayout();
            //
            // panel1
            //
            panel1.BackColor = Color.FromArgb(15, 23, 42);
            panel1.Controls.Add(lblLema);
            panel1.Controls.Add(pictureBox1);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(320, 460);
            panel1.TabIndex = 0;
            panel1.Tag = "Sidebar";
            //
            // lblLema
            //
            lblLema.Font = new Font("Segoe UI", 10F);
            lblLema.ForeColor = Color.FromArgb(203, 213, 225);
            lblLema.Location = new Point(30, 300);
            lblLema.Name = "lblLema";
            lblLema.Size = new Size(260, 72);
            lblLema.TabIndex = 1;
            lblLema.Text = "Gestión integral para librerías:\r\nventas, inventario y caja en un solo lugar.";
            lblLema.TextAlign = ContentAlignment.TopCenter;
            //
            // pictureBox1
            //
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(50, 110);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(220, 170);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            //
            // pbCerrar
            //
            pbCerrar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbCerrar.Cursor = Cursors.Hand;
            pbCerrar.Image = (Image)resources.GetObject("pbCerrar.Image");
            pbCerrar.Location = new Point(720, 12);
            pbCerrar.Name = "pbCerrar";
            pbCerrar.Size = new Size(28, 28);
            pbCerrar.SizeMode = PictureBoxSizeMode.CenterImage;
            pbCerrar.TabIndex = 9;
            pbCerrar.TabStop = false;
            pbCerrar.Click += pbCerrar_Click;
            //
            // label1
            //
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(15, 23, 42);
            label1.Location = new Point(370, 64);
            label1.Name = "label1";
            label1.Size = new Size(195, 41);
            label1.TabIndex = 3;
            label1.Text = "Iniciar sesión";
            //
            // lblSubtitulo
            //
            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Font = new Font("Segoe UI", 9.75F);
            lblSubtitulo.ForeColor = Color.FromArgb(100, 116, 139);
            lblSubtitulo.Location = new Point(373, 108);
            lblSubtitulo.Name = "lblSubtitulo";
            lblSubtitulo.Size = new Size(284, 23);
            lblSubtitulo.TabIndex = 10;
            lblSubtitulo.Text = "Ingresá tus credenciales para continuar.";
            //
            // lblUsuarioTitulo
            //
            lblUsuarioTitulo.AutoSize = true;
            lblUsuarioTitulo.Font = new Font("Segoe UI Semibold", 9.75F);
            lblUsuarioTitulo.ForeColor = Color.FromArgb(51, 65, 85);
            lblUsuarioTitulo.Location = new Point(373, 154);
            lblUsuarioTitulo.Name = "lblUsuarioTitulo";
            lblUsuarioTitulo.Size = new Size(62, 23);
            lblUsuarioTitulo.TabIndex = 11;
            lblUsuarioTitulo.Text = "Usuario";
            //
            // txtUsuario
            //
            txtUsuario.BackColor = Color.White;
            txtUsuario.BorderStyle = BorderStyle.FixedSingle;
            txtUsuario.Font = new Font("Segoe UI", 11F);
            txtUsuario.ForeColor = Color.FromArgb(15, 23, 42);
            txtUsuario.Location = new Point(373, 180);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(340, 32);
            txtUsuario.TabIndex = 1;
            txtUsuario.Text = "USUARIO";
            txtUsuario.Enter += txtUsuario_Enter;
            txtUsuario.Leave += txtUsuario_Leave;
            //
            // lblClaveTitulo
            //
            lblClaveTitulo.AutoSize = true;
            lblClaveTitulo.Font = new Font("Segoe UI Semibold", 9.75F);
            lblClaveTitulo.ForeColor = Color.FromArgb(51, 65, 85);
            lblClaveTitulo.Location = new Point(373, 230);
            lblClaveTitulo.Name = "lblClaveTitulo";
            lblClaveTitulo.Size = new Size(89, 23);
            lblClaveTitulo.TabIndex = 12;
            lblClaveTitulo.Text = "Contraseña";
            //
            // txtClave
            //
            txtClave.BackColor = Color.White;
            txtClave.BorderStyle = BorderStyle.FixedSingle;
            txtClave.Font = new Font("Segoe UI", 11F);
            txtClave.ForeColor = Color.FromArgb(15, 23, 42);
            txtClave.Location = new Point(373, 256);
            txtClave.Name = "txtClave";
            txtClave.Size = new Size(340, 32);
            txtClave.TabIndex = 2;
            txtClave.Text = "CONTRASEÑA";
            txtClave.Enter += txtClave_Enter;
            txtClave.Leave += txtClave_Leave;
            //
            // btnAceptar
            //
            btnAceptar.BackColor = Color.FromArgb(37, 99, 235);
            btnAceptar.Cursor = Cursors.Hand;
            btnAceptar.FlatAppearance.BorderSize = 0;
            btnAceptar.FlatStyle = FlatStyle.Flat;
            btnAceptar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAceptar.ForeColor = Color.White;
            btnAceptar.Location = new Point(373, 316);
            btnAceptar.Name = "btnAceptar";
            btnAceptar.Size = new Size(340, 42);
            btnAceptar.TabIndex = 6;
            btnAceptar.Tag = "Primario";
            btnAceptar.Text = "Iniciar sesión";
            btnAceptar.UseVisualStyleBackColor = false;
            btnAceptar.Click += btnAceptar_Click;
            //
            // lblRecuperarContraseña
            //
            lblRecuperarContraseña.AutoSize = true;
            lblRecuperarContraseña.Cursor = Cursors.Hand;
            lblRecuperarContraseña.Font = new Font("Segoe UI Semibold", 9.75F);
            lblRecuperarContraseña.ForeColor = Color.FromArgb(37, 99, 235);
            lblRecuperarContraseña.Location = new Point(468, 378);
            lblRecuperarContraseña.Name = "lblRecuperarContraseña";
            lblRecuperarContraseña.Size = new Size(160, 23);
            lblRecuperarContraseña.TabIndex = 5;
            lblRecuperarContraseña.Text = "Olvidé mi contraseña";
            lblRecuperarContraseña.Click += lblRecuperarContraseña_Click;
            //
            // FrmIniciarSesión
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(760, 460);
            ControlBox = false;
            Controls.Add(pbCerrar);
            Controls.Add(btnAceptar);
            Controls.Add(lblRecuperarContraseña);
            Controls.Add(txtClave);
            Controls.Add(lblClaveTitulo);
            Controls.Add(txtUsuario);
            Controls.Add(lblUsuarioTitulo);
            Controls.Add(lblSubtitulo);
            Controls.Add(label1);
            Controls.Add(panel1);
            ForeColor = Color.FromArgb(71, 85, 105);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FrmIniciarSesión";
            StartPosition = FormStartPosition.CenterScreen;
            Load += FrmIniciarSesión_Load;
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbCerrar).EndInit();
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
        private Label lblLema;
        private Label lblSubtitulo;
        private Label lblUsuarioTitulo;
        private Label lblClaveTitulo;
    }
}
