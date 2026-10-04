using Microsoft.EntityFrameworkCore.Update;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Vista.Properties;

namespace Vista.Theme
{
    internal static class FuturisticTheme
    {

        public static readonly Color fondoPrincipal = ColorTranslator.FromHtml("#D6E4F0");
        public static readonly Color paneles = ColorTranslator.FromHtml("#4C6378");
        public static readonly Color hover = ColorTranslator.FromHtml("#6C8399");
        public static readonly Color botones = ColorTranslator.FromHtml("#34495E");
        public static readonly Color Black = ColorTranslator.FromHtml("#1F1F1F");
        public static readonly Color White = ColorTranslator.FromHtml("#ffffff");


        // Fondo principal
        public static readonly Color Background = fondoPrincipal;
        // Panels y sidebar
        public static readonly Color Panel = paneles;
        // Hover / selección suave
        public static readonly Color Accent = hover;
        // Botones principales / títulos
        public static readonly Color Secondary = botones;
        // Texto principal
        public static readonly Color Text = Black;
        public static readonly Font DefaultFont = new Font("Segoe UI", 9F, FontStyle.Bold);
        public static readonly Font ButtonFont = new Font(DefaultFont.FontFamily, 9F, FontStyle.Bold);

        // Track which forms we've already themed
        private static readonly HashSet<Form> _appliedForms = new();

        // Store per-button themed default color and selection state
        private static readonly ConditionalWeakTable<Button, ButtonState> _buttonStates = new();
        private static readonly Lazy<Icon?> AppIcon = new(() =>
        {
            try { return Icon.FromHandle(Resources.icoScriptum.GetHicon()); }
            catch { return null; /* ignore resource issues */ }
        });

        private sealed class ButtonState { public Color DefaultColor; public bool IsSelected; public string OriginalText; public ButtonState(Color c) { DefaultColor = c; IsSelected = false; OriginalText = string.Empty; } }

        public static void Initialize()
        {
            // Apply immediately to already open forms
            ApplyToOpenForms();
            // Subscribe to Idle to catch forms when they're created/shown
            Application.Idle -= OnApplicationIdle;
            Application.Idle += OnApplicationIdle;
        }

        private static void OnApplicationIdle(object? sender, EventArgs e)
        {
            ApplyToOpenForms();
        }

        private static void ApplyToOpenForms()
        {
            foreach (Form f in Application.OpenForms)
            {
                // Skip theming for the login form (keeps original UI)
                if (string.Equals(f.GetType().Name, "FrmIniciarSesión", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!_appliedForms.Contains(f))
                {
                    try { ApplyToForm(f); }
                    catch { /* ignore per-form errors */ }
                    _appliedForms.Add(f);
                }
            }

            // Clean up closed forms from the set
            var toRemove = _appliedForms.Where(f => f.IsDisposed).ToList();
            foreach (var d in toRemove) _appliedForms.Remove(d);
        }

        public static void ApplyToForm(Form form)
        {
            ArgumentNullException.ThrowIfNull(form);
            // Do not apply theme to the login form — preserve its original look
            if (string.Equals(form.GetType().Name, "FrmIniciarSesión", StringComparison.OrdinalIgnoreCase))
                return;

            _appliedForms.Add(form);

            form.BackColor = Background;
            form.ForeColor = Text;
            // Do NOT override form.Font: keep original sizes

            // Un formulario embebido no muestra ícono; los de primer nivel comparten uno solo
            // (GetHicon crea un handle nativo que nunca se libera: crearlo por formulario es una fuga de GDI).
            if (form.TopLevel && AppIcon.Value != null)
                form.Icon = AppIcon.Value;

            ApplyToControlCollection(form.Controls);
        }

        private static void ApplyToControlCollection(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                ApplyToControl(control);
                if (control.HasChildren)
                    ApplyToControlCollection(control.Controls);
            }
        }

