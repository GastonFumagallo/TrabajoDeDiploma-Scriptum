using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora.MetodoPagoStrategy
{
    public interface IMetodoPagoStrategy
    {
        decimal CalcularTotal(decimal subtotal);

    }
}
