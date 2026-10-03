using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Modelo
{
    /// <summary>Tipos de movimiento de stock. Se guardan como texto en el historial.</summary>
    public static class TipoMovimientoStock
    {
        public const string AltaInicial = "ALTA_INICIAL";
        public const string Venta = "VENTA";
        public const string AnulacionVenta = "ANULACION_VENTA";
        public const string RecepcionOrden = "RECEPCION_ORDEN";
        public const string AjusteIngreso = "AJUSTE_INGRESO";
        public const string AjusteEgreso = "AJUSTE_EGRESO";
        public const string ConteoFisico = "CONTEO_FISICO";

        public static string Descripcion(string tipo) => tipo switch
        {
            AltaInicial => "Alta inicial",
            Venta => "Venta",
            AnulacionVenta => "Anulación de venta",
            RecepcionOrden => "Recepción de orden",
            AjusteIngreso => "Ajuste (ingreso)",
            AjusteEgreso => "Ajuste (egreso)",
            ConteoFisico => "Conteo físico",
            _ => tipo,
        };
    }

    /// <summary>
    /// Historial (kardex) de stock: cada cambio de LIB_Stock deja una fila con el antes, el después,
    /// el motivo, el usuario y el comprobante que lo originó. Es la auditoría del inventario.
    /// </summary>
    public class MovimientoStock
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MOV_ID { get; set; }

        public int LIB_ID { get; set; }
        public Libro MOV_Libro { get; set; }

        public DateTime MOV_Fecha { get; set; } = DateTime.Now;

        [StringLength(30)]
        public string MOV_Tipo { get; set; } = string.Empty;

        /// <summary>Variación con signo: positiva = ingreso, negativa = egreso.</summary>
        public int MOV_Cantidad { get; set; }

        public int MOV_StockAnterior { get; set; }
        public int MOV_StockResultante { get; set; }

        [StringLength(100)]
        public string? MOV_Motivo { get; set; }

        [StringLength(250)]
        public string? MOV_Observacion { get; set; }

        [StringLength(100)]
        public string? MOV_Usuario { get; set; }

        /// <summary>Comprobante que originó el movimiento (TK-000123, OC-000045...).</summary>
        [StringLength(30)]
        public string? MOV_Referencia { get; set; }
    }
}
