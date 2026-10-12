using Controladora.MetodoPagoStrategy;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using Modelo.Seguridad;
using Servicios;

namespace Controladora
{
    public interface ICajaService
    {
        /// <summary>La caja abierta, o null si no hay ninguna.</summary>
        Task<CajaActivaDTO?> ObtenerCajaActivaAsync(CancellationToken ct = default);

        /// <summary>Abre una caja con el fondo inicial indicado. Devuelve su Id.</summary>
        Task<int> AbrirCajaAsync(decimal montoInicial, CancellationToken ct = default);

        /// <summary>Vista previa del arqueo (lo que el sistema dice que debería haber), sin cerrar.</summary>
        Task<ArqueoCajaDTO> ObtenerArqueoAsync(int cajaId, CancellationToken ct = default);

        /// <summary>Cierra la caja con el efectivo contado. Devuelve el arqueo definitivo con la diferencia.</summary>
        Task<ArqueoCajaDTO> CerrarCajaAsync(int cajaId, decimal efectivoContado, string? observaciones, CancellationToken ct = default);
    }

    /// <summary>
    /// Control de caja diaria: apertura con fondo inicial, ventas asociadas a la caja abierta y cierre con arqueo.
    /// <list type="bullet">
    /// <item>Nunca hay más de una caja abierta: lo valida este servicio y lo garantiza el índice único filtrado
    /// <c>UX_Cajas_UnaAbierta</c> (dos puestos abriendo a la vez no pueden ganar los dos).</item>
    /// <item>No se vende sin caja abierta: <see cref="FacadeVentas"/> bloquea la fila de la caja dentro de la misma
    /// transacción de la venta (<see cref="BloquearCajaAbiertaAsync"/>) y el cierre bloquea la misma fila antes de
    /// totalizar. Así ninguna venta queda afuera del arqueo ni entra en una caja ya cerrada.</item>
    /// <item>El arqueo cuenta las ventas no anuladas; efectivo esperado = monto inicial + ventas en efectivo.</item>
    /// <item>Permisos exigidos en esta capa: <c>AbrirCaja</c> y <c>CerrarCaja</c>.</item>
    /// </list>
    /// </summary>
    public sealed class CajaService : ICajaService
    {
        public const decimal MontoMaximo = 9_999_999_999.99m;   // decimal(18,2) holgado y a salvo de typos
        private const int LargoObservaciones = 500;

        private static CajaService? instancia;
        public static CajaService Instancia => instancia ??= new CajaService();

        public async Task<CajaActivaDTO?> ObtenerCajaActivaAsync(CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await db.Cajas.AsNoTracking()
                .Where(c => c.CAJ_Estado == EstadoCaja.Abierta)
                .Select(c => new CajaActivaDTO(c.CAJ_ID, c.CAJ_FechaApertura, c.CAJ_UsuarioApertura.USU_Nombre, c.CAJ_MontoInicial))
                .FirstOrDefaultAsync(ct);
        }

