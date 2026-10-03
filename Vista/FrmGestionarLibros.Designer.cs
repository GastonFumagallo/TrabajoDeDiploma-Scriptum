namespace Vista
{
    partial class FrmGestionarLibros
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
            panelAcciones = new Panel();
            btnNuevo = new Button();
            btnEditar = new Button();
            btnCambiarEstado = new Button();
            btnExportar = new Button();
            btnSalir = new Button();
            panelFiltros = new Panel();
            lblBuscar = new Label();
            txtBuscar = new TextBox();
            lblGenero = new Label();
            cbGenero = new ComboBox();
            lblEstado = new Label();
            cbEstado = new ComboBox();
            btnLimpiarFiltros = new Button();
            dgvLibros = new DataGridView();
            lblResumen = new Label();
            panelAcciones.SuspendLayout();
            panelFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLibros).BeginInit();
            SuspendLayout();
            //
            // panelAcciones
            //
            panelAcciones.Controls.Add(btnNuevo);
            panelAcciones.Controls.Add(btnEditar);
            panelAcciones.Controls.Add(btnCambiarEstado);
            panelAcciones.Controls.Add(btnExportar);
            panelAcciones.Controls.Add(btnSalir);
            panelAcciones.Dock = DockStyle.Top;
            panelAcciones.Location = new Point(0, 0);
            panelAcciones.Name = "panelAcciones";
            panelAcciones.Size = new Size(1427, 80);
            panelAcciones.TabIndex = 0;
            //
            // btnNuevo
            //
            btnNuevo.Location = new Point(12, 8);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(130, 64);
            btnNuevo.TabIndex = 0;
            btnNuevo.Text = "Nuevo (Ctrl+N)";
            btnNuevo.UseVisualStyleBackColor = true;
            //
            // btnEditar
            //
            btnEditar.Location = new Point(148, 8);
            btnEditar.Name = "btnEditar";
            btnEditar.Size = new Size(130, 64);
            btnEditar.TabIndex = 1;
            btnEditar.Text = "Editar (Enter)";
            btnEditar.UseVisualStyleBackColor = true;
            //
            // btnCambiarEstado
            //
            btnCambiarEstado.Location = new Point(284, 8);
            btnCambiarEstado.Name = "btnCambiarEstado";
            btnCambiarEstado.Size = new Size(130, 64);
            btnCambiarEstado.TabIndex = 2;
            btnCambiarEstado.Text = "Desactivar (Supr)";
            btnCambiarEstado.UseVisualStyleBackColor = true;
            //
            // btnExportar
            //
            btnExportar.Location = new Point(420, 8);
            btnExportar.Name = "btnExportar";
            btnExportar.Size = new Size(130, 64);
            btnExportar.TabIndex = 3;
            btnExportar.Text = "Exportar a Excel";
            btnExportar.UseVisualStyleBackColor = true;
            //
            // btnSalir
            //
            btnSalir.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSalir.Location = new Point(1295, 8);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(120, 64);
            btnSalir.TabIndex = 4;
            btnSalir.Text = "Volver";
            btnSalir.UseVisualStyleBackColor = true;
            //
            // panelFiltros
            //
            panelFiltros.Controls.Add(lblBuscar);
            panelFiltros.Controls.Add(txtBuscar);
            panelFiltros.Controls.Add(lblGenero);
            panelFiltros.Controls.Add(cbGenero);
            panelFiltros.Controls.Add(lblEstado);
            panelFiltros.Controls.Add(cbEstado);
            panelFiltros.Controls.Add(btnLimpiarFiltros);
            panelFiltros.Dock = DockStyle.Top;
            panelFiltros.Location = new Point(0, 80);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new Size(1427, 70);
            panelFiltros.TabIndex = 1;
            //
            // lblBuscar
            //
            lblBuscar.AutoSize = true;
            lblBuscar.Location = new Point(12, 6);
            lblBuscar.Name = "lblBuscar";
            lblBuscar.Size = new Size(279, 20);
            lblBuscar.TabIndex = 0;
            lblBuscar.Tag = "BLANCO";
            lblBuscar.Text = "Buscar (ISBN, título, autor o editorial)";
            //
            // txtBuscar
            //
            txtBuscar.Location = new Point(12, 32);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(380, 27);
            txtBuscar.TabIndex = 0;
            //
            // lblGenero
            //
            lblGenero.AutoSize = true;
            lblGenero.Location = new Point(406, 6);
            lblGenero.Name = "lblGenero";
            lblGenero.Size = new Size(57, 20);
            lblGenero.TabIndex = 2;
            lblGenero.Tag = "BLANCO";
            lblGenero.Text = "Género";
            //
            // cbGenero
            //
            cbGenero.DropDownStyle = ComboBoxStyle.DropDownList;
            cbGenero.Location = new Point(406, 31);
            cbGenero.Name = "cbGenero";
            cbGenero.Size = new Size(220, 28);
            cbGenero.TabIndex = 1;
            //
            // lblEstado
            //
            lblEstado.AutoSize = true;
            lblEstado.Location = new Point(640, 6);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(54, 20);
            lblEstado.TabIndex = 4;
            lblEstado.Tag = "BLANCO";
            lblEstado.Text = "Estado";
            //
            // cbEstado
            //
            cbEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEstado.Location = new Point(640, 31);
            cbEstado.Name = "cbEstado";
            cbEstado.Size = new Size(160, 28);
            cbEstado.TabIndex = 2;
            //
            // btnLimpiarFiltros
            //
            btnLimpiarFiltros.Location = new Point(814, 26);
            btnLimpiarFiltros.Name = "btnLimpiarFiltros";
            btnLimpiarFiltros.Size = new Size(140, 38);
            btnLimpiarFiltros.TabIndex = 3;
            btnLimpiarFiltros.Text = "Limpiar filtros";
            btnLimpiarFiltros.UseVisualStyleBackColor = true;
            //
            // dgvLibros
            //
            dgvLibros.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLibros.Dock = DockStyle.Fill;
            dgvLibros.Location = new Point(0, 150);
            dgvLibros.Name = "dgvLibros";
            dgvLibros.RowHeadersWidth = 51;
            dgvLibros.Size = new Size(1427, 575);
            dgvLibros.TabIndex = 2;
            //
            // lblResumen
            //
            lblResumen.Dock = DockStyle.Bottom;
            lblResumen.Location = new Point(0, 725);
            lblResumen.Name = "lblResumen";
            lblResumen.Padding = new Padding(8, 0, 0, 0);
            lblResumen.Size = new Size(1427, 28);
            lblResumen.TabIndex = 3;
            lblResumen.TextAlign = ContentAlignment.MiddleLeft;
            //
            // FrmGestionarLibros
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1427, 753);
            ControlBox = false;
            Controls.Add(dgvLibros);
            Controls.Add(lblResumen);
            Controls.Add(panelFiltros);
            Controls.Add(panelAcciones);
            KeyPreview = true;
            Name = "FrmGestionarLibros";
            Text = "LIBROS";
            Load += FrmGestionarLibros_Load;
            panelAcciones.ResumeLayout(false);
            panelFiltros.ResumeLayout(false);
            panelFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLibros).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelAcciones;
        private Button btnNuevo;
        private Button btnEditar;
        private Button btnCambiarEstado;
        private Button btnExportar;
        private Button btnSalir;
        private Panel panelFiltros;
        private Label lblBuscar;
        private TextBox txtBuscar;
        private Label lblGenero;
        private ComboBox cbGenero;
        private Label lblEstado;
        private ComboBox cbEstado;
        private Button btnLimpiarFiltros;
        private DataGridView dgvLibros;
        private Label lblResumen;
    }
}
