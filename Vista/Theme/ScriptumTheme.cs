using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Vista.Properties;

namespace Vista.Theme
{
    /// <summary>
    /// Identidad visual de SCRIPTUM: tema claro "tinta y oro" (fondo gris perla, tarjetas blancas, menú lateral
    /// azul tinta y acento ámbar tomado del logo). Toda la paleta, la tipografía y el estilo de cada tipo de control
    /// viven acá; los formularios no definen colores propios salvo los semánticos (estado de stock, faltantes, etc.),
    /// que también se toman de esta clase.
    /// <para>
    /// Se aplica solo: <see cref="Initialize"/> tematiza cada formulario que se abre (Application.Idle) y
    /// <see cref="ApplyToForm"/> lo hace antes de mostrar las secciones embebidas en el menú. Los controles que se
    /// agregan después (botones del menú creados por código, tarjetas de reportes) se tematizan al agregarse.
    /// </para>
    /// <para>
    /// Roles opcionales por <c>Tag</c>: "Sidebar" (panel oscuro de navegación o marca), "Encabezado" (barra superior
    /// blanca), "Titulo" (título de sección), "Primario" / "Secundario" / "Peligro" / "Neutro" (forzar el tipo de botón).
    /// Sin Tag, el tipo de botón se deduce de su texto (Guardar → primario, Volver → secundario, Anular → peligro).
    /// </para>
    /// </summary>
    internal static class ScriptumTheme
    {
        #region Paleta

        public static readonly Color Fondo = ColorTranslator.FromHtml("#F4F6F9");
        public static readonly Color Superficie = Color.White;
        public static readonly Color SuperficieSuave = ColorTranslator.FromHtml("#F8FAFC");
        public static readonly Color Borde = ColorTranslator.FromHtml("#E2E8F0");
        public static readonly Color BordeFuerte = ColorTranslator.FromHtml("#CBD5E1");

        public static readonly Color Sidebar = ColorTranslator.FromHtml("#0F172A");
        public static readonly Color SidebarHover = ColorTranslator.FromHtml("#1E293B");
        public static readonly Color SidebarActivo = ColorTranslator.FromHtml("#1E293B");
        public static readonly Color SidebarTexto = ColorTranslator.FromHtml("#CBD5E1");
        public static readonly Color SidebarTextoActivo = ColorTranslator.FromHtml("#F8FAFC");
        public static readonly Color SidebarTextoSuave = ColorTranslator.FromHtml("#64748B");

        /// <summary>Ámbar del logo: sección activa, foco y detalles.</summary>
        public static readonly Color Acento = ColorTranslator.FromHtml("#D97706");
        public static readonly Color Primario = ColorTranslator.FromHtml("#2563EB");
        public static readonly Color PrimarioHover = ColorTranslator.FromHtml("#1D4ED8");
        public static readonly Color PrimarioPresionado = ColorTranslator.FromHtml("#1E40AF");
        public static readonly Color Secundario = ColorTranslator.FromHtml("#E2E8F0");
        public static readonly Color SecundarioHover = ColorTranslator.FromHtml("#CBD5E1");
        public static readonly Color SecundarioTexto = ColorTranslator.FromHtml("#334155");
        public static readonly Color Peligro = ColorTranslator.FromHtml("#EF4444");
        public static readonly Color PeligroHover = ColorTranslator.FromHtml("#DC2626");
        public static readonly Color NeutroHover = ColorTranslator.FromHtml("#F1F5F9");

        public static readonly Color TextoTitulo = ColorTranslator.FromHtml("#0F172A");
        public static readonly Color TextoSubtitulo = ColorTranslator.FromHtml("#334155");
        public static readonly Color Texto = ColorTranslator.FromHtml("#475569");
        public static readonly Color TextoSuave = ColorTranslator.FromHtml("#64748B");
        public static readonly Color TextoInactivo = ColorTranslator.FromHtml("#94A3B8");

        // Semánticos (legibles sobre blanco).
        public static readonly Color Exito = ColorTranslator.FromHtml("#15803D");
        public static readonly Color Advertencia = ColorTranslator.FromHtml("#B45309");
        public static readonly Color Error = ColorTranslator.FromHtml("#DC2626");
        public static readonly Color ExitoSuave = ColorTranslator.FromHtml("#DCFCE7");
        public static readonly Color AdvertenciaSuave = ColorTranslator.FromHtml("#FEF3C7");
        public static readonly Color ErrorSuave = ColorTranslator.FromHtml("#FEE2E2");
        public static readonly Color InfoSuave = ColorTranslator.FromHtml("#DBEAFE");

        public static readonly Color GrillaEncabezado = ColorTranslator.FromHtml("#F1F5F9");
        public static readonly Color GrillaAlterna = SuperficieSuave;
        public static readonly Color GrillaSeleccion = ColorTranslator.FromHtml("#DBEAFE");

        #endregion

        #region Tipografía

        public const string Familia = "Segoe UI";
        public static readonly Font FuenteTitulo = new(Familia, 17F, FontStyle.Bold);
        public static readonly Font FuenteSubtitulo = new("Segoe UI Semibold", 11F);
        public static readonly Font FuenteTexto = new(Familia, 9.75F);
        public static readonly Font FuenteTextoNegrita = new(Familia, 9.75F, FontStyle.Bold);
        public static readonly Font FuenteBoton = new("Segoe UI Semibold", 9.75F);
        public static readonly Font FuenteBotonPrincipal = new(Familia, 9.75F, FontStyle.Bold);
        public static readonly Font FuenteMenu = new("Segoe UI Semibold", 10.5F);
        private static readonly Font FuenteIconos = CrearFuenteIconos();

        private static Font CrearFuenteIconos()
        {
            foreach (var nombre in new[] { "Segoe Fluent Icons", "Segoe MDL2 Assets" })
            {
                using var prueba = new Font(nombre, 12F);
                if (prueba.Name == nombre) return new Font(nombre, 12F);
            }
            return new Font(Familia, 12F);
        }

