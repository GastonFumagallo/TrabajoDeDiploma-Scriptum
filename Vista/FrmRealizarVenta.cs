using Controladora;
using Controladora.MetodoPagoStrategy;
using Modelo;
using Servicios;
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
    /// Punto de venta en una sola pantalla: encabezado (fecha, comprobante, usuario, cliente),
    /// carga rápida de libros (lector de código de barras o búsqueda por texto), carrito y panel de cierre
    /// (descuento, recargo del medio de pago, IVA, total, monto recibido y vuelto).
    ///
    /// La pantalla sólo arma la <see cref="SolicitudVenta"/> y muestra la vista previa de importes con
    /// <see cref="CalculadoraVenta"/>; el registro definitivo (precios, stock, transacción) lo hace
    /// <see cref="FacadeVentas.RegistrarVentaAsync"/>.
    /// </summary>
    public partial class FrmRealizarVenta : Form
    {
        #region Tipos auxiliares

        /// <summary>Línea del carrito enlazada a la grilla.</summary>
        public sealed class LineaCarrito
        {
            public int LibroId { get; init; }
            public string Producto { get; init; } = string.Empty;
            public decimal PrecioUnitario { get; init; }
            public int Cantidad { get; set; }
            public decimal Subtotal => PrecioUnitario * Cantidad;

            /// <summary>Stock al momento de cargar el catálogo. Validación de UI; la definitiva la hace el servicio.</summary>
            public int StockMaximo { get; set; }
        }

        /// <summary>Ítem del combo de clientes: texto buscable "Nombre — documento".</summary>
        private sealed record ClienteItem(ClienteDTO Cliente, bool EsConsumidorFinal)
        {
            public override string ToString() =>
                EsConsumidorFinal || Cliente.Documento == null ? Cliente.Nombre : $"{Cliente.Nombre} — {Cliente.Documento}";
        }

        /// <summary>Ítem de la lista de sugerencias de búsqueda.</summary>
        private sealed record SugerenciaLibro(LibroCatalogoDTO Libro, int Disponible)
        {
            public override string ToString() =>
                $"{Libro.Titulo} — {Libro.Autor}    ${Libro.Precio:N2}    (stock {Disponible})";
        }

        #endregion

        private const int MaxSugerencias = 30;

        private List<LibroCatalogoDTO> catalogo = new();
        private Dictionary<string, LibroCatalogoDTO> catalogoPorIsbn = new();
        private readonly BindingList<LineaCarrito> carrito = new();
        private LibroCatalogoDTO? libroSeleccionado;

        private ClienteItem? clienteActual;
        private ClienteItem? consumidorFinal;

        private CalculoVenta calculo = CalculadoraVenta.Calcular(0, 0, null, null, 0);
        private bool montoRecibidoEditado;   // si el cajero tipeó el monto, no se lo pisa al recalcular
        private bool actualizandoMonto;
        private bool procesando;
        private int? ultimaVentaId;

        private readonly System.Windows.Forms.Timer timerBusqueda = new() { Interval = 150 };
        private readonly System.Windows.Forms.Timer timerReloj = new() { Interval = 30_000 };
        private readonly CancellationTokenSource cts = new();

        public FrmRealizarVenta()
        {
            InitializeComponent();

            ConfigurarCarrito();
            ConfigurarBusqueda();
            ConfigurarCliente();
            ConfigurarCierre();

            btnVolver.Click += (_, _) => Volver();
            FormClosing += FrmRealizarVenta_FormClosing;
            Disposed += (_, _) => { timerBusqueda.Dispose(); timerReloj.Dispose(); cts.Dispose(); };

            timerReloj.Tick += (_, _) => ActualizarEncabezado();
        }

        #region Carga inicial

        private async void FrmRealizarVenta_Load(object sender, EventArgs e)
        {
            ActualizarEncabezado();
            timerReloj.Start();
            Recalcular();

            try
            {
                UseWaitCursor = true;
                panelCarga.Enabled = panelCierre.Enabled = panelEncabezado.Enabled = false;
                await Task.WhenAll(CargarCatalogoAsync(), CargarClientesAsync(), CargarMetodosPagoAsync());
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                MostrarError("No se pudieron cargar los datos del punto de venta.", ex);
            }
            finally
            {
                UseWaitCursor = false;
                panelCarga.Enabled = panelCierre.Enabled = panelEncabezado.Enabled = true;
            }

            txtBuscar.Focus();
        }

        private async Task CargarCatalogoAsync()
        {
            catalogo = await Controladora.Abm.LibroService.Instancia.ObtenerCatalogoVentaAsync(cts.Token);
            catalogoPorIsbn = catalogo
                .Select(l => (Libro: l, Isbn: Libro.NormalizarISBN(l.ISBN)))
                .Where(x => x.Isbn != null)
                .GroupBy(x => x.Isbn!)
                .ToDictionary(g => g.Key, g => g.First().Libro);

            // Refresca el tope de stock de lo que ya está en el carrito.
            var stockPorId = catalogo.ToDictionary(l => l.Id, l => l.Stock);
            foreach (var linea in carrito)
                linea.StockMaximo = stockPorId.TryGetValue(linea.LibroId, out int stock) ? stock : 0;

            if (libroSeleccionado != null)
                SeleccionarLibro(catalogo.FirstOrDefault(l => l.Id == libroSeleccionado.Id), moverFoco: false);
        }

        private async Task CargarClientesAsync(int? seleccionarClienteId = null)
        {
            // Activos, con Consumidor Final primero (lo crea si no existe).
            var clientes = await Controladora.Abm.ClienteService.Instancia.ObtenerParaVentaAsync(cts.Token);
            var items = clientes.Select(c => new ClienteItem(c, c.EsConsumidorFinal)).ToList();
            consumidorFinal = items.FirstOrDefault(i => i.EsConsumidorFinal);

            cbCliente.DataSource = items;
            var aSeleccionar = items.FirstOrDefault(i => i.Cliente.CLIDTO_ID == (seleccionarClienteId ?? clienteActual?.Cliente.CLIDTO_ID))
                               ?? consumidorFinal;
            EstablecerCliente(aSeleccionar);
        }

        private async Task CargarMetodosPagoAsync()
        {
            var metodos = await ControladoraMetodosPago.Instancia.ObtenerMetodosPagoAsync(soloActivos: true, cts.Token);
            int? anterior = (cbMetodoPago.SelectedItem as MetodoPago)?.MP_ID;

            cbMetodoPago.DataSource = metodos;
            cbMetodoPago.DisplayMember = nameof(MetodoPago.MP_Nombre);
            cbMetodoPago.ValueMember = nameof(MetodoPago.MP_ID);

            // Por defecto: el que estaba elegido, si no Efectivo, si no el primero.
            var porDefecto = metodos.FirstOrDefault(m => m.MP_ID == anterior)
                             ?? metodos.FirstOrDefault(MetodoPagoStrategyFactory.EsEfectivo)
                             ?? metodos.FirstOrDefault();
            cbMetodoPago.SelectedItem = porDefecto;
            Recalcular();
        }

        private void ActualizarEncabezado()
        {
            lblFecha.Text = $"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}";
            lblUsuario.Text = $"Usuario: {UsuarioActual}";
            lblComprobante.Text = ultimaVentaId is int id
                ? $"Comprobante: nuevo · última {Venta.FormatearComprobante(id)}"
                : "Comprobante: nuevo (se numera al registrar)";
        }

        private static string UsuarioActual => PermisoService.Instancia.UsuarioActual?.USU_Nombre ?? "—";

        #endregion

        #region Cliente

        private void ConfigurarCliente()
        {
            cbCliente.DropDownStyle = ComboBoxStyle.DropDown;
            cbCliente.SelectionChangeCommitted += (_, _) =>
            {
                if (cbCliente.SelectedItem is ClienteItem item) EstablecerCliente(item);
            };
            // Si el texto tipeado no corresponde a ningún cliente, se vuelve al último válido.
            cbCliente.Validating += (_, _) =>
            {
                var item = cbCliente.SelectedItem as ClienteItem
                           ?? (cbCliente.DataSource as List<ClienteItem>)?
                               .FirstOrDefault(i => string.Equals(i.ToString(), cbCliente.Text.Trim(), StringComparison.OrdinalIgnoreCase));
                EstablecerCliente(item ?? clienteActual ?? consumidorFinal);
            };
            cbCliente.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    txtBuscar.Focus();   // dispara Validating y vuelve a la carga de productos
                }
            };

            btnConsumidorFinal.Click += (_, _) => EstablecerCliente(consumidorFinal);
            btnNuevoCliente.Click += async (_, _) => await AltaRapidaClienteAsync();
            toolTip.SetToolTip(btnNuevoCliente, "Dar de alta un cliente nuevo");
        }

        private void EstablecerCliente(ClienteItem? item)
        {
            if (item == null) return;
            clienteActual = item;
            if (!Equals(cbCliente.SelectedItem, item))
                cbCliente.SelectedItem = item;
        }

        private async Task AltaRapidaClienteAsync()
        {
            int? nuevoId;
            using (var frm = new FrmEditarCliente(null))
            {
                if (frm.ShowDialog(this) != DialogResult.OK) return;
                nuevoId = frm.ClienteId;
            }

            try
            {
                // Se recarga la lista y el cliente recién creado (o reactivado) queda seleccionado.
                await CargarClientesAsync(nuevoId);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                MostrarError("No se pudo actualizar la lista de clientes.", ex);
            }
        }

        #endregion

        #region Búsqueda y carga de productos

        private void ConfigurarBusqueda()
        {
            timerBusqueda.Tick += (_, _) => { timerBusqueda.Stop(); MostrarSugerencias(); };
            txtBuscar.TextChanged += (_, _) => { timerBusqueda.Stop(); timerBusqueda.Start(); };
            txtBuscar.KeyDown += txtBuscar_KeyDown;
            txtBuscar.Leave += (_, _) => BeginInvoke(() => { if (!lstSugerencias.Focused) OcultarSugerencias(); });

            lstSugerencias.KeyDown += lstSugerencias_KeyDown;
            lstSugerencias.MouseClick += (_, _) => ElegirSugerencia();
            lstSugerencias.Leave += (_, _) => BeginInvoke(() => { if (!txtBuscar.Focused) OcultarSugerencias(); });

            numCantidad.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    AgregarSeleccionado((int)numCantidad.Value);
                }
            };
            btnAgregar.Click += (_, _) => AgregarSeleccionado((int)numCantidad.Value);
            numCantidad.Maximum = 100_000;
        }

        private void txtBuscar_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    e.SuppressKeyPress = true;
                    timerBusqueda.Stop();
                    ResolverBusqueda();
                    break;
                case Keys.Down when lstSugerencias.Visible && lstSugerencias.Items.Count > 0:
                    e.SuppressKeyPress = true;
                    lstSugerencias.Focus();
                    lstSugerencias.SelectedIndex = Math.Max(0, lstSugerencias.SelectedIndex);
                    break;
            }
        }

        private void lstSugerencias_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    e.SuppressKeyPress = true;
                    ElegirSugerencia();
                    break;
                case Keys.Up when lstSugerencias.SelectedIndex <= 0:
                    e.SuppressKeyPress = true;
                    txtBuscar.Focus();
                    break;
            }
        }

        /// <summary>
        /// Enter en el buscador. Prioridad: 1) código de barras/ISBN exacto → se agrega 1 unidad directo
        /// (flujo de lector); 2) sugerencia resaltada o resultado único → se selecciona; 3) si no, se muestran opciones.
        /// </summary>
        private void ResolverBusqueda()
        {
            string texto = txtBuscar.Text.Trim();
            if (texto.Length == 0) return;

            var isbn = Libro.NormalizarISBN(texto);
            if (isbn != null && isbn.Length >= 8 && catalogoPorIsbn.TryGetValue(isbn, out var porCodigo))
            {
                OcultarSugerencias();
                SeleccionarLibro(porCodigo, moverFoco: false);
                if (AgregarSeleccionado(1))
                    txtBuscar.Clear();
                return;
            }

            var coincidencias = BuscarEnCatalogo(texto).Take(2).ToList();
            if (coincidencias.Count == 1)
            {
                OcultarSugerencias();
                SeleccionarLibro(coincidencias[0], moverFoco: true);
            }
            else if (coincidencias.Count == 0)
            {
                MostrarEstado($"No se encontró ningún libro para \"{texto}\".", esError: true);
            }
            else
            {
                MostrarSugerencias();
                lstSugerencias.Focus();
                lstSugerencias.SelectedIndex = 0;
            }
        }

        private IEnumerable<LibroCatalogoDTO> BuscarEnCatalogo(string texto) =>
            catalogo.Where(l => Contiene(l.Titulo, texto) || Contiene(l.Autor, texto) || Contiene(l.Editorial, texto)
                                || (l.ISBN?.Contains(texto, StringComparison.OrdinalIgnoreCase) ?? false))
                    .OrderBy(l => l.Titulo);

        private static bool Contiene(string? campo, string texto) =>
            campo?.Contains(texto, StringComparison.OrdinalIgnoreCase) ?? false;

        private void MostrarSugerencias()
        {
            string texto = txtBuscar.Text.Trim();
            if (texto.Length < 2)
            {
                OcultarSugerencias();
                return;
            }

            var sugerencias = BuscarEnCatalogo(texto)
                .Take(MaxSugerencias)
                .Select(l => new SugerenciaLibro(l, StockDisponible(l)))
                .ToArray();

            if (sugerencias.Length == 0)
            {
                OcultarSugerencias();
                return;
            }

            lstSugerencias.BeginUpdate();
            lstSugerencias.Items.Clear();
            lstSugerencias.Items.AddRange(sugerencias);
            lstSugerencias.EndUpdate();

            // Se ubica justo debajo del buscador, por encima de la grilla.
            var origen = PointToClient(txtBuscar.Parent!.PointToScreen(txtBuscar.Location));
            lstSugerencias.Location = new Point(origen.X, origen.Y + txtBuscar.Height + 2);
            lstSugerencias.Width = Math.Max(txtBuscar.Width, 760);
            lstSugerencias.Height = Math.Min(260, lstSugerencias.ItemHeight * sugerencias.Length + 6);
            lstSugerencias.Visible = true;
            lstSugerencias.BringToFront();
        }

        private void OcultarSugerencias() => lstSugerencias.Visible = false;

        private void ElegirSugerencia()
        {
            if (lstSugerencias.SelectedItem is not SugerenciaLibro s) return;
            OcultarSugerencias();
            SeleccionarLibro(s.Libro, moverFoco: true);
        }

        private int CantidadEnCarrito(int libroId) => carrito.Where(l => l.LibroId == libroId).Sum(l => l.Cantidad);

        private int StockDisponible(LibroCatalogoDTO libro) => libro.Stock - CantidadEnCarrito(libro.Id);

        /// <summary>Muestra el libro elegido (precio y stock visibles) y, si se pide, pasa el foco a la cantidad.</summary>
        private void SeleccionarLibro(LibroCatalogoDTO? libro, bool moverFoco)
        {
            libroSeleccionado = libro;
            if (libro == null)
            {
                lblSeleccion.Text = string.Empty;
                return;
            }

            int disponible = StockDisponible(libro);
            lblSeleccion.Text = $"{libro.Titulo} — ${libro.Precio:N2} — Stock disponible: {disponible}";
            lblSeleccion.ForeColor = disponible > 0 ? Color.White : Color.Gold;
            numCantidad.Value = 1;

            if (moverFoco)
            {
                numCantidad.Focus();
                numCantidad.Select(0, numCantidad.Text.Length);
            }
        }

        /// <summary>Agrega (o suma a la línea existente) validando contra el stock en memoria.</summary>
        private bool AgregarSeleccionado(int cantidad)
        {
            if (procesando) return false;
            var libro = libroSeleccionado;
            if (libro == null)
            {
                MostrarEstado("Busque y seleccione un libro primero.", esError: true);
                txtBuscar.Focus();
                return false;
            }

            int disponible = StockDisponible(libro);
            if (cantidad > disponible)
            {
                MostrarEstado(disponible <= 0
                    ? $"Sin stock disponible de '{libro.Titulo}'."
                    : $"Stock insuficiente de '{libro.Titulo}': disponible {disponible}.", esError: true);
                System.Media.SystemSounds.Beep.Play();
                return false;
            }

            var existente = carrito.FirstOrDefault(l => l.LibroId == libro.Id);
            if (existente != null)
            {
                existente.Cantidad += cantidad;
                carrito.ResetItem(carrito.IndexOf(existente));
            }
            else
            {
                existente = new LineaCarrito
                {
                    LibroId = libro.Id,
                    Producto = $"{libro.Titulo} — {libro.Autor}",
                    PrecioUnitario = libro.Precio,
                    Cantidad = cantidad,
                    StockMaximo = libro.Stock,
                };
                carrito.Add(existente);
            }

            SeleccionarFilaCarrito(existente);
            MostrarEstado($"Agregado: {cantidad} × {libro.Titulo}", esError: false);
            Recalcular();

            // Listo para el próximo producto.
            libroSeleccionado = null;
            lblSeleccion.Text = string.Empty;
            numCantidad.Value = 1;
            txtBuscar.Clear();
            txtBuscar.Focus();
            return true;
        }

        private void MostrarEstado(string mensaje, bool esError)
        {
            lblEstado.Text = mensaje;
            lblEstado.ForeColor = esError ? Color.Gold : Color.White;
        }

        #endregion

        #region Carrito

        private void ConfigurarCarrito()
        {
            typeof(DataGridView)
                .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)?
                .SetValue(dgvCarrito, true);

            dgvCarrito.AutoGenerateColumns = false;
            dgvCarrito.DataSource = carrito;
            colPrecio.DefaultCellStyle.Format = "N2";
            colSubtotal.DefaultCellStyle.Format = "N2";
            colPrecio.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colSubtotal.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colCantidad.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvCarrito.CellContentClick += dgvCarrito_CellContentClick;
            dgvCarrito.CellValidating += dgvCarrito_CellValidating;
            dgvCarrito.CellEndEdit += (_, e) =>
            {
                dgvCarrito.Rows[e.RowIndex].ErrorText = string.Empty;
                carrito.ResetItem(e.RowIndex);
                Recalcular();
            };
            dgvCarrito.DataError += (_, e) => e.Cancel = true;
            dgvCarrito.KeyDown += dgvCarrito_KeyDown;
            carrito.ListChanged += (_, _) => btnRegistrar.Enabled = !procesando && carrito.Count > 0;
        }

        private LineaCarrito? LineaActual => dgvCarrito.CurrentRow?.DataBoundItem as LineaCarrito;

        private void dgvCarrito_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || procesando || dgvCarrito.Rows[e.RowIndex].DataBoundItem is not LineaCarrito linea) return;

            if (e.ColumnIndex == colMenos.Index) CambiarCantidad(linea, -1);
            else if (e.ColumnIndex == colMas.Index) CambiarCantidad(linea, +1);
            else if (e.ColumnIndex == colQuitar.Index) QuitarLinea(linea);
        }

        private void dgvCarrito_KeyDown(object? sender, KeyEventArgs e)
        {
            if (dgvCarrito.IsCurrentCellInEditMode || procesando || LineaActual is not LineaCarrito linea) return;

            switch (e.KeyCode)
            {
                case Keys.Delete:
                    QuitarLinea(linea);
                    e.Handled = true;
                    break;
                case Keys.Add or Keys.Oemplus:
                    CambiarCantidad(linea, +1);
                    e.Handled = e.SuppressKeyPress = true;
                    break;
                case Keys.Subtract or Keys.OemMinus:
                    CambiarCantidad(linea, -1);
                    e.Handled = e.SuppressKeyPress = true;
                    break;
            }
        }

        private void CambiarCantidad(LineaCarrito linea, int delta)
        {
            int nueva = linea.Cantidad + delta;
            if (nueva < 1)
            {
                MostrarEstado("La cantidad mínima es 1. Use \"Quitar\" (Supr) para sacar la línea.", esError: true);
                return;
            }
            if (nueva > linea.StockMaximo)
            {
                MostrarEstado($"Stock disponible: {linea.StockMaximo}.", esError: true);
                System.Media.SystemSounds.Beep.Play();
                return;
            }

            linea.Cantidad = nueva;
            carrito.ResetItem(carrito.IndexOf(linea));
            Recalcular();
        }

        private void QuitarLinea(LineaCarrito linea)
        {
            int indice = carrito.IndexOf(linea);
            carrito.Remove(linea);
            MostrarEstado($"Quitado: {linea.Producto}", esError: false);
            Recalcular();

            if (carrito.Count > 0)
                dgvCarrito.CurrentCell = dgvCarrito.Rows[Math.Min(indice, carrito.Count - 1)].Cells[colProducto.Index];
            else
                txtBuscar.Focus();
        }

        private void SeleccionarFilaCarrito(LineaCarrito linea)
        {
            int i = carrito.IndexOf(linea);
            if (i >= 0 && i < dgvCarrito.Rows.Count)
                dgvCarrito.CurrentCell = dgvCarrito.Rows[i].Cells[colProducto.Index];
        }

        private void dgvCarrito_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex != colCantidad.Index || dgvCarrito.Rows[e.RowIndex].DataBoundItem is not LineaCarrito linea) return;

            if (!int.TryParse(Convert.ToString(e.FormattedValue), out int nueva) || nueva < 1)
            {
                dgvCarrito.Rows[e.RowIndex].ErrorText = "La cantidad debe ser un número entero mayor a 0.";
                e.Cancel = true;
            }
            else if (nueva > linea.StockMaximo)
            {
                dgvCarrito.Rows[e.RowIndex].ErrorText = $"Stock disponible: {linea.StockMaximo}.";
                e.Cancel = true;
            }
        }

        #endregion

        #region Cierre: importes, medio de pago y vuelto

        private void ConfigurarCierre()
        {
            numDescuento.Maximum = ConfiguracionVentas.DescuentoMaximoPorcentaje;
            // El descuento manual requiere permiso (el Administrador siempre lo tiene).
            numDescuento.Enabled = PermisoService.Instancia.TienePermiso("AplicarDescuento");
            numDescuento.ValueChanged += (_, _) => Recalcular();

            cbMetodoPago.SelectedIndexChanged += (_, _) =>
            {
                montoRecibidoEditado = false;  // al cambiar de medio, se vuelve a proponer el total
                Recalcular();
            };
            numMontoRecibido.ValueChanged += (_, _) =>
            {
                if (actualizandoMonto) return;
                montoRecibidoEditado = true;
                Recalcular();
            };
            numMontoRecibido.Enter += (_, _) => numMontoRecibido.Select(0, numMontoRecibido.Text.Length);
            numMontoRecibido.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    btnRegistrar.PerformClick();
                }
            };

            btnRegistrar.Click += async (_, _) => await RegistrarVentaAsync();
            btnCancelar.Click += (_, _) => CancelarVenta();
            btnImprimir.Click += async (_, _) => await ImprimirUltimoTicketAsync();
            btnMediosPago.Click += async (_, _) => await GestionarMediosPagoAsync();
            toolTip.SetToolTip(btnMediosPago, "Administrar medios de pago");

            btnRegistrar.Enabled = false;
            btnImprimir.Enabled = false;
        }

        private MetodoPago? MetodoSeleccionado => cbMetodoPago.SelectedItem as MetodoPago;

        private bool EsEfectivo => MetodoSeleccionado is MetodoPago m && MetodoPagoStrategyFactory.EsEfectivo(m);

        private decimal SubtotalCarrito => carrito.Sum(l => l.Subtotal);

        /// <summary>Recalcula y muestra todos los importes. Es la única fuente de los totales en pantalla.</summary>
        private void Recalcular()
        {
            var metodo = MetodoSeleccionado;
            bool efectivo = EsEfectivo;

            // Primero sin monto recibido para conocer el total y proponerlo como "pago exacto".
            var previo = CalculadoraVenta.Calcular(SubtotalCarrito, numDescuento.Value, metodo, null, ConfiguracionVentas.TasaIVA);
            if (!efectivo || !montoRecibidoEditado)
                FijarMontoRecibido(previo.Total);

            calculo = CalculadoraVenta.Calcular(SubtotalCarrito, numDescuento.Value, metodo, numMontoRecibido.Value, ConfiguracionVentas.TasaIVA);

            lblSubtotal.Text = Moneda(calculo.Subtotal);
            lblDescuento.Text = calculo.Descuento == 0 ? Moneda(0) : "-" + Moneda(calculo.Descuento);
            lblAjuste.Text = (calculo.AjusteMedioPago > 0 ? "+" : "") + Moneda(calculo.AjusteMedioPago);
            lblAjusteTitulo.Text = calculo.AjusteMedioPago switch
            {
                > 0 => "Recargo medio de pago",
                < 0 => "Descuento medio de pago",
                _ => "Recargo/desc. medio pago",
            };
            lblIVA.Text = Moneda(calculo.IVA);
            lblTotal.Text = Moneda(calculo.Total);

            numMontoRecibido.Enabled = efectivo && !procesando;
            lblRecibidoTitulo.Text = efectivo ? "Monto recibido (F6)" : "Monto a cobrar";
            if (calculo.PagoInsuficiente && carrito.Count > 0)
            {
                lblVueltoTitulo.Text = "Falta";
                lblVuelto.Text = Moneda(calculo.Total - calculo.Recibido);
                lblVuelto.ForeColor = Color.Gold;
            }
            else
            {
                lblVueltoTitulo.Text = "Vuelto";
                lblVuelto.Text = Moneda(calculo.Vuelto);
                lblVuelto.ForeColor = Color.White;
            }

            btnRegistrar.Enabled = !procesando && carrito.Count > 0;
        }

        private void FijarMontoRecibido(decimal valor)
        {
            actualizandoMonto = true;
            numMontoRecibido.Value = Math.Clamp(valor, numMontoRecibido.Minimum, numMontoRecibido.Maximum);
            actualizandoMonto = false;
        }

        private static string Moneda(decimal valor) => $"${valor:N2}";

        private async Task GestionarMediosPagoAsync()
        {
            using (var frm = new FrmMetodosPago())
                frm.ShowDialog(this);

            try { await CargarMetodosPagoAsync(); }
            catch (Exception ex) { MostrarError("No se pudieron recargar los medios de pago.", ex); }
        }

        #endregion

        #region Registrar, imprimir, cancelar

        /// <summary>Validaciones de pantalla. Las mismas reglas se vuelven a verificar en el servicio contra la base.</summary>
        private bool ValidarAntesDeRegistrar()
        {
            if (dgvCarrito.IsCurrentCellInEditMode && !dgvCarrito.EndEdit())
                return false;

            if (carrito.Count == 0)
                return Avisar("Agregue al menos un libro a la venta.", txtBuscar);

            if (clienteActual == null)
                return Avisar("Seleccione un cliente o use \"Consumidor final\".", cbCliente);

            if (MetodoSeleccionado == null)
                return Avisar("Seleccione un medio de pago.", cbMetodoPago);

            var excedida = carrito.FirstOrDefault(l => l.Cantidad > l.StockMaximo);
            if (excedida != null)
                return Avisar($"La cantidad de '{excedida.Producto}' supera el stock disponible ({excedida.StockMaximo}).", dgvCarrito);

            Recalcular();
            if (calculo.PagoInsuficiente)
                return Avisar($"El monto recibido ({Moneda(calculo.Recibido)}) no cubre el total ({Moneda(calculo.Total)}).", numMontoRecibido);

            return true;

            bool Avisar(string mensaje, Control foco)
            {
                MessageBox.Show(mensaje, "Revise la venta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                foco.Focus();
                return false;
            }
        }

        private async Task RegistrarVentaAsync()
        {
            // Guard contra doble clic / doble F5: el botón se deshabilita, pero el flag cubre también el atajo.
            if (procesando || !ValidarAntesDeRegistrar()) return;

            var cliente = clienteActual!;
            var metodo = MetodoSeleccionado!;
            var resumen = calculo;

            var confirmacion = MessageBox.Show(
                $"Cliente: {cliente}\n" +
                $"Medio de pago: {metodo.MP_Nombre}\n" +
                $"Artículos: {carrito.Sum(l => l.Cantidad)}\n\n" +
                $"TOTAL: {Moneda(resumen.Total)}" +
                (EsEfectivo ? $"\nRecibido: {Moneda(resumen.Recibido)}\nVuelto: {Moneda(resumen.Vuelto)}" : "") +
                "\n\n¿Registrar la venta?",
                "Confirmar venta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes) return;

            var solicitud = new SolicitudVenta
            {
                ClienteId = cliente.EsConsumidorFinal ? null : cliente.Cliente.CLIDTO_ID,
                MetodoPagoId = metodo.MP_ID,
                PorcentajeDescuento = numDescuento.Value,
                MontoRecibido = EsEfectivo ? numMontoRecibido.Value : null,
                Usuario = UsuarioActual,
                Items = carrito.Select(l => new ItemVentaSolicitud(l.LibroId, l.Cantidad)).ToList(),
            };

            SetProcesando(true);
            try
            {
                var resultado = await FacadeVentas.Instancia.RegistrarVentaAsync(solicitud, resumen.Total, cts.Token);

                if (!resultado.Exito)
                {
                    MessageBox.Show(resultado.Mensaje + "\n\nLa venta NO se registró.", "Venta no registrada",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    await CargarCatalogoAsync();   // stock y precios frescos para corregir el carrito
                    Recalcular();
                    return;
                }

                ultimaVentaId = resultado.VentaId;
                var final = resultado.Calculo ?? resumen;
                lblUltimaVenta.Text = $"✔ {resultado.Comprobante} registrada · Total {Moneda(final.Total)}" +
                                      (final.Vuelto > 0 ? $" · Vuelto {Moneda(final.Vuelto)}" : "");

                LimpiarVenta();
                await CargarCatalogoAsync();

                var imprimir = MessageBox.Show(
                    $"Venta {resultado.Comprobante} registrada.\n" +
                    (final.Vuelto > 0 ? $"\nVUELTO: {Moneda(final.Vuelto)}\n" : "") +
                    "\n¿Imprimir el ticket?",
                    "Venta registrada", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (imprimir == DialogResult.Yes)
                    await ImprimirUltimoTicketAsync();
            }
            catch (OperationCanceledException)
            {
                // El formulario se está cerrando.
            }
            catch (Exception ex)
            {
                MostrarError("No se pudo registrar la venta. No se guardó ningún cambio; puede reintentar.", ex);
            }
            finally
            {
                SetProcesando(false);
                txtBuscar.Focus();
            }
        }

        private async Task ImprimirUltimoTicketAsync()
        {
            if (ultimaVentaId is not int id)
            {
                MessageBox.Show("Todavía no se registró ninguna venta en esta sesión.", "Imprimir ticket",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var ticket = await ControladoraVentas.Instancia.GenerarTicketAsync(id, cts.Token);
                if (ticket == null) return;
                using var frmTicket = new FrmTickets(ticket);
                frmTicket.ShowDialog(this);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                MostrarError("La venta está registrada, pero no se pudo generar el ticket. Puede reimprimirlo desde Gestionar ventas.", ex);
            }
        }

        /// <summary>Esc: descarta la venta en curso (con confirmación si hay algo cargado).</summary>
        private void CancelarVenta()
        {
            if (procesando) return;
            if (carrito.Count > 0 &&
                MessageBox.Show("¿Descartar la venta en curso?", "Cancelar venta", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            LimpiarVenta();
            MostrarEstado("Venta descartada.", esError: false);
        }

        /// <summary>Deja la pantalla lista para la próxima venta.</summary>
        private void LimpiarVenta()
        {
            carrito.Clear();
            libroSeleccionado = null;
            lblSeleccion.Text = string.Empty;
            txtBuscar.Clear();
            numCantidad.Value = 1;
            numDescuento.Value = 0;
            montoRecibidoEditado = false;
            EstablecerCliente(consumidorFinal);
            OcultarSugerencias();
            ActualizarEncabezado();
            Recalcular();
            txtBuscar.Focus();
        }

        /// <summary>Bloquea la pantalla mientras se registra, para evitar doble cobro o cambios a mitad de la operación.</summary>
        private void SetProcesando(bool valor)
        {
            procesando = valor;
            UseWaitCursor = valor;
            panelEncabezado.Enabled = !valor;
            panelCarga.Enabled = !valor;
            dgvCarrito.Enabled = !valor;
            numDescuento.Enabled = !valor && PermisoService.Instancia.TienePermiso("AplicarDescuento");
            cbMetodoPago.Enabled = !valor;
            btnMediosPago.Enabled = !valor;
            btnCancelar.Enabled = !valor;
            btnImprimir.Enabled = !valor && ultimaVentaId != null;
            btnRegistrar.Enabled = !valor && carrito.Count > 0;
            btnRegistrar.Text = valor ? "Registrando..." : "Registrar venta (F5)";
            numMontoRecibido.Enabled = !valor && EsEfectivo;
        }

        #endregion

        #region Atajos de teclado, navegación y cierre

        /// <summary>Atajos globales: funcionan con el foco en cualquier control del formulario.</summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (procesando)
                return keyData == Keys.Escape || base.ProcessCmdKey(ref msg, keyData);

            switch (keyData)
            {
                case Keys.F2:
                    txtBuscar.Focus();
                    txtBuscar.SelectAll();
                    return true;
                case Keys.F3:
                    cbCliente.Focus();
                    cbCliente.SelectAll();
                    return true;
                case Keys.F4:
                    cbMetodoPago.Focus();
                    cbMetodoPago.DroppedDown = true;
                    return true;
                case Keys.F5:
                    btnRegistrar.PerformClick();
                    return true;
                case Keys.F6:
                    if (numMontoRecibido.Enabled) numMontoRecibido.Focus();
                    return true;
                case Keys.F8:
                    btnImprimir.PerformClick();
                    return true;
                case Keys.Escape:
                    // Esc cierra primero lo "pequeño": sugerencias, edición de celda, combo abierto.
                    if (lstSugerencias.Visible) { OcultarSugerencias(); txtBuscar.Focus(); return true; }
                    if (dgvCarrito.IsCurrentCellInEditMode || cbMetodoPago.DroppedDown || cbCliente.DroppedDown)
                        return base.ProcessCmdKey(ref msg, keyData);
                    CancelarVenta();
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Volver()
        {
            if (procesando) return;
            if (carrito.Count > 0 &&
                MessageBox.Show("Hay una venta en curso. ¿Desea descartarla y salir?", "Venta en curso",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            if (Application.OpenForms["FrmMenu"] is FrmMenu principal)
                principal.MostrarGestionarVentas();

            Close();
        }

        private void FrmRealizarVenta_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // No se permite cerrar mientras se está registrando una venta.
            if (procesando)
            {
                e.Cancel = true;
                return;
            }
            timerBusqueda.Stop();
            timerReloj.Stop();
            cts.Cancel();
        }

        private void MostrarError(string mensaje, Exception ex) =>
            MessageBox.Show($"{mensaje}\n\nDetalle técnico: {ex.GetBaseException().Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

        #endregion
    }
}
