using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Modelo
{
    public class Genero
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GEN_ID { get; set; }
        public string GEN_Nombre { get; set; }
        public string GEN_Descripcion { get; set; }
        public List<Libro> GEN_Libros { get; set; }
    }
}
