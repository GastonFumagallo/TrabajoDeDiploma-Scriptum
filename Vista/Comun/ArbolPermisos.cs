using Modelo.Seguridad;
using System.ComponentModel;

namespace Vista.Comun
{
    /// <summary>
    /// TreeView con casillas para asignar permisos: Módulo → Formulario → Acción.
    /// <list type="bullet">
    /// <item>La selección (IDs de acción) vive en un HashSet; el árbol es sólo la vista. Filtrar o recargar
    /// nunca pierde ni desincroniza lo marcado.</item>
    /// <item>Marcar un módulo o formulario marca todas sus acciones; un padre queda marcado si lo están todas
    /// sus acciones, y su texto muestra cuántas lo están ("Ventas (2/5)").</item>
    /// <item>Los cambios hechos por código llegan con <see cref="TreeViewAction.Unknown"/> y se ignoran:
    /// no hay eventos en cascada ni hace falta un flag de "actualizando".</item>
    /// <item>Corrige el bug del TreeView nativo en el que el doble clic sobre una casilla cambia lo que se ve
    /// sin que el estado quede sincronizado.</item>
    /// </list>
    /// </summary>
    internal sealed class ArbolPermisos : TreeView
    {
        private const int WM_LBUTTONDBLCLK = 0x0203;

        private List<ModuloPermisosDTO> catalogo = new();
        private readonly HashSet<int> seleccion = new();
        private readonly HashSet<int> heredados = new();
        private string filtro = "";

        public ArbolPermisos()
        {
            CheckBoxes = true;
            HideSelection = false;
        }

        /// <summary>El usuario marcó o desmarcó algo (no se dispara por cambios hechos por código).</summary>
        public event EventHandler? SeleccionCambiada;

        /// <summary>Muestra la selección pero no deja cambiarla (se puede seguir recorriendo y expandiendo).</summary>
        [DefaultValue(false)]
        public bool SoloLectura { get; set; }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IReadOnlyCollection<int> Seleccion => seleccion;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TotalPermisos => catalogo.Sum(m => m.Formularios.Sum(f => f.Acciones.Count));

        public void CargarCatalogo(List<ModuloPermisosDTO> modulos)
        {
            catalogo = modulos;
            Reconstruir();
        }

        public void EstablecerSeleccion(IEnumerable<int> accionIds)
        {
            seleccion.Clear();
            seleccion.UnionWith(accionIds);
            Reconstruir();
        }

