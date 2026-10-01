namespace Vista.Seguridad
{
    partial class FrmCambiarClave
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmCambiarClave));
            txtClaveActual = new TextBox();
            txtConfirmar = new TextBox();
            txtClaveNueva = new TextBox();
            btnCancelar = new Button();
            btnAceptar = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            panelSuperior = new Panel();
            button1 = new Button();
            pbLogo = new PictureBox();
            panelSuperior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbLogo).BeginInit();
            SuspendLayout();
            // 
            // txtClaveActual
            // 
            txtClaveActual.Location = new Point(195, 84);
            txtClaveActual.Margin = new Padding(5, 4, 5, 4);
            txtClaveActual.Name = "txtClaveActual";
            txtClaveActual.Size = new Size(324, 27);
            txtClaveActual.TabIndex = 2;
            // 
            // txtConfirmar
            // 
            txtConfirmar.Location = new Point(195, 213);
            txtConfirmar.Margin = new Padding(5, 4, 5, 4);
            txtConfirmar.Name = "txtConfirmar";
            txtConfirmar.Size = new Size(324, 27);
            txtConfirmar.TabIndex = 3;
            // 
            // txtClaveNueva
            // 
            txtClaveNueva.Location = new Point(195, 147);
            txtClaveNueva.Margin = new Padding(5, 4, 5, 4);
            txtClaveNueva.Name = "txtClaveNueva";
            txtClaveNueva.Size = new Size(324, 27);
            txtClaveNueva.TabIndex = 4;
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(490, 261);
            btnCancelar.Margin = new Padding(3, 4, 3, 4);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(101, 53);
            btnCancelar.TabIndex = 10;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // btnAceptar
            // 
            btnAceptar.Location = new Point(365, 261);
            btnAceptar.Margin = new Padding(3, 4, 3, 4);
            btnAceptar.Name = "btnAceptar";
            btnAceptar.Size = new Size(101, 53);
            btnAceptar.TabIndex = 9;
            btnAceptar.Text = "Aceptar";
            btnAceptar.UseVisualStyleBackColor = true;
            btnAceptar.Click += btnAceptar_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(29, 83);
            label1.Margin = new Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(120, 28);
            label1.TabIndex = 12;
            label1.Text = "Clave actual:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(29, 146);
            label2.Margin = new Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new Size(120, 28);
            label2.TabIndex = 13;
            label2.Text = "Clave nueva:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(14, 212);
            label3.Margin = new Padding(5, 0, 5, 0);
            label3.Name = "label3";
            label3.Size = new Size(152, 28);
            label3.TabIndex = 14;
            label3.Text = "Confirmar clave:";
            // 
            // panelSuperior
            // 
            panelSuperior.Controls.Add(button1);
            panelSuperior.Controls.Add(pbLogo);
            panelSuperior.Dock = DockStyle.Top;
            panelSuperior.Location = new Point(0, 0);
            panelSuperior.Name = "panelSuperior";
            panelSuperior.Size = new Size(591, 56);
            panelSuperior.TabIndex = 18;
            // 
            // button1
            // 
            button1.Location = new Point(469, 12);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 18;
            button1.Text = "Cancelar";
            button1.UseVisualStyleBackColor = true;
            // 
            // pbLogo
            // 
            pbLogo.Image = (Image)resources.GetObject("pbLogo.Image");
            pbLogo.Location = new Point(0, -1);
            pbLogo.Name = "pbLogo";
            pbLogo.Size = new Size(125, 62);
            pbLogo.SizeMode = PictureBoxSizeMode.Zoom;
            pbLogo.TabIndex = 10;
            pbLogo.TabStop = false;
            // 
            // FrmCambiarClave
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(591, 319);
            ControlBox = false;
            Controls.Add(panelSuperior);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnCancelar);
            Controls.Add(btnAceptar);
            Controls.Add(txtClaveNueva);
            Controls.Add(txtConfirmar);
            Controls.Add(txtClaveActual);
            Name = "FrmCambiarClave";
            panelSuperior.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox txtClaveActual;
        private TextBox txtConfirmar;
        private TextBox txtClaveNueva;
        private Button btnCancelar;
        private Button btnAceptar;
        private Label label1;
        private Label label2;
        private Label label3;
        private Panel panelSuperior;
        private Button button1;
        private PictureBox pbLogo;
    }
}