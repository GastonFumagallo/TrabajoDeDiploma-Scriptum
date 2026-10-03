using Controladora.Abm;
using Modelo;
using Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using Vista.Comun;

namespace Vista
{
    /// <summary>
    /// Modal único de alta y edición de libros: <c>new FrmEditarLibro(null)</c> = alta, <c>new FrmEditarLibro(id)</c> = edición.
    ///
    /// Validación en dos capas: acá (ErrorProvider, feedback inmediato) y en <see cref="LibroService"/> (la definitiva).
    /// Los errores que devuelve el servicio se marcan en el mismo control gracias al mapa campo → control.
    /// Si se guardó, devuelve <see cref="DialogResult.OK"/> y el id en <see cref="LibroId"/>.
    /// </summary>
    public partial class FrmEditarLibro : Form
    {
        private readonly LibroService servicio = LibroService.Instancia;
        private readonly ValidadorFormulario validador;
        private readonly BindingList<ProveedorPrecioDTO> proveedores = new();
        private readonly CancellationTokenSource cts = new();
        private readonly DataGridViewButtonColumn colQuitar = new() { Name = "Quitar", HeaderText = "", Text = "Quitar", UseColumnTextForButtonValue = true, FillWeight = 40 };

        private int? libroId;
        private int version;
        private bool cargando = true;
        private bool modificado;
        private bool guardando;
        private bool reactivado;   // se reactivó un libro inactivo: el listado debe refrescarse aunque no se guarde

        /// <summary>Id del libro guardado (o reactivado). Válido cuando el diálogo devuelve OK.</summary>
        public int? LibroId => libroId;

        private bool EsAlta => libroId == null;

        public FrmEditarLibro(int? libroId)
        {
            InitializeComponent();
            this.libroId = libroId;

            validador = new ValidadorFormulario(errorProvider)
                .Mapear(nameof(LibroEdicionDTO.ISBN), txtIsbn)
                .Mapear(nameof(LibroEdicionDTO.Titulo), txtTitulo)
                .Mapear(nameof(LibroEdicionDTO.Autor), txtAutor)
                .Mapear(nameof(LibroEdicionDTO.Editorial), txtEditorial)
                .Mapear(nameof(LibroEdicionDTO.GeneroId), cbGenero)
                .Mapear(nameof(LibroEdicionDTO.AnioPublicacion), numAnio)
                .Mapear(nameof(LibroEdicionDTO.PrecioCosto), numCosto)
                .Mapear(nameof(LibroEdicionDTO.PrecioVenta), numVenta)
                .Mapear(nameof(LibroEdicionDTO.Stock), numStock)
                .Mapear(nameof(LibroEdicionDTO.StockMinimo), numMinimo)
                .Mapear(nameof(LibroEdicionDTO.PuntoReposicion), numPunto)
                .Mapear(nameof(LibroEdicionDTO.StockOptimo), numOptimo)
                .Mapear(nameof(LibroEdicionDTO.Proveedores), dgvProveedores);

            ConfigurarProveedores();
            ConfigurarEventos();
            FormClosing += FrmEditarLibro_FormClosing;
            Disposed += (_, _) => cts.Dispose();
        }

        #region Carga

