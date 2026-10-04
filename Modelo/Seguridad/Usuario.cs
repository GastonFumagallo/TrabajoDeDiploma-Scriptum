using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.RegularExpressions;

namespace Modelo.Seguridad
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int USU_ID { get; set; }
        public Usuario()
        {
            Acciones = new HashSet<Accion>();
            Grupos = new HashSet<Grupo>();
        }

        [StringLength(60)]
        public string USU_Nombre { get; set; }

        /// <summary>Hash PBKDF2 (ver Servicios.HasherClaves). Nunca se guarda ni se muestra la clave en texto plano.</summary>
        [StringLength(200)]
        public string USU_Clave { get; set; }

        [StringLength(60)]
        public string USU_Mail { get; set; }

        /// <summary>Intentos de login fallidos consecutivos; se reinicia al entrar bien o al bloquear.</summary>
        public int USU_IntentosFallidos { get; set; }

        /// <summary>Mientras sea posterior a ahora, el login se rechaza aunque la clave sea correcta.</summary>
        public DateTime? USU_BloqueadoHasta { get; set; }

        /// <summary>La clave actual es temporal (alta o reseteo): se exige cambiarla al entrar.</summary>
        public bool USU_DebeCambiarClave { get; set; }

        /// <summary>
        /// Clave de recuperación pedida desde el login. Convive con la clave actual hasta que se usa o vence,
        /// así nadie puede dejar sin acceso a otro usuario solo con conocer su nombre y su email.
        /// </summary>
        [StringLength(200)]
        public string? USU_ClaveTemporal { get; set; }

        public DateTime? USU_ClaveTemporalVence { get; set; }

        [ForeignKey("Estado_Usuario")]
        public int? EST_USU_ID { get; set; }

        public int PER_ID { get; set; }
        public Persona USU_Persona { get; set; }
        public virtual Estado_Usuario Estado_Usuario { get; set; }
        public virtual ICollection<Accion> Acciones { get; set; }
        public virtual ICollection<Grupo> Grupos { get; set; }

       
        public bool AgregarAccion(Accion accion)
        {
            var ok = false;
            var ac = Acciones.FirstOrDefault(x => x.ACC_ID == accion.ACC_ID);
            if (ac == null)
            {
                var accionGrupo = Grupos.FirstOrDefault(x => x.Acciones.Any(a => a.ACC_ID == accion.ACC_ID));
                if (accionGrupo == null)
                {
                    Acciones.Add(accion);
                    ok = true;
                }
            }
            return ok;
        }

        
        public bool QuitarAccion(Accion accion)
        {
            var ok = false;
            var ac = Acciones.FirstOrDefault(x => x.ACC_ID == accion.ACC_ID);
            if (ac != null)
            {
                Acciones.Remove(ac);
                ok = true;

            }
            return ok;
        }


        public bool AgregarGrupo(Grupo grupo)
        {
            var grupoExistente = Grupos.FirstOrDefault(x => x.GRU_ID == grupo.GRU_ID);
            if (grupoExistente != null) return false;

            var accionesPersonalizadas = Acciones.Where(x => grupo.Acciones.Any(a => a.ACC_ID == x.ACC_ID)).ToList();
            if (accionesPersonalizadas.Any())
            {
                foreach (var accion in accionesPersonalizadas)
                {
                    Acciones.Remove(accion);
                }
            }

            Grupos.Add(grupo);
            return true;
        }


        public bool QuitarGrupo(Grupo grupo)
        {
            var grupoExistente = Grupos.FirstOrDefault(x => x.GRU_ID == grupo.GRU_ID);
            if (grupoExistente == null) return false;
            else
            {
                Grupos.Remove(grupoExistente);
            }
            return true;
        }

        public ReadOnlyCollection<Grupo> getAllGruposActivos()
        {
            return Grupos.Where(x => x.EstaActivo).ToList().AsReadOnly();
        }


        public ReadOnlyCollection<Accion> getAllAcciones()
        {
            return Acciones.ToList().AsReadOnly();
        }

        public override string ToString()
        {
            return USU_Nombre.ToString();
        }
    }


    public class UsuarioDTO
    {
        public int USUDTO_ID { get; set; }
        public string NombrePersona { get; set; }
        public string Usuario { get; set; }
        public string Mail { get; set; }
        public string Estado { get; set; }
    }


}
