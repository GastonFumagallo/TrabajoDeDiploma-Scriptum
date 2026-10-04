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

        /// <summary>Alta con clave temporal generada (cambio obligatorio al entrar). Se envía por mail después de guardar.</summary>
        Task<ResultadoClaveTemporal> CrearAsync(UsuarioEdicionDTO dto, CancellationToken ct = default);
        /// <summary>Datos, estado, grupos y permisos. Nunca toca la clave ni el nombre de usuario.</summary>
        Task<int> EditarAsync(UsuarioEdicionDTO dto, int operadorId, CancellationToken ct = default);
        Task<UsuarioEdicionDTO?> ReactivarAsync(int id, CancellationToken ct = default);
        Task CambiarEstadoAsync(int id, bool activo, int operadorId, CancellationToken ct = default);

        /// <summary>Blanqueo administrativo: clave temporal nueva, cambio obligatorio y desbloqueo.</summary>
        Task<ResultadoClaveTemporal> ResetearClaveAsync(int id, int operadorId, CancellationToken ct = default);
        Task DesbloquearAsync(int id, CancellationToken ct = default);

        Task<ResultadoLogin> AutenticarAsync(string usuario, string clave, CancellationToken ct = default);
        /// <summary>Usuario con todo lo que necesitan PermisoService y la sesión (grupos, acciones, persona).</summary>
        Task<Usuario?> ObtenerUsuarioSesionAsync(int id, CancellationToken ct = default);
        /// <summary>Cambio por el propio usuario (también el obligatorio del primer ingreso): exige la clave actual.</summary>
        Task CambiarClaveAsync(int id, string claveActual, string claveNueva, CancellationToken ct = default);

        /// <summary>Envía un código si usuario y email coinciden. No informa si existen (la respuesta es siempre la misma).</summary>
        Task SolicitarCodigoRecuperacionAsync(string usuario, string email, CancellationToken ct = default);
        /// <summary>Cambia la clave si el código es correcto y no venció.</summary>
        Task RecuperarClaveAsync(string usuario, string codigo, string claveNueva, CancellationToken ct = default);
    }

    /// <summary>
    /// ABM y seguridad de cuentas de usuario. Lecturas AsNoTracking proyectadas a DTO (sin N+1); escrituras con un
    /// DbContext por operación. Reglas: nombre de usuario y email únicos, sin borrado físico (baja lógica), nadie se
    /// desactiva a sí mismo y siempre queda al menos un administrador activo. Claves con PBKDF2 (<see cref="HasherClaves"/>),
    /// bloqueo temporal por intentos fallidos y recuperación por código con vencimiento.
    /// </summary>
    public sealed class UsuarioService : ServicioAbmBase, IUsuarioService
    {
        public const int IntentosMaximos = 5;
        public static readonly TimeSpan DuracionBloqueo = TimeSpan.FromMinutes(15);
        public const int MinutosVigenciaCodigo = 15;
        private const int LargoMaximo = 60;
        private static readonly Regex FormatoUsuario = new(@"^[A-Za-z0-9._-]{3,60}$", RegexOptions.Compiled);

        private static UsuarioService? instancia;
        public static UsuarioService Instancia => instancia ??= new UsuarioService();

        protected override string NombreEntidad => "el usuario";

        public const string ReglaNombreUsuario =
            "El usuario debe tener entre 3 y 60 caracteres: letras, números, punto, guion o guion bajo (sin espacios).";

        public static bool EsNombreUsuarioValido(string? usuario) => usuario != null && FormatoUsuario.IsMatch(usuario.Trim());

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
                q = q.Where(u => u.Estado_Usuario.EST_USU_Nombre == Estado_Usuario.Activo);
            else if (f.Estado == FiltroEstadoActivo.Inactivos)
                q = q.Where(u => u.Estado_Usuario.EST_USU_Nombre != Estado_Usuario.Activo);

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
                    Activo = u.Estado_Usuario.EST_USU_Nombre == Estado_Usuario.Activo,
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
                    Activo = u.Estado_Usuario.EST_USU_Nombre == Estado_Usuario.Activo,
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

        public async Task<Usuario?> ObtenerUsuarioSesionAsync(int id, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await db.Usuarios
                .AsNoTrackingWithIdentityResolution()
                .AsSplitQuery()
                .Include(u => u.USU_Persona)
                .Include(u => u.Estado_Usuario)
                .Include(u => u.Grupos).ThenInclude(g => g.Acciones).ThenInclude(a => a.Formulario)
                .Include(u => u.Grupos).ThenInclude(g => g.Estado_Grupo)
                .Include(u => u.Acciones).ThenInclude(a => a.Formulario)
                .FirstOrDefaultAsync(u => u.USU_ID == id, ct);
        }

        #endregion

        #region Alta, edición y estado

        public async Task<ResultadoClaveTemporal> CrearAsync(UsuarioEdicionDTO dto, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(dto);
            await using var db = new Libreria();
            var nombre = dto.Usuario?.Trim() ?? "";
            var email = dto.Email?.Trim() ?? "";

            // Si el usuario o el email son de una cuenta dada de baja, se ofrece reactivarla en vez de duplicarla.
            var inactivo = await db.Usuarios.AsNoTracking()
                .Where(u => (u.USU_Nombre == nombre || u.USU_Mail == email) && u.Estado_Usuario.EST_USU_Nombre != Estado_Usuario.Activo)
                .Select(u => new { u.USU_ID, u.USU_Nombre })
                .FirstOrDefaultAsync(ct);
            if (inactivo != null)
                throw new EntidadInactivaException(inactivo.USU_ID,
                    $"Ya existe la cuenta \"{inactivo.USU_Nombre}\" con ese usuario o email, pero está desactivada.");

            var errores = new Errores();
            errores.Requerido(nombre, nameof(dto.Usuario), "El nombre de usuario", LargoMaximo);
            errores.Si(nombre.Length > 0 && !FormatoUsuario.IsMatch(nombre), nameof(dto.Usuario), ReglaNombreUsuario);
            await ValidarDatosAsync(db, dto, null, errores, ct);
            if (!errores.HayErrores)
                errores.Si(await db.Usuarios.AnyAsync(u => u.USU_Nombre == nombre, ct), nameof(dto.Usuario), "Ese nombre de usuario ya está en uso.");
            errores.Lanzar();

            var temporal = HasherClaves.GenerarTemporal();
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

            // El mail sale DESPUÉS de guardar: nunca llega una clave que no funciona.
            var enviado = ServiciosUsuario.EnviarClaveTemporal(usuario.USU_Persona.PER_Nombre, email, nombre, temporal, esAlta: true);
            return new ResultadoClaveTemporal(usuario.USU_ID, enviado, enviado ? null : temporal);
        }

        public async Task<int> EditarAsync(UsuarioEdicionDTO dto, int operadorId, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(dto);
            if (dto.Id is not int id) throw new ArgumentException("Para un alta usá CrearAsync.", nameof(dto));

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
            await ValidarProteccionesAsync(db, u, dto.Activo, dto.GrupoIds, operadorId, errores, ct);
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
            return id;
        }

        public async Task<UsuarioEdicionDTO?> ReactivarAsync(int id, CancellationToken ct = default)
        {
            await using (var db = new Libreria())
            {
                var u = await db.Usuarios.FirstOrDefaultAsync(x => x.USU_ID == id, ct);
                if (u == null) return null;
                u.EST_USU_ID = await IdEstadoAsync(db, Estado_Usuario.Activo, ct);
                u.USU_Version++;
                await GuardarCambiosAsync(db, ct);
            }
            return await ObtenerPorIdAsync(id, ct);
        }

        public async Task CambiarEstadoAsync(int id, bool activo, int operadorId, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            var u = await db.Usuarios.Include(x => x.Grupos).FirstOrDefaultAsync(x => x.USU_ID == id, ct)
                ?? throw new ConcurrenciaException("La cuenta ya no existe.");

            var errores = new Errores();
            await ValidarProteccionesAsync(db, u, activo, u.Grupos.Select(g => g.GRU_ID).ToList(), operadorId, errores, ct);
            errores.Lanzar();

            u.EST_USU_ID = await IdEstadoAsync(db, activo ? Estado_Usuario.Activo : Estado_Usuario.Inactivo, ct);
            u.USU_Version++;
            await GuardarCambiosAsync(db, ct);
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

        /// <summary>Nadie se desactiva a sí mismo, y siempre queda al menos un administrador activo.</summary>
        private static async Task ValidarProteccionesAsync(Libreria db, Usuario u, bool quedaActivo, IReadOnlyCollection<int> grupoIds,
            int operadorId, Errores errores, CancellationToken ct)
        {
            errores.Si(u.USU_ID == operadorId && !quedaActivo, nameof(UsuarioEdicionDTO.Activo), "No podés desactivar tu propia cuenta.");

            var adminId = await db.Grupos
                .Where(g => g.GRU_Nombre == Grupo.NombreAdministrador)
                .Select(g => (int?)g.GRU_ID)
                .FirstOrDefaultAsync(ct);
            if (adminId is not int admin) return;

            var eraAdmin = u.Grupos.Any(g => g.GRU_ID == admin);
            var sigueAdmin = quedaActivo && grupoIds.Contains(admin);
            if (!eraAdmin || sigueAdmin) return;

            errores.Si(u.USU_ID == operadorId, nameof(UsuarioEdicionDTO.GrupoIds),
                $"No podés quitarte el grupo {Grupo.NombreAdministrador}: otro administrador tiene que hacerlo.");
            var quedanOtros = await db.Usuarios.AnyAsync(x => x.USU_ID != u.USU_ID
                && x.Estado_Usuario.EST_USU_Nombre == Estado_Usuario.Activo
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

        #endregion

        #region Claves y bloqueo

        public async Task<ResultadoClaveTemporal> ResetearClaveAsync(int id, int operadorId, CancellationToken ct = default)
        {
            if (id == operadorId)
                throw new ValidacionException("Clave", "Para tu propia cuenta usá \"Cambiar mi clave\".");

            await using var db = new Libreria();
            var u = await db.Usuarios.Include(x => x.USU_Persona).FirstOrDefaultAsync(x => x.USU_ID == id, ct)
                ?? throw new ConcurrenciaException("La cuenta ya no existe.");

            var temporal = HasherClaves.GenerarTemporal();
            u.USU_Clave = HasherClaves.Hashear(temporal);
            u.USU_DebeCambiarClave = true;
            u.USU_IntentosFallidos = 0;
            u.USU_BloqueadoHasta = null;
            u.USU_CodigoRecuperacion = null;
            u.USU_CodigoVence = null;
            u.USU_Version++;
            await GuardarCambiosAsync(db, ct);

            var enviado = ServiciosUsuario.EnviarClaveTemporal(u.USU_Persona?.PER_Nombre ?? u.USU_Nombre, u.USU_Mail, u.USU_Nombre, temporal, esAlta: false);
            return new ResultadoClaveTemporal(id, enviado, enviado ? null : temporal);
        }

        public async Task DesbloquearAsync(int id, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            await db.Usuarios
                .Where(u => u.USU_ID == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.USU_BloqueadoHasta, (DateTime?)null)
                    .SetProperty(u => u.USU_IntentosFallidos, 0)
                    .SetProperty(u => u.USU_Version, u => u.USU_Version + 1), ct);
        }

        public async Task<ResultadoLogin> AutenticarAsync(string usuario, string clave, CancellationToken ct = default)
        {
            var nombre = usuario?.Trim() ?? "";
            await using var db = new Libreria();
            var u = await db.Usuarios.Include(x => x.Estado_Usuario).FirstOrDefaultAsync(x => x.USU_Nombre == nombre, ct);
            if (u is null)
            {
                HasherClaves.Hashear(clave ?? "");   // mismo costo que con un usuario existente: no revela qué nombres existen
                return new ResultadoLogin(EstadoLogin.Invalido);
            }

            var ahora = DateTime.Now;
            if (u.USU_BloqueadoHasta > ahora)
                return new ResultadoLogin(EstadoLogin.Bloqueado, BloqueadoHasta: u.USU_BloqueadoHasta);

            if (!HasherClaves.Verificar(clave ?? "", u.USU_Clave, out var rehash))
            {
                RegistrarIntentoFallido(u, ahora);
                await db.SaveChangesAsync(ct);
                return u.USU_BloqueadoHasta > ahora
                    ? new ResultadoLogin(EstadoLogin.Bloqueado, BloqueadoHasta: u.USU_BloqueadoHasta)
                    : new ResultadoLogin(EstadoLogin.Invalido);
            }

            if (u.Estado_Usuario?.EST_USU_Nombre != Estado_Usuario.Activo)
                return new ResultadoLogin(EstadoLogin.Inactivo);

            u.USU_IntentosFallidos = 0;
            u.USU_BloqueadoHasta = null;
            u.USU_UltimoAcceso = ahora;
            if (rehash) u.USU_Clave = HasherClaves.Hashear(clave!);   // migra el hash viejo sin pedirle nada al usuario
            await db.SaveChangesAsync(ct);
            return new ResultadoLogin(EstadoLogin.Ok, u.USU_ID, u.USU_DebeCambiarClave);
        }

        private static void RegistrarIntentoFallido(Usuario u, DateTime ahora)
        {
            u.USU_IntentosFallidos++;
            if (u.USU_IntentosFallidos >= IntentosMaximos)
            {
                u.USU_BloqueadoHasta = ahora + DuracionBloqueo;
                u.USU_IntentosFallidos = 0;
            }
        }

        public async Task CambiarClaveAsync(int id, string claveActual, string claveNueva, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            var u = await db.Usuarios.FirstOrDefaultAsync(x => x.USU_ID == id, ct)
                ?? throw new ConcurrenciaException("La cuenta ya no existe.");

            var errores = new Errores();
            errores.Si(!HasherClaves.Verificar(claveActual ?? "", u.USU_Clave, out _), "ClaveActual", "La clave actual no es correcta.");
            var fortaleza = HasherClaves.ValidarFortaleza(claveNueva, u.USU_Nombre);
            errores.Si(fortaleza != null, "ClaveNueva", fortaleza ?? "");
            errores.Si(fortaleza == null && HasherClaves.Verificar(claveNueva, u.USU_Clave, out _), "ClaveNueva",
                "La clave nueva tiene que ser distinta de la actual.");
            errores.Lanzar();

            u.USU_Clave = HasherClaves.Hashear(claveNueva);
            u.USU_DebeCambiarClave = false;
            u.USU_Version++;
            await GuardarCambiosAsync(db, ct);
        }

        public async Task SolicitarCodigoRecuperacionAsync(string usuario, string email, CancellationToken ct = default)
        {
            var nombre = usuario?.Trim() ?? "";
            var mail = email?.Trim() ?? "";
            await using var db = new Libreria();
            var u = await db.Usuarios
                .Include(x => x.USU_Persona)
                .Include(x => x.Estado_Usuario)
                .FirstOrDefaultAsync(x => x.USU_Nombre == nombre && x.USU_Mail == mail, ct);

            // Cuentas inexistentes, desactivadas o bloqueadas: no se envía nada y la respuesta es la misma.
            if (u is null || u.Estado_Usuario?.EST_USU_Nombre != Estado_Usuario.Activo || u.USU_BloqueadoHasta > DateTime.Now)
                return;

            var codigo = HasherClaves.GenerarCodigo();
            u.USU_CodigoRecuperacion = HasherClaves.Hashear(codigo);
            u.USU_CodigoVence = DateTime.Now.AddMinutes(MinutosVigenciaCodigo);
            await db.SaveChangesAsync(ct);

            ServiciosUsuario.EnviarCodigoRecuperacion(u.USU_Persona?.PER_Nombre ?? u.USU_Nombre, u.USU_Mail, codigo, MinutosVigenciaCodigo);
        }

        public async Task RecuperarClaveAsync(string usuario, string codigo, string claveNueva, CancellationToken ct = default)
        {
            const string Invalido = "El código es incorrecto o venció. Pedí uno nuevo.";
            var nombre = usuario?.Trim() ?? "";
            await using var db = new Libreria();
            var u = await db.Usuarios.FirstOrDefaultAsync(x => x.USU_Nombre == nombre, ct);
            var ahora = DateTime.Now;

            if (u is null || u.USU_CodigoRecuperacion is null || !(u.USU_CodigoVence > ahora))
                throw new ValidacionException("Codigo", Invalido);
            if (u.USU_BloqueadoHasta > ahora)
                throw new ValidacionException("Codigo", $"La cuenta está bloqueada por intentos fallidos hasta las {u.USU_BloqueadoHasta:HH:mm}.");

            if (!HasherClaves.Verificar(codigo?.Trim() ?? "", u.USU_CodigoRecuperacion, out _))
            {
                RegistrarIntentoFallido(u, ahora);
                if (u.USU_BloqueadoHasta > ahora)
                {
                    u.USU_CodigoRecuperacion = null;   // con el bloqueo, el código queda invalidado
                    u.USU_CodigoVence = null;
                }
                await db.SaveChangesAsync(ct);
                throw new ValidacionException("Codigo", Invalido);
            }

            var fortaleza = HasherClaves.ValidarFortaleza(claveNueva, u.USU_Nombre);
            if (fortaleza != null)
                throw new ValidacionException("ClaveNueva", fortaleza);

            u.USU_Clave = HasherClaves.Hashear(claveNueva);
            u.USU_DebeCambiarClave = false;
            u.USU_CodigoRecuperacion = null;
            u.USU_CodigoVence = null;
            u.USU_IntentosFallidos = 0;
            u.USU_Version++;
            await GuardarCambiosAsync(db, ct);
        }

        #endregion
    }
}
