using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora
{
    /// <summary>
    /// Única puerta de entrada para modificar LIB_Stock. Todas las operaciones (venta, anulación, recepción,
    /// ajustes) pasan por acá, dentro de la transacción de quien llama, y cada cambio genera su
    /// <see cref="MovimientoStock"/> con el stock anterior y el resultante (auditoría/kardex).
    ///
    /// Los cambios se hacen con UPDATE condicionales en la base (no leyendo y escribiendo desde memoria),
    /// así dos operaciones simultáneas nunca pisan el stock ni lo dejan negativo.
    /// </summary>
    internal static class RegistroStock
    {
        /// <summary>
        /// Suma (delta &gt; 0) o resta (delta &lt; 0) unidades. Una resta sólo se aplica si alcanza el stock.
        /// Devuelve el movimiento a registrar (sin agregarlo al contexto, para que quien llama complete la
        /// referencia cuando la conozca), o null si no había stock suficiente.
        /// </summary>
        public static async Task<MovimientoStock?> AplicarAsync(Libreria db, int libroId, int delta, string tipo,
            string? usuario, string? motivo = null, string? observacion = null, string? referencia = null,
            CancellationToken ct = default)
        {
            var query = db.Libros.Where(l => l.LIB_ID == libroId);
            if (delta < 0)
                query = query.Where(l => l.LIB_Stock >= -delta);

            int filas = await query.ExecuteUpdateAsync(s => s.SetProperty(l => l.LIB_Stock, l => l.LIB_Stock + delta), ct);
            if (filas == 0)
                return null;

            // La fila quedó bloqueada por el UPDATE dentro de la transacción: esta lectura es consistente.
            int resultante = await db.Libros.Where(l => l.LIB_ID == libroId).Select(l => l.LIB_Stock).FirstAsync(ct);

            return new MovimientoStock
            {
                LIB_ID = libroId,
                MOV_Fecha = DateTime.Now,
                MOV_Tipo = tipo,
                MOV_Cantidad = delta,
                MOV_StockAnterior = resultante - delta,
                MOV_StockResultante = resultante,
                MOV_Motivo = Recortar(motivo, 100),
                MOV_Observacion = Recortar(observacion, 250),
                MOV_Usuario = Recortar(usuario, 100),
                MOV_Referencia = Recortar(referencia, 30),
            };
        }

        /// <summary>
        /// Fija el stock en un valor absoluto (conteo físico). Usa concurrencia optimista: si el stock cambió
        /// entre la lectura y la escritura (ej. se vendió algo en otra caja), no aplica y devuelve null.
        /// </summary>
        public static async Task<MovimientoStock?> FijarAsync(Libreria db, int libroId, int nuevoStock, string? usuario,
            string? motivo, string? observacion, CancellationToken ct = default)
        {
            int? anterior = await db.Libros.Where(l => l.LIB_ID == libroId).Select(l => (int?)l.LIB_Stock).FirstOrDefaultAsync(ct);
            if (anterior == null)
                return null;

            int filas = await db.Libros
                .Where(l => l.LIB_ID == libroId && l.LIB_Stock == anterior)
                .ExecuteUpdateAsync(s => s.SetProperty(l => l.LIB_Stock, nuevoStock), ct);
            if (filas == 0)
                return null;

            return new MovimientoStock
            {
                LIB_ID = libroId,
                MOV_Fecha = DateTime.Now,
                MOV_Tipo = TipoMovimientoStock.ConteoFisico,
                MOV_Cantidad = nuevoStock - anterior.Value,
                MOV_StockAnterior = anterior.Value,
                MOV_StockResultante = nuevoStock,
                MOV_Motivo = Recortar(motivo, 100),
                MOV_Observacion = Recortar(observacion, 250),
                MOV_Usuario = Recortar(usuario, 100),
            };
        }

        private static string? Recortar(string? texto, int max) =>
            string.IsNullOrWhiteSpace(texto) ? null : texto.Trim().Length <= max ? texto.Trim() : texto.Trim()[..max];
    }
}
