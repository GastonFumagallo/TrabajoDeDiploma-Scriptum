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
            panelMenu.Size = new Size(321, 1100);
            panelMenu.TabIndex = 0;
            // 
            // btnGestionarInventario
            // 
            btnGestionarInventario.BackColor = SystemColors.Control;
            btnGestionarInventario.Font = new Font("Segoe UI Semibold", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnGestionarInventario.Location = new Point(0, 594);
            btnGestionarInventario.Name = "btnGestionarInventario";
            btnGestionarInventario.Size = new Size(321, 80);
            btnGestionarInventario.TabIndex = 12;
            btnGestionarInventario.Text = "INVENTARIO";
            btnGestionarInventario.UseVisualStyleBackColor = false;
            btnGestionarInventario.Click += btnGestionarInventario_Click;
            //
            // btnMiClave
            //
            btnMiClave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnMiClave.BackColor = SystemColors.Control;
            btnMiClave.Font = new Font("Segoe UI Semibold", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnMiClave.Location = new Point(0, 687);
            btnMiClave.Name = "btnMiClave";
            btnMiClave.Size = new Size(321, 80);
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
            panel2.Size = new Size(321, 193);
            panel2.TabIndex = 11;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(321, 193);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
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
            btnCerrarSesion.Font = new Font("Segoe UI Semibold", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCerrarSesion.Location = new Point(0, 1020);
            btnCerrarSesion.Name = "btnCerrarSesion";
            btnCerrarSesion.Size = new Size(321, 80);
            btnCerrarSesion.TabIndex = 10;
            btnCerrarSesion.Text = "CERRAR SESIÓN";
            btnCerrarSesion.UseVisualStyleBackColor = false;
            btnCerrarSesion.Click += btnCerrarSesion_Click;
            // 
            // btnGrupos
            // 
            btnGrupos.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnGrupos.BackColor = SystemColors.Control;
            btnGrupos.Font = new Font("Segoe UI Semibold", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnGrupos.Location = new Point(0, 940);
            btnGrupos.Name = "btnGrupos";
            btnGrupos.Size = new Size(321, 80);
            btnGrupos.TabIndex = 9;
            btnGrupos.Text = "GRUPOS";
            btnGrupos.UseVisualStyleBackColor = false;
            btnGrupos.Click += btnGrupos_Click;
            // 
            // btnUsuarios
            // 
            btnUsuarios.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnUsuarios.BackColor = SystemColors.Control;
            btnUsuarios.Font = new Font("Segoe UI Semibold", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnUsuarios.Location = new Point(0, 860);
            btnUsuarios.Name = "btnUsuarios";
            btnUsuarios.Size = new Size(321, 80);
            btnUsuarios.TabIndex = 8;
            btnUsuarios.Text = "USUARIOS";
            btnUsuarios.UseVisualStyleBackColor = false;
            btnUsuarios.Click += btnUsuarios_Click;
            // 
            // btnReportes
            // 
            btnReportes.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnReportes.BackColor = SystemColors.Control;
            btnReportes.Font = new Font("Segoe UI Semibold", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnReportes.Location = new Point(0, 780);
            btnReportes.Name = "btnReportes";
            btnReportes.Size = new Size(321, 80);
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
            btnVentas.Font = new Font("Segoe UI Semibold", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnVentas.Location = new Point(0, 513);
            btnVentas.Name = "btnVentas";
            btnVentas.Size = new Size(321, 80);
            btnVentas.TabIndex = 5;
            btnVentas.Text = "VENTAS";
            btnVentas.UseVisualStyleBackColor = false;
            btnVentas.Click += btnVentas_Click;
            // 
            // btnProveedores
            // 
            btnProveedores.BackColor = SystemColors.Control;
            btnProveedores.Font = new Font("Segoe UI Semibold", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnProveedores.Location = new Point(0, 433);
            btnProveedores.Name = "btnProveedores";
            btnProveedores.Size = new Size(321, 80);
            btnProveedores.TabIndex = 4;
            btnProveedores.Text = "PROVEEDORES";
            btnProveedores.UseVisualStyleBackColor = false;
            btnProveedores.Click += btnProveedores_Click;
            // 
            // btnLibros
            // 
            btnLibros.BackColor = SystemColors.Control;
            btnLibros.Font = new Font("Segoe UI Semibold", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLibros.Location = new Point(0, 353);
            btnLibros.Name = "btnLibros";
            btnLibros.Size = new Size(321, 80);
            btnLibros.TabIndex = 3;
            btnLibros.Text = "LIBROS";
            btnLibros.UseVisualStyleBackColor = false;
            btnLibros.Click += btnLibros_Click;
            // 
            // btnClientes
            // 
            btnClientes.BackColor = SystemColors.Control;
            btnClientes.Font = new Font("Segoe UI Semibold", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnClientes.Location = new Point(0, 273);
            btnClientes.Name = "btnClientes";
            btnClientes.Size = new Size(321, 80);
            btnClientes.TabIndex = 2;
            btnClientes.Text = "CLIENTES";
            btnClientes.UseVisualStyleBackColor = false;
            btnClientes.Click += btnClientes_Click;
            // 
            // btnInicio
            // 
            btnInicio.BackColor = SystemColors.Control;
            btnInicio.Font = new Font("Segoe UI Semibold", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnInicio.ForeColor = Color.Black;
            btnInicio.Location = new Point(0, 193);
            btnInicio.Name = "btnInicio";
            btnInicio.Size = new Size(321, 80);
            btnInicio.TabIndex = 1;
            btnInicio.Text = "INICIO";
            btnInicio.UseVisualStyleBackColor = false;
            btnInicio.Click += btnInicio_Click;
            // 
            // lblTitulo
            // 
            lblTitulo.BackColor = SystemColors.Control;
            lblTitulo.Dock = DockStyle.Fill;
            lblTitulo.Font = new Font("Segoe UI Semibold", 30F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.ImageAlign = ContentAlignment.MiddleLeft;
            lblTitulo.Location = new Point(0, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(928, 60);
            lblTitulo.TabIndex = 2;
            lblTitulo.Tag = "Titulo";
            lblTitulo.Text = "INICIO";
            lblTitulo.TextAlign = ContentAlignment.TopCenter;
            // 
            // panelTitulo
            // 
            panelTitulo.Controls.Add(panelTituloCerrar);
            panelTitulo.Controls.Add(lblTitulo);
            panelTitulo.Dock = DockStyle.Top;
            panelTitulo.Location = new Point(321, 0);
            panelTitulo.Name = "panelTitulo";
            panelTitulo.Size = new Size(928, 60);
            panelTitulo.TabIndex = 2;
            // 
            // panelTituloCerrar
            // 
            panelTituloCerrar.BackColor = SystemColors.Control;
            panelTituloCerrar.Controls.Add(pbCerrar);
            panelTituloCerrar.Dock = DockStyle.Right;
            panelTituloCerrar.Location = new Point(887, 0);
            panelTituloCerrar.Name = "panelTituloCerrar";
            panelTituloCerrar.Size = new Size(41, 60);
            panelTituloCerrar.TabIndex = 0;
            // 
            // pbCerrar
            // 
            pbCerrar.Image = (Image)resources.GetObject("pbCerrar.Image");
            pbCerrar.Location = new Point(13, 12);
            pbCerrar.Name = "pbCerrar";
            pbCerrar.Size = new Size(16, 16);
            pbCerrar.SizeMode = PictureBoxSizeMode.Zoom;
            pbCerrar.TabIndex = 0;
            pbCerrar.TabStop = false;
            pbCerrar.Click += pbCerrar_Click;
            // 
            // panelForm
            // 
            panelForm.BackColor = SystemColors.Control;
            panelForm.Cursor = Cursors.Hand;
            panelForm.Dock = DockStyle.Fill;
            panelForm.Location = new Point(321, 60);
            panelForm.Name = "panelForm";
            panelForm.Size = new Size(928, 1040);
            panelForm.TabIndex = 3;
            // 
            // FrmMenu
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = SystemColors.Control;
            ClientSize = new Size(1249, 1100);
            ControlBox = false;
            Controls.Add(panelForm);
            Controls.Add(panelTitulo);
            Controls.Add(panelMenu);
            Cursor = Cursors.Hand;
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
        private Panel panelTituloCerrar;
        private Panel panelTitulo;
        private PictureBox pbCerrar;
        private Panel panelForm;
        private Button btnGestionarInventario;
        private Button btnMiClave;
    }
}