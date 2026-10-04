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
        public const string EstadoInactivo = Estado_Usuario.Inactivo;
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
        /// Usuario con todo lo que usan el login, PermisoService y la sesión: persona, estado,
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
                usuario.USU_UltimoAcceso = ahora;
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
    }
}
