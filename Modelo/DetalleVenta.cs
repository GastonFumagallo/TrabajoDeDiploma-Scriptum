using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Modelo
{
        public class DetalleVenta
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int DV_ID{ get; set; }

            public int VEN_ID { get; set; }
            public Venta DV_Venta { get; set; }

            public int LIB_ID { get; set; }
            public Libro DV_Libro { get; set; }

            public int DV_Cantidad { get; set; }

            [Column(TypeName = "decimal(18,2)")]
            public decimal DV_PrecioUnitario { get; set; }

            [Column(TypeName = "decimal(18,2)")]

            [NotMapped]
            public decimal DV_Subtotal
            {
                get
                {
                    decimal subtotal = 0;
                    subtotal = DV_PrecioUnitario * DV_Cantidad;
                    return Math.Round(subtotal, 2);
                }
            }
        }
    public class DetalleVentaDTO
    {
        public int DVDTO_ID { get; set; }
        public string Libro { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }


}
