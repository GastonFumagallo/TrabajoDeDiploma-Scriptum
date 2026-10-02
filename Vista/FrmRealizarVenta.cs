using Controladora;
using Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Vista
{
    /// <summary>
    /// Punto de venta: 1) elegir cliente, 2) armar el carrito, 3) cobrar.
    /// El cobro se delega a <see cref="FrmConcretarVenta"/> (diálogo modal que sólo elige el método de pago)
    /// y la persistencia a <see cref="FacadeVentas.RealizarVentaAsync"/> (una única transacción).
    ///
    /// Atajos: F2 buscar libro · Enter en la grilla de libros → cantidad · Enter en cantidad → agregar ·
    /// Doble clic agrega 1 · Supr quita la línea del carrito · F12 cobrar.
    /// </summary>
    public partial class FrmRealizarVenta : Form
    {
        /// <summary>Línea del carrito. Subtotal calculado para que la grilla lo muestre sin lógica extra.</summary>
        public sealed class LineaCarrito
        {
            public int LibroId { get; init; }
            public string Titulo { get; init; } = string.Empty;
            public string Autor { get; init; } = string.Empty;
            public string Editorial { get; init; } = string.Empty;
            public decimal Precio { get; init; }
            public int Cantidad { get; set; }
            public decimal Subtotal => Precio * Cantidad;

            /// <summary>Stock en base al momento de cargar el catálogo. Validación de UI; la definitiva la hace el back-end.</summary>
            public int StockMaximo { get; set; }

            public LibroVentaDTO ToDto() => new LibroVentaDTO
            {
                LVDTO_ID = LibroId,
                Titulo = Titulo,
                Autor = Autor,
                Editorial = Editorial,
                Precio = Precio,
                Cantidad = Cantidad,
            };
        }

        private const string ColCantidad = nameof(LineaCarrito.Cantidad);

        // Catálogo en memoria: se carga una vez (async) y se filtra localmente,
        // en vez de ir a la base en cada tecla como antes.
        private List<LibroDTO> catalogo = new();
        private List<ClienteDTO> clientes = new();

        private readonly BindingSource bsLibros = new();
        private readonly BindingSource bsClientes = new();
        private readonly BindingList<LineaCarrito> carrito = new();

        private ClienteDTO? clienteActual;
        private bool procesando;
        private string textoBotonCobrar = string.Empty;

        // Debounce del filtro: espera a que el usuario deje de tipear antes de filtrar.
        private readonly System.Windows.Forms.Timer timerFiltroLibros = new() { Interval = 250 };
        private readonly CancellationTokenSource cts = new();

        public FrmRealizarVenta()
        {
            InitializeComponent();

            KeyPreview = true;
            KeyDown += FrmRealizarVenta_KeyDown;
            FormClosing += FrmRealizarVenta_FormClosing;
            Disposed += (_, _) => { timerFiltroLibros.Dispose(); cts.Dispose(); };

            ConfigurarGrillaLibros();
            ConfigurarGrillaCarrito();
            ConfigurarGrillaClientes();

            numCantidad.Minimum = 1;
            numCantidad.Value = 1;
            numCantidad.KeyDown += numCantidad_KeyDown;

            txtFiltrarLibro.KeyDown += txtFiltrarLibro_KeyDown;
            timerFiltroLibros.Tick += (_, _) => { timerFiltroLibros.Stop(); AplicarFiltroLibros(); };

            // Estos controles existían en el diseñador pero no tenían eventos conectados.
            txtFiltrarCliente.TextChanged += (_, _) => AplicarFiltroClientes();
            btnFiltrarCliente.Click += (_, _) => AplicarFiltroClientes();
            btnBorrarFiltrosCliente.Click += (_, _) => { txtFiltrarCliente.Text = string.Empty; };
            dgvClientes.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) btnSeleccionarCliente_Click(this, EventArgs.Empty); };
        }

        #region Configuración de grillas

        private static void HabilitarDoubleBuffer(DataGridView dgv) =>
            typeof(DataGridView)
                .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)?
                .SetValue(dgv, true);

        private void ConfigurarGrillaLibros()
        {
            HabilitarDoubleBuffer(dgvLibros);
            dgvLibros.AutoGenerateColumns = true;
            dgvLibros.AllowUserToAddRows = false;
            dgvLibros.AllowUserToDeleteRows = false;
            dgvLibros.MultiSelect = false;
            dgvLibros.RowHeadersVisible = false;
            dgvLibros.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLibros.DataSource = bsLibros;

            dgvLibros.DataBindingComplete += (_, _) =>
            {
                foreach (var col in new[] { "LIBDTO_ID", "Descripcion", "AñoPublicacion", "Genero" })
                    if (dgvLibros.Columns.Contains(col)) dgvLibros.Columns[col].Visible = false;
                if (dgvLibros.Columns.Contains("Precio"))
                    dgvLibros.Columns["Precio"].DefaultCellStyle.Format = "N2";
            };
            dgvLibros.SelectionChanged += (_, _) => MostrarLibroSeleccionado();
            dgvLibros.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) AgregarLibroSeleccionado(1); };
            dgvLibros.KeyDown += dgvLibros_KeyDown;
            dgvLibros.CellFormatting += dgvLibros_CellFormatting;
        }

        private void ConfigurarGrillaCarrito()
        {
            HabilitarDoubleBuffer(dgvVentas);
            dgvVentas.AutoGenerateColumns = true;
            dgvVentas.AllowUserToAddRows = false;
            dgvVentas.AllowUserToDeleteRows = false;
            dgvVentas.MultiSelect = false;
            dgvVentas.RowHeadersVisible = false;
            dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVentas.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            dgvVentas.ReadOnly = false;
            dgvVentas.DataSource = carrito;

            // Las columnas se generan recién al enlazar (puede ser después del constructor),
            // por eso se configuran acá y no inline.
            dgvVentas.DataBindingComplete += (_, _) =>
            {
                foreach (DataGridViewColumn col in dgvVentas.Columns)
                    col.ReadOnly = col.Name != ColCantidad;  // sólo la cantidad es editable

                foreach (var oculta in new[] { nameof(LineaCarrito.LibroId), nameof(LineaCarrito.StockMaximo) })
                    if (dgvVentas.Columns.Contains(oculta)) dgvVentas.Columns[oculta].Visible = false;
                foreach (var moneda in new[] { nameof(LineaCarrito.Precio), nameof(LineaCarrito.Subtotal) })
                    if (dgvVentas.Columns.Contains(moneda)) dgvVentas.Columns[moneda].DefaultCellStyle.Format = "N2";
            };

            dgvVentas.CellValidating += dgvVentas_CellValidating;
            dgvVentas.CellEndEdit += (_, e) =>
            {
                dgvVentas.Rows[e.RowIndex].ErrorText = string.Empty;
                carrito.ResetItem(e.RowIndex);  // refresca el Subtotal de la fila
                ActualizarTotales();
            };
            dgvVentas.DataError += (_, e) => e.Cancel = true;
            dgvVentas.KeyDown += dgvVentas_KeyDown;
        }

        private void ConfigurarGrillaClientes()
        {
            HabilitarDoubleBuffer(dgvClientes);
            dgvClientes.AllowUserToAddRows = false;
            dgvClientes.ReadOnly = true;
            dgvClientes.MultiSelect = false;
            dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClientes.DataSource = bsClientes;
            dgvClientes.DataBindingComplete += (_, _) =>
            {
                if (dgvClientes.Columns.Contains("CLIDTO_ID")) dgvClientes.Columns["CLIDTO_ID"].Visible = false;
            };
        }

        #endregion

        #region Carga de datos (async)

        private async void FrmRealizarVenta_Load(object sender, EventArgs e)
        {
            sinLibrosVenta();
            modoInicio();

            try
            {
                UseWaitCursor = true;
                await Task.WhenAll(CargarCatalogoAsync(), CargarClientesAsync());
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                MostrarError("No se pudieron cargar los datos iniciales.", ex);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private async Task CargarCatalogoAsync()
        {
            catalogo = await ControladoraLibros.Instancia.ObtenerLibrosGridAsync(cts.Token);
            SincronizarStockCarrito();
            AplicarFiltroLibros();
        }

        private async Task CargarClientesAsync()
        {
            clientes = await ControladoraClientes.Instancia.ObtenerClientesGridAsync(cts.Token);
            AplicarFiltroClientes();
        }

        #endregion

        #region Paso 1: cliente

        void modoInicio()
        {
            gbSeleccionarLibros.Enabled = false;
            btnSelectOtroCliente.Visible = false;
            lblClienteSeleccionado.Visible = false;
            lblNombreCliente.Visible = false;
        }

        private void AplicarFiltroClientes()
        {
            string filtro = txtFiltrarCliente.Text.Trim();
            bsClientes.DataSource = string.IsNullOrEmpty(filtro)
                ? clientes
                : clientes.Where(c => c.DNI.ToString().Contains(filtro)
                                   || (c.Nombre?.Contains(filtro, StringComparison.OrdinalIgnoreCase) ?? false))
                          .ToList();
        }

        private void btnSeleccionarCliente_Click(object sender, EventArgs e)
        {
            if (dgvClientes.CurrentRow?.DataBoundItem is not ClienteDTO cliente)
            {
                MessageBox.Show("Seleccione un cliente.", "Sin selección", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Sin confirmación extra: el cliente se puede cambiar en cualquier momento con "Seleccionar otro".
            clienteActual = cliente;
            gbSeleccionarCliente.Enabled = false;
            lblClienteSeleccionado.Visible = true;
            lblNombreCliente.Text = cliente.Nombre;
            lblNombreCliente.Visible = true;
            btnSelectOtroCliente.Visible = true;
            gbSeleccionarLibros.Enabled = true;
            txtFiltrarLibro.Focus();
        }

        private void btnSelectOtroCliente_Click(object sender, EventArgs e)
        {
            // El carrito se conserva: cambiar de cliente no obliga a volver a cargar los libros.
            clienteActual = null;
            gbSeleccionarCliente.Enabled = true;
            gbSeleccionarLibros.Enabled = false;
            btnSelectOtroCliente.Visible = false;
            lblClienteSeleccionado.Visible = false;
            lblNombreCliente.Visible = false;
            lblNombreCliente.Text = "";
            txtFiltrarCliente.Focus();
        }

        #endregion

        #region Paso 2: búsqueda y carrito

        private void AplicarFiltroLibros()
        {
            string filtro = txtFiltrarLibro.Text.Trim();
            bsLibros.DataSource = string.IsNullOrEmpty(filtro)
                ? catalogo
                : catalogo.Where(l => Contiene(l.Titulo, filtro) || Contiene(l.Autor, filtro) || Contiene(l.Editorial, filtro))
                          .ToList();
            MostrarLibroSeleccionado();

            static bool Contiene(string? texto, string filtro) =>
                texto?.Contains(filtro, StringComparison.OrdinalIgnoreCase) ?? false;
        }

        private void txtFiltrar_TextChanged(object sender, EventArgs e)
        {
            timerFiltroLibros.Stop();
            timerFiltroLibros.Start();
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            timerFiltroLibros.Stop();
            AplicarFiltroLibros();
            dgvLibros.Focus();
        }

        private void btnBorrarFiltros_Click(object sender, EventArgs e)
        {
            txtFiltrarLibro.Text = string.Empty;
            timerFiltroLibros.Stop();
            AplicarFiltroLibros();
            txtFiltrarLibro.Focus();
        }

        private void txtFiltrarLibro_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode is Keys.Enter or Keys.Down)
            {
                e.SuppressKeyPress = true;
                timerFiltroLibros.Stop();
                AplicarFiltroLibros();
                dgvLibros.Focus();
            }
        }

        private void dgvLibros_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;  // evita que Enter baje de fila
                numCantidad.Focus();
                numCantidad.Select(0, numCantidad.Text.Length);
            }
        }

        private void numCantidad_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnAgregarLibro_Click(this, EventArgs.Empty);
            }
        }

        private void dgvLibros_CellClick(object sender, DataGridViewCellEventArgs e) => MostrarLibroSeleccionado();

        private LibroDTO? LibroSeleccionado => dgvLibros.CurrentRow?.DataBoundItem as LibroDTO;

        private int CantidadEnCarrito(int libroId) =>
            carrito.Where(l => l.LibroId == libroId).Sum(l => l.Cantidad);

        /// <summary>Stock que todavía se puede agregar: stock del catálogo menos lo que ya está en el carrito.</summary>
        private int StockDisponible(LibroDTO libro) => libro.Stock - CantidadEnCarrito(libro.LIBDTO_ID);

        private void MostrarLibroSeleccionado()
        {
            var libro = LibroSeleccionado;
            if (libro == null)
            {
                lblTitulo.Text = "";
                lblPrecio.Text = "";
                btnAgregarLibro.Enabled = false;
                return;
            }

            int disponible = StockDisponible(libro);
            lblTitulo.Text = libro.Titulo;
            lblPrecio.Text = libro.Precio.ToString("N2");
            numCantidad.Maximum = Math.Max(1, disponible);
            btnAgregarLibro.Enabled = disponible > 0;
            numCantidad.Enabled = disponible > 0;
        }

        /// <summary>Atenúa los libros sin stock disponible (contando lo que ya está en el carrito).</summary>
        private void dgvLibros_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvLibros.Rows[e.RowIndex].DataBoundItem is not LibroDTO libro) return;
            if (StockDisponible(libro) <= 0)
                e.CellStyle.ForeColor = Color.Gray;
        }

        private void btnAgregarLibro_Click(object sender, EventArgs e) => AgregarLibroSeleccionado((int)numCantidad.Value);

        private void AgregarLibroSeleccionado(int cantidad)
        {
            var libro = LibroSeleccionado;
            if (libro == null)
            {
                MessageBox.Show("Seleccione un libro.", "Sin selección", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int disponible = StockDisponible(libro);
            if (cantidad > disponible)
            {
                MessageBox.Show($"No hay suficiente stock de '{libro.Titulo}'.\nDisponible: {disponible}, solicitado: {cantidad}.",
                    "Stock insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Si el libro ya está en el carrito se suma la cantidad en lugar de rechazarlo.
            var existente = carrito.FirstOrDefault(l => l.LibroId == libro.LIBDTO_ID);
            if (existente != null)
            {
                existente.Cantidad += cantidad;
                carrito.ResetItem(carrito.IndexOf(existente));
            }
            else
            {
                carrito.Add(new LineaCarrito
                {
                    LibroId = libro.LIBDTO_ID,
                    Titulo = libro.Titulo,
                    Autor = libro.Autor,
                    Editorial = libro.Editorial,
                    Precio = libro.Precio,
                    Cantidad = cantidad,
                    StockMaximo = libro.Stock,
                });
            }

            ActualizarTotales();
            numCantidad.Value = 1;
            txtFiltrarLibro.Focus();
            txtFiltrarLibro.SelectAll();
        }

        private void dgvVentas_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dgvVentas.Columns[e.ColumnIndex].Name != ColCantidad) return;
            if (dgvVentas.Rows[e.RowIndex].DataBoundItem is not LineaCarrito linea) return;

            if (!int.TryParse(Convert.ToString(e.FormattedValue), out int nueva) || nueva < 1)
            {
                dgvVentas.Rows[e.RowIndex].ErrorText = "La cantidad debe ser un número mayor a 0.";
                e.Cancel = true;
            }
            else if (nueva > linea.StockMaximo)
            {
                dgvVentas.Rows[e.RowIndex].ErrorText = $"Stock disponible: {linea.StockMaximo}.";
                e.Cancel = true;
            }
        }

        private void dgvVentas_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && !dgvVentas.IsCurrentCellInEditMode)
            {
                e.SuppressKeyPress = true;
                btnEliminarLibro_Click(this, EventArgs.Empty);
            }
        }

        private void btnEliminarLibro_Click(object sender, EventArgs e)
        {
            if (dgvVentas.CurrentRow?.DataBoundItem is not LineaCarrito linea)
            {
                MessageBox.Show("Seleccione un libro de la venta para eliminar.", "Sin selección", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacion = MessageBox.Show($"¿Desea quitar '{linea.Titulo}' de la venta?", "Confirmar eliminación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (confirmacion == DialogResult.Yes)
            {
                carrito.Remove(linea);
                ActualizarTotales();
            }
        }

        private decimal Subtotal => carrito.Sum(l => l.Subtotal);

        /// <summary>Único lugar donde se recalcula el total y se muestran/ocultan los controles del paso 3.</summary>
        private void ActualizarTotales()
        {
            if (carrito.Count == 0)
            {
                sinLibrosVenta();
            }
            else
            {
                lblPasoTres.Visible = true;
                dgvVentas.Visible = true;
                btnEliminarLibro.Visible = true;
                btnGenerarVenta.Visible = true;
                lblTotal.Text = "Subtotal:";
                lblTotalNumero.Text = Subtotal.ToString("N2");
            }

            dgvLibros.Invalidate();  // re-evalúa el atenuado de filas sin stock
            MostrarLibroSeleccionado();
        }

        void sinLibrosVenta()
        {
            lblPasoTres.Visible = false;
            lblTotal.Text = "";
            lblTotalNumero.Text = "";
            dgvVentas.Visible = false;
            btnEliminarLibro.Visible = false;
            btnGenerarVenta.Visible = false;
        }

        /// <summary>Tras recargar el catálogo, actualiza el tope de stock de las líneas que siguen en el carrito.</summary>
        private void SincronizarStockCarrito()
        {
            var stockPorId = catalogo.ToDictionary(l => l.LIBDTO_ID, l => l.Stock);
            foreach (var linea in carrito)
                linea.StockMaximo = stockPorId.TryGetValue(linea.LibroId, out int stock) ? stock : 0;
        }

        #endregion

        #region Paso 3: cobro y persistencia

        private bool ValidarAntesDeCobrar()
        {
            if (dgvVentas.IsCurrentCellInEditMode && !dgvVentas.EndEdit())
                return false;  // hay una cantidad inválida a medio editar

            if (clienteActual == null)
            {
                MessageBox.Show("Debe seleccionar un cliente.", "Cliente no seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (carrito.Count == 0)
            {
                MessageBox.Show("Debe agregar al menos un libro a la venta.", "Venta sin productos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            var excedida = carrito.FirstOrDefault(l => l.Cantidad > l.StockMaximo);
            if (excedida != null)
            {
                MessageBox.Show($"La cantidad de '{excedida.Titulo}' supera el stock disponible ({excedida.StockMaximo}).",
                    "Stock insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private async void btnGenerarVenta_Click(object sender, EventArgs e)
        {
            // Guard contra doble clic / doble F12: el botón se deshabilita, pero el flag cubre también el atajo.
            if (procesando || !ValidarAntesDeCobrar()) return;

            var cliente = clienteActual!;
            decimal totalMostrado;
            int metodoPagoId;

            using (var frmCobro = new FrmConcretarVenta(cliente, Subtotal))
            {
                if (frmCobro.ShowDialog(this) != DialogResult.OK || frmCobro.MetodoPagoSeleccionado == null)
                    return;
                metodoPagoId = frmCobro.MetodoPagoSeleccionado.MP_ID;
                totalMostrado = frmCobro.TotalFinal;
            }

            SetProcesando(true);
            try
            {
                var items = carrito.Select(l => l.ToDto()).ToList();
                var resultado = await FacadeVentas.Instancia.RealizarVentaAsync(cliente.CLIDTO_ID, metodoPagoId, items, cts.Token);

                if (!resultado.Exito)
                {
                    MessageBox.Show(resultado.Mensaje + "\n\nNo se registró la venta. El stock se actualizó; revise el carrito.",
                        "Venta no registrada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    await CargarCatalogoAsync();
                    return;
                }

                string aviso = resultado.Total != totalMostrado
                    ? $"\n\nAtención: el total registrado difiere del mostrado (${totalMostrado:N2}) porque cambiaron precios."
                    : string.Empty;

                var verTicket = MessageBox.Show(
                    $"Venta N° {resultado.VentaId} registrada.\nTotal cobrado: ${resultado.Total:N2}{aviso}\n\n¿Desea ver el ticket?",
                    "Venta registrada", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                ReiniciarVenta();
                await CargarCatalogoAsync();  // stock fresco para la próxima venta

                if (verTicket == DialogResult.Yes)
                    await MostrarTicketAsync(resultado.VentaId);
            }
            catch (OperationCanceledException)
            {
                // El formulario se está cerrando.
            }
            catch (Exception ex)
            {
                MostrarError("No se pudo registrar la venta. No se guardó ningún cambio.", ex);
            }
            finally
            {
                SetProcesando(false);
            }
        }

        private async Task MostrarTicketAsync(int ventaId)
        {
            try
            {
                var ticket = await ControladoraVentas.Instancia.GenerarTicketAsync(ventaId, cts.Token);
                if (ticket == null) return;
                using var frmTicket = new FrmTickets(ticket);
                frmTicket.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MostrarError("La venta se registró, pero no se pudo generar el ticket.", ex);
            }
        }

        /// <summary>Bloquea la UI mientras se guarda, para evitar doble cobro o cambios en el carrito a mitad de la operación.</summary>
        private void SetProcesando(bool valor)
        {
            procesando = valor;
            UseWaitCursor = valor;
            gbSeleccionarLibros.Enabled = !valor && clienteActual != null;
            gbSeleccionarCliente.Enabled = !valor && clienteActual == null;
            btnSelectOtroCliente.Enabled = !valor;
            btnEliminarLibro.Enabled = !valor;
            dgvVentas.Enabled = !valor;
            btnVolverGestionarVEN.Enabled = !valor;
            btnGenerarVenta.Enabled = !valor;

            if (valor)
            {
                textoBotonCobrar = btnGenerarVenta.Text;
                btnGenerarVenta.Text = "Procesando...";
            }
            else if (!string.IsNullOrEmpty(textoBotonCobrar))
            {
                btnGenerarVenta.Text = textoBotonCobrar;
            }
        }

        /// <summary>Deja el formulario listo para la próxima venta (mismo flujo que al abrirlo).</summary>
        private void ReiniciarVenta()
        {
            carrito.Clear();
            clienteActual = null;
            txtFiltrarLibro.Text = string.Empty;
            txtFiltrarCliente.Text = string.Empty;
            numCantidad.Value = 1;
            gbSeleccionarCliente.Enabled = true;
            lblNombreCliente.Text = "";
            sinLibrosVenta();
            modoInicio();
            txtFiltrarCliente.Focus();
        }

        #endregion

        #region Navegación, atajos y cierre

        private void FrmRealizarVenta_KeyDown(object? sender, KeyEventArgs e)
        {
            if (procesando) return;

            if (e.KeyCode == Keys.F2 && gbSeleccionarLibros.Enabled)
            {
                txtFiltrarLibro.Focus();
                txtFiltrarLibro.SelectAll();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F12 && btnGenerarVenta.Visible && btnGenerarVenta.Enabled)
            {
                btnGenerarVenta.PerformClick();
                e.Handled = true;
            }
        }

        private void btnVolverGestionarVEN_Click(object sender, EventArgs e)
        {
            if (carrito.Count > 0 &&
                MessageBox.Show("Hay una venta en curso. ¿Desea descartarla?", "Venta en curso",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            if (Application.OpenForms["FrmMenu"] is FrmMenu principal)
                principal.MostrarGestionarVentas();

            Close();
        }

        private void FrmRealizarVenta_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // No se permite cerrar mientras se está guardando una venta.
            if (procesando)
            {
                e.Cancel = true;
                return;
            }
            timerFiltroLibros.Stop();
            cts.Cancel();
        }

        private void MostrarError(string mensaje, Exception ex) =>
            MessageBox.Show($"{mensaje}\n\nDetalle: {ex.GetBaseException().Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

        #endregion
    }
}
