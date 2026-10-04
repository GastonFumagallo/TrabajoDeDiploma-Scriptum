using Modelo.Seguridad;
using System;
using System.Collections.Generic;
using System.Text;

namespace Servicios
{
    public class PermisoService
    {
        private static PermisoService _instancia;

        public Usuario UsuarioActual{ get; set; }
        public List<string> Permisos { get; private set; } = new List<string>();
        public HashSet<string> FormsHabilitados { get; private set; } = new HashSet<string>();
        public static PermisoService Instancia
        {
            get
            {
                if (_instancia == null) _instancia = new PermisoService();
                return _instancia;
            }
        }

        public bool TienePermiso(string nombreAccion)
        {
            if (UsuarioActual != null && UsuarioActual.Grupos.Any(g => g.GRU_Nombre == Grupo.NombreAdministrador)) return true;
            return Permisos.Contains(nombreAccion);
        }
        public bool PuedeAccederFormulario(string nombreFormulario)
        {
            if (UsuarioActual != null && UsuarioActual.Grupos.Any(g => g.GRU_Nombre == Grupo.NombreAdministrador)) return true;

            return FormsHabilitados.Contains(nombreFormulario);
        }
        public void CargarPermisos(List<string> acciones, List<string> formularios)
        {
            Permisos = acciones;
            FormsHabilitados = new HashSet<string>(formularios);
        }
        public void Logout()
        {
            UsuarioActual = null;
            Permisos.Clear();
            FormsHabilitados.Clear();
        }
    }

}
