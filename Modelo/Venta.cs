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
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; }
        public decimal TotalVenta { get; set; }
        public string MetodoPago { get; set; }
        public string Estado { get; set; } = "Vigente";
        public bool Anulada { get; set; }
        public string? MotivoAnulacion { get; set; }
    }

    public enum EstadoVentaFiltro { Todas, Vigentes, Anuladas }

    /// <summary>Criterios de búsqueda del listado de ventas. Todos opcionales y combinables.</summary>
    public class FiltroVentas
    {
        /// <summary>Busca por nombre del cliente o por DNI (coincidencia parcial).</summary>
        public string? Cliente { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public EstadoVentaFiltro Estado { get; set; } = EstadoVentaFiltro.Todas;
    }

    public class PaginaVentas
    {
        public List<VentaDTO> Ventas { get; set; } = new();
        public int TotalRegistros { get; set; }
        /// <summary>Suma de las ventas vigentes que cumplen el filtro (no sólo las de la página).</summary>
        public decimal TotalFacturado { get; set; }
    }




}
