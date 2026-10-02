using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora
{
    public class ControladoraDetallesVenta
    {
        private static ControladoraDetallesVenta instancia;

        public static ControladoraDetallesVenta Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new ControladoraDetallesVenta();
                }
                return instancia;
            }
        }

        public List<DetalleVentaDTO> BuscarDetalles(int ventaID)
        {
            return Libreria.Contexto.DetallesVenta.Include(d => d.DV_Libro).Where(d => d.DV_Venta.VEN_ID == ventaID)
                .Select(d => new DetalleVentaDTO
                {
                    DVDTO_ID = d.DV_ID,
                    Libro = d.DV_Libro.LIB_Titulo,   
                    Cantidad = d.DV_Cantidad,
                    PrecioUnitario = d.DV_PrecioUnitario,
                    Subtotal = d.DV_Subtotal,
                }).ToList();
        }

    }
}
