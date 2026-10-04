using Controladora;
using Modelo.Seguridad;
using Servicios;
using System.Runtime.InteropServices;
using Vista.Comun;
using Vista.Seguridad;
using Vista.Theme;

namespace Vista
{
    /// <summary>
    /// Shell de la aplicación: menú lateral y un panel central donde vive <b>una sola sección a la vez</b>.
    /// <para>
    /// Regla de diseño: listados, tableros y reportes se embeben con <see cref="Navegar{T}"/>; altas, ediciones,
    /// cobros y confirmaciones se abren desde la sección con <c>ShowDialog(this)</c> dentro de un <c>using</c>.
    /// </para>
    /// <para>
    /// Ciclo de vida: al cambiar de sección la anterior se cierra con <c>Close()</c>, que dispara su
    /// <c>FormClosing</c> (cambios sin guardar, operación en curso) y, si no lo cancela, la libera con
    /// <c>Dispose()</c>: se detienen sus timers, se cancelan sus cargas y se suelta su ventana.
    /// </para>
    /// </summary>
    public partial class FrmMenu : Form
    {
        /// <param name="Permiso">Formulario que el usuario debe tener habilitado; null = acceso libre.</param>
        /// <param name="Titulo">Título de la barra superior; null = el <c>Text</c> del formulario.</param>
        private sealed record Seccion(Button Boton, string? Permiso, string? Titulo = null);

        private readonly Dictionary<Type, Seccion> secciones;
        private Form? seccionActual;

        /// <summary>true si el usuario cerró la aplicación; false si solo cerró la sesión.</summary>
        public bool SalirDeLaAplicacion { get; private set; }

        public FrmMenu()
        {
            InitializeComponent();
            DoubleBuffered = true;

            secciones = new()
            {
                [typeof(FrmHome)] = new(btnInicio, null, "INICIO"),
                [typeof(FrmGestionarClientes)] = new(btnClientes, nameof(FrmGestionarClientes)),
                [typeof(FrmGestionarLibros)] = new(btnLibros, nameof(FrmGestionarLibros)),
                [typeof(FrmGestionarProveedores)] = new(btnProveedores, nameof(FrmGestionarProveedores)),
                [typeof(FrmGestionarVentas)] = new(btnVentas, nameof(FrmGestionarVentas), "VENTAS"),
                [typeof(FrmRealizarVenta)] = new(btnVentas, nameof(FrmGestionarVentas)),
                [typeof(FrmGestionarReportes)] = new(btnReportes, nameof(FrmGestionarReportes)),
                [typeof(FrmGestionarInventario)] = new(btnGestionarInventario, nameof(FrmGestionarInventario)),
                [typeof(FrmGestionarUsuarios)] = new(btnUsuarios, nameof(FrmGestionarUsuarios)),
                [typeof(FrmGestionarGrupos)] = new(btnGrupos, nameof(FrmGestionarGrupos)),
            };

            FormClosing += FrmMenu_FormClosing;
        }

        #region Navegación

        /// <summary>
        /// Muestra la sección <typeparamref name="T"/> en el panel central. Si ya está abierta no la recrea.
        /// Devuelve false si no hay permiso, si la sección actual canceló su cierre o si la nueva no pudo crearse.
        /// </summary>
        public bool Navegar<T>() where T : Form, new()
        {
            if (seccionActual is T abierta)
            {
                abierta.Focus();
                return true;
            }

            secciones.TryGetValue(typeof(T), out var seccion);
            if (seccion?.Permiso != null && !PermisoService.Instancia.PuedeAccederFormulario(seccion.Permiso))
            {
                MessageBox.Show(this, "No tiene permisos para acceder a esta sección.", "Acceso denegado",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!CerrarSeccionActual())
                return false;

            T? formulario = null;
            SuspenderRedibujado(panelForm);
            try
            {
                formulario = new T
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };
                panelForm.Controls.Add(formulario);
                // Se tematiza ya emparentado y antes de mostrarse: así no se repinta después en Application.Idle.
                FuturisticTheme.ApplyToForm(formulario);
                formulario.FormClosed += Seccion_FormClosed;
                seccionActual = formulario;
                formulario.Show();
            }
            catch (Exception ex)
            {
                seccionActual = null;
                formulario?.Dispose();
                ReanudarRedibujado(panelForm);
                ManejadorErrores.Mostrar(this, ex, "No se pudo abrir la sección.");
                return false;
            }
            ReanudarRedibujado(panelForm);

            lblTitulo.Text = seccion?.Titulo ?? formulario.Text;
            FuturisticTheme.SeleccionarBotonMenu(panelMenu, seccion?.Boton);
            return true;
        }

