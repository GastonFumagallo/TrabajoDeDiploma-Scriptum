namespace Modelo
{
    /// <summary>Caja abierta en este momento (para mostrarla en Ventas y en el punto de venta).</summary>
    public sealed record CajaActivaDTO(int Id, DateTime FechaApertura, string UsuarioApertura, decimal MontoInicial)
    {
        public string Descripcion => $"Caja #{Id} abierta el {FechaApertura:dd/MM HH:mm} por {UsuarioApertura}";
    }

    /// <summary>Recaudado por un medio de pago en una caja.</summary>
    public sealed record TotalMedioCajaDTO(int MetodoPagoId, string MedioPago, bool EsEfectivo, int Cantidad, decimal Total);

    /// <summary>
    /// Arqueo de una caja: lo que el sistema dice que debería haber. Se usa como vista previa al cerrar
    /// (<see cref="Contado"/> null) y como resultado del cierre (con lo contado y la diferencia).
    /// </summary>
    public sealed class ArqueoCajaDTO
    {
        public int CajaId { get; init; }
        public DateTime FechaApertura { get; init; }
        public string UsuarioApertura { get; init; } = "";
        public decimal MontoInicial { get; init; }

        public IReadOnlyList<TotalMedioCajaDTO> TotalesPorMedio { get; init; } = Array.Empty<TotalMedioCajaDTO>();
        public int CantidadVentas { get; init; }
        public decimal TotalVentas { get; init; }
        public decimal VentasEfectivo => TotalesPorMedio.Where(t => t.EsEfectivo).Sum(t => t.Total);

        /// <summary>Monto inicial + ventas en efectivo.</summary>
        public decimal EfectivoEsperado => MontoInicial + VentasEfectivo;

        // Sólo en el resultado del cierre.
        public DateTime? FechaCierre { get; init; }
        public decimal? Contado { get; init; }
        public decimal? Diferencia => Contado - EfectivoEsperado;

        public static string DescribirDiferencia(decimal diferencia) => diferencia switch
        {
            > 0 => $"Sobrante de {diferencia:C2}",
            < 0 => $"Faltante de {-diferencia:C2}",
            _ => "Sin diferencia",
        };
    }

    // ── Historial de cajas cerradas ───────────────────────────────

    /// <summary>
    /// Criterios del historial de cajas cerradas. El período se aplica sobre la fecha de apertura (el día del
    /// turno). El usuario coincide si abrió o cerró la caja.
    /// </summary>
    public sealed class FiltroHistorialCajas
    {
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public int? UsuarioId { get; set; }
    }

    /// <summary>Una caja cerrada en la grilla del historial. Los importes son la foto guardada al cerrar.</summary>
    public sealed class CajaHistorialDTO
    {
        public int Id { get; init; }
        public string Numero => $"#{Id}";
        public DateTime FechaApertura { get; init; }
        public string UsuarioApertura { get; init; } = "";
        public DateTime FechaCierre { get; init; }
        public string UsuarioCierre { get; init; } = "";
        public decimal MontoInicial { get; init; }
        public int CantidadVentas { get; init; }
        public decimal TotalVentas { get; init; }
        public decimal EfectivoEsperado { get; init; }
        public decimal EfectivoContado { get; init; }
        /// <summary>Contado − esperado: positivo = sobrante, negativo = faltante.</summary>
        public decimal Diferencia { get; init; }
        public string Resultado => ArqueoCajaDTO.DescribirDiferencia(Diferencia);
        public string? Observaciones { get; init; }
    }

    /// <summary>Usuario que abrió o cerró alguna caja (opciones del filtro por cajero).</summary>
    public sealed record UsuarioCajaDTO(int Id, string Nombre)
    {
        public override string ToString() => Nombre;
    }

    /// <summary>Totales del historial filtrado (todas las páginas), calculados en SQL.</summary>
    public sealed class MetricasHistorialCajas
    {
        public int CantidadCajas { get; init; }
        public decimal TotalRecaudado { get; init; }
        /// <summary>Suma de sobrantes y faltantes: un sobrante puede tapar un faltante, por eso se informan aparte.</summary>
        public decimal DiferenciaNeta { get; init; }
        public decimal TotalFaltantes { get; init; }
        public int CajasConFaltante { get; init; }
    }

    public sealed class PaginaHistorialCajas
    {
        public IReadOnlyList<CajaHistorialDTO> Cajas { get; init; } = Array.Empty<CajaHistorialDTO>();
        public MetricasHistorialCajas Metricas { get; init; } = new();
        public int TotalRegistros => Metricas.CantidadCajas;
    }

    /// <summary>Detalle de una caja cerrada: desglose por medio de pago y observaciones del cierre.</summary>
    public sealed class CajaDetalleDTO
    {
        public CajaHistorialDTO Caja { get; init; } = new();
        public IReadOnlyList<TotalMedioCajaDTO> TotalesPorMedio { get; init; } = Array.Empty<TotalMedioCajaDTO>();
        public string? Observaciones => Caja.Observaciones;
        /// <summary>
        /// Ventas de la caja anuladas después del cierre. El arqueo guardado no las descuenta (es una foto),
        /// pero conviene verlas al auditar.
        /// </summary>
        public int VentasAnuladasLuego { get; init; }
    }
}
