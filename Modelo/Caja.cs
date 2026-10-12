using Modelo.Seguridad;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modelo
{
    public enum EstadoCaja
    {
        Abierta = 0,
        Cerrada = 1,
    }

    /// <summary>
    /// Sesión de caja (turno o jornada): apertura con un fondo en efectivo, ventas asociadas y cierre con arqueo.
    /// Nunca puede haber más de una abierta (índice único filtrado en la base, ver Libreria.OnModelCreating).
    /// Los importes del cierre son una foto: anular después una venta de una caja cerrada no los modifica.
    /// </summary>
    public class Caja
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CAJ_ID { get; set; }

        public EstadoCaja CAJ_Estado { get; set; } = EstadoCaja.Abierta;

        // ── Apertura ──────────────────────────────────────────────
        public DateTime CAJ_FechaApertura { get; set; }

        public int CAJ_UsuarioAperturaId { get; set; }
        public Usuario CAJ_UsuarioApertura { get; set; } = null!;

        /// <summary>Fondo de caja: efectivo con el que arranca el turno.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal CAJ_MontoInicial { get; set; }

        // ── Cierre (arqueo) ───────────────────────────────────────
        public DateTime? CAJ_FechaCierre { get; set; }

        public int? CAJ_UsuarioCierreId { get; set; }
        public Usuario? CAJ_UsuarioCierre { get; set; }

        /// <summary>Ventas no anuladas de la caja, en todos los medios de pago.</summary>
        public int? CAJ_CantidadVentas { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CAJ_TotalVentas { get; set; }

        /// <summary>Monto inicial + ventas en efectivo: lo que debería haber en el cajón.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CAJ_EfectivoEsperado { get; set; }

        /// <summary>Efectivo contado en mano al cerrar.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CAJ_EfectivoContado { get; set; }

        /// <summary>Contado − esperado: positivo = sobrante, negativo = faltante.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CAJ_Diferencia { get; set; }

        [StringLength(500)]
        public string? CAJ_Observaciones { get; set; }

        public ICollection<Venta> CAJ_Ventas { get; set; } = new List<Venta>();
        public ICollection<CajaTotalMedio> CAJ_TotalesPorMedio { get; set; } = new List<CajaTotalMedio>();
    }

    /// <summary>Foto, al cierre, de lo recaudado por cada medio de pago en una caja.</summary>
    public class CajaTotalMedio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CTM_ID { get; set; }

        public int CAJ_ID { get; set; }
        public Caja CTM_Caja { get; set; } = null!;

        public int MP_ID { get; set; }
        public MetodoPago CTM_MetodoPago { get; set; } = null!;

        /// <summary>Nombre del medio al momento del cierre (si después se renombra, el arqueo no cambia).</summary>
        [StringLength(60)]
        public string CTM_MedioPago { get; set; } = "";

        public bool CTM_EsEfectivo { get; set; }

        public int CTM_Cantidad { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CTM_Total { get; set; }
    }
}