        public async Task<int> AbrirCajaAsync(decimal montoInicial, CancellationToken ct = default)
        {
            PermisoService.Instancia.Exigir("AbrirCaja");
            var usuarioId = UsuarioLogueadoId();

            if (montoInicial < 0)
                throw new ValidacionException("MontoInicial", "El monto inicial no puede ser negativo.");
            if (montoInicial > MontoMaximo)
                throw new ValidacionException("MontoInicial", "El monto inicial es demasiado grande. Revisá el importe.");
            if (decimal.Round(montoInicial, 2) != montoInicial)
                throw new ValidacionException("MontoInicial", "El monto inicial admite hasta dos decimales.");

            await using var db = new Libreria();
            var abierta = await db.Cajas.AsNoTracking()
                .Where(c => c.CAJ_Estado == EstadoCaja.Abierta)
                .Select(c => new { c.CAJ_ID, c.CAJ_UsuarioApertura.USU_Nombre })
                .FirstOrDefaultAsync(ct);
            if (abierta != null)
                throw new ReglaNegocioException($"Ya hay una caja abierta (caja #{abierta.CAJ_ID}, de {abierta.USU_Nombre}). Cerrala antes de abrir otra.");

            var caja = new Caja
            {
                CAJ_Estado = EstadoCaja.Abierta,
                CAJ_FechaApertura = DateTime.Now,
                CAJ_UsuarioAperturaId = usuarioId,
                CAJ_MontoInicial = montoInicial,
            };
            db.Cajas.Add(caja);
            try
            {
                await db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
            {
                // Otro puesto abrió una caja entre la verificación y el guardado: el índice único lo frenó.
                throw new ReglaNegocioException("Otro usuario acaba de abrir una caja. Actualizá la pantalla para verla.");
            }
            return caja.CAJ_ID;
        }

        public async Task<ArqueoCajaDTO> ObtenerArqueoAsync(int cajaId, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            var caja = await db.Cajas.AsNoTracking()
                .Where(c => c.CAJ_ID == cajaId)
                .Select(c => new { c.CAJ_ID, c.CAJ_FechaApertura, Usuario = c.CAJ_UsuarioApertura.USU_Nombre, c.CAJ_MontoInicial })
                .FirstOrDefaultAsync(ct)
                ?? throw new ReglaNegocioException("La caja ya no existe.");

            var (totales, cantidad) = await TotalizarAsync(db, cajaId, ct);
            return new ArqueoCajaDTO
            {
                CajaId = caja.CAJ_ID,
                FechaApertura = caja.CAJ_FechaApertura,
                UsuarioApertura = caja.Usuario,
                MontoInicial = caja.CAJ_MontoInicial,
                TotalesPorMedio = totales,
                CantidadVentas = cantidad,
                TotalVentas = totales.Sum(t => t.Total),
            };
        }

        public async Task<ArqueoCajaDTO> CerrarCajaAsync(int cajaId, decimal efectivoContado, string? observaciones, CancellationToken ct = default)
        {
            PermisoService.Instancia.Exigir("CerrarCaja");
            var usuarioId = UsuarioLogueadoId();
            observaciones = string.IsNullOrWhiteSpace(observaciones) ? null : observaciones.Trim();

            if (efectivoContado < 0)
                throw new ValidacionException("EfectivoContado", "El efectivo contado no puede ser negativo.");
            if (efectivoContado > MontoMaximo)
                throw new ValidacionException("EfectivoContado", "El efectivo contado es demasiado grande. Revisá el importe.");
            if (decimal.Round(efectivoContado, 2) != efectivoContado)
                throw new ValidacionException("EfectivoContado", "El efectivo contado admite hasta dos decimales.");
            if (observaciones?.Length > LargoObservaciones)
                throw new ValidacionException("Observaciones", $"Las observaciones no pueden superar los {LargoObservaciones} caracteres.");

            await using var db = new Libreria();
            await using var tx = await db.Database.BeginTransactionAsync(ct);

            // Bloquea la fila de la caja: espera a que terminen las ventas en curso (que la bloquean al registrarse)
            // y frena las nuevas hasta que el cierre se confirme. Ninguna venta queda afuera del arqueo.
            var caja = await db.Cajas
                .FromSqlInterpolated($"SELECT * FROM Cajas WITH (UPDLOCK, ROWLOCK) WHERE CAJ_ID = {cajaId}")
                .FirstOrDefaultAsync(ct)
                ?? throw new ReglaNegocioException("La caja ya no existe.");
            if (caja.CAJ_Estado == EstadoCaja.Cerrada)
                throw new ReglaNegocioException($"La caja #{cajaId} ya fue cerrada{(caja.CAJ_FechaCierre is DateTime f ? $" el {f:dd/MM HH:mm}" : "")}.");

            var (totales, cantidad) = await TotalizarAsync(db, cajaId, ct);
            var esperado = caja.CAJ_MontoInicial + totales.Where(t => t.EsEfectivo).Sum(t => t.Total);
            var diferencia = efectivoContado - esperado;

            // Un sobrante o faltante tiene que quedar explicado.
            if (diferencia != 0 && observaciones == null)
                throw new ValidacionException("Observaciones",
                    $"{ArqueoCajaDTO.DescribirDiferencia(diferencia)}: explicá la diferencia en las observaciones.");

            var ahora = DateTime.Now;
            caja.CAJ_Estado = EstadoCaja.Cerrada;
            caja.CAJ_FechaCierre = ahora;
            caja.CAJ_UsuarioCierreId = usuarioId;
            caja.CAJ_CantidadVentas = cantidad;
            caja.CAJ_TotalVentas = totales.Sum(t => t.Total);
            caja.CAJ_EfectivoEsperado = esperado;
            caja.CAJ_EfectivoContado = efectivoContado;
            caja.CAJ_Diferencia = diferencia;
            caja.CAJ_Observaciones = observaciones;

            foreach (var t in totales)
            {
                db.CajaTotalesMedio.Add(new CajaTotalMedio
                {
                    CAJ_ID = cajaId,
                    MP_ID = t.MetodoPagoId,
                    CTM_MedioPago = t.MedioPago,
                    CTM_EsEfectivo = t.EsEfectivo,
                    CTM_Cantidad = t.Cantidad,
                    CTM_Total = t.Total,
                });
            }

            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            var usuarioApertura = await db.Usuarios.AsNoTracking()
                .Where(u => u.USU_ID == caja.CAJ_UsuarioAperturaId).Select(u => u.USU_Nombre).FirstAsync(ct);
            return new ArqueoCajaDTO
            {
                CajaId = cajaId,
                FechaApertura = caja.CAJ_FechaApertura,
                UsuarioApertura = usuarioApertura,
                MontoInicial = caja.CAJ_MontoInicial,
                TotalesPorMedio = totales,
                CantidadVentas = cantidad,
                TotalVentas = totales.Sum(t => t.Total),
                FechaCierre = ahora,
                Contado = efectivoContado,
            };
        }

        /// <summary>
        /// Para <see cref="FacadeVentas"/>: la caja abierta, bloqueada hasta el fin de la transacción de la venta,
        /// o null si no hay ninguna. Se llama dentro de la transacción ya iniciada.
        /// </summary>
        internal static async Task<Caja?> BloquearCajaAbiertaAsync(Libreria db, CancellationToken ct)
        {
            var id = await db.Cajas
                .Where(c => c.CAJ_Estado == EstadoCaja.Abierta)
                .Select(c => (int?)c.CAJ_ID)
                .FirstOrDefaultAsync(ct);
            if (id is null) return null;

            // Se bloquea por clave primaria, el mismo recurso que bloquea el cierre. (Bloquear filtrando por estado
            // puede resolverse sólo con el índice filtrado y no tocar la fila: entonces el cierre no esperaría a esta
            // venta, la dejaría afuera del arqueo y terminaría en deadlock al actualizar el índice.)
            var caja = await db.Cajas
                .FromSqlInterpolated($"SELECT * FROM Cajas WITH (UPDLOCK, ROWLOCK) WHERE CAJ_ID = {id.Value}")
                .FirstOrDefaultAsync(ct);
            return caja?.CAJ_Estado == EstadoCaja.Abierta ? caja : null;   // se cerró entre la búsqueda y el bloqueo
        }

        public const string MensajeSinCajaAbierta =
            "No hay una caja abierta. Abrí la caja desde Ventas antes de registrar ventas.";

        /// <summary>Ventas no anuladas de la caja, agrupadas por medio de pago (un único pago por venta).</summary>
        private static async Task<(List<TotalMedioCajaDTO> Totales, int Cantidad)> TotalizarAsync(Libreria db, int cajaId, CancellationToken ct)
        {
            var filas = await db.PagosVenta.AsNoTracking()
                .Where(p => p.PAG_Venta.CAJ_ID == cajaId && !p.PAG_Venta.VEN_Anulada)
                .GroupBy(p => new { p.MP_ID, p.PAG_MetodoPago.MP_Nombre })
                .Select(g => new { g.Key.MP_ID, Medio = g.Key.MP_Nombre, Cantidad = g.Select(p => p.VEN_ID).Distinct().Count(), Total = g.Sum(p => p.PAG_Monto) })
                .ToListAsync(ct);

            var cantidad = await db.Ventas.CountAsync(v => v.CAJ_ID == cajaId && !v.VEN_Anulada, ct);
            var totales = filas
                .Select(f => new TotalMedioCajaDTO(f.MP_ID, f.Medio, MetodoPagoStrategyFactory.EsEfectivo(f.Medio), f.Cantidad, f.Total))
                .OrderByDescending(t => t.EsEfectivo).ThenBy(t => t.MedioPago)
                .ToList();
            return (totales, cantidad);
        }

        private static int UsuarioLogueadoId() =>
            Sesion.Instancia.Usuario?.USU_ID
            ?? throw new AccesoDenegadoException("sesión iniciada");
    }
}
