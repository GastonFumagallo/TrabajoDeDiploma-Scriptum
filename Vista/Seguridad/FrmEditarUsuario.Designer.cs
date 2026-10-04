namespace Vista.Seguridad
{
    partial class FrmEditarUsuario
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
            components = new System.ComponentModel.Container();
            tabFicha = new TabControl();
            tabDatos = new TabPage();
            lblClaveInfo = new Label();
            txtTelefono = new TextBox();
            lblTelefono = new Label();
            txtDni = new TextBox();
            lblDni = new Label();
            txtEmail = new TextBox();
            lblEmail = new Label();
            txtNombre = new TextBox();
            lblNombre = new Label();
            lblUsuarioInfo = new Label();
            txtUsuario = new TextBox();
            lblUsuario = new Label();
            tabPermisos = new TabPage();
            arbolPermisos = new Vista.Comun.ArbolPermisos();
            lblConteoPermisos = new Label();
            txtFiltrarPermisos = new TextBox();
            lblPermisos = new Label();
            clbGrupos = new CheckedListBox();
            lblGrupos = new Label();
            lblEstado = new Label();
            btnGuardar = new Button();
            btnCancelar = new Button();
            errorProvider = new ErrorProvider(components);
            tabFicha.SuspendLayout();
            tabDatos.SuspendLayout();
            tabPermisos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
            SuspendLayout();
            //
            // tabFicha
            //
            tabFicha.Controls.Add(tabDatos);
            tabFicha.Controls.Add(tabPermisos);
            tabFicha.Location = new Point(12, 12);
            tabFicha.Name = "tabFicha";
            tabFicha.SelectedIndex = 0;
            tabFicha.Size = new Size(756, 500);
            tabFicha.TabIndex = 0;
            //
            // tabDatos
            //
            tabDatos.Controls.Add(lblClaveInfo);
            tabDatos.Controls.Add(txtTelefono);
            tabDatos.Controls.Add(lblTelefono);
            tabDatos.Controls.Add(txtDni);
            tabDatos.Controls.Add(lblDni);
            tabDatos.Controls.Add(txtEmail);
            tabDatos.Controls.Add(lblEmail);
            tabDatos.Controls.Add(txtNombre);
            tabDatos.Controls.Add(lblNombre);
            tabDatos.Controls.Add(lblUsuarioInfo);
            tabDatos.Controls.Add(txtUsuario);
            tabDatos.Controls.Add(lblUsuario);
            tabDatos.Location = new Point(4, 29);
            tabDatos.Name = "tabDatos";
            tabDatos.Padding = new Padding(3);
            tabDatos.Size = new Size(748, 467);
            tabDatos.TabIndex = 0;
            tabDatos.Text = "Datos";
            tabDatos.UseVisualStyleBackColor = true;
            //
            // lblClaveInfo
            //
            lblClaveInfo.Location = new Point(20, 270);
            lblClaveInfo.Name = "lblClaveInfo";
            lblClaveInfo.Size = new Size(700, 80);
            lblClaveInfo.TabIndex = 11;
            lblClaveInfo.Tag = "BLANCO";
            //
            // txtTelefono
            //
            txtTelefono.Location = new Point(200, 216);
            txtTelefono.MaxLength = 30;
            txtTelefono.Name = "txtTelefono";
            txtTelefono.Size = new Size(220, 27);
            txtTelefono.TabIndex = 10;
            //
            // lblTelefono
            //
            lblTelefono.AutoSize = true;
            lblTelefono.Location = new Point(20, 220);
            lblTelefono.Name = "lblTelefono";
            lblTelefono.Size = new Size(67, 20);
            lblTelefono.TabIndex = 9;
            lblTelefono.Tag = "BLANCO";
            lblTelefono.Text = "Teléfono";
            //
            // txtDni
            //
            txtDni.Location = new Point(200, 172);
            txtDni.MaxLength = 10;
            txtDni.Name = "txtDni";
            txtDni.Size = new Size(160, 27);
            txtDni.TabIndex = 8;
            //
            // lblDni
            //
            lblDni.AutoSize = true;
            lblDni.Location = new Point(20, 176);
            lblDni.Name = "lblDni";
            lblDni.Size = new Size(45, 20);
            lblDni.TabIndex = 7;
            lblDni.Tag = "BLANCO";
            lblDni.Text = "DNI *";
            //
            // txtEmail
            //
            txtEmail.Location = new Point(200, 128);
            txtEmail.MaxLength = 60;
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(400, 27);
            txtEmail.TabIndex = 6;
            //
            // lblEmail
            //
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(20, 132);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(58, 20);
            lblEmail.TabIndex = 5;
            lblEmail.Tag = "BLANCO";
            lblEmail.Text = "Email *";
            //
            // txtNombre
            //
            txtNombre.Location = new Point(200, 84);
            txtNombre.MaxLength = 60;
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(400, 27);
            txtNombre.TabIndex = 4;
            //
            // lblNombre
            //
            lblNombre.AutoSize = true;
            lblNombre.Location = new Point(20, 88);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(140, 20);
            lblNombre.TabIndex = 3;
            lblNombre.Tag = "BLANCO";
            lblNombre.Text = "Nombre completo *";
            //
            // lblUsuarioInfo
            //
            lblUsuarioInfo.AutoSize = true;
            lblUsuarioInfo.Location = new Point(200, 50);
            lblUsuarioInfo.Name = "lblUsuarioInfo";
            lblUsuarioInfo.Size = new Size(0, 20);
            lblUsuarioInfo.TabIndex = 2;
            lblUsuarioInfo.Tag = "BLANCO";
            //
            // txtUsuario
            //
            txtUsuario.Location = new Point(200, 20);
            txtUsuario.MaxLength = 60;
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(260, 27);
            txtUsuario.TabIndex = 1;
            //
            // lblUsuario
            //
            lblUsuario.AutoSize = true;
            lblUsuario.Location = new Point(20, 24);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(71, 20);
            lblUsuario.TabIndex = 0;
            lblUsuario.Tag = "BLANCO";
            lblUsuario.Text = "Usuario *";
            //
            // tabPermisos
            //
            tabPermisos.Controls.Add(arbolPermisos);
            tabPermisos.Controls.Add(lblConteoPermisos);
            tabPermisos.Controls.Add(txtFiltrarPermisos);
            tabPermisos.Controls.Add(lblPermisos);
            tabPermisos.Controls.Add(clbGrupos);
            tabPermisos.Controls.Add(lblGrupos);
            tabPermisos.Location = new Point(4, 29);
            tabPermisos.Name = "tabPermisos";
            tabPermisos.Padding = new Padding(3);
            tabPermisos.Size = new Size(748, 467);
            tabPermisos.TabIndex = 1;
            tabPermisos.Text = "Grupos y permisos";
            tabPermisos.UseVisualStyleBackColor = true;
            //
            // arbolPermisos
            //
            arbolPermisos.CheckBoxes = true;
            arbolPermisos.HideSelection = false;
            arbolPermisos.Location = new Point(246, 74);
            arbolPermisos.Name = "arbolPermisos";
            arbolPermisos.Size = new Size(490, 384);
            arbolPermisos.TabIndex = 5;
            //
            // lblConteoPermisos
            //
            lblConteoPermisos.AutoSize = true;
            lblConteoPermisos.Location = new Point(476, 42);
            lblConteoPermisos.Name = "lblConteoPermisos";
            lblConteoPermisos.Size = new Size(0, 20);
            lblConteoPermisos.TabIndex = 4;
            lblConteoPermisos.Tag = "BLANCO";
            //
            // txtFiltrarPermisos
            //
            txtFiltrarPermisos.Location = new Point(246, 38);
            txtFiltrarPermisos.Name = "txtFiltrarPermisos";
            txtFiltrarPermisos.PlaceholderText = "Filtrar permisos...";
            txtFiltrarPermisos.Size = new Size(220, 27);
            txtFiltrarPermisos.TabIndex = 3;
            //
            // lblPermisos
            //
            lblPermisos.AutoSize = true;
            lblPermisos.Location = new Point(246, 12);
            lblPermisos.Name = "lblPermisos";
            lblPermisos.Size = new Size(338, 20);
            lblPermisos.TabIndex = 2;
            lblPermisos.Tag = "BLANCO";
            lblPermisos.Text = "Permisos directos (además de los de sus grupos)";
            //
            // clbGrupos
            //
            clbGrupos.CheckOnClick = true;
            clbGrupos.FormattingEnabled = true;
            clbGrupos.Location = new Point(12, 38);
            clbGrupos.Name = "clbGrupos";
            clbGrupos.Size = new Size(220, 418);
            clbGrupos.TabIndex = 1;
            //
            // lblGrupos
            //
            lblGrupos.AutoSize = true;
            lblGrupos.Location = new Point(12, 12);
            lblGrupos.Name = "lblGrupos";
            lblGrupos.Size = new Size(68, 20);
            lblGrupos.TabIndex = 0;
            lblGrupos.Tag = "BLANCO";
            lblGrupos.Text = "Grupos *";
            //
            // lblEstado
            //
            lblEstado.AutoEllipsis = true;
            lblEstado.Location = new Point(12, 520);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(480, 44);
            lblEstado.TabIndex = 3;
            lblEstado.Text = "Los campos con * son obligatorios.";
            lblEstado.TextAlign = ContentAlignment.MiddleLeft;
            //
            // btnGuardar
            //
            btnGuardar.Location = new Point(500, 520);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(128, 44);
            btnGuardar.TabIndex = 1;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            //
            // btnCancelar
            //
            btnCancelar.DialogResult = DialogResult.Cancel;
            btnCancelar.Location = new Point(640, 520);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(128, 44);
            btnCancelar.TabIndex = 2;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            //
            // errorProvider
            //
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            errorProvider.ContainerControl = this;
            //
            // FrmEditarUsuario
            //
            AcceptButton = btnGuardar;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancelar;
            ClientSize = new Size(780, 576);
            Controls.Add(btnCancelar);
            Controls.Add(btnGuardar);
            Controls.Add(lblEstado);
            Controls.Add(tabFicha);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmEditarUsuario";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Usuario";
            Load += FrmEditarUsuario_Load;
            tabFicha.ResumeLayout(false);
            tabDatos.ResumeLayout(false);
            tabDatos.PerformLayout();
            tabPermisos.ResumeLayout(false);
            tabPermisos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabFicha;
        private TabPage tabDatos;
        private Label lblUsuario;
        private TextBox txtUsuario;
        private Label lblUsuarioInfo;
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblDni;
        private TextBox txtDni;
        private Label lblTelefono;
        private TextBox txtTelefono;
        private Label lblClaveInfo;
        private TabPage tabPermisos;
        private Label lblGrupos;
        private CheckedListBox clbGrupos;
        private Label lblPermisos;
        private TextBox txtFiltrarPermisos;
        private Label lblConteoPermisos;
        private Vista.Comun.ArbolPermisos arbolPermisos;
        private Label lblEstado;
        private Button btnGuardar;
        private Button btnCancelar;
        private ErrorProvider errorProvider;
    }
}
