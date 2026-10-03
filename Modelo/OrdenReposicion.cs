using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Modelo
{
    /// <summary>Estados de una orden de reposición. Se guardan como texto para que la base sea legible.</summary>
    public static class EstadoOrden
    {
        /// <summary>Borrador: editable, todavía no se pidió al proveedor.</summary>
        public const string Pendiente = "PENDIENTE";
        /// <summary>Pedida al proveedor (por email u otro medio). Ya no se editan los ítems.</summary>
        public const string Solicitada = "SOLICITADA";
        /// <summary>Mercadería recibida: el stock ya se actualizó.</summary>
        public const string Recibida = "RECIBIDA";
        public const string Cancelada = "CANCELADA";

        public static bool EsActiva(string estado) => estado is Pendiente or Solicitada;

        public static string Descripcion(string estado) => estado switch
        {
            Pendiente => "Pendiente (borrador)",
            Solicitada => "Solicitada",
            Recibida => "Recibida",
            Cancelada => "Cancelada",
            _ => estado,
        };
    }

    /// <summary>Cabecera de la orden de compra/reposición a un proveedor.</summary>
    public class OrdenReposicion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OR_ID { get; set; }

        public int OR_PROV_ID { get; set; }
        public Proveedor OR_Proveedor { get; set; }

        /// <summary>Fecha de emisión (creación de la orden).</summary>
        public DateTime OR_Fecha { get; set; } = DateTime.Now;

        [StringLength(20)]
        public string OR_Estado { get; set; } = EstadoOrden.Pendiente;

        [StringLength(100)]
        public string? OR_Usuario { get; set; }

        [StringLength(500)]
        public string? OR_Observaciones { get; set; }

        public DateTime? OR_FechaSolicitud { get; set; }

        public DateTime? OR_FechaRecepcion { get; set; }

        [StringLength(100)]
        public string? OR_UsuarioRecepcion { get; set; }

        public DateTime? OR_FechaCancelacion { get; set; }

        [StringLength(100)]
        public string? OR_UsuarioCancelacion { get; set; }

        [StringLength(250)]
        public string? OR_MotivoCancelacion { get; set; }

        public ICollection<DetalleOrdenReposicion> OR_Detalles { get; set; } = new List<DetalleOrdenReposicion>();

        public static string FormatearNumero(int ordenId) => $"OC-{ordenId:D6}";
    }

    /// <summary>Ítem de la orden: qué libro, cuánto se pidió y a qué costo, y qué llegó realmente.</summary>
    public class DetalleOrdenReposicion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DOR_ID { get; set; }

        public int OR_ID { get; set; }
        public OrdenReposicion DOR_Orden { get; set; }

        public int LIB_ID { get; set; }
        public Libro DOR_Libro { get; set; }

        public int DOR_CantidadPedida { get; set; }

        /// <summary>Costo unitario pactado al emitir la orden.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal DOR_CostoUnitario { get; set; }

        /// <summary>Cantidad efectivamente recibida (null hasta la recepción). Puede diferir de la pedida.</summary>
        public int? DOR_CantidadRecibida { get; set; }

        /// <summary>Costo unitario real facturado en la recepción.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DOR_CostoRecibido { get; set; }
    }
}
