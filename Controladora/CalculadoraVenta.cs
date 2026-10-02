using Controladora.MetodoPagoStrategy;
using Modelo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora
{
    /// <summary>Parámetros de venta configurables en appsettings.json (sección "Ventas").</summary>
    public static class ConfiguracionVentas
    {
        /// <summary>
        /// Tasa de IVA contenida en los precios (0.21 = 21 %). Por defecto 0: en Argentina la venta de libros
        /// está exenta de IVA. Si se configura, el IVA se informa como "contenido" (los precios son finales).
        /// </summary>
        public static decimal TasaIVA => ConfigurationHelper.GetDecimal("Ventas:TasaIVA", 0m);

        /// <summary>Tope del descuento manual que puede aplicar un vendedor, en porcentaje.</summary>
        public static decimal DescuentoMaximoPorcentaje => ConfigurationHelper.GetDecimal("Ventas:DescuentoMaximoPorcentaje", 30m);
    }

    /// <summary>
    /// Única fórmula de importes de una venta. La usa la pantalla (vista previa en tiempo real) y el servicio
    /// (registro definitivo con precios de la base), así lo que ve el cajero es exactamente lo que se guarda.
    ///
    ///   Subtotal  = Σ precio × cantidad
    ///   Descuento = Subtotal × % descuento manual
    ///   Base      = Subtotal − Descuento
    ///   Total     = estrategia del medio de pago aplicada sobre Base   (recargo crédito, descuento débito, etc.)
    ///   Ajuste    = Total − Base
    ///   IVA       = Total − Total / (1 + tasa)                          (contenido, informativo)
    ///   Vuelto    = Recibido − Total                                    (sólo efectivo)
    /// </summary>
    public static class CalculadoraVenta
    {
        public static CalculoVenta Calcular(decimal subtotal, decimal porcentajeDescuento, MetodoPago? metodoPago,
            decimal? montoRecibido, decimal tasaIva)
        {
            subtotal = Redondear(subtotal);
            porcentajeDescuento = Math.Clamp(porcentajeDescuento, 0m, 100m);

            decimal descuento = Redondear(subtotal * porcentajeDescuento / 100m);
            decimal baseImponible = subtotal - descuento;

            decimal total = metodoPago == null
                ? baseImponible
                : Redondear(MetodoPagoStrategyFactory.Obtener(metodoPago).CalcularTotal(baseImponible));

            decimal ajuste = total - baseImponible;
            decimal iva = tasaIva > 0 ? Redondear(total - total / (1 + tasaIva)) : 0m;

            // Sólo en efectivo el cliente entrega un monto distinto del total; en el resto se cobra exacto.
            bool esEfectivo = metodoPago != null && MetodoPagoStrategyFactory.EsEfectivo(metodoPago);
            decimal recibido = esEfectivo ? Redondear(montoRecibido ?? 0m) : total;
            decimal vuelto = esEfectivo ? Math.Max(0m, recibido - total) : 0m;

            return new CalculoVenta(subtotal, porcentajeDescuento, descuento, ajuste, total, iva, recibido, vuelto);
        }

        private static decimal Redondear(decimal valor) => Math.Round(valor, 2, MidpointRounding.AwayFromZero);
    }
}
