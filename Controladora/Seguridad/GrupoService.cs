using Controladora.Abm;
using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using Modelo.Seguridad;
using Servicios;
using System.Data;

namespace Controladora.Seguridad
{
    public interface IGrupoService
    {
        Task<List<GrupoListadoDTO>> ObtenerTodosAsync(string? texto, int? estadoId, CancellationToken ct = default);
        /// <summary>Ficha para editar, o null si no existe.</summary>
        Task<GrupoEdicionDTO?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
        Task<List<EstadoGrupoDTO>> ObtenerEstadosAsync(CancellationToken ct = default);
        Task<List<ModuloPermisosDTO>> ObtenerArbolPermisosAsync(CancellationToken ct = default);
        Task<List<GrupoUsuarioDTO>> ObtenerUsuariosAsync(int grupoId, CancellationToken ct = default);
        Task<bool> ExisteNombreAsync(string nombre, int? excluirId = null, CancellationToken ct = default);
        /// <summary>Alta (Id null) o modificación. Devuelve el Id guardado.</summary>
        Task<int> GuardarAsync(GrupoEdicionDTO dto, CancellationToken ct = default);
        /// <summary>Borrado físico: sólo grupos que no son de sistema y no tienen usuarios. Si no, deshabilitarlo.</summary>
        Task EliminarAsync(int id, CancellationToken ct = default);
    }

    /// <summary>
    /// ABM de grupos de permisos. Lecturas AsNoTracking proyectadas a DTO; escrituras con un DbContext por operación
    /// que carga el grupo y sincroniza su colección de acciones por ID (sólo cambian las filas de AccionGrupo
    /// que difieren). Protege al grupo de sistema (Administrador) y no borra grupos con usuarios.
    /// </summary>
    public sealed class GrupoService : ServicioAbmBase, IGrupoService
    {
        private const int LargoMaximo = 60;   // [StringLength(60)] en Grupo

        private static GrupoService? instancia;
        public static GrupoService Instancia => instancia ??= new GrupoService();

        protected override string NombreEntidad => "el grupo";

        public async Task<List<GrupoListadoDTO>> ObtenerTodosAsync(string? texto, int? estadoId, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            var query = db.Grupos.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(texto))
            {
                var t = texto.Trim();
                query = query.Where(g => g.GRU_Nombre.Contains(t) || g.GRU_Descripcion.Contains(t));
            }
            if (estadoId is > 0)
                query = query.Where(g => g.EST_GRU_ID == estadoId);

            return await query
                .OrderBy(g => g.GRU_Nombre)
                .Select(g => new GrupoListadoDTO(
                    g.GRU_ID,
                    g.GRU_Nombre,
                    g.GRU_Descripcion,
                    g.Estado_Grupo.EST_GRU_Nombre,
                    g.Estado_Grupo.EST_GRU_Nombre == Estado_Grupo.Activo,
                    g.Usuarios.Count(),
                    g.GRU_Nombre == Grupo.NombreAdministrador))
                .ToListAsync(ct);
        }