        /// <summary>
        /// Permisos que ya da otra fuente (los grupos de un usuario): se ven marcados, en gris y "(por grupo)", y no se
        /// pueden quitar. Si alguno estaba también en la selección directa, se saca de ella porque sería redundante.
        /// </summary>
        public void EstablecerHeredados(IEnumerable<int> accionIds)
        {
            heredados.Clear();
            heredados.UnionWith(accionIds);
            seleccion.ExceptWith(heredados);
            Reconstruir();
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CantidadHeredados => heredados.Count;

        /// <summary>Muestra sólo lo que coincide con el texto (por módulo, formulario o acción). Lo marcado se conserva.</summary>
        public void Filtrar(string texto)
        {
            filtro = texto.Trim();
            Reconstruir();
        }

        /// <summary>Marca o desmarca todas las acciones visibles (respeta el filtro).</summary>
        public void MarcarVisibles(bool marcado)
        {
            if (SoloLectura) return;
            var cambio = false;
            foreach (var hoja in Hojas(Nodes).Where(h => !heredados.Contains((int)h.Tag!)))
                cambio |= Aplicar((int)hoja.Tag!, marcado);
            if (!cambio) return;
            Reconstruir();
            SeleccionCambiada?.Invoke(this, EventArgs.Empty);
        }

        private void Reconstruir()
        {
            BeginUpdate();
            try
            {
                Nodes.Clear();
                foreach (var modulo in catalogo)
                {
                    var coincideModulo = Coincide(modulo.Nombre);
                    var nodoModulo = new TreeNode { Name = modulo.Nombre };

                    foreach (var form in modulo.Formularios)
                    {
                        var coincideForm = coincideModulo || Coincide(form.Nombre);
                        var nodoForm = new TreeNode { Name = form.Nombre };

                        foreach (var accion in form.Acciones.Where(a => coincideForm || Coincide(a.Nombre)))
                        {
                            var heredado = heredados.Contains(accion.Id);
                            nodoForm.Nodes.Add(new TreeNode(heredado ? $"{accion.Nombre} (por grupo)" : accion.Nombre)
                            {
                                Name = accion.Nombre,
                                Tag = accion.Id,
                                Checked = heredado || seleccion.Contains(accion.Id),
                                ForeColor = heredado ? SystemColors.GrayText : Color.Empty,
                            });
                        }

                        if (nodoForm.Nodes.Count > 0)
                            nodoModulo.Nodes.Add(nodoForm);
                    }

                    if (nodoModulo.Nodes.Count == 0) continue;
                    Nodes.Add(nodoModulo);
                    ActualizarDesdeHijos(nodoModulo);
                }
                ExpandAll();
                if (Nodes.Count > 0) Nodes[0].EnsureVisible();
            }
            finally
            {
                EndUpdate();
            }
        }

        private bool Coincide(string texto) =>
            filtro.Length == 0 || texto.Contains(filtro, StringComparison.CurrentCultureIgnoreCase);

        protected override void OnBeforeCheck(TreeViewCancelEventArgs e)
        {
            var esUsuario = e.Action != TreeViewAction.Unknown;
            if (esUsuario && (SoloLectura || e.Node?.Tag is int id && heredados.Contains(id)))
                e.Cancel = true;
            base.OnBeforeCheck(e);
        }

        protected override void OnAfterCheck(TreeViewEventArgs e)
        {
            base.OnAfterCheck(e);
            if (e.Action == TreeViewAction.Unknown || e.Node is null) return;   // lo marcó el código, no el usuario

            var nodo = e.Node;
            BeginUpdate();
            try
            {
                if (nodo.Tag is int id)
                {
                    Aplicar(id, nodo.Checked);
                }
                else
                {
                    foreach (var hoja in Hojas(nodo.Nodes))
                    {
                        var hojaId = (int)hoja.Tag!;
                        if (heredados.Contains(hojaId))
                        {
                            hoja.Checked = true;   // lo da un grupo: no se puede quitar desde acá
                            continue;
                        }
                        hoja.Checked = nodo.Checked;
                        Aplicar(hojaId, nodo.Checked);
                    }
                    ActualizarDesdeHijos(nodo);
                }

                for (var padre = nodo.Parent; padre != null; padre = padre.Parent)
                    ActualizarEstado(padre);
            }
            finally
            {
                EndUpdate();
            }
            SeleccionCambiada?.Invoke(this, EventArgs.Empty);
        }

        private bool Aplicar(int accionId, bool marcado) =>
            marcado ? seleccion.Add(accionId) : seleccion.Remove(accionId);

        /// <summary>Recalcula de abajo hacia arriba los nodos intermedios de una rama.</summary>
        private static void ActualizarDesdeHijos(TreeNode nodo)
        {
            foreach (TreeNode hijo in nodo.Nodes)
                if (hijo.Tag is not int) ActualizarDesdeHijos(hijo);
            ActualizarEstado(nodo);
        }

        /// <summary>Un nodo intermedio está marcado si lo están todas sus acciones; el texto muestra el conteo.</summary>
        private static void ActualizarEstado(TreeNode nodo)
        {
            var hojas = Hojas(nodo.Nodes).ToList();
            var marcadas = hojas.Count(h => h.Checked);
            nodo.Checked = hojas.Count > 0 && marcadas == hojas.Count;
            nodo.Text = $"{nodo.Name} ({marcadas}/{hojas.Count})";
        }

        private static IEnumerable<TreeNode> Hojas(TreeNodeCollection nodos)
        {
            foreach (TreeNode nodo in nodos)
            {
                if (nodo.Tag is int) yield return nodo;
                else foreach (var hoja in Hojas(nodo.Nodes)) yield return hoja;
            }
        }

        protected override void WndProc(ref Message m)
        {
            // El doble clic sobre la casilla se descarta: el primer clic ya la cambió.
            if (m.Msg == WM_LBUTTONDBLCLK && CheckBoxes)
            {
                var lParam = m.LParam.ToInt64();
                var punto = new Point((short)(lParam & 0xFFFF), (short)((lParam >> 16) & 0xFFFF));
                if (HitTest(punto).Location == TreeViewHitTestLocations.StateImage)
                {
                    m.Result = IntPtr.Zero;
                    return;
                }
            }
            base.WndProc(ref m);
        }
    }
}
