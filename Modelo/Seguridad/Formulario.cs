using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;

namespace Modelo.Seguridad
{
    public partial class Formulario
    {
        public Formulario()
        {
            Acciones = new HashSet<Accion>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FORM_ID { get; set; }

        [StringLength(60)]
        public string FORM_Nombre { get; set; }


        [ForeignKey("Modulo")]
        public int? MOD_ID { get; set; }

        public virtual ICollection<Accion> Acciones { get; set; }

        public Modulo Modulo { get; set; }
    }
}