        private async void FrmEditarLibro_Load(object sender, EventArgs e)
        {
            numAnio.Maximum = DateTime.Today.Year + 1;
            try
            {
                UseWaitCursor = true;
                Enabled = false;
                var generosTask = servicio.ObtenerGenerosAsync(cts.Token);
                var proveedoresTask = servicio.ObtenerProveedoresAsync(cts.Token);
                await Task.WhenAll(generosTask, proveedoresTask);
                CargarCombo(cbGenero, generosTask.Result);
                CargarCombo(cbProveedor, proveedoresTask.Result);

                if (libroId is int id)
                {
                    var dto = await servicio.ObtenerPorIdAsync(id, cts.Token);
                    if (dto == null)
                    {
                        MessageBox.Show(this, "El libro ya no existe.", "Libro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        DialogResult = DialogResult.Cancel;
                        return;
                    }
                    MostrarLibro(dto);
                }
                else
                {
                    MostrarLibro(new LibroEdicionDTO { GeneroId = (cbGenero.SelectedItem as OpcionDTO)?.Id ?? 0 });
                }
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo abrir la ficha del libro.");
                DialogResult = DialogResult.Cancel;
            }
            finally
            {
                Enabled = true;
                UseWaitCursor = false;
                cargando = false;
            }

            (EsAlta ? txtIsbn : txtTitulo).Focus();
        }

        private static void CargarCombo(ComboBox combo, List<OpcionDTO> opciones)
        {
            combo.DataSource = opciones;
            combo.DisplayMember = nameof(OpcionDTO.Nombre);
            combo.ValueMember = nameof(OpcionDTO.Id);
        }

        /// <summary>Vuelca el DTO en los controles. Se usa al abrir y al reactivar un libro inactivo.</summary>
        private void MostrarLibro(LibroEdicionDTO dto)
        {
            cargando = true;
            libroId = dto.Id;
            version = dto.Version;

            txtIsbn.Text = dto.ISBN;
            txtTitulo.Text = dto.Titulo;
            txtAutor.Text = dto.Autor;
            txtEditorial.Text = dto.Editorial;
            txtDescripcion.Text = dto.Descripcion;
            numAnio.Value = Math.Clamp(dto.AnioPublicacion, (int)numAnio.Minimum, (int)numAnio.Maximum);
            if (dto.GeneroId > 0) cbGenero.SelectedValue = dto.GeneroId;
            numCosto.Value = Math.Min(numCosto.Maximum, dto.PrecioCosto);
            numVenta.Value = Math.Min(numVenta.Maximum, dto.PrecioVenta);
            numStock.Value = Math.Clamp(dto.Stock, 0, (int)numStock.Maximum);
            numMinimo.Value = dto.StockMinimo;
            numPunto.Value = dto.PuntoReposicion;
            numOptimo.Value = dto.StockOptimo;

            proveedores.Clear();
            foreach (var p in dto.Proveedores) proveedores.Add(p);

            // El stock sólo se carga en el alta; después se ajusta desde Inventario (queda auditado).
            numStock.Enabled = EsAlta;
            lblStockInfo.Text = EsAlta ? "Stock inicial del alta" : "Se ajusta desde Inventario";

            Text = EsAlta ? "Nuevo libro" : $"Editar libro: {dto.Titulo}";
            lblEstadoLibro.Text = dto.Activo
                ? "Los campos con * son obligatorios."
                : "Este libro está INACTIVO: no se vende ni se repone. Reactivalo desde el listado.";
            lblEstadoLibro.ForeColor = dto.Activo ? SystemColors.ControlText : Color.Firebrick;

            ActualizarMargen();
            validador.Limpiar();
            cargando = false;
            modificado = false;
        }

        #endregion

        #region Eventos y validación en vivo

        private void ConfigurarEventos()
        {
            btnGuardar.Click += async (_, _) => await GuardarAsync();
            btnNuevoGenero.Click += async (_, _) => await AltaGeneroAsync();
            btnAgregarProveedor.Click += (_, _) => AgregarProveedor();

            numCosto.ValueChanged += (_, _) => ActualizarMargen();
            numVenta.ValueChanged += (_, _) => ActualizarMargen();

            // Cualquier cambio marca la ficha como modificada (para avisar al cerrar sin guardar).
            foreach (Control c in new Control[] { txtIsbn, txtTitulo, txtAutor, txtEditorial, txtDescripcion })
                c.TextChanged += (_, _) => MarcarModificado(c);
            foreach (var n in new[] { numAnio, numCosto, numVenta, numStock, numMinimo, numPunto, numOptimo })
                n.ValueChanged += (_, _) => MarcarModificado(n);
            cbGenero.SelectedIndexChanged += (_, _) => MarcarModificado(cbGenero);

            // ISBN: formato al salir del campo y unicidad contra la base (async, no bloquea).
            txtIsbn.Leave += async (_, _) => await ValidarIsbnAsync();
        }

        private void MarcarModificado(Control control)
        {
            if (cargando) return;
            modificado = true;
            validador.Limpiar(control);   // el usuario está corrigiendo: se saca la marca de error
        }

        private void ActualizarMargen()
        {
            decimal costo = numCosto.Value, venta = numVenta.Value;
            if (costo <= 0 || venta <= 0)
            {
                lblMargen.Text = "Margen: —";
                lblMargen.ForeColor = SystemColors.ControlText;
                return;
            }
            decimal margen = (venta - costo) / venta * 100;
            lblMargen.Text = $"Margen\n{margen:N1} %\n(${venta - costo:N2})";
            lblMargen.ForeColor = margen > 0 ? Color.LightGreen : Color.LightCoral;
        }

        private async Task ValidarIsbnAsync()
        {
            validador.Limpiar(txtIsbn);
            string? isbn = Libro.NormalizarISBN(txtIsbn.Text);
            if (isbn == null)
            {
                lblIsbnInfo.Text = "Opcional. 10 o 13 dígitos (se aceptan guiones).";
                return;
            }
            if (!Identificadores.EsISBNValido(isbn))
            {
                validador.Marcar(txtIsbn, "El ISBN no es válido: revisá los dígitos (el último es verificador).");
                lblIsbnInfo.Text = "ISBN inválido.";
                return;
            }

            try
            {
                bool existe = await servicio.ExisteIdentificadorAsync(isbn, libroId, cts.Token);
                if (existe)
                {
                    validador.Marcar(txtIsbn, "Ya hay otro libro con este ISBN.");
                    lblIsbnInfo.Text = "Ya existe otro libro con este ISBN.";
                }
                else
                {
                    lblIsbnInfo.Text = $"ISBN-{isbn.Length} válido ✓";
                }
            }
            catch (Exception)
            {
                // La verificación en vivo es una ayuda: si falla, el servicio vuelve a validar al guardar.
            }
        }

        /// <summary>Validación de formulario (primera capa). Devuelve true si se puede llamar al servicio.</summary>
        private bool ValidarFormulario()
        {
            validador.Limpiar();
            string? isbn = Libro.NormalizarISBN(txtIsbn.Text);

            validador.Regla(txtIsbn, isbn != null && !Identificadores.EsISBNValido(isbn), "El ISBN no es válido.");
            validador.Requerido(txtTitulo, "El título");
            validador.Requerido(txtAutor, "El autor");
            validador.Requerido(txtEditorial, "La editorial");
            validador.Regla(cbGenero, cbGenero.SelectedItem == null, "Seleccione un género.");
            validador.Regla(numVenta, numVenta.Value <= 0, "El precio de venta debe ser mayor a 0.");
            validador.Regla(numVenta, numCosto.Value > 0 && numVenta.Value <= numCosto.Value,
                "El precio de venta debe ser mayor al de costo (margen positivo).");
            validador.Regla(numPunto, numPunto.Value < numMinimo.Value, "No puede ser menor que el stock mínimo.");
            validador.Regla(numOptimo, numOptimo.Value < numPunto.Value || numOptimo.Value <= 0,
                "Debe ser mayor a 0 y no menor que el punto de reposición.");

            validador.EnfocarPrimero();
            return validador.EsValido;
        }

        #endregion

        #region Proveedores

        private void ConfigurarProveedores()
        {
            GrillaHelper.ConfigurarListado(dgvProveedores);
            dgvProveedores.ReadOnly = false;
            dgvProveedores.SelectionMode = DataGridViewSelectionMode.CellSelect;
            GrillaHelper.Columna(dgvProveedores, nameof(ProveedorPrecioDTO.Proveedor), "Proveedor", peso: 200).ReadOnly = true;
            GrillaHelper.Columna(dgvProveedores, nameof(ProveedorPrecioDTO.PrecioCompra), "Precio compra", peso: 90, formato: "N2", derecha: true);
            dgvProveedores.Columns.Add(colQuitar);
            dgvProveedores.DataSource = proveedores;

            dgvProveedores.CellContentClick += (_, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == colQuitar.Index)
                {
                    proveedores.RemoveAt(e.RowIndex);
                    modificado = true;
                }
            };
            dgvProveedores.CellValidating += (_, e) =>
            {
                if (dgvProveedores.Columns[e.ColumnIndex].Name != nameof(ProveedorPrecioDTO.PrecioCompra)) return;
                bool valido = decimal.TryParse(Convert.ToString(e.FormattedValue), NumberStyles.Number, CultureInfo.CurrentCulture, out var precio) && precio > 0;
                dgvProveedores.Rows[e.RowIndex].ErrorText = valido ? string.Empty : "El precio debe ser mayor a 0.";
                e.Cancel = !valido;
            };
            dgvProveedores.CellEndEdit += (_, _) => modificado = true;
            dgvProveedores.DataError += (_, e) => e.Cancel = true;
        }

