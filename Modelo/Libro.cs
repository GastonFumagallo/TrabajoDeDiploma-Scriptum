using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Modelo
{
    public class Libro
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int LIB_ID { get; set; }
        public string LIB_Titulo { get; set; }
        public string LIB_Autor { get; set; }
        public string LIB_Descripcion { get; set; }
        public string LIB_Editorial { get; set; }

        public int LIB_Stock { get; set; }
        public int LIB_AñoPublicacion { get; set; }

        public int GEN_ID { get; set; }
        public Genero LIB_Genero { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LIB_PrecioVenta { get; set; }

        public ICollection<ProveedorLibro> LIB_Proveedores { get; set; } = new List<ProveedorLibro>();
    }

    public class LibroInventarioDTO
    {
        public int LINVDTO_ID { get; set; }
        public string Titulo { get; set; }
        public int Stock { get; set; }
        public string Estado { get; set; }
    }
    public class LibroVentaDTO
    {
        public int LVDTO_ID { get; set; }
        public string Titulo { get; set; }
        public string Editorial { get; set; }
        public string Autor { get; set; }   
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
    }
    public class LibroDTO
    {
        public int LIBDTO_ID { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string Descripcion { get; set; }
        public string Editorial { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int AñoPublicacion { get; set; }
        public string Genero { get; set; }
    }


}
