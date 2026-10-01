using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Modelo
{
    public class Venta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VEN_ID { get; set; }
        public DateTime VEN_Fecha { get; set; } = DateTime.Now;
        public int CLI_ID { get; set; }
        public Cliente VEN_Cliente { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal VEN_Total { get; set; }

        public int MP_ID { get; set; }
        public MetodoPago VEN_MetodoPago { get; set; }

        public ICollection<DetalleVenta> VEN_Detalles { get; set; } = new List<DetalleVenta>();

    }

    public class VentaDTO
    {
        public int VENDTO_ID { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; }
        public decimal TotalVenta { get; set; }
        public string MetodoPago { get; set; }
    }




}
