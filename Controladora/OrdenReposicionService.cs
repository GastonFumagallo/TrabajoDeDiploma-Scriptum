using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using Servicios;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora
{
    /// <summary>
    /// Flujo completo de las órdenes de reposición:
    /// Pendiente (borrador editable) → Solicitada (enviada al proveedor) → Recibida (impacta stock) | Cancelada.
    ///
    /// Los cambios de estado usan UPDATE condicionales sobre el estado actual: si dos usuarios reciben o cancelan
    /// la misma orden a la vez, sólo uno gana y el stock nunca se suma dos veces.
    /// </summary>
    public class OrdenReposicionService
    {
        private static OrdenReposicionService? instancia;
        public static OrdenReposicionService Instancia => instancia ??= new OrdenReposicionService();
        private OrdenReposicionService() { }

        private static readonly string[] EstadosActivos = { EstadoOrden.Pendiente, EstadoOrden.Solicitada };

        #region Consultas

        public async Task<List<OrdenReposicionDTO>> ObtenerOrdenesAsync(FiltroOrdenes filtro, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            var query = db.OrdenesReposicion.AsNoTracking();

            if (filtro.Estado == "ACTIVAS")
                query = query.Where(o => o.OR_Estado == EstadoOrden.Pendiente || o.OR_Estado == EstadoOrden.Solicitada);
            else if (!string.IsNullOrEmpty(filtro.Estado))
                query = query.Where(o => o.OR_Estado == filtro.Estado);

            if (filtro.ProveedorId is int proveedorId)
                query = query.Where(o => o.OR_PROV_ID == proveedorId);

            return await query
                .OrderByDescending(o => o.OR_Fecha)
                .Select(o => new OrdenReposicionDTO
                {
                    OrdenId = o.OR_ID,
                    Fecha = o.OR_Fecha,
                    Proveedor = o.OR_Proveedor.PROV_Empresa != "" ? o.OR_Proveedor.PROV_Empresa : o.OR_Proveedor.PER_Proveedor.PER_Nombre,
                    EstadoCodigo = o.OR_Estado,
                    Items = o.OR_Detalles.Count(),
                    Unidades = o.OR_Detalles.Sum(d => (int?)d.DOR_CantidadPedida) ?? 0,
                    TotalEstimado = o.OR_Detalles.Sum(d => (decimal?)(d.DOR_CantidadPedida * d.DOR_CostoUnitario)) ?? 0m,
                    Usuario = o.OR_Usuario,
                    FechaRecepcion = o.OR_FechaRecepcion,
                })
                .ToListAsync(ct);
        }

        public async Task<OrdenReposicionDetalleDTO?> ObtenerOrdenAsync(int ordenId, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await db.OrdenesReposicion.AsNoTracking()
                .Where(o => o.OR_ID == ordenId)
                .Select(o => new OrdenReposicionDetalleDTO
                {
                    OrdenId = o.OR_ID,
                    Fecha = o.OR_Fecha,
                    ProveedorId = o.OR_PROV_ID,
                    EstadoCodigo = o.OR_Estado,
                    Usuario = o.OR_Usuario,
                    Observaciones = o.OR_Observaciones,
                    FechaRecepcion = o.OR_FechaRecepcion,
                    UsuarioRecepcion = o.OR_UsuarioRecepcion,
                    MotivoCancelacion = o.OR_MotivoCancelacion,
                    Items = o.OR_Detalles.Select(d => new ItemOrdenDTO
                    {
                        DetalleId = d.DOR_ID,
                        LibroId = d.LIB_ID,
                        Codigo = d.DOR_Libro.LIB_ISBN,
                        Titulo = d.DOR_Libro.LIB_Titulo,
                        StockActual = d.DOR_Libro.LIB_Stock,
                        StockOptimo = d.DOR_Libro.LIB_StockOptimo,
                        CantidadPedida = d.DOR_CantidadPedida,
                        CostoUnitario = d.DOR_CostoUnitario,
                        CantidadRecibida = d.DOR_CantidadRecibida,
                        CostoRecibido = d.DOR_CostoRecibido,
                    }).ToList(),
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<List<ProveedorContactoDTO>> ObtenerProveedoresAsync(CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await db.Proveedores.AsNoTracking()
                .OrderBy(p => p.PROV_Empresa).ThenBy(p => p.PER_Proveedor.PER_Nombre)
                .Select(p => new ProveedorContactoDTO
                {
                    ProveedorId = p.PROV_ID,
                    Empresa = p.PROV_Empresa,
                    Contacto = p.PER_Proveedor.PER_Nombre,
                    Telefono = p.PER_Proveedor.PER_Telefono,
                    Email = p.PER_Proveedor.PER_Mail,
                })
                .ToListAsync(ct);
        }

        /// <summary>
        /// Arma ítems sugeridos para una orden: cantidad para llegar al stock óptimo (descontando lo ya pedido
        /// en otras órdenes activas, mínimo 1) y costo = precio pactado con el proveedor, o el último costo.
        /// </summary>
        public async Task<List<ItemOrdenDTO>> SugerirItemsAsync(IEnumerable<int> libroIds, int? proveedorId, int? excluirOrdenId = null,
            CancellationToken ct = default)
        {
            var ids = libroIds.Distinct().ToList();
            if (ids.Count == 0) return new();

            await using var db = new Libreria();
            return await db.Libros.AsNoTracking()
                .Where(l => ids.Contains(l.LIB_ID))
                .OrderBy(l => l.LIB_Titulo)
                .Select(l => new
                {
                    l.LIB_ID,
                    l.LIB_ISBN,
                    l.LIB_Titulo,
                    l.LIB_Stock,
                    l.LIB_StockOptimo,
                    l.LIB_PrecioCosto,
                    PrecioProveedor = l.LIB_Proveedores.Where(pl => pl.PROV_ID == proveedorId).Select(pl => (decimal?)pl.PL_PrecioCompra).FirstOrDefault(),
                    EnPedido = db.DetallesOrdenReposicion
                        .Where(d => d.LIB_ID == l.LIB_ID && d.OR_ID != excluirOrdenId
                                 && (d.DOR_Orden.OR_Estado == EstadoOrden.Pendiente || d.DOR_Orden.OR_Estado == EstadoOrden.Solicitada))
                        .Sum(d => (int?)d.DOR_CantidadPedida) ?? 0,
                })
                .Select(x => new ItemOrdenDTO
                {
                    LibroId = x.LIB_ID,
                    Codigo = x.LIB_ISBN,
                    Titulo = x.LIB_Titulo,
                    StockActual = x.LIB_Stock,
                    StockOptimo = x.LIB_StockOptimo,
                    CantidadPedida = x.LIB_StockOptimo - x.LIB_Stock - x.EnPedido > 1 ? x.LIB_StockOptimo - x.LIB_Stock - x.EnPedido : 1,
                    CostoUnitario = x.PrecioProveedor ?? x.LIB_PrecioCosto,
                })
                .ToListAsync(ct);
        }

        /// <summary>Precio pactado de cada libro con un proveedor (para recalcular costos al cambiar de proveedor).</summary>
        public async Task<Dictionary<int, decimal>> ObtenerPreciosProveedorAsync(int proveedorId, IEnumerable<int> libroIds, CancellationToken ct = default)
        {
            var ids = libroIds.Distinct().ToList();
            await using var db = new Libreria();
            return await db.ProveedoresLibros.AsNoTracking()
                .Where(pl => pl.PROV_ID == proveedorId && ids.Contains(pl.LIB_ID))
                .ToDictionaryAsync(pl => pl.LIB_ID, pl => pl.PL_PrecioCompra, ct);
        }

        #endregion

        #region Crear / editar / generar

        /// <summary>Crea una orden nueva o actualiza una existente (sólo si sigue en Pendiente). Reemplaza todos sus ítems.</summary>
        public async Task<ResultadoOperacion> GuardarAsync(GuardarOrdenSolicitud s, CancellationToken ct = default)
        {
            string? error = Validar(s);
            if (error != null) return ResultadoOperacion.Error(error);

            // Si el mismo libro vino dos veces, se suman las cantidades (y se toma el último costo).
            var items = s.Items
                .GroupBy(i => i.LibroId)
                .Select(g => (LibroId: g.Key, Cantidad: g.Sum(i => i.Cantidad), Costo: g.Last().CostoUnitario))
                .ToList();

            await using var db = new Libreria();
            await using var tx = await db.Database.BeginTransactionAsync(ct);
            try
            {
                if (!await db.Proveedores.AnyAsync(p => p.PROV_ID == s.ProveedorId, ct))
                    return await InventarioService.FallarAsync(tx, "El proveedor seleccionado ya no existe.");

                var ids = items.Select(i => i.LibroId).ToList();
                if (await db.Libros.CountAsync(l => ids.Contains(l.LIB_ID), ct) != ids.Count)
                    return await InventarioService.FallarAsync(tx, "Uno o más libros de la orden ya no existen.");

                OrdenReposicion orden;
                if (s.OrdenId is int ordenId)
                {
                    // UPDLOCK: nadie puede emitir/recibir/cancelar esta orden mientras se reescribe.
                    var existente = await db.OrdenesReposicion
                        .FromSqlInterpolated($"SELECT * FROM OrdenesReposicion WITH (UPDLOCK, ROWLOCK) WHERE OR_ID = {ordenId}")
                        .Include(o => o.OR_Detalles)
                        .FirstOrDefaultAsync(ct);

                    if (existente == null)
                        return await InventarioService.FallarAsync(tx, "La orden ya no existe.");
                    orden = existente;
                    if (orden.OR_Estado != EstadoOrden.Pendiente)
                        return await InventarioService.FallarAsync(tx,
                            $"La orden está {EstadoOrden.Descripcion(orden.OR_Estado).ToLower()} y ya no se puede modificar.");

                    db.DetallesOrdenReposicion.RemoveRange(orden.OR_Detalles);
                    orden.OR_Detalles.Clear();
                }
                else
                {
                    orden = new OrdenReposicion
                    {
                        OR_Fecha = DateTime.Now,
                        OR_Estado = EstadoOrden.Pendiente,
                        OR_Usuario = s.Usuario,
                    };
                    db.OrdenesReposicion.Add(orden);
                }

                orden.OR_PROV_ID = s.ProveedorId;
                orden.OR_Observaciones = string.IsNullOrWhiteSpace(s.Observaciones) ? null : s.Observaciones.Trim();
                foreach (var (libroId, cantidad, costo) in items)
                {
                    orden.OR_Detalles.Add(new DetalleOrdenReposicion
                    {
                        LIB_ID = libroId,
                        DOR_CantidadPedida = cantidad,
                        DOR_CostoUnitario = Math.Round(costo, 2),
                    });
                }

                await db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                return ResultadoOperacion.Ok(orden.OR_ID);
            }
            catch
            {
                await InventarioService.RollbackSeguroAsync(tx);
                throw;
            }
        }

        private static string? Validar(GuardarOrdenSolicitud s)
        {
            if (s.ProveedorId <= 0) return "Seleccione un proveedor.";
            if (s.Items.Count == 0) return "La orden no tiene ítems.";
            if (s.Items.Any(i => i.Cantidad <= 0)) return "Todas las cantidades deben ser mayores a cero.";
            if (s.Items.Any(i => i.CostoUnitario < 0)) return "Los costos no pueden ser negativos.";
            if (s.Observaciones?.Length > 500) return "Las observaciones no pueden superar los 500 caracteres.";
            return null;
        }

        /// <summary>
        /// Genera borradores automáticos para los libros indicados: una orden por proveedor habitual
        /// (el de menor precio pactado). Los libros sin proveedor asociado se informan y no se incluyen.
        /// </summary>
        public async Task<(List<int> OrdenesCreadas, List<string> SinProveedor)> GenerarBorradoresAsync(
            IEnumerable<int> libroIds, string usuario, CancellationToken ct = default)
        {
            var ids = libroIds.Distinct().ToList();
            List<(int LibroId, string Titulo, int? ProveedorId)> libros;

            await using (var db = new Libreria())
            {
                libros = (await db.Libros.AsNoTracking()
                    .Where(l => ids.Contains(l.LIB_ID))
                    .Select(l => new
                    {
                        l.LIB_ID,
                        l.LIB_Titulo,
                        ProveedorId = l.LIB_Proveedores.OrderBy(pl => pl.PL_PrecioCompra).Select(pl => (int?)pl.PROV_ID).FirstOrDefault(),
                    })
                    .ToListAsync(ct))
                    .Select(x => (x.LIB_ID, x.LIB_Titulo, x.ProveedorId))
                    .ToList();
            }

            var creadas = new List<int>();
            foreach (var grupo in libros.Where(l => l.ProveedorId != null).GroupBy(l => l.ProveedorId!.Value))
            {
                var items = await SugerirItemsAsync(grupo.Select(l => l.LibroId), grupo.Key, ct: ct);
                var resultado = await GuardarAsync(new GuardarOrdenSolicitud
                {
                    ProveedorId = grupo.Key,
                    Usuario = usuario,
                    Observaciones = "Generada automáticamente desde Inventario.",
                    Items = items.Select(i => (i.LibroId, i.CantidadPedida, i.CostoUnitario)).ToList(),
                }, ct);
                if (resultado.Exito) creadas.Add(resultado.Id);
            }

            var sinProveedor = libros.Where(l => l.ProveedorId == null).Select(l => l.Titulo).ToList();
            return (creadas, sinProveedor);
        }

        #endregion

        #region Cambios de estado

        /// <summary>
        /// Emite la orden: opcionalmente envía el email al proveedor y la pasa a Solicitada.
        /// Si el email falla, la orden queda Pendiente y se devuelve el motivo (la UI puede marcarla igual).
        /// </summary>
        public async Task<ResultadoOperacion> EmitirAsync(int ordenId, bool enviarEmail, CancellationToken ct = default)
        {
            if (enviarEmail)
            {
                SolicitudReposicionEmail datos;
                await using (var db = new Libreria())
                {
                    var orden = await db.OrdenesReposicion.AsNoTracking()
                        .Where(o => o.OR_ID == ordenId && o.OR_Estado == EstadoOrden.Pendiente)
                        .Select(o => new
                        {
                            Email = o.OR_Proveedor.PER_Proveedor.PER_Mail,
                            Nombre = o.OR_Proveedor.PROV_Empresa != "" ? o.OR_Proveedor.PROV_Empresa : o.OR_Proveedor.PER_Proveedor.PER_Nombre,
                            o.OR_Observaciones,
                            Items = o.OR_Detalles.Select(d => new { d.DOR_Libro.LIB_Titulo, d.DOR_Libro.LIB_ISBN, d.DOR_CantidadPedida }).ToList(),
                        })
                        .FirstOrDefaultAsync(ct);
                    if (orden == null)
                        return ResultadoOperacion.Error("La orden no existe o ya no está pendiente.");

                    datos = new SolicitudReposicionEmail
                    {
                        NumeroOrden = OrdenReposicion.FormatearNumero(ordenId),
                        EmailProveedor = orden.Email,
                        NombreProveedor = orden.Nombre,
                        Observaciones = orden.OR_Observaciones,
                        Items = orden.Items.Select(i => (i.LIB_Titulo, i.LIB_ISBN, i.DOR_CantidadPedida)).ToList(),
                    };
                }

                // El email va fuera de cualquier transacción: no se puede "deshacer" un correo enviado.
                string? errorEmail = await ServiciosOrdenReposicion.EnviarSolicitudReposicionAsync(datos, ct);
                if (errorEmail != null)
                    return ResultadoOperacion.Error(errorEmail);
            }

            await using var ctx = new Libreria();
            int filas = await ctx.OrdenesReposicion
                .Where(o => o.OR_ID == ordenId && o.OR_Estado == EstadoOrden.Pendiente)
                .ExecuteUpdateAsync(u => u
                    .SetProperty(o => o.OR_Estado, EstadoOrden.Solicitada)
                    .SetProperty(o => o.OR_FechaSolicitud, DateTime.Now), ct);

            return filas == 0
                ? ResultadoOperacion.Error("La orden no existe o ya no está pendiente.")
                : ResultadoOperacion.Ok(ordenId, enviarEmail ? "Orden emitida y enviada al proveedor." : "Orden marcada como solicitada.");
        }

        /// <summary>
        /// Registra la recepción de mercadería en una única transacción atómica:
        /// 1) pasa la orden a Recibida (sólo si estaba activa: evita doble recepción),
        /// 2) guarda cantidad y costo real recibidos por ítem,
        /// 3) suma stock con su movimiento de historial,
        /// 4) opcionalmente actualiza el último costo del libro y el precio pactado con el proveedor.
        /// La recepción cierra la orden aunque sea parcial (lo no recibido queda registrado como diferencia).
        /// </summary>
        public async Task<ResultadoOperacion> RecibirAsync(RecepcionOrdenSolicitud s, CancellationToken ct = default)
        {
            if (s.Lineas.Any(l => l.CantidadRecibida < 0 || l.CostoRecibido < 0))
                return ResultadoOperacion.Error("Las cantidades y costos recibidos no pueden ser negativos.");
            if (s.Lineas.Sum(l => l.CantidadRecibida) <= 0)
                return ResultadoOperacion.Error("No se recibió ninguna unidad. Si la mercadería no va a llegar, cancele la orden.");

            await using var db = new Libreria();
            await using var tx = await db.Database.BeginTransactionAsync(ct);
            try
            {
                var ahora = DateTime.Now;
                int filas = await db.OrdenesReposicion
                    .Where(o => o.OR_ID == s.OrdenId && (o.OR_Estado == EstadoOrden.Pendiente || o.OR_Estado == EstadoOrden.Solicitada))
                    .ExecuteUpdateAsync(u => u
                        .SetProperty(o => o.OR_Estado, EstadoOrden.Recibida)
                        .SetProperty(o => o.OR_FechaRecepcion, ahora)
                        .SetProperty(o => o.OR_UsuarioRecepcion, s.Usuario), ct);
                if (filas == 0)
                    return await InventarioService.FallarAsync(tx, "La orden ya fue recibida o cancelada por otro usuario.");

                var orden = await db.OrdenesReposicion
                    .Include(o => o.OR_Detalles).ThenInclude(d => d.DOR_Libro)
                    .FirstAsync(o => o.OR_ID == s.OrdenId, ct);
                string referencia = OrdenReposicion.FormatearNumero(orden.OR_ID);
                var lineas = s.Lineas.ToDictionary(l => l.DetalleId);

                foreach (var detalle in orden.OR_Detalles)
                {
                    // Una línea que la UI no mandó se considera no recibida.
                    var (_, cantidad, costo) = lineas.TryGetValue(detalle.DOR_ID, out var l) ? l : (detalle.DOR_ID, 0, detalle.DOR_CostoUnitario);
                    detalle.DOR_CantidadRecibida = cantidad;
                    detalle.DOR_CostoRecibido = Math.Round(costo, 2);

                    if (cantidad <= 0) continue;

                    var mov = await RegistroStock.AplicarAsync(db, detalle.LIB_ID, cantidad, TipoMovimientoStock.RecepcionOrden,
                        s.Usuario, motivo: "Recepción de mercadería",
                        observacion: cantidad != detalle.DOR_CantidadPedida ? $"Pedido {detalle.DOR_CantidadPedida}, recibido {cantidad}" : null,
                        referencia: referencia, ct: ct);
                    if (mov != null) db.MovimientosStock.Add(mov);

                    if (s.ActualizarCostos)
                        await ActualizarCostoAsync(db, detalle.DOR_Libro, orden.OR_PROV_ID, Math.Round(costo, 2), ct);
                }

                await db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                int recibidas = orden.OR_Detalles.Sum(d => d.DOR_CantidadRecibida ?? 0);
                int pedidas = orden.OR_Detalles.Sum(d => d.DOR_CantidadPedida);
                return ResultadoOperacion.Ok(orden.OR_ID, recibidas == pedidas
                    ? $"Recepción completa: {recibidas} unidades ingresadas al stock."
                    : $"Recepción parcial: {recibidas} de {pedidas} unidades ingresadas al stock.");
            }
            catch
            {
                await InventarioService.RollbackSeguroAsync(tx);
                throw;
            }
        }

        /// <summary>Actualiza el último costo del libro y el precio pactado con el proveedor (o crea la asociación).</summary>
        private static async Task ActualizarCostoAsync(Libreria db, Libro libro, int proveedorId, decimal costo, CancellationToken ct)
        {
            if (libro.LIB_PrecioCosto != costo)
                libro.LIB_PrecioCosto = costo;

            var asociacion = await db.ProveedoresLibros.FirstOrDefaultAsync(pl => pl.PROV_ID == proveedorId && pl.LIB_ID == libro.LIB_ID, ct);
            if (asociacion == null)
                db.ProveedoresLibros.Add(new ProveedorLibro { PROV_ID = proveedorId, LIB_ID = libro.LIB_ID, PL_PrecioCompra = costo });
            else if (asociacion.PL_PrecioCompra != costo)
                asociacion.PL_PrecioCompra = costo;
        }

        public async Task<ResultadoOperacion> CancelarAsync(int ordenId, string motivo, string usuario, CancellationToken ct = default)
        {
            motivo = motivo?.Trim() ?? string.Empty;
            if (motivo.Length < 5)
                return ResultadoOperacion.Error("Indique el motivo de la cancelación (al menos 5 caracteres).");

            await using var db = new Libreria();
            int filas = await db.OrdenesReposicion
                .Where(o => o.OR_ID == ordenId && (o.OR_Estado == EstadoOrden.Pendiente || o.OR_Estado == EstadoOrden.Solicitada))
                .ExecuteUpdateAsync(u => u
                    .SetProperty(o => o.OR_Estado, EstadoOrden.Cancelada)
                    .SetProperty(o => o.OR_FechaCancelacion, DateTime.Now)
                    .SetProperty(o => o.OR_MotivoCancelacion, motivo.Length > 250 ? motivo.Substring(0, 250) : motivo)
                    .SetProperty(o => o.OR_UsuarioCancelacion, usuario), ct);

            return filas == 0
                ? ResultadoOperacion.Error("La orden ya fue recibida o cancelada.")
                : ResultadoOperacion.Ok(ordenId, "Orden cancelada.");
        }

        #endregion
    }
}
