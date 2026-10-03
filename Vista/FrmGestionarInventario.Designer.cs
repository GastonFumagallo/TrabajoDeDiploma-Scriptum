namespace Vista
{
    partial class FrmGestionarInventario
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
            panelTitulo = new Panel();
            lblTitulo = new Label();
            btnSalir = new Button();
            tabControl = new TabControl();
            tabExistencias = new TabPage();
            dgvInventario = new DataGridView();
            panelGrafico = new Panel();
            lblResumenInventario = new Label();
            panelIndicadores = new Panel();
            lblIndTotal = new Label();
            lblIndNormales = new Label();
            lblIndReponer = new Label();
            lblIndCriticos = new Label();
            lblIndAgotados = new Label();
            lblIndValor = new Label();
            panelFiltros = new Panel();
            lblBuscar = new Label();
            txtBuscar = new TextBox();
            lblCategoria = new Label();
            cbCategoria = new ComboBox();
            lblProveedor = new Label();
            cbProveedor = new ComboBox();
            lblEstado = new Label();
            cbEstado = new ComboBox();
            btnLimpiarFiltros = new Button();
            panelAcciones = new Panel();
            btnAjustarStock = new Button();
            btnParametros = new Button();
            btnHistorial = new Button();
            btnVerProveedores = new Button();
            btnOrdenReposicion = new Button();
            btnExportarExcel = new Button();
            btnExportarPdf = new Button();
            tabOrdenes = new TabPage();
            dgvOrdenes = new DataGridView();
            panelAccionesOrdenes = new Panel();
            btnNuevaOrden = new Button();
            btnAbrirOrden = new Button();
            btnRegistrarRecepcion = new Button();
            btnCancelarOrden = new Button();
            lblEstadoOrden = new Label();
            cbEstadoOrden = new ComboBox();
            lblProveedorOrden = new Label();
            cbProveedorOrden = new ComboBox();
            panelTitulo.SuspendLayout();
            tabControl.SuspendLayout();
            tabExistencias.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvInventario).BeginInit();
            panelIndicadores.SuspendLayout();
            panelFiltros.SuspendLayout();
            panelAcciones.SuspendLayout();
            tabOrdenes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvOrdenes).BeginInit();
            panelAccionesOrdenes.SuspendLayout();
            SuspendLayout();
            //
            // panelTitulo
            //
            panelTitulo.Controls.Add(lblTitulo);
            panelTitulo.Controls.Add(btnSalir);
            panelTitulo.Dock = DockStyle.Top;
            panelTitulo.Location = new Point(0, 0);
            panelTitulo.Name = "panelTitulo";
            panelTitulo.Size = new Size(1525, 58);
            panelTitulo.TabIndex = 0;
            //
            // lblTitulo
            //
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitulo.Location = new Point(12, 10);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(255, 37);
            lblTitulo.TabIndex = 0;
            lblTitulo.Tag = "Titulo";
            lblTitulo.Text = "Inventario y reposición";
            //
            // btnSalir
            //
            btnSalir.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSalir.Location = new Point(1405, 8);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(110, 42);
            btnSalir.TabIndex = 1;
            btnSalir.Text = "Volver";
            btnSalir.UseVisualStyleBackColor = true;
            //
            // tabControl
            //
            tabControl.Controls.Add(tabExistencias);
            tabControl.Controls.Add(tabOrdenes);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("Segoe UI", 10F);
            tabControl.Location = new Point(0, 58);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(1525, 997);
            tabControl.TabIndex = 1;
            //
            // tabExistencias
            //
            tabExistencias.Controls.Add(dgvInventario);
            tabExistencias.Controls.Add(panelGrafico);
            tabExistencias.Controls.Add(lblResumenInventario);
            tabExistencias.Controls.Add(panelIndicadores);
            tabExistencias.Controls.Add(panelFiltros);
            tabExistencias.Controls.Add(panelAcciones);
            tabExistencias.Location = new Point(4, 32);
            tabExistencias.Name = "tabExistencias";
            tabExistencias.Size = new Size(1517, 961);
            tabExistencias.TabIndex = 0;
            tabExistencias.Text = "Existencias";
            //
            // dgvInventario
            //
            dgvInventario.AllowUserToAddRows = false;
            dgvInventario.AllowUserToDeleteRows = false;
            dgvInventario.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvInventario.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvInventario.Dock = DockStyle.Fill;
            dgvInventario.Location = new Point(0, 206);
            dgvInventario.Name = "dgvInventario";
            dgvInventario.ReadOnly = true;
            dgvInventario.RowHeadersVisible = false;
            dgvInventario.RowHeadersWidth = 51;
            dgvInventario.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvInventario.Size = new Size(1157, 727);
            dgvInventario.TabIndex = 3;
            //
            // panelGrafico
            //
            panelGrafico.Dock = DockStyle.Right;
            panelGrafico.Location = new Point(1157, 206);
            panelGrafico.Name = "panelGrafico";
            panelGrafico.Size = new Size(360, 727);
            panelGrafico.TabIndex = 4;
            //
            // lblResumenInventario
            //
            lblResumenInventario.Dock = DockStyle.Bottom;
            lblResumenInventario.Location = new Point(0, 933);
            lblResumenInventario.Name = "lblResumenInventario";
            lblResumenInventario.Padding = new Padding(8, 0, 0, 0);
            lblResumenInventario.Size = new Size(1517, 28);
            lblResumenInventario.TabIndex = 5;
            lblResumenInventario.Tag = "BLANCO";
            lblResumenInventario.TextAlign = ContentAlignment.MiddleLeft;
            //
            // panelIndicadores
            //
            panelIndicadores.Controls.Add(lblIndTotal);
            panelIndicadores.Controls.Add(lblIndNormales);
            panelIndicadores.Controls.Add(lblIndReponer);
            panelIndicadores.Controls.Add(lblIndCriticos);
            panelIndicadores.Controls.Add(lblIndAgotados);
            panelIndicadores.Controls.Add(lblIndValor);
            panelIndicadores.Dock = DockStyle.Top;
            panelIndicadores.Location = new Point(0, 140);
            panelIndicadores.Name = "panelIndicadores";
            panelIndicadores.Size = new Size(1517, 66);
            panelIndicadores.TabIndex = 2;
            //
            // lblIndTotal
            //
            lblIndTotal.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblIndTotal.Location = new Point(12, 8);
            lblIndTotal.Name = "lblIndTotal";
            lblIndTotal.Size = new Size(200, 50);
            lblIndTotal.TabIndex = 0;
            lblIndTotal.Tag = "BLANCO";
            lblIndTotal.Text = "Productos\r\n0";
            //
            // lblIndNormales
            //
            lblIndNormales.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblIndNormales.Location = new Point(222, 8);
            lblIndNormales.Name = "lblIndNormales";
            lblIndNormales.Size = new Size(200, 50);
            lblIndNormales.TabIndex = 1;
            lblIndNormales.Tag = "BLANCO";
            lblIndNormales.Text = "Stock normal\r\n0";
            //
            // lblIndReponer
            //
            lblIndReponer.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblIndReponer.Location = new Point(432, 8);
            lblIndReponer.Name = "lblIndReponer";
            lblIndReponer.Size = new Size(200, 50);
            lblIndReponer.TabIndex = 2;
            lblIndReponer.Tag = "BLANCO";
            lblIndReponer.Text = "A reponer\r\n0";
            //
            // lblIndCriticos
            //
            lblIndCriticos.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblIndCriticos.Location = new Point(642, 8);
            lblIndCriticos.Name = "lblIndCriticos";
            lblIndCriticos.Size = new Size(200, 50);
            lblIndCriticos.TabIndex = 3;
            lblIndCriticos.Tag = "BLANCO";
            lblIndCriticos.Text = "Críticos (≤ mínimo)\r\n0";
            //
            // lblIndAgotados
            //
            lblIndAgotados.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblIndAgotados.Location = new Point(852, 8);
            lblIndAgotados.Name = "lblIndAgotados";
            lblIndAgotados.Size = new Size(200, 50);
            lblIndAgotados.TabIndex = 4;
            lblIndAgotados.Tag = "BLANCO";
            lblIndAgotados.Text = "Sin stock\r\n0";
            //
            // lblIndValor
            //
            lblIndValor.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblIndValor.Location = new Point(1062, 8);
            lblIndValor.Name = "lblIndValor";
            lblIndValor.Size = new Size(260, 50);
            lblIndValor.TabIndex = 5;
            lblIndValor.Tag = "BLANCO";
            lblIndValor.Text = "Valor a costo\r\n$0,00";
            //
            // panelFiltros
            //
            panelFiltros.Controls.Add(lblBuscar);
            panelFiltros.Controls.Add(txtBuscar);
            panelFiltros.Controls.Add(lblCategoria);
            panelFiltros.Controls.Add(cbCategoria);
            panelFiltros.Controls.Add(lblProveedor);
            panelFiltros.Controls.Add(cbProveedor);
            panelFiltros.Controls.Add(lblEstado);
            panelFiltros.Controls.Add(cbEstado);
            panelFiltros.Controls.Add(btnLimpiarFiltros);
            panelFiltros.Dock = DockStyle.Top;
            panelFiltros.Location = new Point(0, 72);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new Size(1517, 68);
            panelFiltros.TabIndex = 1;
            //
            // lblBuscar
            //
            lblBuscar.AutoSize = true;
            lblBuscar.Location = new Point(12, 6);
            lblBuscar.Name = "lblBuscar";
            lblBuscar.Size = new Size(236, 23);
            lblBuscar.TabIndex = 0;
            lblBuscar.Tag = "BLANCO";
            lblBuscar.Text = "Buscar (ISBN, título, autor)";
            //
            // txtBuscar
            //
            txtBuscar.Location = new Point(12, 32);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(300, 30);
            txtBuscar.TabIndex = 0;
            //
            // lblCategoria
            //
            lblCategoria.AutoSize = true;
            lblCategoria.Location = new Point(326, 6);
            lblCategoria.Name = "lblCategoria";
            lblCategoria.Size = new Size(87, 23);
            lblCategoria.TabIndex = 2;
            lblCategoria.Tag = "BLANCO";
            lblCategoria.Text = "Categoría";
            //
            // cbCategoria
            //
            cbCategoria.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCategoria.Location = new Point(326, 31);
            cbCategoria.Name = "cbCategoria";
            cbCategoria.Size = new Size(200, 31);
            cbCategoria.TabIndex = 1;
            //
            // lblProveedor
            //
            lblProveedor.AutoSize = true;
            lblProveedor.Location = new Point(540, 6);
            lblProveedor.Name = "lblProveedor";
            lblProveedor.Size = new Size(90, 23);
            lblProveedor.TabIndex = 4;
            lblProveedor.Tag = "BLANCO";
            lblProveedor.Text = "Proveedor";
            //
            // cbProveedor
            //
            cbProveedor.DropDownStyle = ComboBoxStyle.DropDownList;
            cbProveedor.Location = new Point(540, 31);
            cbProveedor.Name = "cbProveedor";
            cbProveedor.Size = new Size(240, 31);
            cbProveedor.TabIndex = 2;
            //
            // lblEstado
            //
            lblEstado.AutoSize = true;
            lblEstado.Location = new Point(794, 6);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(63, 23);
            lblEstado.TabIndex = 6;
            lblEstado.Tag = "BLANCO";
            lblEstado.Text = "Estado";
            //
            // cbEstado
            //
            cbEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEstado.Location = new Point(794, 31);
            cbEstado.Name = "cbEstado";
            cbEstado.Size = new Size(250, 31);
            cbEstado.TabIndex = 3;
            //
            // btnLimpiarFiltros
            //
            btnLimpiarFiltros.Location = new Point(1058, 26);
            btnLimpiarFiltros.Name = "btnLimpiarFiltros";
            btnLimpiarFiltros.Size = new Size(140, 38);
            btnLimpiarFiltros.TabIndex = 4;
            btnLimpiarFiltros.Text = "Limpiar filtros";
            btnLimpiarFiltros.UseVisualStyleBackColor = true;
            //
            // panelAcciones
            //
            panelAcciones.Controls.Add(btnAjustarStock);
            panelAcciones.Controls.Add(btnParametros);
            panelAcciones.Controls.Add(btnHistorial);
            panelAcciones.Controls.Add(btnVerProveedores);
            panelAcciones.Controls.Add(btnOrdenReposicion);
            panelAcciones.Controls.Add(btnExportarExcel);
            panelAcciones.Controls.Add(btnExportarPdf);
            panelAcciones.Dock = DockStyle.Top;
            panelAcciones.Location = new Point(0, 0);
            panelAcciones.Name = "panelAcciones";
            panelAcciones.Size = new Size(1517, 72);
            panelAcciones.TabIndex = 0;
            //
            // btnAjustarStock
            //
            btnAjustarStock.Location = new Point(12, 8);
            btnAjustarStock.Name = "btnAjustarStock";
            btnAjustarStock.Size = new Size(140, 56);
            btnAjustarStock.TabIndex = 0;
            btnAjustarStock.Text = "Ajustar stock";
            btnAjustarStock.UseVisualStyleBackColor = true;
            //
            // btnParametros
            //
            btnParametros.Location = new Point(158, 8);
            btnParametros.Name = "btnParametros";
            btnParametros.Size = new Size(140, 56);
            btnParametros.TabIndex = 1;
            btnParametros.Text = "Mínimo / óptimo";
            btnParametros.UseVisualStyleBackColor = true;
            //
            // btnHistorial
            //
            btnHistorial.Location = new Point(304, 8);
            btnHistorial.Name = "btnHistorial";
            btnHistorial.Size = new Size(140, 56);
            btnHistorial.TabIndex = 2;
            btnHistorial.Text = "Ver movimientos";
            btnHistorial.UseVisualStyleBackColor = true;
            //
            // btnVerProveedores
            //
            btnVerProveedores.Location = new Point(450, 8);
            btnVerProveedores.Name = "btnVerProveedores";
            btnVerProveedores.Size = new Size(140, 56);
            btnVerProveedores.TabIndex = 3;
            btnVerProveedores.Text = "Ver proveedores";
            btnVerProveedores.UseVisualStyleBackColor = true;
            //
            // btnOrdenReposicion
            //
            btnOrdenReposicion.Location = new Point(596, 8);
            btnOrdenReposicion.Name = "btnOrdenReposicion";
            btnOrdenReposicion.Size = new Size(200, 56);
            btnOrdenReposicion.TabIndex = 4;
            btnOrdenReposicion.Text = "Generar orden de reposición";
            btnOrdenReposicion.UseVisualStyleBackColor = true;
            //
            // btnExportarExcel
            //
            btnExportarExcel.Location = new Point(802, 8);
            btnExportarExcel.Name = "btnExportarExcel";
            btnExportarExcel.Size = new Size(140, 56);
            btnExportarExcel.TabIndex = 5;
            btnExportarExcel.Text = "Exportar Excel";
            btnExportarExcel.UseVisualStyleBackColor = true;
            //
            // btnExportarPdf
            //
            btnExportarPdf.Location = new Point(948, 8);
            btnExportarPdf.Name = "btnExportarPdf";
            btnExportarPdf.Size = new Size(140, 56);
            btnExportarPdf.TabIndex = 6;
            btnExportarPdf.Text = "Exportar PDF";
            btnExportarPdf.UseVisualStyleBackColor = true;
            //
            // tabOrdenes
            //
            tabOrdenes.Controls.Add(dgvOrdenes);
            tabOrdenes.Controls.Add(panelAccionesOrdenes);
            tabOrdenes.Location = new Point(4, 32);
            tabOrdenes.Name = "tabOrdenes";
            tabOrdenes.Size = new Size(1517, 961);
            tabOrdenes.TabIndex = 1;
            tabOrdenes.Text = "Órdenes de reposición";
            //
            // dgvOrdenes
            //
            dgvOrdenes.AllowUserToAddRows = false;
            dgvOrdenes.AllowUserToDeleteRows = false;
            dgvOrdenes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvOrdenes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvOrdenes.Dock = DockStyle.Fill;
            dgvOrdenes.Location = new Point(0, 72);
            dgvOrdenes.MultiSelect = false;
            dgvOrdenes.Name = "dgvOrdenes";
            dgvOrdenes.ReadOnly = true;
            dgvOrdenes.RowHeadersVisible = false;
            dgvOrdenes.RowHeadersWidth = 51;
            dgvOrdenes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOrdenes.Size = new Size(1517, 889);
            dgvOrdenes.TabIndex = 1;
            //
            // panelAccionesOrdenes
            //
            panelAccionesOrdenes.Controls.Add(btnNuevaOrden);
            panelAccionesOrdenes.Controls.Add(btnAbrirOrden);
            panelAccionesOrdenes.Controls.Add(btnRegistrarRecepcion);
            panelAccionesOrdenes.Controls.Add(btnCancelarOrden);
            panelAccionesOrdenes.Controls.Add(lblEstadoOrden);
            panelAccionesOrdenes.Controls.Add(cbEstadoOrden);
            panelAccionesOrdenes.Controls.Add(lblProveedorOrden);
            panelAccionesOrdenes.Controls.Add(cbProveedorOrden);
            panelAccionesOrdenes.Dock = DockStyle.Top;
            panelAccionesOrdenes.Location = new Point(0, 0);
            panelAccionesOrdenes.Name = "panelAccionesOrdenes";
            panelAccionesOrdenes.Size = new Size(1517, 72);
            panelAccionesOrdenes.TabIndex = 0;
            //
            // btnNuevaOrden
            //
            btnNuevaOrden.Location = new Point(12, 8);
            btnNuevaOrden.Name = "btnNuevaOrden";
            btnNuevaOrden.Size = new Size(140, 56);
            btnNuevaOrden.TabIndex = 0;
            btnNuevaOrden.Text = "Nueva orden";
            btnNuevaOrden.UseVisualStyleBackColor = true;
            //
            // btnAbrirOrden
            //
            btnAbrirOrden.Location = new Point(158, 8);
            btnAbrirOrden.Name = "btnAbrirOrden";
            btnAbrirOrden.Size = new Size(140, 56);
            btnAbrirOrden.TabIndex = 1;
            btnAbrirOrden.Text = "Abrir / editar";
            btnAbrirOrden.UseVisualStyleBackColor = true;
            //
            // btnRegistrarRecepcion
            //
            btnRegistrarRecepcion.Location = new Point(304, 8);
            btnRegistrarRecepcion.Name = "btnRegistrarRecepcion";
            btnRegistrarRecepcion.Size = new Size(160, 56);
            btnRegistrarRecepcion.TabIndex = 2;
            btnRegistrarRecepcion.Text = "Registrar recepción";
            btnRegistrarRecepcion.UseVisualStyleBackColor = true;
            //
            // btnCancelarOrden
            //
            btnCancelarOrden.Location = new Point(470, 8);
            btnCancelarOrden.Name = "btnCancelarOrden";
            btnCancelarOrden.Size = new Size(140, 56);
            btnCancelarOrden.TabIndex = 3;
            btnCancelarOrden.Text = "Cancelar orden";
            btnCancelarOrden.UseVisualStyleBackColor = true;
            //
            // lblEstadoOrden
            //
            lblEstadoOrden.AutoSize = true;
            lblEstadoOrden.Location = new Point(640, 6);
            lblEstadoOrden.Name = "lblEstadoOrden";
            lblEstadoOrden.Size = new Size(63, 23);
            lblEstadoOrden.TabIndex = 4;
            lblEstadoOrden.Tag = "BLANCO";
            lblEstadoOrden.Text = "Estado";
            //
            // cbEstadoOrden
            //
            cbEstadoOrden.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEstadoOrden.Location = new Point(640, 31);
            cbEstadoOrden.Name = "cbEstadoOrden";
            cbEstadoOrden.Size = new Size(220, 31);
            cbEstadoOrden.TabIndex = 4;
            //
            // lblProveedorOrden
            //
            lblProveedorOrden.AutoSize = true;
            lblProveedorOrden.Location = new Point(874, 6);
            lblProveedorOrden.Name = "lblProveedorOrden";
            lblProveedorOrden.Size = new Size(90, 23);
            lblProveedorOrden.TabIndex = 6;
            lblProveedorOrden.Tag = "BLANCO";
            lblProveedorOrden.Text = "Proveedor";
            //
            // cbProveedorOrden
            //
            cbProveedorOrden.DropDownStyle = ComboBoxStyle.DropDownList;
            cbProveedorOrden.Location = new Point(874, 31);
            cbProveedorOrden.Name = "cbProveedorOrden";
            cbProveedorOrden.Size = new Size(260, 31);
            cbProveedorOrden.TabIndex = 5;
            //
            // FrmGestionarInventario
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1525, 1055);
            ControlBox = false;
            Controls.Add(tabControl);
            Controls.Add(panelTitulo);
            Name = "FrmGestionarInventario";
            Text = "INVENTARIO";
            Load += FrmGestionarInventario_Load;
            panelTitulo.ResumeLayout(false);
            panelTitulo.PerformLayout();
            tabControl.ResumeLayout(false);
            tabExistencias.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvInventario).EndInit();
            panelIndicadores.ResumeLayout(false);
            panelFiltros.ResumeLayout(false);
            panelFiltros.PerformLayout();
            panelAcciones.ResumeLayout(false);
            tabOrdenes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvOrdenes).EndInit();
            panelAccionesOrdenes.ResumeLayout(false);
            panelAccionesOrdenes.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelTitulo;
        private Label lblTitulo;
        private Button btnSalir;
        private TabControl tabControl;
        private TabPage tabExistencias;
        private DataGridView dgvInventario;
        private Panel panelGrafico;
        private Label lblResumenInventario;
        private Panel panelIndicadores;
        private Label lblIndTotal;
        private Label lblIndNormales;
        private Label lblIndReponer;
        private Label lblIndCriticos;
        private Label lblIndAgotados;
        private Label lblIndValor;
        private Panel panelFiltros;
        private Label lblBuscar;
        private TextBox txtBuscar;
        private Label lblCategoria;
        private ComboBox cbCategoria;
        private Label lblProveedor;
        private ComboBox cbProveedor;
        private Label lblEstado;
        private ComboBox cbEstado;
        private Button btnLimpiarFiltros;
        private Panel panelAcciones;
        private Button btnAjustarStock;
        private Button btnParametros;
        private Button btnHistorial;
        private Button btnVerProveedores;
        private Button btnOrdenReposicion;
        private Button btnExportarExcel;
        private Button btnExportarPdf;
        private TabPage tabOrdenes;
        private DataGridView dgvOrdenes;
        private Panel panelAccionesOrdenes;
        private Button btnNuevaOrden;
        private Button btnAbrirOrden;
        private Button btnRegistrarRecepcion;
        private Button btnCancelarOrden;
        private Label lblEstadoOrden;
        private ComboBox cbEstadoOrden;
        private Label lblProveedorOrden;
        private ComboBox cbProveedorOrden;
    }
}
