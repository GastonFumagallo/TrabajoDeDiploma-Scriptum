using Controladora.Seguridad;
using Modelo;
using Modelo.Seguridad;
using Servicios;
using Vista.Comun;

namespace Vista.Seguridad
{
    /// <summary>
    /// Gestión de cuentas de usuario. El listado (búsqueda en vivo, filtro de estado, F2/F3/F4, exportar, imprimir)
    /// lo aporta <see cref="ListadoAbm{TListado}"/>; este formulario agrega el filtro por grupo, "sólo bloqueados",
    /// el blanqueo de clave (Ctrl+R) y el desbloqueo. La grilla nunca recibe datos de la clave.
    /// </summary>
    public partial class FrmGestionarUsuarios : Form
    {
        private const string TodosLosGrupos = "Todos";

        private readonly IUsuarioService servicio = UsuarioService.Instancia;
        private readonly ListadoAbm<UsuarioListadoDTO> listado;
        private bool puedeResetear, puedeModificar;

        private static int OperadorId => Sesion.Instancia.Usuario?.USU_ID ?? 0;

        public FrmGestionarUsuarios()
        {
            InitializeComponent();
            ConfigurarGrilla();

            listado = new ListadoAbm<UsuarioListadoDTO>(this, new()
            {
                Grilla = dgvListado,
                Buscar = txtBuscar,
                Estado = cbEstado,
                Resumen = lblResumen,
                Nuevo = btnNuevo,
                Editar = btnEditar,
                CambiarEstado = btnCambiarEstado,
                Exportar = btnExportar,
                Imprimir = btnImprimir,
                Entidad = "usuario",
                EntidadPlural = "usuarios",
                Id = u => u.Id,
                Activo = u => u.Activo,
                Descripcion = u => string.IsNullOrWhiteSpace(u.NombreCompleto) ? u.Usuario : $"{u.Usuario} ({u.NombreCompleto})",
                PermiteCambiarEstado = u => u.Id != OperadorId,   // nadie se desactiva a sí mismo
                Cargar = (texto, estado, ct) => servicio.ObtenerTodosAsync(new FiltroUsuarios
                {
                    Texto = texto,
                    Estado = estado,
                    GrupoId = (cbFiltroExtra.SelectedItem as GrupoAsignableDTO)?.Id,
                    SoloBloqueados = chkSoloBloqueados.Checked,
                }, ct),
                AbrirEdicion = AbrirEdicion,
                CambiarEstadoServicio = (id, activo, ct) => servicio.CambiarEstadoAsync(id, activo, OperadorId, ct),
                AdvertenciaBaja = u => u.EsAdministrador ? "es administrador: va a perder el acceso total." : null,
            });

            listado.AccionesActualizadas += (_, _) => ActualizarAccionesPropias();
            cbFiltroExtra.SelectedIndexChanged += async (_, _) => await listado.CargarAsync();
            chkSoloBloqueados.CheckedChanged += async (_, _) =>
            {
                cbEstado.Enabled = !chkSoloBloqueados.Checked;   // "bloqueados" ignora el filtro de estado
                await listado.CargarAsync();
            };
            btnLimpiarFiltros.Click += async (_, _) =>
            {
                if (cbFiltroExtra.Items.Count > 0) cbFiltroExtra.SelectedIndex = 0;
                chkSoloBloqueados.Checked = false;
                await listado.LimpiarFiltrosAsync();
            };
            btnResetearClave.Click += async (_, _) => await ResetearClaveAsync();
            btnDesbloquear.Click += async (_, _) => await DesbloquearAsync();
            btnSalir.Click += (_, _) => Salir();
            KeyDown += async (_, e) =>
            {
                if (e.KeyData != (Keys.Control | Keys.R)) return;
                e.Handled = e.SuppressKeyPress = true;
                await ResetearClaveAsync();
            };
        }

