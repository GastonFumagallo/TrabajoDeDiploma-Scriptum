using Controladora.Seguridad;
using Modelo;
using Modelo.Seguridad;
using Vista.Comun;

namespace Vista.Seguridad
{
    /// <summary>
    /// Modal único de alta y edición de usuarios: <c>new FrmEditarUsuario(null)</c> = alta, <c>new FrmEditarUsuario(id)</c> = edición.
    /// Pestañas: Datos, y Grupos y permisos (permisos heredados de los grupos en gris, más los directos).
    /// No tiene ningún campo de clave: en el alta el servicio genera una temporal (ver <see cref="ResultadoAlta"/>) y los
    /// cambios de clave van por "Resetear clave" en el listado o por "Mi clave" para la propia cuenta.
    /// El ciclo de vida (carga, cambios sin guardar, reactivación, concurrencia) lo maneja <see cref="EdicionAbm{TEdicion}"/>.
    /// </summary>
    public partial class FrmEditarUsuario : Form
    {
        private readonly IUsuarioService servicio = UsuarioService.Instancia;
        private readonly IGrupoService servicioGrupos = GrupoService.Instancia;
        private readonly ValidadorFormulario validador;
        private readonly EdicionAbm<UsuarioEdicionDTO> edicion;
        private readonly int? idInicial;
        private List<GrupoAsignableDTO> grupos = new();
        private int version;
        private bool activo = true;
        private bool cargandoGrupos;

        public int? UsuarioId => edicion.Id;
        /// <summary>Resultado del alta (clave temporal enviada o a entregar), o null si fue una edición.</summary>
        public ResultadoClaveTemporal? ResultadoAlta { get; private set; }
        public string NombreUsuario => txtUsuario.Text.Trim();

        private static int OperadorId => Sesion.Instancia.Usuario?.USU_ID ?? 0;

        public FrmEditarUsuario(int? usuarioId)
        {
            InitializeComponent();
            idInicial = usuarioId;

            // Mapa "campo del DTO → control": los errores del servicio se marcan en el control correcto.
            validador = new ValidadorFormulario(errorProvider)
                .Mapear(nameof(UsuarioEdicionDTO.Usuario), txtUsuario)
                .Mapear(nameof(UsuarioEdicionDTO.NombreCompleto), txtNombre)
                .Mapear(nameof(UsuarioEdicionDTO.Email), txtEmail)
                .Mapear(nameof(UsuarioEdicionDTO.Dni), txtDni)
                .Mapear(nameof(UsuarioEdicionDTO.Telefono), txtTelefono)
                .Mapear(nameof(UsuarioEdicionDTO.GrupoIds), clbGrupos)
                .Mapear(nameof(UsuarioEdicionDTO.AccionIds), arbolPermisos);

            edicion = new EdicionAbm<UsuarioEdicionDTO>(this, new()
            {
                Validador = validador,
                Guardar = btnGuardar,
                Estado = lblEstado,
                Entidad = "usuario",
                Descripcion = u => u.Usuario,
                Id = u => u.Id,
                Activo = u => u.Activo,
                Obtener = servicio.ObtenerPorIdAsync,
                GuardarServicio = GuardarAsync,
                Reactivar = servicio.ReactivarAsync,
                Mostrar = Mostrar,
                Armar = Armar,
                ValidarFormulario = ValidarFormulario,
                PrimerControl = txtUsuario,
            });

            edicion.Vigilar(txtUsuario, txtNombre, txtEmail, txtDni, txtTelefono);
            clbGrupos.ItemCheck += (_, _) =>
            {
                if (cargandoGrupos) return;
                edicion.MarcarModificado(clbGrupos);
                BeginInvoke(ActualizarHeredados);   // ItemCheck llega antes de que cambie el estado
            };
            arbolPermisos.SeleccionCambiada += (_, _) => { edicion.MarcarModificado(arbolPermisos); ActualizarConteo(); };
            txtFiltrarPermisos.TextChanged += (_, _) => arbolPermisos.Filtrar(txtFiltrarPermisos.Text);
            txtUsuario.Leave += async (_, _) => await ValidarUsuarioAlSalirAsync();
            txtEmail.Leave += async (_, _) => await ValidarEmailAlSalirAsync();
        }

        private async void FrmEditarUsuario_Load(object? sender, EventArgs e)
        {
            try
            {
                UseWaitCursor = true;
                var tareaGrupos = servicio.ObtenerGruposAsignablesAsync(edicion.Token);
                var tareaArbol = servicioGrupos.ObtenerArbolPermisosAsync(edicion.Token);
                await Task.WhenAll(tareaGrupos, tareaArbol);
                grupos = tareaGrupos.Result;
                clbGrupos.Items.AddRange(grupos.Cast<object>().ToArray());
                arbolPermisos.CargarCatalogo(tareaArbol.Result);
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo abrir la ficha del usuario.");
                DialogResult = DialogResult.Cancel;
                return;
            }
            finally
            {
                UseWaitCursor = false;
            }
            await edicion.CargarAsync(idInicial, () => new UsuarioEdicionDTO());
        }

        private async Task<int> GuardarAsync(UsuarioEdicionDTO dto, string _, CancellationToken ct)
        {
            if (dto.Id is null)
            {
                ResultadoAlta = await servicio.CrearAsync(dto, ct);
                return ResultadoAlta.UsuarioId;
            }
            return await servicio.EditarAsync(dto, ct);
        }

        #region Binding DTO ↔ controles

