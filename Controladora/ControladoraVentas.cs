using Controladora.MetodoPagoStrategy;
using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Controladora
{
    public class ControladoraVentas
    {
        private static ControladoraVentas instancia;

        public static ControladoraVentas Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new ControladoraVentas();
                }
                return instancia;
            }
        }
        public TicketVentaDTO GenerarTicket(Venta venta)
        {
            Venta ventaCompleta = Libreria.Contexto.Ventas
                .Include(v => v.VEN_Cliente)
                    .ThenInclude(c => c.CLI_Persona)
                .Include(v => v.VEN_MetodoPago)
                .Include(v => v.VEN_Detalles)
                    .ThenInclude(d => d.DV_Libro)
                .FirstOrDefault(v => v.VEN_ID == venta.VEN_ID);

            if (ventaCompleta == null)
                return null;

            TicketVentaDTO ticket = new TicketVentaDTO
            {
                NumeroVenta = ventaCompleta.VEN_ID,
                Fecha = ventaCompleta.VEN_Fecha,
                Cliente = ventaCompleta.VEN_Cliente.CLI_Persona.PER_Nombre,
                MetodoPago = ventaCompleta.VEN_MetodoPago.MP_Nombre,
                Total = ventaCompleta.VEN_Total
            };

            foreach (var detalle in ventaCompleta.VEN_Detalles)
            {
                ticket.Detalles.Add(new DetalleTicketDTO
                {
                    Libro = detalle.DV_Libro.LIB_Titulo,
                    Cantidad = detalle.DV_Cantidad,
                    PrecioUnitario = detalle.DV_PrecioUnitario,
                    Subtotal = detalle.DV_Cantidad * detalle.DV_PrecioUnitario
                });
            }

            return ticket;
        }
        public List<VentaDTO> filtrarVentasPorFecha(DateTime desde, DateTime hasta)
        {
            return Libreria.Contexto.Ventas.Where(v => v.VEN_Fecha >= desde && v.VEN_Fecha <= hasta)
            .Select(v => new VentaDTO
            {
                VENDTO_ID = v.VEN_ID,
                Fecha = v.VEN_Fecha,
                TotalVenta = v.VEN_Total,
                MetodoPago = v.VEN_MetodoPago.MP_Nombre,
                Cliente = v.VEN_Cliente.CLI_Persona.PER_Nombre,
            }).ToList();
        }

        public List<VentaDTO> obtenerVentasGrid()
        {
            return Libreria.Contexto.Ventas.Include(p => p.VEN_MetodoPago).Include(p => p.VEN_Cliente).ThenInclude(c => c.CLI_Persona)
            .Select(p => new VentaDTO
            {
                VENDTO_ID = p.VEN_ID,
                Fecha = p.VEN_Fecha,
                TotalVenta = p.VEN_Total,
                MetodoPago = p.VEN_MetodoPago.MP_Nombre,
                Cliente = p.VEN_Cliente.CLI_Persona.PER_Nombre,
            }).ToList();
        }
        public Venta buscarVenta(VentaDTO venta)
        {
            return Libreria.Contexto.Ventas
                          .Include(p => p.VEN_MetodoPago).Include(p => p.VEN_Cliente).ThenInclude(c => c.CLI_Persona)
                          .FirstOrDefault(p => p.VEN_ID == venta.VENDTO_ID);
        }
        public Venta CrearVenta(ClienteDTO cliente1, List<LibroVentaDTO> librosVenta, MetodoPago metodoPago, decimal totalParcial)
        {
            IMetodoPagoStrategy estrategia = MetodoPagoStrategyFactory.Obtener(metodoPago);
            decimal totalFinal =estrategia.CalcularTotal(totalParcial);
            var clienteDb = ControladoraClientes.Instancia.BuscarClienteIndividual(cliente1);
            if (clienteDb != null)
            {
                Venta nuevaVenta = new Venta
                {
                    VEN_Fecha = DateTime.Now,
                    VEN_Cliente = clienteDb,
                    VEN_Total = totalFinal,
                    VEN_MetodoPago = metodoPago,
                };
                Libreria.Contexto.Ventas.Add(nuevaVenta);
                Libreria.Contexto.SaveChanges();
                return nuevaVenta;
            }
            else
            {
                return null;
            }
        }
        public List<VentaDTO> buscarVentasPorCliente(string filtro)
        {
            return obtenerVentasGrid().Where(p => p.Cliente.ToString().ToLower().Contains(filtro)).ToList();
        }
        

    }
}
