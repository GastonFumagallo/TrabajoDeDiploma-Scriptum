using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Modelo.Seguridad
{
    public partial class Persona
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PER_ID { get; set; }

        [StringLength(60)]
        public string PER_Nombre { get; set; }

        public string PER_Mail { get; set; }

        public string PER_Telefono { get; set; }

        public int PER_DNI { get; set; }
        public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

        public virtual ICollection<Proveedor> Profesores { get; set; } = new List<Proveedor>();

        public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

    }
}
