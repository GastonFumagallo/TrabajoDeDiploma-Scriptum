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
            lblInicio = new Label();
            SuspendLayout();
            // 
            // lblInicio
            // 
            lblInicio.AutoSize = true;
            lblInicio.BackColor = SystemColors.Control;
            lblInicio.Font = new Font("Segoe UI", 32F, FontStyle.Bold);
            lblInicio.Location = new Point(8, 9);
            lblInicio.MaximumSize = new Size(600, 0);
            lblInicio.Name = "lblInicio";
            lblInicio.Size = new Size(313, 72);
            lblInicio.TabIndex = 0;
            lblInicio.Tag = "Bienvenido";
            lblInicio.Text = "Bienvenido";
            lblInicio.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FrmHome
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lblInicio);
            Name = "FrmHome";
            Text = "INICIO";
            Load += FrmHome_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblInicio;
    }
}