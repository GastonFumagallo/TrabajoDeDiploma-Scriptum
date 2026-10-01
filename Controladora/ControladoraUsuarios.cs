using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using Modelo.Seguridad;
using Servicios;
using System.Collections.ObjectModel;

namespace Controladora
{
    public class ControladoraUsuarios
    {
        private static ControladoraUsuarios instancia;

        public static ControladoraUsuarios Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new ControladoraUsuarios();
                }
                return instancia;
            }
        }
        public Usuario? IniciarSesion(string Usuario, string Clave)
        {
            Clave = ServiciosUsuario.EncriptarClave(Clave);
            var usuario = getAllUsuarios().Where(x => x.USU_Nombre.Equals(Usuario) && x.USU_Clave.Equals(Clave)).FirstOrDefault();
            if (usuario == null)
            {
                return null;
            }
            else
            {
                if (usuario.Estado_Usuario.EST_USU_Nombre == "Inactivo")
                {
                    return null;
                }
                
            }
            return usuario;
        }


        public List<UsuarioDTO> FiltrarUsuarios(
        string nombreUsuario,
        int? idGrupo,
        int? idEstado)
        {
            var query = Libreria.Contexto.Usuarios
                .Include(u => u.Grupos)
                .Include(u => u.Estado_Usuario)
                .AsQueryable();

            // Filtro por nombre
            if (!string.IsNullOrWhiteSpace(nombreUsuario))
            {
                query = query.Where(u =>
                    u.USU_Persona.PER_Nombre.Contains(nombreUsuario));
            }

            // Filtro por grupo
            if (idGrupo.HasValue && idGrupo.Value != 0)
            {
                query = query.Where(u =>
                    u.Grupos.Any(g =>
                        g.GRU_ID == idGrupo.Value));
            }

            // Filtro por estado
            if (idEstado.HasValue && idEstado.Value != 0)
            {
                query = query.Where(u =>
                    u.Estado_Usuario.EST_USU_ID == idEstado.Value);
            }

            return query.Select(p => new UsuarioDTO
            {
                USUDTO_ID = p.USU_ID,
                NombrePersona = p.USU_Persona.PER_Nombre,
                Usuario = p.USU_Nombre,
                Mail = p.USU_Persona.PER_Mail,
                Clave = p.USU_Clave,
                Estado = p.Estado_Usuario.EST_USU_Nombre,
            }).ToList();
        }


        public string AgregarUsuario(Usuario usuario)
        {
            var UsuarioExistente = Libreria.Contexto.Usuarios.ToList().FirstOrDefault(x => x.USU_Mail == usuario.USU_Mail);
            if (UsuarioExistente == null)
            {
                if (ServiciosUsuario.SendMail(usuario, usuario.USU_Clave))
                {
                    usuario.USU_Clave = ServiciosUsuario.EncriptarClave(usuario.USU_Clave);
                    Libreria.Contexto.Usuarios.Add(usuario);
                    Libreria.Contexto.SaveChanges();
                    return "Usuario agregado correctamente";
                }
                else { return "No fue posible realizar la accion"; }

            }
            return "Email ya registrado";

        }
        public List<UsuarioDTO> obtenerUsuariosGrid()
        {
            return Libreria.Contexto.Usuarios.Include(p => p.Estado_Usuario).Include(p => p.USU_Persona)
            .Select(p => new UsuarioDTO
            {
                USUDTO_ID = p.USU_ID,
                NombrePersona = p.USU_Persona.PER_Nombre,
                Usuario = p.USU_Nombre,
                Mail = p.USU_Persona.PER_Mail,
                Clave = p.USU_Clave,
                Estado = p.Estado_Usuario.EST_USU_Nombre,
            }).ToList();
        }

        public List<UsuarioDTO> FiltrarUsuariosPorGrupo(string nombreUsuario, int? idGrupo)
        {
            var usuarios = Libreria.Contexto.Usuarios
                .Include(u => u.Grupos)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombreUsuario))
            {
                usuarios = usuarios.Where(u =>u.USU_Persona.PER_Nombre.Contains(nombreUsuario));
            }

            if (idGrupo.HasValue)
            {
                usuarios = usuarios.Where(u =>u.Grupos.Any(g =>g.GRU_ID == idGrupo.Value));
            }

            return usuarios.Select(u => new UsuarioDTO
            {
                USUDTO_ID = u.USU_ID,
                NombrePersona = u.USU_Persona.PER_Nombre,
                Usuario = u.USU_Nombre,
                Mail = u.USU_Persona.PER_Mail,
                Clave = u.USU_Clave,
                Estado = u.Estado_Usuario.EST_USU_Nombre,
            }).ToList();
        }
        public ReadOnlyCollection<UsuarioDTO> getAllUsuariosByGrupo(string nombre, Grupo grupo)
        {
            var usuarios = getAllUsuarios().Where(x => x.USU_Persona.PER_Nombre == nombre && x.Grupos.Contains(grupo)).
            Select(p => new UsuarioDTO
            {
                USUDTO_ID = p.USU_ID,
                NombrePersona = p.USU_Persona.PER_Nombre,
                Usuario = p.USU_Nombre,
                Mail = p.USU_Persona.PER_Mail,
                Clave = p.USU_Clave,
                Estado = p.Estado_Usuario.EST_USU_Nombre,
            }).ToList();
            return usuarios.AsReadOnly();
        }
        

        public bool RecuperarClave(string Usuario, string mail, string claveNueva)
        {
            var usuario = getAllUsuarios().Where(x => x.USU_Nombre.Equals(Usuario) && x.USU_Mail.Equals(mail)).FirstOrDefault();
            if (usuario != null)
            {
                if (Servicios.ServiciosUsuario.SendMail(usuario, claveNueva))
                {
                    claveNueva = ServiciosUsuario.EncriptarClave(claveNueva);
                    usuario.USU_Clave = claveNueva;
                    Libreria.Contexto.Usuarios.Update(usuario);
                    Libreria.Contexto.SaveChanges();
                    return true;
                }
                else { return false; }
            }
            return false;
        }


        public bool ModificarUsuario(Usuario usuario)
        {
            var UsuarioExistente = Libreria.Contexto.Usuarios.ToList().FirstOrDefault(x => x.USU_Mail == usuario.USU_Mail);
            if (UsuarioExistente != null)
            {
                Libreria.Contexto.Usuarios.Update(usuario);
                Libreria.Contexto.SaveChanges();
                return true;
            }
            return false;

        }

        public ReadOnlyCollection<Estado_Usuario> getAllEstadosUsuario()
        {
            return Libreria.Contexto.Estados_Usuarios.ToList().AsReadOnly();
        }
        public Usuario buscarUsuarioIndividual(UsuarioDTO usuarioSeleccionado)
        {
            return Libreria.Contexto.Usuarios
                          .Include(p => p.Grupos)
                          .Include(p => p.Acciones)
                          .Include(p => p.USU_Persona)
                          .Include(p => p.Estado_Usuario)
                          .FirstOrDefault(p => p.USU_ID == usuarioSeleccionado.USUDTO_ID);
        }

        public string EliminarUsuario(Usuario usuario)
        {
            var UsuarioExistente = Libreria.Contexto.Usuarios.ToList().FirstOrDefault(x => x.USU_Mail == usuario.USU_Mail);
            if (UsuarioExistente != null)
            {
                Libreria.Contexto.Usuarios.Remove(usuario);
                Libreria.Contexto.SaveChanges();
                return "Usuario eliminado correctamente";
            }
            return "Email no registrado";
        }

        public bool ResetearClaveUsuario(Usuario usuario1, string claveNueva)
        {
            if (usuario1 != null)
            {
                if (Servicios.ServiciosUsuario.SendMail(usuario1, claveNueva))
                {
                    claveNueva = ServiciosUsuario.EncriptarClave(claveNueva);
                    usuario1.USU_Clave = claveNueva;
                    Libreria.Contexto.Usuarios.Update(usuario1);
                    Libreria.Contexto.SaveChanges();
                    return true;
                }
                else 
                { 
                    return false; 
                }
            }
            return false;
        }
        public List<UsuarioDTO> buscarUsuario(string filtro)
        {
            return obtenerUsuariosGrid().Where(p => p.NombrePersona.ToString().ToLower().Contains(filtro)).ToList();
        }

        public ReadOnlyCollection<Usuario> getAllUsuarios()
        {
            return Libreria.Contexto.Usuarios
            .Include(u => u.USU_Persona)
            .Include(u => u.Estado_Usuario)

            .Include(u => u.Grupos)
                .ThenInclude(g => g.Acciones)
                .ThenInclude(a => a.Formulario)

            .Include(u => u.Grupos)
                .ThenInclude(g => g.Estado_Grupo)

            .Include(u => u.Acciones)
                .ThenInclude(a => a.Formulario)

            .ToList()
            .AsReadOnly();
        }
    }
}
