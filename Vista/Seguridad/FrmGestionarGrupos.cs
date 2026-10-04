using Controladora.Seguridad;
using Modelo;
using Modelo.Seguridad;
using Servicios;
using Vista.Comun;

namespace Vista.Seguridad
{
    /// <summary>
    /// Gestión de grupos de permisos en un solo formulario master-detail: a la izquierda el listado
    /// (búsqueda en vivo y filtro de estado), a la derecha la ficha del grupo seleccionado con su árbol de permisos
    /// y sus usuarios. No usa DbContext: todo pasa por <see cref="IGrupoService"/>.
    /// <list type="bullet">
    /// <item>Atajos: F2 nuevo · Ctrl+S guardar · Esc descartar · Supr eliminar (desde la grilla) · F5 refrescar · Ctrl+F buscar.</item>
    /// <item>Cambios sin guardar: al cambiar de grupo, crear uno nuevo o volver, pregunta si guardar, descartar o cancelar.</item>
    /// <item>El grupo de sistema (Administrador) sólo permite editar la descripción.</item>
    /// </list>
    /// </summary>
    public partial class FrmGestionarGrupos : Form
    {
        private static readonly EstadoGrupoDTO TodosLosEstados = new(0, "Todos");

        private readonly IGrupoService servicio = GrupoService.Instancia;
        private readonly ValidadorFormulario validador;
        private readonly BindingSource bsGrupos = new();
        private readonly BindingSource bsUsuarios = new();
        private readonly System.Windows.Forms.Timer timerBusqueda = new() { Interval = 300 };
        private readonly CancellationTokenSource ctsForm = new();
        private CancellationTokenSource? ctsListado;
        private CancellationTokenSource? ctsFicha;
        private Font? fuenteSistema;
        private Font? fuenteInactivo;

        private List<EstadoGrupoDTO> estados = new();
        /// <summary>Grupo abierto en la ficha (Id null = alta en curso), o null si no hay ninguno.</summary>
        private GrupoEdicionDTO? ficha;
        private bool hayCambios;
        /// <summary>Mientras se vuelcan datos a los controles: sus eventos no cuentan como cambios del usuario.</summary>
        private bool mostrandoFicha;
        /// <summary>Mientras el código mueve la selección de la grilla: no abre fichas.</summary>
        private bool ignorarSeleccion;
        private bool listo;
        private bool puedeAgregar, puedeModificar, puedeEliminar;

        public FrmGestionarGrupos()
        {
            InitializeComponent();
            ConfigurarGrillas();

            validador = new ValidadorFormulario(errorProvider)
                .Mapear(nameof(GrupoEdicionDTO.Nombre), txtNombre)
                .Mapear(nameof(GrupoEdicionDTO.Descripcion), txtDescripcion)
                .Mapear(nameof(GrupoEdicionDTO.EstadoId), cbEstado)
                .Mapear(nameof(GrupoEdicionDTO.AccionIds), arbolPermisos);

            txtBuscar.TextChanged += (_, _) => { timerBusqueda.Stop(); timerBusqueda.Start(); };
            timerBusqueda.Tick += async (_, _) => { timerBusqueda.Stop(); await CargarListadoAsync(); };
            cbFiltroEstado.SelectedIndexChanged += async (_, _) => await CargarListadoAsync();
            dgvGrupos.SelectionChanged += async (_, _) => await CambioDeSeleccionAsync();

            txtNombre.TextChanged += (_, _) => MarcarCambios();
            txtNombre.Leave += async (_, _) => await ValidarNombreUnicoAsync();
            txtDescripcion.TextChanged += (_, _) => MarcarCambios();
            cbEstado.SelectionChangeCommitted += (_, _) => MarcarCambios();
            arbolPermisos.SeleccionCambiada += (_, _) => { validador.Limpiar(arbolPermisos); MarcarCambios(); };
            txtFiltrarPermisos.TextChanged += (_, _) => arbolPermisos.Filtrar(txtFiltrarPermisos.Text);
            btnMarcarTodo.Click += (_, _) => arbolPermisos.MarcarVisibles(true);
            btnDesmarcarTodo.Click += (_, _) => arbolPermisos.MarcarVisibles(false);

            btnNuevo.Click += async (_, _) => await NuevoAsync();
            btnGuardar.Click += async (_, _) => await GuardarAsync();
            btnDescartar.Click += async (_, _) => await DescartarAsync();
            btnEliminar.Click += async (_, _) => await EliminarAsync();
            btnVolver.Click += async (_, _) => await VolverAsync();

            KeyDown += FrmGestionarGrupos_KeyDown;
            Load += FrmGestionarGrupos_Load;
            FormClosing += (_, _) => { timerBusqueda.Stop(); ctsForm.Cancel(); };
            Disposed += (_, _) =>
            {
                timerBusqueda.Dispose();
                ctsForm.Dispose();
                fuenteSistema?.Dispose();
                fuenteInactivo?.Dispose();
            };
        }

