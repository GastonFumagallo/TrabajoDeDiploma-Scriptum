namespace Vista.Seguridad
{
    partial class FrmGestionarUsuarios
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
            btnResetearClave = new Button();
            btnDesbloquear = new Button();
            btnExportar = new Button();
            btnImprimir = new Button();
            btnSalir = new Button();
            panelFiltros = new Panel();
            lblBuscar = new Label();
            txtBuscar = new TextBox();
            lblEstado = new Label();
            cbEstado = new ComboBox();
            lblFiltroExtra = new Label();
            cbFiltroExtra = new ComboBox();
            chkSoloBloqueados = new CheckBox();
            btnLimpiarFiltros = new Button();
            dgvListado = new DataGridView();
            lblResumen = new Label();
            panelAcciones.SuspendLayout();
            panelFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvListado).BeginInit();
            SuspendLayout();
            //
            // panelAcciones
            //
            panelAcciones.Controls.Add(btnNuevo);
            panelAcciones.Controls.Add(btnEditar);
            panelAcciones.Controls.Add(btnCambiarEstado);
            panelAcciones.Controls.Add(btnResetearClave);
            panelAcciones.Controls.Add(btnDesbloquear);
            panelAcciones.Controls.Add(btnExportar);
            panelAcciones.Controls.Add(btnImprimir);
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
            btnNuevo.Text = "Nuevo (F2)";
            btnNuevo.UseVisualStyleBackColor = true;
            //
            // btnEditar
            //
            btnEditar.Location = new Point(148, 8);
            btnEditar.Name = "btnEditar";
            btnEditar.Size = new Size(130, 64);
            btnEditar.TabIndex = 1;
            btnEditar.Text = "Modificar (F3)";
            btnEditar.UseVisualStyleBackColor = true;
            //
            // btnCambiarEstado
            //
            btnCambiarEstado.Location = new Point(284, 8);
            btnCambiarEstado.Name = "btnCambiarEstado";
            btnCambiarEstado.Size = new Size(130, 64);
            btnCambiarEstado.TabIndex = 2;
            btnCambiarEstado.Text = "Dar de baja (F4)";
            btnCambiarEstado.UseVisualStyleBackColor = true;
            //
            // btnResetearClave
            //
            btnResetearClave.Location = new Point(420, 8);
            btnResetearClave.Name = "btnResetearClave";
            btnResetearClave.Size = new Size(130, 64);
            btnResetearClave.TabIndex = 3;
            btnResetearClave.Text = "Resetear clave (Ctrl+R)";
            btnResetearClave.UseVisualStyleBackColor = true;
            //
            // btnDesbloquear
            //
            btnDesbloquear.Location = new Point(556, 8);
            btnDesbloquear.Name = "btnDesbloquear";
            btnDesbloquear.Size = new Size(130, 64);
            btnDesbloquear.TabIndex = 4;
            btnDesbloquear.Text = "Desbloquear";
            btnDesbloquear.UseVisualStyleBackColor = true;
            //
            // btnExportar
            //
            btnExportar.Location = new Point(692, 8);
            btnExportar.Name = "btnExportar";
            btnExportar.Size = new Size(130, 64);
            btnExportar.TabIndex = 5;
            btnExportar.Text = "Exportar a Excel";
            btnExportar.UseVisualStyleBackColor = true;
            //
            // btnImprimir
            //
            btnImprimir.Location = new Point(828, 8);
            btnImprimir.Name = "btnImprimir";
            btnImprimir.Size = new Size(130, 64);
            btnImprimir.TabIndex = 6;
            btnImprimir.Text = "Imprimir (Ctrl+P)";
            btnImprimir.UseVisualStyleBackColor = true;
            //
            // btnSalir
            //
            btnSalir.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSalir.Location = new Point(1295, 8);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(120, 64);
            btnSalir.TabIndex = 7;
            btnSalir.Text = "Volver";
            btnSalir.UseVisualStyleBackColor = true;
            //
            // panelFiltros
            //
            panelFiltros.Controls.Add(lblBuscar);
            panelFiltros.Controls.Add(txtBuscar);
            panelFiltros.Controls.Add(lblEstado);
            panelFiltros.Controls.Add(cbEstado);
            panelFiltros.Controls.Add(lblFiltroExtra);
            panelFiltros.Controls.Add(cbFiltroExtra);
            panelFiltros.Controls.Add(chkSoloBloqueados);
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
            lblBuscar.Size = new Size(220, 20);
            lblBuscar.TabIndex = 0;
            lblBuscar.Tag = "BLANCO";
            lblBuscar.Text = "Buscar (usuario, nombre o email)";
            //
            // txtBuscar
            //
            txtBuscar.Location = new Point(12, 32);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(380, 27);
            txtBuscar.TabIndex = 0;
            //
            // lblEstado
            //
            lblEstado.AutoSize = true;
            lblEstado.Location = new Point(406, 6);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(54, 20);
            lblEstado.TabIndex = 2;
            lblEstado.Tag = "BLANCO";
            lblEstado.Text = "Estado";
            //
            // cbEstado
            //
            cbEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEstado.Location = new Point(406, 31);
            cbEstado.Name = "cbEstado";
            cbEstado.Size = new Size(160, 28);
            cbEstado.TabIndex = 1;
            //
            // lblFiltroExtra
            //
            lblFiltroExtra.AutoSize = true;
            lblFiltroExtra.Location = new Point(580, 6);
            lblFiltroExtra.Name = "lblFiltroExtra";
            lblFiltroExtra.Size = new Size(52, 20);
            lblFiltroExtra.TabIndex = 4;
            lblFiltroExtra.Tag = "BLANCO";
            lblFiltroExtra.Text = "Grupo";
            //
            // cbFiltroExtra
            //
            cbFiltroExtra.DropDownStyle = ComboBoxStyle.DropDownList;
            cbFiltroExtra.Location = new Point(580, 31);
            cbFiltroExtra.Name = "cbFiltroExtra";
            cbFiltroExtra.Size = new Size(220, 28);
            cbFiltroExtra.TabIndex = 2;
            //
            // chkSoloBloqueados
            //
            chkSoloBloqueados.AutoSize = true;
            chkSoloBloqueados.Location = new Point(814, 34);
            chkSoloBloqueados.Name = "chkSoloBloqueados";
            chkSoloBloqueados.Size = new Size(140, 24);
            chkSoloBloqueados.TabIndex = 3;
            chkSoloBloqueados.Text = "Sólo bloqueados";
            chkSoloBloqueados.UseVisualStyleBackColor = true;
            //
            // btnLimpiarFiltros
            //
            btnLimpiarFiltros.Location = new Point(970, 26);
            btnLimpiarFiltros.Name = "btnLimpiarFiltros";
            btnLimpiarFiltros.Size = new Size(140, 38);
            btnLimpiarFiltros.TabIndex = 4;
            btnLimpiarFiltros.Text = "Limpiar filtros";
            btnLimpiarFiltros.UseVisualStyleBackColor = true;
            //
            // dgvListado
            //
            dgvListado.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvListado.Dock = DockStyle.Fill;
            dgvListado.Location = new Point(0, 150);
            dgvListado.Name = "dgvListado";
            dgvListado.RowHeadersWidth = 51;
            dgvListado.Size = new Size(1427, 575);
            dgvListado.TabIndex = 2;
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
            // FrmGestionarUsuarios
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1427, 753);
            ControlBox = false;
            Controls.Add(dgvListado);
            Controls.Add(lblResumen);
            Controls.Add(panelFiltros);
            Controls.Add(panelAcciones);
            KeyPreview = true;
            Name = "FrmGestionarUsuarios";
            Text = "USUARIOS";
            Load += FrmGestionarUsuarios_Load;
            panelAcciones.ResumeLayout(false);
            panelFiltros.ResumeLayout(false);
            panelFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvListado).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelAcciones;
        private Button btnNuevo;
        private Button btnEditar;
        private Button btnCambiarEstado;
        private Button btnResetearClave;
        private Button btnDesbloquear;
        private Button btnExportar;
        private Button btnImprimir;
        private Button btnSalir;
        private Panel panelFiltros;
        private Label lblBuscar;
        private TextBox txtBuscar;
        private Label lblEstado;
        private ComboBox cbEstado;
        private Label lblFiltroExtra;
        private ComboBox cbFiltroExtra;
        private CheckBox chkSoloBloqueados;
        private Button btnLimpiarFiltros;
        private DataGridView dgvListado;
        private Label lblResumen;
    }
}
