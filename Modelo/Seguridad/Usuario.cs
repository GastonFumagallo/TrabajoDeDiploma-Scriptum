using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

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

        /// <summary>Nombre de inicio de sesión. Único y no editable: ventas, movimientos y órdenes lo guardan como texto.</summary>
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

        public DateTime? USU_UltimoAcceso { get; set; }

        /// <summary>Concurrencia optimista: dos administradores editando la misma cuenta no se pisan en silencio.</summary>
        [ConcurrencyCheck]
        public int USU_Version { get; set; }

        [ForeignKey("Estado_Usuario")]
        public int? EST_USU_ID { get; set; }

        public int PER_ID { get; set; }
        public Persona USU_Persona { get; set; }
        public virtual Estado_Usuario Estado_Usuario { get; set; }
        public virtual ICollection<Accion> Acciones { get; set; }
        public virtual ICollection<Grupo> Grupos { get; set; }

        public override string ToString()
        {
            return USU_Nombre.ToString();
        }
    }
}
