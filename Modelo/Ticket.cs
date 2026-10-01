using System;
using System.Collections.Generic;
using System.Text;

namespace Modelo
{
    public class TicketVentaDTO
    {
        public int NumeroVenta { get; set; }
        public DateTime Fecha { get; set; }

        public string Cliente { get; set; }
        public string MetodoPago { get; set; }

        public decimal Total { get; set; }

        public List<DetalleTicketDTO> Detalles { get; set; } = new();
    }
    public class DetalleTicketDTO
    {
        public string Libro { get; set; }
        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }



}
