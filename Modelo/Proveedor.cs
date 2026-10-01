using Modelo.Seguridad;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Modelo
{
    public class Proveedor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PROV_ID { get; set; }
        public int PER_ID { get; set; }
        public Persona PER_Proveedor { get; set; }
        public string PROV_Empresa { get; set; }
        public ICollection<ProveedorLibro> PROV_Libros { get; set; } = new List<ProveedorLibro>();
    }
    public class ProveedorDTO
    {
        public int PROVDTO_ID { get; set; }
        public int DNI { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Empresa { get; set; }
    }

}