        // Las fuentes derivadas se cachean: el tema las pide para cada control.
        private static readonly Dictionary<(string, float, FontStyle), Font> fuentes = new();
        private static Font ObtenerFuente(string familia, float tamaño, FontStyle estilo)
        {
            var clave = (familia, tamaño, estilo);
            if (!fuentes.TryGetValue(clave, out var f))
                fuentes[clave] = f = new Font(familia, tamaño, estilo);
            return f;
        }

        /// <summary>Lleva la fuente a Segoe UI; el texto de 9 pt regular sube a 9.75 pt (cuerpo del sistema).</summary>
        private static Font Normalizar(Font f, bool agrandarCuerpo)
        {
            string familia = f.FontFamily.Name.StartsWith(Familia, StringComparison.OrdinalIgnoreCase) ? f.FontFamily.Name : Familia;
            float tamaño = agrandarCuerpo && f.Size < 9.5F && f.Size >= 8.5F ? 9.75F : f.Size;
            if (familia == f.FontFamily.Name && tamaño == f.Size) return f;
            return ObtenerFuente(familia, tamaño, f.Style);
        }

        #endregion

        #region Aplicación

        private static readonly HashSet<Form> formulariosTematizados = new();
        private static readonly ConditionalWeakTable<Control, object> contenedoresObservados = new();

        private static readonly Lazy<Icon?> IconoApp = new(() =>
        {
            try { return Icon.FromHandle(Resources.icoScriptum.GetHicon()); }
            catch { return null; }
        });

        public static void Initialize()
        {
            AplicarAFormulariosAbiertos();
            Application.Idle -= Application_Idle;
            Application.Idle += Application_Idle;
            // Un diálogo modal se tematiza al entrar en su bucle, antes de pintarse por primera vez.
            Application.EnterThreadModal -= Application_Idle;
            Application.EnterThreadModal += Application_Idle;
        }

        private static void Application_Idle(object? sender, EventArgs e) => AplicarAFormulariosAbiertos();

        private static void AplicarAFormulariosAbiertos()
        {
            foreach (Form f in Application.OpenForms)
            {
                if (formulariosTematizados.Contains(f)) continue;
                try { ApplyToForm(f); }
                catch { /* un error de estilo no puede romper la pantalla */ }
            }
            formulariosTematizados.RemoveWhere(f => f.IsDisposed);
        }

        public static void ApplyToForm(Form form)
        {
            ArgumentNullException.ThrowIfNull(form);
            if (!formulariosTematizados.Add(form)) return;

            form.BackColor = Fondo;
            form.ForeColor = Texto;
            if (form.TopLevel && IconoApp.Value != null)
                form.Icon = IconoApp.Value;

            ObservarContenedor(form);
            AplicarAColeccion(form.Controls);
            ReacomodarBotones(form);
        }

        private static void AplicarAColeccion(Control.ControlCollection controles)
        {
            foreach (Control c in controles)
                AplicarRecursivo(c);
        }

        private static void AplicarRecursivo(Control c)
        {
            AplicarAControl(c);
            if (c is DataGridView or ComboBox or NumericUpDown or DateTimePicker or TextBoxBase) return;
            ObservarContenedor(c);
            if (c.HasChildren)
            {
                AplicarAColeccion(c.Controls);
                ReacomodarBotones(c);
            }
        }

        /// <summary>Lo que se agregue más adelante a este contenedor también se tematiza.</summary>
        private static void ObservarContenedor(Control c)
        {
            if (contenedoresObservados.TryGetValue(c, out _)) return;
            contenedoresObservados.Add(c, new object());
            c.ControlAdded += (_, e) =>
            {
                if (e.Control is null) return;
                try { AplicarRecursivo(e.Control); } catch { }
            };
        }

        private static void AplicarAControl(Control control)
        {
            switch (control)
            {
                case SplitContainer sc:
                    sc.BackColor = FondoEfectivo(sc.Parent);
                    break;
                case TabPage tp:
                    tp.UseVisualStyleBackColor = false;
                    tp.BackColor = Fondo;
                    tp.ForeColor = Texto;
                    break;
                case Panel p:
                    AplicarPanel(p);
                    break;
                case TabControl tc:
                    AplicarTabControl(tc);
                    break;
                case GroupBox gb:
                    AplicarGroupBox(gb);
                    break;
                case Button b:
                    AplicarBoton(b);
                    break;
                case CheckBox or RadioButton:
                    control.BackColor = Color.Transparent;
                    if (EsColorNeutro(control.ForeColor)) control.ForeColor = EnSidebar(control) ? SidebarTexto : TextoSubtitulo;
                    control.Font = Normalizar(control.Font, HayLugarParaAgrandar(control));
                    control.Cursor = Cursors.Hand;
                    break;
                case LinkLabel ll:
                    ll.BackColor = Color.Transparent;
                    ll.LinkColor = ll.ActiveLinkColor = Primario;
                    ll.VisitedLinkColor = Primario;
                    break;
                case Label l:
                    AplicarLabel(l);
                    break;
                case RichTextBox rtb:
                    rtb.BackColor = Superficie;
                    rtb.BorderStyle = BorderStyle.None;
                    break;
                case TextBox t:
                    t.BorderStyle = BorderStyle.FixedSingle;
                    t.BackColor = t.ReadOnly ? GrillaEncabezado : Superficie;
                    t.ForeColor = TextoTitulo;
                    t.Font = Normalizar(t.Font, true);
                    break;
                case ComboBox cb:
                    AplicarComboBox(cb);
                    break;
                case NumericUpDown n:
                    n.BorderStyle = BorderStyle.FixedSingle;
                    n.BackColor = Superficie;
                    n.ForeColor = TextoTitulo;
                    n.Font = Normalizar(n.Font, true);
                    break;
                case DateTimePicker dtp:
                    dtp.Font = Normalizar(dtp.Font, true);
                    dtp.CalendarTitleBackColor = Sidebar;
                    dtp.CalendarTitleForeColor = SidebarTextoActivo;
                    dtp.CalendarMonthBackground = Superficie;
                    break;
                case TreeView tv:
                    tv.BorderStyle = BorderStyle.FixedSingle;
                    tv.BackColor = Superficie;
                    tv.ForeColor = TextoTitulo;
                    tv.LineColor = BordeFuerte;
                    tv.Font = Normalizar(tv.Font, true);
                    break;
                case ListBox lb:   // incluye CheckedListBox
                    lb.BorderStyle = BorderStyle.FixedSingle;
                    lb.BackColor = Superficie;
                    lb.ForeColor = TextoTitulo;
                    lb.Font = Normalizar(lb.Font, true);
                    break;
                case DataGridView dgv:
                    AplicarGrilla(dgv);
                    break;
                case PictureBox pic:
                    AplicarPictureBox(pic);
                    break;
            }
        }

