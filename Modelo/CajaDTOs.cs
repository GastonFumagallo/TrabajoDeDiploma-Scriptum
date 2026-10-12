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
}
