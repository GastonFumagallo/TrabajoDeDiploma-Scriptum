using Microsoft.EntityFrameworkCore;
using Modelo.Contexto;
using Modelo.Seguridad;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora
{
    /// <summary>
    /// Auditoría de sesiones y cierre por inactividad.
    /// El timer corre en un hilo del ThreadPool, por eso cada operación usa su propio DbContext
    /// y el estado compartido (sesión actual, última actividad) se maneja con lock / Interlocked.
    /// <see cref="OnSesionExpirada"/> se dispara desde ese hilo: los suscriptores de UI deben pasar al hilo de UI.
    /// </summary>
    public class ControladoraSesiones
    {
        private static ControladoraSesiones instancia;
        private readonly object sincronizacion = new();
        private int? sesionActualId;
        private System.Threading.Timer timerActividad;
        private long ultimaActividadTicks;
        private const int TIMEOUT_MINUTOS = 15;
        private static readonly TimeSpan IntervaloVerificacion = TimeSpan.FromSeconds(30);

        public event Action<string>? OnSesionExpirada;

        public static ControladoraSesiones Instancia
        {
            get
            {
                if (instancia == null)
                    instancia = new ControladoraSesiones();
                return instancia;
            }
        }

        private ControladoraSesiones() { }




        public int RegistrarLogin(Usuario usuario)
        {

            CerrarSesionesAbandonadas(usuario.PER_ID);

            var auditoria = new AuditoriaSesion
            {
                AS_USU_ID = usuario.PER_ID,
                AS_FechaHoraLogin = DateTime.Now,
                AS_SesionActiva = true
            };

            using (var db = new Libreria())
            {
                db.AuditoriaSesiones.Add(auditoria);
                // Se vincula por la FK (sombra) y no por la navegación: el usuario viene desconectado
                // y, adjuntándolo, EF intentaría insertarlo junto con sus grupos y acciones.
                db.Entry(auditoria).Property("AS_UsuarioUSU_ID").CurrentValue = usuario.USU_ID;
                db.SaveChanges();
            }

            lock (sincronizacion)
            {
                sesionActualId = auditoria.AS_ID;
                RegistrarActividad();
                IniciarMonitoreoActividad();
            }

            return auditoria.AS_ID;


        }

        public void RegistrarLogout(string motivo = "Normal")
        {
            CerrarSesionActual(motivo);
        }

        /// <summary>Cierra la sesión actual. Devuelve false si no había sesión abierta (p. ej. ya la cerró otro hilo).</summary>
        private bool CerrarSesionActual(string motivo)
        {
            int sesionId;
            lock (sincronizacion)
            {
                // Se toma y limpia la sesión de forma atómica: si el timer y el usuario cierran a la vez,
                // sólo uno de los dos registra el logout.
                if (!sesionActualId.HasValue)
                    return false;
                sesionId = sesionActualId.Value;
                sesionActualId = null;
                DetenerMonitoreoActividad();
            }

            try
            {
                using var db = new Libreria();
                var auditoria = db.AuditoriaSesiones
                    .FirstOrDefault(a => a.AS_ID == sesionId);

                if (auditoria != null)
                {
                    var logout = DateTime.Now;
                    auditoria.AS_FechaHoraLogout = logout;
                    auditoria.AS_SesionActiva = false;
                    auditoria.AS_TipoLogout = motivo;
                    auditoria.AS_TiempoSesion = (int)(logout - auditoria.AS_FechaHoraLogin).TotalSeconds;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en RegistrarLogout: {ex.Message}");
            }
            return true;
        }

        /// <summary>Reinicia el contador de inactividad. Es barato: se puede llamar en cada evento de mouse/teclado.</summary>
        public void RegistrarActividad()
        {
            Interlocked.Exchange(ref ultimaActividadTicks, DateTime.UtcNow.Ticks);
        }

        private void CerrarSesionesAbandonadas(int usuarioId)
        {
            try
            {
                using var db = new Libreria();
                var sesionesAbiertas = db.AuditoriaSesiones
                            .Where(a => a.AS_USU_ID == usuarioId && a.AS_SesionActiva)
                            .ToList();
                foreach (var sesion in sesionesAbiertas)
                {
                    sesion.AS_SesionActiva = false;
                    sesion.AS_TipoLogout = "Cierre forzado";
                    sesion.AS_FechaHoraLogout = null;
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en CerrarSesionesAbandonadas: {ex.Message}");
            }
        }

        // Llamar con el lock tomado.
        private void IniciarMonitoreoActividad()
        {
            DetenerMonitoreoActividad();
            timerActividad = new System.Threading.Timer(VerificarTimeout, null,
                IntervaloVerificacion, IntervaloVerificacion);
        }

        // Llamar con el lock tomado.
        private void DetenerMonitoreoActividad()
        {
            timerActividad?.Dispose();
            timerActividad = null;
        }

        private void VerificarTimeout(object? state)
        {
            lock (sincronizacion)
            {
                if (!sesionActualId.HasValue)
                    return;
            }

            var ultimaActividad = new DateTime(Interlocked.Read(ref ultimaActividadTicks), DateTimeKind.Utc);
            var tiempoInactivo = DateTime.UtcNow - ultimaActividad;

            if (tiempoInactivo.TotalMinutes >= TIMEOUT_MINUTOS && CerrarSesionActual("Timeout por inactividad"))
            {
                OnSesionExpirada?.Invoke("Su sesión ha expirado por inactividad");
            }
        }


        public IReadOnlyCollection<AuditoriaSesion> AllHistorialSesiones()
        {
            using var db = new Libreria();
            return db.AuditoriaSesiones.AsNoTracking().ToList().AsReadOnly();
        }
    }
}
