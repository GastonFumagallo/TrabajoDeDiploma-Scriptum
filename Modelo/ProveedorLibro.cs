using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Modelo
{
    public class ProveedorLibro
    {
        public int PROV_ID { get; set; }
        public int LIB_ID { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal PL_PrecioCompra { get; set; }
        public Proveedor PL_Proveedor { get; set; }
        public Libro PL_Libro { get; set; }
    }

    public class ProveedorLibroDTO
    {
        public int PLDTO_PROVID { get; set; }
        public string Proveedor { get; set; }
        public decimal Precio { get; set; }
    }

    public class ProveedorLibroInventarioDTO
    {
        public string NombreProveedor { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
    }

}