        private void ConfigurarGrilla()
        {
            GrillaHelper.ConfigurarListado(dgvListado);
            GrillaHelper.Columna(dgvListado, nameof(UsuarioListadoDTO.Id), "ID", peso: 40, derecha: true);
            GrillaHelper.Columna(dgvListado, nameof(UsuarioListadoDTO.Usuario), "Usuario", peso: 100);
            GrillaHelper.Columna(dgvListado, nameof(UsuarioListadoDTO.NombreCompleto), "Nombre", peso: 160);
            GrillaHelper.Columna(dgvListado, nameof(UsuarioListadoDTO.Email), "Email", peso: 180);
            GrillaHelper.Columna(dgvListado, nameof(UsuarioListadoDTO.Grupos), "Grupos", peso: 140);
            GrillaHelper.Columna(dgvListado, nameof(UsuarioListadoDTO.Estado), "Estado", peso: 110);
            GrillaHelper.Columna(dgvListado, nameof(UsuarioListadoDTO.UltimoAcceso), "Último acceso", peso: 100, formato: "dd/MM/yyyy HH:mm");
            dgvListado.CellFormatting += (_, e) =>
            {
                if (e.RowIndex < 0 || e.CellStyle == null || dgvListado.Rows[e.RowIndex].DataBoundItem is not UsuarioListadoDTO u) return;
                if (u.Activo && u.Bloqueado)
                    e.CellStyle.ForeColor = Color.Firebrick;
            };
            dgvListado.CellToolTipTextNeeded += (_, e) =>
            {
                if (e.RowIndex >= 0 && dgvListado.Rows[e.RowIndex].DataBoundItem is UsuarioListadoDTO { Bloqueado: true } u)
                    e.ToolTipText = $"Bloqueado por intentos fallidos hasta las {u.BloqueadoHasta:HH:mm}.";
            };
        }

        private async void FrmGestionarUsuarios_Load(object? sender, EventArgs e)
        {
            btnNuevo.Visible = PermisoService.Instancia.TienePermiso("AgregarUsuario");
            puedeModificar = PermisoService.Instancia.TienePermiso("ModificarUsuario");
            btnEditar.Visible = puedeModificar;
            btnCambiarEstado.Visible = PermisoService.Instancia.TienePermiso("EliminarUsuario");
            puedeResetear = PermisoService.Instancia.TienePermiso("ResetearClave");
            btnResetearClave.Visible = puedeResetear;
            btnDesbloquear.Visible = false;

            try
            {
                var grupos = await servicio.ObtenerGruposAsignablesAsync(listado.Token);
                cbFiltroExtra.Items.Add(TodosLosGrupos);
                cbFiltroExtra.Items.AddRange(grupos.Cast<object>().ToArray());
                cbFiltroExtra.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudieron cargar los grupos.");
            }
            await listado.IniciarAsync();
        }

        /// <summary>Reset y desbloqueo según la fila: nunca sobre la propia cuenta (eso va por "Mi clave").</summary>
        private void ActualizarAccionesPropias()
        {
            var u = listado.Seleccionado;
            btnResetearClave.Enabled = puedeResetear && u != null && u.Id != OperadorId;
            btnDesbloquear.Visible = puedeModificar && u?.Bloqueado == true;
        }

        /// <summary>Abre el modal único (null = alta). Devuelve el id guardado, o null si se canceló.</summary>
        private int? AbrirEdicion(int? id)
        {
            using var frm = new FrmEditarUsuario(id);
            if (frm.ShowDialog(this) != DialogResult.OK) return null;
            if (frm.ResultadoAlta is { } alta)
                FrmClaveTemporal.Mostrar(this, frm.NombreUsuario, alta);
            return frm.UsuarioId;
        }

        private async Task ResetearClaveAsync()
        {
            if (!btnResetearClave.Visible || !btnResetearClave.Enabled || listado.Seleccionado is not { } u) return;

            if (MessageBox.Show(this,
                    $"¿Blanquear la clave de \"{u.Usuario}\"?\n\n" +
                    $"• Se genera una clave temporal y se envía a {u.Email}.\n" +
                    "• La clave actual deja de funcionar en este momento.\n" +
                    "• Deberá cambiarla al iniciar sesión. Si estaba bloqueada, la cuenta se desbloquea.",
                    "Resetear clave", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            try
            {
                UseWaitCursor = true;
                var resultado = await Task.Run(() => servicio.ResetearClaveAsync(u.Id, OperadorId, listado.Token));
                UseWaitCursor = false;
                FrmClaveTemporal.Mostrar(this, u.Usuario, resultado);
                await listado.CargarAsync(u.Id);
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo resetear la clave.");
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private async Task DesbloquearAsync()
        {
            if (!btnDesbloquear.Visible || listado.Seleccionado is not { Bloqueado: true } u) return;
            if (MessageBox.Show(this, $"¿Desbloquear la cuenta \"{u.Usuario}\"? Podrá volver a intentar iniciar sesión ya mismo.",
                    "Desbloquear", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                await servicio.DesbloquearAsync(u.Id, listado.Token);
                await listado.CargarAsync(u.Id);
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo desbloquear la cuenta.");
            }
        }

        private void Salir()
        {
            if (Application.OpenForms["FrmMenu"] is FrmMenu principal)
                principal.MostrarInicio();
            Close();
        }
    }
}
