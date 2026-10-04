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

        /// <summary>Hash PBKDF2 (ver Servicios.HasherClaves). Nunca sale de la capa de servicio.</summary>
        [StringLength(200)]
        public string USU_Clave { get; set; }

        [StringLength(60)]
        public string USU_Mail { get; set; }

        [ForeignKey("Estado_Usuario")]
        public int? EST_USU_ID { get; set; }

        public int PER_ID { get; set; }
        public Persona USU_Persona { get; set; }
        public virtual Estado_Usuario Estado_Usuario { get; set; }
        public virtual ICollection<Accion> Acciones { get; set; }
        public virtual ICollection<Grupo> Grupos { get; set; }

        /// <summary>La clave es temporal (alta o blanqueo): hay que cambiarla antes de entrar al sistema.</summary>
        public bool USU_DebeCambiarClave { get; set; }

        /// <summary>Intentos fallidos consecutivos (login o código de recuperación).</summary>
        public int USU_IntentosFallidos { get; set; }

        /// <summary>Bloqueo temporal por intentos fallidos; se levanta solo al vencer o lo desbloquea un administrador.</summary>
        public DateTime? USU_BloqueadoHasta { get; set; }

        public DateTime? USU_UltimoAcceso { get; set; }

        /// <summary>Hash del código de recuperación enviado por mail (la clave no cambia hasta usarlo).</summary>
        [StringLength(200)]
        public string? USU_CodigoRecuperacion { get; set; }

        public DateTime? USU_CodigoVence { get; set; }

        [ConcurrencyCheck]
        public int USU_Version { get; set; }

        public override string ToString()
        {
            return USU_Nombre.ToString();
        }
    }
}
