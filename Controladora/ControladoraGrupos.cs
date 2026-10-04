using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using Modelo.Seguridad;
using Servicios;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Controladora
{
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

        public List<GrupoDTO> FiltrarGrupos(string nombreGrupo, int? idEstado)
        {
            using var db = new Libreria();
            var query = db.Grupos.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(nombreGrupo))
            {
                query = query.Where(g =>
                    g.GRU_Nombre.Contains(nombreGrupo));
            }

            if (idEstado.HasValue && idEstado.Value != 0)
            {
                query = query.Where(g =>
                    g.Estado_Grupo.EST_GRU_ID == idEstado.Value);
            }

            return query.Select(p => new GrupoDTO
            {
                GRUDTO_ID = p.GRU_ID,
                Nombre = p.GRU_Nombre,
                Descripcion = p.GRU_Descripcion,
                EstadoGrupo = p.Estado_Grupo.EST_GRU_Nombre,
            }).ToList();
        }
        public List<GrupoDTO> obtenerGruposGrid()
        {
            using var db = new Libreria();
            return db.Grupos.AsNoTracking()
            .Select(p => new GrupoDTO
            {
                GRUDTO_ID = p.GRU_ID,
                Nombre = p.GRU_Nombre,
                Descripcion = p.GRU_Descripcion,
                EstadoGrupo = p.Estado_Grupo.EST_GRU_Nombre,
            }).ToList();
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

        public ReadOnlyCollection<Grupo> getAllGruposByEstado(Estado_Grupo estado)
        {
            using var db = new Libreria();
            return db.Grupos.AsNoTrackingWithIdentityResolution()
                .Include(g => g.Acciones)
                .Include(g => g.Estado_Grupo)
                .Where(x => x.EST_GRU_ID == estado.EST_GRU_ID)
                .ToList().AsReadOnly();
        }

        public ReadOnlyCollection<Estado_Grupo> getAllEstadosGrupo()
        {
            using var db = new Libreria();
            return db.Estados_Grupos.AsNoTracking().ToList().AsReadOnly();
        }

        public Grupo buscarGrupoIndividual(GrupoDTO grupoDTO)
        {
            using var db = new Libreria();
            return db.Grupos.AsNoTrackingWithIdentityResolution()
                          .AsSplitQuery()
                          .Include(p => p.Acciones)
                          .Include(p => p.Estado_Grupo)
                          .Include(p => p.Usuarios)
                          .FirstOrDefault(p => p.GRU_ID == grupoDTO.GRUDTO_ID);
        }

        public ReadOnlyCollection<Modulo> getAllModulos()
        {
            using var db = new Libreria();
            return db.Modulos.AsNoTrackingWithIdentityResolution()
                .Include(m => m.Formularios)
                .ThenInclude(f => f.Acciones)
                .ToList().AsReadOnly();
        }

        public string AgregarGrupo(Grupo grupo)
        {
            PermisoService.Instancia.Exigir("AgregarGrupo");

            using var db = new Libreria();
            if (db.Grupos.Any(x => x.GRU_Nombre == grupo.GRU_Nombre))
            {
                return "Ya existe el grupo " + grupo.GRU_Nombre + " en el sistema";
            }

            // El estado y las acciones de la UI vienen de otras consultas: se vinculan por ID.
            var nuevo = new Grupo
            {
                GRU_Nombre = grupo.GRU_Nombre,
                GRU_Descripcion = grupo.GRU_Descripcion,
                EST_GRU_ID = grupo.Estado_Grupo?.EST_GRU_ID ?? grupo.EST_GRU_ID,
            };
            AplicarAcciones(db, nuevo, grupo);

            db.Grupos.Add(nuevo);
            db.SaveChanges();
            grupo.GRU_ID = nuevo.GRU_ID;
            BitacoraSeguridad.Registrar(BitacoraSeguridad.GrupoCreado, DescribirGrupo(nuevo));
            return "Grupo agreado correctamente";
        }

        public string EliminarGrupo(Grupo grupo)
        {
            PermisoService.Instancia.Exigir("EliminarGrupo");

            if (grupo == null)
            {
                return "El grupo solicitado no fue encontrado";
            }

            using var db = new Libreria();
            var GrupoExistente = db.Grupos.FirstOrDefault(x => x.GRU_ID == grupo.GRU_ID);
            if (GrupoExistente != null)
            {
                if (GrupoExistente.GRU_Nombre == PermisoService.GrupoAdministrador)
                    return "El grupo Administrador no puede eliminarse";

                if (!db.Usuarios.Any(u => u.Grupos.Any(g => g.GRU_ID == grupo.GRU_ID)))
                {
                    db.Grupos.Remove(GrupoExistente);
                    db.SaveChanges();
                    BitacoraSeguridad.Registrar(BitacoraSeguridad.GrupoEliminado, GrupoExistente.GRU_Nombre);
                    return "El grupo fue eliminado";
                }
                else
                {
                    return "Existen usuarios asociados a este grupo";
                }
            }
            return "El grupo solicitado no fue encontrado";
        }

        public string ModificarGrupo(Grupo grupo)
        {
            PermisoService.Instancia.Exigir("ModificarGrupo");

            using var db = new Libreria();
            var GrupoExistente = db.Grupos
                .Include(g => g.Acciones)
                .FirstOrDefault(x => x.GRU_ID == grupo.GRU_ID);
            if (GrupoExistente != null)
            {
                // Ser "Administrador" depende del nombre del grupo: renombrar otro grupo así daba acceso total
                // a todos sus miembros. Antes este método ni siquiera validaba nombres repetidos.
                bool eraAdministrador = GrupoExistente.GRU_Nombre == PermisoService.GrupoAdministrador;
                if (eraAdministrador != (grupo.GRU_Nombre == PermisoService.GrupoAdministrador))
                    return "El grupo Administrador no puede renombrarse, ni otro grupo tomar ese nombre";
                if (db.Grupos.Any(x => x.GRU_Nombre == grupo.GRU_Nombre && x.GRU_ID != grupo.GRU_ID))
                    return "Ya existe el grupo " + grupo.GRU_Nombre + " en el sistema";

                int estadoNuevo = grupo.Estado_Grupo?.EST_GRU_ID ?? grupo.EST_GRU_ID;
                if (eraAdministrador && estadoNuevo != GrupoExistente.EST_GRU_ID)
                    return "El grupo Administrador no puede deshabilitarse";

                // Nadie se amplía los permisos a través de un grupo propio: lo tiene que hacer otro administrador.
                var usuarioActual = Sesion.Instancia.Usuario;
                bool perteneceAlGrupo = usuarioActual?.Grupos.Any(g => g.GRU_ID == grupo.GRU_ID) == true;
                bool cambianAcciones = !GrupoExistente.Acciones.Select(a => a.ACC_ID).ToHashSet()
                    .SetEquals(grupo.Acciones.Select(a => a.ACC_ID));
                if (perteneceAlGrupo && cambianAcciones && !eraAdministrador)
                    return "No puede modificar los permisos de un grupo al que pertenece";

                GrupoExistente.GRU_Nombre = grupo.GRU_Nombre;
                GrupoExistente.GRU_Descripcion = grupo.GRU_Descripcion;
                GrupoExistente.EST_GRU_ID = estadoNuevo;
                AplicarAcciones(db, GrupoExistente, grupo);

                db.SaveChanges();
                BitacoraSeguridad.Registrar(BitacoraSeguridad.GrupoModificado, DescribirGrupo(GrupoExistente));
                return "El grupo fue Modificado";
            }
            else
            {
                return "El grupo solicitado no fue encontrado";
            }
        }

        private static void AplicarAcciones(Libreria db, Grupo destino, Grupo origen)
        {
            SincronizadorColecciones.Sincronizar(destino.Acciones, origen.Acciones.Select(a => a.ACC_ID), a => a.ACC_ID,
                ids => db.Acciones.Where(a => ids.Contains(a.ACC_ID)).ToList());
        }

        private static string DescribirGrupo(Grupo grupo) =>
            $"{grupo.GRU_Nombre} · acciones: [{string.Join(", ", grupo.Acciones.Select(a => a.ACC_Nombre))}]";
    }
}