        private void AgregarProveedor()
        {
            validador.Limpiar(dgvProveedores);
            if (cbProveedor.SelectedItem is not OpcionDTO proveedor) return;
            if (numPrecioCompra.Value <= 0)
            {
                validador.Marcar(numPrecioCompra, "Indicá el precio de compra pactado.");
                numPrecioCompra.Focus();
                return;
            }
            validador.Limpiar(numPrecioCompra);

            var existente = proveedores.FirstOrDefault(p => p.ProveedorId == proveedor.Id);
            if (existente != null)
            {
                existente.PrecioCompra = numPrecioCompra.Value;   // ya estaba: se actualiza el precio
                proveedores.ResetItem(proveedores.IndexOf(existente));
            }
            else
            {
                proveedores.Add(new ProveedorPrecioDTO { ProveedorId = proveedor.Id, Proveedor = proveedor.Nombre, PrecioCompra = numPrecioCompra.Value });
            }

            // Si todavía no hay costo cargado, se propone el precio de compra.
            if (numCosto.Value == 0) numCosto.Value = numPrecioCompra.Value;
            numPrecioCompra.Value = 0;
            modificado = true;
        }

        private async Task AltaGeneroAsync()
        {
            var antes = (cbGenero.DataSource as List<OpcionDTO>)?.Select(g => g.Id).ToHashSet() ?? new();
            using (var frm = new FrmAgregarGenero())
                frm.ShowDialog(this);

            try
            {
                var generos = await servicio.ObtenerGenerosAsync(cts.Token);
                int? seleccionado = generos.Where(g => !antes.Contains(g.Id)).Select(g => (int?)g.Id).FirstOrDefault()
                                    ?? (cbGenero.SelectedItem as OpcionDTO)?.Id;
                CargarCombo(cbGenero, generos);
                if (seleccionado != null) cbGenero.SelectedValue = seleccionado;
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudieron recargar los géneros.");
            }
        }

