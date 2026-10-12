namespace Vista.ControlCaja
{
    partial class FrmAperturaCaja
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
            lblInfo = new Label();
            lblMonto = new Label();
            numMonto = new NumericUpDown();
            lblAyuda = new Label();
            btnAbrir = new Button();
            btnCancelar = new Button();
            errorProvider = new ErrorProvider(components);
            ((System.ComponentModel.ISupportInitialize)numMonto).BeginInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
            SuspendLayout();
            //
            // lblInfo
            //
            lblInfo.Location = new Point(16, 14);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(428, 48);
            lblInfo.TabIndex = 0;
            //
            // lblMonto
            //
            lblMonto.AutoSize = true;
            lblMonto.Location = new Point(16, 74);
            lblMonto.Name = "lblMonto";
            lblMonto.Size = new Size(201, 20);
            lblMonto.TabIndex = 1;
            lblMonto.Text = "Monto inicial en efectivo *";
            //
            // numMonto
            //
            numMonto.DecimalPlaces = 2;
            numMonto.Font = new Font("Segoe UI", 12F);
            numMonto.Location = new Point(16, 98);
            numMonto.Maximum = new decimal(new int[] { 99999999, 0, 0, 0 });
            numMonto.Name = "numMonto";
            numMonto.Size = new Size(220, 34);
            numMonto.TabIndex = 2;
            numMonto.TextAlign = HorizontalAlignment.Right;
            numMonto.ThousandsSeparator = true;
            //
            // lblAyuda
            //
            lblAyuda.Location = new Point(16, 140);
            lblAyuda.Name = "lblAyuda";
            lblAyuda.Size = new Size(428, 44);
            lblAyuda.TabIndex = 3;
            lblAyuda.Text = "Contá el efectivo del cajón (fondo para dar cambio). Puede ser 0. Al cerrar, se compara contra lo vendido en efectivo.";
            //
            // btnAbrir
            //
            btnAbrir.Location = new Point(194, 198);
            btnAbrir.Name = "btnAbrir";
            btnAbrir.Size = new Size(120, 44);
            btnAbrir.TabIndex = 4;
            btnAbrir.Text = "Abrir caja";
            btnAbrir.UseVisualStyleBackColor = true;
            //
            // btnCancelar
            //
            btnCancelar.DialogResult = DialogResult.Cancel;
            btnCancelar.Location = new Point(324, 198);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(120, 44);
            btnCancelar.TabIndex = 5;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            //
            // errorProvider
            //
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            errorProvider.ContainerControl = this;
            //
            // FrmAperturaCaja
            //
            AcceptButton = btnAbrir;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancelar;
            ClientSize = new Size(460, 256);
            Controls.Add(btnCancelar);
            Controls.Add(btnAbrir);
            Controls.Add(lblAyuda);
            Controls.Add(numMonto);
            Controls.Add(lblMonto);
            Controls.Add(lblInfo);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmAperturaCaja";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Abrir caja";
            ((System.ComponentModel.ISupportInitialize)numMonto).EndInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblInfo;
        private Label lblMonto;
        private NumericUpDown numMonto;
        private Label lblAyuda;
        private Button btnAbrir;
        private Button btnCancelar;
        private ErrorProvider errorProvider;
    }
}