        #endregion

        #region Contenedores

        private static string RolDe(Control c) => c.Tag as string ?? string.Empty;

        private static bool EsSidebar(Control c) =>
            RolDe(c) == "Sidebar" || string.Equals(c.Name, "panelMenu", StringComparison.Ordinal);

        private static bool EnSidebar(Control c)
        {
            for (var p = c.Parent; p != null; p = p.Parent)
                if (EsSidebar(p)) return true;
            return false;
        }

        /// <summary>Primer fondo no transparente desde <paramref name="c"/> hacia arriba.</summary>
        public static Color FondoEfectivo(Control? c)
        {
            for (; c != null; c = c.Parent)
                if (c.BackColor.A == 255) return c.BackColor;
            return Fondo;
        }

        /// <summary>true si el fondo real de <paramref name="contenedor"/> es oscuro (texto claro encima).</summary>
        public static bool EsFondoOscuro(Control? contenedor)
        {
            var f = FondoEfectivo(contenedor);
            return 0.299 * f.R + 0.587 * f.G + 0.114 * f.B < 140;
        }

        private static void AplicarPanel(Panel p)
        {
            p.ForeColor = Texto;

            if (EsSidebar(p) || EnSidebar(p))
            {
                p.BackColor = Sidebar;
                return;
            }

            if (RolDe(p) == "Encabezado")
            {
                p.BackColor = Superficie;
                PintarBorde(p, DockStyle.Top);
                return;
            }

            // Paneles de distribución: dejan ver el fondo de su contenedor.
            if (p is SplitterPanel || p is TableLayoutPanel || p is FlowLayoutPanel || p.Name == "panelForm")
            {
                p.BackColor = FondoEfectivo(p.Parent);
                return;
            }

            // Un panel apoyado sobre el fondo de la ventana es una tarjeta blanca con borde sutil;
            // uno dentro de otra tarjeta comparte su color.
            var fondoPadre = FondoEfectivo(p.Parent);
            if (fondoPadre.ToArgb() == Fondo.ToArgb())
            {
                p.BackColor = Superficie;
                PintarBorde(p, p.Dock);
            }
            else
            {
                p.BackColor = fondoPadre;
            }
        }

        private static readonly ConditionalWeakTable<Control, object> conBorde = new();

        /// <summary>Línea de 1 px color <see cref="Borde"/>: del lado que toca el resto de la pantalla según el Dock.</summary>
        private static void PintarBorde(Control c, DockStyle dock)
        {
            if (conBorde.TryGetValue(c, out _)) return;
            conBorde.Add(c, new object());
            c.Paint += (s, e) =>
            {
                var ctl = (Control)s!;
                using var lapiz = new Pen(Borde);
                int w = ctl.ClientSize.Width - 1, h = ctl.ClientSize.Height - 1;
                switch (ctl.Dock)
                {
                    case DockStyle.Top: e.Graphics.DrawLine(lapiz, 0, h, w, h); break;
                    case DockStyle.Bottom: e.Graphics.DrawLine(lapiz, 0, 0, w, 0); break;
                    case DockStyle.Left: e.Graphics.DrawLine(lapiz, w, 0, w, h); break;
                    case DockStyle.Right: e.Graphics.DrawLine(lapiz, 0, 0, 0, h); break;
                    default: e.Graphics.DrawRectangle(lapiz, 0, 0, w, h); break;
                }
            };
            c.Resize += (s, _) => ((Control)s!).Invalidate();
        }

        private static void AplicarGroupBox(GroupBox gb)
        {
            var fondoPadre = FondoEfectivo(gb.Parent);
            gb.BackColor = fondoPadre.ToArgb() == Fondo.ToArgb() ? Superficie : fondoPadre;
            gb.ForeColor = TextoSubtitulo;
            gb.FlatStyle = FlatStyle.Standard;

            if (conBorde.TryGetValue(gb, out _)) return;
            conBorde.Add(gb, new object());
            gb.Paint += GroupBox_Paint;
            gb.Resize += (s, _) => ((Control)s!).Invalidate();
        }

        /// <summary>Repinta el marco del GroupBox: borde redondeado claro y título semibold.</summary>
        private static void GroupBox_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is not GroupBox gb) return;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(FondoEfectivo(gb.Parent));

            var fuente = FuenteSubtitulo;
            var tamañoTitulo = string.IsNullOrEmpty(gb.Text) ? Size.Empty : TextRenderer.MeasureText(g, gb.Text, fuente);
            int arriba = Math.Max(tamañoTitulo.Height / 2, 1);
            var marco = new RectangleF(0.5f, arriba + 0.5f, gb.Width - 1.5f, gb.Height - arriba - 1.5f);
            using (var path = Redondeado(marco, 8))
            {
                using var relleno = new SolidBrush(gb.BackColor);
                g.FillPath(relleno, path);
                using var lapiz = new Pen(Borde);
                g.DrawPath(lapiz, path);
            }

