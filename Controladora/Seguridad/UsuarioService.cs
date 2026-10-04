using Controladora.Abm;
using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using Modelo.Seguridad;
using Servicios;
using System.Text.RegularExpressions;

namespace Controladora.Seguridad
{
    public interface IUsuarioService
    {
        Task<List<UsuarioListadoDTO>> ObtenerTodosAsync(FiltroUsuarios filtro, CancellationToken ct = default);
        Task<UsuarioEdicionDTO?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
        Task<List<GrupoAsignableDTO>> ObtenerGruposAsignablesAsync(CancellationToken ct = default);
        Task<bool> ExisteNombreUsuarioAsync(string usuario, int? excluirId = null, CancellationToken ct = default);
        Task<bool> ExisteEmailAsync(string email, int? excluirId = null, CancellationToken ct = default);

        /// <summary>Alta con clave temporal generada (cambio obligatorio al entrar), enviada por mail después de guardar.</summary>
        Task<ResultadoClaveTemporal> CrearAsync(UsuarioEdicionDTO dto, CancellationToken ct = default);
        /// <summary>Datos, grupos y permisos directos. Nunca toca la clave ni el nombre de usuario.</summary>
        Task<int> EditarAsync(UsuarioEdicionDTO dto, CancellationToken ct = default);
        Task<UsuarioEdicionDTO?> ReactivarAsync(int id, CancellationToken ct = default);
        /// <summary>Baja lógica (estado Inactivo) o reactivación.</summary>
        Task CambiarEstadoAsync(int id, bool activo, CancellationToken ct = default);

        /// <summary>Blanqueo administrativo: clave temporal nueva, cambio obligatorio y desbloqueo.</summary>
        Task<ResultadoClaveTemporal> ResetearClaveAsync(int id, CancellationToken ct = default);
        Task DesbloquearAsync(int id, CancellationToken ct = default);
    }

    /// <summary>
    /// Administración de cuentas de usuario (el login, el cambio de clave propio y la recuperación siguen en
    /// <see cref="ControladoraUsuarios"/>). Lecturas AsNoTracking proyectadas a DTO, sin N+1 y sin datos de clave;
    /// escrituras con un DbContext por operación.
    /// <list type="bullet">
    /// <item>Permisos exigidos en esta capa (<see cref="PermisoService.Exigir"/>) y todo cambio en AuditoriaSeguridad.</item>
    /// <item>Nombre de usuario y email únicos; el nombre no se edita (el historial lo guarda como texto).</item>
    /// <item>Sin borrado físico: baja lógica con reactivación.</item>
    /// <item>Nadie se da de baja ni cambia sus propios grupos o permisos, y siempre queda un administrador activo.</item>
    /// </list>
    /// </summary>
    public sealed class UsuarioService : ServicioAbmBase, IUsuarioService
    {
        private const int LargoMaximo = 60;
        private static readonly Regex FormatoUsuario = new(@"^[A-Za-z0-9._-]{3,60}$", RegexOptions.Compiled);

        public const string ReglaNombreUsuario =
            "El usuario debe tener entre 3 y 60 caracteres: letras, números, punto, guion o guion bajo (sin espacios).";

        private static UsuarioService? instancia;
        public static UsuarioService Instancia => instancia ??= new UsuarioService();

        protected override string NombreEntidad => "el usuario";

        public static bool EsNombreUsuarioValido(string? usuario) => usuario != null && FormatoUsuario.IsMatch(usuario.Trim());

        private static int? OperadorId => Sesion.Instancia.Usuario?.USU_ID;

        #region Lecturas

        public async Task<List<UsuarioListadoDTO>> ObtenerTodosAsync(FiltroUsuarios f, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            var ahora = DateTime.Now;
            var q = db.Usuarios.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(f.Texto))
            {
                var t = f.Texto.Trim();
                q = q.Where(u => u.USU_Nombre.Contains(t) || u.USU_Persona.PER_Nombre.Contains(t) || u.USU_Mail.Contains(t));
            }
            if (f.GrupoId is int grupoId)
                q = q.Where(u => u.Grupos.Any(g => g.GRU_ID == grupoId));

            if (f.SoloBloqueados)
                q = q.Where(u => u.USU_BloqueadoHasta > ahora);
            else if (f.Estado == FiltroEstadoActivo.Activos)
                q = q.Where(u => u.Estado_Usuario == null || u.Estado_Usuario.EST_USU_Nombre != Estado_Usuario.Inactivo);
            else if (f.Estado == FiltroEstadoActivo.Inactivos)
                q = q.Where(u => u.Estado_Usuario.EST_USU_Nombre == Estado_Usuario.Inactivo);

