namespace Vista
{
    partial class FrmMetodosPago
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMetodosPago));
            txtMetodoNuevo = new TextBox();
            btnGuardarMetodo = new Button();
            gbNuevoMetodo = new GroupBox();
            gbMetodoExistente = new GroupBox();
            checkActivo = new CheckBox();
            cbMetodos = new ComboBox();
            btnActivar = new Button();
            btnDarDeBaja = new Button();
            panelSuperior = new Panel();
            pbLogo = new PictureBox();
            btnCancelar = new Button();
            gbNuevoMetodo.SuspendLayout();
            gbMetodoExistente.SuspendLayout();
            panelSuperior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbLogo).BeginInit();
            SuspendLayout();
            // 
            // txtMetodoNuevo
            // 
            txtMetodoNuevo.Location = new Point(20, 56);
            txtMetodoNuevo.Name = "txtMetodoNuevo";
            txtMetodoNuevo.Size = new Size(265, 27);
            txtMetodoNuevo.TabIndex = 0;
            // 
            // btnGuardarMetodo
            // 
            btnGuardarMetodo.Location = new Point(76, 138);
            btnGuardarMetodo.Name = "btnGuardarMetodo";
            btnGuardarMetodo.Size = new Size(151, 45);
            btnGuardarMetodo.TabIndex = 1;
            btnGuardarMetodo.Text = "Guardar Método";
            btnGuardarMetodo.UseVisualStyleBackColor = true;
            btnGuardarMetodo.Click += btnGuardarMetodo_Click;
            // 
            // gbNuevoMetodo
            // 
            gbNuevoMetodo.Controls.Add(txtMetodoNuevo);
            gbNuevoMetodo.Controls.Add(btnGuardarMetodo);
            gbNuevoMetodo.Location = new Point(391, 77);
            gbNuevoMetodo.Name = "gbNuevoMetodo";
            gbNuevoMetodo.Size = new Size(305, 224);
            gbNuevoMetodo.TabIndex = 2;
            gbNuevoMetodo.TabStop = false;
            gbNuevoMetodo.Text = "Nuevo método de pago";
            // 
            // gbMetodoExistente
            // 
            gbMetodoExistente.Controls.Add(checkActivo);
            gbMetodoExistente.Controls.Add(cbMetodos);
            gbMetodoExistente.Controls.Add(btnActivar);
            gbMetodoExistente.Controls.Add(btnDarDeBaja);
            gbMetodoExistente.Location = new Point(12, 77);
            gbMetodoExistente.Name = "gbMetodoExistente";
            gbMetodoExistente.Size = new Size(346, 224);
            gbMetodoExistente.TabIndex = 3;
            gbMetodoExistente.TabStop = false;
            gbMetodoExistente.Text = "Gestionar métodos existentes";
            // 
            // checkActivo
            // 
            checkActivo.AutoSize = true;
            checkActivo.Location = new Point(22, 90);
            checkActivo.Name = "checkActivo";
            checkActivo.Size = new Size(145, 24);
            checkActivo.TabIndex = 2;
            checkActivo.Text = "✓ Método Activo";
            checkActivo.UseVisualStyleBackColor = true;
            // 
            // cbMetodos
            // 
            cbMetodos.FormattingEnabled = true;
            cbMetodos.Location = new Point(22, 56);
            cbMetodos.Name = "cbMetodos";
            cbMetodos.Size = new Size(286, 28);
            cbMetodos.TabIndex = 4;
            cbMetodos.SelectedIndexChanged += cbMetodos_SelectedIndexChanged;
            // 
            // btnActivar
            // 
            btnActivar.Location = new Point(6, 138);
            btnActivar.Name = "btnActivar";
            btnActivar.Size = new Size(145, 45);
            btnActivar.TabIndex = 2;
            btnActivar.Text = "Activar";
            btnActivar.UseVisualStyleBackColor = true;
            btnActivar.Click += btnActivar_Click;
            // 
            // btnDarDeBaja
            // 
            btnDarDeBaja.Location = new Point(165, 138);
            btnDarDeBaja.Name = "btnDarDeBaja";
            btnDarDeBaja.Size = new Size(158, 45);
            btnDarDeBaja.TabIndex = 1;
            btnDarDeBaja.Text = "Dar de baja";
            btnDarDeBaja.UseVisualStyleBackColor = true;
            btnDarDeBaja.Click += btnDarDeBaja_Click;
            // 
            // panelSuperior
            // 
            panelSuperior.Controls.Add(pbLogo);
            panelSuperior.Controls.Add(btnCancelar);
            panelSuperior.Dock = DockStyle.Top;
            panelSuperior.Location = new Point(0, 0);
            panelSuperior.Name = "panelSuperior";
            panelSuperior.Size = new Size(717, 61);
            panelSuperior.TabIndex = 24;
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
            // btnCancelar
            // 
            btnCancelar.Location = new Point(611, 12);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(94, 29);
            btnCancelar.TabIndex = 8;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // FrmMetodosPago
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(717, 385);
            Controls.Add(panelSuperior);
            Controls.Add(gbMetodoExistente);
            Controls.Add(gbNuevoMetodo);
            Name = "FrmMetodosPago";
            gbNuevoMetodo.ResumeLayout(false);
            gbNuevoMetodo.PerformLayout();
            gbMetodoExistente.ResumeLayout(false);
            gbMetodoExistente.PerformLayout();
            panelSuperior.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbLogo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TextBox txtMetodoNuevo;
        private Button btnGuardarMetodo;
        private GroupBox gbNuevoMetodo;
        private GroupBox gbMetodoExistente;
        private ComboBox cbMetodos;
        private Button btnActivar;
        private Button btnDarDeBaja;
        private CheckBox checkActivo;
        private Panel panelSuperior;
        private PictureBox pbLogo;
        private Button btnCancelar;
    }
}