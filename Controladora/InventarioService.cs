using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Modelo;
using Modelo.Contexto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora
{
    /// <summary>
    /// Lógica del inventario: consultas para el panel de control, ajustes manuales auditados y
    /// parámetros de reposición. Cada método usa su propio DbContext de vida corta; las lecturas
    /// son proyecciones a DTO con AsNoTracking (una sola consulta SQL, sin N+1).
    /// </summary>
    public class InventarioService
    {
        private static InventarioService? instancia;
        public static InventarioService Instancia => instancia ??= new InventarioService();
        private InventarioService() { }

        #region Consultas

        /// <summary>Productos del inventario con estado, proveedor habitual y unidades ya pedidas. Ordena los más urgentes primero.</summary>
        public async Task<List<ProductoInventarioDTO>> ObtenerInventarioAsync(FiltroInventario filtro, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            var query = db.Libros.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filtro.Texto))
            {
                string texto = filtro.Texto.Trim();
                string? isbn = Libro.NormalizarISBN(texto);
                query = query.Where(l => l.LIB_Titulo.Contains(texto)
                                      || l.LIB_Autor.Contains(texto)
                                      || l.LIB_Editorial.Contains(texto)
                                      || (isbn != null && l.LIB_ISBN != null && l.LIB_ISBN.Contains(isbn)));
            }

            if (filtro.GeneroId is int generoId)
                query = query.Where(l => l.GEN_ID == generoId);

            if (filtro.ProveedorId is int proveedorId)
                query = query.Where(l => l.LIB_Proveedores.Any(pl => pl.PROV_ID == proveedorId));

            query = filtro.Estado switch
            {
                FiltroEstadoStock.Agotados => query.Where(l => l.LIB_Stock <= 0),
                FiltroEstadoStock.BajoMinimo => query.Where(l => l.LIB_Stock <= l.LIB_StockMinimo),
                FiltroEstadoStock.RequierenReposicion => query.Where(l => l.LIB_Stock <= l.LIB_PuntoReposicion),
                _ => query,
            };

            return await query
                // Primero lo más urgente: agotados, críticos, a reponer, normales.
                .OrderBy(l => l.LIB_Stock <= 0 ? 0 : l.LIB_Stock <= l.LIB_StockMinimo ? 1 : l.LIB_Stock <= l.LIB_PuntoReposicion ? 2 : 3)
                .ThenBy(l => l.LIB_Titulo)
                .Select(l => new ProductoInventarioDTO
                {
                    LibroId = l.LIB_ID,
                    Codigo = l.LIB_ISBN,
                    Titulo = l.LIB_Titulo,
                    Autor = l.LIB_Autor,
                    Categoria = l.LIB_Genero.GEN_Nombre,
                    // Proveedor habitual: el asociado con menor precio de compra (subconsulta, no N+1).
                    ProveedorHabitualId = l.LIB_Proveedores.OrderBy(pl => pl.PL_PrecioCompra).Select(pl => (int?)pl.PROV_ID).FirstOrDefault(),
                    ProveedorHabitual = l.LIB_Proveedores.OrderBy(pl => pl.PL_PrecioCompra)
                        .Select(pl => pl.PL_Proveedor.PROV_Empresa != "" ? pl.PL_Proveedor.PROV_Empresa : pl.PL_Proveedor.PER_Proveedor.PER_Nombre)
                        .FirstOrDefault(),
                    Stock = l.LIB_Stock,
                    StockMinimo = l.LIB_StockMinimo,
                    PuntoReposicion = l.LIB_PuntoReposicion,
                    StockOptimo = l.LIB_StockOptimo,
                    PrecioCosto = l.LIB_PrecioCosto,
                    PrecioVenta = l.LIB_PrecioVenta,
                    EnPedido = db.DetallesOrdenReposicion
                        .Where(d => d.LIB_ID == l.LIB_ID
                                 && (d.DOR_Orden.OR_Estado == EstadoOrden.Pendiente || d.DOR_Orden.OR_Estado == EstadoOrden.Solicitada))
                        .Sum(d => (int?)d.DOR_CantidadPedida) ?? 0,
                })
                .ToListAsync(ct);
        }

        /// <summary>Indicadores de todo el catálogo, en una sola consulta agregada.</summary>
        public async Task<ResumenInventarioDTO> ObtenerResumenAsync(CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await db.Libros.AsNoTracking()
                .GroupBy(_ => 1)
                .Select(g => new ResumenInventarioDTO
                {
                    Total = g.Count(),
                    Agotados = g.Count(l => l.LIB_Stock <= 0),
                    Criticos = g.Count(l => l.LIB_Stock > 0 && l.LIB_Stock <= l.LIB_StockMinimo),
                    AReponer = g.Count(l => l.LIB_Stock > l.LIB_StockMinimo && l.LIB_Stock <= l.LIB_PuntoReposicion),
                    Normales = g.Count(l => l.LIB_Stock > l.LIB_PuntoReposicion),
                    ValorCosto = g.Sum(l => (decimal?)(l.LIB_Stock * l.LIB_PrecioCosto)) ?? 0m,
                })
                .FirstOrDefaultAsync(ct) ?? new ResumenInventarioDTO();
        }

        public async Task<List<OpcionDTO>> ObtenerGenerosAsync(CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await db.Generos.AsNoTracking()
                .OrderBy(g => g.GEN_Nombre)
                .Select(g => new OpcionDTO(g.GEN_ID, g.GEN_Nombre))
                .ToListAsync(ct);
        }

        /// <summary>Proveedores asociados a un libro, del más barato al más caro (el primero es el habitual).</summary>
        public async Task<List<ProveedorDeLibroDTO>> ObtenerProveedoresDeLibroAsync(int libroId, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await db.ProveedoresLibros.AsNoTracking()
                .Where(pl => pl.LIB_ID == libroId)
                .OrderBy(pl => pl.PL_PrecioCompra)
                .Select(pl => new ProveedorDeLibroDTO
                {
                    Empresa = pl.PL_Proveedor.PROV_Empresa,
                    Contacto = pl.PL_Proveedor.PER_Proveedor.PER_Nombre,
                    Telefono = pl.PL_Proveedor.PER_Proveedor.PER_Telefono,
                    Email = pl.PL_Proveedor.PER_Proveedor.PER_Mail,
                    PrecioCompra = pl.PL_PrecioCompra,
                })
                .ToListAsync(ct);
        }

        /// <summary>Historial de movimientos de un libro, del más reciente al más antiguo.</summary>
        public async Task<List<MovimientoStockDTO>> ObtenerMovimientosAsync(int libroId, int maximo = 500, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            var movimientos = await db.MovimientosStock.AsNoTracking()
                .Where(m => m.LIB_ID == libroId)
                .OrderByDescending(m => m.MOV_Fecha).ThenByDescending(m => m.MOV_ID)
                .Take(maximo)
                .Select(m => new MovimientoStockDTO
                {
                    Fecha = m.MOV_Fecha,
                    Tipo = m.MOV_Tipo,
                    Cantidad = m.MOV_Cantidad,
                    StockAnterior = m.MOV_StockAnterior,
                    StockResultante = m.MOV_StockResultante,
                    Motivo = m.MOV_Motivo,
                    Observacion = m.MOV_Observacion,
                    Referencia = m.MOV_Referencia,
                    Usuario = m.MOV_Usuario,
                })
                .ToListAsync(ct);

            // La traducción del código a texto se hace en memoria (no es traducible a SQL).
            movimientos.ForEach(m => m.Tipo = TipoMovimientoStock.Descripcion(m.Tipo));
            return movimientos;
        }

        #endregion

        #region Operaciones

        /// <summary>
        /// Ajuste manual de stock (rotura, deterioro, devolución, conteo físico...). Exige motivo,
        /// valida que un egreso no deje stock negativo y registra el movimiento en el historial.
        /// </summary>
        public async Task<ResultadoOperacion> AjustarStockAsync(AjusteStockSolicitud s, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(s.Motivo))
                return ResultadoOperacion.Error("Indique el motivo del ajuste.");
            if (s.Tipo == TipoAjusteStock.ConteoFisico ? s.Cantidad < 0 : s.Cantidad <= 0)
                return ResultadoOperacion.Error(s.Tipo == TipoAjusteStock.ConteoFisico
                    ? "El stock contado no puede ser negativo."
                    : "La cantidad debe ser mayor a cero.");

            await using var db = new Libreria();
            await using var tx = await db.Database.BeginTransactionAsync(ct);
            try
            {
                if (!await db.Libros.AnyAsync(l => l.LIB_ID == s.LibroId, ct))
                    return await FallarAsync(tx, "El libro ya no existe.");

                MovimientoStock? mov = s.Tipo switch
                {
                    TipoAjusteStock.Ingreso => await RegistroStock.AplicarAsync(db, s.LibroId, s.Cantidad,
                        TipoMovimientoStock.AjusteIngreso, s.Usuario, s.Motivo, s.Observacion, ct: ct),
                    TipoAjusteStock.Egreso => await RegistroStock.AplicarAsync(db, s.LibroId, -s.Cantidad,
                        TipoMovimientoStock.AjusteEgreso, s.Usuario, s.Motivo, s.Observacion, ct: ct),
                    _ => await RegistroStock.FijarAsync(db, s.LibroId, s.Cantidad, s.Usuario, s.Motivo, s.Observacion, ct),
                };

                if (mov == null)
                    return await FallarAsync(tx, s.Tipo == TipoAjusteStock.Egreso
                        ? "No hay stock suficiente para ese egreso."
                        : "El stock cambió mientras se hacía el ajuste (por ejemplo, por una venta). Revise y vuelva a intentar.");

                db.MovimientosStock.Add(mov);
                await db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                return ResultadoOperacion.Ok(s.LibroId, $"Stock actualizado: {mov.MOV_StockAnterior} → {mov.MOV_StockResultante}.");
            }
            catch
            {
                await RollbackSeguroAsync(tx);
                throw;
            }
        }

        /// <summary>Actualiza mínimo, punto de reposición y óptimo, validando que sean coherentes.</summary>
        public async Task<ResultadoOperacion> ActualizarParametrosAsync(ParametrosStockSolicitud s, CancellationToken ct = default)
        {
            if (s.StockMinimo < 0 || s.PuntoReposicion < s.StockMinimo || s.StockOptimo < s.PuntoReposicion || s.StockOptimo <= 0)
                return ResultadoOperacion.Error("Los valores deben cumplir: 0 ≤ mínimo ≤ punto de reposición ≤ óptimo, y el óptimo debe ser mayor a 0.");

            await using var db = new Libreria();
            int filas = await db.Libros
                .Where(l => l.LIB_ID == s.LibroId)
                .ExecuteUpdateAsync(u => u
                    .SetProperty(l => l.LIB_StockMinimo, s.StockMinimo)
                    .SetProperty(l => l.LIB_PuntoReposicion, s.PuntoReposicion)
                    .SetProperty(l => l.LIB_StockOptimo, s.StockOptimo), ct);

            return filas == 0 ? ResultadoOperacion.Error("El libro ya no existe.") : ResultadoOperacion.Ok(s.LibroId);
        }

        #endregion

        internal static async Task<ResultadoOperacion> FallarAsync(IDbContextTransaction tx, string mensaje)
        {
            await RollbackSeguroAsync(tx);
            return ResultadoOperacion.Error(mensaje);
        }

        /// <summary>Rollback que nunca tapa la excepción original.</summary>
        internal static async Task RollbackSeguroAsync(IDbContextTransaction tx)
        {
            try { await tx.RollbackAsync(CancellationToken.None); }
            catch { /* la transacción se descarta igual al hacer Dispose */ }
        }
    }
}