            // Una sola consulta: los grupos se concatenan en SQL (STRING_AGG), sin Include ni N+1.
            return await q
                .OrderBy(u => u.USU_Nombre)
                .Select(u => new UsuarioListadoDTO
                {
                    Id = u.USU_ID,
                    Usuario = u.USU_Nombre,
                    NombreCompleto = u.USU_Persona.PER_Nombre,
                    Email = u.USU_Mail,
                    Grupos = string.Join(", ", u.Grupos.Select(g => g.GRU_Nombre)),
                    Activo = u.Estado_Usuario == null || u.Estado_Usuario.EST_USU_Nombre != Estado_Usuario.Inactivo,
                    BloqueadoHasta = u.USU_BloqueadoHasta,
                    UltimoAcceso = u.USU_UltimoAcceso,
                    DebeCambiarClave = u.USU_DebeCambiarClave,
                    EsAdministrador = u.Grupos.Any(g => g.GRU_Nombre == Grupo.NombreAdministrador),
                })
                .ToListAsync(ct);
        }

        public async Task<UsuarioEdicionDTO?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await db.Usuarios.AsNoTracking()
                .Where(u => u.USU_ID == id)
                .Select(u => new UsuarioEdicionDTO
                {
                    Id = u.USU_ID,
                    Usuario = u.USU_Nombre,
                    NombreCompleto = u.USU_Persona.PER_Nombre,
                    Email = u.USU_Mail,
                    Telefono = u.USU_Persona.PER_Telefono,
                    Dni = u.USU_Persona.PER_DNI == 0 ? "" : u.USU_Persona.PER_DNI.ToString(),
                    Activo = u.Estado_Usuario == null || u.Estado_Usuario.EST_USU_Nombre != Estado_Usuario.Inactivo,
                    Version = u.USU_Version,
                    GrupoIds = u.Grupos.Select(g => g.GRU_ID).ToList(),
                    AccionIds = u.Acciones.Select(a => a.ACC_ID).ToList(),
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<List<GrupoAsignableDTO>> ObtenerGruposAsignablesAsync(CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await db.Grupos.AsNoTracking()
                .OrderBy(g => g.GRU_Nombre)
                .Select(g => new GrupoAsignableDTO(
                    g.GRU_ID,
                    g.GRU_Nombre,
                    g.Estado_Grupo.EST_GRU_Nombre == Estado_Grupo.Activo,
                    g.Acciones.Select(a => a.ACC_ID).ToList()))
                .ToListAsync(ct);
        }

        public async Task<bool> ExisteNombreUsuarioAsync(string usuario, int? excluirId = null, CancellationToken ct = default)
        {
            var n = usuario?.Trim() ?? "";
            if (n.Length == 0) return false;
            await using var db = new Libreria();
            return await db.Usuarios.AnyAsync(u => u.USU_Nombre == n && u.USU_ID != (excluirId ?? 0), ct);
        }

        public async Task<bool> ExisteEmailAsync(string email, int? excluirId = null, CancellationToken ct = default)
        {
            var m = email?.Trim() ?? "";
            if (m.Length == 0) return false;
            await using var db = new Libreria();
            return await db.Usuarios.AnyAsync(u => u.USU_Mail == m && u.USU_ID != (excluirId ?? 0), ct);
        }

        #endregion

        #region Alta, edición y estado

        public async Task<ResultadoClaveTemporal> CrearAsync(UsuarioEdicionDTO dto, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(dto);
            PermisoService.Instancia.Exigir("AgregarUsuario");
            await using var db = new Libreria();
            var nombre = dto.Usuario?.Trim() ?? "";
            var email = dto.Email?.Trim() ?? "";

            // Si el usuario o el email son de una cuenta dada de baja, se ofrece reactivarla en vez de duplicarla.
            var inactivo = await db.Usuarios.AsNoTracking()
                .Where(u => (u.USU_Nombre == nombre || u.USU_Mail == email) && u.Estado_Usuario.EST_USU_Nombre == Estado_Usuario.Inactivo)
                .Select(u => new { u.USU_ID, u.USU_Nombre })
                .FirstOrDefaultAsync(ct);
            if (inactivo != null)
                throw new EntidadInactivaException(inactivo.USU_ID,
                    $"Ya existe la cuenta \"{inactivo.USU_Nombre}\" con ese usuario o email, pero está dada de baja.");

            var errores = new Errores();
            errores.Requerido(nombre, nameof(dto.Usuario), "El nombre de usuario", LargoMaximo);
            errores.Si(nombre.Length > 0 && !FormatoUsuario.IsMatch(nombre), nameof(dto.Usuario), ReglaNombreUsuario);
            await ValidarDatosAsync(db, dto, null, errores, ct);
            if (!errores.HayErrores)
                errores.Si(await db.Usuarios.AnyAsync(u => u.USU_Nombre == nombre, ct), nameof(dto.Usuario), "Ese nombre de usuario ya está en uso.");
            errores.Lanzar();

            // La clave inicial la genera la capa de negocio, no la UI, y se exige cambiarla al entrar.
            var temporal = ServiciosUsuario.GenerarPassword();
            var usuario = new Usuario
            {
                USU_Nombre = nombre,
                USU_Mail = email,
                USU_Clave = HasherClaves.Hashear(temporal),
                USU_DebeCambiarClave = true,
                EST_USU_ID = await IdEstadoAsync(db, Estado_Usuario.Activo, ct),
                USU_Persona = new Persona
                {
                    PER_Nombre = dto.NombreCompleto.Trim(),
                    PER_Mail = email,
                    PER_Telefono = dto.Telefono?.Trim() ?? "",
                    PER_DNI = int.Parse(Identificadores.SoloDigitos(dto.Dni)!),
                },
            };
            await SincronizarPermisosAsync(db, usuario, dto, ct);

            db.Usuarios.Add(usuario);
            await GuardarCambiosAsync(db, ct, nameof(dto.Usuario), "nombre de usuario o email");
            BitacoraSeguridad.Registrar(BitacoraSeguridad.UsuarioCreado, DescribirPermisos(usuario));

            // El mail sale DESPUÉS de guardar: nunca llega una clave que no funciona. Si falla (SMTP sin
            // configurar), la cuenta igual se crea y la temporal se le muestra una única vez al operador.
            var enviado = ServiciosUsuario.SendMail(usuario, temporal);
            return new ResultadoClaveTemporal(usuario.USU_ID, enviado, enviado ? null : temporal);
        }

        public async Task<int> EditarAsync(UsuarioEdicionDTO dto, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(dto);
            if (dto.Id is not int id) throw new ArgumentException("Para un alta usá CrearAsync.", nameof(dto));
            PermisoService.Instancia.Exigir("ModificarUsuario");

            await using var db = new Libreria();
            var u = await db.Usuarios
                .Include(x => x.USU_Persona)
                .Include(x => x.Grupos)
                .Include(x => x.Acciones)
                .Include(x => x.Estado_Usuario)
                .FirstOrDefaultAsync(x => x.USU_ID == id, ct)
                ?? throw new ConcurrenciaException("Otro usuario eliminó esta cuenta mientras la editabas.");
            if (u.USU_Version != dto.Version)
                throw new ConcurrenciaException("Otro usuario modificó esta cuenta mientras la editabas.");

            var errores = new Errores();
            await ValidarDatosAsync(db, dto, id, errores, ct);
            // Nadie puede ampliarse (ni recortarse) sus propios permisos: lo tiene que hacer otro administrador.
            var cambiaPermisos = !MismosIds(u.Grupos.Select(g => g.GRU_ID), dto.GrupoIds)
                                 || !MismosIds(u.Acciones.Select(a => a.ACC_ID), dto.AccionIds);
            errores.Si(id == OperadorId && cambiaPermisos, nameof(dto.GrupoIds),
                "No podés modificar tus propios grupos ni permisos: lo tiene que hacer otro administrador.");
            await ValidarProteccionesAsync(db, u, dto.Activo, dto.GrupoIds, errores, ct);
            errores.Lanzar();

            var email = dto.Email.Trim();
            u.USU_Mail = email;
            u.USU_Persona.PER_Mail = email;
            u.USU_Persona.PER_Nombre = dto.NombreCompleto.Trim();
            u.USU_Persona.PER_Telefono = dto.Telefono?.Trim() ?? "";
            u.USU_Persona.PER_DNI = int.Parse(Identificadores.SoloDigitos(dto.Dni)!);
            u.EST_USU_ID = await IdEstadoAsync(db, dto.Activo ? Estado_Usuario.Activo : Estado_Usuario.Inactivo, ct);
            await SincronizarPermisosAsync(db, u, dto, ct);
            u.USU_Version++;

            await GuardarCambiosAsync(db, ct, nameof(dto.Email), "email");
            BitacoraSeguridad.Registrar(BitacoraSeguridad.UsuarioModificado, DescribirPermisos(u));
            return id;
        }

        public async Task<UsuarioEdicionDTO?> ReactivarAsync(int id, CancellationToken ct = default)
        {
            await CambiarEstadoAsync(id, activo: true, ct);
            return await ObtenerPorIdAsync(id, ct);
        }

        public async Task CambiarEstadoAsync(int id, bool activo, CancellationToken ct = default)
        {
            PermisoService.Instancia.Exigir("EliminarUsuario");
            await using var db = new Libreria();
            var u = await db.Usuarios.Include(x => x.Grupos).FirstOrDefaultAsync(x => x.USU_ID == id, ct)
                ?? throw new ConcurrenciaException("La cuenta ya no existe.");

            var errores = new Errores();
            await ValidarProteccionesAsync(db, u, activo, u.Grupos.Select(g => g.GRU_ID).ToList(), errores, ct);
            errores.Lanzar();

            u.EST_USU_ID = await IdEstadoAsync(db, activo ? Estado_Usuario.Activo : Estado_Usuario.Inactivo, ct);
            if (!activo)
            {
                u.USU_ClaveTemporal = null;   // una recuperación pendiente no sirve para una cuenta dada de baja
                u.USU_ClaveTemporalVence = null;
            }
            u.USU_Version++;
            await GuardarCambiosAsync(db, ct);
            BitacoraSeguridad.Registrar(activo ? BitacoraSeguridad.UsuarioReactivado : BitacoraSeguridad.UsuarioDadoDeBaja, u.USU_Nombre);
        }

        private async Task ValidarDatosAsync(Libreria db, UsuarioEdicionDTO dto, int? id, Errores errores, CancellationToken ct)
        {
            var email = dto.Email?.Trim() ?? "";
            errores.Requerido(dto.NombreCompleto, nameof(dto.NombreCompleto), "El nombre completo", LargoMaximo);
            errores.Requerido(email, nameof(dto.Email), "El email", LargoMaximo);
            errores.Si(email.Length > 0 && !Identificadores.EsEmailValido(email), nameof(dto.Email), "El email no tiene un formato válido.");
            errores.Si(!Identificadores.EsDniValido(dto.Dni), nameof(dto.Dni), "El DNI debe tener 7 u 8 dígitos.");
            errores.Si(!string.IsNullOrWhiteSpace(dto.Telefono) && !Identificadores.EsTelefonoValido(dto.Telefono), nameof(dto.Telefono),
                "El teléfono no es válido (sólo números, espacios, guiones, paréntesis y +).");
            errores.Si(dto.GrupoIds.Count == 0, nameof(dto.GrupoIds), "Asigná al menos un grupo.");

            if (!errores.HayErrores)
                errores.Si(await db.Usuarios.AnyAsync(u => u.USU_Mail == email && u.USU_ID != (id ?? 0), ct),
                    nameof(dto.Email), "Ese email ya está registrado en otra cuenta.");
        }

        /// <summary>Nadie se da de baja a sí mismo, y siempre queda al menos un administrador activo.</summary>
        private static async Task ValidarProteccionesAsync(Libreria db, Usuario u, bool quedaActivo, IReadOnlyCollection<int> grupoIds,
            Errores errores, CancellationToken ct)
        {
            errores.Si(u.USU_ID == OperadorId && !quedaActivo, nameof(UsuarioEdicionDTO.Activo), "No podés dar de baja tu propia cuenta.");

            var adminId = await db.Grupos
                .Where(g => g.GRU_Nombre == Grupo.NombreAdministrador)
                .Select(g => (int?)g.GRU_ID)
                .FirstOrDefaultAsync(ct);
            if (adminId is not int admin) return;

            var eraAdmin = u.Grupos.Any(g => g.GRU_ID == admin);
            var sigueAdmin = quedaActivo && grupoIds.Contains(admin);
            if (!eraAdmin || sigueAdmin) return;

            var quedanOtros = await db.Usuarios.AnyAsync(x => x.USU_ID != u.USU_ID
                && (x.Estado_Usuario == null || x.Estado_Usuario.EST_USU_Nombre != Estado_Usuario.Inactivo)
                && x.Grupos.Any(g => g.GRU_ID == admin), ct);
            errores.Si(!quedanOtros, nameof(UsuarioEdicionDTO.GrupoIds),
                "Es el último administrador activo: asigná otro administrador antes de quitarle el acceso.");
        }

        /// <summary>Grupos y acciones directas por ID: sólo cambian las filas que difieren.</summary>
        private static async Task SincronizarPermisosAsync(Libreria db, Usuario u, UsuarioEdicionDTO dto, CancellationToken ct)
        {
            var gruposDeseados = dto.GrupoIds.Distinct().ToList();
            var accionesDeseadas = dto.AccionIds.Distinct().ToList();
            var grupos = await db.Grupos.Where(g => gruposDeseados.Contains(g.GRU_ID)).ToListAsync(ct);
            var acciones = await db.Acciones.Where(a => accionesDeseadas.Contains(a.ACC_ID)).ToListAsync(ct);

            SincronizadorColecciones.Sincronizar(u.Grupos, gruposDeseados, g => g.GRU_ID,
                ids => grupos.Where(g => ids.Contains(g.GRU_ID)));
            SincronizadorColecciones.Sincronizar(u.Acciones, accionesDeseadas, a => a.ACC_ID,
                ids => acciones.Where(a => ids.Contains(a.ACC_ID)));
        }

        private static async Task<int> IdEstadoAsync(Libreria db, string nombre, CancellationToken ct) =>
            await db.Estados_Usuarios.Where(e => e.EST_USU_Nombre == nombre).Select(e => (int?)e.EST_USU_ID).FirstOrDefaultAsync(ct)
            ?? throw new InvalidOperationException($"Falta el estado de usuario \"{nombre}\": aplicá las migraciones.");

        private static bool MismosIds(IEnumerable<int> a, IEnumerable<int> b) => a.ToHashSet().SetEquals(b);

        private static string DescribirPermisos(Usuario u) =>
            $"{u.USU_Nombre} · grupos: [{string.Join(", ", u.Grupos.Select(g => g.GRU_Nombre))}]" +
            $" · acciones directas: [{string.Join(", ", u.Acciones.Select(a => a.ACC_Nombre))}]";

        #endregion

        #region Clave y bloqueo

        public async Task<ResultadoClaveTemporal> ResetearClaveAsync(int id, CancellationToken ct = default)
        {
            PermisoService.Instancia.Exigir("ResetearClave");
            if (id == OperadorId)
                throw new ValidacionException("Clave", "Para tu propia cuenta usá \"Mi clave\" en el menú.");

            await using var db = new Libreria();
            var u = await db.Usuarios.Include(x => x.USU_Persona).FirstOrDefaultAsync(x => x.USU_ID == id, ct)
                ?? throw new ConcurrenciaException("La cuenta ya no existe.");

            var temporal = ServiciosUsuario.GenerarPassword();
            u.USU_Clave = HasherClaves.Hashear(temporal);
            u.USU_DebeCambiarClave = true;
            u.USU_IntentosFallidos = 0;
            u.USU_BloqueadoHasta = null;
            u.USU_ClaveTemporal = null;
            u.USU_ClaveTemporalVence = null;
            u.USU_Version++;
            await GuardarCambiosAsync(db, ct);
            BitacoraSeguridad.Registrar(BitacoraSeguridad.ClaveReseteada, u.USU_Nombre);

            var enviado = ServiciosUsuario.SendMail(u, temporal);
            return new ResultadoClaveTemporal(id, enviado, enviado ? null : temporal);
        }

        public async Task DesbloquearAsync(int id, CancellationToken ct = default)
        {
            PermisoService.Instancia.Exigir("ModificarUsuario");
            await using var db = new Libreria();
            var u = await db.Usuarios.FirstOrDefaultAsync(x => x.USU_ID == id, ct)
                ?? throw new ConcurrenciaException("La cuenta ya no existe.");
            u.USU_BloqueadoHasta = null;
            u.USU_IntentosFallidos = 0;
            u.USU_Version++;
            await GuardarCambiosAsync(db, ct);
            BitacoraSeguridad.Registrar(BitacoraSeguridad.UsuarioDesbloqueado, u.USU_Nombre);
        }

        #endregion
    }
}
