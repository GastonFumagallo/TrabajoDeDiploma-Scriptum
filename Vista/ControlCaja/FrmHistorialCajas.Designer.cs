namespace Vista.ControlCaja
{
    partial class FrmHistorialCajas
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
            panelFiltros = new Panel();
            lblDesdeTitulo = new Label();
            dtpDesde = new DateTimePicker();
            lblHastaTitulo = new Label();
            dtpHasta = new DateTimePicker();
            lblUsuarioTitulo = new Label();
            cbUsuario = new ComboBox();
            btnFiltrar = new Button();
            btnLimpiarFiltros = new Button();
            btnVolver = new Button();
            dgvCajas = new DataGridView();
            panelDetalle = new Panel();
            lblDetalleTitulo = new Label();
            lblDetalleInfo = new Label();
            dgvMedios = new DataGridView();
            lblObservacionesTitulo = new Label();
            txtObservaciones = new TextBox();
            lblAnuladasLuego = new Label();
            panelMetricas = new Panel();
            lblCantidadTitulo = new Label();
            lblCantidad = new Label();
            lblRecaudadoTitulo = new Label();
            lblRecaudado = new Label();
            lblDiferenciaTitulo = new Label();
            lblDiferencia = new Label();
            lblFaltantesTitulo = new Label();
            lblFaltantes = new Label();
            panelPie = new Panel();
            lblResumen = new Label();
            btnPaginaAnterior = new Button();
            lblPagina = new Label();
            btnPaginaSiguiente = new Button();
            panelFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCajas).BeginInit();
            panelDetalle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMedios).BeginInit();
            panelMetricas.SuspendLayout();
            panelPie.SuspendLayout();
            SuspendLayout();
            //
            // panelFiltros
            //
            panelFiltros.Controls.Add(lblDesdeTitulo);
            panelFiltros.Controls.Add(dtpDesde);
            panelFiltros.Controls.Add(lblHastaTitulo);
            panelFiltros.Controls.Add(dtpHasta);
            panelFiltros.Controls.Add(lblUsuarioTitulo);
            panelFiltros.Controls.Add(cbUsuario);
            panelFiltros.Controls.Add(btnFiltrar);
            panelFiltros.Controls.Add(btnLimpiarFiltros);
            panelFiltros.Controls.Add(btnVolver);
            panelFiltros.Dock = DockStyle.Top;
            panelFiltros.Location = new Point(0, 0);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new Size(1427, 80);
            panelFiltros.TabIndex = 0;
            //
            // lblDesdeTitulo
            //
            lblDesdeTitulo.AutoSize = true;
            lblDesdeTitulo.Location = new Point(12, 10);
            lblDesdeTitulo.Name = "lblDesdeTitulo";
            lblDesdeTitulo.Size = new Size(115, 20);
            lblDesdeTitulo.TabIndex = 0;
            lblDesdeTitulo.Tag = "BLANCO";
            lblDesdeTitulo.Text = "Abiertas desde";
            //
            // dtpDesde
            //
            dtpDesde.Format = DateTimePickerFormat.Short;
            dtpDesde.Location = new Point(12, 38);
            dtpDesde.Name = "dtpDesde";
            dtpDesde.Size = new Size(140, 27);
            dtpDesde.TabIndex = 1;
            //
            // lblHastaTitulo
            //
            lblHastaTitulo.AutoSize = true;
            lblHastaTitulo.Location = new Point(166, 10);
            lblHastaTitulo.Name = "lblHastaTitulo";
            lblHastaTitulo.Size = new Size(46, 20);
            lblHastaTitulo.TabIndex = 2;
            lblHastaTitulo.Tag = "BLANCO";
            lblHastaTitulo.Text = "Hasta";
            //
            // dtpHasta
            //
            dtpHasta.Format = DateTimePickerFormat.Short;
            dtpHasta.Location = new Point(166, 38);
            dtpHasta.Name = "dtpHasta";
            dtpHasta.Size = new Size(140, 27);
            dtpHasta.TabIndex = 3;
            //
            // lblUsuarioTitulo
            //
            lblUsuarioTitulo.AutoSize = true;
            lblUsuarioTitulo.Location = new Point(320, 10);
            lblUsuarioTitulo.Name = "lblUsuarioTitulo";
            lblUsuarioTitulo.Size = new Size(173, 20);
            lblUsuarioTitulo.TabIndex = 4;
            lblUsuarioTitulo.Tag = "BLANCO";
            lblUsuarioTitulo.Text = "Cajero (abrió o cerró)";
            //
            // cbUsuario
            //
            cbUsuario.DropDownStyle = ComboBoxStyle.DropDownList;
            cbUsuario.FormattingEnabled = true;
            cbUsuario.Location = new Point(320, 38);
            cbUsuario.Name = "cbUsuario";
            cbUsuario.Size = new Size(220, 28);
            cbUsuario.TabIndex = 5;
            //
            // btnFiltrar
            //
            btnFiltrar.Location = new Point(556, 30);
            btnFiltrar.Name = "btnFiltrar";
            btnFiltrar.Size = new Size(120, 40);
            btnFiltrar.TabIndex = 6;
            btnFiltrar.Text = "Buscar";
            btnFiltrar.UseVisualStyleBackColor = true;
            //
            // btnLimpiarFiltros
            //
            btnLimpiarFiltros.Location = new Point(684, 30);
            btnLimpiarFiltros.Name = "btnLimpiarFiltros";
            btnLimpiarFiltros.Size = new Size(140, 40);
            btnLimpiarFiltros.TabIndex = 7;
            btnLimpiarFiltros.Text = "Limpiar filtros";
            btnLimpiarFiltros.UseVisualStyleBackColor = true;
            //
            // btnVolver
            //
            btnVolver.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnVolver.Location = new Point(1275, 10);
            btnVolver.Name = "btnVolver";
            btnVolver.Size = new Size(140, 60);
            btnVolver.TabIndex = 8;
            btnVolver.Text = "Volver a ventas";
            btnVolver.UseVisualStyleBackColor = true;
            //
            // dgvCajas
            //
            dgvCajas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCajas.Dock = DockStyle.Fill;
            dgvCajas.Location = new Point(0, 80);
            dgvCajas.Name = "dgvCajas";
            dgvCajas.RowHeadersWidth = 51;
            dgvCajas.Size = new Size(997, 553);
            dgvCajas.TabIndex = 1;
            //
            // panelDetalle
            //
            panelDetalle.Controls.Add(lblDetalleTitulo);
            panelDetalle.Controls.Add(lblDetalleInfo);
            panelDetalle.Controls.Add(dgvMedios);
            panelDetalle.Controls.Add(lblObservacionesTitulo);
            panelDetalle.Controls.Add(txtObservaciones);
            panelDetalle.Controls.Add(lblAnuladasLuego);
            panelDetalle.Dock = DockStyle.Right;
            panelDetalle.Location = new Point(997, 80);
            panelDetalle.Name = "panelDetalle";
            panelDetalle.Padding = new Padding(12);
            panelDetalle.Size = new Size(430, 553);
            panelDetalle.TabIndex = 2;
            //
            // lblDetalleTitulo
            //
            lblDetalleTitulo.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblDetalleTitulo.Location = new Point(12, 10);
            lblDetalleTitulo.Name = "lblDetalleTitulo";
            lblDetalleTitulo.Size = new Size(406, 32);
            lblDetalleTitulo.TabIndex = 0;
            lblDetalleTitulo.Tag = "BLANCO";
            lblDetalleTitulo.Text = "Detalle de la caja";
            //
            // lblDetalleInfo
            //
            lblDetalleInfo.Location = new Point(12, 46);
            lblDetalleInfo.Name = "lblDetalleInfo";
            lblDetalleInfo.Size = new Size(406, 112);
            lblDetalleInfo.TabIndex = 1;
            lblDetalleInfo.Tag = "BLANCO";
            lblDetalleInfo.Text = "Seleccioná una caja para ver el desglose por medio de pago.";
            //
            // dgvMedios
            //
            dgvMedios.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvMedios.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMedios.Location = new Point(12, 164);
            dgvMedios.Name = "dgvMedios";
            dgvMedios.RowHeadersWidth = 51;
            dgvMedios.Size = new Size(406, 170);
            dgvMedios.TabIndex = 2;
            dgvMedios.TabStop = false;
            //
            // lblObservacionesTitulo
            //
            lblObservacionesTitulo.AutoSize = true;
            lblObservacionesTitulo.Location = new Point(12, 344);
            lblObservacionesTitulo.Name = "lblObservacionesTitulo";
            lblObservacionesTitulo.Size = new Size(212, 20);
            lblObservacionesTitulo.TabIndex = 3;
            lblObservacionesTitulo.Tag = "BLANCO";
            lblObservacionesTitulo.Text = "Observaciones del cierre";
            //
            // txtObservaciones
            //
            txtObservaciones.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtObservaciones.Location = new Point(12, 368);
            txtObservaciones.Multiline = true;
            txtObservaciones.Name = "txtObservaciones";
            txtObservaciones.ReadOnly = true;
            txtObservaciones.ScrollBars = ScrollBars.Vertical;
            txtObservaciones.Size = new Size(406, 130);
            txtObservaciones.TabIndex = 4;
            txtObservaciones.TabStop = false;
            //
            // lblAnuladasLuego
            //
            lblAnuladasLuego.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblAnuladasLuego.Location = new Point(12, 504);
            lblAnuladasLuego.Name = "lblAnuladasLuego";
            lblAnuladasLuego.Size = new Size(406, 40);
            lblAnuladasLuego.TabIndex = 5;
            lblAnuladasLuego.Tag = "BLANCO";
            //
            // panelMetricas
            //
            panelMetricas.Controls.Add(lblCantidadTitulo);
            panelMetricas.Controls.Add(lblCantidad);
            panelMetricas.Controls.Add(lblRecaudadoTitulo);
            panelMetricas.Controls.Add(lblRecaudado);
            panelMetricas.Controls.Add(lblDiferenciaTitulo);
            panelMetricas.Controls.Add(lblDiferencia);
            panelMetricas.Controls.Add(lblFaltantesTitulo);
            panelMetricas.Controls.Add(lblFaltantes);
            panelMetricas.Dock = DockStyle.Bottom;
            panelMetricas.Location = new Point(0, 633);
            panelMetricas.Name = "panelMetricas";
            panelMetricas.Size = new Size(1427, 72);
            panelMetricas.TabIndex = 3;
            //
            // lblCantidadTitulo
            //
            lblCantidadTitulo.AutoSize = true;
            lblCantidadTitulo.Location = new Point(12, 6);
            lblCantidadTitulo.Name = "lblCantidadTitulo";
            lblCantidadTitulo.Size = new Size(116, 20);
            lblCantidadTitulo.TabIndex = 0;
            lblCantidadTitulo.Tag = "BLANCO";
            lblCantidadTitulo.Text = "Cajas cerradas";
            //
            // lblCantidad
            //
            lblCantidad.AutoSize = true;
            lblCantidad.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblCantidad.Location = new Point(12, 28);
            lblCantidad.Name = "lblCantidad";
            lblCantidad.Size = new Size(32, 37);
            lblCantidad.TabIndex = 1;
            lblCantidad.Tag = "BLANCO";
            lblCantidad.Text = "0";
            //
            // lblRecaudadoTitulo
            //
            lblRecaudadoTitulo.AutoSize = true;
            lblRecaudadoTitulo.Location = new Point(220, 6);
            lblRecaudadoTitulo.Name = "lblRecaudadoTitulo";
            lblRecaudadoTitulo.Size = new Size(206, 20);
            lblRecaudadoTitulo.TabIndex = 2;
            lblRecaudadoTitulo.Tag = "BLANCO";
            lblRecaudadoTitulo.Text = "Total recaudado acumulado";
            //
            // lblRecaudado
            //
            lblRecaudado.AutoSize = true;
            lblRecaudado.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblRecaudado.Location = new Point(220, 28);
            lblRecaudado.Name = "lblRecaudado";
            lblRecaudado.Size = new Size(76, 37);
            lblRecaudado.TabIndex = 3;
            lblRecaudado.Tag = "BLANCO";
            lblRecaudado.Text = "$ 0,00";
            //
            // lblDiferenciaTitulo
            //
            lblDiferenciaTitulo.AutoSize = true;
            lblDiferenciaTitulo.Location = new Point(520, 6);
            lblDiferenciaTitulo.Name = "lblDiferenciaTitulo";
            lblDiferenciaTitulo.Size = new Size(228, 20);
            lblDiferenciaTitulo.TabIndex = 4;
            lblDiferenciaTitulo.Tag = "BLANCO";
            lblDiferenciaTitulo.Text = "Total de diferencias (neto)";
            //
            // lblDiferencia
            //
            lblDiferencia.AutoSize = true;
            lblDiferencia.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblDiferencia.Location = new Point(520, 28);
            lblDiferencia.Name = "lblDiferencia";
            lblDiferencia.Size = new Size(76, 37);
            lblDiferencia.TabIndex = 5;
            lblDiferencia.Tag = "BLANCO";
            lblDiferencia.Text = "$ 0,00";
            //
            // lblFaltantesTitulo
            //
            lblFaltantesTitulo.AutoSize = true;
            lblFaltantesTitulo.Location = new Point(820, 6);
            lblFaltantesTitulo.Name = "lblFaltantesTitulo";
            lblFaltantesTitulo.Size = new Size(70, 20);
            lblFaltantesTitulo.TabIndex = 6;
            lblFaltantesTitulo.Tag = "BLANCO";
            lblFaltantesTitulo.Text = "Faltantes";
            //
            // lblFaltantes
            //
            lblFaltantes.AutoSize = true;
            lblFaltantes.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblFaltantes.Location = new Point(820, 28);
            lblFaltantes.Name = "lblFaltantes";
            lblFaltantes.Size = new Size(76, 37);
            lblFaltantes.TabIndex = 7;
            lblFaltantes.Tag = "BLANCO";
            lblFaltantes.Text = "$ 0,00";
            //
            // panelPie
            //
            panelPie.Controls.Add(lblResumen);
            panelPie.Controls.Add(btnPaginaAnterior);
            panelPie.Controls.Add(lblPagina);
            panelPie.Controls.Add(btnPaginaSiguiente);
            panelPie.Dock = DockStyle.Bottom;
            panelPie.Location = new Point(0, 705);
            panelPie.Name = "panelPie";
            panelPie.Size = new Size(1427, 48);
            panelPie.TabIndex = 4;
            //
            // lblResumen
            //
            lblResumen.AutoSize = true;
            lblResumen.Location = new Point(12, 14);
            lblResumen.Name = "lblResumen";
            lblResumen.Size = new Size(0, 20);
            lblResumen.TabIndex = 0;
            lblResumen.Tag = "BLANCO";
            //
            // btnPaginaAnterior
            //
            btnPaginaAnterior.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPaginaAnterior.Location = new Point(1123, 7);
            btnPaginaAnterior.Name = "btnPaginaAnterior";
            btnPaginaAnterior.Size = new Size(60, 34);
            btnPaginaAnterior.TabIndex = 1;
            btnPaginaAnterior.Text = "◀";
            btnPaginaAnterior.UseVisualStyleBackColor = true;
            //
            // lblPagina
            //
            lblPagina.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblPagina.Location = new Point(1189, 7);
            lblPagina.Name = "lblPagina";
            lblPagina.Size = new Size(160, 34);
            lblPagina.TabIndex = 2;
            lblPagina.Tag = "BLANCO";
            lblPagina.TextAlign = ContentAlignment.MiddleCenter;
            //
            // btnPaginaSiguiente
            //
            btnPaginaSiguiente.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPaginaSiguiente.Location = new Point(1355, 7);
            btnPaginaSiguiente.Name = "btnPaginaSiguiente";
            btnPaginaSiguiente.Size = new Size(60, 34);
            btnPaginaSiguiente.TabIndex = 3;
            btnPaginaSiguiente.Text = "▶";
            btnPaginaSiguiente.UseVisualStyleBackColor = true;
            //
            // FrmHistorialCajas
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1427, 753);
            ControlBox = false;
            Controls.Add(dgvCajas);
            Controls.Add(panelDetalle);
            Controls.Add(panelMetricas);
            Controls.Add(panelPie);
            Controls.Add(panelFiltros);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FrmHistorialCajas";
            Text = "HISTORIAL DE CAJAS";
            panelFiltros.ResumeLayout(false);
            panelFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCajas).EndInit();
            panelDetalle.ResumeLayout(false);
            panelDetalle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMedios).EndInit();
            panelMetricas.ResumeLayout(false);
            panelMetricas.PerformLayout();
            panelPie.ResumeLayout(false);
            panelPie.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelFiltros;
        private Label lblDesdeTitulo;
        private DateTimePicker dtpDesde;
        private Label lblHastaTitulo;
        private DateTimePicker dtpHasta;
        private Label lblUsuarioTitulo;
        private ComboBox cbUsuario;
        private Button btnFiltrar;
        private Button btnLimpiarFiltros;
        private Button btnVolver;
        private DataGridView dgvCajas;
        private Panel panelDetalle;
        private Label lblDetalleTitulo;
        private Label lblDetalleInfo;
        private DataGridView dgvMedios;
        private Label lblObservacionesTitulo;
        private TextBox txtObservaciones;
        private Label lblAnuladasLuego;
        private Panel panelMetricas;
        private Label lblCantidadTitulo;
        private Label lblCantidad;
        private Label lblRecaudadoTitulo;
        private Label lblRecaudado;
        private Label lblDiferenciaTitulo;
        private Label lblDiferencia;
        private Label lblFaltantesTitulo;
        private Label lblFaltantes;
        private Panel panelPie;
        private Label lblResumen;
        private Button btnPaginaAnterior;
        private Label lblPagina;
        private Button btnPaginaSiguiente;
    }
}
