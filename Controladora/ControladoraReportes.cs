using Microsoft.EntityFrameworkCore;
using Modelo;
using System;
using System.Collections.Generic;
using System.Text;
using Modelo.Contexto;
using static Modelo.Reportes;

namespace Controladora
{
    public class ControladoraReportes
    {
        private static ControladoraReportes instancia;

        public static ControladoraReportes Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new ControladoraReportes();
                }
                return instancia;
            }
        }

        public List<ReporteIngresos> ObtenerIngresos(DateTime fechaDesde, DateTime fechaHasta)
        {
            return Libreria.Contexto.Ventas
            .Where(v => v.VEN_Fecha >= fechaDesde &&
                        v.VEN_Fecha <= fechaHasta)
            .GroupBy(v => new
            {
                v.VEN_Fecha.Year,
                v.VEN_Fecha.Month
            })
            .Select(g => new ReporteIngresos
            {
                Año = g.Key.Year,
                MesNumero = g.Key.Month,
                TotalIngresos = g.Sum(v => v.VEN_Total)
            })
            .OrderBy(x => x.Año)
            .ThenBy(x => x.MesNumero)
            .ToList();
        }
        public List<ReporteLibroMasVendido> ObtenerLibrosMasVendidos(DateTime fechaDesde, DateTime fechaHasta)
        {
            return Libreria.Contexto.DetallesVenta
                .Where(dv => dv.DV_Venta.VEN_Fecha >= fechaDesde &&
                             dv.DV_Venta.VEN_Fecha <= fechaHasta)
                .GroupBy(dv => dv.DV_Libro.LIB_Titulo)
                .Select(g => new Reportes.ReporteLibroMasVendido
                {
                    Titulo = g.Key,
                    CantidadVendida = g.Sum(dv => dv.DV_Cantidad),
                    IngresoGenerado = g.Sum(dv => dv.DV_Cantidad * dv.DV_PrecioUnitario)
                })
                .OrderByDescending(x => x.CantidadVendida)
                .Take(10)
                .ToList();
        }

        public List<ReporteVentasPorGenero> ObtenerVentasPorGenero(DateTime fechaDesde, DateTime fechaHasta)
        {
            return Libreria.Contexto.DetallesVenta
                .Where(dv => dv.DV_Venta.VEN_Fecha >= fechaDesde &&
                             dv.DV_Venta.VEN_Fecha <= fechaHasta)
                .GroupBy(dv => dv.DV_Libro.LIB_Genero.GEN_Nombre)
                .Select(g => new ReporteVentasPorGenero
                {
                    Genero = g.Key,
                    CantidadVendida = g.Sum(dv => dv.DV_Cantidad)
                })
                .OrderByDescending(x => x.CantidadVendida)
                .ToList();
        }

    }
}
