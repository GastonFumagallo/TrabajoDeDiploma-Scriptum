using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora.MetodoPagoStrategy
{
    public class CreditoStrategy : IMetodoPagoStrategy
    {
        public decimal CalcularTotal(decimal subtotal)
        {
            return subtotal * 1.10m;
        }
    }
}
