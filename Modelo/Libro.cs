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

        // Parámetros de reposición. Se cumple (validado en InventarioService): 0 <= Mínimo <= PuntoReposición <= Óptimo.
        /// <summary>Por debajo o igual a este valor el stock es crítico.</summary>
        public int LIB_StockMinimo { get; set; } = 2;

        /// <summary>Al llegar a este valor hay que pedir reposición.</summary>
        public int LIB_PuntoReposicion { get; set; } = 5;

        /// <summary>Stock objetivo: la cantidad sugerida a pedir es Óptimo − Stock − lo ya pedido.</summary>
        public int LIB_StockOptimo { get; set; } = 10;

        /// <summary>Último costo de compra conocido (se actualiza al recibir mercadería).</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal LIB_PrecioCosto { get; set; }

        /// <summary>ISBN / código de barras. Opcional; permite la carga con lector en el punto de venta.</summary>
        [StringLength(20)]
        public string? LIB_ISBN { get; set; }

        /// <summary>Deja sólo dígitos y la X final (ISBN-10); vacío → null. Así "978-950-..." y "978950..." coinciden.</summary>
        public static string? NormalizarISBN(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return null;
            var limpio = new string(texto.Where(c => char.IsDigit(c) || c is 'X' or 'x').ToArray()).ToUpperInvariant();
            return limpio.Length == 0 ? null : limpio;
        }

        public ICollection<ProveedorLibro> LIB_Proveedores { get; set; } = new List<ProveedorLibro>();
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
        public string? ISBN { get; set; }
    }


}
