using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Modelo.Seguridad
{
    public class AuditoriaSesion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AS_ID { get; set; }
        public int AS_USU_ID { get; set; }
        public  Usuario AS_Usuario { get; set; }
        public DateTime AS_FechaHoraLogin { get; set; }
        public DateTime? AS_FechaHoraLogout { get; set; }
        public string? AS_TipoLogout { get; set; }
        public bool AS_SesionActiva { get; set; }
        public int? AS_TiempoSesion { get; set; }
    }
}
