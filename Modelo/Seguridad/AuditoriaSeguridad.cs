using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modelo.Seguridad
{
    /// <summary>
    /// Bitácora de eventos de seguridad: logins (correctos, fallidos, bloqueos), recuperación y reseteo de claves,
    /// y cambios en usuarios y grupos. Guarda nombres y no claves foráneas para que el registro sobreviva aunque
    /// el usuario cambie o se dé de baja, y para poder registrar intentos con nombres que no existen.
    /// </summary>
    public class AuditoriaSeguridad
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AUD_ID { get; set; }

        public DateTime AUD_Fecha { get; set; }

        /// <summary>Quién hizo la acción (usuario logueado), o el nombre tipeado en el login.</summary>
        [StringLength(60)]
        public string? AUD_Usuario { get; set; }

        /// <summary>Código del evento, ej. LOGIN_OK, LOGIN_FALLIDO, USUARIO_BLOQUEADO, GRUPO_MODIFICADO.</summary>
        [StringLength(40)]
        public string AUD_Evento { get; set; } = string.Empty;

        [StringLength(400)]
        public string? AUD_Detalle { get; set; }

        [StringLength(100)]
        public string? AUD_Equipo { get; set; }
    }
}
