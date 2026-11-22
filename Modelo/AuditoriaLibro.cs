using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modelo
{
    /// <summary>Operaciones que dejan rastro en la trazabilidad de un libro.</summary>
    public static class OperacionAuditoriaLibro
    {
        public const string Alta = "Alta";
        public const string Modificacion = "Modificación";
        public const string Baja = "Baja";
        public const string Reactivacion = "Reactivación";
        public const string ParametrosStock = "Parámetros de stock";
        public const string RecepcionOrden = "Recepción de orden";
    }

    /// <summary>
    /// Trazabilidad de libros: una fila por cada campo que cambió en una operación, con el valor original y el nuevo,
    /// el usuario y la fecha. Las filas de una misma operación comparten <see cref="AUL_Lote"/>. Los cambios de stock
    /// no se guardan acá: ya quedan en <see cref="MovimientoStock"/>, y el reporte de trazabilidad une ambas fuentes.
    /// </summary>
    public class AuditoriaLibro
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AUL_ID { get; set; }

        public int LIB_ID { get; set; }
        public Libro AUL_Libro { get; set; } = null!;

        public DateTime AUL_Fecha { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string AUL_Usuario { get; set; } = string.Empty;

        [StringLength(60)]
        public string AUL_Operacion { get; set; } = string.Empty;

        [StringLength(60)]
        public string AUL_Campo { get; set; } = string.Empty;

        [StringLength(500)]
        public string? AUL_ValorAnterior { get; set; }

        [StringLength(500)]
        public string? AUL_ValorNuevo { get; set; }

        /// <summary>Agrupa los campos modificados en una misma operación.</summary>
        public Guid AUL_Lote { get; set; }

        [StringLength(100)]
        public string? AUL_Referencia { get; set; }
    }
}
