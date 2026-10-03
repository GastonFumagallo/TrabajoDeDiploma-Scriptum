using Controladora;
using Controladora.MetodoPagoStrategy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Modelo.Contexto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modelo
{
    /// <summary>
    /// Operaciones de escritura del módulo de ventas. Cada operación:
    ///  - usa su propio DbContext de vida corta (nunca el contexto global compartido),
    ///  - corre dentro de una transacción explícita, con Commit al final y Rollback ante cualquier error,
    ///  - devuelve <see cref="ResultadoVenta.Error"/> para errores de negocio (mensaje apto para el usuario)
    ///    y deja propagar las excepciones técnicas para que la UI las informe.
    /// </summary>
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

        #region Registrar venta

        /// <summary>
        /// Registra la venta completa en una única transacción:
        /// 1) valida cliente, medio de pago, libros y pago suficiente con los datos actuales de la base;
        /// 2) descuenta stock con un UPDATE condicional (no puede quedar negativo aunque vendan dos cajas a la vez);
        /// 3) inserta cabecera, detalles y pago; 4) Commit. Ante cualquier falla, Rollback y no queda nada a medias.
        /// </summary>
        public async Task<ResultadoVenta> RegistrarVentaAsync(SolicitudVenta solicitud, decimal? totalEsperado = null, CancellationToken ct = default)
        {
            string? error = ValidarSolicitud(solicitud);
            if (error != null)
                return ResultadoVenta.Error(error);

            // Consolidamos por libro por si la UI mandara el mismo ID en dos líneas.
            var cantidades = solicitud.Items
                .GroupBy(i => i.LibroId)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.Cantidad));

            await using var db = new Libreria();
            await using var tx = await db.Database.BeginTransactionAsync(ct);

            try
            {
                // 1. Cliente (o Consumidor Final por defecto).
                var cliente = solicitud.ClientePersonaId is int personaId
                    ? await db.Clientes.FirstOrDefaultAsync(c => c.CLI_Persona.PER_ID == personaId, ct)
                    : await ControladoraClientes.ObtenerOCrearConsumidorFinalAsync(db, ct);
                if (cliente == null)
                    return await FallarAsync(tx, "El cliente seleccionado ya no existe.");

                // 2. Medio de pago activo.
                var metodoPago = await db.MetodosPago.FirstOrDefaultAsync(m => m.MP_ID == solicitud.MetodoPagoId, ct);
                if (metodoPago == null || !metodoPago.MP_Estado)
                    return await FallarAsync(tx, "El medio de pago seleccionado no existe o está inactivo.");

                // 3. Libros con precio actual de la base (el precio de la grilla puede estar desactualizado).
                var ids = cantidades.Keys.ToList();
                var libros = await db.Libros.Where(l => ids.Contains(l.LIB_ID)).ToDictionaryAsync(l => l.LIB_ID, ct);
                if (libros.Count != ids.Count)
                    return await FallarAsync(tx, "Uno o más libros del carrito ya no existen.");
                var inactivo = libros.Values.FirstOrDefault(l => !l.LIB_Activo);
                if (inactivo != null)
                    return await FallarAsync(tx, $"'{inactivo.LIB_Titulo}' está dado de baja y no se puede vender.");

                // 4. Importes con la misma fórmula que ve el cajero.
                decimal subtotal = cantidades.Sum(kv => libros[kv.Key].LIB_PrecioVenta * kv.Value);
                var calculo = CalculadoraVenta.Calcular(subtotal, solicitud.PorcentajeDescuento, metodoPago,
                    solicitud.MontoRecibido, ConfiguracionVentas.TasaIVA);

                if (totalEsperado is decimal esperado && esperado != calculo.Total)
                    return await FallarAsync(tx,
                        $"El total cambió desde que se armó el carrito (antes ${esperado:N2}, ahora ${calculo.Total:N2}) " +
                        "porque se modificaron precios. Revise la venta antes de cobrar.");

                if (calculo.PagoInsuficiente)
                    return await FallarAsync(tx, $"El monto recibido (${calculo.Recibido:N2}) no cubre el total (${calculo.Total:N2}).");

                // 5. Descuento de stock atómico (sólo si todavía alcanza) con su movimiento de historial.
                var movimientos = new List<MovimientoStock>();
                foreach (var (libroId, cantidad) in cantidades)
                {
                    var mov = await RegistroStock.AplicarAsync(db, libroId, -cantidad, TipoMovimientoStock.Venta,
                        solicitud.Usuario, motivo: "Venta", ct: ct);

                    if (mov == null)
                        return await FallarAsync(tx, $"Stock insuficiente para '{libros[libroId].LIB_Titulo}'.");
                    movimientos.Add(mov);
                }

                // 6. Cabecera + detalles + pago (movimiento de caja), en un solo SaveChanges.
                var ahora = DateTime.Now;
                var venta = new Venta
                {
                    VEN_Fecha = ahora,
                    CLI_ID = cliente.CLI_ID,
                    VEN_Cliente = cliente,
                    MP_ID = metodoPago.MP_ID,
                    VEN_MetodoPago = metodoPago,
                    VEN_Subtotal = calculo.Subtotal,
                    VEN_PorcentajeDescuento = calculo.PorcentajeDescuento,
                    VEN_Descuento = calculo.Descuento,
                    VEN_AjusteMedioPago = calculo.AjusteMedioPago,
                    VEN_IVA = calculo.IVA,
                    VEN_Total = calculo.Total,
                    VEN_Usuario = solicitud.Usuario,
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

                venta.VEN_Pagos.Add(new PagoVenta
                {
                    PAG_Venta = venta,
                    MP_ID = metodoPago.MP_ID,
                    PAG_MetodoPago = metodoPago,
                    PAG_Monto = calculo.Total,
                    PAG_Recibido = calculo.Recibido,
                    PAG_Vuelto = calculo.Vuelto,
                    PAG_Fecha = ahora,
                });

                db.Ventas.Add(venta);
                await db.SaveChangesAsync(ct);

                // 7. Historial de stock con el número de comprobante (recién se conoce tras el primer guardado).
                foreach (var mov in movimientos)
                    mov.MOV_Referencia = Venta.FormatearComprobante(venta.VEN_ID);
                db.MovimientosStock.AddRange(movimientos);
                await db.SaveChangesAsync(ct);

                await tx.CommitAsync(ct);

                return ResultadoVenta.Ok(venta.VEN_ID, calculo);
            }
            catch
            {
                // Rollback explícito y re-lanzamos: la UI informa el error técnico al usuario.
                await RollbackSeguroAsync(tx);
                throw;
            }
        }

        private static string? ValidarSolicitud(SolicitudVenta s)
        {
            if (s.Items == null || s.Items.Count == 0)
                return "El carrito está vacío.";
            if (s.Items.Any(i => i.Cantidad <= 0))
                return "Todas las cantidades deben ser mayores a cero.";
            if (s.MetodoPagoId <= 0)
                return "Seleccione un medio de pago.";
            if (s.PorcentajeDescuento < 0 || s.PorcentajeDescuento > ConfiguracionVentas.DescuentoMaximoPorcentaje)
                return $"El descuento debe estar entre 0 % y {ConfiguracionVentas.DescuentoMaximoPorcentaje:0.##} %.";
            return null;
        }

        #endregion

        #region Anular venta

        /// <summary>
        /// Anula una venta (baja lógica) y devuelve al stock las unidades vendidas, todo en una transacción.
        /// La venta no se borra: queda en el historial con fecha, motivo y usuario de la anulación (auditoría).
        /// </summary>
        public async Task<ResultadoVenta> AnularVentaAsync(int ventaId, string motivo, string usuario, CancellationToken ct = default)
        {
            motivo = motivo?.Trim() ?? string.Empty;
            if (motivo.Length < 5)
                return ResultadoVenta.Error("Indique un motivo de anulación (al menos 5 caracteres).");
            if (motivo.Length > 250)
                motivo = motivo[..250];

            await using var db = new Libreria();
            await using var tx = await db.Database.BeginTransactionAsync(ct);

            try
            {
                // UPDATE condicional: si dos usuarios anulan la misma venta a la vez, sólo uno afecta la fila,
                // y por lo tanto el stock se repone una única vez.
                int filas = await db.Ventas
                    .Where(v => v.VEN_ID == ventaId && !v.VEN_Anulada)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(v => v.VEN_Anulada, true)
                        .SetProperty(v => v.VEN_FechaAnulacion, DateTime.Now)
                        .SetProperty(v => v.VEN_MotivoAnulacion, motivo)
                        .SetProperty(v => v.VEN_UsuarioAnulacion, usuario), ct);

                if (filas == 0)
                {
                    bool existe = await db.Ventas.AnyAsync(v => v.VEN_ID == ventaId, ct);
                    return await FallarAsync(tx, existe ? "La venta ya estaba anulada." : "La venta no existe.");
                }

                var detalles = await db.DetallesVenta.AsNoTracking()
                    .Where(d => d.VEN_ID == ventaId)
                    .GroupBy(d => d.LIB_ID)
                    .Select(g => new { LibroId = g.Key, Cantidad = g.Sum(d => d.DV_Cantidad) })
                    .ToListAsync(ct);

                foreach (var d in detalles)
                {
                    var mov = await RegistroStock.AplicarAsync(db, d.LibroId, d.Cantidad, TipoMovimientoStock.AnulacionVenta,
                        usuario, motivo: "Anulación de venta", observacion: motivo,
                        referencia: Venta.FormatearComprobante(ventaId), ct: ct);
                    if (mov != null)
                        db.MovimientosStock.Add(mov);
                }

                await db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                return ResultadoVenta.Ok(ventaId);
            }
            catch
            {
                await RollbackSeguroAsync(tx);
                throw;
            }
        }

        #endregion

        #region Helpers transaccionales

        private static async Task<ResultadoVenta> FallarAsync(IDbContextTransaction tx, string mensaje)
        {
            await RollbackSeguroAsync(tx);
            return ResultadoVenta.Error(mensaje);
        }

        /// <summary>Rollback que nunca tapa la excepción original (si la conexión ya se cayó, el rollback también falla).</summary>
        private static async Task RollbackSeguroAsync(IDbContextTransaction tx)
        {
            try { await tx.RollbackAsync(CancellationToken.None); }
            catch { /* la transacción se descarta igual al hacer Dispose */ }
        }

        #endregion
    }
}
