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
            switch (metodoPago.MP_Nombre.ToUpper())
            {
                case "Efectivo":
                    return new EfectivoStrategy();

                case "Tarjeta de debito":
                    return new DebitoStrategy();

                case "Tarjeta de credito":
                    return new CreditoStrategy();

                case "Transferencia":
                    return new TransferenciaStrategy();

                default:
                    return new EfectivoStrategy();
            }
        }
    }
}
