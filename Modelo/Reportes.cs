using System;
using System.Collections.Generic;
using System.Text;

namespace Modelo
{
    public class Reportes
    {
        public class ReporteIngresos
        {
            public int Año { get; set; }
            public int MesNumero { get; set; }
            public decimal TotalIngresos { get; set; }
        }

        public class ReporteLibroMasVendido
        {
            public string Titulo { get; set; }

            public int CantidadVendida { get; set; }

            public decimal IngresoGenerado { get; set; }
        }

        public class ReporteVentasPorGenero
        {
            public string Genero { get; set; }
            public int CantidadVendida { get; set; }
        }
    }
}