        private static void ApplyToControl(Control control)
        {
            if (control is null) return;

            // Respect existing font sizes and styles; do not override control.Font
            control.ForeColor = Text;

            switch (control)
            {
                case Panel p:
                    // panels act like cards on the white background
                    var lowerPanelName = p.Name?.ToLowerInvariant() ?? string.Empty;
                    // Make menu, title and logo panels use the same color for contrast
                    if (lowerPanelName.Contains("menu") || lowerPanelName.Contains("panelmenu") || lowerPanelName.Contains("titulo") || lowerPanelName.Contains("title") || lowerPanelName.Contains("paneltitulo") || lowerPanelName.Contains("logo") || lowerPanelName.Contains("panellogo"))
                    {
                        // use same themed color (the same as panelMenu)
                        p.BackColor = Secondary;
                    }
                    else
                    {
                        p.BackColor = Panel;
                    }
                    break;

                case CheckBox cb:
                    cb.BackColor = Color.Transparent;
                    cb.ForeColor = White;
                    break;
         

                case Button b:
                    ApplyButtonStyle(b);
                    break;

                case Label l:
                    if (l.Tag?.ToString() == "Bienvenido")
                    {
                        l.BackColor = Color.Transparent;
                        l.ForeColor = paneles;
                    }
                    else if (l.Tag?.ToString() == "Titulo")
                    {
                        l.BackColor = Color.Transparent;
                        l.ForeColor = White;
                    }
                    else if (l.Tag?.ToString() == "BLANCO")
                    {
                        l.BackColor = Color.Transparent;
                        l.ForeColor = White;
                    }
                    else
                    {
                        l.BackColor = Color.Transparent;
                        l.ForeColor = Text;
                    }

                    break;

                case TextBox t:
                    // keep textboxes bright and readable on light UI
                    t.BackColor = Color.FromArgb(255, 255, 255);
                    t.ForeColor = Text;
                    t.BorderStyle = BorderStyle.FixedSingle;
                    break;

                case ComboBox cb:
                    ApplyComboBoxStyle(cb);
                    break;

                case DataGridView dgv:
                    ApplyDataGridViewStyle(dgv);
                    break;

                case GroupBox gb:
                    gb.BackColor = Panel;
                    gb.ForeColor = White;
                    foreach (Control control1 in gb.Controls)
                    {
                        if (control1 is Label lbl)
                        {
                            lbl.ForeColor = White;
                        }
                    }

                    break;

                case PictureBox pic:
                    // Ensure transparent background where possible
                    pic.BackColor = Color.Transparent;
                    // If the designer didn't set an image and the picturebox name suggests a logo, set the gym logo resource
                    try
                    {
                        var lower = pic.Name?.ToLowerInvariant() ?? string.Empty;
                        if (pic.Image == null && (lower.Contains("logo") || lower.Contains("gim") || lower.Contains("brand")))
                        {
                            pic.Image = Resources.scriptumLogo;
                            pic.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                    }
                    catch { }
                    break;
            }
        }

        private static void ApplyButtonStyle(Button b)
        {
            b.FlatStyle = FlatStyle.Flat;

            bool isMenuButton = IsInMenuPanel(b);

            // Determine themed default color: menu buttons -> black, others -> Secondary
            var themedDefault = isMenuButton ? Black : Secondary;

            // Store themed default for the button
            if (!_buttonStates.TryGetValue(b, out var state))
            {
                state = new ButtonState(themedDefault);
                _buttonStates.Add(b, state);
            }
            else
            {
                state.DefaultColor = themedDefault; // update if theme changed
            }

            // Preserve original text for potential restore and enable multiline for buttons inside forms
            if (string.IsNullOrEmpty(state.OriginalText))
                state.OriginalText = b.Text ?? string.Empty;

            if (IsInPanelForm(b) || (!IsInMenuPanel(b) && state.OriginalText?.Trim().Contains(' ') == true))
            {
                b.Text = MakeMultiline(state.OriginalText);
                b.TextAlign = ContentAlignment.MiddleCenter;
            }

            // Apply visual defaults but preserve font/size/padding
            b.BackColor = state.DefaultColor;
            b.ForeColor = isMenuButton ? White : White;
            b.FlatAppearance.BorderSize = 0;

            // Attach custom paint to draw accent border and subtle glow
            b.Paint -= Button_Paint;
            b.Paint += Button_Paint;

            // Mouse events to slightly change appearance
            b.MouseEnter -= Button_MouseEnter;
            b.MouseEnter += Button_MouseEnter;
            b.MouseLeave -= Button_MouseLeave;
            b.MouseLeave += Button_MouseLeave;
            // La selección del menú no se marca al hacer clic: la decide el shell con SeleccionarBotonMenu,
            // porque la navegación puede denegarse (permisos) o cancelarse (cambios sin guardar).
        }

        private static bool IsInPanelForm(Control c)
        {
            Control? cur = c.Parent;
            while (cur != null)
            {
                var name = cur.Name?.ToLowerInvariant() ?? string.Empty;
                if (name.Contains("panelform") || name.Contains("panelform"))
                    return true;
                cur = cur.Parent;
            }
            return false;
        }

        private static string MakeMultiline(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            if (text.Contains('\n')) return text; // already multiline

            // If two or more words, replace the last space with a newline so last word goes below
            var idx = text.LastIndexOf(' ');
            if (idx > 0)
            {
                return text.Substring(0, idx) + "\n" + text.Substring(idx + 1);
            }

            return text;
        }

        private static bool IsInMenuPanel(Control c)
        {
            Control? cur = c.Parent;
            while (cur != null)
            {
                var name = cur.Name?.ToLowerInvariant() ?? string.Empty;
                if (name.Contains("menu") || name.Contains("panelmenu"))
                    return true;
                cur = cur.Parent;
            }
            return false;
        }

        private static void Button_MouseEnter(object? sender, EventArgs e)
        {
            if (sender is not Button b) return;
            if (!_buttonStates.TryGetValue(b, out var state)) return;

            // If selected, keep selection color
            if (state.IsSelected) return;

            // If menu button, hover -> DarkMaroon; otherwise subtle accent (use Secondary)
            if (IsInMenuPanel(b))
                b.BackColor = Secondary; // DarkMaroon
            else
                b.BackColor = Accent;
        }

        private static void Button_MouseLeave(object? sender, EventArgs e)
        {
            if (sender is not Button b) return;
            if (!_buttonStates.TryGetValue(b, out var state)) return;

            // If selected, keep selection color
            if (state.IsSelected)
            {
                b.BackColor = Accent;
                return;
            }

            // Restore themed default
            b.BackColor = state.DefaultColor;
        }

        /// <summary>Marca <paramref name="seleccionado"/> como la sección activa de <paramref name="menuPanel"/> (null = ninguna).</summary>
        public static void SeleccionarBotonMenu(Control menuPanel, Button? seleccionado)
        {
            foreach (var btn in GetAllControls(menuPanel).OfType<Button>())
            {
                if (!_buttonStates.TryGetValue(btn, out var st))
                {
                    st = new ButtonState(Black);
                    _buttonStates.Add(btn, st);
                }

                if (ReferenceEquals(btn, seleccionado))
                {
                    st.IsSelected = true;
                    btn.BackColor = Accent; // BrightRed
                    btn.ForeColor = White;
                }
                else
                {
                    st.IsSelected = false;
                    btn.BackColor = st.DefaultColor; // black
                    btn.ForeColor = White;
                }
            }
        }

        private static IEnumerable<Control> GetAllControls(Control root)
        {
            var stack = new Stack<Control>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                var c = stack.Pop();
                foreach (Control child in c.Controls)
                {
                    yield return child;
                    if (child.HasChildren) stack.Push(child);
                }
            }
        }

