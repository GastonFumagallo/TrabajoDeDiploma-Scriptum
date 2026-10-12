namespace Vista.ControlCaja
{
    partial class FrmCierreCaja
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
            lblEncabezado = new Label();
            lblApertura = new Label();
            dgvTotales = new DataGridView();
            lblCantidadTitulo = new Label();
            lblCantidad = new Label();
            lblTotalVentasTitulo = new Label();
            lblTotalVentas = new Label();
            lblEsperadoTitulo = new Label();
            lblEsperado = new Label();
            lblContado = new Label();
            numContado = new NumericUpDown();
            lblDiferencia = new Label();
            lblObservaciones = new Label();
            txtObservaciones = new TextBox();
            btnActualizar = new Button();
            btnCerrarCaja = new Button();
            btnCancelar = new Button();
            errorProvider = new ErrorProvider(components);
            ((System.ComponentModel.ISupportInitialize)dgvTotales).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numContado).BeginInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
            SuspendLayout();
            //
            // lblEncabezado
            //
            lblEncabezado.AutoSize = true;
            lblEncabezado.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblEncabezado.Location = new Point(16, 10);
            lblEncabezado.Name = "lblEncabezado";
            lblEncabezado.Size = new Size(0, 30);
            lblEncabezado.TabIndex = 0;
            //
            // lblApertura
            //
            lblApertura.Location = new Point(16, 42);
            lblApertura.Name = "lblApertura";
            lblApertura.Size = new Size(608, 24);
            lblApertura.TabIndex = 1;
            //
            // dgvTotales
            //
            dgvTotales.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTotales.Location = new Point(16, 72);
            dgvTotales.Name = "dgvTotales";
            dgvTotales.RowHeadersWidth = 51;
            dgvTotales.Size = new Size(608, 160);
            dgvTotales.TabIndex = 2;
            dgvTotales.TabStop = false;
            //
            // lblCantidadTitulo
            //
            lblCantidadTitulo.AutoSize = true;
            lblCantidadTitulo.Location = new Point(16, 244);
            lblCantidadTitulo.Name = "lblCantidadTitulo";
            lblCantidadTitulo.Size = new Size(118, 20);
            lblCantidadTitulo.TabIndex = 3;
            lblCantidadTitulo.Text = "Ventas en la caja";
            //
            // lblCantidad
            //
            lblCantidad.Location = new Point(424, 244);
            lblCantidad.Name = "lblCantidad";
            lblCantidad.Size = new Size(200, 20);
            lblCantidad.TabIndex = 4;
            lblCantidad.TextAlign = ContentAlignment.MiddleRight;
            //
            // lblTotalVentasTitulo
            //
            lblTotalVentasTitulo.AutoSize = true;
            lblTotalVentasTitulo.Location = new Point(16, 270);
            lblTotalVentasTitulo.Name = "lblTotalVentasTitulo";
            lblTotalVentasTitulo.Size = new Size(205, 20);
            lblTotalVentasTitulo.TabIndex = 5;
            lblTotalVentasTitulo.Text = "Total vendido (todos los medios)";
            //
            // lblTotalVentas
            //
            lblTotalVentas.Location = new Point(424, 270);
            lblTotalVentas.Name = "lblTotalVentas";
            lblTotalVentas.Size = new Size(200, 20);
            lblTotalVentas.TabIndex = 6;
            lblTotalVentas.TextAlign = ContentAlignment.MiddleRight;
            //
            // lblEsperadoTitulo
            //
            lblEsperadoTitulo.AutoSize = true;
            lblEsperadoTitulo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblEsperadoTitulo.Location = new Point(16, 296);
            lblEsperadoTitulo.Name = "lblEsperadoTitulo";
            lblEsperadoTitulo.Size = new Size(368, 20);
            lblEsperadoTitulo.TabIndex = 7;
            lblEsperadoTitulo.Text = "Efectivo esperado (monto inicial + ventas en efectivo)";
            //
            // lblEsperado
            //
            lblEsperado.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblEsperado.Location = new Point(424, 296);
            lblEsperado.Name = "lblEsperado";
            lblEsperado.Size = new Size(200, 20);
            lblEsperado.TabIndex = 8;
            lblEsperado.TextAlign = ContentAlignment.MiddleRight;
            //
            // lblContado
            //
            lblContado.AutoSize = true;
            lblContado.Location = new Point(16, 340);
            lblContado.Name = "lblContado";
            lblContado.Size = new Size(187, 20);
            lblContado.TabIndex = 9;
            lblContado.Text = "Efectivo contado en caja *";
            //
            // numContado
            //
            numContado.DecimalPlaces = 2;
            numContado.Font = new Font("Segoe UI", 12F);
            numContado.Location = new Point(404, 332);
            numContado.Maximum = new decimal(new int[] { 99999999, 0, 0, 0 });
            numContado.Name = "numContado";
            numContado.Size = new Size(220, 34);
            numContado.TabIndex = 0;
            numContado.TextAlign = HorizontalAlignment.Right;
            numContado.ThousandsSeparator = true;
            //
            // lblDiferencia
            //
            lblDiferencia.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblDiferencia.Location = new Point(16, 376);
            lblDiferencia.Name = "lblDiferencia";
            lblDiferencia.Size = new Size(608, 28);
            lblDiferencia.TabIndex = 10;
            lblDiferencia.TextAlign = ContentAlignment.MiddleRight;
            //
            // lblObservaciones
            //
            lblObservaciones.AutoSize = true;
            lblObservaciones.Location = new Point(16, 414);
            lblObservaciones.Name = "lblObservaciones";
            lblObservaciones.Size = new Size(330, 20);
            lblObservaciones.TabIndex = 11;
            lblObservaciones.Text = "Observaciones (obligatorias si hay diferencia)";
            //
            // txtObservaciones
            //
            txtObservaciones.Location = new Point(16, 438);
            txtObservaciones.MaxLength = 500;
            txtObservaciones.Multiline = true;
            txtObservaciones.Name = "txtObservaciones";
            txtObservaciones.ScrollBars = ScrollBars.Vertical;
            txtObservaciones.Size = new Size(608, 84);
            txtObservaciones.TabIndex = 1;
            //
            // btnActualizar
            //
            btnActualizar.Location = new Point(16, 538);
            btnActualizar.Name = "btnActualizar";
            btnActualizar.Size = new Size(160, 44);
            btnActualizar.TabIndex = 4;
            btnActualizar.Text = "Actualizar totales";
            btnActualizar.UseVisualStyleBackColor = true;
            //
            // btnCerrarCaja
            //
            btnCerrarCaja.Location = new Point(374, 538);
            btnCerrarCaja.Name = "btnCerrarCaja";
            btnCerrarCaja.Size = new Size(120, 44);
            btnCerrarCaja.TabIndex = 2;
            btnCerrarCaja.Text = "Cerrar caja";
            btnCerrarCaja.UseVisualStyleBackColor = true;
            //
            // btnCancelar
            //
            btnCancelar.DialogResult = DialogResult.Cancel;
            btnCancelar.Location = new Point(504, 538);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(120, 44);
            btnCancelar.TabIndex = 3;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            //
            // errorProvider
            //
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            errorProvider.ContainerControl = this;
            //
            // FrmCierreCaja
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancelar;
            ClientSize = new Size(640, 598);
            Controls.Add(btnCancelar);
            Controls.Add(btnCerrarCaja);
            Controls.Add(btnActualizar);
            Controls.Add(txtObservaciones);
            Controls.Add(lblObservaciones);
            Controls.Add(lblDiferencia);
            Controls.Add(numContado);
            Controls.Add(lblContado);
            Controls.Add(lblEsperado);
            Controls.Add(lblEsperadoTitulo);
            Controls.Add(lblTotalVentas);
            Controls.Add(lblTotalVentasTitulo);
            Controls.Add(lblCantidad);
            Controls.Add(lblCantidadTitulo);
            Controls.Add(dgvTotales);
            Controls.Add(lblApertura);
            Controls.Add(lblEncabezado);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmCierreCaja";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Cerrar caja (arqueo)";
            ((System.ComponentModel.ISupportInitialize)dgvTotales).EndInit();
            ((System.ComponentModel.ISupportInitialize)numContado).EndInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblEncabezado;
        private Label lblApertura;
        private DataGridView dgvTotales;
        private Label lblCantidadTitulo;
        private Label lblCantidad;
        private Label lblTotalVentasTitulo;
        private Label lblTotalVentas;
        private Label lblEsperadoTitulo;
        private Label lblEsperado;
        private Label lblContado;
        private NumericUpDown numContado;
        private Label lblDiferencia;
        private Label lblObservaciones;
        private TextBox txtObservaciones;
        private Button btnActualizar;
        private Button btnCerrarCaja;
        private Button btnCancelar;
        private ErrorProvider errorProvider;
    }
}
