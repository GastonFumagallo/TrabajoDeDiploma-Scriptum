using Modelo;
using Modelo.Seguridad;

namespace Servicios
{
    /// <summary>
    /// Permisos del usuario logueado. La UI los usa para ocultar o deshabilitar controles (<see cref="TienePermiso"/>)
    /// y la capa de negocio los vuelve a exigir antes de ejecutar (<see cref="Exigir"/>): un botón oculto no es seguridad.
    /// </summary>
    public class PermisoService
    {
        public const string GrupoAdministrador = Grupo.NombreAdministrador;

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

        // Un grupo deshabilitado no otorga nada, tampoco el de administradores.
        private bool EsAdministrador =>
            UsuarioActual != null &&
            UsuarioActual.Grupos.Any(g => g.GRU_Nombre == GrupoAdministrador && g.EstaActivo);

        public bool TienePermiso(string nombreAccion)
        {
            if (UsuarioActual == null) return false;
            return EsAdministrador || Permisos.Contains(nombreAccion);
        }

        public bool PuedeAccederFormulario(string nombreFormulario)
        {
            if (UsuarioActual == null) return false;
            return EsAdministrador || FormsHabilitados.Contains(nombreFormulario);
        }

        /// <summary>Lanza <see cref="AccesoDenegadoException"/> si el usuario logueado no tiene el permiso.</summary>
        public void Exigir(string nombreAccion)
        {
            if (!TienePermiso(nombreAccion))
                throw new AccesoDenegadoException(nombreAccion);
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
