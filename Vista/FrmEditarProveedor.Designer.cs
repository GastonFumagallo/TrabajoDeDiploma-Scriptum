namespace Vista
{
    partial class FrmEditarProveedor
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
            gbDatos = new GroupBox();
            lblCuit = new Label();
            txtCuit = new TextBox();
            lblCuitInfo = new Label();
            lblRazonSocial = new Label();
            txtRazonSocial = new TextBox();
            lblCondicion = new Label();
            cbCondicion = new ComboBox();
            lblContacto = new Label();
            txtContacto = new TextBox();
            lblTelefono = new Label();
            txtTelefono = new TextBox();
            lblEmail = new Label();
            txtEmail = new TextBox();
            lblDireccion = new Label();
            txtDireccion = new TextBox();
            lblObservaciones = new Label();
            txtObservaciones = new TextBox();
            lblEstado = new Label();
            btnGuardar = new Button();
            btnCancelar = new Button();
            errorProvider = new ErrorProvider(components);
            gbDatos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
            SuspendLayout();
            //
            // gbDatos
            //
            gbDatos.Controls.Add(lblCuit);
            gbDatos.Controls.Add(txtCuit);
            gbDatos.Controls.Add(lblCuitInfo);
            gbDatos.Controls.Add(lblRazonSocial);
            gbDatos.Controls.Add(txtRazonSocial);
            gbDatos.Controls.Add(lblCondicion);
            gbDatos.Controls.Add(cbCondicion);
            gbDatos.Controls.Add(lblContacto);
            gbDatos.Controls.Add(txtContacto);
            gbDatos.Controls.Add(lblTelefono);
            gbDatos.Controls.Add(txtTelefono);
            gbDatos.Controls.Add(lblEmail);
            gbDatos.Controls.Add(txtEmail);
            gbDatos.Controls.Add(lblDireccion);
            gbDatos.Controls.Add(txtDireccion);
            gbDatos.Controls.Add(lblObservaciones);
            gbDatos.Controls.Add(txtObservaciones);
            gbDatos.Location = new Point(12, 12);
            gbDatos.Name = "gbDatos";
            gbDatos.Size = new Size(596, 436);
            gbDatos.TabIndex = 0;
            gbDatos.TabStop = false;
            gbDatos.Text = "Datos del proveedor";
            //
            // lblCuit
            //
            lblCuit.AutoSize = true;
            lblCuit.Location = new Point(16, 34);
            lblCuit.Name = "lblCuit";
            lblCuit.Size = new Size(50, 20);
            lblCuit.TabIndex = 0;
            lblCuit.Text = "CUIT *";
            //
            // txtCuit
            //
            txtCuit.Location = new Point(170, 31);
            txtCuit.MaxLength = 13;
            txtCuit.Name = "txtCuit";
            txtCuit.Size = new Size(180, 27);
            txtCuit.TabIndex = 0;
            //
            // lblCuitInfo
            //
            lblCuitInfo.AutoEllipsis = true;
            lblCuitInfo.Font = new Font("Segoe UI", 8F);
            lblCuitInfo.Location = new Point(356, 34);
            lblCuitInfo.Name = "lblCuitInfo";
            lblCuitInfo.Size = new Size(228, 20);
            lblCuitInfo.TabIndex = 2;
            lblCuitInfo.Text = "11 dígitos (se aceptan guiones).";
            //
            // lblRazonSocial
            //
            lblRazonSocial.AutoSize = true;
            lblRazonSocial.Location = new Point(16, 72);
            lblRazonSocial.Name = "lblRazonSocial";
            lblRazonSocial.Size = new Size(104, 20);
            lblRazonSocial.TabIndex = 3;
            lblRazonSocial.Text = "Razón social *";
            //
            // txtRazonSocial
            //
            txtRazonSocial.Location = new Point(170, 69);
            txtRazonSocial.MaxLength = 200;
            txtRazonSocial.Name = "txtRazonSocial";
            txtRazonSocial.Size = new Size(410, 27);
            txtRazonSocial.TabIndex = 1;
            //
            // lblCondicion
            //
            lblCondicion.AutoSize = true;
            lblCondicion.Location = new Point(16, 110);
            lblCondicion.Name = "lblCondicion";
            lblCondicion.Size = new Size(118, 20);
            lblCondicion.TabIndex = 5;
            lblCondicion.Text = "Condición fiscal";
            //
            // cbCondicion
            //
            cbCondicion.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCondicion.Location = new Point(170, 107);
            cbCondicion.Name = "cbCondicion";
            cbCondicion.Size = new Size(240, 28);
            cbCondicion.TabIndex = 2;
            //
            // lblContacto
            //
            lblContacto.AutoSize = true;
            lblContacto.Location = new Point(16, 148);
            lblContacto.Name = "lblContacto";
            lblContacto.Size = new Size(81, 20);
            lblContacto.TabIndex = 7;
            lblContacto.Text = "Contacto *";
            //
            // txtContacto
            //
            txtContacto.Location = new Point(170, 145);
            txtContacto.MaxLength = 60;
            txtContacto.Name = "txtContacto";
            txtContacto.Size = new Size(410, 27);
            txtContacto.TabIndex = 3;
            //
            // lblTelefono
            //
            lblTelefono.AutoSize = true;
            lblTelefono.Location = new Point(16, 186);
            lblTelefono.Name = "lblTelefono";
            lblTelefono.Size = new Size(79, 20);
            lblTelefono.TabIndex = 9;
            lblTelefono.Text = "Teléfono *";
            //
            // txtTelefono
            //
            txtTelefono.Location = new Point(170, 183);
            txtTelefono.MaxLength = 30;
            txtTelefono.Name = "txtTelefono";
            txtTelefono.Size = new Size(200, 27);
            txtTelefono.TabIndex = 4;
            //
            // lblEmail
            //
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(16, 224);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(46, 20);
            lblEmail.TabIndex = 11;
            lblEmail.Text = "Email";
            //
            // txtEmail
            //
            txtEmail.Location = new Point(170, 221);
            txtEmail.MaxLength = 150;
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(410, 27);
            txtEmail.TabIndex = 5;
            //
            // lblDireccion
            //
            lblDireccion.AutoSize = true;
            lblDireccion.Location = new Point(16, 262);
            lblDireccion.Name = "lblDireccion";
            lblDireccion.Size = new Size(72, 20);
            lblDireccion.TabIndex = 13;
            lblDireccion.Text = "Dirección";
            //
            // txtDireccion
            //
            txtDireccion.Location = new Point(170, 259);
            txtDireccion.MaxLength = 200;
            txtDireccion.Name = "txtDireccion";
            txtDireccion.Size = new Size(410, 27);
            txtDireccion.TabIndex = 6;
            //
            // lblObservaciones
            //
            lblObservaciones.AutoSize = true;
            lblObservaciones.Location = new Point(16, 300);
            lblObservaciones.Name = "lblObservaciones";
            lblObservaciones.Size = new Size(105, 20);
            lblObservaciones.TabIndex = 15;
            lblObservaciones.Text = "Observaciones";
            //
            // txtObservaciones
            //
            txtObservaciones.AcceptsReturn = true;
            txtObservaciones.Location = new Point(170, 297);
            txtObservaciones.MaxLength = 500;
            txtObservaciones.Multiline = true;
            txtObservaciones.Name = "txtObservaciones";
            txtObservaciones.ScrollBars = ScrollBars.Vertical;
            txtObservaciones.Size = new Size(410, 120);
            txtObservaciones.TabIndex = 7;
            //
            // lblEstado
            //
            lblEstado.AutoEllipsis = true;
            lblEstado.Location = new Point(12, 460);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(326, 44);
            lblEstado.TabIndex = 3;
            lblEstado.Text = "Los campos con * son obligatorios.";
            lblEstado.TextAlign = ContentAlignment.MiddleLeft;
            //
            // btnGuardar
            //
            btnGuardar.Location = new Point(346, 460);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(128, 44);
            btnGuardar.TabIndex = 1;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            //
            // btnCancelar
            //
            btnCancelar.DialogResult = DialogResult.Cancel;
            btnCancelar.Location = new Point(480, 460);
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
            // FrmEditarProveedor
            //
            AcceptButton = btnGuardar;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancelar;
            ClientSize = new Size(620, 516);
            Controls.Add(gbDatos);
            Controls.Add(lblEstado);
            Controls.Add(btnGuardar);
            Controls.Add(btnCancelar);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmEditarProveedor";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Proveedor";
            Load += FrmEditarProveedor_Load;
            gbDatos.ResumeLayout(false);
            gbDatos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox gbDatos;
        private Label lblCuit;
        private TextBox txtCuit;
        private Label lblCuitInfo;
        private Label lblRazonSocial;
        private TextBox txtRazonSocial;
        private Label lblCondicion;
        private ComboBox cbCondicion;
        private Label lblContacto;
        private TextBox txtContacto;
        private Label lblTelefono;
        private TextBox txtTelefono;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblDireccion;
        private TextBox txtDireccion;
        private Label lblObservaciones;
        private TextBox txtObservaciones;
        private Label lblEstado;
        private Button btnGuardar;
        private Button btnCancelar;
        private ErrorProvider errorProvider;
    }
}
