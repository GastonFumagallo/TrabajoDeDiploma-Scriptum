using Modelo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora.MetodoPagoStrategy
{
    public static class MetodoPagoStrategyFactory
    {
        public static IMetodoPagoStrategy Obtener(MetodoPago metodoPago)
        {
            // Se compara en mayúsculas contra etiquetas en mayúsculas: antes se hacía ToUpper()
            // contra "Efectivo", "Tarjeta de debito", etc., y nunca coincidía (siempre caía en Efectivo).
            switch (metodoPago?.MP_Nombre?.Trim().ToUpperInvariant())
            {
                case "EFECTIVO":
                    return new EfectivoStrategy();

                case "TARJETA DE DEBITO":
                case "TARJETA DE DÉBITO":
                    return new DebitoStrategy();

                case "TARJETA DE CREDITO":
                case "TARJETA DE CRÉDITO":
                    return new CreditoStrategy();

                case "TRANSFERENCIA":
                    return new TransferenciaStrategy();

                default:
                    return new EfectivoStrategy();
            }
        }
    }
}
