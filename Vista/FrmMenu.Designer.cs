namespace Vista
{
    partial class FrmMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMenu));
            panelMenu = new Panel();
            btnGestionarInventario = new Button();
            btnMiClave = new Button();
            panel2 = new Panel();
            pictureBox1 = new PictureBox();
            panel1 = new Panel();
            btnCerrarSesion = new Button();
            btnGrupos = new Button();
            btnUsuarios = new Button();
            btnReportes = new Button();
            button6 = new Button();
            btnVentas = new Button();
            btnProveedores = new Button();
            btnLibros = new Button();
            btnClientes = new Button();
            btnInicio = new Button();
            lblTitulo = new Label();
            lblUsuarioSesion = new Label();
            panelTitulo = new Panel();
            panelTituloCerrar = new Panel();
            pbCerrar = new PictureBox();
            panelForm = new Panel();
            panelMenu.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panelTitulo.SuspendLayout();
            panelTituloCerrar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbCerrar).BeginInit();
            SuspendLayout();
            // 
            // panelMenu
            // 
            panelMenu.BackColor = SystemColors.Control;
            panelMenu.Controls.Add(btnMiClave);
            panelMenu.Controls.Add(btnGestionarInventario);
            panelMenu.Controls.Add(panel2);
            panelMenu.Controls.Add(panel1);
            panelMenu.Controls.Add(btnCerrarSesion);
            panelMenu.Controls.Add(btnGrupos);
            panelMenu.Controls.Add(btnUsuarios);
            panelMenu.Controls.Add(btnReportes);
            panelMenu.Controls.Add(button6);
            panelMenu.Controls.Add(btnVentas);
            panelMenu.Controls.Add(btnProveedores);
            panelMenu.Controls.Add(btnLibros);
            panelMenu.Controls.Add(btnClientes);
            panelMenu.Controls.Add(btnInicio);
            panelMenu.Dock = DockStyle.Left;
            panelMenu.Location = new Point(0, 0);
            panelMenu.Name = "panelMenu";
            panelMenu.Size = new Size(248, 760);
            panelMenu.Tag = "Sidebar";
            panelMenu.TabIndex = 0;
            // 
            // btnGestionarInventario
            // 
            btnGestionarInventario.BackColor = SystemColors.Control;
            btnGestionarInventario.Font = new Font("Segoe UI Semibold", 10.5F);
            btnGestionarInventario.Location = new Point(0, 370);
            btnGestionarInventario.Name = "btnGestionarInventario";
            btnGestionarInventario.Size = new Size(248, 46);
            btnGestionarInventario.TabIndex = 12;
            btnGestionarInventario.Text = "INVENTARIO";
            btnGestionarInventario.UseVisualStyleBackColor = false;
            btnGestionarInventario.Click += btnGestionarInventario_Click;
            // 
            // btnMiClave
            // 
            btnMiClave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnMiClave.BackColor = SystemColors.Control;
            btnMiClave.Font = new Font("Segoe UI Semibold", 10.5F);
            btnMiClave.Location = new Point(0, 512);
            btnMiClave.Name = "btnMiClave";
            btnMiClave.Size = new Size(248, 46);
            btnMiClave.TabIndex = 13;
            btnMiClave.Text = "MI CLAVE";
            btnMiClave.UseVisualStyleBackColor = false;
            btnMiClave.Click += btnMiClave_Click;
            // 
            // panel2
            // 
            panel2.Controls.Add(pictureBox1);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Padding = new Padding(28, 22, 28, 14);
            panel2.Size = new Size(248, 132);
            panel2.TabIndex = 11;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(192, 96);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            panel1.Location = new Point(327, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(250, 125);
            panel1.TabIndex = 1;
            // 
            // btnCerrarSesion
            // 
            btnCerrarSesion.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnCerrarSesion.BackColor = SystemColors.Control;
            btnCerrarSesion.Font = new Font("Segoe UI Semibold", 10.5F);
            btnCerrarSesion.Location = new Point(0, 702);
            btnCerrarSesion.Name = "btnCerrarSesion";
            btnCerrarSesion.Size = new Size(248, 46);
            btnCerrarSesion.TabIndex = 10;
            btnCerrarSesion.Text = "CERRAR SESIÓN";
            btnCerrarSesion.UseVisualStyleBackColor = false;
            btnCerrarSesion.Click += btnCerrarSesion_Click;
            // 
            // btnGrupos
            // 
            btnGrupos.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnGrupos.BackColor = SystemColors.Control;
            btnGrupos.Font = new Font("Segoe UI Semibold", 10.5F);
            btnGrupos.Location = new Point(0, 650);
            btnGrupos.Name = "btnGrupos";
            btnGrupos.Size = new Size(248, 46);
            btnGrupos.TabIndex = 9;
            btnGrupos.Text = "GRUPOS";
            btnGrupos.UseVisualStyleBackColor = false;
            btnGrupos.Click += btnGrupos_Click;
            // 
            // btnUsuarios
            // 
            btnUsuarios.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnUsuarios.BackColor = SystemColors.Control;
            btnUsuarios.Font = new Font("Segoe UI Semibold", 10.5F);
            btnUsuarios.Location = new Point(0, 604);
            btnUsuarios.Name = "btnUsuarios";
            btnUsuarios.Size = new Size(248, 46);
            btnUsuarios.TabIndex = 8;
            btnUsuarios.Text = "USUARIOS";
            btnUsuarios.UseVisualStyleBackColor = false;
            btnUsuarios.Click += btnUsuarios_Click;
            // 
            // btnReportes
            // 
            btnReportes.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnReportes.BackColor = SystemColors.Control;
            btnReportes.Font = new Font("Segoe UI Semibold", 10.5F);
            btnReportes.Location = new Point(0, 558);
            btnReportes.Name = "btnReportes";
            btnReportes.Size = new Size(248, 46);
            btnReportes.TabIndex = 7;
            btnReportes.Text = "REPORTES";
            btnReportes.UseVisualStyleBackColor = false;
            btnReportes.Click += btnReportes_Click;
            // 
            // button6
            // 
            button6.Location = new Point(0, 573);
            button6.Name = "button6";
            button6.Size = new Size(0, 0);
            button6.TabIndex = 6;
            button6.Text = "button6";
            button6.UseVisualStyleBackColor = true;
            // 
            // btnVentas
            // 
            btnVentas.BackColor = SystemColors.Control;
            btnVentas.Font = new Font("Segoe UI Semibold", 10.5F);
            btnVentas.Location = new Point(0, 324);
            btnVentas.Name = "btnVentas";
            btnVentas.Size = new Size(248, 46);
            btnVentas.TabIndex = 5;
            btnVentas.Text = "VENTAS";
            btnVentas.UseVisualStyleBackColor = false;
            btnVentas.Click += btnVentas_Click;
            // 
            // btnProveedores
            // 
            btnProveedores.BackColor = SystemColors.Control;
            btnProveedores.Font = new Font("Segoe UI Semibold", 10.5F);
            btnProveedores.Location = new Point(0, 278);
            btnProveedores.Name = "btnProveedores";
            btnProveedores.Size = new Size(248, 46);
            btnProveedores.TabIndex = 4;
            btnProveedores.Text = "PROVEEDORES";
            btnProveedores.UseVisualStyleBackColor = false;
            btnProveedores.Click += btnProveedores_Click;
            // 
            // btnLibros
            // 
            btnLibros.BackColor = SystemColors.Control;
            btnLibros.Font = new Font("Segoe UI Semibold", 10.5F);
            btnLibros.Location = new Point(0, 232);
            btnLibros.Name = "btnLibros";
            btnLibros.Size = new Size(248, 46);
            btnLibros.TabIndex = 3;
            btnLibros.Text = "LIBROS";
            btnLibros.UseVisualStyleBackColor = false;
            btnLibros.Click += btnLibros_Click;
            // 
            // btnClientes
            // 
            btnClientes.BackColor = SystemColors.Control;
            btnClientes.Font = new Font("Segoe UI Semibold", 10.5F);
            btnClientes.Location = new Point(0, 186);
            btnClientes.Name = "btnClientes";
            btnClientes.Size = new Size(248, 46);
            btnClientes.TabIndex = 2;
            btnClientes.Text = "CLIENTES";
            btnClientes.UseVisualStyleBackColor = false;
            btnClientes.Click += btnClientes_Click;
            // 
            // btnInicio
            // 
            btnInicio.BackColor = SystemColors.Control;
            btnInicio.Font = new Font("Segoe UI Semibold", 10.5F);
            
            btnInicio.Location = new Point(0, 140);
            btnInicio.Name = "btnInicio";
            btnInicio.Size = new Size(248, 46);
            btnInicio.TabIndex = 1;
            btnInicio.Text = "INICIO";
            btnInicio.UseVisualStyleBackColor = false;
            btnInicio.Click += btnInicio_Click;
            // 
            // lblTitulo
            // 
            lblTitulo.BackColor = SystemColors.Control;
            lblTitulo.Dock = DockStyle.Fill;
            lblTitulo.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
            lblTitulo.Padding = new Padding(28, 0, 0, 0);
            lblTitulo.ForeColor = Color.FromArgb(15, 23, 42);
            lblTitulo.ImageAlign = ContentAlignment.MiddleLeft;
            lblTitulo.Location = new Point(0, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(656, 63);
            lblTitulo.TabIndex = 2;
            lblTitulo.Tag = "Titulo";
            lblTitulo.Text = "INICIO";
            lblTitulo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblUsuarioSesion
            // 
            lblUsuarioSesion.Dock = DockStyle.Right;
            lblUsuarioSesion.Font = new Font("Segoe UI", 9.75F);
            lblUsuarioSesion.Location = new Point(656, 0);
            lblUsuarioSesion.Name = "lblUsuarioSesion";
            lblUsuarioSesion.Padding = new Padding(0, 0, 8, 0);
            lblUsuarioSesion.Size = new Size(320, 64);
            lblUsuarioSesion.TabIndex = 3;
            lblUsuarioSesion.TextAlign = ContentAlignment.MiddleRight;
            // 
            // panelTitulo
            // 
            panelTitulo.Controls.Add(lblTitulo);
            panelTitulo.Controls.Add(lblUsuarioSesion);
            panelTitulo.Controls.Add(panelTituloCerrar);
            panelTitulo.Dock = DockStyle.Top;
            panelTitulo.Location = new Point(248, 0);
            panelTitulo.Name = "panelTitulo";
            panelTitulo.Size = new Size(1032, 64);
            panelTitulo.Padding = new Padding(0, 0, 0, 1);
            panelTitulo.Tag = "Encabezado";
            panelTitulo.TabIndex = 2;
            // 
            // panelTituloCerrar
            // 
            panelTituloCerrar.BackColor = SystemColors.Control;
            panelTituloCerrar.Controls.Add(pbCerrar);
            panelTituloCerrar.Dock = DockStyle.Right;
            panelTituloCerrar.Location = new Point(976, 0);
            panelTituloCerrar.Name = "panelTituloCerrar";
            panelTituloCerrar.Size = new Size(56, 64);
            panelTituloCerrar.TabIndex = 0;
            // 
            // pbCerrar
            // 
            pbCerrar.Image = (Image)resources.GetObject("pbCerrar.Image");
            pbCerrar.Cursor = Cursors.Hand;
            pbCerrar.Location = new Point(12, 16);
            pbCerrar.Name = "pbCerrar";
            pbCerrar.Size = new Size(32, 32);
            pbCerrar.SizeMode = PictureBoxSizeMode.Zoom;
            pbCerrar.TabIndex = 0;
            pbCerrar.TabStop = false;
            pbCerrar.Click += pbCerrar_Click;
            // 
            // panelForm
            // 
            panelForm.BackColor = SystemColors.Control;
            panelForm.Padding = new Padding(24, 20, 24, 20);
            panelForm.Dock = DockStyle.Fill;
            panelForm.Location = new Point(248, 64);
            panelForm.Name = "panelForm";
            panelForm.Size = new Size(1032, 696);
            panelForm.TabIndex = 3;
            // 
            // FrmMenu
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(1280, 760);
            ControlBox = false;
            Controls.Add(panelForm);
            Controls.Add(panelTitulo);
            Controls.Add(panelMenu);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FrmMenu";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            Load += FrmMenu_Load;
            panelMenu.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panelTitulo.ResumeLayout(false);
            panelTituloCerrar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbCerrar).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelMenu;
        private Button btnUsuarios;
        private Button btnReportes;
        private Button button6;
        private Button btnVentas;
        private Button btnProveedores;
        private Button btnLibros;
        private Button btnClientes;
        private Button btnInicio;
        private Button btnCerrarSesion;
        private Button btnGrupos;
        private Panel panel1;
        private PictureBox pictureBox1;
        private Panel panel2;
        private Label lblTitulo;
        private Label lblUsuarioSesion;
        private Panel panelTituloCerrar;
        private Panel panelTitulo;
        private PictureBox pbCerrar;
        private Panel panelForm;
        private Button btnGestionarInventario;
        private Button btnMiClave;
    }
}