        private static void Button_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is not Button b) return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, b.Width - 1, b.Height - 1);

            // Fill background (the control already has BackColor)
            using (var back = new SolidBrush(b.BackColor))
                g.FillRectangle(back, rect);

            // Draw subtle rounded border with accent
            using (var path = RoundedRect(rect, 6))
            {
                using (var pen = new Pen(Color.FromArgb(200, Accent), 2))
                {
                    g.DrawPath(pen, path);
                }

                // Inner thin line with Secondary color for depth
                using (var inner = new Pen(Color.FromArgb(180, Secondary), 1))
                {
                    g.DrawPath(inner, path);
                }
            }

            // Draw text centered
            TextRenderer.DrawText(g, b.Text, b.Font, rect, b.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;
            var arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

            // top-left arc
            path.AddArc(arc, 180, 90);

            // top-right arc
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom-right arc
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom-left arc
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        private static void ApplyComboBoxStyle(ComboBox cb)
        {
            // Keep combo background white to match app background, but use mid gray border
            // Use a slightly darker panel color for combo background to distinguish from form background
            cb.BackColor = Color.FromArgb(240, 240, 240); // light gray
            cb.ForeColor = Text;
            cb.FlatStyle = FlatStyle.Flat;
            // Do not change font size

            // Use owner draw to style items and selection
            cb.DrawMode = DrawMode.OwnerDrawFixed;
            cb.DrawItem -= ComboBox_DrawItem;
            cb.DrawItem += ComboBox_DrawItem;

            // Improve contrast for dropdown and editable area depending on style
            cb.DropDownStyle = cb.DropDownStyle; // leave as-is
            // If data-bound and no DisplayMember set, try to use 'Descripcion' or 'Nombre' if present
            try
            {
                if (cb.DataSource != null && string.IsNullOrWhiteSpace(cb.DisplayMember))
                {
                    var first = cb.Items.Count > 0 ? cb.Items[0] : null;
                    if (first != null)
                    {
                        var t = first.GetType();
                        if (t.GetProperty("Descripcion") != null) cb.DisplayMember = "Descripcion";
                        else if (t.GetProperty("Nombre") != null) cb.DisplayMember = "Nombre";
                        else if (t.GetProperty("NombreUsuario") != null) cb.DisplayMember = "NombreUsuario";
                    }
                }
            }
            catch { }
        }

        private static void ComboBox_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (sender is not ComboBox cb) return;
            if (e.Index < 0) return;

            e.DrawBackground();
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            // Use GetItemText so DataSource + DisplayMember are respected (avoids Modelo.Modelo... ToString display)
            string text = cb.GetItemText(cb.Items[e.Index]);

            var back = selected ? new SolidBrush(Accent) : new SolidBrush(White);
            var fore = selected ? White : Text; // ensure high contrast when selected

            using (back)
                g.FillRectangle(back, e.Bounds);

            TextRenderer.DrawText(g, text, cb.Font, e.Bounds, fore, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

            e.DrawFocusRectangle();
        }

        private static void ApplyDataGridViewStyle(DataGridView dgv)
        {
            dgv.BackgroundColor = Background;
            dgv.GridColor = Color.FromArgb(220, 220, 220);
            dgv.EnableHeadersVisualStyles = false;

            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.RowHeadersVisible = false;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = Secondary; // dark maroon header
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = White;
            dgv.ColumnHeadersDefaultCellStyle.Font = ButtonFont;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgv.DefaultCellStyle.BackColor = White;
            dgv.DefaultCellStyle.ForeColor = Text;
            dgv.DefaultCellStyle.SelectionBackColor = Accent;
            dgv.DefaultCellStyle.SelectionForeColor = White;

            // Use a subtle mid-gray for alternating rows to improve readability
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            // Improve readability
            dgv.RowTemplate.Height = dgv.RowTemplate.Height; // preserve user size
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Remove focus rectangle flicker
            dgv.DoubleBuffered(true);
        }

        private static void DoubleBuffered(this DataGridView dgv, bool setting)
        {
            var dgvType = dgv.GetType();
            var pi = dgvType.GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            pi?.SetValue(dgv, setting, null);
        }
    }
}