        public async Task<GrupoEdicionDTO?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await db.Grupos.AsNoTracking()
                .Where(g => g.GRU_ID == id)
                .Select(g => new GrupoEdicionDTO
                {
                    Id = g.GRU_ID,
                    Nombre = g.GRU_Nombre,
                    Descripcion = g.GRU_Descripcion,
                    EstadoId = g.EST_GRU_ID,
                    AccionIds = g.Acciones.Select(a => a.ACC_ID).ToList(),
                    EsSistema = g.GRU_Nombre == Grupo.NombreAdministrador,
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<List<EstadoGrupoDTO>> ObtenerEstadosAsync(CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await db.Estados_Grupos.AsNoTracking()
                .OrderBy(e => e.EST_GRU_ID)
                .Select(e => new EstadoGrupoDTO(e.EST_GRU_ID, e.EST_GRU_Nombre))
                .ToListAsync(ct);
        }

        /// <summary>Catálogo completo de permisos en una sola consulta (sin N+1 ni entidades rastreadas).</summary>
        public async Task<List<ModuloPermisosDTO>> ObtenerArbolPermisosAsync(CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await db.Modulos.AsNoTracking()
                .OrderBy(m => m.MOD_Nombre)
                .Select(m => new ModuloPermisosDTO(
                    m.MOD_Nombre,
                    m.Formularios
                        .OrderBy(f => f.FORM_Nombre)
                        .Select(f => new FormularioPermisosDTO(
                            f.FORM_Nombre,
                            f.Acciones
                                .OrderBy(a => a.ACC_Nombre)
                                .Select(a => new AccionPermisoDTO(a.ACC_ID, a.ACC_Nombre))
                                .ToList()))
                        .ToList()))
                .ToListAsync(ct);
        }

        public async Task<List<GrupoUsuarioDTO>> ObtenerUsuariosAsync(int grupoId, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await db.Usuarios.AsNoTracking()
                .Where(u => u.Grupos.Any(g => g.GRU_ID == grupoId))
                .OrderBy(u => u.USU_Nombre)
                .Select(u => new GrupoUsuarioDTO(u.USU_Nombre, u.USU_Persona.PER_Nombre, u.Estado_Usuario.EST_USU_Nombre))
                .ToListAsync(ct);
        }

        public async Task<bool> ExisteNombreAsync(string nombre, int? excluirId = null, CancellationToken ct = default)
        {
            var n = nombre?.Trim() ?? "";
            if (n.Length == 0) return false;
            await using var db = new Libreria();
            return await db.Grupos.AnyAsync(g => g.GRU_Nombre == n && g.GRU_ID != (excluirId ?? 0), ct);
        }

        public async Task<int> GuardarAsync(GrupoEdicionDTO dto, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(dto);
            // Un botón oculto no es seguridad: el permiso se vuelve a exigir acá.
            PermisoService.Instancia.Exigir(dto.Id is null ? "AgregarGrupo" : "ModificarGrupo");
            var nombre = dto.Nombre?.Trim() ?? "";
            var descripcion = dto.Descripcion?.Trim() ?? "";

            await using var db = new Libreria();

            Grupo grupo;
            bool esSistema;
            if (dto.Id is null)
            {
                grupo = new Grupo();
                esSistema = false;
            }
            else
            {
                grupo = await db.Grupos
                    .Include(g => g.Acciones)
                    .FirstOrDefaultAsync(g => g.GRU_ID == dto.Id, ct)
                    ?? throw new ConcurrenciaException("Otro usuario eliminó este grupo mientras lo editabas.");
                esSistema = grupo.GRU_Nombre == Grupo.NombreAdministrador;
            }

            var errores = new Errores();
            errores.Requerido(nombre, nameof(dto.Nombre), "El nombre", LargoMaximo);
            errores.Requerido(descripcion, nameof(dto.Descripcion), "La descripción", LargoMaximo);
            if (esSistema)
            {
                errores.Si(nombre != Grupo.NombreAdministrador, nameof(dto.Nombre),
                    $"El grupo {Grupo.NombreAdministrador} es del sistema y no se puede renombrar.");
                errores.Si(dto.EstadoId != grupo.EST_GRU_ID, nameof(dto.EstadoId),
                    $"El grupo {Grupo.NombreAdministrador} es del sistema y no se puede deshabilitar.");
            }
            else
            {
                errores.Si(nombre == Grupo.NombreAdministrador, nameof(dto.Nombre),
                    $"\"{Grupo.NombreAdministrador}\" es el nombre reservado del grupo de sistema.");
                errores.Si(dto.AccionIds.Count == 0, nameof(dto.AccionIds), "Seleccioná al menos un permiso.");
                errores.Si(!await db.Estados_Grupos.AnyAsync(e => e.EST_GRU_ID == dto.EstadoId, ct),
                    nameof(dto.EstadoId), "Seleccioná un estado válido.");

                // Nadie se amplía los permisos a través de un grupo propio: lo tiene que hacer otro administrador.
                var cambianAcciones = dto.Id != null && !grupo.Acciones.Select(a => a.ACC_ID).ToHashSet().SetEquals(dto.AccionIds);
                errores.Si(cambianAcciones && EsGrupoPropio(grupo.GRU_ID), nameof(dto.AccionIds),
                    "No podés modificar los permisos de un grupo al que pertenecés: lo tiene que hacer otro administrador.");
            }
            // Unicidad en alta y en modificación (el índice único la garantiza ante altas simultáneas).
            if (!errores.HayErrores)
                errores.Si(await db.Grupos.AnyAsync(g => g.GRU_Nombre == nombre && g.GRU_ID != (dto.Id ?? 0), ct),
                    nameof(dto.Nombre), "Ya existe un grupo con ese nombre.");
            errores.Lanzar();

            grupo.GRU_Nombre = nombre;
            grupo.GRU_Descripcion = descripcion;
            if (!esSistema)
            {
                grupo.EST_GRU_ID = dto.EstadoId;
                await SincronizarAccionesAsync(db, grupo, dto.AccionIds, ct);
            }

            if (dto.Id is null)
                db.Grupos.Add(grupo);

            await GuardarCambiosAsync(db, ct, nameof(dto.Nombre), "nombre");   // un SaveChanges = una transacción
            BitacoraSeguridad.Registrar(dto.Id is null ? BitacoraSeguridad.GrupoCreado : BitacoraSeguridad.GrupoModificado,
                $"{grupo.GRU_Nombre} · acciones: [{string.Join(", ", grupo.Acciones.Select(a => a.ACC_Nombre))}]");
            return grupo.GRU_ID;
        }

        /// <summary>El usuario logueado pertenece al grupo (según los grupos con los que inició sesión).</summary>
        public static bool EsGrupoPropio(int grupoId) =>
            Sesion.Instancia.Usuario?.Grupos.Any(g => g.GRU_ID == grupoId) == true;

        /// <summary>Quita las acciones desmarcadas y agrega las nuevas; las que no cambian no se tocan.</summary>
        private static async Task SincronizarAccionesAsync(Libreria db, Grupo grupo, IEnumerable<int> accionIds, CancellationToken ct)
        {
            var deseadas = accionIds.ToHashSet();

            foreach (var quitar in grupo.Acciones.Where(a => !deseadas.Contains(a.ACC_ID)).ToList())
                grupo.Acciones.Remove(quitar);

            var faltantes = deseadas.Except(grupo.Acciones.Select(a => a.ACC_ID)).ToList();
            if (faltantes.Count == 0) return;

            foreach (var agregar in await db.Acciones.Where(a => faltantes.Contains(a.ACC_ID)).ToListAsync(ct))
                grupo.Acciones.Add(agregar);
        }

        public async Task EliminarAsync(int id, CancellationToken ct = default)
        {
            PermisoService.Instancia.Exigir("EliminarGrupo");
            await using var db = new Libreria();
            // Serializable: nadie puede asignar un usuario al grupo entre el conteo y el borrado
            // (la FK de GrupoUsuario es en cascada y se llevaría esa asignación sin avisar).
            await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);

            var grupo = await db.Grupos.FirstOrDefaultAsync(g => g.GRU_ID == id, ct);
            if (grupo is null) return;

            if (grupo.GRU_Nombre == Grupo.NombreAdministrador)
                throw new ValidacionException("Grupo", $"El grupo {Grupo.NombreAdministrador} es del sistema y no se puede eliminar.");

            var usuarios = await db.Usuarios.CountAsync(u => u.Grupos.Any(g => g.GRU_ID == id), ct);
            if (usuarios > 0)
                throw new ValidacionException("Grupo",
                    $"El grupo tiene {usuarios} usuario(s) asignado(s). Quitalos del grupo o, si querés conservar el historial, deshabilitalo en lugar de eliminarlo.");

            db.Grupos.Remove(grupo);   // las filas de AccionGrupo se borran en cascada
            await GuardarCambiosAsync(db, ct);
            await tx.CommitAsync(ct);
            BitacoraSeguridad.Registrar(BitacoraSeguridad.GrupoEliminado, grupo.GRU_Nombre);
        }
    }
}
