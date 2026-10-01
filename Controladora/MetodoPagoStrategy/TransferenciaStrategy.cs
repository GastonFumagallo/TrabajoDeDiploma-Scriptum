using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora.MetodoPagoStrategy
{
    public class TransferenciaStrategy : IMetodoPagoStrategy
    {
        public decimal CalcularTotal(decimal subtotal)
        {
            return subtotal * 0.98m;
        }
    }
}
