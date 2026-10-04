namespace Vista
{
    partial class FrmGestionarReportes
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
            panelParametros = new Panel();
            lblTipo = new Label();
            cbTipoReporte = new ComboBox();
            lblRango = new Label();
            cbRango = new ComboBox();
            lblDesde = new Label();
            dtpDesde = new DateTimePicker();
            lblHasta = new Label();
            dtpHasta = new DateTimePicker();
            lblAgrupacion = new Label();
            cbAgrupacion = new ComboBox();
            flpFiltros = new FlowLayoutPanel();
            pnlFiltroMedio = new Panel();
            lblMedio = new Label();
            cbMedioPago = new ComboBox();
            pnlFiltroCategoria = new Panel();
            lblCategoria = new Label();
            cbCategoria = new ComboBox();
            pnlFiltroProveedor = new Panel();
            lblProveedor = new Label();
            cbProveedor = new ComboBox();
            pnlFiltroTop = new Panel();
            lblTop = new Label();
            cbTop = new ComboBox();
            pnlFiltroCriterio = new Panel();
            lblCriterio = new Label();
            cbCriterio = new ComboBox();
            pnlFiltroConsumidor = new Panel();
            chkExcluirConsumidorFinal = new CheckBox();
            btnGenerar = new Button();
            btnLimpiar = new Button();
            btnSalir = new Button();
            btnExcel = new Button();
            btnPdf = new Button();
            tlpKpis = new TableLayoutPanel();
            panelKpi1 = new Panel();
            lblKpiValor1 = new Label();
            lblKpiDetalle1 = new Label();
            lblKpiTitulo1 = new Label();
            panelKpi2 = new Panel();
            lblKpiValor2 = new Label();
            lblKpiDetalle2 = new Label();
            lblKpiTitulo2 = new Label();
            panelKpi3 = new Panel();
            lblKpiValor3 = new Label();
            lblKpiDetalle3 = new Label();
            lblKpiTitulo3 = new Label();
            panelKpi4 = new Panel();
            lblKpiValor4 = new Label();
            lblKpiDetalle4 = new Label();
            lblKpiTitulo4 = new Label();
            lblDescripcion = new Label();
            tabResultados = new TabControl();
            tabTabla = new TabPage();
            dgvReporte = new DataGridView();
            tabGraficos = new TabPage();
            tlpGraficos = new TableLayoutPanel();
            panelCargando = new Panel();
            lblCargando = new Label();
            pbCargando = new ProgressBar();
            panelParametros.SuspendLayout();
            flpFiltros.SuspendLayout();
            pnlFiltroMedio.SuspendLayout();
            pnlFiltroCategoria.SuspendLayout();
            pnlFiltroProveedor.SuspendLayout();
            pnlFiltroTop.SuspendLayout();
            pnlFiltroCriterio.SuspendLayout();
            pnlFiltroConsumidor.SuspendLayout();
            tlpKpis.SuspendLayout();
            panelKpi1.SuspendLayout();
            panelKpi2.SuspendLayout();
            panelKpi3.SuspendLayout();
            panelKpi4.SuspendLayout();
            tabResultados.SuspendLayout();
            tabTabla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvReporte).BeginInit();
            tabGraficos.SuspendLayout();
            panelCargando.SuspendLayout();
            SuspendLayout();
            //
            // panelParametros
            //
            panelParametros.Controls.Add(lblTipo);
            panelParametros.Controls.Add(cbTipoReporte);
            panelParametros.Controls.Add(lblRango);
            panelParametros.Controls.Add(cbRango);
            panelParametros.Controls.Add(lblDesde);
            panelParametros.Controls.Add(dtpDesde);
            panelParametros.Controls.Add(lblHasta);
            panelParametros.Controls.Add(dtpHasta);
            panelParametros.Controls.Add(lblAgrupacion);
            panelParametros.Controls.Add(cbAgrupacion);
            panelParametros.Controls.Add(flpFiltros);
            panelParametros.Controls.Add(btnGenerar);
            panelParametros.Controls.Add(btnLimpiar);
            panelParametros.Controls.Add(btnSalir);
            panelParametros.Controls.Add(btnExcel);
            panelParametros.Controls.Add(btnPdf);
            panelParametros.Dock = DockStyle.Top;
            panelParametros.Location = new Point(0, 0);
            panelParametros.Name = "panelParametros";
            panelParametros.Size = new Size(1427, 136);
            panelParametros.TabIndex = 0;
            //
            // lblTipo
            //
            lblTipo.AutoSize = true;
            lblTipo.Location = new Point(12, 8);
            lblTipo.Name = "lblTipo";
            lblTipo.Size = new Size(60, 20);
            lblTipo.TabIndex = 0;
            lblTipo.Tag = "BLANCO";
            lblTipo.Text = "Reporte";
            //
            // cbTipoReporte
            //
            cbTipoReporte.DropDownStyle = ComboBoxStyle.DropDownList;
            cbTipoReporte.Font = new Font("Segoe UI", 10F);
            cbTipoReporte.Location = new Point(12, 31);
            cbTipoReporte.Name = "cbTipoReporte";
            cbTipoReporte.Size = new Size(290, 31);
            cbTipoReporte.TabIndex = 0;
            //
            // lblRango
            //
            lblRango.AutoSize = true;
            lblRango.Location = new Point(314, 8);
            lblRango.Name = "lblRango";
            lblRango.Size = new Size(59, 20);
            lblRango.TabIndex = 2;
            lblRango.Tag = "BLANCO";
            lblRango.Text = "Período";
            //
            // cbRango
            //
            cbRango.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRango.Font = new Font("Segoe UI", 10F);
            cbRango.Location = new Point(314, 31);
            cbRango.Name = "cbRango";
            cbRango.Size = new Size(170, 31);
            cbRango.TabIndex = 1;
            //
            // lblDesde
            //
            lblDesde.AutoSize = true;
            lblDesde.Location = new Point(496, 8);
            lblDesde.Name = "lblDesde";
            lblDesde.Size = new Size(50, 20);
            lblDesde.TabIndex = 4;
            lblDesde.Tag = "BLANCO";
            lblDesde.Text = "Desde";
            //
            // dtpDesde
            //
            dtpDesde.Font = new Font("Segoe UI", 10F);
            dtpDesde.Format = DateTimePickerFormat.Short;
            dtpDesde.Location = new Point(496, 31);
            dtpDesde.Name = "dtpDesde";
            dtpDesde.Size = new Size(130, 30);
            dtpDesde.TabIndex = 2;
            //
            // lblHasta
            //
            lblHasta.AutoSize = true;
            lblHasta.Location = new Point(638, 8);
            lblHasta.Name = "lblHasta";
            lblHasta.Size = new Size(47, 20);
            lblHasta.TabIndex = 6;
            lblHasta.Tag = "BLANCO";
            lblHasta.Text = "Hasta";
            //
            // dtpHasta
            //
            dtpHasta.Font = new Font("Segoe UI", 10F);
            dtpHasta.Format = DateTimePickerFormat.Short;
            dtpHasta.Location = new Point(638, 31);
            dtpHasta.Name = "dtpHasta";
            dtpHasta.Size = new Size(130, 30);
            dtpHasta.TabIndex = 3;
            //
            // lblAgrupacion
            //
            lblAgrupacion.AutoSize = true;
            lblAgrupacion.Location = new Point(780, 8);
            lblAgrupacion.Name = "lblAgrupacion";
            lblAgrupacion.Size = new Size(86, 20);
            lblAgrupacion.TabIndex = 8;
            lblAgrupacion.Tag = "BLANCO";
            lblAgrupacion.Text = "Agrupación";
            //
            // cbAgrupacion
            //
            cbAgrupacion.DropDownStyle = ComboBoxStyle.DropDownList;
            cbAgrupacion.Font = new Font("Segoe UI", 10F);
            cbAgrupacion.Location = new Point(780, 31);
            cbAgrupacion.Name = "cbAgrupacion";
            cbAgrupacion.Size = new Size(130, 31);
            cbAgrupacion.TabIndex = 4;
            //
            // flpFiltros
            //
            flpFiltros.Controls.Add(pnlFiltroMedio);
            flpFiltros.Controls.Add(pnlFiltroCategoria);
            flpFiltros.Controls.Add(pnlFiltroProveedor);
            flpFiltros.Controls.Add(pnlFiltroTop);
            flpFiltros.Controls.Add(pnlFiltroCriterio);
            flpFiltros.Controls.Add(pnlFiltroConsumidor);
            flpFiltros.Location = new Point(8, 70);
            flpFiltros.Name = "flpFiltros";
            flpFiltros.Size = new Size(1010, 62);
            flpFiltros.TabIndex = 5;
            flpFiltros.WrapContents = false;
            //
            // pnlFiltroMedio
            //
            pnlFiltroMedio.Controls.Add(lblMedio);
            pnlFiltroMedio.Controls.Add(cbMedioPago);
            pnlFiltroMedio.Margin = new Padding(4, 0, 8, 0);
            pnlFiltroMedio.Name = "pnlFiltroMedio";
            pnlFiltroMedio.Size = new Size(200, 60);
            pnlFiltroMedio.TabIndex = 0;
            //
            // lblMedio
            //
            lblMedio.AutoSize = true;
            lblMedio.Location = new Point(0, 2);
            lblMedio.Name = "lblMedio";
            lblMedio.Size = new Size(108, 20);
            lblMedio.TabIndex = 0;
            lblMedio.Tag = "BLANCO";
            lblMedio.Text = "Medio de pago";
            //
            // cbMedioPago
            //
            cbMedioPago.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMedioPago.Location = new Point(0, 26);
            cbMedioPago.Name = "cbMedioPago";
            cbMedioPago.Size = new Size(196, 28);
            cbMedioPago.TabIndex = 1;
            //
            // pnlFiltroCategoria
            //
            pnlFiltroCategoria.Controls.Add(lblCategoria);
            pnlFiltroCategoria.Controls.Add(cbCategoria);
            pnlFiltroCategoria.Margin = new Padding(4, 0, 8, 0);
            pnlFiltroCategoria.Name = "pnlFiltroCategoria";
            pnlFiltroCategoria.Size = new Size(200, 60);
            pnlFiltroCategoria.TabIndex = 1;
            //
            // lblCategoria
            //
            lblCategoria.AutoSize = true;
            lblCategoria.Location = new Point(0, 2);
            lblCategoria.Name = "lblCategoria";
            lblCategoria.Size = new Size(74, 20);
            lblCategoria.TabIndex = 0;
            lblCategoria.Tag = "BLANCO";
            lblCategoria.Text = "Categoría";
            //
            // cbCategoria
            //
            cbCategoria.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCategoria.Location = new Point(0, 26);
            cbCategoria.Name = "cbCategoria";
            cbCategoria.Size = new Size(196, 28);
            cbCategoria.TabIndex = 1;
            //
            // pnlFiltroProveedor
            //
            pnlFiltroProveedor.Controls.Add(lblProveedor);
            pnlFiltroProveedor.Controls.Add(cbProveedor);
            pnlFiltroProveedor.Margin = new Padding(4, 0, 8, 0);
            pnlFiltroProveedor.Name = "pnlFiltroProveedor";
            pnlFiltroProveedor.Size = new Size(250, 60);
            pnlFiltroProveedor.TabIndex = 2;
            //
            // lblProveedor
            //
            lblProveedor.AutoSize = true;
            lblProveedor.Location = new Point(0, 2);
            lblProveedor.Name = "lblProveedor";
            lblProveedor.Size = new Size(77, 20);
            lblProveedor.TabIndex = 0;
            lblProveedor.Tag = "BLANCO";
            lblProveedor.Text = "Proveedor";
            //
            // cbProveedor
            //
            cbProveedor.DropDownStyle = ComboBoxStyle.DropDownList;
            cbProveedor.Location = new Point(0, 26);
            cbProveedor.Name = "cbProveedor";
            cbProveedor.Size = new Size(246, 28);
            cbProveedor.TabIndex = 1;
            //
            // pnlFiltroTop
            //
            pnlFiltroTop.Controls.Add(lblTop);
            pnlFiltroTop.Controls.Add(cbTop);
            pnlFiltroTop.Margin = new Padding(4, 0, 8, 0);
            pnlFiltroTop.Name = "pnlFiltroTop";
            pnlFiltroTop.Size = new Size(110, 60);
            pnlFiltroTop.TabIndex = 3;
            //
            // lblTop
            //
            lblTop.AutoSize = true;
            lblTop.Location = new Point(0, 2);
            lblTop.Name = "lblTop";
            lblTop.Size = new Size(61, 20);
            lblTop.TabIndex = 0;
            lblTop.Tag = "BLANCO";
            lblTop.Text = "Mostrar";
            //
            // cbTop
            //
            cbTop.DropDownStyle = ComboBoxStyle.DropDownList;
            cbTop.Location = new Point(0, 26);
            cbTop.Name = "cbTop";
            cbTop.Size = new Size(106, 28);
            cbTop.TabIndex = 1;
            //
            // pnlFiltroCriterio
            //
            pnlFiltroCriterio.Controls.Add(lblCriterio);
            pnlFiltroCriterio.Controls.Add(cbCriterio);
            pnlFiltroCriterio.Margin = new Padding(4, 0, 8, 0);
            pnlFiltroCriterio.Name = "pnlFiltroCriterio";
            pnlFiltroCriterio.Size = new Size(170, 60);
            pnlFiltroCriterio.TabIndex = 4;
            //
            // lblCriterio
            //
            lblCriterio.AutoSize = true;
            lblCriterio.Location = new Point(0, 2);
            lblCriterio.Name = "lblCriterio";
            lblCriterio.Size = new Size(87, 20);
            lblCriterio.TabIndex = 0;
            lblCriterio.Tag = "BLANCO";
            lblCriterio.Text = "Ordenar por";
            //
            // cbCriterio
            //
            cbCriterio.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCriterio.Location = new Point(0, 26);
            cbCriterio.Name = "cbCriterio";
            cbCriterio.Size = new Size(166, 28);
            cbCriterio.TabIndex = 1;
            //
            // pnlFiltroConsumidor
            //
            pnlFiltroConsumidor.Controls.Add(chkExcluirConsumidorFinal);
            pnlFiltroConsumidor.Margin = new Padding(4, 0, 8, 0);
            pnlFiltroConsumidor.Name = "pnlFiltroConsumidor";
            pnlFiltroConsumidor.Size = new Size(220, 60);
            pnlFiltroConsumidor.TabIndex = 5;
            //
            // chkExcluirConsumidorFinal
            //
            chkExcluirConsumidorFinal.AutoSize = true;
            chkExcluirConsumidorFinal.Checked = true;
            chkExcluirConsumidorFinal.CheckState = CheckState.Checked;
            chkExcluirConsumidorFinal.Location = new Point(0, 28);
            chkExcluirConsumidorFinal.Name = "chkExcluirConsumidorFinal";
            chkExcluirConsumidorFinal.Size = new Size(201, 24);
            chkExcluirConsumidorFinal.TabIndex = 0;
            chkExcluirConsumidorFinal.Text = "Excluir Consumidor Final";
            chkExcluirConsumidorFinal.UseVisualStyleBackColor = true;
            //
            // btnGenerar
            //
            btnGenerar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGenerar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnGenerar.Location = new Point(1030, 8);
            btnGenerar.Name = "btnGenerar";
            btnGenerar.Size = new Size(180, 56);
            btnGenerar.TabIndex = 6;
            btnGenerar.Text = "Generar reporte (F5)";
            btnGenerar.UseVisualStyleBackColor = true;
            //
            // btnLimpiar
            //
            btnLimpiar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLimpiar.Location = new Point(1216, 8);
            btnLimpiar.Name = "btnLimpiar";
            btnLimpiar.Size = new Size(100, 56);
            btnLimpiar.TabIndex = 7;
            btnLimpiar.Text = "Limpiar filtros";
            btnLimpiar.UseVisualStyleBackColor = true;
            //
            // btnSalir
            //
            btnSalir.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSalir.Location = new Point(1322, 8);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(93, 56);
            btnSalir.TabIndex = 8;
            btnSalir.Text = "Volver";
            btnSalir.UseVisualStyleBackColor = true;
            //
            // btnExcel
            //
            btnExcel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExcel.Location = new Point(1030, 72);
            btnExcel.Name = "btnExcel";
            btnExcel.Size = new Size(190, 56);
            btnExcel.TabIndex = 9;
            btnExcel.Text = "Exportar a Excel";
            btnExcel.UseVisualStyleBackColor = true;
            //
            // btnPdf
            //
            btnPdf.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPdf.Location = new Point(1226, 72);
            btnPdf.Name = "btnPdf";
            btnPdf.Size = new Size(189, 56);
            btnPdf.TabIndex = 10;
            btnPdf.Text = "Exportar a PDF";
            btnPdf.UseVisualStyleBackColor = true;
            //
            // tlpKpis
            //
            tlpKpis.ColumnCount = 4;
            tlpKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpKpis.Controls.Add(panelKpi1, 0, 0);
            tlpKpis.Controls.Add(panelKpi2, 1, 0);
            tlpKpis.Controls.Add(panelKpi3, 2, 0);
            tlpKpis.Controls.Add(panelKpi4, 3, 0);
            tlpKpis.Dock = DockStyle.Top;
            tlpKpis.Location = new Point(0, 136);
            tlpKpis.Name = "tlpKpis";
            tlpKpis.Padding = new Padding(6, 4, 6, 4);
            tlpKpis.RowCount = 1;
            tlpKpis.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpKpis.Size = new Size(1427, 104);
            tlpKpis.TabIndex = 1;
            //
            // panelKpi1
            //
            panelKpi1.Controls.Add(lblKpiValor1);
            panelKpi1.Controls.Add(lblKpiDetalle1);
            panelKpi1.Controls.Add(lblKpiTitulo1);
            panelKpi1.Dock = DockStyle.Fill;
            panelKpi1.Margin = new Padding(6);
            panelKpi1.Name = "panelKpi1";
            panelKpi1.Padding = new Padding(10, 6, 10, 6);
            panelKpi1.TabIndex = 0;
            //
            // lblKpiValor1
            //
            lblKpiValor1.Dock = DockStyle.Fill;
            lblKpiValor1.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblKpiValor1.Name = "lblKpiValor1";
            lblKpiValor1.Tag = "BLANCO";
            lblKpiValor1.Text = "—";
            lblKpiValor1.TextAlign = ContentAlignment.MiddleLeft;
            //
            // lblKpiDetalle1
            //
            lblKpiDetalle1.Dock = DockStyle.Bottom;
            lblKpiDetalle1.Font = new Font("Segoe UI", 8.5F);
            lblKpiDetalle1.Name = "lblKpiDetalle1";
            lblKpiDetalle1.Size = new Size(100, 18);
            lblKpiDetalle1.Tag = "BLANCO";
            //
            // lblKpiTitulo1
            //
            lblKpiTitulo1.Dock = DockStyle.Top;
            lblKpiTitulo1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblKpiTitulo1.Name = "lblKpiTitulo1";
            lblKpiTitulo1.Size = new Size(100, 20);
            lblKpiTitulo1.Tag = "BLANCO";
            //
            // panelKpi2
            //
            panelKpi2.Controls.Add(lblKpiValor2);
            panelKpi2.Controls.Add(lblKpiDetalle2);
            panelKpi2.Controls.Add(lblKpiTitulo2);
            panelKpi2.Dock = DockStyle.Fill;
            panelKpi2.Margin = new Padding(6);
            panelKpi2.Name = "panelKpi2";
            panelKpi2.Padding = new Padding(10, 6, 10, 6);
            panelKpi2.TabIndex = 1;
            //
            // lblKpiValor2
            //
            lblKpiValor2.Dock = DockStyle.Fill;
            lblKpiValor2.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblKpiValor2.Name = "lblKpiValor2";
            lblKpiValor2.Tag = "BLANCO";
            lblKpiValor2.Text = "—";
            lblKpiValor2.TextAlign = ContentAlignment.MiddleLeft;
            //
            // lblKpiDetalle2
            //
            lblKpiDetalle2.Dock = DockStyle.Bottom;
            lblKpiDetalle2.Font = new Font("Segoe UI", 8.5F);
            lblKpiDetalle2.Name = "lblKpiDetalle2";
            lblKpiDetalle2.Size = new Size(100, 18);
            lblKpiDetalle2.Tag = "BLANCO";
            //
            // lblKpiTitulo2
            //
            lblKpiTitulo2.Dock = DockStyle.Top;
            lblKpiTitulo2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblKpiTitulo2.Name = "lblKpiTitulo2";
            lblKpiTitulo2.Size = new Size(100, 20);
            lblKpiTitulo2.Tag = "BLANCO";
            //
            // panelKpi3
            //
            panelKpi3.Controls.Add(lblKpiValor3);
            panelKpi3.Controls.Add(lblKpiDetalle3);
            panelKpi3.Controls.Add(lblKpiTitulo3);
            panelKpi3.Dock = DockStyle.Fill;
            panelKpi3.Margin = new Padding(6);
            panelKpi3.Name = "panelKpi3";
            panelKpi3.Padding = new Padding(10, 6, 10, 6);
            panelKpi3.TabIndex = 2;
            //
            // lblKpiValor3
            //
            lblKpiValor3.Dock = DockStyle.Fill;
            lblKpiValor3.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblKpiValor3.Name = "lblKpiValor3";
            lblKpiValor3.Tag = "BLANCO";
            lblKpiValor3.Text = "—";
            lblKpiValor3.TextAlign = ContentAlignment.MiddleLeft;
            //
            // lblKpiDetalle3
            //
            lblKpiDetalle3.Dock = DockStyle.Bottom;
            lblKpiDetalle3.Font = new Font("Segoe UI", 8.5F);
            lblKpiDetalle3.Name = "lblKpiDetalle3";
            lblKpiDetalle3.Size = new Size(100, 18);
            lblKpiDetalle3.Tag = "BLANCO";
            //
            // lblKpiTitulo3
            //
            lblKpiTitulo3.Dock = DockStyle.Top;
            lblKpiTitulo3.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblKpiTitulo3.Name = "lblKpiTitulo3";
            lblKpiTitulo3.Size = new Size(100, 20);
            lblKpiTitulo3.Tag = "BLANCO";
            //
            // panelKpi4
            //
            panelKpi4.Controls.Add(lblKpiValor4);
            panelKpi4.Controls.Add(lblKpiDetalle4);
            panelKpi4.Controls.Add(lblKpiTitulo4);
            panelKpi4.Dock = DockStyle.Fill;
            panelKpi4.Margin = new Padding(6);
            panelKpi4.Name = "panelKpi4";
            panelKpi4.Padding = new Padding(10, 6, 10, 6);
            panelKpi4.TabIndex = 3;
            //
            // lblKpiValor4
            //
            lblKpiValor4.Dock = DockStyle.Fill;
            lblKpiValor4.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblKpiValor4.Name = "lblKpiValor4";
            lblKpiValor4.Tag = "BLANCO";
            lblKpiValor4.Text = "—";
            lblKpiValor4.TextAlign = ContentAlignment.MiddleLeft;
            //
            // lblKpiDetalle4
            //
            lblKpiDetalle4.Dock = DockStyle.Bottom;
            lblKpiDetalle4.Font = new Font("Segoe UI", 8.5F);
            lblKpiDetalle4.Name = "lblKpiDetalle4";
            lblKpiDetalle4.Size = new Size(100, 18);
            lblKpiDetalle4.Tag = "BLANCO";
            //
            // lblKpiTitulo4
            //
            lblKpiTitulo4.Dock = DockStyle.Top;
            lblKpiTitulo4.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblKpiTitulo4.Name = "lblKpiTitulo4";
            lblKpiTitulo4.Size = new Size(100, 20);
            lblKpiTitulo4.Tag = "BLANCO";
            //
            // lblDescripcion
            //
            lblDescripcion.Dock = DockStyle.Top;
            lblDescripcion.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblDescripcion.Location = new Point(0, 240);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Padding = new Padding(10, 0, 0, 0);
            lblDescripcion.Size = new Size(1427, 30);
            lblDescripcion.TabIndex = 2;
            lblDescripcion.Text = "Elegí un reporte y un período. Se genera automáticamente.";
            lblDescripcion.TextAlign = ContentAlignment.MiddleLeft;
            //
            // tabResultados
            //
            tabResultados.Controls.Add(tabTabla);
            tabResultados.Controls.Add(tabGraficos);
            tabResultados.Dock = DockStyle.Fill;
            tabResultados.Font = new Font("Segoe UI", 10F);
            tabResultados.Location = new Point(0, 270);
            tabResultados.Name = "tabResultados";
            tabResultados.SelectedIndex = 0;
            tabResultados.Size = new Size(1427, 630);
            tabResultados.TabIndex = 3;
            //
            // tabTabla
            //
            tabTabla.Controls.Add(dgvReporte);
            tabTabla.Location = new Point(4, 32);
            tabTabla.Name = "tabTabla";
            tabTabla.Size = new Size(1419, 594);
            tabTabla.TabIndex = 0;
            tabTabla.Text = "Tabla / detalle";
            //
            // dgvReporte
            //
            dgvReporte.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvReporte.Dock = DockStyle.Fill;
            dgvReporte.Location = new Point(0, 0);
            dgvReporte.Name = "dgvReporte";
            dgvReporte.RowHeadersWidth = 51;
            dgvReporte.Size = new Size(1419, 594);
            dgvReporte.TabIndex = 0;
            //
            // tabGraficos
            //
            tabGraficos.Controls.Add(tlpGraficos);
            tabGraficos.Location = new Point(4, 32);
            tabGraficos.Name = "tabGraficos";
            tabGraficos.Size = new Size(1419, 594);
            tabGraficos.TabIndex = 1;
            tabGraficos.Text = "Gráficos";
            //
            // tlpGraficos
            //
            tlpGraficos.ColumnCount = 2;
            tlpGraficos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpGraficos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpGraficos.Dock = DockStyle.Fill;
            tlpGraficos.Location = new Point(0, 0);
            tlpGraficos.Name = "tlpGraficos";
            tlpGraficos.RowCount = 2;
            tlpGraficos.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlpGraficos.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlpGraficos.Size = new Size(1419, 594);
            tlpGraficos.TabIndex = 0;
            //
            // panelCargando
            //
            panelCargando.BorderStyle = BorderStyle.FixedSingle;
            panelCargando.Controls.Add(lblCargando);
            panelCargando.Controls.Add(pbCargando);
            panelCargando.Location = new Point(553, 450);
            panelCargando.Name = "panelCargando";
            panelCargando.Size = new Size(320, 96);
            panelCargando.TabIndex = 4;
            panelCargando.Visible = false;
            //
            // lblCargando
            //
            lblCargando.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblCargando.Location = new Point(10, 12);
            lblCargando.Name = "lblCargando";
            lblCargando.Size = new Size(298, 30);
            lblCargando.TabIndex = 0;
            lblCargando.Tag = "BLANCO";
            lblCargando.Text = "Generando reporte...";
            lblCargando.TextAlign = ContentAlignment.MiddleCenter;
            //
            // pbCargando
            //
            pbCargando.Location = new Point(20, 52);
            pbCargando.MarqueeAnimationSpeed = 25;
            pbCargando.Name = "pbCargando";
            pbCargando.Size = new Size(278, 22);
            pbCargando.Style = ProgressBarStyle.Marquee;
            pbCargando.TabIndex = 1;
            //
            // FrmGestionarReportes
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1427, 900);
            ControlBox = false;
            Controls.Add(panelCargando);
            Controls.Add(tabResultados);
            Controls.Add(lblDescripcion);
            Controls.Add(tlpKpis);
            Controls.Add(panelParametros);
            KeyPreview = true;
            Name = "FrmGestionarReportes";
            Text = "REPORTES";
            Load += FrmGestionarReportes_Load;
            panelParametros.ResumeLayout(false);
            panelParametros.PerformLayout();
            flpFiltros.ResumeLayout(false);
            pnlFiltroMedio.ResumeLayout(false);
            pnlFiltroMedio.PerformLayout();
            pnlFiltroCategoria.ResumeLayout(false);
            pnlFiltroCategoria.PerformLayout();
            pnlFiltroProveedor.ResumeLayout(false);
            pnlFiltroProveedor.PerformLayout();
            pnlFiltroTop.ResumeLayout(false);
            pnlFiltroTop.PerformLayout();
            pnlFiltroCriterio.ResumeLayout(false);
            pnlFiltroCriterio.PerformLayout();
            pnlFiltroConsumidor.ResumeLayout(false);
            pnlFiltroConsumidor.PerformLayout();
            tlpKpis.ResumeLayout(false);
            panelKpi1.ResumeLayout(false);
            panelKpi2.ResumeLayout(false);
            panelKpi3.ResumeLayout(false);
            panelKpi4.ResumeLayout(false);
            tabResultados.ResumeLayout(false);
            tabTabla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvReporte).EndInit();
            tabGraficos.ResumeLayout(false);
            panelCargando.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panelParametros;
        private Label lblTipo;
        private ComboBox cbTipoReporte;
        private Label lblRango;
        private ComboBox cbRango;
        private Label lblDesde;
        private DateTimePicker dtpDesde;
        private Label lblHasta;
        private DateTimePicker dtpHasta;
        private Label lblAgrupacion;
        private ComboBox cbAgrupacion;
        private FlowLayoutPanel flpFiltros;
        private Panel pnlFiltroMedio;
        private Label lblMedio;
        private ComboBox cbMedioPago;
        private Panel pnlFiltroCategoria;
        private Label lblCategoria;
        private ComboBox cbCategoria;
        private Panel pnlFiltroProveedor;
        private Label lblProveedor;
        private ComboBox cbProveedor;
        private Panel pnlFiltroTop;
        private Label lblTop;
        private ComboBox cbTop;
        private Panel pnlFiltroCriterio;
        private Label lblCriterio;
        private ComboBox cbCriterio;
        private Panel pnlFiltroConsumidor;
        private CheckBox chkExcluirConsumidorFinal;
        private Button btnGenerar;
        private Button btnLimpiar;
        private Button btnSalir;
        private Button btnExcel;
        private Button btnPdf;
        private TableLayoutPanel tlpKpis;
        private Panel panelKpi1;
        private Label lblKpiValor1;
        private Label lblKpiDetalle1;
        private Label lblKpiTitulo1;
        private Panel panelKpi2;
        private Label lblKpiValor2;
        private Label lblKpiDetalle2;
        private Label lblKpiTitulo2;
        private Panel panelKpi3;
        private Label lblKpiValor3;
        private Label lblKpiDetalle3;
        private Label lblKpiTitulo3;
        private Panel panelKpi4;
        private Label lblKpiValor4;
        private Label lblKpiDetalle4;
        private Label lblKpiTitulo4;
        private Label lblDescripcion;
        private TabControl tabResultados;
        private TabPage tabTabla;
        private DataGridView dgvReporte;
        private TabPage tabGraficos;
        private TableLayoutPanel tlpGraficos;
        private Panel panelCargando;
        private Label lblCargando;
        private ProgressBar pbCargando;
    }
}
