using Controladora;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modelo
{
    public class FacadeVentas
    {
        private static FacadeVentas instancia;

        public static FacadeVentas Instancia
        {
            get
            {
                if (instancia == null)
                    instancia = new FacadeVentas();

                return instancia;
            }
        }

        private FacadeVentas() { }

        public Venta RealizarVenta(ClienteDTO cliente, List<LibroVentaDTO> librosVenta,MetodoPago metodoPago)
        {
            decimal total = librosVenta.Sum(x => x.Precio * x.Cantidad);

            Venta venta = ControladoraVentas.Instancia.CrearVenta(cliente,librosVenta,metodoPago, total);
            if (venta != null)
            {
                ControladoraLibros.Instancia.ModificarStock(librosVenta);
                ControladoraDetallesVenta.Instancia.crearDetalles(venta, librosVenta);
            }
            return venta;
        }
    }
}
