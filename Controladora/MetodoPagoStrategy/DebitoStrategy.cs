using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora.MetodoPagoStrategy
{
    public class DebitoStrategy : IMetodoPagoStrategy
    {
        public decimal CalcularTotal(decimal subtotal)
        {
            return subtotal * 0.95m;
        }
    }
}
