using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using Modelo.Seguridad;
using Servicios;
using System.Collections.ObjectModel;

namespace Controladora
{
    /// <summary>Resultado del login. Si <see cref="Error"/> no es null, el acceso fue rechazado.</summary>
    public sealed record ResultadoLogin(Usuario? Usuario, string? Error, bool DebeCambiarClave)
    {
        public bool Exitoso => Error == null;
    }

    public class ControladoraUsuarios
    {
        public const string EstadoInactivo = "Inactivo";
        private const int MaxIntentosFallidos = 5;
        private const int MinutosBloqueo = 15;
        private const int MinutosVigenciaRecuperacion = 30;
        private const string MensajeCredencialesInvalidas = "Usuario o clave incorrecta.";

        private static ControladoraUsuarios instancia;

        public static ControladoraUsuarios Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new ControladoraUsuarios();
                }
                return instancia;
            }
        }

        /// <summary>
        /// Usuario con todo lo que usan el login, PermisoService y FrmUsuario: persona, estado,
        /// grupos (con estado y acciones con formulario) y acciones directas. Se devuelve desconectado:
        /// no hay lazy loading, así que todo lo que la UI recorre tiene que venir incluido acá.
        /// </summary>
        private static IQueryable<Usuario> UsuariosConPermisos(Libreria db)
        {
            return db.Usuarios
                .AsNoTrackingWithIdentityResolution()
                .AsSplitQuery()
                .Include(u => u.USU_Persona)
                .Include(u => u.Estado_Usuario)
                .Include(u => u.Grupos)
                    .ThenInclude(g => g.Acciones)
                    .ThenInclude(a => a.Formulario)
                .Include(u => u.Grupos)
                    .ThenInclude(g => g.Estado_Grupo)
                .Include(u => u.Acciones)
                    .ThenInclude(a => a.Formulario);
        }

        /// <summary>Los hashes no salen de la capa de negocio: no quedan en la sesión ni en la UI.</summary>
        private static Usuario? SinSecretos(Usuario? usuario)
        {
            if (usuario != null)
            {
                usuario.USU_Clave = null!;
                usuario.USU_ClaveTemporal = null;
            }
            return usuario;
        }

        // El filtro en SQL depende de la intercalación (suele ignorar mayúsculas); se confirma exacto en memoria.
        private static Usuario? BuscarPorNombre(IQueryable<Usuario> usuarios, string nombre) =>
            usuarios.Where(x => x.USU_Nombre == nombre)
                .AsEnumerable()
                .FirstOrDefault(x => x.USU_Nombre.Equals(nombre));

        #region Autenticación

        /// <summary>
        /// Valida las credenciales con bloqueo por intentos fallidos. Si el hash guardado es del formato viejo
        /// (SHA-256 sin salt) lo reemplaza por uno PBKDF2. Toda la operación queda en AuditoriaSeguridad.
        /// </summary>
        public ResultadoLogin IniciarSesion(string nombre, string clave)
        {
            var ahora = DateTime.Now;
            int usuarioId;
            bool debeCambiarClave;
            bool conClaveTemporal;

            using (var db = new Libreria())
            {
                var usuario = BuscarPorNombre(db.Usuarios.Include(u => u.Estado_Usuario), nombre);
                if (usuario == null)
                {
                    HasherClaves.SimularVerificacion(clave);   // misma demora: no revela si el usuario existe
                    BitacoraSeguridad.Registrar(BitacoraSeguridad.LoginFallido, "Usuario inexistente", nombre);
                    return Rechazo(MensajeCredencialesInvalidas);
                }

                if (usuario.USU_BloqueadoHasta > ahora)
                {
                    BitacoraSeguridad.Registrar(BitacoraSeguridad.LoginRechazado, "Usuario bloqueado", nombre);
                    int minutos = (int)Math.Ceiling((usuario.USU_BloqueadoHasta.Value - ahora).TotalMinutes);
                    return Rechazo($"El usuario está bloqueado por intentos fallidos. Intente nuevamente en {minutos} minuto(s).");
                }

                bool claveOk = HasherClaves.Verificar(clave, usuario.USU_Clave, out bool requiereRehash);
                conClaveTemporal = !claveOk && usuario.USU_ClaveTemporalVence > ahora &&
                                   HasherClaves.Verificar(clave, usuario.USU_ClaveTemporal, out _);

                if (!claveOk && !conClaveTemporal)
                {
                    usuario.USU_IntentosFallidos++;
                    if (usuario.USU_IntentosFallidos >= MaxIntentosFallidos)
                    {
                        usuario.USU_IntentosFallidos = 0;
                        usuario.USU_BloqueadoHasta = ahora.AddMinutes(MinutosBloqueo);
                        db.SaveChanges();
                        BitacoraSeguridad.Registrar(BitacoraSeguridad.UsuarioBloqueado, $"Bloqueado {MinutosBloqueo} minutos", nombre);
                        return Rechazo($"Demasiados intentos fallidos. El usuario quedó bloqueado {MinutosBloqueo} minutos.");
                    }
                    db.SaveChanges();
                    BitacoraSeguridad.Registrar(BitacoraSeguridad.LoginFallido,
                        $"Intento {usuario.USU_IntentosFallidos} de {MaxIntentosFallidos}", nombre);
                    return Rechazo(MensajeCredencialesInvalidas);
                }

                // Recién acá se informa el estado: la clave ya se verificó, no le revela nada a un extraño.
                if (usuario.Estado_Usuario?.EST_USU_Nombre == EstadoInactivo)
                {
                    BitacoraSeguridad.Registrar(BitacoraSeguridad.LoginRechazado, "Usuario dado de baja", nombre);
                    return Rechazo("El usuario está dado de baja.");
                }

                usuario.USU_IntentosFallidos = 0;
                usuario.USU_BloqueadoHasta = null;
                if (conClaveTemporal)
                {
                    usuario.USU_Clave = usuario.USU_ClaveTemporal!;
                    usuario.USU_DebeCambiarClave = true;
                }
                else if (requiereRehash)
                {
                    usuario.USU_Clave = HasherClaves.Hashear(clave);
                }
                // Entrar (con cualquiera de las dos claves) invalida una recuperación pendiente.
                usuario.USU_ClaveTemporal = null;
                usuario.USU_ClaveTemporalVence = null;
                db.SaveChanges();

                usuarioId = usuario.USU_ID;
                debeCambiarClave = usuario.USU_DebeCambiarClave;
            }

            Usuario? conPermisos;
            using (var db = new Libreria())
                conPermisos = SinSecretos(UsuariosConPermisos(db).FirstOrDefault(u => u.USU_ID == usuarioId));

            BitacoraSeguridad.Registrar(BitacoraSeguridad.LoginOk, conClaveTemporal ? "Con clave de recuperación" : null, nombre);
            return new ResultadoLogin(conPermisos, null, debeCambiarClave);

            static ResultadoLogin Rechazo(string mensaje) => new(null, mensaje, false);
        }

        /// <summary>Cambio de clave por el propio usuario: exige la clave actual y aplica la política de claves.</summary>
        public void CambiarClave(int usuarioId, string claveActual, string claveNueva)
        {
            using var db = new Libreria();
            var usuario = db.Usuarios.FirstOrDefault(u => u.USU_ID == usuarioId)
                ?? throw new ValidacionException("ClaveActual", "El usuario no existe.");

            if (!HasherClaves.Verificar(claveActual, usuario.USU_Clave, out _))
                throw new ValidacionException("ClaveActual", "La clave actual es incorrecta.");

            var error = PoliticaClaves.Validar(claveNueva, usuario.USU_Nombre);
            if (error != null)
                throw new ValidacionException("ClaveNueva", error);

            if (HasherClaves.Verificar(claveNueva, usuario.USU_Clave, out _))
                throw new ValidacionException("ClaveNueva", "La clave nueva debe ser distinta de la actual.");

            usuario.USU_Clave = HasherClaves.Hashear(claveNueva);
            usuario.USU_DebeCambiarClave = false;
            db.SaveChanges();
            BitacoraSeguridad.Registrar(BitacoraSeguridad.ClaveCambiada, null, usuario.USU_Nombre);
        }

        /// <summary>
        /// Recuperación desde el login. Envía una clave temporal que vence en 30 minutos y convive con la actual:
        /// quien conozca el nombre y el email de otro no puede dejarlo sin acceso. No informa si los datos existen.
        /// </summary>
        public void SolicitarRecuperacion(string nombre, string mail)
        {
            using var db = new Libreria();
            var usuario = BuscarPorNombre(db.Usuarios.Include(u => u.USU_Persona), nombre);
            if (usuario == null || !string.Equals(usuario.USU_Mail, mail, StringComparison.OrdinalIgnoreCase))
            {
                BitacoraSeguridad.Registrar(BitacoraSeguridad.RecuperacionSolicitada, "Usuario o email inexistentes", nombre);
                return;
            }

            var temporal = ServiciosUsuario.GenerarPassword();
            if (!ServiciosUsuario.SendMail(usuario, temporal))
            {
                BitacoraSeguridad.Registrar(BitacoraSeguridad.RecuperacionSolicitada, "No se pudo enviar el email", nombre);
                return;
            }

            usuario.USU_ClaveTemporal = HasherClaves.Hashear(temporal);
            usuario.USU_ClaveTemporalVence = DateTime.Now.AddMinutes(MinutosVigenciaRecuperacion);
            db.SaveChanges();
            BitacoraSeguridad.Registrar(BitacoraSeguridad.RecuperacionSolicitada, "Clave temporal enviada", nombre);
        }

        #endregion

        #region Consultas

        public List<UsuarioDTO> FiltrarUsuarios(
        string nombreUsuario,
        int? idGrupo,
        int? idEstado)
        {
            using var db = new Libreria();
            var query = db.Usuarios.AsNoTracking();

            // Filtro por nombre
            if (!string.IsNullOrWhiteSpace(nombreUsuario))
            {
                query = query.Where(u =>
                    u.USU_Persona.PER_Nombre.Contains(nombreUsuario));
            }

            // Filtro por grupo
            if (idGrupo.HasValue && idGrupo.Value != 0)
            {
                query = query.Where(u =>
                    u.Grupos.Any(g =>
                        g.GRU_ID == idGrupo.Value));
            }

            // Filtro por estado
            if (idEstado.HasValue && idEstado.Value != 0)
            {
                query = query.Where(u =>
                    u.Estado_Usuario.EST_USU_ID == idEstado.Value);
            }

            return query.Select(p => new UsuarioDTO
            {
                USUDTO_ID = p.USU_ID,
                NombrePersona = p.USU_Persona.PER_Nombre,
                Usuario = p.USU_Nombre,
                Mail = p.USU_Persona.PER_Mail,
                Estado = p.Estado_Usuario.EST_USU_Nombre,
            }).ToList();
        }

        public List<UsuarioDTO> obtenerUsuariosGrid()
        {
            using var db = new Libreria();
            return db.Usuarios.AsNoTracking()
            .Select(p => new UsuarioDTO
            {
                USUDTO_ID = p.USU_ID,
                NombrePersona = p.USU_Persona.PER_Nombre,
                Usuario = p.USU_Nombre,
                Mail = p.USU_Persona.PER_Mail,
                Estado = p.Estado_Usuario.EST_USU_Nombre,
            }).ToList();
        }

        public List<UsuarioDTO> FiltrarUsuariosPorGrupo(string nombreUsuario, int? idGrupo)
        {
            using var db = new Libreria();
            var usuarios = db.Usuarios.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(nombreUsuario))
            {
                usuarios = usuarios.Where(u =>u.USU_Persona.PER_Nombre.Contains(nombreUsuario));
            }

            if (idGrupo.HasValue)
            {
                usuarios = usuarios.Where(u =>u.Grupos.Any(g =>g.GRU_ID == idGrupo.Value));
            }

            return usuarios.Select(u => new UsuarioDTO
            {
                USUDTO_ID = u.USU_ID,
                NombrePersona = u.USU_Persona.PER_Nombre,
                Usuario = u.USU_Nombre,
                Mail = u.USU_Persona.PER_Mail,
                Estado = u.Estado_Usuario.EST_USU_Nombre,
            }).ToList();
        }
        public ReadOnlyCollection<UsuarioDTO> getAllUsuariosByGrupo(string nombre, Grupo grupo)
        {
            using var db = new Libreria();
            var usuarios = db.Usuarios.AsNoTracking()
            .Where(x => x.USU_Persona.PER_Nombre == nombre && x.Grupos.Any(g => g.GRU_ID == grupo.GRU_ID))
            .Select(p => new UsuarioDTO
            {
                USUDTO_ID = p.USU_ID,
                NombrePersona = p.USU_Persona.PER_Nombre,
                Usuario = p.USU_Nombre,
                Mail = p.USU_Persona.PER_Mail,
                Estado = p.Estado_Usuario.EST_USU_Nombre,
            }).ToList();
            return usuarios.AsReadOnly();
        }

        public ReadOnlyCollection<Estado_Usuario> getAllEstadosUsuario()
        {
            using var db = new Libreria();
            return db.Estados_Usuarios.AsNoTracking().ToList().AsReadOnly();
        }

        public Usuario buscarUsuarioIndividual(UsuarioDTO usuarioSeleccionado)
        {
            using var db = new Libreria();
            return SinSecretos(UsuariosConPermisos(db)
                          .FirstOrDefault(p => p.USU_ID == usuarioSeleccionado.USUDTO_ID))!;
        }

        public List<UsuarioDTO> buscarUsuario(string filtro)
        {
            return obtenerUsuariosGrid().Where(p => p.NombrePersona.ToString().ToLower().Contains(filtro)).ToList();
        }

        public ReadOnlyCollection<Usuario> getAllUsuarios()
        {
            using var db = new Libreria();
            return UsuariosConPermisos(db)
            .AsEnumerable()
            .Select(u => SinSecretos(u)!)
            .ToList()
            .AsReadOnly();
        }

        #endregion

        #region Administración (exige permisos en esta capa, no solo en la UI)

        public string AgregarUsuario(Usuario usuario)
        {
            PermisoService.Instancia.Exigir("AgregarUsuario");

            using var db = new Libreria();
            if (db.Usuarios.Any(x => x.USU_Mail == usuario.USU_Mail))
            {
                return "Email ya registrado";
            }
            if (db.Usuarios.Any(x => x.USU_Nombre == usuario.USU_Nombre))
            {
                return "Nombre de usuario ya registrado";
            }

            // La clave inicial la genera el servidor de negocio, no la UI, y se exige cambiarla al entrar.
            var claveTemporal = ServiciosUsuario.GenerarPassword();
            if (!ServiciosUsuario.SendMail(usuario, claveTemporal))
            {
                return "No fue posible enviar el email con la clave inicial";
            }

            // Se arma una entidad nueva: el estado, los grupos y las acciones que trae la UI vienen de
            // otras consultas y, si se agregaran tal cual, EF intentaría insertarlos de nuevo.
            var nuevo = new Usuario
            {
                USU_Nombre = usuario.USU_Nombre,
                USU_Mail = usuario.USU_Mail,
                USU_Clave = HasherClaves.Hashear(claveTemporal),
                USU_DebeCambiarClave = true,
                EST_USU_ID = usuario.Estado_Usuario?.EST_USU_ID ?? usuario.EST_USU_ID,
            };
            if (usuario.USU_Persona is { PER_ID: 0 } personaNueva)
                nuevo.USU_Persona = personaNueva;
            else
                nuevo.PER_ID = usuario.USU_Persona?.PER_ID ?? usuario.PER_ID;

            AplicarGruposYAcciones(db, nuevo, usuario);

            db.Usuarios.Add(nuevo);
            db.SaveChanges();
            usuario.USU_ID = nuevo.USU_ID;
            BitacoraSeguridad.Registrar(BitacoraSeguridad.UsuarioCreado, DescribirPermisos(nuevo, usuario.USU_Nombre));
            return "Usuario agregado correctamente";
        }

        /// <summary>
        /// Guarda los datos, la persona, el estado, los grupos y las acciones directas del usuario.
        /// El objeto que llega está desconectado: se carga el registro actual y se le aplican los cambios.
        /// La clave no se toca acá: solo cambia con <see cref="CambiarClave"/> o <see cref="ResetearClaveUsuario"/>.
        /// </summary>
        public bool ModificarUsuario(Usuario usuario)
        {
            PermisoService.Instancia.Exigir("ModificarUsuario");

            using var db = new Libreria();
            var existente = db.Usuarios
                .Include(u => u.USU_Persona)
                .Include(u => u.Grupos)
                .Include(u => u.Acciones)
                .FirstOrDefault(x => x.USU_ID == usuario.USU_ID);
            if (existente == null)
            {
                return false;
            }

            if (db.Usuarios.Any(x => x.USU_Nombre == usuario.USU_Nombre && x.USU_ID != usuario.USU_ID))
                throw new ValidacionException("Usuario", "Ya existe otro usuario con ese nombre.");

            // Nadie puede ampliarse (ni recortarse) sus propios permisos: lo tiene que hacer otro administrador.
            if (usuario.USU_ID == Sesion.Instancia.Usuario?.USU_ID &&
                (!MismosIds(existente.Grupos.Select(g => g.GRU_ID), usuario.Grupos.Select(g => g.GRU_ID)) ||
                 !MismosIds(existente.Acciones.Select(a => a.ACC_ID), usuario.Acciones.Select(a => a.ACC_ID))))
                throw new ValidacionException("Grupos", "No puede modificar sus propios grupos ni permisos.");

            existente.USU_Nombre = usuario.USU_Nombre;
            existente.USU_Mail = usuario.USU_Mail;
            existente.EST_USU_ID = usuario.Estado_Usuario?.EST_USU_ID ?? usuario.EST_USU_ID;

            if (existente.USU_Persona != null && usuario.USU_Persona != null)
            {
                existente.USU_Persona.PER_Nombre = usuario.USU_Persona.PER_Nombre;
                existente.USU_Persona.PER_Mail = usuario.USU_Persona.PER_Mail;
                existente.USU_Persona.PER_Telefono = usuario.USU_Persona.PER_Telefono;
                existente.USU_Persona.PER_DNI = usuario.USU_Persona.PER_DNI;
            }

            AplicarGruposYAcciones(db, existente, usuario);

            db.SaveChanges();
            BitacoraSeguridad.Registrar(BitacoraSeguridad.UsuarioModificado, DescribirPermisos(existente, existente.USU_Nombre));
            return true;
        }

        /// <summary>
        /// Baja lógica: el usuario pasa a estado "Inactivo" y ya no puede entrar, pero se conserva para la
        /// auditoría (sesiones, ventas, movimientos). Antes se borraba y, en cascada, su historial de sesiones.
        /// </summary>
        public string DarDeBajaUsuario(Usuario usuario)
        {
            PermisoService.Instancia.Exigir("EliminarUsuario");

            if (usuario == null)
                return "Usuario no encontrado";
            if (usuario.USU_ID == Sesion.Instancia.Usuario?.USU_ID)
                return "No puede darse de baja a sí mismo";

            using var db = new Libreria();
            var existente = db.Usuarios.FirstOrDefault(x => x.USU_ID == usuario.USU_ID);
            if (existente == null)
                return "Usuario no encontrado";

            var inactivo = db.Estados_Usuarios.FirstOrDefault(e => e.EST_USU_Nombre == EstadoInactivo);
            if (inactivo == null)
                return $"Falta el estado \"{EstadoInactivo}\" en la base de datos";

            existente.EST_USU_ID = inactivo.EST_USU_ID;
            existente.USU_ClaveTemporal = null;
            existente.USU_ClaveTemporalVence = null;
            db.SaveChanges();
            BitacoraSeguridad.Registrar(BitacoraSeguridad.UsuarioDadoDeBaja, existente.USU_Nombre);
            return "Usuario dado de baja correctamente";
        }

        /// <summary>Reseteo por un administrador: clave temporal nueva por email, desbloqueo y cambio obligatorio.</summary>
        public bool ResetearClaveUsuario(Usuario usuario)
        {
            PermisoService.Instancia.Exigir("ResetearClave");

            if (usuario == null)
                return false;

            var claveTemporal = ServiciosUsuario.GenerarPassword();
            if (!ServiciosUsuario.SendMail(usuario, claveTemporal))
                return false;

            using var db = new Libreria();
            var existente = db.Usuarios.FirstOrDefault(x => x.USU_ID == usuario.USU_ID);
            if (existente == null)
                return false;

            existente.USU_Clave = HasherClaves.Hashear(claveTemporal);
            existente.USU_DebeCambiarClave = true;
            existente.USU_IntentosFallidos = 0;
            existente.USU_BloqueadoHasta = null;
            existente.USU_ClaveTemporal = null;
            existente.USU_ClaveTemporalVence = null;
            db.SaveChanges();
            BitacoraSeguridad.Registrar(BitacoraSeguridad.ClaveReseteada, existente.USU_Nombre);
            return true;
        }

        private static void AplicarGruposYAcciones(Libreria db, Usuario destino, Usuario origen)
        {
            SincronizadorColecciones.Sincronizar(destino.Grupos, origen.Grupos.Select(g => g.GRU_ID), g => g.GRU_ID,
                ids => db.Grupos.Where(g => ids.Contains(g.GRU_ID)).ToList());
            SincronizadorColecciones.Sincronizar(destino.Acciones, origen.Acciones.Select(a => a.ACC_ID), a => a.ACC_ID,
                ids => db.Acciones.Where(a => ids.Contains(a.ACC_ID)).ToList());
        }

        private static bool MismosIds(IEnumerable<int> a, IEnumerable<int> b) => a.ToHashSet().SetEquals(b);

        private static string DescribirPermisos(Usuario usuario, string nombre) =>
            $"{nombre} · grupos: [{string.Join(", ", usuario.Grupos.Select(g => g.GRU_Nombre))}]" +
            $" · acciones directas: [{string.Join(", ", usuario.Acciones.Select(a => a.ACC_Nombre))}]";

        #endregion
    }
}
