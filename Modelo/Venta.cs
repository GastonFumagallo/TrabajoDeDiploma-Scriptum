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

        // Desglose del cierre. Se cumple: Total = Subtotal - Descuento + AjusteMedioPago.
        /// <summary>Suma de precio unitario × cantidad de los detalles.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal VEN_Subtotal { get; set; }

        /// <summary>Descuento manual aplicado por el vendedor, en porcentaje sobre el subtotal.</summary>
        [Column(TypeName = "decimal(5,2)")]
        public decimal VEN_PorcentajeDescuento { get; set; }

        /// <summary>Importe del descuento manual.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal VEN_Descuento { get; set; }

        /// <summary>Recargo (positivo) o descuento (negativo) que aplica el medio de pago.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal VEN_AjusteMedioPago { get; set; }

        /// <summary>IVA contenido en el total (los precios son finales; los libros suelen estar exentos).</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal VEN_IVA { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal VEN_Total { get; set; }

        public int MP_ID { get; set; }
        public MetodoPago VEN_MetodoPago { get; set; }

        /// <summary>Usuario que registró la venta.</summary>
        [StringLength(100)]
        public string? VEN_Usuario { get; set; }

        public ICollection<DetalleVenta> VEN_Detalles { get; set; } = new List<DetalleVenta>();
        public ICollection<PagoVenta> VEN_Pagos { get; set; } = new List<PagoVenta>();

        /// <summary>Número de comprobante visible (TK-000123).</summary>
        public static string FormatearComprobante(int ventaId) => $"TK-{ventaId:D6}";

        // Anulación lógica: una venta nunca se borra, se marca como anulada y se repone el stock.
        public bool VEN_Anulada { get; set; }
        public DateTime? VEN_FechaAnulacion { get; set; }

        [StringLength(250)]
        public string? VEN_MotivoAnulacion { get; set; }

        [StringLength(100)]
        public string? VEN_UsuarioAnulacion { get; set; }
    }

    public class VentaDTO
    {
        public int VENDTO_ID { get; set; }
        public string Comprobante => Venta.FormatearComprobante(VENDTO_ID);
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; }
        public string MetodoPago { get; set; }
        public decimal TotalVenta { get; set; }
        public string Estado { get; set; } = "Completada";
        public string? Usuario { get; set; }
        public bool Anulada { get; set; }
        public string? MotivoAnulacion { get; set; }
    }

    public enum EstadoVentaFiltro { Todas, Completadas, Anuladas }

    /// <summary>Criterios de búsqueda del listado de ventas. Todos opcionales y combinables.</summary>
    public class FiltroVentas
    {
        /// <summary>Busca por nombre del cliente o por DNI (coincidencia parcial).</summary>
        public string? Cliente { get; set; }
        /// <summary>Número de comprobante: acepta "TK-000123" o "123".</summary>
        public int? NumeroComprobante { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public int? MetodoPagoId { get; set; }
        public EstadoVentaFiltro Estado { get; set; } = EstadoVentaFiltro.Todas;
    }

    /// <summary>Métricas del período filtrado, calculadas en SQL sobre todas las ventas que cumplen el filtro.</summary>
    public class MetricasVentas
    {
        public int CantidadCompletadas { get; set; }
        public int CantidadAnuladas { get; set; }
        public decimal TotalRecaudado { get; set; }
        public decimal TicketPromedio => CantidadCompletadas == 0 ? 0 : Math.Round(TotalRecaudado / CantidadCompletadas, 2);
    }

    public class PaginaVentas
    {
        public List<VentaDTO> Ventas { get; set; } = new();
        public int TotalRegistros { get; set; }
        public MetricasVentas Metricas { get; set; } = new();
    }




}