        #endregion

        #region Guardar

        private LibroEdicionDTO ArmarDto() => new()
        {
            Id = libroId,
            Version = version,
            ISBN = txtIsbn.Text,
            Titulo = txtTitulo.Text,
            Autor = txtAutor.Text,
            Editorial = txtEditorial.Text,
            Descripcion = txtDescripcion.Text,
            AnioPublicacion = (int)numAnio.Value,
            GeneroId = (cbGenero.SelectedItem as OpcionDTO)?.Id ?? 0,
            PrecioCosto = numCosto.Value,
            PrecioVenta = numVenta.Value,
            Stock = (int)numStock.Value,
            StockMinimo = (int)numMinimo.Value,
            PuntoReposicion = (int)numPunto.Value,
            StockOptimo = (int)numOptimo.Value,
            Proveedores = proveedores.Select(p => new ProveedorPrecioDTO { ProveedorId = p.ProveedorId, Proveedor = p.Proveedor, PrecioCompra = p.PrecioCompra }).ToList(),
        };

        private async Task GuardarAsync()
        {
            if (guardando) return;
            if (dgvProveedores.IsCurrentCellInEditMode && !dgvProveedores.EndEdit()) return;
            if (!ValidarFormulario()) return;

            guardando = true;
            btnGuardar.Enabled = false;
            UseWaitCursor = true;
            try
            {
                string usuario = PermisoService.Instancia.UsuarioActual?.USU_Nombre ?? "desconocido";
                libroId = await servicio.GuardarAsync(ArmarDto(), usuario, cts.Token);
                modificado = false;
                DialogResult = DialogResult.OK;
            }
            catch (ValidacionException ex)
            {
                // Errores de negocio (ej. ISBN duplicado): se marcan en su control.
                string? resto = validador.MostrarErrores(ex);
                if (resto != null)
                    MessageBox.Show(this, resto, "Revise los datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (EntidadInactivaException ex)
            {
                await OfrecerReactivacionAsync(ex);
            }
            catch (ConcurrenciaException ex)
            {
                if (MessageBox.Show(this, ex.Message + "\n\n¿Recargar los datos actuales? (se perderán tus cambios)",
                        "Datos modificados por otro usuario", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes
                    && libroId is int id)
                {
                    var actual = await servicio.ObtenerPorIdAsync(id, cts.Token);
                    if (actual != null) MostrarLibro(actual);
                    else DialogResult = DialogResult.Cancel;
                }
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo guardar el libro.");
            }
            finally
            {
                guardando = false;
                btnGuardar.Enabled = true;
                UseWaitCursor = false;
            }
        }

        /// <summary>
        /// El ISBN pertenece a un libro dado de baja: en lugar de duplicarlo se ofrece reactivarlo
        /// y se abre su ficha para revisarla (los datos tipeados se descartan).
        /// </summary>
        private async Task OfrecerReactivacionAsync(EntidadInactivaException ex)
        {
            var respuesta = MessageBox.Show(this,
                $"{ex.Message}\n\n¿Reactivar ese libro en lugar de crear uno nuevo?\nSe abrirá su ficha para que revises los datos.",
                "Libro existente inactivo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (respuesta != DialogResult.Yes)
            {
                validador.Marcar(txtIsbn, "Este ISBN pertenece a un libro inactivo.");
                return;
            }

            try
            {
                var ficha = await servicio.ReactivarAsync(ex.EntidadId, cts.Token);
                if (ficha == null) return;
                MostrarLibro(ficha);
                this.reactivado = true;
                lblEstadoLibro.Text = "Libro reactivado. Revisá los datos y guardá si hace falta actualizar algo.";
                lblEstadoLibro.ForeColor = Color.DarkGreen;
            }
            catch (Exception error)
            {
                ManejadorErrores.Mostrar(this, error, "No se pudo reactivar el libro.");
            }
        }

        private void FrmEditarLibro_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (guardando)
            {
                e.Cancel = true;
                return;
            }

            if (DialogResult != DialogResult.OK && modificado &&
                MessageBox.Show(this, "Hay cambios sin guardar. ¿Descartarlos?", "Cambios sin guardar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            // Si se reactivó un libro, el listado tiene que refrescarse aunque no se haya vuelto a guardar.
            if (DialogResult != DialogResult.OK && reactivado)
                DialogResult = DialogResult.OK;

            cts.Cancel();
        }

        #endregion
    }
}