            if (tamañoTitulo != Size.Empty)
            {
                var rectTitulo = new Rectangle(12, 0, tamañoTitulo.Width + 4, tamañoTitulo.Height);
                using var fondoTitulo = new SolidBrush(gb.BackColor);
                g.FillRectangle(fondoTitulo, rectTitulo);
                TextRenderer.DrawText(g, gb.Text, fuente, rectTitulo, gb.Enabled ? TextoSubtitulo : TextoInactivo,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.NoPadding);
            }
        }

        private static void AplicarTabControl(TabControl tc)
        {
            if (tc.DrawMode == TabDrawMode.OwnerDrawFixed) return;
            tc.DrawMode = TabDrawMode.OwnerDrawFixed;
            tc.Padding = new Point(18, 7);
            tc.Font = Normalizar(tc.Font, true);
            tc.Cursor = Cursors.Hand;
            tc.DrawItem += (s, e) =>
            {
                var t = (TabControl)s!;
                bool sel = e.Index == t.SelectedIndex;
                var r = e.Bounds;
                using (var fondo = new SolidBrush(sel ? Superficie : Fondo))
                    e.Graphics.FillRectangle(fondo, r);
                if (sel)
                {
                    using var barra = new SolidBrush(Acento);
                    e.Graphics.FillRectangle(barra, r.X, r.Bottom - 3, r.Width, 3);
                }
                var fuente = sel ? ObtenerFuente("Segoe UI Semibold", t.Font.Size, FontStyle.Regular) : t.Font;
                TextRenderer.DrawText(e.Graphics, t.TabPages[e.Index].Text, fuente, r, sel ? TextoTitulo : TextoSuave,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
            };
        }

        #endregion

        #region Textos

        private static void AplicarLabel(Label l)
        {
            l.BackColor = Color.Transparent;
            bool autoSize = l.AutoSize && HayLugarParaAgrandar(l);

            if (EnSidebar(l) || EsFondoOscuro(l.Parent))
            {
                if (EsColorNeutro(l.ForeColor)) l.ForeColor = SidebarTexto;
                l.Font = Normalizar(l.Font, autoSize);
                return;
            }

            if (RolDe(l) == "Titulo")
            {
                l.Font = FuenteTitulo;
                l.ForeColor = TextoTitulo;
                FormatoTitulo(l);
                return;
            }

            l.Font = Normalizar(l.Font, autoSize);
            if (!EsColorNeutro(l.ForeColor)) return;   // color semántico puesto a propósito (error, estado)

            var f = l.Font;
            if (f.Size >= 15F) l.ForeColor = TextoTitulo;
            else if (f.Bold || f.Size >= 11F || f.FontFamily.Name.Contains("Semibold")) l.ForeColor = TextoSubtitulo;
            else l.ForeColor = Texto;
        }


        /// <summary>
        /// El cuerpo de texto pasa de 9 a 9.75 pt; en formularios densos eso puede hacer que una etiqueta
        /// autoajustable pise el control de al lado. Si pasaría, la etiqueta conserva su tamaño.
        /// </summary>
        private static bool HayLugarParaAgrandar(Control c)
        {
            if (c.Parent is null || c.Parent is FlowLayoutPanel or TableLayoutPanel || c.Dock != DockStyle.None) return true;
            var nueva = Normalizar(c.Font, true);
            if (ReferenceEquals(nueva, c.Font)) return true;
            var medida = TextRenderer.MeasureText(c.Text ?? string.Empty, nueva);
            var r = new Rectangle(c.Left, c.Top, Math.Max(c.Width, medida.Width + c.Padding.Horizontal),
                                  Math.Max(c.Height, medida.Height + c.Padding.Vertical));
            if (c.Parent.ClientSize.Width > 0 && r.Right > c.Parent.ClientSize.Width) return false;
            foreach (Control otro in c.Parent.Controls)
                if (otro != c && r.IntersectsWith(otro.Bounds) && !c.Bounds.IntersectsWith(otro.Bounds))
                    return false;
            return true;
        }
        /// <summary>Gris, negro o blanco: el color "por defecto" que el tema puede reemplazar.</summary>
        private static bool EsColorNeutro(Color c)
        {
            int max = Math.Max(c.R, Math.Max(c.G, c.B)), min = Math.Min(c.R, Math.Min(c.G, c.B));
            if (max - min <= 28) return true;
            // Colores del tema anterior (azul pizarra) y SystemColors.ControlText/HotTrack.
            int[] anteriores = { 0x4C6378, 0x34495E, 0x6C8399, 0x1A2F4C, 0x2D3748 };
            int rgb = c.ToArgb() & 0xFFFFFF;
            return anteriores.Contains(rgb);
        }

        private static readonly ConditionalWeakTable<Label, object> titulosObservados = new();

        /// <summary>Los títulos de sección se muestran en formato oración ("HISTORIAL DE CAJAS" → "Historial de cajas").</summary>
        private static void FormatoTitulo(Label l)
        {
            Formatear(l);
            if (titulosObservados.TryGetValue(l, out _)) return;
            titulosObservados.Add(l, new object());
            l.TextChanged += (s, _) => Formatear((Label)s!);

            static void Formatear(Label l)
            {
                var t = FormatoOracion(l.Text);
                if (t != l.Text) l.Text = t;
            }
        }

        public static string FormatoOracion(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return texto ?? string.Empty;
            if (texto.Any(char.IsLower)) return texto;   // ya viene con mayúsculas y minúsculas
            var minus = texto.ToLower(CultureInfo.CurrentCulture);
            return char.ToUpper(minus[0], CultureInfo.CurrentCulture) + minus[1..];
        }

        #endregion

        #region Botones

        private enum TipoBoton { Primario, Secundario, Peligro, Neutro, Menu }

        private sealed class EstadoBoton
        {
            public TipoBoton Tipo;
            public bool Hover, Presionado, Seleccionado, Inicializado;
        }

        private static readonly ConditionalWeakTable<Button, EstadoBoton> estadosBoton = new();

        private static string SinAcentos(string s)
        {
            var d = s.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(d.Length);
            foreach (var ch in d)
                if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark) sb.Append(ch);
            return sb.ToString().ToLowerInvariant();
        }

        private static TipoBoton Clasificar(Button b)
        {
            switch (RolDe(b))
            {
                case "Primario": return TipoBoton.Primario;
                case "Secundario": return TipoBoton.Secundario;
                case "Peligro": return TipoBoton.Peligro;
                case "Neutro": return TipoBoton.Neutro;
            }
            if (EnSidebar(b)) return TipoBoton.Menu;

            var t = SinAcentos(b.Text ?? string.Empty);
            if (t.Trim().Length <= 2) return TipoBoton.Neutro;   // "+", "…", "◀"

            string[] peligro = { "anular", "dar de baja", "eliminar", "cancelar orden", "descartar", "borrar" };
            string[] primario = { "guardar", "registrar", "aceptar", "iniciar sesion", "abrir caja", "cerrar caja", "nueva venta",
                                  "nuevo", "nueva orden", "generar", "agregar", "emitir", "continuar", "confirmar", "buscar", "cobrar" };
            string[] secundario = { "limpiar", "cancelar", "volver", "salir", "cerrar", "ya la anote", "atras" };

            if (peligro.Any(t.Contains)) return TipoBoton.Peligro;
            if (primario.Any(t.Contains)) return TipoBoton.Primario;
            if (secundario.Any(t.Contains)) return TipoBoton.Secundario;
            return TipoBoton.Neutro;
        }

        private static void AplicarBoton(Button b)
        {
            var st = estadosBoton.GetValue(b, _ => new EstadoBoton());
            st.Tipo = Clasificar(b);

            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.UseVisualStyleBackColor = false;
            b.Cursor = Cursors.Hand;
            ActualizarColoresBase(b, st);

            if (st.Tipo == TipoBoton.Menu)
            {
                b.Font = FuenteMenu;
                b.TextAlign = ContentAlignment.MiddleLeft;
            }
            else
            {
                bool grande = b.Font.Size >= 11F;
                var estilo = st.Tipo is TipoBoton.Primario or TipoBoton.Peligro ? FontStyle.Bold : FontStyle.Regular;
                string familia = estilo == FontStyle.Bold ? Familia : "Segoe UI Semibold";
                b.Font = ObtenerFuente(familia, grande ? b.Font.Size : 9.75F, estilo);
            }

            if (st.Inicializado) { b.Invalidate(); return; }
            st.Inicializado = true;

            // Alto de botón estándar (38 px) centrado en el lugar original; los botones destacados
            // (fuente grande, como "Registrar venta") y los acoplados conservan su tamaño.
            if (st.Tipo != TipoBoton.Menu && b.Dock == DockStyle.None && b.Height > 44 && b.Font.Size < 11F)
            {
                int alto = 38;
                b.Top += (b.Height - alto) / 2;
                b.Height = alto;
            }

            b.Paint += Boton_Paint;
            b.MouseEnter += (s, _) => { if (Estado(s) is { } e) { e.Hover = true; ((Control)s!).Invalidate(); } };
            b.MouseLeave += (s, _) => { if (Estado(s) is { } e) { e.Hover = false; e.Presionado = false; ((Control)s!).Invalidate(); } };
            b.MouseDown += (s, a) => { if (a.Button == MouseButtons.Left && Estado(s) is { } e) { e.Presionado = true; ((Control)s!).Invalidate(); } };
            b.MouseUp += (s, _) => { if (Estado(s) is { } e) { e.Presionado = false; ((Control)s!).Invalidate(); } };
            // Algunos botones cambian de función ("Dar de baja" ↔ "Reactivar", "Abrir caja" ↔ "Cerrar caja").
            b.TextChanged += (s, _) =>
            {
                if (s is Button bt && Estado(bt) is { } e && RolDe(bt) == string.Empty && e.Tipo != TipoBoton.Menu)
                {
                    e.Tipo = Clasificar(bt);
                    ActualizarColoresBase(bt, e);
                    bt.Invalidate();
                }
            };
            b.EnabledChanged += (s, _) => ((Control)s!).Invalidate();
        }

        private static EstadoBoton? Estado(object? s) => s is Button b && estadosBoton.TryGetValue(b, out var e) ? e : null;

        private static void ActualizarColoresBase(Button b, EstadoBoton st)
        {
            var (relleno, _, texto) = Colores(st, false, false);
            b.BackColor = relleno;
            b.ForeColor = texto;
            b.FlatAppearance.MouseOverBackColor = relleno;
            b.FlatAppearance.MouseDownBackColor = relleno;
        }

        private static (Color relleno, Color borde, Color texto) Colores(EstadoBoton st, bool hover, bool presionado) => st.Tipo switch
        {
            TipoBoton.Primario => (presionado ? PrimarioPresionado : hover ? PrimarioHover : Primario, Color.Empty, Color.White),
            TipoBoton.Peligro => (presionado || hover ? PeligroHover : Peligro, Color.Empty, Color.White),
            TipoBoton.Secundario => (presionado || hover ? SecundarioHover : Secundario, Color.Empty, SecundarioTexto),
            TipoBoton.Menu => (st.Seleccionado ? SidebarActivo : hover ? SidebarHover : Sidebar, Color.Empty,
                               st.Seleccionado ? SidebarTextoActivo : SidebarTexto),
            _ => (presionado ? Secundario : hover ? NeutroHover : Superficie, BordeFuerte, SecundarioTexto),
        };

        private static void Boton_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is not Button b || Estado(b) is not { } st) return;
            var g = e.Graphics;
            var fondo = FondoEfectivo(b.Parent);
            var (relleno, borde, texto) = Colores(st, st.Hover, st.Presionado);

            if (st.Tipo == TipoBoton.Menu)
            {
                PintarBotonMenu(b, st, g, relleno, texto);
                return;
            }

            if (!b.Enabled)
            {
                relleno = Mezclar(relleno, fondo, st.Tipo == TipoBoton.Neutro ? 0.3 : 0.55);
                texto = st.Tipo == TipoBoton.Neutro ? TextoInactivo : Mezclar(texto, relleno, 0.35);
                if (!borde.IsEmpty) borde = Borde;
            }

            g.Clear(fondo);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new RectangleF(0.5f, 0.5f, b.Width - 1.5f, b.Height - 1.5f);
            using (var path = Redondeado(rect, 6))
            {
                using var brocha = new SolidBrush(relleno);
                g.FillPath(brocha, path);
                if (!borde.IsEmpty)
                {
                    using var lapiz = new Pen(borde);
                    g.DrawPath(lapiz, path);
                }
            }

            if (b.Focused && b.Enabled && !st.Hover)
            {
                using var foco = Redondeado(RectangleF.Inflate(rect, -2f, -2f), 5);
                using var lapizFoco = new Pen(Color.FromArgb(150, st.Tipo == TipoBoton.Neutro || st.Tipo == TipoBoton.Secundario ? Primario : Color.White), 1.5f);
                g.DrawPath(lapizFoco, foco);
            }

            var area = Rectangle.Inflate(b.ClientRectangle, -8, -2);
            var flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
            var fuente = b.Font;
            // Si el texto no entra en una línea, se achica un poco antes de partirlo en dos.
            while (TextRenderer.MeasureText(g, b.Text, fuente).Width > area.Width && fuente.Size > 8.5F)
                fuente = ObtenerFuente(fuente.FontFamily.Name, fuente.Size - 0.75F, fuente.Style);
            bool dosLineas = TextRenderer.MeasureText(g, b.Text, fuente).Width > area.Width && area.Height >= fuente.Height * 2;
            flags |= dosLineas ? TextFormatFlags.WordBreak : TextFormatFlags.SingleLine;
            TextRenderer.DrawText(g, b.Text, fuente, area, texto, flags);
        }

        private static void PintarBotonMenu(Button b, EstadoBoton st, Graphics g, Color relleno, Color texto)
        {
            g.Clear(Sidebar);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            if (st.Seleccionado || st.Hover)
            {
                var r = new RectangleF(10.5f, 3.5f, b.Width - 21f, b.Height - 7f);
                using var path = Redondeado(r, 8);
                using var brocha = new SolidBrush(relleno);
                g.FillPath(brocha, path);
            }
            if (st.Seleccionado)
            {
                using var barra = new SolidBrush(Acento);
                g.FillRectangle(barra, 0, 8, 4, b.Height - 16);
            }

            int x = 26;
            var glifo = GlifoMenu(b.Text);
            if (glifo != null)
            {
                var rIcono = new Rectangle(x, 0, 24, b.Height);
                TextRenderer.DrawText(g, glifo, FuenteIconos, rIcono, st.Seleccionado ? Acento : texto,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
                x += 36;
            }
            var rTexto = new Rectangle(x, 0, b.Width - x - 12, b.Height);
            TextRenderer.DrawText(g, FormatoOracion(b.Text), b.Font, rTexto, texto,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
        }

        /// <summary>Ícono de Segoe Fluent Icons / MDL2 para cada sección del menú (por su texto).</summary>
        private static string? GlifoMenu(string texto)
        {
            var t = SinAcentos(texto);
            int codigo = t switch
            {
                _ when t.Contains("inicio") => 0xE80F,          // casa
                _ when t.Contains("cliente") => 0xE716,         // personas
                _ when t.Contains("libro") => 0xE8F1,           // biblioteca
                _ when t.Contains("proveedor") => 0xE806,       // reparto
                _ when t.Contains("inventario") => 0xE7B8,      // caja de archivo (antes que "venta": inVENTArio)
                _ when t.Contains("venta") => 0xE7BF,           // carrito
                _ when t.Contains("reporte") => 0xE9F9,         // informe con gráfico
                _ when t.Contains("usuario") => 0xE77B,         // contacto
                _ when t.Contains("grupo") => 0xE902,           // grupo
                _ when t.Contains("clave") => 0xE72E,           // candado
                _ when t.Contains("auditoria") => 0xE81C,       // historial
                _ when t.Contains("resguardo") => 0xE74E,       // guardar
                _ when t.Contains("cerrar sesion") => 0xF3B1,   // salir
                _ => 0,
            };
            return codigo == 0 ? null : ((char)codigo).ToString();
        }

        /// <summary>Marca <paramref name="seleccionado"/> como la sección activa del menú (null = ninguna).</summary>
        public static void SeleccionarBotonMenu(Control menuPanel, Button? seleccionado)
        {
            foreach (var btn in TodosLosControles(menuPanel).OfType<Button>())
            {
                var st = estadosBoton.GetValue(btn, _ => new EstadoBoton { Tipo = TipoBoton.Menu });
                st.Seleccionado = ReferenceEquals(btn, seleccionado);
                ActualizarColoresBase(btn, st);
                btn.Invalidate();
            }
        }

        private static IEnumerable<Control> TodosLosControles(Control raiz)
        {
            var pila = new Stack<Control>();
            pila.Push(raiz);
            while (pila.Count > 0)
            {
                foreach (Control hijo in pila.Pop().Controls)
                {
                    yield return hijo;
                    if (hijo.HasChildren) pila.Push(hijo);
                }
            }
        }

        #endregion
        private static readonly ConditionalWeakTable<Control, object> filasReacomodadas = new();

        /// <summary>
        /// Con la tipografía nueva algunos textos ("Generar orden de reposición") ya no entran en una línea del botón
        /// de 38 px: se ensancha el botón y se corren los controles que tiene a continuación en la misma fila
        /// (hacia la derecha, o hacia la izquierda si el botón está anclado a la derecha).
        /// </summary>
        private static void ReacomodarBotones(Control contenedor)
        {
            if (contenedor is FlowLayoutPanel or TableLayoutPanel || EsSidebar(contenedor) || EnSidebar(contenedor)) return;
            if (filasReacomodadas.TryGetValue(contenedor, out _)) return;
            filasReacomodadas.Add(contenedor, new object());

            var botones = contenedor.Controls.OfType<Button>()
                .Where(b => b.Dock == DockStyle.None && !string.IsNullOrWhiteSpace(b.Text))
                .ToList();

            static bool AncladoDerecha(Control c) => (c.Anchor & AnchorStyles.Right) != 0 && (c.Anchor & AnchorStyles.Left) == 0;
            static bool MismaFila(Control a, Control b) => a.Top < b.Bottom - 4 && b.Top < a.Bottom - 4;
            static int Necesario(Button b) => TextRenderer.MeasureText(b.Text, b.Font).Width + 26;

            foreach (var b in botones.Where(x => !AncladoDerecha(x)).OrderBy(x => x.Left))
            {
                int delta = Necesario(b) - b.Width;
                if (delta <= 0) continue;
                int borde = b.Right;
                int finFila = contenedor.Controls.Cast<Control>()
                    .Where(o => o.Dock == DockStyle.None && !AncladoDerecha(o) && (o == b || o.Left >= borde - 2 && MismaFila(o, b))
                                && (o.Anchor & AnchorStyles.Right) == 0)
                    .Max(o => o.Right);
                if (finFila + delta > contenedor.ClientSize.Width - 4) continue;   // no hay lugar: el texto se achica al pintar
                foreach (Control otro in contenedor.Controls)
                    if (otro != b && otro.Dock == DockStyle.None && !AncladoDerecha(otro) && otro.Left >= borde - 2 && MismaFila(otro, b))
                    {
                        // Un control estirado (anclado a izquierda y derecha) se angosta en vez de correrse,
                        // para no tapar lo que está anclado a la derecha.
                        bool estirado = (otro.Anchor & AnchorStyles.Right) != 0 && otro.Width - delta >= 60;
                        otro.Left += delta;
                        if (estirado) otro.Width -= delta;
                    }
                b.Width += delta;
            }

            foreach (var b in botones.Where(AncladoDerecha).OrderByDescending(x => x.Right))
            {
                int delta = Necesario(b) - b.Width;
                if (delta <= 0) continue;
                int borde = b.Left;
                foreach (Control otro in contenedor.Controls)
                    if (otro != b && otro.Dock == DockStyle.None && AncladoDerecha(otro) && otro.Right <= borde + 2 && MismaFila(otro, b))
                        otro.Left -= delta;
                b.Left -= delta;
                b.Width += delta;
            }
        }


        #region Listas, combos y grillas

        private static void AplicarComboBox(ComboBox cb)
        {
            cb.BackColor = Superficie;
            cb.ForeColor = TextoTitulo;
            cb.Font = Normalizar(cb.Font, true);
            cb.Cursor = Cursors.Hand;
            if (cb.DrawMode == DrawMode.OwnerDrawFixed) return;
            cb.DrawMode = DrawMode.OwnerDrawFixed;
            cb.DrawItem += ComboBox_DrawItem;
        }

        private static void ComboBox_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (sender is not ComboBox cb) return;
            bool seleccionado = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            bool edicion = (e.State & DrawItemState.ComboBoxEdit) == DrawItemState.ComboBoxEdit;
            var fondo = seleccionado && !edicion ? GrillaSeleccion : cb.Enabled ? Superficie : GrillaEncabezado;
            using (var brocha = new SolidBrush(fondo))
                e.Graphics.FillRectangle(brocha, e.Bounds);
            if (e.Index < 0) return;
            var texto = cb.GetItemText(cb.Items[e.Index]);
            var r = new Rectangle(e.Bounds.X + 4, e.Bounds.Y, e.Bounds.Width - 4, e.Bounds.Height);
            TextRenderer.DrawText(e.Graphics, texto, cb.Font, r, cb.Enabled ? TextoTitulo : TextoInactivo,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
        }

        private static void AplicarGrilla(DataGridView dgv)
        {
            dgv.EnableHeadersVisualStyles = false;
            dgv.BackgroundColor = Superficie;
            dgv.BorderStyle = BorderStyle.None;
            dgv.GridColor = Borde;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.RowHeadersVisible = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            var enc = dgv.ColumnHeadersDefaultCellStyle;
            enc.BackColor = GrillaEncabezado;
            enc.ForeColor = TextoSubtitulo;
            enc.SelectionBackColor = GrillaEncabezado;
            enc.SelectionForeColor = TextoSubtitulo;
            enc.Font = FuenteTextoNegrita;
            enc.Alignment = DataGridViewContentAlignment.MiddleLeft;
            enc.Padding = new Padding(6, 0, 6, 0);
            enc.WrapMode = DataGridViewTriState.False;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 40;

            var celda = dgv.DefaultCellStyle;
            celda.BackColor = Superficie;
            celda.ForeColor = TextoTitulo;
            celda.Font = FuenteTexto;
            celda.SelectionBackColor = GrillaSeleccion;
            celda.SelectionForeColor = TextoTitulo;
            celda.Padding = new Padding(6, 0, 6, 0);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = GrillaAlterna;
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = GrillaSeleccion;
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = TextoTitulo;

            dgv.RowTemplate.Height = 34;
            foreach (DataGridViewRow fila in dgv.Rows)
                if (!fila.IsNewRow && fila.Height < 34) fila.Height = 34;

            foreach (DataGridViewColumn col in dgv.Columns) AplicarColumna(col);
            dgv.ColumnAdded -= Grilla_ColumnAdded;
            dgv.ColumnAdded += Grilla_ColumnAdded;

            typeof(DataGridView).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(dgv, true);
        }

        private static void Grilla_ColumnAdded(object? sender, DataGridViewColumnEventArgs e) => AplicarColumna(e.Column);

        private static void AplicarColumna(DataGridViewColumn col)
        {
            if (col is DataGridViewButtonColumn bc)
            {
                bc.FlatStyle = FlatStyle.Flat;
                bc.DefaultCellStyle.BackColor = GrillaEncabezado;
                bc.DefaultCellStyle.ForeColor = Primario;
                bc.DefaultCellStyle.SelectionBackColor = InfoSuave;
                bc.DefaultCellStyle.SelectionForeColor = PrimarioPresionado;
                bc.DefaultCellStyle.Font = FuenteTextoNegrita;
                bc.DefaultCellStyle.Padding = new Padding(4, 5, 4, 5);
                bc.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        #endregion

        #region Imágenes

        private static void AplicarPictureBox(PictureBox pic)
        {
            pic.BackColor = Color.Transparent;
            var nombre = pic.Name?.ToLowerInvariant() ?? string.Empty;

            if (nombre.Contains("cerrar"))
            {
                pic.Image = Cruz(EsFondoOscuro(pic.Parent) ? SidebarTexto : TextoSuave);
                pic.SizeMode = PictureBoxSizeMode.CenterImage;
                pic.Cursor = Cursors.Hand;
                return;
            }

            if (EnSidebar(pic) || EsSidebar(pic.Parent!))
            {
                pic.Image = LogoClaro.Value;
                pic.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (nombre.Contains("logo") || nombre.Contains("brand"))
            {
                pic.Image = LogoTransparente.Value;
                pic.SizeMode = PictureBoxSizeMode.Zoom;
                // Que el logo entre completo en su franja (antes se estiraba y quedaba cortado).
                if (pic.Parent is Panel franja && pic.Dock == DockStyle.None && pic.Bottom > franja.ClientSize.Height - 4)
                    pic.Height = Math.Max(24, franja.ClientSize.Height - pic.Top - 6);
            }
        }

        private static readonly Dictionary<int, Bitmap> cruces = new();

        private static Bitmap Cruz(Color color)
        {
            if (cruces.TryGetValue(color.ToArgb(), out var bmp)) return bmp;
            bmp = new Bitmap(14, 14);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using var lapiz = new Pen(color, 1.8f) { StartCap = LineCap.Round, EndCap = LineCap.Round };
                g.DrawLine(lapiz, 2, 2, 12, 12);
                g.DrawLine(lapiz, 12, 2, 2, 12);
            }
            cruces[color.ToArgb()] = bmp;
            return bmp;
        }

        /// <summary>
        /// El logo para fondos oscuros: el blanco del original se vuelve transparente, el azul tinta pasa a
        /// blanco y el dorado de la pluma se conserva.
        /// </summary>
        public static readonly Lazy<Bitmap> LogoClaro = new(() => ProcesarLogo(claro: true));

        /// <summary>El logo con su fondo blanco hecho transparente, para apoyarlo sobre tarjetas.</summary>
        public static readonly Lazy<Bitmap> LogoTransparente = new(() => ProcesarLogo(claro: false));

        private static Bitmap ProcesarLogo(bool claro)
        {
            var original = Resources.scriptumLogo;
            var bmp = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp)) g.DrawImage(original, 0, 0, original.Width, original.Height);

            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var datos = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            try
            {
                int bytes = Math.Abs(datos.Stride) * bmp.Height;
                var px = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(datos.Scan0, px, 0, bytes);
                for (int i = 0; i < bytes; i += 4)
                {
                    int b = px[i], gr = px[i + 1], r = px[i + 2];
                    double lum = 0.299 * r + 0.587 * gr + 0.114 * b;
                    double tinta = Math.Clamp((245 - lum) / 150.0, 0, 1);   // cuánto se aleja del blanco
                    if (claro)
                    {
                        bool dorado = r > b + 35;
                        if (dorado) { px[i] = 0x3C; px[i + 1] = 0xA8; px[i + 2] = 0xE0; }   // #E0A83C
                        else { px[i] = 0xFC; px[i + 1] = 0xFA; px[i + 2] = 0xF8; }          // #F8FAFC
                    }
                    else if (tinta > 0.02)
                    {
                        // Se "despega" el blanco del borde suavizado para que no quede un halo claro.
                        static byte Sin(int c, double a) => (byte)Math.Clamp((c - (1 - a) * 255) / a, 0, 255);
                        px[i] = Sin(b, tinta); px[i + 1] = Sin(gr, tinta); px[i + 2] = Sin(r, tinta);
                    }
                    px[i + 3] = (byte)(tinta * 255);
                }
                System.Runtime.InteropServices.Marshal.Copy(px, 0, datos.Scan0, bytes);
            }
            finally { bmp.UnlockBits(datos); }
            return bmp;
        }

        #endregion

        #region Utilidades de dibujo

        private static Color Mezclar(Color a, Color b, double t) => Color.FromArgb(
            (int)(a.R + (b.R - a.R) * t), (int)(a.G + (b.G - a.G) * t), (int)(a.B + (b.B - a.B) * t));

        private static GraphicsPath Redondeado(RectangleF r, float radio)
        {
            var path = new GraphicsPath();
            float d = Math.Min(radio * 2, Math.Min(r.Width, r.Height));
            if (d <= 0) { path.AddRectangle(r); return path; }
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        #endregion
    }
}
