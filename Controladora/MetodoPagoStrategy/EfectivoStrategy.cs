using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora.MetodoPagoStrategy
{
    public class EfectivoStrategy : IMetodoPagoStrategy
    {
        public decimal CalcularTotal(decimal subtotal)
        {
            return subtotal;
        }
    }
}
