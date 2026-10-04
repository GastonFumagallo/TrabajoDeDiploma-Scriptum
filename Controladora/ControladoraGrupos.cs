using Microsoft.EntityFrameworkCore;
using Modelo.Contexto;
using Modelo.Seguridad;
using System.Collections.ObjectModel;

namespace Controladora
{
    /// <summary>
    /// Lecturas de grupos para las pantallas de usuarios (FrmUsuario, FrmGestionarUsuarios).
    /// El ABM de grupos vive en <see cref="Seguridad.GrupoService"/>.
    /// </summary>
    public class ControladoraGrupos
    {
        private static ControladoraGrupos instancia;

        public static ControladoraGrupos Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new ControladoraGrupos();
                }
                return instancia;
            }
        }

        private ControladoraGrupos()
        {

        }

        /// <summary>Grupos con estado y acciones (FrmUsuario las recorre para marcar los permisos heredados).</summary>
        public ReadOnlyCollection<Grupo> getAllGrupos()
        {
            using var db = new Libreria();
            return db.Grupos.AsNoTrackingWithIdentityResolution()
                .Include(g => g.Acciones)
                .Include(g => g.Estado_Grupo)
                .ToList().AsReadOnly();
        }

        public ReadOnlyCollection<Modulo> getAllModulos()
        {
            using var db = new Libreria();
            return db.Modulos.AsNoTrackingWithIdentityResolution()
                .Include(m => m.Formularios)
                .ThenInclude(f => f.Acciones)
                .ToList().AsReadOnly();
        }
    }
}
