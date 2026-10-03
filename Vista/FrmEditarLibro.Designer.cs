namespace Vista
{
    partial class FrmEditarLibro
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
            components = new System.ComponentModel.Container();
            gbDatos = new GroupBox();
            lblIsbn = new Label();
            txtIsbn = new TextBox();
            lblIsbnInfo = new Label();
            lblTitulo = new Label();
            txtTitulo = new TextBox();
            lblAutor = new Label();
            txtAutor = new TextBox();
            lblEditorial = new Label();
            txtEditorial = new TextBox();
            lblGenero = new Label();
            cbGenero = new ComboBox();
            btnNuevoGenero = new Button();
            lblAnio = new Label();
            numAnio = new NumericUpDown();
            lblDescripcion = new Label();
            txtDescripcion = new TextBox();
            gbPrecios = new GroupBox();
            lblCosto = new Label();
            numCosto = new NumericUpDown();
            lblVenta = new Label();
            numVenta = new NumericUpDown();
            lblMargen = new Label();
            gbStock = new GroupBox();
            lblStock = new Label();
            numStock = new NumericUpDown();
            lblStockInfo = new Label();
            lblMinimo = new Label();
            numMinimo = new NumericUpDown();
            lblPunto = new Label();
            numPunto = new NumericUpDown();
            lblOptimo = new Label();
            numOptimo = new NumericUpDown();
            gbProveedores = new GroupBox();
            cbProveedor = new ComboBox();
            numPrecioCompra = new NumericUpDown();
            btnAgregarProveedor = new Button();
            dgvProveedores = new DataGridView();
            btnGuardar = new Button();
            btnCancelar = new Button();
            lblEstadoLibro = new Label();
            errorProvider = new ErrorProvider(components);
            gbDatos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numAnio).BeginInit();
            gbPrecios.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numCosto).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numVenta).BeginInit();
            gbStock.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numStock).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMinimo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPunto).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numOptimo).BeginInit();
            gbProveedores.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numPrecioCompra).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvProveedores).BeginInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
            SuspendLayout();
            //
            // gbDatos
            //
            gbDatos.Controls.Add(lblIsbn);
            gbDatos.Controls.Add(txtIsbn);
            gbDatos.Controls.Add(lblIsbnInfo);
            gbDatos.Controls.Add(lblTitulo);
            gbDatos.Controls.Add(txtTitulo);
            gbDatos.Controls.Add(lblAutor);
            gbDatos.Controls.Add(txtAutor);
            gbDatos.Controls.Add(lblEditorial);
            gbDatos.Controls.Add(txtEditorial);
            gbDatos.Controls.Add(lblGenero);
            gbDatos.Controls.Add(cbGenero);
            gbDatos.Controls.Add(btnNuevoGenero);
            gbDatos.Controls.Add(lblAnio);
            gbDatos.Controls.Add(numAnio);
            gbDatos.Controls.Add(lblDescripcion);
            gbDatos.Controls.Add(txtDescripcion);
            gbDatos.Location = new Point(12, 12);
            gbDatos.Name = "gbDatos";
            gbDatos.Size = new Size(440, 560);
            gbDatos.TabIndex = 0;
            gbDatos.TabStop = false;
            gbDatos.Text = "Datos del libro";
            //
            // lblIsbn
            //
            lblIsbn.AutoSize = true;
            lblIsbn.Location = new Point(14, 32);
            lblIsbn.Name = "lblIsbn";
            lblIsbn.Size = new Size(40, 20);
            lblIsbn.TabIndex = 0;
            lblIsbn.Text = "ISBN";
            //
            // txtIsbn
            //
            txtIsbn.Location = new Point(130, 29);
            txtIsbn.MaxLength = 20;
            txtIsbn.Name = "txtIsbn";
            txtIsbn.Size = new Size(220, 27);
            txtIsbn.TabIndex = 0;
            //
            // lblIsbnInfo
            //
            lblIsbnInfo.AutoEllipsis = true;
            lblIsbnInfo.Font = new Font("Segoe UI", 8F);
            lblIsbnInfo.Location = new Point(130, 58);
            lblIsbnInfo.Name = "lblIsbnInfo";
            lblIsbnInfo.Size = new Size(290, 20);
            lblIsbnInfo.TabIndex = 2;
            lblIsbnInfo.Text = "Opcional. 10 o 13 dígitos (se aceptan guiones).";
            //
            // lblTitulo
            //
            lblTitulo.AutoSize = true;
            lblTitulo.Location = new Point(14, 88);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(55, 20);
            lblTitulo.TabIndex = 3;
            lblTitulo.Text = "Título *";
            //
            // txtTitulo
            //
            txtTitulo.Location = new Point(130, 85);
            txtTitulo.MaxLength = 200;
            txtTitulo.Name = "txtTitulo";
            txtTitulo.Size = new Size(285, 27);
            txtTitulo.TabIndex = 1;
            //
            // lblAutor
            //
            lblAutor.AutoSize = true;
            lblAutor.Location = new Point(14, 126);
            lblAutor.Name = "lblAutor";
            lblAutor.Size = new Size(57, 20);
            lblAutor.TabIndex = 5;
            lblAutor.Text = "Autor *";
            //
            // txtAutor
            //
            txtAutor.Location = new Point(130, 123);
            txtAutor.MaxLength = 150;
            txtAutor.Name = "txtAutor";
            txtAutor.Size = new Size(285, 27);
            txtAutor.TabIndex = 2;
            //
            // lblEditorial
            //
            lblEditorial.AutoSize = true;
            lblEditorial.Location = new Point(14, 164);
            lblEditorial.Name = "lblEditorial";
            lblEditorial.Size = new Size(75, 20);
            lblEditorial.TabIndex = 7;
            lblEditorial.Text = "Editorial *";
            //
            // txtEditorial
            //
            txtEditorial.Location = new Point(130, 161);
            txtEditorial.MaxLength = 150;
            txtEditorial.Name = "txtEditorial";
            txtEditorial.Size = new Size(285, 27);
            txtEditorial.TabIndex = 3;
            //
            // lblGenero
            //
            lblGenero.AutoSize = true;
            lblGenero.Location = new Point(14, 202);
            lblGenero.Name = "lblGenero";
            lblGenero.Size = new Size(68, 20);
            lblGenero.TabIndex = 9;
            lblGenero.Text = "Género *";
            //
            // cbGenero
            //
            cbGenero.DropDownStyle = ComboBoxStyle.DropDownList;
            cbGenero.Location = new Point(130, 199);
            cbGenero.Name = "cbGenero";
            cbGenero.Size = new Size(240, 28);
            cbGenero.TabIndex = 4;
            //
            // btnNuevoGenero
            //
            btnNuevoGenero.Location = new Point(376, 198);
            btnNuevoGenero.Name = "btnNuevoGenero";
            btnNuevoGenero.Size = new Size(39, 30);
            btnNuevoGenero.TabIndex = 5;
            btnNuevoGenero.Text = "+";
            btnNuevoGenero.UseVisualStyleBackColor = true;
            //
            // lblAnio
            //
            lblAnio.AutoSize = true;
            lblAnio.Location = new Point(14, 240);
            lblAnio.Name = "lblAnio";
            lblAnio.Size = new Size(110, 20);
            lblAnio.TabIndex = 12;
            lblAnio.Text = "Año publicación";
            //
            // numAnio
            //
            numAnio.Location = new Point(130, 237);
            numAnio.Maximum = new decimal(new int[] { 3000, 0, 0, 0 });
            numAnio.Name = "numAnio";
            numAnio.Size = new Size(100, 27);
            numAnio.TabIndex = 6;
            //
            // lblDescripcion
            //
            lblDescripcion.AutoSize = true;
            lblDescripcion.Location = new Point(14, 278);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Size = new Size(87, 20);
            lblDescripcion.TabIndex = 14;
            lblDescripcion.Text = "Descripción";
            //
            // txtDescripcion
            //
            txtDescripcion.AcceptsReturn = true;
            txtDescripcion.Location = new Point(14, 302);
            txtDescripcion.MaxLength = 2000;
            txtDescripcion.Multiline = true;
            txtDescripcion.Name = "txtDescripcion";
            txtDescripcion.ScrollBars = ScrollBars.Vertical;
            txtDescripcion.Size = new Size(401, 240);
            txtDescripcion.TabIndex = 7;
            //
            // gbPrecios
            //
            gbPrecios.Controls.Add(lblCosto);
            gbPrecios.Controls.Add(numCosto);
            gbPrecios.Controls.Add(lblVenta);
            gbPrecios.Controls.Add(numVenta);
            gbPrecios.Controls.Add(lblMargen);
            gbPrecios.Location = new Point(464, 12);
            gbPrecios.Name = "gbPrecios";
            gbPrecios.Size = new Size(434, 116);
            gbPrecios.TabIndex = 1;
            gbPrecios.TabStop = false;
            gbPrecios.Text = "Precios";
            //
            // lblCosto
            //
            lblCosto.AutoSize = true;
            lblCosto.Location = new Point(14, 34);
            lblCosto.Name = "lblCosto";
            lblCosto.Size = new Size(110, 20);
            lblCosto.TabIndex = 0;
            lblCosto.Text = "Precio de costo";
            //
            // numCosto
            //
            numCosto.DecimalPlaces = 2;
            numCosto.Location = new Point(170, 31);
            numCosto.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            numCosto.Name = "numCosto";
            numCosto.Size = new Size(130, 27);
            numCosto.TabIndex = 0;
            numCosto.TextAlign = HorizontalAlignment.Right;
            numCosto.ThousandsSeparator = true;
            //
            // lblVenta
            //
            lblVenta.AutoSize = true;
            lblVenta.Location = new Point(14, 72);
            lblVenta.Name = "lblVenta";
            lblVenta.Size = new Size(121, 20);
            lblVenta.TabIndex = 2;
            lblVenta.Text = "Precio de venta *";
            //
            // numVenta
            //
            numVenta.DecimalPlaces = 2;
            numVenta.Location = new Point(170, 69);
            numVenta.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            numVenta.Name = "numVenta";
            numVenta.Size = new Size(130, 27);
            numVenta.TabIndex = 1;
            numVenta.TextAlign = HorizontalAlignment.Right;
            numVenta.ThousandsSeparator = true;
            //
            // lblMargen
            //
            lblMargen.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblMargen.Location = new Point(310, 34);
            lblMargen.Name = "lblMargen";
            lblMargen.Size = new Size(116, 62);
            lblMargen.TabIndex = 4;
            lblMargen.Text = "Margen: —";
            lblMargen.TextAlign = ContentAlignment.MiddleCenter;
            //
            // gbStock
            //
            gbStock.Controls.Add(lblStock);
            gbStock.Controls.Add(numStock);
            gbStock.Controls.Add(lblStockInfo);
            gbStock.Controls.Add(lblMinimo);
            gbStock.Controls.Add(numMinimo);
            gbStock.Controls.Add(lblPunto);
            gbStock.Controls.Add(numPunto);
            gbStock.Controls.Add(lblOptimo);
            gbStock.Controls.Add(numOptimo);
            gbStock.Location = new Point(464, 136);
            gbStock.Name = "gbStock";
            gbStock.Size = new Size(434, 150);
            gbStock.TabIndex = 2;
            gbStock.TabStop = false;
            gbStock.Text = "Inventario";
            //
            // lblStock
            //
            lblStock.AutoSize = true;
            lblStock.Location = new Point(14, 34);
            lblStock.Name = "lblStock";
            lblStock.Size = new Size(92, 20);
            lblStock.TabIndex = 0;
            lblStock.Text = "Stock actual";
            //
            // numStock
            //
            numStock.Location = new Point(170, 31);
            numStock.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numStock.Name = "numStock";
            numStock.Size = new Size(90, 27);
            numStock.TabIndex = 0;
            numStock.TextAlign = HorizontalAlignment.Right;
            //
            // lblStockInfo
            //
            lblStockInfo.AutoEllipsis = true;
            lblStockInfo.Font = new Font("Segoe UI", 8F);
            lblStockInfo.Location = new Point(266, 34);
            lblStockInfo.Name = "lblStockInfo";
            lblStockInfo.Size = new Size(160, 20);
            lblStockInfo.TabIndex = 2;
            lblStockInfo.Text = "Stock inicial del alta";
            //
            // lblMinimo
            //
            lblMinimo.AutoSize = true;
            lblMinimo.Location = new Point(14, 72);
            lblMinimo.Name = "lblMinimo";
            lblMinimo.Size = new Size(101, 20);
            lblMinimo.TabIndex = 3;
            lblMinimo.Text = "Stock mínimo";
            //
            // numMinimo
            //
            numMinimo.Location = new Point(170, 69);
            numMinimo.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numMinimo.Name = "numMinimo";
            numMinimo.Size = new Size(90, 27);
            numMinimo.TabIndex = 1;
            numMinimo.TextAlign = HorizontalAlignment.Right;
            //
            // lblPunto
            //
            lblPunto.AutoSize = true;
            lblPunto.Location = new Point(14, 110);
            lblPunto.Name = "lblPunto";
            lblPunto.Size = new Size(145, 20);
            lblPunto.TabIndex = 5;
            lblPunto.Text = "Punto de reposición";
            //
            // numPunto
            //
            numPunto.Location = new Point(170, 107);
            numPunto.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numPunto.Name = "numPunto";
            numPunto.Size = new Size(90, 27);
            numPunto.TabIndex = 2;
            numPunto.TextAlign = HorizontalAlignment.Right;
            //
            // lblOptimo
            //
            lblOptimo.AutoSize = true;
            lblOptimo.Location = new Point(276, 72);
            lblOptimo.Name = "lblOptimo";
            lblOptimo.Size = new Size(58, 20);
            lblOptimo.TabIndex = 7;
            lblOptimo.Text = "Óptimo";
            //
            // numOptimo
            //
            numOptimo.Location = new Point(340, 69);
            numOptimo.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numOptimo.Name = "numOptimo";
            numOptimo.Size = new Size(80, 27);
            numOptimo.TabIndex = 3;
            numOptimo.TextAlign = HorizontalAlignment.Right;
            //
            // gbProveedores
            //
            gbProveedores.Controls.Add(cbProveedor);
            gbProveedores.Controls.Add(numPrecioCompra);
            gbProveedores.Controls.Add(btnAgregarProveedor);
            gbProveedores.Controls.Add(dgvProveedores);
            gbProveedores.Location = new Point(464, 294);
            gbProveedores.Name = "gbProveedores";
            gbProveedores.Size = new Size(434, 278);
            gbProveedores.TabIndex = 3;
            gbProveedores.TabStop = false;
            gbProveedores.Text = "Proveedores y precio de compra";
            //
            // cbProveedor
            //
            cbProveedor.DropDownStyle = ComboBoxStyle.DropDownList;
            cbProveedor.Location = new Point(14, 30);
            cbProveedor.Name = "cbProveedor";
            cbProveedor.Size = new Size(220, 28);
            cbProveedor.TabIndex = 0;
            //
            // numPrecioCompra
            //
            numPrecioCompra.DecimalPlaces = 2;
            numPrecioCompra.Location = new Point(240, 31);
            numPrecioCompra.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            numPrecioCompra.Name = "numPrecioCompra";
            numPrecioCompra.Size = new Size(100, 27);
            numPrecioCompra.TabIndex = 1;
            numPrecioCompra.TextAlign = HorizontalAlignment.Right;
            numPrecioCompra.ThousandsSeparator = true;
            //
            // btnAgregarProveedor
            //
            btnAgregarProveedor.Location = new Point(346, 29);
            btnAgregarProveedor.Name = "btnAgregarProveedor";
            btnAgregarProveedor.Size = new Size(74, 30);
            btnAgregarProveedor.TabIndex = 2;
            btnAgregarProveedor.Text = "Agregar";
            btnAgregarProveedor.UseVisualStyleBackColor = true;
            //
            // dgvProveedores
            //
            dgvProveedores.AllowUserToAddRows = false;
            dgvProveedores.AllowUserToDeleteRows = false;
            dgvProveedores.AllowUserToResizeRows = false;
            dgvProveedores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProveedores.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProveedores.Location = new Point(14, 68);
            dgvProveedores.MultiSelect = false;
            dgvProveedores.Name = "dgvProveedores";
            dgvProveedores.RowHeadersVisible = false;
            dgvProveedores.RowHeadersWidth = 51;
            dgvProveedores.Size = new Size(406, 196);
            dgvProveedores.TabIndex = 3;
            //
            // btnGuardar
            //
            btnGuardar.Location = new Point(632, 584);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(130, 44);
            btnGuardar.TabIndex = 4;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            //
            // btnCancelar
            //
            btnCancelar.DialogResult = DialogResult.Cancel;
            btnCancelar.Location = new Point(768, 584);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(130, 44);
            btnCancelar.TabIndex = 5;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            //
            // lblEstadoLibro
            //
            lblEstadoLibro.AutoEllipsis = true;
            lblEstadoLibro.Location = new Point(12, 584);
            lblEstadoLibro.Name = "lblEstadoLibro";
            lblEstadoLibro.Size = new Size(600, 44);
            lblEstadoLibro.TabIndex = 6;
            lblEstadoLibro.Text = "Los campos con * son obligatorios.";
            lblEstadoLibro.TextAlign = ContentAlignment.MiddleLeft;
            //
            // errorProvider
            //
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            errorProvider.ContainerControl = this;
            //
            // FrmEditarLibro
            //
            AcceptButton = btnGuardar;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancelar;
            ClientSize = new Size(912, 640);
            Controls.Add(gbDatos);
            Controls.Add(gbPrecios);
            Controls.Add(gbStock);
            Controls.Add(gbProveedores);
            Controls.Add(btnGuardar);
            Controls.Add(btnCancelar);
            Controls.Add(lblEstadoLibro);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmEditarLibro";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Libro";
            Load += FrmEditarLibro_Load;
            gbDatos.ResumeLayout(false);
            gbDatos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numAnio).EndInit();
            gbPrecios.ResumeLayout(false);
            gbPrecios.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numCosto).EndInit();
            ((System.ComponentModel.ISupportInitialize)numVenta).EndInit();
            gbStock.ResumeLayout(false);
            gbStock.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numStock).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMinimo).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPunto).EndInit();
            ((System.ComponentModel.ISupportInitialize)numOptimo).EndInit();
            gbProveedores.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numPrecioCompra).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvProveedores).EndInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox gbDatos;
        private Label lblIsbn;
        private TextBox txtIsbn;
        private Label lblIsbnInfo;
        private Label lblTitulo;
        private TextBox txtTitulo;
        private Label lblAutor;
        private TextBox txtAutor;
        private Label lblEditorial;
        private TextBox txtEditorial;
        private Label lblGenero;
        private ComboBox cbGenero;
        private Button btnNuevoGenero;
        private Label lblAnio;
        private NumericUpDown numAnio;
        private Label lblDescripcion;
        private TextBox txtDescripcion;
        private GroupBox gbPrecios;
        private Label lblCosto;
        private NumericUpDown numCosto;
        private Label lblVenta;
        private NumericUpDown numVenta;
        private Label lblMargen;
        private GroupBox gbStock;
        private Label lblStock;
        private NumericUpDown numStock;
        private Label lblStockInfo;
        private Label lblMinimo;
        private NumericUpDown numMinimo;
        private Label lblPunto;
        private NumericUpDown numPunto;
        private Label lblOptimo;
        private NumericUpDown numOptimo;
        private GroupBox gbProveedores;
        private ComboBox cbProveedor;
        private NumericUpDown numPrecioCompra;
        private Button btnAgregarProveedor;
        private DataGridView dgvProveedores;
        private Button btnGuardar;
        private Button btnCancelar;
        private Label lblEstadoLibro;
        private ErrorProvider errorProvider;
    }
}
