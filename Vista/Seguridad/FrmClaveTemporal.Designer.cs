namespace Vista.Seguridad
{
    partial class FrmClaveTemporal
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
            lblMensaje = new Label();
            txtClave = new TextBox();
            btnCopiar = new Button();
            btnCerrar = new Button();
            SuspendLayout();
            //
            // lblMensaje
            //
            lblMensaje.Location = new Point(16, 14);
            lblMensaje.Name = "lblMensaje";
            lblMensaje.Size = new Size(468, 92);
            lblMensaje.TabIndex = 0;
            //
            // txtClave
            //
            txtClave.Font = new Font("Consolas", 16F, FontStyle.Bold);
            txtClave.Location = new Point(16, 112);
            txtClave.Name = "txtClave";
            txtClave.ReadOnly = true;
            txtClave.Size = new Size(330, 39);
            txtClave.TabIndex = 1;
            txtClave.TextAlign = HorizontalAlignment.Center;
            //
            // btnCopiar
            //
            btnCopiar.Location = new Point(356, 112);
            btnCopiar.Name = "btnCopiar";
            btnCopiar.Size = new Size(128, 39);
            btnCopiar.TabIndex = 2;
            btnCopiar.Text = "Copiar";
            btnCopiar.UseVisualStyleBackColor = true;
            //
            // btnCerrar
            //
            btnCerrar.DialogResult = DialogResult.OK;
            btnCerrar.Location = new Point(356, 168);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(128, 44);
            btnCerrar.TabIndex = 3;
            btnCerrar.Text = "Ya la anoté";
            btnCerrar.UseVisualStyleBackColor = true;
            //
            // FrmClaveTemporal
            //
            AcceptButton = btnCerrar;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCerrar;
            ClientSize = new Size(500, 226);
            Controls.Add(btnCerrar);
            Controls.Add(btnCopiar);
            Controls.Add(txtClave);
            Controls.Add(lblMensaje);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmClaveTemporal";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Clave temporal";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblMensaje;
        private TextBox txtClave;
        private Button btnCopiar;
        private Button btnCerrar;
    }
}
