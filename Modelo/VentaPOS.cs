using System;
using System.Collections.Generic;
using System.Text;

namespace Modelo
{
    /// <summary>Una línea del carrito tal como la envía la UI: sólo qué libro y cuántos. El precio lo pone el servidor.</summary>
    public sealed record ItemVentaSolicitud(int LibroId, int Cantidad);

    /// <summary>Todo lo que la UI necesita mandar para registrar una venta.</summary>
    public sealed class SolicitudVenta
    {
        /// <summary>CLI_ID del cliente (<see cref="ClienteDTO.CLIDTO_ID"/>). null = Consumidor Final.</summary>
        public int? ClienteId { get; init; }
        public int MetodoPagoId { get; init; }
        public decimal PorcentajeDescuento { get; init; }
        /// <summary>Importe entregado por el cliente. Sólo se exige en efectivo; en otros medios se toma el total.</summary>
        public decimal? MontoRecibido { get; init; }
        public string Usuario { get; init; } = string.Empty;
        public IReadOnlyList<ItemVentaSolicitud> Items { get; init; } = Array.Empty<ItemVentaSolicitud>();
    }

    /// <summary>
    /// Desglose de importes de una venta. Lo produce <c>CalculadoraVenta</c> tanto para la vista previa
    /// en pantalla como para el registro definitivo, así ambos nunca difieren en la fórmula.
    /// </summary>
    public sealed record CalculoVenta(
        decimal Subtotal,
        decimal PorcentajeDescuento,
        decimal Descuento,
        decimal AjusteMedioPago,
        decimal Total,
        decimal IVA,
        decimal Recibido,
        decimal Vuelto)
    {
        /// <summary>true si el monto recibido no alcanza a cubrir el total.</summary>
        public bool PagoInsuficiente => Recibido < Total;
    }

    /// <summary>
    /// Resultado de una operación de venta/anulación. Si <see cref="Exito"/> es false, <see cref="Mensaje"/>
    /// es apto para mostrar al usuario y no se persistió nada.
    /// </summary>
    public sealed class ResultadoVenta
    {
        public bool Exito { get; private init; }
        public string Mensaje { get; private init; } = string.Empty;
        public int VentaId { get; private init; }
        public string Comprobante => Venta.FormatearComprobante(VentaId);
        public CalculoVenta? Calculo { get; private init; }

        public static ResultadoVenta Ok(int ventaId, CalculoVenta? calculo = null) =>
            new() { Exito = true, VentaId = ventaId, Calculo = calculo };

        public static ResultadoVenta Error(string mensaje) =>
            new() { Exito = false, Mensaje = mensaje };
    }
}
