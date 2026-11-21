namespace Vista
{
    partial class FrmHome
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
            panelBienvenida = new Panel();
            lblAyuda = new Label();
            lblFecha = new Label();
            lblInicio = new Label();
            pbLogo = new PictureBox();
            panelBienvenida.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbLogo).BeginInit();
            SuspendLayout();
            // 
            // panelBienvenida
            // 
            panelBienvenida.Anchor = AnchorStyles.None;
            panelBienvenida.BackColor = Color.White;
            panelBienvenida.Controls.Add(lblAyuda);
            panelBienvenida.Controls.Add(lblFecha);
            panelBienvenida.Controls.Add(lblInicio);
            panelBienvenida.Controls.Add(pbLogo);
            panelBienvenida.Location = new Point(20, 45);
            panelBienvenida.Name = "panelBienvenida";
            panelBienvenida.Size = new Size(760, 300);
            panelBienvenida.TabIndex = 1;
            // 
            // pbLogo
            // 
            pbLogo.Location = new Point(36, 50);
            pbLogo.Name = "pbLogo";
            pbLogo.Size = new Size(240, 200);
            pbLogo.SizeMode = PictureBoxSizeMode.Zoom;
            pbLogo.TabIndex = 0;
            pbLogo.TabStop = false;
            // 
            // lblInicio
            // 
            lblInicio.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblInicio.ForeColor = Color.FromArgb(15, 23, 42);
            lblInicio.Location = new Point(304, 70);
            lblInicio.Name = "lblInicio";
            lblInicio.Size = new Size(430, 100);
            lblInicio.TabIndex = 0;
            lblInicio.Tag = "Bienvenido";
            lblInicio.Text = "Bienvenido";
            lblInicio.TextAlign = ContentAlignment.BottomLeft;
            // 
            // lblFecha
            // 
            lblFecha.Font = new Font("Segoe UI Semibold", 11F);
            lblFecha.ForeColor = Color.FromArgb(180, 83, 9);
            lblFecha.Location = new Point(306, 176);
            lblFecha.Name = "lblFecha";
            lblFecha.Size = new Size(428, 26);
            lblFecha.TabIndex = 2;
            lblFecha.Text = "Hoy";
            // 
            // lblAyuda
            // 
            lblAyuda.Font = new Font("Segoe UI", 9.75F);
            lblAyuda.ForeColor = Color.FromArgb(71, 85, 105);
            lblAyuda.Location = new Point(306, 206);
            lblAyuda.Name = "lblAyuda";
            lblAyuda.Size = new Size(428, 48);
            lblAyuda.TabIndex = 3;
            lblAyuda.Text = "Elegí una sección en el menú de la izquierda para empezar a trabajar.";
            // 
            // FrmHome
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panelBienvenida);
            Name = "FrmHome";
            Text = "INICIO";
            panelBienvenida.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbLogo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label lblInicio;
        private Panel panelBienvenida;
        private PictureBox pbLogo;
        private Label lblFecha;
        private Label lblAyuda;
    }
}