        public void MostrarInicio() => Navegar<FrmHome>();

        /// <summary>Cierra la sección activa. Devuelve false si ella canceló el cierre (cambios sin guardar).</summary>
        private bool CerrarSeccionActual()
        {
            if (seccionActual == null) return true;

            var anterior = seccionActual;
            anterior.Close();   // FormClosing puede cancelarlo; si no, Close() la libera (Dispose)
            if (!anterior.IsDisposed) return false;

            seccionActual = null;
            return true;
        }

        /// <summary>La sección se cerró sola (o la cerramos nosotros): dejamos de referenciarla.</summary>
        private void Seccion_FormClosed(object? sender, FormClosedEventArgs e)
        {
            if (sender is Form f)
            {
                f.FormClosed -= Seccion_FormClosed;
                if (ReferenceEquals(f, seccionActual)) seccionActual = null;
            }
        }

        #endregion

        #region Anti-parpadeo

        private const int WM_SETREDRAW = 0x000B;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        // Congela el pintado del panel mientras se cambia la sección: se ve el cambio de una sola vez,
        // sin el fondo vacío ni el formulario a medio dibujar.
        private static void SuspenderRedibujado(Control c) => SendMessage(c.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);

        private static void ReanudarRedibujado(Control c)
        {
            SendMessage(c.Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
            c.Invalidate(true);
            c.Update();
        }

        #endregion

        #region Ciclo de vida y sesión

        private void FrmMenu_Load(object sender, EventArgs e)
        {
            AplicarSeguridad();
            FuturisticTheme.ApplyToForm(this);
            lblTitulo.ForeColor = Color.White;
            MostrarInicio();
        }

        private void AplicarSeguridad()
        {
            foreach (var s in secciones.Values)
                if (s.Permiso != null)
                    s.Boton.Visible = PermisoService.Instancia.PuedeAccederFormulario(s.Permiso);
        }

        private void FrmMenu_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // La sección activa puede tener cambios sin guardar o una venta registrándose.
            if (!CerrarSeccionActual())
            {
                e.Cancel = true;
                SalirDeLaAplicacion = false;
                return;
            }

            ControladoraSesiones.Instancia.RegistrarLogout();
            PermisoService.Instancia.Logout();
            Sesion.Instancia.Usuario = null;
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "¿Desea cerrar la sesión?", "Cerrar sesión",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Close();   // FrmIniciarSesión vuelve a mostrarse al terminar el ShowDialog
        }

        private void pbCerrar_Click(object sender, EventArgs e)
        {
            SalirDeLaAplicacion = true;
            Close();
        }

        #endregion

        #region Menú

        private void btnInicio_Click(object sender, EventArgs e) => MostrarInicio();
        private void btnClientes_Click(object sender, EventArgs e) => Navegar<FrmGestionarClientes>();
        private void btnLibros_Click(object sender, EventArgs e) => Navegar<FrmGestionarLibros>();
        private void btnProveedores_Click(object sender, EventArgs e) => Navegar<FrmGestionarProveedores>();
        private void btnVentas_Click(object sender, EventArgs e) => Navegar<FrmGestionarVentas>();
        private void btnReportes_Click(object sender, EventArgs e) => Navegar<FrmGestionarReportes>();
        private void btnGestionarInventario_Click(object sender, EventArgs e) => Navegar<FrmGestionarInventario>();
        private void btnUsuarios_Click(object sender, EventArgs e) => Navegar<FrmGestionarUsuarios>();
        private void btnGrupos_Click(object sender, EventArgs e) => Navegar<FrmGestionarGrupos>();

        #endregion
    }
}