        private void Mostrar(UsuarioEdicionDTO u)
        {
            var esAlta = u.Id == null;
            version = u.Version;
            activo = u.Activo;

            txtUsuario.Text = u.Usuario;
            txtUsuario.ReadOnly = !esAlta;
            lblUsuarioInfo.Text = esAlta
                ? "Letras, números, punto, guion o guion bajo. No se puede cambiar después."
                : "No se puede cambiar: lo usan las ventas, los movimientos y las órdenes.";
            txtNombre.Text = u.NombreCompleto;
            txtEmail.Text = u.Email;
            txtDni.Text = u.Dni;
            txtTelefono.Text = u.Telefono;

            cargandoGrupos = true;
            try
            {
                for (var i = 0; i < grupos.Count; i++)
                    clbGrupos.SetItemChecked(i, u.GrupoIds.Contains(grupos[i].Id));
            }
            finally
            {
                cargandoGrupos = false;
            }
            arbolPermisos.EstablecerSeleccion(u.AccionIds);
            ActualizarHeredados();

            // Nadie cambia sus propios grupos ni permisos (lo exige también UsuarioService): se ven pero no se editan.
            var esPropia = u.Id == OperadorId;
            clbGrupos.Enabled = !esPropia;
            arbolPermisos.SoloLectura = esPropia;

            lblClaveInfo.Text = esAlta
                ? "La clave no se carga acá: al guardar se genera una clave temporal segura y se envía por email. " +
                  "El usuario la tiene que cambiar en su primer ingreso."
                : "La clave no se edita desde esta ficha: para blanquearla usá \"Resetear clave\" (Ctrl+R) en el listado.";
            if (esPropia)
                lblClaveInfo.Text += "\n\nEs tu propia cuenta: tus grupos y permisos los tiene que cambiar otro administrador, " +
                                     "y tu clave se cambia desde \"Mi clave\" en el menú.";
        }

        private UsuarioEdicionDTO Armar() => new()
        {
            Id = edicion.Id,
            Version = version,
            Activo = activo,   // el estado se cambia desde el listado (F4), no desde la ficha
            Usuario = txtUsuario.Text.Trim(),
            NombreCompleto = txtNombre.Text.Trim(),
            Email = txtEmail.Text.Trim(),
            Dni = txtDni.Text.Trim(),
            Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim(),
            GrupoIds = GruposMarcados().Select(g => g.Id).ToList(),
            AccionIds = arbolPermisos.Seleccion.ToList(),
        };

        private IEnumerable<GrupoAsignableDTO> GruposMarcados() => clbGrupos.CheckedItems.Cast<GrupoAsignableDTO>();

        /// <summary>Los permisos de los grupos activos marcados se ven en el árbol como heredados (no se pueden quitar).</summary>
        private void ActualizarHeredados()
        {
            if (IsDisposed) return;
            arbolPermisos.EstablecerHeredados(GruposMarcados().Where(g => g.Activo).SelectMany(g => g.AccionIds));
            ActualizarConteo();
        }

        private void ActualizarConteo() =>
            lblConteoPermisos.Text = $"{arbolPermisos.Seleccion.Count} directos · {arbolPermisos.CantidadHeredados} por grupo";

        #endregion

        #region Validación (primera capa: feedback inmediato)

        private bool ValidarFormulario()
        {
            validador.Limpiar();
            if (edicion.EsAlta)
            {
                validador.Requerido(txtUsuario, "El usuario");
                validador.Regla(txtUsuario, !UsuarioService.EsNombreUsuarioValido(txtUsuario.Text), UsuarioService.ReglaNombreUsuario);
            }
            validador.Requerido(txtNombre, "El nombre completo");
            validador.Requerido(txtEmail, "El email");
            validador.Regla(txtEmail, !Identificadores.EsEmailValido(txtEmail.Text), "El email no tiene un formato válido.");
            validador.Regla(txtDni, !Identificadores.EsDniValido(txtDni.Text), "El DNI debe tener 7 u 8 dígitos.");
            validador.Regla(txtTelefono, !string.IsNullOrWhiteSpace(txtTelefono.Text) && !Identificadores.EsTelefonoValido(txtTelefono.Text),
                "El teléfono no es válido (sólo números, espacios, guiones, paréntesis y +).");
            validador.Regla(clbGrupos, clbGrupos.CheckedItems.Count == 0, "Asigná al menos un grupo.");
            validador.EnfocarPrimero();
            return validador.EsValido;
        }

        private async Task ValidarUsuarioAlSalirAsync()
        {
            if (!edicion.EsAlta || string.IsNullOrWhiteSpace(txtUsuario.Text)) return;
            validador.Limpiar(txtUsuario);
            if (!UsuarioService.EsNombreUsuarioValido(txtUsuario.Text))
            {
                validador.Marcar(txtUsuario, UsuarioService.ReglaNombreUsuario);
                return;
            }
            try
            {
                if (await servicio.ExisteNombreUsuarioAsync(txtUsuario.Text, null, edicion.Token))
                    validador.Marcar(txtUsuario, "Ese nombre de usuario ya está en uso.");
            }
            catch
            {
                // La verificación en vivo es una ayuda: al guardar, el servicio vuelve a validar.
            }
        }

        private async Task ValidarEmailAlSalirAsync()
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text)) return;
            validador.Limpiar(txtEmail);
            if (!Identificadores.EsEmailValido(txtEmail.Text))
            {
                validador.Marcar(txtEmail, "El email no tiene un formato válido.");
                return;
            }
            try
            {
                if (await servicio.ExisteEmailAsync(txtEmail.Text, edicion.Id, edicion.Token))
                    validador.Marcar(txtEmail, "Ese email ya está registrado en otra cuenta.");
            }
            catch
            {
                // Ídem: el servicio vuelve a validar al guardar.
            }
        }

        #endregion
    }
}
