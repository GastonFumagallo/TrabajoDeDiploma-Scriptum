using Modelo.Contexto;
using Modelo.Seguridad;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora
{
    public class ControladoraSesiones
    {
        private static ControladoraSesiones instancia;
        private int? sesionActualId;
        private System.Threading.Timer timerActividad;
        private DateTime ultimaActividad;
        private const int TIMEOUT_MINUTOS = 15;
        public Action<string> OnSesionExpirada { get; set; }
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
                AS_Usuario = usuario,
                AS_FechaHoraLogin = DateTime.Now,
                AS_SesionActiva = true
            };
            Libreria.Contexto.AuditoriaSesiones.Add(auditoria);
            Libreria.Contexto.SaveChanges();
            sesionActualId = auditoria.AS_ID;
            ultimaActividad = DateTime.Now;
            IniciarMonitoreoActividad();

            return auditoria.AS_ID;


        }

        public void RegistrarLogout(string motivo = "Normal")
        {
            if (!sesionActualId.HasValue)
                return;

            try
            {
                var auditoria = Libreria.Contexto.AuditoriaSesiones
                    .FirstOrDefault(a => a.AS_ID == sesionActualId.Value);

                if (auditoria != null)
                {
                    auditoria.AS_FechaHoraLogout = DateTime.Now;
                    auditoria.AS_SesionActiva = false;
                    auditoria.AS_TipoLogout = motivo;
                    if (auditoria.AS_FechaHoraLogout is DateTime logout)
                    {
                        auditoria.AS_TiempoSesion = (int)(logout - auditoria.AS_FechaHoraLogin).TotalSeconds;
                    }
                    Libreria.Contexto.AuditoriaSesiones.Update(auditoria);
                    Libreria.Contexto.SaveChanges();
                }
                DetenerMonitoreoActividad();
                sesionActualId = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en RegistrarLogout: {ex.Message}");
            }
        }

        public void RegistrarActividad()
        {
            ultimaActividad = DateTime.Now;
        }

        private void CerrarSesionesAbandonadas(int usuarioId)
        {
            try
            {
                var sesionesAbiertas = Libreria.Contexto.AuditoriaSesiones
                            .Where(a => a.AS_USU_ID == usuarioId && a.AS_SesionActiva)
                            .ToList();
                foreach (var sesion in sesionesAbiertas)
                {
                    sesion.AS_SesionActiva = false;
                    sesion.AS_TipoLogout = "Cierre forzado";
                    sesion.AS_FechaHoraLogout = null;
                    Libreria.Contexto.Update(sesion);
                }
                Libreria.Contexto.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en CerrarSesionesAbandonadas: {ex.Message}");
            }
        }

        private void IniciarMonitoreoActividad()
        {
            timerActividad = new System.Threading.Timer(VerificarTimeout, null,
                TimeSpan.FromMinutes(0.5), TimeSpan.FromMinutes(0.5));
        }

        private void DetenerMonitoreoActividad()
        {
            timerActividad?.Dispose();
            timerActividad = null;
        }

        private void VerificarTimeout(object state)
        {
            if (!sesionActualId.HasValue)
                return;

            var tiempoInactivo = DateTime.Now - ultimaActividad;

            if (tiempoInactivo.TotalMinutes >= TIMEOUT_MINUTOS)
            {
                RegistrarLogout("Timeout por inactividad");
                OnSesionExpirada?.Invoke("Su sesión ha expirado por inactividad");
            }
        }


        public IReadOnlyCollection<AuditoriaSesion> AllHistorialSesiones()
        {
            return Libreria.Contexto.AuditoriaSesiones.ToList().AsReadOnly();
        }
    }
}
