namespace Vista.Seguridad
{
    partial class FrmUsuario
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmUsuario));
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            txtTelefono = new TextBox();
            label6 = new Label();
            txtDNI = new TextBox();
            label5 = new Label();
            txtEmail = new TextBox();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            cbEstados = new ComboBox();
            txtNombre = new TextBox();
            txtUsuario = new TextBox();
            tabPage2 = new TabPage();
            clbGrupos = new CheckedListBox();
            tabPage3 = new TabPage();
            tvAcciones1 = new TreeView();
            btnGuardar = new Button();
            panelSuperior = new Panel();
            pbLogo = new PictureBox();
            btnCancelar = new Button();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            panelSuperior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbLogo).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Location = new Point(0, 74);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(957, 429);
            tabControl1.TabIndex = 12;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(txtTelefono);
            tabPage1.Controls.Add(label6);
            tabPage1.Controls.Add(txtDNI);
            tabPage1.Controls.Add(label5);
            tabPage1.Controls.Add(txtEmail);
            tabPage1.Controls.Add(label4);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(cbEstados);
            tabPage1.Controls.Add(txtNombre);
            tabPage1.Controls.Add(txtUsuario);
            tabPage1.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tabPage1.Location = new Point(4, 29);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(949, 396);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Mis datos";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // txtTelefono
            // 
            txtTelefono.Location = new Point(372, 267);
            txtTelefono.Margin = new Padding(5, 4, 5, 4);
            txtTelefono.Name = "txtTelefono";
            txtTelefono.Size = new Size(252, 30);
            txtTelefono.TabIndex = 12;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label6.Location = new Point(255, 272);
            label6.Name = "label6";
            label6.Size = new Size(89, 25);
            label6.TabIndex = 11;
            label6.Text = "Telefono";
            // 
            // txtDNI
            // 
            txtDNI.Location = new Point(372, 212);
            txtDNI.Margin = new Padding(5, 4, 5, 4);
            txtDNI.Name = "txtDNI";
            txtDNI.Size = new Size(252, 30);
            txtDNI.TabIndex = 10;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.Location = new Point(255, 217);
            label5.Name = "label5";
            label5.Size = new Size(45, 25);
            label5.TabIndex = 9;
            label5.Text = "DNI";
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(372, 153);
            txtEmail.Margin = new Padding(5, 4, 5, 4);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(252, 30);
            txtEmail.TabIndex = 8;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(255, 337);
            label4.Name = "label4";
            label4.Size = new Size(73, 25);
            label4.TabIndex = 7;
            label4.Text = "Estado";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(255, 158);
            label3.Name = "label3";
            label3.Size = new Size(67, 25);
            label3.TabIndex = 6;
            label3.Text = "E-mail";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(255, 100);
            label2.Name = "label2";
            label2.Size = new Size(81, 25);
            label2.TabIndex = 5;
            label2.Text = "Nombre";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(255, 38);
            label1.Name = "label1";
            label1.Size = new Size(79, 25);
            label1.TabIndex = 4;
            label1.Text = "Usuario";
            // 
            // cbEstados
            // 
            cbEstados.FormattingEnabled = true;
            cbEstados.Location = new Point(372, 331);
            cbEstados.Name = "cbEstados";
            cbEstados.Size = new Size(252, 33);
            cbEstados.TabIndex = 3;
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(372, 95);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(252, 30);
            txtNombre.TabIndex = 1;
            // 
            // txtUsuario
            // 
            txtUsuario.Location = new Point(372, 33);
            txtUsuario.Margin = new Padding(5, 4, 5, 4);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(252, 30);
            txtUsuario.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(clbGrupos);
            tabPage2.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tabPage2.Location = new Point(4, 29);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(949, 396);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Grupos";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // clbGrupos
            // 
            clbGrupos.FormattingEnabled = true;
            clbGrupos.Location = new Point(16, 33);
            clbGrupos.Name = "clbGrupos";
            clbGrupos.Size = new Size(830, 229);
            clbGrupos.TabIndex = 0;
            clbGrupos.ItemCheck += clbGrupos_ItemCheck;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(tvAcciones1);
            tabPage3.Location = new Point(4, 29);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(949, 396);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Acciones";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // tvAcciones1
            // 
            tvAcciones1.CheckBoxes = true;
            tvAcciones1.Font = new Font("Segoe UI", 12F);
            tvAcciones1.Location = new Point(19, 25);
            tvAcciones1.Name = "tvAcciones1";
            tvAcciones1.Size = new Size(906, 351);
            tvAcciones1.TabIndex = 0;
            tvAcciones1.AfterCheck += tvAcciones1_AfterCheck;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = SystemColors.Control;
            btnGuardar.Font = new Font("Microsoft Sans Serif", 11.25F);
            btnGuardar.Location = new Point(834, 510);
            btnGuardar.Margin = new Padding(5, 4, 5, 4);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(123, 60);
            btnGuardar.TabIndex = 10;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = false;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // panelSuperior
            // 
            panelSuperior.Controls.Add(pbLogo);
            panelSuperior.Controls.Add(btnCancelar);
            panelSuperior.Dock = DockStyle.Top;
            panelSuperior.Location = new Point(0, 0);
            panelSuperior.Name = "panelSuperior";
            panelSuperior.Size = new Size(967, 56);
            panelSuperior.TabIndex = 13;
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
            btnCancelar.Location = new Point(863, 12);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(94, 29);
            btnCancelar.TabIndex = 8;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // FrmUsuario
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(967, 583);
            ControlBox = false;
            Controls.Add(panelSuperior);
            Controls.Add(tabControl1);
            Controls.Add(btnGuardar);
            Name = "FrmUsuario";
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            panelSuperior.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbLogo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPage1;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private ComboBox cbEstados;
        private TextBox txtDescripcion;
        private TextBox txtNombre;
        private TextBox txtUsuario;
        private TabPage tabPage2;
        private TreeView tvAcciones;
        private TreeView tvAcciones1;
        private CheckedListBox clbGrupos;
        private TextBox txtEmail;
        private TabPage tabPage3;
        private Button btnGuardar;
        private Panel panelSuperior;
        private PictureBox pbLogo;
        private Button btnCancelar;
        private TextBox txtTelefono;
        private Label label6;
        private TextBox txtDNI;
        private Label label5;
    }
}