        private void ConfigurarGrillas()
        {
            GrillaHelper.ConfigurarListado(dgvGrupos);
            GrillaHelper.Columna(dgvGrupos, nameof(GrupoListadoDTO.Nombre), "Grupo", peso: 160);
            GrillaHelper.Columna(dgvGrupos, nameof(GrupoListadoDTO.Estado), "Estado", peso: 70);
            GrillaHelper.Columna(dgvGrupos, nameof(GrupoListadoDTO.Usuarios), "Usuarios", peso: 60, derecha: true);
            dgvGrupos.DataSource = bsGrupos;
            dgvGrupos.CellFormatting += (_, e) =>
            {
                if (e.RowIndex < 0 || e.CellStyle == null || dgvGrupos.Rows[e.RowIndex].DataBoundItem is not GrupoListadoDTO g) return;
                if (g.EsSistema)
                    e.CellStyle.Font = fuenteSistema ??= new Font(dgvGrupos.Font, FontStyle.Bold);
                else if (!g.Activo)
                {
                    e.CellStyle.ForeColor = Color.Gray;
                    e.CellStyle.Font = fuenteInactivo ??= new Font(dgvGrupos.Font, FontStyle.Italic);
                }
            };
            dgvGrupos.CellToolTipTextNeeded += (_, e) =>
            {
                if (e.RowIndex >= 0 && dgvGrupos.Rows[e.RowIndex].DataBoundItem is GrupoListadoDTO { EsSistema: true })
                    e.ToolTipText = "Grupo del sistema: acceso total.";
            };

            GrillaHelper.ConfigurarListado(dgvUsuarios);
            GrillaHelper.Columna(dgvUsuarios, nameof(GrupoUsuarioDTO.Usuario), "Usuario", peso: 100);
            GrillaHelper.Columna(dgvUsuarios, nameof(GrupoUsuarioDTO.Nombre), "Nombre", peso: 160);
            GrillaHelper.Columna(dgvUsuarios, nameof(GrupoUsuarioDTO.Estado), "Estado", peso: 60);
            dgvUsuarios.DataSource = bsUsuarios;
        }

