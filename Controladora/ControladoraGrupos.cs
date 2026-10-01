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
            var query = Libreria.Contexto.Grupos
                .Include(g => g.Estado_Grupo)
                .AsQueryable();

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
            return Libreria.Contexto.Grupos.Include(p => p.Estado_Grupo)
            .Select(p => new GrupoDTO
            {
                GRUDTO_ID = p.GRU_ID,
                Nombre = p.GRU_Nombre,
                Descripcion = p.GRU_Descripcion,
                EstadoGrupo = p.Estado_Grupo.EST_GRU_Nombre,
            }).ToList();
        }
        public ReadOnlyCollection<Grupo> getAllGrupos()
        {
            return Libreria.Contexto.Grupos.Include("Acciones").Include("Estado_Grupo").ToList().AsReadOnly();
        }

        public ReadOnlyCollection<Grupo> getAllGruposByEstado(Estado_Grupo estado)
        {
            return Libreria.Contexto.Grupos.Include("Acciones").Include("Estado_Grupo").Where(x => x.EST_GRU_ID == estado.EST_GRU_ID).ToList().AsReadOnly();
        }

        public ReadOnlyCollection<Estado_Grupo> getAllEstadosGrupo()
        {
            return Libreria.Contexto.Estados_Grupos.ToList().AsReadOnly();
        }

        public Grupo buscarGrupoIndividual(GrupoDTO grupoDTO)
        {
            return Libreria.Contexto.Grupos
                          .Include(p => p.Acciones)
                          .Include(p => p.Estado_Grupo)
                          .Include(p => p.Usuarios)
                          .FirstOrDefault(p => p.GRU_ID == grupoDTO.GRUDTO_ID);
        }

        public ReadOnlyCollection<Modulo> getAllModulos()
        {
            return Libreria.Contexto.Modulos.Include(m => m.Formularios).ThenInclude(f => f.Acciones).ToList().AsReadOnly();
        }

        public string AgregarGrupo(Grupo grupo)
        {
            var GrupoExistente = Libreria.Contexto.Grupos.ToList().FirstOrDefault(x => x.GRU_Nombre == grupo.GRU_Nombre);
            if (GrupoExistente == null)
            {
                Libreria.Contexto.Grupos.Add(grupo);
                Libreria.Contexto.SaveChanges();
                return "Grupo agreado correctamente";
            }
            return "Ya existe el grupo " + grupo.GRU_Nombre + " en el sistema";

        }

        public string EliminarGrupo(Grupo grupo)
        {
            var GrupoExistente = Libreria.Contexto.Grupos.ToList().FirstOrDefault(x => x.GRU_ID == grupo.GRU_ID);
            if (GrupoExistente != null)
            {
                if (!GrupoExistente.Usuarios.Any())
                {
                    Libreria.Contexto.Grupos.Remove(grupo);
                    Libreria.Contexto.SaveChanges();
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
            var GrupoExistente = Libreria.Contexto.Grupos.ToList().FirstOrDefault(x => x.GRU_ID == grupo.GRU_ID);
            if (GrupoExistente != null)
            {
                Libreria.Contexto.Grupos.Update(grupo);
                Libreria.Contexto.SaveChanges();
                return "El grupo fue Modificado";
            }
            else
            {
                return "El grupo solicitado no fue encontrado";
            }
        }
    }
}
