using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Modelo.Seguridad
{
    public partial class Estado_Usuario
    {
        /// <summary>Estados de usuario de sistema (los inserta la migración GestionUsuarios). Se identifican por nombre.</summary>
        public const string Activo = "Activo";
        public const string Inactivo = "Inactivo";


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EST_USU_ID { get; set; }

        [StringLength(60)]
        public string EST_USU_Nombre { get; set; }


        public override string ToString()
        {
            return EST_USU_Nombre.ToString();
        }
    }
}
