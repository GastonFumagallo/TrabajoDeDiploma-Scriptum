using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using Modelo.Seguridad;
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
            return "Grupo agreado correctamente";
        }

        public string EliminarGrupo(Grupo grupo)
        {
            if (grupo == null)
            {
                return "El grupo solicitado no fue encontrado";
            }

            using var db = new Libreria();
            var GrupoExistente = db.Grupos.FirstOrDefault(x => x.GRU_ID == grupo.GRU_ID);
            if (GrupoExistente != null)
            {
                if (!db.Usuarios.Any(u => u.Grupos.Any(g => g.GRU_ID == grupo.GRU_ID)))
                {
                    db.Grupos.Remove(GrupoExistente);
                    db.SaveChanges();
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
            using var db = new Libreria();
            var GrupoExistente = db.Grupos
                .Include(g => g.Acciones)
                .FirstOrDefault(x => x.GRU_ID == grupo.GRU_ID);
            if (GrupoExistente != null)
            {
                GrupoExistente.GRU_Nombre = grupo.GRU_Nombre;
                GrupoExistente.GRU_Descripcion = grupo.GRU_Descripcion;
                GrupoExistente.EST_GRU_ID = grupo.Estado_Grupo?.EST_GRU_ID ?? grupo.EST_GRU_ID;
                AplicarAcciones(db, GrupoExistente, grupo);

                db.SaveChanges();
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
    }
}