        private async void FrmGestionarGrupos_Load(object? sender, EventArgs e)
        {
            AplicarSeguridad();
            try
            {
                UseWaitCursor = true;
                var tareaEstados = servicio.ObtenerEstadosAsync(ctsForm.Token);
                var tareaArbol = servicio.ObtenerArbolPermisosAsync(ctsForm.Token);
                await Task.WhenAll(tareaEstados, tareaArbol);

                estados = tareaEstados.Result;
                cbFiltroEstado.Items.Add(TodosLosEstados);
                cbFiltroEstado.Items.AddRange(estados.Cast<object>().ToArray());
                cbFiltroEstado.SelectedIndex = 0;
                cbEstado.Items.AddRange(estados.Cast<object>().ToArray());
                arbolPermisos.CargarCatalogo(tareaArbol.Result);

                MostrarFicha(null);
                listo = true;
                await CargarListadoAsync();
                txtBuscar.Focus();
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo abrir la gestión de grupos.");
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void AplicarSeguridad()
        {
            puedeAgregar = PermisoService.Instancia.TienePermiso("AgregarGrupo");
            puedeModificar = PermisoService.Instancia.TienePermiso("ModificarGrupo");
            puedeEliminar = PermisoService.Instancia.TienePermiso("EliminarGrupo");
            btnNuevo.Visible = puedeAgregar;
            btnEliminar.Visible = puedeEliminar;
        }

        // ── Listado ─────────────────────────────────────────────────────────────

        private IReadOnlyList<GrupoListadoDTO> Filas => bsGrupos.DataSource as List<GrupoListadoDTO> ?? new List<GrupoListadoDTO>();

        /// <summary>
        /// Recarga el listado con los filtros actuales y mantiene la ficha coherente: re-selecciona el grupo abierto;
        /// si se está editando uno que quedó fuera del filtro, lo deja abierto; si no, abre el primero.
        /// </summary>
        private async Task CargarListadoAsync(int? seleccionarId = null)
        {
            if (!listo) return;

            ctsListado?.Cancel();
            ctsListado = CancellationTokenSource.CreateLinkedTokenSource(ctsForm.Token);
            var token = ctsListado.Token;

            try
            {
                var estadoId = (cbFiltroEstado.SelectedItem as EstadoGrupoDTO)?.Id;
                var filas = await servicio.ObtenerTodosAsync(txtBuscar.Text, estadoId, token);
                if (token.IsCancellationRequested) return;

                var objetivo = seleccionarId ?? ficha?.Id;
                ignorarSeleccion = true;
                try { bsGrupos.DataSource = filas; }
                finally { ignorarSeleccion = false; }

                lblResumen.Text = filas.Count == 0
                    ? "No hay grupos que coincidan con los filtros."
                    : $"{filas.Count} grupo(s) · F2 nuevo · Ctrl+S guardar · Supr eliminar";

                if (objetivo is int id && filas.Any(g => g.Id == id))
                    SeleccionarEnGrilla(id);
                else if (ficha != null && (ficha.Id == null || hayCambios))
                    SeleccionarEnGrilla(null);   // se sigue editando lo que está abierto
                else if (filas.Count > 0)
                {
                    SeleccionarEnGrilla(filas[0].Id);
                    await AbrirGrupoAsync(filas[0].Id);
                }
                else
                {
                    SeleccionarEnGrilla(null);
                    MostrarFicha(null);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo cargar el listado de grupos.");
            }
        }

        private void SeleccionarEnGrilla(int? id)
        {
            ignorarSeleccion = true;
            try
            {
                if (id is int valor)
                    GrillaHelper.Seleccionar<GrupoListadoDTO>(dgvGrupos, g => g.Id == valor);
                else
                    dgvGrupos.CurrentCell = null;
            }
            finally
            {
                ignorarSeleccion = false;
            }
        }

        private async Task CambioDeSeleccionAsync()
        {
            if (ignorarSeleccion || dgvGrupos.CurrentRow?.DataBoundItem is not GrupoListadoDTO fila || ficha?.Id == fila.Id)
                return;

            var teniaCambios = hayCambios;
            if (!await ConfirmarDescartarCambiosAsync())
            {
                // No se puede mover la selección dentro de SelectionChanged (reentrada): se difiere.
                var idAbierto = ficha?.Id;
                BeginInvoke(() => SeleccionarEnGrilla(idAbierto));
                return;
            }

            await AbrirGrupoAsync(fila.Id);
            if (teniaCambios)
                await CargarListadoAsync();   // si se guardó, refresca nombres y contadores
        }

        // ── Ficha ───────────────────────────────────────────────────────────────

        private async Task AbrirGrupoAsync(int id)
        {
            ctsFicha?.Cancel();
            ctsFicha = CancellationTokenSource.CreateLinkedTokenSource(ctsForm.Token);
            var token = ctsFicha.Token;

            try
            {
                UseWaitCursor = true;
                var tareaFicha = servicio.ObtenerPorIdAsync(id, token);
                var tareaUsuarios = servicio.ObtenerUsuariosAsync(id, token);
                await Task.WhenAll(tareaFicha, tareaUsuarios);
                if (token.IsCancellationRequested) return;

                if (tareaFicha.Result is null)
                {
                    MessageBox.Show(this, "El grupo ya no existe: otro usuario lo eliminó.", "Grupo eliminado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ficha = null;
                    hayCambios = false;
                    await CargarListadoAsync();
                    return;
                }
                MostrarFicha(tareaFicha.Result, tareaUsuarios.Result);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo abrir el grupo.");
            }
            finally
            {
                if (!token.IsCancellationRequested) UseWaitCursor = false;
            }
        }

        /// <summary>Vuelca un grupo a los controles (null = ninguno abierto). No cuenta como cambio del usuario.</summary>
        private void MostrarFicha(GrupoEdicionDTO? dto, List<GrupoUsuarioDTO>? usuarios = null)
        {
            mostrandoFicha = true;
            try
            {
                validador.Limpiar();
                ficha = dto;
                txtNombre.Text = dto?.Nombre ?? "";
                txtDescripcion.Text = dto?.Descripcion ?? "";
                cbEstado.SelectedItem = estados.FirstOrDefault(e => e.Id == dto?.EstadoId);
                arbolPermisos.EstablecerSeleccion(dto?.AccionIds ?? new List<int>());
                bsUsuarios.DataSource = usuarios ?? new List<GrupoUsuarioDTO>();
                tabUsuarios.Text = $"Usuarios ({bsUsuarios.Count})";
                hayCambios = false;
            }
            finally
            {
                mostrandoFicha = false;
            }
            ActualizarEstadoFicha();
        }

        private void MarcarCambios()
        {
            if (mostrandoFicha || ficha is null) return;
            hayCambios = true;
            ActualizarEstadoFicha();
        }

        /// <summary>Habilita controles y arma los avisos según el grupo abierto, los permisos del usuario y si hay cambios.</summary>
        private void ActualizarEstadoFicha()
        {
            var hayFicha = ficha != null;
            var esNuevo = ficha?.Id == null;
            var esSistema = ficha?.EsSistema == true;
            var editable = hayFicha && (esNuevo ? puedeAgregar : puedeModificar);

            txtNombre.ReadOnly = !editable || esSistema;
            txtDescripcion.ReadOnly = !editable;
            cbEstado.Enabled = editable && !esSistema;
            arbolPermisos.SoloLectura = !editable || esSistema;
            txtFiltrarPermisos.Enabled = hayFicha;
            btnMarcarTodo.Enabled = btnDesmarcarTodo.Enabled = editable && !esSistema;

            btnGuardar.Enabled = editable && hayCambios;
            btnDescartar.Enabled = hayFicha && hayCambios;
            btnEliminar.Enabled = puedeEliminar && hayFicha && !esNuevo && !esSistema;

            lblEncabezado.Text = !hayFicha ? "Seleccioná un grupo"
                : esNuevo ? "Nuevo grupo"
                : $"Grupo: {ficha!.Nombre}{(hayCambios ? " (sin guardar)" : "")}";

            var avisos = new List<string>();
            if (esSistema)
                avisos.Add($"🔒 Grupo del sistema: tiene acceso total. Sólo se puede editar la descripción.");
            else if (hayFicha && !editable)
                avisos.Add("Sólo lectura: no tenés permiso para modificar grupos.");
            if (hayFicha && !esNuevo && EsGrupoPropio(ficha!.Id!.Value))
                avisos.Add("Pertenecés a este grupo: los cambios en tus permisos se aplican la próxima vez que inicies sesión.");
            if (cbEstado.SelectedItem is EstadoGrupoDTO estado && estado.Nombre != Estado_Grupo.Activo)
                avisos.Add("Grupo deshabilitado: sus permisos no cuentan para sus usuarios.");
            lblAviso.Text = string.Join(Environment.NewLine, avisos);

            lblConteoPermisos.Text = esSistema
                ? "Acceso total"
                : $"{arbolPermisos.Seleccion.Count} de {arbolPermisos.TotalPermisos} asignados";
        }

        private static bool EsGrupoPropio(int grupoId) =>
            Sesion.Instancia.Usuario?.Grupos.Any(g => g.GRU_ID == grupoId) == true;

        private async Task ValidarNombreUnicoAsync()
        {
            if (ficha is null || txtNombre.ReadOnly || string.IsNullOrWhiteSpace(txtNombre.Text)) return;
            try
            {
                if (await servicio.ExisteNombreAsync(txtNombre.Text, ficha.Id, ctsForm.Token))
                    validador.Marcar(txtNombre, "Ya existe un grupo con ese nombre.");
                else
                    validador.Limpiar(txtNombre);
            }
            catch (OperationCanceledException) { }
            catch
            {
                // Es sólo una ayuda: el servicio vuelve a validar al guardar.
            }
        }

        // ── Acciones ────────────────────────────────────────────────────────────

        private async Task NuevoAsync()
        {
            if (!puedeAgregar || !await ConfirmarDescartarCambiosAsync()) return;

            SeleccionarEnGrilla(null);
            var activo = estados.FirstOrDefault(e => e.Nombre == Estado_Grupo.Activo) ?? estados.FirstOrDefault();
            MostrarFicha(new GrupoEdicionDTO { EstadoId = activo?.Id ?? 0 });
            tabDetalle.SelectedTab = tabPermisos;
            txtNombre.Focus();
        }

        private GrupoEdicionDTO ArmarDto() => new()
        {
            Id = ficha?.Id,
            Nombre = txtNombre.Text.Trim(),
            Descripcion = txtDescripcion.Text.Trim(),
            EstadoId = (cbEstado.SelectedItem as EstadoGrupoDTO)?.Id ?? 0,
            AccionIds = arbolPermisos.Seleccion.ToList(),
            EsSistema = ficha?.EsSistema == true,
        };

        private bool ValidarFormulario(GrupoEdicionDTO dto)
        {
            validador.Limpiar();
            validador.Requerido(txtNombre, "El nombre");
            validador.Requerido(txtDescripcion, "La descripción");
            validador.Regla(cbEstado, dto.EstadoId == 0, "Seleccioná un estado.");
            validador.Regla(arbolPermisos, !dto.EsSistema && dto.AccionIds.Count == 0, "Seleccioná al menos un permiso.");
            validador.EnfocarPrimero();
            return validador.EsValido;
        }

        /// <param name="recargar">false cuando se guarda antes de cambiar de grupo (quien llama abre el siguiente).</param>
        /// <returns>true si se guardó (o no había nada que guardar).</returns>
        private async Task<bool> GuardarAsync(bool recargar = true)
        {
            if (ficha is null || !hayCambios || !btnGuardar.Enabled) return !hayCambios;

            var dto = ArmarDto();
            if (!ValidarFormulario(dto)) return false;

            // Deshabilitar un grupo con usuarios les quita esos permisos: se confirma.
            var fila = Filas.FirstOrDefault(g => g.Id == dto.Id);
            var deshabilita = fila is { Activo: true, Usuarios: > 0 }
                && estados.FirstOrDefault(e => e.Id == dto.EstadoId)?.Nombre != Estado_Grupo.Activo;
            if (deshabilita && MessageBox.Show(this,
                    $"Los {fila!.Usuarios} usuario(s) de \"{fila.Nombre}\" van a perder los permisos de este grupo.\n\n¿Deshabilitarlo igual?",
                    "Deshabilitar grupo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return false;

            try
            {
                btnGuardar.Enabled = false;
                UseWaitCursor = true;
                var id = await servicio.GuardarAsync(dto, ctsForm.Token);
                hayCambios = false;

                if (recargar)
                {
                    ficha = new GrupoEdicionDTO { Id = id };   // para que el listado re-seleccione el grupo guardado
                    await CargarListadoAsync(id);
                    await AbrirGrupoAsync(id);
                }

                if (EsGrupoPropio(id))
                    MessageBox.Show(this,
                        "Guardado. Pertenecés a este grupo: los cambios en tus permisos se aplican la próxima vez que inicies sesión.",
                        "Grupo guardado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (ValidacionException ex)
            {
                var sinControl = validador.MostrarErrores(ex);
                if (sinControl != null)
                    MessageBox.Show(this, sinControl, "Revise los datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo guardar el grupo.");
                return false;
            }
            finally
            {
                UseWaitCursor = false;
                ActualizarEstadoFicha();
            }
        }

        private async Task DescartarAsync()
        {
            if (ficha is null || !hayCambios) return;

            hayCambios = false;
            if (ficha.Id is int id)
                await AbrirGrupoAsync(id);
            else
            {
                ficha = null;
                await CargarListadoAsync();   // vuelve a abrir el primer grupo del listado
            }
        }

        private async Task EliminarAsync()
        {
            if (!btnEliminar.Visible || !btnEliminar.Enabled || ficha?.Id is not int id) return;

            var usuarios = Filas.FirstOrDefault(g => g.Id == id)?.Usuarios ?? 0;
            if (usuarios > 0)
            {
                MessageBox.Show(this,
                    $"\"{ficha.Nombre}\" tiene {usuarios} usuario(s) asignado(s) y no se puede eliminar.\n\n" +
                    "Quitalos del grupo desde Usuarios o, si querés conservarlo, cambiá su estado a deshabilitado.",
                    "No se puede eliminar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(this, $"¿Eliminar el grupo \"{ficha.Nombre}\"? Esta acción no se puede deshacer.",
                    "Eliminar grupo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            try
            {
                UseWaitCursor = true;
                await servicio.EliminarAsync(id, ctsForm.Token);
                ficha = null;
                hayCambios = false;
                await CargarListadoAsync();
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo eliminar el grupo.");
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private async Task VolverAsync()
        {
            if (!await ConfirmarDescartarCambiosAsync()) return;
            if (Application.OpenForms["FrmMenu"] is FrmMenu principal)
                principal.MostrarInicio();
            Close();
        }

        /// <summary>Si hay cambios sin guardar, pregunta: Sí guarda, No descarta, Cancelar se queda. Devuelve si se puede seguir.</summary>
        private async Task<bool> ConfirmarDescartarCambiosAsync()
        {
            if (!hayCambios || ficha is null) return true;

            var nombre = string.IsNullOrWhiteSpace(txtNombre.Text) ? "el grupo nuevo" : $"\"{txtNombre.Text.Trim()}\"";
            var respuesta = MessageBox.Show(this, $"Hay cambios sin guardar en {nombre}. ¿Querés guardarlos?",
                "Cambios sin guardar", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

            if (respuesta == DialogResult.Cancel) return false;
            if (respuesta == DialogResult.Yes) return await GuardarAsync(recargar: false);
            hayCambios = false;
            return true;
        }

        private async void FrmGestionarGrupos_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.F2:
                    e.SuppressKeyPress = true;
                    await NuevoAsync();
                    break;
                case Keys.Control | Keys.S:
                    e.SuppressKeyPress = true;
                    await GuardarAsync();
                    break;
                case Keys.Escape when hayCambios:
                    e.SuppressKeyPress = true;
                    await DescartarAsync();
                    break;
                case Keys.Delete when dgvGrupos.Focused:
                    e.SuppressKeyPress = true;
                    await EliminarAsync();
                    break;
                case Keys.F5:
                    e.SuppressKeyPress = true;
                    await CargarListadoAsync();
                    break;
                case Keys.Control | Keys.F:
                    e.SuppressKeyPress = true;
                    txtBuscar.Focus();
                    txtBuscar.SelectAll();
                    break;
            }
        }
    }
}
