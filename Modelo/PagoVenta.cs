using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Modelo
{
    /// <summary>
    /// Cobro asociado a una venta (movimiento de caja). Hoy cada venta tiene un único pago,
    /// pero la relación 1:N deja preparado el pago combinado (ej. parte efectivo, parte tarjeta).
    /// </summary>
    public class PagoVenta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PAG_ID { get; set; }

        public int VEN_ID { get; set; }
        public Venta PAG_Venta { get; set; }

        public int MP_ID { get; set; }
        public MetodoPago PAG_MetodoPago { get; set; }

        /// <summary>Importe imputado a la venta.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal PAG_Monto { get; set; }

        /// <summary>Importe entregado por el cliente (en efectivo puede superar al monto).</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal PAG_Recibido { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PAG_Vuelto { get; set; }

        public DateTime PAG_Fecha { get; set; } = DateTime.Now;
    }
}
