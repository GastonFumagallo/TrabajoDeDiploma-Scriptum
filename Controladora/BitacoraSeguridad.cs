using Modelo.Contexto;
using Modelo.Seguridad;

namespace Controladora
{
    /// <summary>Escribe en la tabla AuditoriaSeguridad. Un fallo al auditar nunca interrumpe la operación.</summary>
    public static class BitacoraSeguridad
    {
        public const string LoginOk = "LOGIN_OK";
        public const string LoginFallido = "LOGIN_FALLIDO";
        public const string LoginRechazado = "LOGIN_RECHAZADO";
        public const string UsuarioBloqueado = "USUARIO_BLOQUEADO";
        public const string RecuperacionSolicitada = "RECUPERACION_SOLICITADA";
        public const string ClaveReseteada = "CLAVE_RESETEADA";
        public const string ClaveCambiada = "CLAVE_CAMBIADA";
        public const string UsuarioCreado = "USUARIO_CREADO";
        public const string UsuarioModificado = "USUARIO_MODIFICADO";
        public const string UsuarioDadoDeBaja = "USUARIO_BAJA";
        public const string UsuarioReactivado = "USUARIO_REACTIVADO";
        public const string UsuarioDesbloqueado = "USUARIO_DESBLOQUEADO";
        public const string GrupoCreado = "GRUPO_CREADO";
        public const string GrupoModificado = "GRUPO_MODIFICADO";
        public const string GrupoEliminado = "GRUPO_ELIMINADO";

        /// <param name="usuario">Por defecto, el usuario logueado. En el login, el nombre que se tipeó.</param>
        public static void Registrar(string evento, string? detalle = null, string? usuario = null)
        {
            try
            {
                using var db = new Libreria();
                db.AuditoriaSeguridad.Add(new AuditoriaSeguridad
                {
                    AUD_Fecha = DateTime.Now,
                    AUD_Usuario = Recortar(usuario ?? Sesion.Instancia.Usuario?.USU_Nombre, 60),
                    AUD_Evento = evento,
                    AUD_Detalle = Recortar(detalle, 400),
                    AUD_Equipo = Recortar(Environment.MachineName, 100),
                });
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"No se pudo auditar {evento}: {ex.Message}");
            }
        }

        private static string? Recortar(string? texto, int largo) =>
            texto == null || texto.Length <= largo ? texto : texto[..largo];
    }
}
