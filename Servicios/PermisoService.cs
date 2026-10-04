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
        /// <summary>
        /// Carga los permisos efectivos del usuario: acciones de sus grupos activos más sus acciones directas.
        /// El usuario tiene que venir con Grupos (Estado_Grupo, Acciones.Formulario) y Acciones.Formulario cargados.
        /// </summary>
        public void IniciarSesion(Usuario usuario)
        {
            var acciones = usuario.Grupos.Where(g => g.EstaActivo).SelectMany(g => g.Acciones)
                .Concat(usuario.Acciones)
                .ToList();

            UsuarioActual = usuario;
            CargarPermisos(
                acciones.Select(a => a.ACC_Nombre).Distinct().ToList(),
                acciones.Where(a => a.Formulario != null).Select(a => a.Formulario.FORM_Nombre).Distinct().ToList());
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
