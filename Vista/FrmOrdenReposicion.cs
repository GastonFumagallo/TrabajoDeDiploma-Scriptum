using Controladora;
using Modelo;
using Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Vista
{
    /// <summary>
    /// Orden de reposición (compra a proveedor): cabecera con número, fecha, proveedor y contacto,
    /// solicitante y estado; ítems con cantidad y costo editables; total y observaciones.
    ///
    /// Flujo: Pendiente (borrador editable) → Emitir (email al proveedor) → Solicitada → Registrar recepción → Recibida.
    /// La recepción es un modo de la misma pantalla: se cargan cantidad y costo realmente recibidos y
    /// <see cref="OrdenReposicionService.RecibirAsync"/> impacta stock, costos e historial en una transacción.
    /// </summary>
    public partial class FrmOrdenReposicion : Form
    {
        /// <summary>Ítem de la lista de sugerencias.</summary>
        private sealed record Sugerencia(ProductoInventarioDTO Libro, decimal Costo)
        {
            public override string ToString() =>
                $"{Libro.Titulo} — {Libro.Autor}   |   stock {Libro.Stock} (mín. {Libro.StockMinimo})   |   " +
                $"sugerido {Math.Max(1, Libro.CantidadSugerida)}   |   costo ${Costo:N2}";
        }

        private readonly int? ordenIdInicial;
        private readonly int? proveedorInicial;
        private readonly IReadOnlyList<int>? libroIdsIniciales;
        private readonly bool iniciarRecepcion;

        private readonly BindingList<ItemOrdenDTO> items = new();
        private readonly System.Windows.Forms.Timer timerBusqueda = new() { Interval = 150 };
        private readonly CancellationTokenSource cts = new();

        private List<ProductoInventarioDTO> catalogo = new();
        private List<ProveedorContactoDTO> proveedores = new();
        private Dictionary<int, decimal> preciosProveedor = new();
        private ProductoInventarioDTO? libroSeleccionado;

        private string estado = EstadoOrden.Pendiente;
        private bool modoRecepcion;
        private bool modificado;
        private bool ocupado;
        private bool cargando = true;

        /// <summary>Id de la orden (null mientras es una orden nueva sin guardar). Lo usa el inventario para re-seleccionarla.</summary>
        public int? OrdenId { get; private set; }

        /// <summary>Orden nueva y vacía.</summary>
        public FrmOrdenReposicion() : this(null, null, null, false) { }

        /// <summary>Orden nueva precargada con libros (desde Inventario), con cantidades sugeridas hasta el stock óptimo.</summary>
        public FrmOrdenReposicion(int? proveedorId, IReadOnlyList<int> libroIds) : this(null, proveedorId, libroIds, false) { }

        /// <summary>Abre una orden existente; con <paramref name="iniciarRecepcion"/> entra directo a cargar lo recibido.</summary>
        public FrmOrdenReposicion(int ordenId, bool iniciarRecepcion) : this(ordenId, null, null, iniciarRecepcion) { }

        private FrmOrdenReposicion(int? ordenId, int? proveedorId, IReadOnlyList<int>? libroIds, bool iniciarRecepcion)
        {
            InitializeComponent();
            ordenIdInicial = ordenId;
            proveedorInicial = proveedorId;
            libroIdsIniciales = libroIds;
            this.iniciarRecepcion = iniciarRecepcion;

            ConfigurarGrilla();
            ConfigurarBusqueda();
            ConfigurarCabeceraYPie();

            FormClosing += FrmOrdenReposicion_FormClosing;
            Disposed += (_, _) => { timerBusqueda.Dispose(); cts.Dispose(); };
        }

        private static string Usuario => PermisoService.Instancia.UsuarioActual?.USU_Nombre ?? "desconocido";

        private bool Editable => estado == EstadoOrden.Pendiente && !modoRecepcion;

        #region Carga

        private async void FrmOrdenReposicion_Load(object sender, EventArgs e)
        {
            try
            {
                UseWaitCursor = true;
                Enabled = false;

                var proveedoresTask = OrdenReposicionService.Instancia.ObtenerProveedoresAsync(incluirInactivos: true, cts.Token);
                var catalogoTask = InventarioService.Instancia.ObtenerInventarioAsync(new FiltroInventario(), cts.Token);
                await Task.WhenAll(proveedoresTask, catalogoTask);
                proveedores = proveedoresTask.Result;
                catalogo = catalogoTask.Result;
                cbProveedor.DataSource = proveedores;

                if (ordenIdInicial is int id)
                {
                    await CargarOrdenAsync(id);
                }
                else
                {
                    OrdenId = null;
                    estado = EstadoOrden.Pendiente;
                    lblFecha.Text = $"Fecha de emisión: {DateTime.Now:dd/MM/yyyy}";
                    lblUsuario.Text = $"Solicitante: {Usuario}";
                    cbProveedor.SelectedItem = proveedores.FirstOrDefault(p => p.ProveedorId == proveedorInicial);
                    if (cbProveedor.SelectedItem == null) cbProveedor.SelectedIndex = -1;
                    await CargarPreciosProveedorAsync();

                    if (libroIdsIniciales is { Count: > 0 })
                    {
                        var sugeridos = await OrdenReposicionService.Instancia.SugerirItemsAsync(
                            libroIdsIniciales, ProveedorSeleccionado?.ProveedorId, ct: cts.Token);
                        foreach (var item in sugeridos) items.Add(item);
                        modificado = items.Count > 0;
                    }
                }
            }
            catch (OperationCanceledException) { return; }
            catch (Exception ex)
            {
                MostrarError("No se pudo cargar la orden.", ex);
            }
            finally
            {
                Enabled = true;
                UseWaitCursor = false;
                cargando = false;
            }

            ActualizarContacto();
            AplicarModo();
            if (iniciarRecepcion && EstadoOrden.EsActiva(estado))
                await RecibirAsync();
            else if (Editable)
                txtBuscar.Focus();
        }

        private async Task CargarOrdenAsync(int ordenId)
        {
            var orden = await OrdenReposicionService.Instancia.ObtenerOrdenAsync(ordenId, cts.Token);
            if (orden == null)
            {
                MessageBox.Show("La orden ya no existe.", "Orden de reposición", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }

            OrdenId = orden.OrdenId;
            estado = orden.EstadoCodigo;
            modoRecepcion = false;
            cargando = true;
            cbProveedor.SelectedItem = proveedores.FirstOrDefault(p => p.ProveedorId == orden.ProveedorId);
            cargando = false;
            txtObservaciones.Text = orden.Observaciones;
            lblFecha.Text = $"Fecha de emisión: {orden.Fecha:dd/MM/yyyy HH:mm}";
            lblUsuario.Text = $"Solicitante: {orden.Usuario ?? "—"}";
            lblInfoEstado.Text = orden.EstadoCodigo switch
            {
                EstadoOrden.Recibida => $"Recibida el {orden.FechaRecepcion:dd/MM/yyyy HH:mm} por {orden.UsuarioRecepcion}.",
                EstadoOrden.Cancelada => $"Cancelada: {orden.MotivoCancelacion}",
                _ => string.Empty,
            };

            items.RaiseListChangedEvents = false;
            items.Clear();
            foreach (var item in orden.Items) items.Add(item);
            items.RaiseListChangedEvents = true;
            items.ResetBindings();

            await CargarPreciosProveedorAsync();
            modificado = false;
        }

        private async Task CargarPreciosProveedorAsync()
        {
            preciosProveedor = ProveedorSeleccionado is ProveedorContactoDTO p
                ? await OrdenReposicionService.Instancia.ObtenerPreciosProveedorAsync(p.ProveedorId, catalogo.Select(l => l.LibroId), cts.Token)
                : new();
        }

        #endregion

        #region Cabecera, modo y totales

        private ProveedorContactoDTO? ProveedorSeleccionado => cbProveedor.SelectedItem as ProveedorContactoDTO;

        private void ConfigurarCabeceraYPie()
        {
            cbProveedor.SelectionChangeCommitted += async (_, _) => await CambiarProveedorAsync();
            txtObservaciones.TextChanged += (_, _) => { if (!cargando && Editable) modificado = true; };
            chkSoloProveedor.CheckedChanged += (_, _) => MostrarSugerencias();

            btnGuardar.Click += async (_, _) => await GuardarAsync(mostrarMensaje: true);
            btnEmitir.Click += async (_, _) => await EmitirAsync();
            btnRecibir.Click += async (_, _) => await RecibirAsync();
            btnCerrar.Click += (_, _) => Close();
        }

        private void ActualizarContacto()
        {
            var p = ProveedorSeleccionado;
            lblContacto.Text = p == null
                ? "Seleccione el proveedor al que se le hace el pedido."
                : $"Contacto: {p.Contacto}   ·   Tel.: {(string.IsNullOrWhiteSpace(p.Telefono) ? "—" : p.Telefono)}   ·   " +
                  $"Email: {(string.IsNullOrWhiteSpace(p.Email) ? "— (no se podrá enviar por correo)" : p.Email)}";
        }

        private async Task CambiarProveedorAsync()
        {
            if (cargando) return;
            ActualizarContacto();
            modificado = true;

            try
            {
                await CargarPreciosProveedorAsync();
            }
            catch (Exception ex)
            {
                MostrarError("No se pudieron cargar los precios del proveedor.", ex);
                return;
            }

            // Ofrece actualizar los costos de los ítems ya cargados con los precios pactados con el nuevo proveedor.
            var conPrecio = items.Where(i => preciosProveedor.ContainsKey(i.LibroId) && preciosProveedor[i.LibroId] != i.CostoUnitario).ToList();
            if (conPrecio.Count > 0 &&
                MessageBox.Show($"El proveedor tiene precio pactado para {conPrecio.Count} ítem(s) de la orden.\n¿Actualizar sus costos?",
                    "Cambio de proveedor", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                foreach (var item in conPrecio) item.CostoUnitario = preciosProveedor[item.LibroId];
                items.ResetBindings();
            }
            Recalcular();
        }

        /// <summary>Habilita/oculta controles según el estado de la orden y si se está registrando la recepción.</summary>
        private void AplicarModo()
        {
            bool editable = Editable;
            bool activa = EstadoOrden.EsActiva(estado);
            bool mostrarRecibido = modoRecepcion || estado == EstadoOrden.Recibida;

            lblNumero.Text = OrdenId is int id
                ? $"Orden {OrdenReposicion.FormatearNumero(id)}" + (modoRecepcion ? " — Recepción de mercadería" : "")
                : "Nueva orden de reposición";
            lblEstadoOrden.Text = $"Estado: {EstadoOrden.Descripcion(estado)}";
            lblEstadoOrden.ForeColor = estado switch
            {
                EstadoOrden.Solicitada => Color.LightSkyBlue,
                EstadoOrden.Recibida => Color.LightGreen,
                EstadoOrden.Cancelada => Color.LightGray,
                _ => Color.Gold,
            };

            cbProveedor.Enabled = editable;
            panelCarga.Visible = editable;
            txtObservaciones.ReadOnly = !editable;

            colQuitar.Visible = editable;
            colCantidad.ReadOnly = colCosto.ReadOnly = !editable;
            colRecibido.Visible = colCostoRecibido.Visible = mostrarRecibido;
            colRecibido.ReadOnly = colCostoRecibido.ReadOnly = !modoRecepcion;
            chkActualizarCostos.Visible = modoRecepcion;

            btnGuardar.Visible = editable;
            btnEmitir.Visible = editable;
            btnRecibir.Visible = activa && PermisoService.Instancia.TienePermiso("RegistrarRecepcionOrden");
            btnRecibir.Text = modoRecepcion ? "Confirmar recepción" : "Registrar recepción";
            btnCerrar.Text = modoRecepcion ? "Cancelar" : "Cerrar";

            lblInfoEstado.Text = modoRecepcion
                ? "Cargue lo que llegó realmente. Lo que no se reciba queda registrado como diferencia y la orden se cierra."
                : lblInfoEstado.Text;

            dgvItems.Invalidate();
            Recalcular();
        }

        private void Recalcular()
        {
            if (modoRecepcion || estado == EstadoOrden.Recibida)
            {
                lblTotalTitulo.Text = "Total recibido";
                lblTotal.Text = $"${items.Sum(i => (i.CantidadRecibida ?? 0) * (i.CostoRecibido ?? i.CostoUnitario)):N2}";
            }
            else
            {
                lblTotalTitulo.Text = $"Total estimado ({items.Sum(i => i.CantidadPedida)} u.)";
                lblTotal.Text = $"${items.Sum(i => i.Subtotal):N2}";
            }
        }

        #endregion

        #region Ítems (grilla)

        private void ConfigurarGrilla()
        {
            typeof(DataGridView).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(dgvItems, true);
            dgvItems.AutoGenerateColumns = false;
            dgvItems.DataSource = items;
            dgvItems.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;

            foreach (var col in new DataGridViewColumn[] { colCosto, colSubtotal, colCostoRecibido })
                col.DefaultCellStyle.Format = "N2";
            foreach (var col in new DataGridViewColumn[] { colStock, colOptimo, colCantidad, colCosto, colSubtotal, colRecibido, colCostoRecibido })
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvItems.CellValidating += dgvItems_CellValidating;
            dgvItems.CellEndEdit += (_, e) =>
            {
                dgvItems.Rows[e.RowIndex].ErrorText = string.Empty;
                items.ResetItem(e.RowIndex);
                if (Editable) modificado = true;
                Recalcular();
            };
            dgvItems.DataError += (_, e) => e.Cancel = true;
            dgvItems.CellContentClick += (_, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == colQuitar.Index && Editable && !ocupado)
                    QuitarItem(e.RowIndex);
            };
            dgvItems.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Delete && Editable && !dgvItems.IsCurrentCellInEditMode && dgvItems.CurrentRow != null)
                {
                    QuitarItem(dgvItems.CurrentRow.Index);
                    e.Handled = true;
                }
            };

            // Resalta las celdas editables y, en recepción, las diferencias contra lo pedido.
            dgvItems.CellFormatting += (_, e) =>
            {
                if (e.RowIndex < 0 || e.CellStyle == null || dgvItems.Rows[e.RowIndex].DataBoundItem is not ItemOrdenDTO item) return;
                var columna = dgvItems.Columns[e.ColumnIndex];
                bool editableCelda = !columna.ReadOnly && columna != colQuitar && !dgvItems.ReadOnly;
                if (editableCelda)
                    e.CellStyle.BackColor = Color.FromArgb(255, 252, 225);
                if (columna == colRecibido && item.CantidadRecibida is int recibido && recibido != item.CantidadPedida)
                    e.CellStyle.ForeColor = Color.Firebrick;
            };
        }

        private void QuitarItem(int indice)
        {
            if (indice < 0 || indice >= items.Count) return;
            items.RemoveAt(indice);
            modificado = true;
            Recalcular();
        }

        private void dgvItems_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
        {
            var columna = dgvItems.Columns[e.ColumnIndex];
            if (columna.ReadOnly) return;
            string texto = Convert.ToString(e.FormattedValue) ?? string.Empty;
            string? error = null;

            if (columna == colCantidad)
                error = int.TryParse(texto, out int n) && n > 0 ? null : "La cantidad debe ser un entero mayor a 0.";
            else if (columna == colRecibido)
                error = int.TryParse(texto, out int n) && n >= 0 ? null : "La cantidad recibida debe ser un entero mayor o igual a 0.";
            else if (columna == colCosto || columna == colCostoRecibido)
                error = decimal.TryParse(texto, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal d) && d >= 0
                    ? null : "El costo debe ser un número mayor o igual a 0.";

            if (error != null)
            {
                dgvItems.Rows[e.RowIndex].ErrorText = error;
                e.Cancel = true;
            }
        }

        #endregion

        #region Búsqueda y carga de libros

        private void ConfigurarBusqueda()
        {
            timerBusqueda.Tick += (_, _) => { timerBusqueda.Stop(); MostrarSugerencias(); };
            txtBuscar.TextChanged += (_, _) => { timerBusqueda.Stop(); timerBusqueda.Start(); };
            txtBuscar.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    ResolverBusqueda();
                }
                else if (e.KeyCode == Keys.Down && lstSugerencias.Visible)
                {
                    e.SuppressKeyPress = true;
                    lstSugerencias.Focus();
                    lstSugerencias.SelectedIndex = Math.Max(0, lstSugerencias.SelectedIndex);
                }
                else if (e.KeyCode == Keys.Escape && lstSugerencias.Visible)
                {
                    e.SuppressKeyPress = true;
                    lstSugerencias.Visible = false;
                }
            };
            txtBuscar.Leave += (_, _) => BeginInvoke(() => { if (!lstSugerencias.Focused) lstSugerencias.Visible = false; });

            lstSugerencias.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; ElegirSugerencia(); }
                else if (e.KeyCode == Keys.Escape) { e.SuppressKeyPress = true; lstSugerencias.Visible = false; txtBuscar.Focus(); }
                else if (e.KeyCode == Keys.Up && lstSugerencias.SelectedIndex <= 0) { e.SuppressKeyPress = true; txtBuscar.Focus(); }
            };
            lstSugerencias.MouseClick += (_, _) => ElegirSugerencia();
            lstSugerencias.Leave += (_, _) => BeginInvoke(() => { if (!txtBuscar.Focused) lstSugerencias.Visible = false; });

            numCantidad.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; numCosto.Focus(); numCosto.Select(0, numCosto.Text.Length); } };
            numCosto.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; AgregarSeleccionado(); } };
            btnAgregar.Click += (_, _) => AgregarSeleccionado();
        }

        private decimal CostoSugerido(ProductoInventarioDTO libro) =>
            preciosProveedor.TryGetValue(libro.LibroId, out var precio) ? precio : libro.PrecioCosto;

        private IEnumerable<ProductoInventarioDTO> Buscar(string texto)
        {
            var isbn = Libro.NormalizarISBN(texto);
            return catalogo
                .Where(l => !chkSoloProveedor.Checked || preciosProveedor.ContainsKey(l.LibroId))
                .Where(l => Contiene(l.Titulo) || Contiene(l.Autor) || (isbn != null && (l.Codigo?.Contains(isbn) ?? false)))
                .OrderBy(l => l.Estado == EstadoStock.Normal)   // primero los que necesitan reposición
                .ThenBy(l => l.Titulo);

            bool Contiene(string? campo) => campo?.Contains(texto, StringComparison.OrdinalIgnoreCase) ?? false;
        }

        private void MostrarSugerencias()
        {
            string texto = txtBuscar.Text.Trim();
            if (texto.Length < 2) { lstSugerencias.Visible = false; return; }

            var sugerencias = Buscar(texto).Take(30).Select(l => new Sugerencia(l, CostoSugerido(l))).ToArray();
            if (sugerencias.Length == 0) { lstSugerencias.Visible = false; return; }

            lstSugerencias.BeginUpdate();
            lstSugerencias.Items.Clear();
            lstSugerencias.Items.AddRange(sugerencias);
            lstSugerencias.EndUpdate();

            var origen = PointToClient(txtBuscar.Parent!.PointToScreen(txtBuscar.Location));
            lstSugerencias.Location = new Point(origen.X, origen.Y + txtBuscar.Height + 2);
            lstSugerencias.Width = 900;
            lstSugerencias.Height = Math.Min(240, lstSugerencias.ItemHeight * sugerencias.Length + 6);
            lstSugerencias.Visible = true;
            lstSugerencias.BringToFront();
        }

        /// <summary>Enter en el buscador: ISBN exacto o resultado único se selecciona directo; si no, se muestran opciones.</summary>
        private void ResolverBusqueda()
        {
            string texto = txtBuscar.Text.Trim();
            if (texto.Length == 0) return;

            if (lstSugerencias.Visible && lstSugerencias.SelectedItem is Sugerencia elegida)
            {
                SeleccionarLibro(elegida.Libro);
                return;
            }

            var isbn = Libro.NormalizarISBN(texto);
            var exacto = isbn is { Length: >= 8 } ? catalogo.FirstOrDefault(l => Libro.NormalizarISBN(l.Codigo) == isbn) : null;
            var coincidencias = exacto != null ? new List<ProductoInventarioDTO> { exacto } : Buscar(texto).Take(2).ToList();

            if (coincidencias.Count == 1)
                SeleccionarLibro(coincidencias[0]);
            else if (coincidencias.Count == 0)
                lblSeleccion.Text = $"No se encontró ningún libro para \"{texto}\".";
            else
            {
                MostrarSugerencias();
                lstSugerencias.Focus();
                lstSugerencias.SelectedIndex = 0;
            }
        }

        private void ElegirSugerencia()
        {
            if (lstSugerencias.SelectedItem is Sugerencia s)
                SeleccionarLibro(s.Libro);
        }

        private void SeleccionarLibro(ProductoInventarioDTO libro)
        {
            lstSugerencias.Visible = false;
            libroSeleccionado = libro;
            numCantidad.Value = Math.Clamp(Math.Max(1, libro.CantidadSugerida), numCantidad.Minimum, numCantidad.Maximum);
            numCosto.Value = Math.Clamp(CostoSugerido(libro), numCosto.Minimum, numCosto.Maximum);

            string yaEnOrden = items.Any(i => i.LibroId == libro.LibroId) ? "   ·   ya está en esta orden (se sumará)" : "";
            string enPedido = libro.EnPedido > 0 ? $"   ·   {libro.EnPedido} u. ya pedidas en otras órdenes" : "";
            lblSeleccion.Text = $"{libro.Titulo}   ·   stock {libro.Stock} / mínimo {libro.StockMinimo} / óptimo {libro.StockOptimo}{enPedido}{yaEnOrden}";

            numCantidad.Focus();
            numCantidad.Select(0, numCantidad.Text.Length);
        }

        private void AgregarSeleccionado()
        {
            if (!Editable || libroSeleccionado is not ProductoInventarioDTO libro)
            {
                lblSeleccion.Text = "Busque y seleccione un libro primero.";
                txtBuscar.Focus();
                return;
            }

            var existente = items.FirstOrDefault(i => i.LibroId == libro.LibroId);
            if (existente != null)
            {
                existente.CantidadPedida += (int)numCantidad.Value;
                existente.CostoUnitario = numCosto.Value;
                items.ResetItem(items.IndexOf(existente));
            }
            else
            {
                items.Add(new ItemOrdenDTO
                {
                    LibroId = libro.LibroId,
                    Codigo = libro.Codigo,
                    Titulo = libro.Titulo,
                    StockActual = libro.Stock,
                    StockOptimo = libro.StockOptimo,
                    CantidadPedida = (int)numCantidad.Value,
                    CostoUnitario = numCosto.Value,
                });
            }

            modificado = true;
            libroSeleccionado = null;
            lblSeleccion.Text = $"Agregado: {(int)numCantidad.Value} × {libro.Titulo}";
            txtBuscar.Clear();
            txtBuscar.Focus();
            Recalcular();
        }

        #endregion

        #region Guardar, emitir, recibir

        private bool ValidarOrden()
        {
            if (dgvItems.IsCurrentCellInEditMode && !dgvItems.EndEdit()) return false;
            if (ProveedorSeleccionado == null)
            {
                MessageBox.Show("Seleccione un proveedor.", "Orden incompleta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbProveedor.Focus();
                return false;
            }
            if (items.Count == 0)
            {
                MessageBox.Show("Agregue al menos un libro a la orden.", "Orden incompleta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBuscar.Focus();
                return false;
            }
            return true;
        }

        private async Task<bool> GuardarAsync(bool mostrarMensaje)
        {
            if (ocupado || !Editable || !ValidarOrden()) return false;

            var solicitud = new GuardarOrdenSolicitud
            {
                OrdenId = OrdenId,
                ProveedorId = ProveedorSeleccionado!.ProveedorId,
                Observaciones = txtObservaciones.Text,
                Usuario = Usuario,
                Items = items.Select(i => (i.LibroId, i.CantidadPedida, i.CostoUnitario)).ToList(),
            };

            bool ok = false;
            await EjecutarAsync(async () =>
            {
                var r = await OrdenReposicionService.Instancia.GuardarAsync(solicitud, cts.Token);
                if (!r.Exito)
                {
                    MessageBox.Show(r.Mensaje, "No se pudo guardar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                await CargarOrdenAsync(r.Id);
                AplicarModo();
                ok = true;
                if (mostrarMensaje)
                    MessageBox.Show($"Orden {OrdenReposicion.FormatearNumero(r.Id)} guardada como borrador.", "Orden guardada",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
            }, "No se pudo guardar la orden.");
            return ok;
        }

        private async Task EmitirAsync()
        {
            if (ocupado || !Editable) return;
            if ((modificado || OrdenId == null) && !await GuardarAsync(mostrarMensaje: false)) return;
            if (OrdenId is not int ordenId) return;

            var proveedor = ProveedorSeleccionado!;
            bool tieneEmail = !string.IsNullOrWhiteSpace(proveedor.Email);
            var respuesta = MessageBox.Show(
                tieneEmail
                    ? $"Se va a emitir la orden {OrdenReposicion.FormatearNumero(ordenId)} a {proveedor}.\n\n" +
                      $"Sí: enviarla por email a {proveedor.Email}.\nNo: marcarla como solicitada sin enviar email (pedido por teléfono, etc.)."
                    : $"El proveedor no tiene email cargado.\n¿Marcar la orden {OrdenReposicion.FormatearNumero(ordenId)} como solicitada (pedido por otro medio)?",
                "Emitir orden", tieneEmail ? MessageBoxButtons.YesNoCancel : MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (respuesta is DialogResult.Cancel) return;
            bool enviarEmail = respuesta == DialogResult.Yes;

            await EjecutarAsync(async () =>
            {
                var r = await OrdenReposicionService.Instancia.EmitirAsync(ordenId, enviarEmail, cts.Token);
                if (!r.Exito && enviarEmail &&
                    MessageBox.Show($"{r.Mensaje}\n\nLa orden sigue pendiente. ¿Marcarla como solicitada igual?",
                        "No se pudo enviar el email", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    r = await OrdenReposicionService.Instancia.EmitirAsync(ordenId, enviarEmail: false, cts.Token);
                }
                else if (!r.Exito && !enviarEmail)
                {
                    MessageBox.Show(r.Mensaje, "Emitir orden", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                if (r.Exito)
                    MessageBox.Show(r.Mensaje, "Orden emitida", MessageBoxButtons.OK, MessageBoxIcon.Information);

                await CargarOrdenAsync(ordenId);
                AplicarModo();
            }, "No se pudo emitir la orden.");
        }

        /// <summary>Primer clic: entra en modo recepción. Segundo clic: confirma y registra todo en una transacción.</summary>
        private async Task RecibirAsync()
        {
            if (ocupado || !EstadoOrden.EsActiva(estado)) return;

            if (!modoRecepcion)
            {
                if (estado == EstadoOrden.Pendiente && (modificado || OrdenId == null))
                {
                    if (MessageBox.Show("Hay cambios sin guardar. Se guardará la orden antes de registrar la recepción. ¿Continuar?",
                            "Registrar recepción", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        return;
                    if (!await GuardarAsync(mostrarMensaje: false)) return;
                }

                // Por defecto se asume que llegó exactamente lo pedido, al costo pactado.
                foreach (var item in items)
                {
                    item.CantidadRecibida ??= item.CantidadPedida;
                    item.CostoRecibido ??= item.CostoUnitario;
                }
                items.ResetBindings();
                modoRecepcion = true;
                AplicarModo();
                dgvItems.CurrentCell = dgvItems.Rows.Count > 0 ? dgvItems.Rows[0].Cells[colRecibido.Index] : null;
                dgvItems.Focus();
                return;
            }

            if (dgvItems.IsCurrentCellInEditMode && !dgvItems.EndEdit()) return;
            if (OrdenId is not int ordenId) return;

            int recibidas = items.Sum(i => i.CantidadRecibida ?? 0);
            int pedidas = items.Sum(i => i.CantidadPedida);
            var diferencias = items.Where(i => (i.CantidadRecibida ?? 0) != i.CantidadPedida).ToList();
            var cambiosCosto = items.Where(i => i.CostoRecibido is decimal c && c != i.CostoUnitario).ToList();

            string resumen =
                $"Se ingresarán {recibidas} unidad(es) al stock (pedidas: {pedidas}).\n" +
                (diferencias.Count > 0 ? $"• {diferencias.Count} ítem(s) con diferencia de cantidad.\n" : "") +
                (cambiosCosto.Count > 0
                    ? $"• {cambiosCosto.Count} ítem(s) con costo distinto al pactado" + (chkActualizarCostos.Checked ? " (se actualizará el costo).\n" : " (no se actualizará).\n")
                    : "") +
                "\nLa orden quedará como RECIBIDA y no se podrá modificar. ¿Confirmar la recepción?";

            if (MessageBox.Show(resumen, "Confirmar recepción", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            var solicitud = new RecepcionOrdenSolicitud
            {
                OrdenId = ordenId,
                Usuario = Usuario,
                ActualizarCostos = chkActualizarCostos.Checked,
                Lineas = items.Where(i => i.DetalleId != null)
                              .Select(i => (i.DetalleId!.Value, i.CantidadRecibida ?? 0, i.CostoRecibido ?? i.CostoUnitario))
                              .ToList(),
            };

            await EjecutarAsync(async () =>
            {
                var r = await OrdenReposicionService.Instancia.RecibirAsync(solicitud, cts.Token);
                MessageBox.Show(r.Mensaje, r.Exito ? "Recepción registrada" : "No se pudo registrar la recepción",
                    MessageBoxButtons.OK, r.Exito ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                if (r.Exito || r.Mensaje.Contains("otro usuario"))
                {
                    await CargarOrdenAsync(ordenId);
                    AplicarModo();
                }
            }, "No se pudo registrar la recepción. No se modificó el stock.");
        }

        #endregion

        #region Cierre y utilidades

        private void FrmOrdenReposicion_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (ocupado)
            {
                e.Cancel = true;
                return;
            }

            if (modoRecepcion && e.CloseReason == CloseReason.UserClosing)
            {
                // "Cancelar" en modo recepción vuelve a la vista normal sin registrar nada.
                if (MessageBox.Show("¿Salir sin registrar la recepción?", "Recepción", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }
            else if (modificado && Editable && e.CloseReason == CloseReason.UserClosing &&
                     MessageBox.Show("Hay cambios sin guardar en la orden. ¿Descartarlos?", "Cambios sin guardar",
                         MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            timerBusqueda.Stop();
            cts.Cancel();
        }

        private async Task EjecutarAsync(Func<Task> accion, string mensajeError)
        {
            if (ocupado) return;
            ocupado = true;
            UseWaitCursor = true;
            panelPie.Enabled = panelCarga.Enabled = panelCabecera.Enabled = dgvItems.Enabled = false;
            try
            {
                await accion();
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                MostrarError(mensajeError, ex);
            }
            finally
            {
                ocupado = false;
                UseWaitCursor = false;
                panelPie.Enabled = panelCarga.Enabled = panelCabecera.Enabled = dgvItems.Enabled = true;
            }
        }

        private void MostrarError(string mensaje, Exception ex) =>
            MessageBox.Show($"{mensaje}\n\nDetalle técnico: {ex.GetBaseException().Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

        #endregion
    }
}
