namespace Vista
{
    partial class FrmEditarCliente
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
            lblDocumento = new Label();
            cbTipoDocumento = new ComboBox();
            txtDocumento = new TextBox();
            lblDocumentoInfo = new Label();
            lblNombre = new Label();
            txtNombre = new TextBox();
            lblTelefono = new Label();
            txtTelefono = new TextBox();
            lblEmail = new Label();
            txtEmail = new TextBox();
            lblDireccion = new Label();
            txtDireccion = new TextBox();
            lblLocalidad = new Label();
            cbLocalidad = new ComboBox();
            lblLimite = new Label();
            numLimite = new NumericUpDown();
            lblLimiteInfo = new Label();
            lblEstado = new Label();
            btnGuardar = new Button();
            btnCancelar = new Button();
            errorProvider = new ErrorProvider(components);
            gbDatos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numLimite).BeginInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
            SuspendLayout();
            //
            // gbDatos
            //
            gbDatos.Controls.Add(lblDocumento);
            gbDatos.Controls.Add(cbTipoDocumento);
            gbDatos.Controls.Add(txtDocumento);
            gbDatos.Controls.Add(lblDocumentoInfo);
            gbDatos.Controls.Add(lblNombre);
            gbDatos.Controls.Add(txtNombre);
            gbDatos.Controls.Add(lblTelefono);
            gbDatos.Controls.Add(txtTelefono);
            gbDatos.Controls.Add(lblEmail);
            gbDatos.Controls.Add(txtEmail);
            gbDatos.Controls.Add(lblDireccion);
            gbDatos.Controls.Add(txtDireccion);
            gbDatos.Controls.Add(lblLocalidad);
            gbDatos.Controls.Add(cbLocalidad);
            gbDatos.Controls.Add(lblLimite);
            gbDatos.Controls.Add(numLimite);
            gbDatos.Controls.Add(lblLimiteInfo);
            gbDatos.Location = new Point(12, 12);
            gbDatos.Name = "gbDatos";
            gbDatos.Size = new Size(596, 308);
            gbDatos.TabIndex = 0;
            gbDatos.TabStop = false;
            gbDatos.Text = "Datos del cliente";
            //
            // lblDocumento
            //
            lblDocumento.AutoSize = true;
            lblDocumento.Location = new Point(16, 34);
            lblDocumento.Name = "lblDocumento";
            lblDocumento.Size = new Size(95, 20);
            lblDocumento.TabIndex = 0;
            lblDocumento.Text = "Documento *";
            //
            // cbTipoDocumento
            //
            cbTipoDocumento.DropDownStyle = ComboBoxStyle.DropDownList;
            cbTipoDocumento.Location = new Point(170, 31);
            cbTipoDocumento.Name = "cbTipoDocumento";
            cbTipoDocumento.Size = new Size(80, 28);
            cbTipoDocumento.TabIndex = 0;
            //
            // txtDocumento
            //
            txtDocumento.Location = new Point(256, 31);
            txtDocumento.MaxLength = 13;
            txtDocumento.Name = "txtDocumento";
            txtDocumento.Size = new Size(160, 27);
            txtDocumento.TabIndex = 1;
            //
            // lblDocumentoInfo
            //
            lblDocumentoInfo.AutoEllipsis = true;
            lblDocumentoInfo.Font = new Font("Segoe UI", 8F);
            lblDocumentoInfo.Location = new Point(422, 34);
            lblDocumentoInfo.Name = "lblDocumentoInfo";
            lblDocumentoInfo.Size = new Size(166, 20);
            lblDocumentoInfo.TabIndex = 3;
            lblDocumentoInfo.Text = "7 u 8 dígitos.";
            //
            // lblNombre
            //
            lblNombre.AutoSize = true;
            lblNombre.Location = new Point(16, 72);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(145, 20);
            lblNombre.TabIndex = 4;
            lblNombre.Text = "Nombre y apellido *";
            //
            // txtNombre
            //
            txtNombre.Location = new Point(170, 69);
            txtNombre.MaxLength = 60;
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(410, 27);
            txtNombre.TabIndex = 2;
            //
            // lblTelefono
            //
            lblTelefono.AutoSize = true;
            lblTelefono.Location = new Point(16, 110);
            lblTelefono.Name = "lblTelefono";
            lblTelefono.Size = new Size(67, 20);
            lblTelefono.TabIndex = 6;
            lblTelefono.Text = "Teléfono";
            //
            // txtTelefono
            //
            txtTelefono.Location = new Point(170, 107);
            txtTelefono.MaxLength = 30;
            txtTelefono.Name = "txtTelefono";
            txtTelefono.Size = new Size(200, 27);
            txtTelefono.TabIndex = 3;
            //
            // lblEmail
            //
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(16, 148);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(46, 20);
            lblEmail.TabIndex = 8;
            lblEmail.Text = "Email";
            //
            // txtEmail
            //
            txtEmail.Location = new Point(170, 145);
            txtEmail.MaxLength = 150;
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(410, 27);
            txtEmail.TabIndex = 4;
            //
            // lblDireccion
            //
            lblDireccion.AutoSize = true;
            lblDireccion.Location = new Point(16, 186);
            lblDireccion.Name = "lblDireccion";
            lblDireccion.Size = new Size(72, 20);
            lblDireccion.TabIndex = 10;
            lblDireccion.Text = "Dirección";
            //
            // txtDireccion
            //
            txtDireccion.Location = new Point(170, 183);
            txtDireccion.MaxLength = 200;
            txtDireccion.Name = "txtDireccion";
            txtDireccion.Size = new Size(410, 27);
            txtDireccion.TabIndex = 5;
            //
            // lblLocalidad
            //
            lblLocalidad.AutoSize = true;
            lblLocalidad.Location = new Point(16, 224);
            lblLocalidad.Name = "lblLocalidad";
            lblLocalidad.Size = new Size(74, 20);
            lblLocalidad.TabIndex = 12;
            lblLocalidad.Text = "Localidad";
            //
            // cbLocalidad
            //
            cbLocalidad.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cbLocalidad.AutoCompleteSource = AutoCompleteSource.ListItems;
            cbLocalidad.Location = new Point(170, 221);
            cbLocalidad.MaxLength = 100;
            cbLocalidad.Name = "cbLocalidad";
            cbLocalidad.Size = new Size(240, 28);
            cbLocalidad.TabIndex = 6;
            //
            // lblLimite
            //
            lblLimite.AutoSize = true;
            lblLimite.Location = new Point(16, 262);
            lblLimite.Name = "lblLimite";
            lblLimite.Size = new Size(122, 20);
            lblLimite.TabIndex = 14;
            lblLimite.Text = "Límite de crédito";
            //
            // numLimite
            //
            numLimite.DecimalPlaces = 2;
            numLimite.Location = new Point(170, 259);
            numLimite.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            numLimite.Name = "numLimite";
            numLimite.Size = new Size(140, 27);
            numLimite.TabIndex = 7;
            numLimite.TextAlign = HorizontalAlignment.Right;
            numLimite.ThousandsSeparator = true;
            //
            // lblLimiteInfo
            //
            lblLimiteInfo.AutoEllipsis = true;
            lblLimiteInfo.Font = new Font("Segoe UI", 8F);
            lblLimiteInfo.Location = new Point(316, 262);
            lblLimiteInfo.Name = "lblLimiteInfo";
            lblLimiteInfo.Size = new Size(264, 20);
            lblLimiteInfo.TabIndex = 16;
            lblLimiteInfo.Text = "0 = sin cuenta corriente.";
            //
            // lblEstado
            //
            lblEstado.AutoEllipsis = true;
            lblEstado.Location = new Point(12, 332);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(326, 44);
            lblEstado.TabIndex = 3;
            lblEstado.Text = "Los campos con * son obligatorios.";
            lblEstado.TextAlign = ContentAlignment.MiddleLeft;
            //
            // btnGuardar
            //
            btnGuardar.Location = new Point(346, 332);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(128, 44);
            btnGuardar.TabIndex = 1;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            //
            // btnCancelar
            //
            btnCancelar.DialogResult = DialogResult.Cancel;
            btnCancelar.Location = new Point(480, 332);
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
            // FrmEditarCliente
            //
            AcceptButton = btnGuardar;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancelar;
            ClientSize = new Size(620, 388);
            Controls.Add(gbDatos);
            Controls.Add(lblEstado);
            Controls.Add(btnGuardar);
            Controls.Add(btnCancelar);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmEditarCliente";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Cliente";
            Load += FrmEditarCliente_Load;
            gbDatos.ResumeLayout(false);
            gbDatos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numLimite).EndInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox gbDatos;
        private Label lblDocumento;
        private ComboBox cbTipoDocumento;
        private TextBox txtDocumento;
        private Label lblDocumentoInfo;
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblTelefono;
        private TextBox txtTelefono;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblDireccion;
        private TextBox txtDireccion;
        private Label lblLocalidad;
        private ComboBox cbLocalidad;
        private Label lblLimite;
        private NumericUpDown numLimite;
        private Label lblLimiteInfo;
        private Label lblEstado;
        private Button btnGuardar;
        private Button btnCancelar;
        private ErrorProvider errorProvider;
    }
}
