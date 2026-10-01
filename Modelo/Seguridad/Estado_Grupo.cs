using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Modelo.Seguridad
{
    public partial class Estado_Grupo
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EST_GRU_ID { get; set; }

        [StringLength(60)]
        public string EST_GRU_Nombre { get; set; }


        public override string ToString()
        {
            return EST_GRU_Nombre.ToString();
        }
    }
}
