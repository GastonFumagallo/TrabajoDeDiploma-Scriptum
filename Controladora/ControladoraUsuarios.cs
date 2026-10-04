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

        /// <summary>
        /// Usuario con todo lo que usan el login, PermisoService y FrmUsuario: persona, estado,
        /// grupos (con estado y acciones con formulario) y acciones directas. Se devuelve desconectado:
        /// no hay lazy loading, así que todo lo que la UI recorre tiene que venir incluido acá.
        /// </summary>
        private static IQueryable<Usuario> UsuariosConPermisos(Libreria db)
        {
            return db.Usuarios
                .AsNoTrackingWithIdentityResolution()
                .AsSplitQuery()
                .Include(u => u.USU_Persona)
                .Include(u => u.Estado_Usuario)
                .Include(u => u.Grupos)
                    .ThenInclude(g => g.Acciones)
                    .ThenInclude(a => a.Formulario)
                .Include(u => u.Grupos)
                    .ThenInclude(g => g.Estado_Grupo)
                .Include(u => u.Acciones)
                    .ThenInclude(a => a.Formulario);
        }

        public Usuario? IniciarSesion(string Usuario, string Clave)
        {
            Clave = ServiciosUsuario.EncriptarClave(Clave);
            using var db = new Libreria();
            // El filtro en SQL depende de la intercalación (suele ignorar mayúsculas); se confirma exacto en memoria.
            var usuario = UsuariosConPermisos(db)
                .Where(x => x.USU_Nombre == Usuario && x.USU_Clave == Clave)
                .AsEnumerable()
                .FirstOrDefault(x => x.USU_Nombre.Equals(Usuario) && x.USU_Clave.Equals(Clave));

            if (usuario == null || usuario.Estado_Usuario?.EST_USU_Nombre == "Inactivo")
            {
                return null;
            }
            return usuario;
        }


        public List<UsuarioDTO> FiltrarUsuarios(
        string nombreUsuario,
        int? idGrupo,
        int? idEstado)
        {
            using var db = new Libreria();
            var query = db.Usuarios.AsNoTracking();

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
            using var db = new Libreria();
            if (db.Usuarios.Any(x => x.USU_Mail == usuario.USU_Mail))
            {
                return "Email ya registrado";
            }

            if (!ServiciosUsuario.SendMail(usuario, usuario.USU_Clave))
            {
                return "No fue posible realizar la accion";
            }

            usuario.USU_Clave = ServiciosUsuario.EncriptarClave(usuario.USU_Clave);

            // Se arma una entidad nueva: el estado, los grupos y las acciones que trae la UI vienen de
            // otras consultas y, si se agregaran tal cual, EF intentaría insertarlos de nuevo.
            var nuevo = new Usuario
            {
                USU_Nombre = usuario.USU_Nombre,
                USU_Mail = usuario.USU_Mail,
                USU_Clave = usuario.USU_Clave,
                EST_USU_ID = usuario.Estado_Usuario?.EST_USU_ID ?? usuario.EST_USU_ID,
            };
            if (usuario.USU_Persona is { PER_ID: 0 } personaNueva)
                nuevo.USU_Persona = personaNueva;
            else
                nuevo.PER_ID = usuario.USU_Persona?.PER_ID ?? usuario.PER_ID;

            AplicarGruposYAcciones(db, nuevo, usuario);

            db.Usuarios.Add(nuevo);
            db.SaveChanges();
            usuario.USU_ID = nuevo.USU_ID;
            return "Usuario agregado correctamente";
        }

        public List<UsuarioDTO> obtenerUsuariosGrid()
        {
            using var db = new Libreria();
            return db.Usuarios.AsNoTracking()
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
            using var db = new Libreria();
            var usuarios = db.Usuarios.AsNoTracking();

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
            using var db = new Libreria();
            var usuarios = db.Usuarios.AsNoTracking()
            .Where(x => x.USU_Persona.PER_Nombre == nombre && x.Grupos.Any(g => g.GRU_ID == grupo.GRU_ID))
            .Select(p => new UsuarioDTO
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
            using var db = new Libreria();
            var usuario = db.Usuarios
                .Include(u => u.USU_Persona)
                .Where(x => x.USU_Nombre == Usuario && x.USU_Mail == mail)
                .AsEnumerable()
                .FirstOrDefault(x => x.USU_Nombre.Equals(Usuario) && x.USU_Mail.Equals(mail));
            if (usuario != null)
            {
                if (Servicios.ServiciosUsuario.SendMail(usuario, claveNueva))
                {
                    usuario.USU_Clave = ServiciosUsuario.EncriptarClave(claveNueva);
                    db.SaveChanges();
                    return true;
                }
                else { return false; }
            }
            return false;
        }


        /// <summary>
        /// Guarda los datos, la persona, el estado, los grupos y las acciones directas del usuario.
        /// El objeto que llega está desconectado: se carga el registro actual y se le aplican los cambios.
        /// </summary>
        public bool ModificarUsuario(Usuario usuario)
        {
            using var db = new Libreria();
            var existente = db.Usuarios
                .Include(u => u.USU_Persona)
                .Include(u => u.Grupos)
                .Include(u => u.Acciones)
                .FirstOrDefault(x => x.USU_ID == usuario.USU_ID);
            if (existente == null)
            {
                return false;
            }

            existente.USU_Nombre = usuario.USU_Nombre;
            existente.USU_Mail = usuario.USU_Mail;
            existente.USU_Clave = usuario.USU_Clave;
            existente.EST_USU_ID = usuario.Estado_Usuario?.EST_USU_ID ?? usuario.EST_USU_ID;

            if (existente.USU_Persona != null && usuario.USU_Persona != null)
            {
                existente.USU_Persona.PER_Nombre = usuario.USU_Persona.PER_Nombre;
                existente.USU_Persona.PER_Mail = usuario.USU_Persona.PER_Mail;
                existente.USU_Persona.PER_Telefono = usuario.USU_Persona.PER_Telefono;
                existente.USU_Persona.PER_DNI = usuario.USU_Persona.PER_DNI;
            }

            AplicarGruposYAcciones(db, existente, usuario);

            db.SaveChanges();
            return true;
        }

        private static void AplicarGruposYAcciones(Libreria db, Usuario destino, Usuario origen)
        {
            SincronizadorColecciones.Sincronizar(destino.Grupos, origen.Grupos.Select(g => g.GRU_ID), g => g.GRU_ID,
                ids => db.Grupos.Where(g => ids.Contains(g.GRU_ID)).ToList());
            SincronizadorColecciones.Sincronizar(destino.Acciones, origen.Acciones.Select(a => a.ACC_ID), a => a.ACC_ID,
                ids => db.Acciones.Where(a => ids.Contains(a.ACC_ID)).ToList());
        }

        public ReadOnlyCollection<Estado_Usuario> getAllEstadosUsuario()
        {
            using var db = new Libreria();
            return db.Estados_Usuarios.AsNoTracking().ToList().AsReadOnly();
        }
        public Usuario buscarUsuarioIndividual(UsuarioDTO usuarioSeleccionado)
        {
            using var db = new Libreria();
            return UsuariosConPermisos(db)
                          .FirstOrDefault(p => p.USU_ID == usuarioSeleccionado.USUDTO_ID);
        }

        public string EliminarUsuario(Usuario usuario)
        {
            if (usuario == null)
            {
                return "Usuario no encontrado";
            }

            using var db = new Libreria();
            var existente = db.Usuarios.FirstOrDefault(x => x.USU_ID == usuario.USU_ID);
            if (existente != null)
            {
                db.Usuarios.Remove(existente);
                db.SaveChanges();
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
                    using var db = new Libreria();
                    var existente = db.Usuarios.FirstOrDefault(x => x.USU_ID == usuario1.USU_ID);
                    if (existente == null)
                    {
                        return false;
                    }
                    existente.USU_Clave = claveNueva;
                    db.SaveChanges();
                    usuario1.USU_Clave = claveNueva;
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
            using var db = new Libreria();
            return UsuariosConPermisos(db)
            .ToList()
            .AsReadOnly();
        }
    }
}
