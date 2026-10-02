using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora
{
    /// <summary>
    /// Consultas de ventas. Cada método usa un DbContext propio, de vida corta y sin tracking:
    /// son lecturas para mostrar en pantalla, no hace falta que EF siga los cambios.
    /// Todas proyectan a DTO con Select(): EF arma un único SELECT con los JOIN necesarios (sin N+1)
    /// y trae sólo las columnas que se muestran.
    /// La escritura (alta y anulación) vive en <see cref="FacadeVentas"/> porque necesita transacción.
    /// </summary>
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

        /// <summary>
        /// Devuelve una página del listado más las métricas de todo el período filtrado.
        /// Filtros, orden, conteo, agregados y paginado se resuelven en SQL.
        /// </summary>
        /// <param name="tamañoPagina">null = sin paginar (para exportar).</param>
        public async Task<PaginaVentas> BuscarVentasAsync(FiltroVentas filtro, int pagina, int? tamañoPagina, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            var query = AplicarFiltro(db.Ventas.AsNoTracking(), filtro);

            // Una sola consulta agregada para las métricas (GROUP BY constante → un único SELECT con COUNT/SUM).
            var metricas = await query
                .GroupBy(_ => 1)
                .Select(g => new MetricasVentas
                {
                    CantidadCompletadas = g.Count(v => !v.VEN_Anulada),
                    CantidadAnuladas = g.Count(v => v.VEN_Anulada),
                    TotalRecaudado = g.Where(v => !v.VEN_Anulada).Sum(v => (decimal?)v.VEN_Total) ?? 0m,
                })
                .FirstOrDefaultAsync(ct) ?? new MetricasVentas();

            var ordenada = query.OrderByDescending(v => v.VEN_Fecha).ThenByDescending(v => v.VEN_ID);
            var paginada = tamañoPagina is int tam
                ? ordenada.Skip(Math.Max(0, pagina - 1) * tam).Take(tam)
                : ordenada;

            var ventas = await paginada
                .Select(v => new VentaDTO
                {
                    VENDTO_ID = v.VEN_ID,
                    Fecha = v.VEN_Fecha,
                    Cliente = v.VEN_Cliente.CLI_Persona.PER_Nombre,
                    MetodoPago = v.VEN_MetodoPago.MP_Nombre,
                    TotalVenta = v.VEN_Total,
                    Anulada = v.VEN_Anulada,
                    Estado = v.VEN_Anulada ? "Anulada" : "Completada",
                    Usuario = v.VEN_Usuario,
                    MotivoAnulacion = v.VEN_MotivoAnulacion,
                })
                .ToListAsync(ct);

            return new PaginaVentas
            {
                Ventas = ventas,
                TotalRegistros = metricas.CantidadCompletadas + metricas.CantidadAnuladas,
                Metricas = metricas,
            };
        }

        private static IQueryable<Venta> AplicarFiltro(IQueryable<Venta> query, FiltroVentas filtro)
        {
            // El número de comprobante identifica una venta: si viene, los demás filtros igual se combinan.
            if (filtro.NumeroComprobante is int numero)
                query = query.Where(v => v.VEN_ID == numero);

            if (!string.IsNullOrWhiteSpace(filtro.Cliente))
            {
                string texto = filtro.Cliente.Trim();
                // Se traduce a LIKE '%texto%' en SQL Server (la intercalación por defecto ya ignora mayúsculas).
                query = query.Where(v => v.VEN_Cliente.CLI_Persona.PER_Nombre.Contains(texto)
                                      || v.VEN_Cliente.CLI_Persona.PER_DNI.ToString().Contains(texto));
            }

            if (filtro.Desde is DateTime desde)
                query = query.Where(v => v.VEN_Fecha >= desde.Date);

            if (filtro.Hasta is DateTime hasta)
            {
                var hastaExclusivo = hasta.Date.AddDays(1);  // incluye todo el día "hasta"
                query = query.Where(v => v.VEN_Fecha < hastaExclusivo);
            }

            if (filtro.MetodoPagoId is int metodoId)
                query = query.Where(v => v.MP_ID == metodoId);

            query = filtro.Estado switch
            {
                EstadoVentaFiltro.Completadas => query.Where(v => !v.VEN_Anulada),
                EstadoVentaFiltro.Anuladas => query.Where(v => v.VEN_Anulada),
                _ => query,
            };

            return query;
        }

        /// <summary>Interpreta "TK-000123", "000123" o "123". Devuelve null si no hay un número válido.</summary>
        public static int? ParsearComprobante(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return null;
            var digitos = new string(texto.Where(char.IsDigit).ToArray());
            return int.TryParse(digitos, out int numero) && numero > 0 ? numero : null;
        }

        public async Task<TicketVentaDTO?> GenerarTicketAsync(int ventaId, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await db.Ventas.AsNoTracking()
                .Where(v => v.VEN_ID == ventaId)
                .Select(v => new TicketVentaDTO
                {
                    NumeroVenta = v.VEN_ID,
                    Fecha = v.VEN_Fecha,
                    Cliente = v.VEN_Cliente.CLI_Persona.PER_Nombre,
                    MetodoPago = v.VEN_MetodoPago.MP_Nombre,
                    Subtotal = v.VEN_Subtotal,
                    Descuento = v.VEN_Descuento,
                    AjusteMedioPago = v.VEN_AjusteMedioPago,
                    IVA = v.VEN_IVA,
                    Total = v.VEN_Total,
                    Recibido = v.VEN_Pagos.Sum(p => (decimal?)p.PAG_Recibido) ?? v.VEN_Total,
                    Vuelto = v.VEN_Pagos.Sum(p => (decimal?)p.PAG_Vuelto) ?? 0m,
                    Usuario = v.VEN_Usuario,
                    Anulada = v.VEN_Anulada,
                    FechaAnulacion = v.VEN_FechaAnulacion,
                    MotivoAnulacion = v.VEN_MotivoAnulacion,
                    Detalles = v.VEN_Detalles.Select(d => new DetalleTicketDTO
                    {
                        Libro = d.DV_Libro.LIB_Titulo,
                        Cantidad = d.DV_Cantidad,
                        PrecioUnitario = d.DV_PrecioUnitario,
                        Subtotal = d.DV_Cantidad * d.DV_PrecioUnitario,
                    }).ToList(),
                })
                .FirstOrDefaultAsync(ct);
        }
    }
}
