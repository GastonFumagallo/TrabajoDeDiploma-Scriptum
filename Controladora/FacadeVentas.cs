using Controladora;
using Controladora.MetodoPagoStrategy;
using Microsoft.EntityFrameworkCore;
using Modelo.Contexto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modelo
{
    /// <summary>
    /// Resultado de intentar registrar una venta. Si <see cref="Exito"/> es false,
    /// <see cref="Mensaje"/> explica el motivo (stock insuficiente, cliente inexistente, etc.)
    /// y no se persistió nada.
    /// </summary>
    public sealed class ResultadoVenta
    {
        public bool Exito { get; private init; }
        public string Mensaje { get; private init; } = string.Empty;
        public int VentaId { get; private init; }
        public decimal Subtotal { get; private init; }
        public decimal Total { get; private init; }

        public static ResultadoVenta Ok(int ventaId, decimal subtotal, decimal total) =>
            new() { Exito = true, VentaId = ventaId, Subtotal = subtotal, Total = total };

        public static ResultadoVenta Error(string mensaje) =>
            new() { Exito = false, Mensaje = mensaje };
    }

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

        /// <summary>
        /// Registra la venta completa (cabecera, detalles y rebaja de stock) en una única transacción.
        /// Usa un DbContext propio y de vida corta para no compartir el contexto global
        /// (que no es thread-safe y acumula entidades trackeadas).
        /// </summary>
        /// <param name="clientePersonaId">Valor de <see cref="ClienteDTO.CLIDTO_ID"/> (es el PER_ID de la persona).</param>
        public async Task<ResultadoVenta> RealizarVentaAsync(int clientePersonaId, int metodoPagoId,
            IReadOnlyCollection<LibroVentaDTO> librosVenta, CancellationToken ct = default)
        {
            if (librosVenta == null || librosVenta.Count == 0)
                return ResultadoVenta.Error("La venta no tiene productos.");

            // Consolidamos por libro por si llegara el mismo ID en dos líneas.
            var cantidades = librosVenta
                .GroupBy(l => l.LVDTO_ID)
                .ToDictionary(g => g.Key, g => g.Sum(l => l.Cantidad));

            if (cantidades.Values.Any(c => c <= 0))
                return ResultadoVenta.Error("Todas las cantidades deben ser mayores a cero.");

            await using var db = new Libreria();
            await using var tx = await db.Database.BeginTransactionAsync(ct);

            var cliente = await db.Clientes.FirstOrDefaultAsync(c => c.CLI_Persona.PER_ID == clientePersonaId, ct);
            if (cliente == null)
                return ResultadoVenta.Error("El cliente seleccionado ya no existe.");

            var metodoPago = await db.MetodosPago.FirstOrDefaultAsync(m => m.MP_ID == metodoPagoId, ct);
            if (metodoPago == null)
                return ResultadoVenta.Error("El método de pago seleccionado ya no existe.");

            var ids = cantidades.Keys.ToList();
            var libros = await db.Libros.Where(l => ids.Contains(l.LIB_ID)).ToDictionaryAsync(l => l.LIB_ID, ct);
            if (libros.Count != ids.Count)
                return ResultadoVenta.Error("Uno o más libros de la venta ya no existen.");

            // Rebaja de stock atómica: el UPDATE sólo afecta la fila si todavía hay stock suficiente.
            // Así dos cajas que venden el mismo libro a la vez no pueden dejar stock negativo.
            foreach (var (libroId, cantidad) in cantidades)
            {
                int filas = await db.Libros
                    .Where(l => l.LIB_ID == libroId && l.LIB_Stock >= cantidad)
                    .ExecuteUpdateAsync(s => s.SetProperty(l => l.LIB_Stock, l => l.LIB_Stock - cantidad), ct);

                if (filas == 0)
                {
                    await tx.RollbackAsync(ct);
                    return ResultadoVenta.Error($"Stock insuficiente para '{libros[libroId].LIB_Titulo}'.");
                }
            }

            // El precio se toma de la base, no de la grilla: la UI puede estar desactualizada.
            decimal subtotal = cantidades.Sum(kv => libros[kv.Key].LIB_PrecioVenta * kv.Value);
            decimal total = Math.Round(MetodoPagoStrategyFactory.Obtener(metodoPago).CalcularTotal(subtotal), 2);

            var venta = new Venta
            {
                VEN_Fecha = DateTime.Now,
                CLI_ID = cliente.CLI_ID,
                VEN_Cliente = cliente,
                MP_ID = metodoPago.MP_ID,
                VEN_MetodoPago = metodoPago,
                VEN_Total = total,
            };

            foreach (var (libroId, cantidad) in cantidades)
            {
                venta.VEN_Detalles.Add(new DetalleVenta
                {
                    DV_Venta = venta,
                    LIB_ID = libroId,
                    DV_Libro = libros[libroId],
                    DV_Cantidad = cantidad,
                    DV_PrecioUnitario = libros[libroId].LIB_PrecioVenta,
                });
            }

            db.Ventas.Add(venta);
            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return ResultadoVenta.Ok(venta.VEN_ID, subtotal, total);
        }
    }
}
