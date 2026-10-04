namespace Vista.Seguridad
{
    partial class FrmGestionarGrupos
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
            splitContainer = new SplitContainer();
            dgvGrupos = new DataGridView();
            lblResumen = new Label();
            panelAccionesLista = new Panel();
            btnEliminar = new Button();
            btnNuevo = new Button();
            panelFiltros = new Panel();
            cbFiltroEstado = new ComboBox();
            lblFiltroEstado = new Label();
            txtBuscar = new TextBox();
            lblBuscar = new Label();
            tabDetalle = new TabControl();
            tabPermisos = new TabPage();
            arbolPermisos = new Vista.Comun.ArbolPermisos();
            panelHerramientasPermisos = new Panel();
            lblConteoPermisos = new Label();
            btnDesmarcarTodo = new Button();
            btnMarcarTodo = new Button();
            txtFiltrarPermisos = new TextBox();
            tabUsuarios = new TabPage();
            dgvUsuarios = new DataGridView();
            panelAccionesFicha = new Panel();
            btnVolver = new Button();
            btnDescartar = new Button();
            btnGuardar = new Button();
            panelDatos = new Panel();
            lblAviso = new Label();
            txtDescripcion = new TextBox();
            lblDescripcion = new Label();
            cbEstado = new ComboBox();
            lblEstado = new Label();
            txtNombre = new TextBox();
            lblNombre = new Label();
            lblEncabezado = new Label();
            errorProvider = new ErrorProvider(components);
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvGrupos).BeginInit();
            panelAccionesLista.SuspendLayout();
            panelFiltros.SuspendLayout();
            tabDetalle.SuspendLayout();
            tabPermisos.SuspendLayout();
            panelHerramientasPermisos.SuspendLayout();
            tabUsuarios.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUsuarios).BeginInit();
            panelAccionesFicha.SuspendLayout();
            panelDatos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
            SuspendLayout();
            //
            // splitContainer
            //
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.FixedPanel = FixedPanel.Panel1;
            splitContainer.Location = new Point(0, 0);
            splitContainer.Name = "splitContainer";
            //
            // splitContainer.Panel1
            //
            splitContainer.Panel1.Controls.Add(dgvGrupos);
            splitContainer.Panel1.Controls.Add(lblResumen);
            splitContainer.Panel1.Controls.Add(panelAccionesLista);
            splitContainer.Panel1.Controls.Add(panelFiltros);
            splitContainer.Panel1MinSize = 300;
            //
            // splitContainer.Panel2
            //
            splitContainer.Panel2.Controls.Add(tabDetalle);
            splitContainer.Panel2.Controls.Add(panelAccionesFicha);
            splitContainer.Panel2.Controls.Add(panelDatos);
            splitContainer.Panel2MinSize = 520;
            splitContainer.Size = new Size(1060, 732);
            splitContainer.SplitterDistance = 380;
            splitContainer.SplitterWidth = 6;
            splitContainer.TabIndex = 0;
            //
            // dgvGrupos
            //
            dgvGrupos.BackgroundColor = SystemColors.Control;
            dgvGrupos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvGrupos.Dock = DockStyle.Fill;
            dgvGrupos.Location = new Point(0, 104);
            dgvGrupos.Name = "dgvGrupos";
            dgvGrupos.RowHeadersWidth = 51;
            dgvGrupos.Size = new Size(380, 536);
            dgvGrupos.TabIndex = 1;
            //
            // lblResumen
            //
            lblResumen.Dock = DockStyle.Bottom;
            lblResumen.Location = new Point(0, 640);
            lblResumen.Name = "lblResumen";
            lblResumen.Padding = new Padding(8, 0, 0, 0);
            lblResumen.Size = new Size(380, 28);
            lblResumen.TabIndex = 2;
            lblResumen.Tag = "BLANCO";
            lblResumen.TextAlign = ContentAlignment.MiddleLeft;
            //
            // panelAccionesLista
            //
            panelAccionesLista.Controls.Add(btnEliminar);
            panelAccionesLista.Controls.Add(btnNuevo);
            panelAccionesLista.Dock = DockStyle.Bottom;
            panelAccionesLista.Location = new Point(0, 668);
            panelAccionesLista.Name = "panelAccionesLista";
            panelAccionesLista.Size = new Size(380, 64);
            panelAccionesLista.TabIndex = 3;
            //
            // btnEliminar
            //
            btnEliminar.Location = new Point(140, 8);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(120, 47);
            btnEliminar.TabIndex = 1;
            btnEliminar.Text = "Eliminar";
            btnEliminar.UseVisualStyleBackColor = true;
            //
            // btnNuevo
            //
            btnNuevo.Location = new Point(12, 8);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(120, 47);
            btnNuevo.TabIndex = 0;
            btnNuevo.Text = "Nuevo (F2)";
            btnNuevo.UseVisualStyleBackColor = true;
            //
            // panelFiltros
            //
            panelFiltros.Controls.Add(cbFiltroEstado);
            panelFiltros.Controls.Add(lblFiltroEstado);
            panelFiltros.Controls.Add(txtBuscar);
            panelFiltros.Controls.Add(lblBuscar);
            panelFiltros.Dock = DockStyle.Top;
            panelFiltros.Location = new Point(0, 0);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new Size(380, 104);
            panelFiltros.TabIndex = 0;
            //
            // cbFiltroEstado
            //
            cbFiltroEstado.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbFiltroEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cbFiltroEstado.FormattingEnabled = true;
            cbFiltroEstado.Location = new Point(80, 66);
            cbFiltroEstado.Name = "cbFiltroEstado";
            cbFiltroEstado.Size = new Size(286, 28);
            cbFiltroEstado.TabIndex = 3;
            //
            // lblFiltroEstado
            //
            lblFiltroEstado.AutoSize = true;
            lblFiltroEstado.Location = new Point(12, 70);
            lblFiltroEstado.Name = "lblFiltroEstado";
            lblFiltroEstado.Size = new Size(57, 20);
            lblFiltroEstado.TabIndex = 2;
            lblFiltroEstado.Tag = "BLANCO";
            lblFiltroEstado.Text = "Estado:";
            //
            // txtBuscar
            //
            txtBuscar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtBuscar.Location = new Point(12, 32);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.PlaceholderText = "Nombre o descripción (Ctrl+F)";
            txtBuscar.Size = new Size(354, 27);
            txtBuscar.TabIndex = 1;
            //
            // lblBuscar
            //
            lblBuscar.AutoSize = true;
            lblBuscar.Location = new Point(12, 9);
            lblBuscar.Name = "lblBuscar";
            lblBuscar.Size = new Size(55, 20);
            lblBuscar.TabIndex = 0;
            lblBuscar.Tag = "BLANCO";
            lblBuscar.Text = "Buscar:";
            //
            // tabDetalle
            //
            tabDetalle.Controls.Add(tabPermisos);
            tabDetalle.Controls.Add(tabUsuarios);
            tabDetalle.Dock = DockStyle.Fill;
            tabDetalle.Location = new Point(0, 190);
            tabDetalle.Name = "tabDetalle";
            tabDetalle.SelectedIndex = 0;
            tabDetalle.Size = new Size(674, 478);
            tabDetalle.TabIndex = 1;
            //
            // tabPermisos
            //
            tabPermisos.Controls.Add(arbolPermisos);
            tabPermisos.Controls.Add(panelHerramientasPermisos);
            tabPermisos.Location = new Point(4, 29);
            tabPermisos.Name = "tabPermisos";
            tabPermisos.Padding = new Padding(3);
            tabPermisos.Size = new Size(666, 445);
            tabPermisos.TabIndex = 0;
            tabPermisos.Text = "Permisos";
            tabPermisos.UseVisualStyleBackColor = true;
            //
            // arbolPermisos
            //
            arbolPermisos.CheckBoxes = true;
            arbolPermisos.Dock = DockStyle.Fill;
            arbolPermisos.HideSelection = false;
            arbolPermisos.Location = new Point(3, 51);
            arbolPermisos.Name = "arbolPermisos";
            arbolPermisos.Size = new Size(660, 391);
            arbolPermisos.TabIndex = 1;
            //
            // panelHerramientasPermisos
            //
            panelHerramientasPermisos.Controls.Add(lblConteoPermisos);
            panelHerramientasPermisos.Controls.Add(btnDesmarcarTodo);
            panelHerramientasPermisos.Controls.Add(btnMarcarTodo);
            panelHerramientasPermisos.Controls.Add(txtFiltrarPermisos);
            panelHerramientasPermisos.Dock = DockStyle.Top;
            panelHerramientasPermisos.Location = new Point(3, 3);
            panelHerramientasPermisos.Name = "panelHerramientasPermisos";
            panelHerramientasPermisos.Size = new Size(660, 48);
            panelHerramientasPermisos.TabIndex = 0;
            //
            // lblConteoPermisos
            //
            lblConteoPermisos.AutoSize = true;
            lblConteoPermisos.Location = new Point(522, 14);
            lblConteoPermisos.Name = "lblConteoPermisos";
            lblConteoPermisos.Size = new Size(0, 20);
            lblConteoPermisos.TabIndex = 3;
            lblConteoPermisos.Tag = "BLANCO";
            //
            // btnDesmarcarTodo
            //
            btnDesmarcarTodo.Location = new Point(394, 6);
            btnDesmarcarTodo.Name = "btnDesmarcarTodo";
            btnDesmarcarTodo.Size = new Size(120, 36);
            btnDesmarcarTodo.TabIndex = 2;
            btnDesmarcarTodo.Text = "Desmarcar todo";
            btnDesmarcarTodo.UseVisualStyleBackColor = true;
            //
            // btnMarcarTodo
            //
            btnMarcarTodo.Location = new Point(276, 6);
            btnMarcarTodo.Name = "btnMarcarTodo";
            btnMarcarTodo.Size = new Size(112, 36);
            btnMarcarTodo.TabIndex = 1;
            btnMarcarTodo.Text = "Marcar todo";
            btnMarcarTodo.UseVisualStyleBackColor = true;
            //
            // txtFiltrarPermisos
            //
            txtFiltrarPermisos.Location = new Point(8, 10);
            txtFiltrarPermisos.Name = "txtFiltrarPermisos";
            txtFiltrarPermisos.PlaceholderText = "Filtrar permisos...";
            txtFiltrarPermisos.Size = new Size(260, 27);
            txtFiltrarPermisos.TabIndex = 0;
            //
            // tabUsuarios
            //
            tabUsuarios.Controls.Add(dgvUsuarios);
            tabUsuarios.Location = new Point(4, 29);
            tabUsuarios.Name = "tabUsuarios";
            tabUsuarios.Padding = new Padding(3);
            tabUsuarios.Size = new Size(666, 445);
            tabUsuarios.TabIndex = 1;
            tabUsuarios.Text = "Usuarios";
            tabUsuarios.UseVisualStyleBackColor = true;
            //
            // dgvUsuarios
            //
            dgvUsuarios.BackgroundColor = SystemColors.Control;
            dgvUsuarios.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvUsuarios.Dock = DockStyle.Fill;
            dgvUsuarios.Location = new Point(3, 3);
            dgvUsuarios.Name = "dgvUsuarios";
            dgvUsuarios.RowHeadersWidth = 51;
            dgvUsuarios.Size = new Size(660, 439);
            dgvUsuarios.TabIndex = 0;
            //
            // panelAccionesFicha
            //
            panelAccionesFicha.Controls.Add(btnVolver);
            panelAccionesFicha.Controls.Add(btnDescartar);
            panelAccionesFicha.Controls.Add(btnGuardar);
            panelAccionesFicha.Dock = DockStyle.Bottom;
            panelAccionesFicha.Location = new Point(0, 668);
            panelAccionesFicha.Name = "panelAccionesFicha";
            panelAccionesFicha.Size = new Size(674, 64);
            panelAccionesFicha.TabIndex = 2;
            //
            // btnVolver
            //
            btnVolver.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnVolver.Location = new Point(542, 8);
            btnVolver.Name = "btnVolver";
            btnVolver.Size = new Size(120, 47);
            btnVolver.TabIndex = 2;
            btnVolver.Text = "Volver";
            btnVolver.UseVisualStyleBackColor = true;
            //
            // btnDescartar
            //
            btnDescartar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDescartar.Location = new Point(414, 8);
            btnDescartar.Name = "btnDescartar";
            btnDescartar.Size = new Size(120, 47);
            btnDescartar.TabIndex = 1;
            btnDescartar.Text = "Descartar (Esc)";
            btnDescartar.UseVisualStyleBackColor = true;
            //
            // btnGuardar
            //
            btnGuardar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGuardar.Location = new Point(286, 8);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(120, 47);
            btnGuardar.TabIndex = 0;
            btnGuardar.Text = "Guardar (Ctrl+S)";
            btnGuardar.UseVisualStyleBackColor = true;
            //
            // panelDatos
            //
            panelDatos.Controls.Add(lblAviso);
            panelDatos.Controls.Add(txtDescripcion);
            panelDatos.Controls.Add(lblDescripcion);
            panelDatos.Controls.Add(cbEstado);
            panelDatos.Controls.Add(lblEstado);
            panelDatos.Controls.Add(txtNombre);
            panelDatos.Controls.Add(lblNombre);
            panelDatos.Controls.Add(lblEncabezado);
            panelDatos.Dock = DockStyle.Top;
            panelDatos.Location = new Point(0, 0);
            panelDatos.Name = "panelDatos";
            panelDatos.Size = new Size(674, 190);
            panelDatos.TabIndex = 0;
            //
            // lblAviso
            //
            lblAviso.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblAviso.Location = new Point(16, 122);
            lblAviso.Name = "lblAviso";
            lblAviso.Size = new Size(644, 62);
            lblAviso.TabIndex = 7;
            lblAviso.Tag = "BLANCO";
            //
            // txtDescripcion
            //
            txtDescripcion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDescripcion.Location = new Point(120, 84);
            txtDescripcion.MaxLength = 60;
            txtDescripcion.Name = "txtDescripcion";
            txtDescripcion.Size = new Size(518, 27);
            txtDescripcion.TabIndex = 6;
            //
            // lblDescripcion
            //
            lblDescripcion.AutoSize = true;
            lblDescripcion.Location = new Point(16, 88);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Size = new Size(90, 20);
            lblDescripcion.TabIndex = 5;
            lblDescripcion.Tag = "BLANCO";
            lblDescripcion.Text = "Descripción:";
            //
            // cbEstado
            //
            cbEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEstado.FormattingEnabled = true;
            cbEstado.Location = new Point(496, 46);
            cbEstado.Name = "cbEstado";
            cbEstado.Size = new Size(142, 28);
            cbEstado.TabIndex = 4;
            //
            // lblEstado
            //
            lblEstado.AutoSize = true;
            lblEstado.Location = new Point(432, 50);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(57, 20);
            lblEstado.TabIndex = 3;
            lblEstado.Tag = "BLANCO";
            lblEstado.Text = "Estado:";
            //
            // txtNombre
            //
            txtNombre.Location = new Point(120, 46);
            txtNombre.MaxLength = 60;
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(290, 27);
            txtNombre.TabIndex = 2;
            //
            // lblNombre
            //
            lblNombre.AutoSize = true;
            lblNombre.Location = new Point(16, 50);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(67, 20);
            lblNombre.TabIndex = 1;
            lblNombre.Tag = "BLANCO";
            lblNombre.Text = "Nombre:";
            //
            // lblEncabezado
            //
            lblEncabezado.AutoSize = true;
            lblEncabezado.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblEncabezado.Location = new Point(16, 10);
            lblEncabezado.Name = "lblEncabezado";
            lblEncabezado.Size = new Size(0, 28);
            lblEncabezado.TabIndex = 0;
            lblEncabezado.Tag = "BLANCO";
            //
            // errorProvider
            //
            errorProvider.ContainerControl = this;
            //
            // FrmGestionarGrupos
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1060, 732);
            ControlBox = false;
            Controls.Add(splitContainer);
            KeyPreview = true;
            Name = "FrmGestionarGrupos";
            Text = "GRUPOS";
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvGrupos).EndInit();
            panelAccionesLista.ResumeLayout(false);
            panelFiltros.ResumeLayout(false);
            panelFiltros.PerformLayout();
            tabDetalle.ResumeLayout(false);
            tabPermisos.ResumeLayout(false);
            panelHerramientasPermisos.ResumeLayout(false);
            panelHerramientasPermisos.PerformLayout();
            tabUsuarios.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvUsuarios).EndInit();
            panelAccionesFicha.ResumeLayout(false);
            panelDatos.ResumeLayout(false);
            panelDatos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer;
        private Panel panelFiltros;
        private Label lblBuscar;
        private TextBox txtBuscar;
        private Label lblFiltroEstado;
        private ComboBox cbFiltroEstado;
        private DataGridView dgvGrupos;
        private Label lblResumen;
        private Panel panelAccionesLista;
        private Button btnNuevo;
        private Button btnEliminar;
        private Panel panelDatos;
        private Label lblEncabezado;
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblEstado;
        private ComboBox cbEstado;
        private Label lblDescripcion;
        private TextBox txtDescripcion;
        private Label lblAviso;
        private TabControl tabDetalle;
        private TabPage tabPermisos;
        private Panel panelHerramientasPermisos;
        private TextBox txtFiltrarPermisos;
        private Button btnMarcarTodo;
        private Button btnDesmarcarTodo;
        private Label lblConteoPermisos;
        private Vista.Comun.ArbolPermisos arbolPermisos;
        private TabPage tabUsuarios;
        private DataGridView dgvUsuarios;
        private Panel panelAccionesFicha;
        private Button btnGuardar;
        private Button btnDescartar;
        private Button btnVolver;
        private ErrorProvider errorProvider;
    }
}
