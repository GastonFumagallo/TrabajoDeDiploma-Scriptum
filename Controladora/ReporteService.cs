using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Controladora
{
    /// <summary>
    /// Lógica analítica del Centro de Reportes. Reglas de rendimiento:
    /// <list type="bullet">
    /// <item>DbContext propio por reporte y todas las consultas con AsNoTracking.</item>
    /// <item>Las agregaciones (GroupBy, Sum, Count, Max) se resuelven en SQL; a memoria sólo llegan filas ya agregadas
    /// (como mucho una por día del período), nunca las ventas o detalles individuales.</item>
    /// <item>Lo único que se hace en C# es agrupar días en semanas/meses y completar períodos sin ventas.</item>
    /// </list>
    /// Las ventas anuladas se excluyen de todos los reportes de ventas.
    /// </summary>
    public sealed class ReporteService
    {
        private static ReporteService? instancia;
        public static ReporteService Instancia => instancia ??= new ReporteService();
        private ReporteService() { }

        private static CultureInfo Cultura => CultureInfo.CurrentCulture;   // la que fija CulturaRegional al iniciar

        public async Task<ResultadoReporte> GenerarAsync(ParametrosReporte p, CancellationToken ct = default)
        {
            if (p.Hasta.Date < p.Desde.Date)
                throw new ValidacionException(nameof(p.Hasta), "La fecha \"hasta\" no puede ser anterior a \"desde\".");

            await using var db = new Libreria();
            return p.Tipo switch
            {
                TipoReporte.VentasPorPeriodo => await VentasPorPeriodoAsync(db, p, ct),
                TipoReporte.VentasPorMedioPago => await VentasPorMedioPagoAsync(db, p, ct),
                TipoReporte.RankingProductos => await RankingProductosAsync(db, p, ct),
                TipoReporte.StockSinMovimiento => await StockSinMovimientoAsync(db, p, ct),
                TipoReporte.ValorizacionStock => await ValorizacionStockAsync(db, p, ct),
                TipoReporte.MejoresClientes => await MejoresClientesAsync(db, p, ct),
                TipoReporte.ComprasPorProveedor => await ComprasPorProveedorAsync(db, p, ct),
                TipoReporte.OrdenesReposicion => await OrdenesReposicionAsync(db, p, ct),
                _ => throw new ArgumentOutOfRangeException(nameof(p.Tipo)),
            };
        }

        #region Consultas base

        /// <summary>Ventas no anuladas del período (y medio de pago, si se filtró). Todavía no ejecuta nada.</summary>
        private static IQueryable<Venta> Ventas(Libreria db, ParametrosReporte p)
        {
            DateTime desde = p.Desde.Date, hastaExclusivo = p.Hasta.Date.AddDays(1);
            var q = db.Ventas.AsNoTracking().Where(v => !v.VEN_Anulada && v.VEN_Fecha >= desde && v.VEN_Fecha < hastaExclusivo);
            if (p.MetodoPagoId is int mp) q = q.Where(v => v.MP_ID == mp);
            return q;
        }

        /// <summary>Detalles de las ventas del período (mismos filtros), opcionalmente de una categoría.</summary>
        private static IQueryable<DetalleVenta> Detalles(Libreria db, ParametrosReporte p)
        {
            DateTime desde = p.Desde.Date, hastaExclusivo = p.Hasta.Date.AddDays(1);
            var q = db.DetallesVenta.AsNoTracking()
                .Where(d => !d.DV_Venta.VEN_Anulada && d.DV_Venta.VEN_Fecha >= desde && d.DV_Venta.VEN_Fecha < hastaExclusivo);
            if (p.MetodoPagoId is int mp) q = q.Where(d => d.DV_Venta.MP_ID == mp);
            if (p.GeneroId is int g) q = q.Where(d => d.DV_Libro.GEN_ID == g);
            return q;
        }

        /// <summary>KPIs de ventas: dos consultas agregadas (cabeceras y detalles), sin materializar ventas.</summary>
        public async Task<ResumenKpisDTO> ObtenerKpisVentasAsync(ParametrosReporte p, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await KpisAsync(db, p, ct);
        }

        private static async Task<ResumenKpisDTO> KpisAsync(Libreria db, ParametrosReporte p, CancellationToken ct)
        {
            var cabecera = await Ventas(db, p)
                .GroupBy(_ => 1)
                .Select(g => new { Cantidad = g.Count(), Total = g.Sum(v => v.VEN_Total) })
                .FirstOrDefaultAsync(ct);

            // Los KPIs de ventas son de toda la venta: la categoría no aplica (una venta puede tener varias).
            var sinCategoria = new ParametrosReporte { Desde = p.Desde, Hasta = p.Hasta, MetodoPagoId = p.MetodoPagoId };
            var detalle = await Detalles(db, sinCategoria)
                .GroupBy(_ => 1)
                .Select(g => new { Unidades = g.Sum(d => d.DV_Cantidad), Cmv = g.Sum(d => d.DV_Cantidad * d.DV_CostoUnitario) })
                .FirstOrDefaultAsync(ct);

            return new ResumenKpisDTO
            {
                CantidadVentas = cabecera?.Cantidad ?? 0,
                TotalFacturado = cabecera?.Total ?? 0,
                Unidades = detalle?.Unidades ?? 0,
                CMV = detalle?.Cmv ?? 0,
            };
        }

        #endregion

        #region 1. Ventas por período

        private static async Task<ResultadoReporte> VentasPorPeriodoAsync(Libreria db, ParametrosReporte p, CancellationToken ct)
        {
            var kpis = await KpisAsync(db, p, ct);

            // Agregados diarios en SQL (CONVERT(date, VEN_Fecha)); a memoria llega una fila por día con ventas.
            var porDia = await Ventas(db, p)
                .GroupBy(v => v.VEN_Fecha.Date)
                .Select(g => new { Dia = g.Key, Cantidad = g.Count(), Total = g.Sum(v => v.VEN_Total) })
                .ToListAsync(ct);

            var detallePorDia = await Detalles(db, new ParametrosReporte { Desde = p.Desde, Hasta = p.Hasta, MetodoPagoId = p.MetodoPagoId })
                .GroupBy(d => d.DV_Venta.VEN_Fecha.Date)
                .Select(g => new { Dia = g.Key, Unidades = g.Sum(d => d.DV_Cantidad), Cmv = g.Sum(d => d.DV_Cantidad * d.DV_CostoUnitario) })
                .ToListAsync(ct);

            var medios = await Ventas(db, p)
                .GroupBy(v => v.VEN_MetodoPago.MP_Nombre)
                .Select(g => new { Medio = g.Key, Total = g.Sum(v => v.VEN_Total) })
                .OrderByDescending(x => x.Total)
                .ToListAsync(ct);

            // Agrupación por semana/mes en memoria (son, como mucho, cientos de filas) y relleno de períodos vacíos
            // para que el gráfico de evolución no "salte" los días sin ventas.
            var ventasDia = porDia.ToDictionary(x => x.Dia);
            var detalleDia = detallePorDia.ToDictionary(x => x.Dia);
            var filas = new List<ReporteVentasDTO>();
            for (var dia = p.Desde.Date; dia <= p.Hasta.Date; dia = dia.AddDays(1))
            {
                var inicio = InicioPeriodo(dia, p.Agrupacion);
                var fila = filas.LastOrDefault();
                if (fila == null || fila.Inicio != inicio)
                {
                    fila = new ReporteVentasDTO { Inicio = inicio, Periodo = EtiquetaPeriodo(inicio, p.Agrupacion) };
                    filas.Add(fila);
                }
                if (ventasDia.TryGetValue(dia, out var v)) { fila.CantidadVentas += v.Cantidad; fila.Facturado += v.Total; }
                if (detalleDia.TryGetValue(dia, out var d)) { fila.Unidades += d.Unidades; fila.CMV += d.Cmv; }
            }

            return new ResultadoReporte
            {
                Titulo = "Ventas y facturación por período",
                Descripcion = Descripcion(p, $"Agrupación {p.Agrupacion.ToString().ToLower()}"),
                Kpis = KpisVentas(kpis),
                Columnas = new()
                {
                    new(nameof(ReporteVentasDTO.Periodo), "Período", FormatoValor.Texto, 120),
                    new(nameof(ReporteVentasDTO.CantidadVentas), "Ventas", FormatoValor.Entero, 60),
                    new(nameof(ReporteVentasDTO.Unidades), "Unidades", FormatoValor.Entero, 60),
                    new(nameof(ReporteVentasDTO.Facturado), "Facturado", FormatoValor.Moneda),
                    new(nameof(ReporteVentasDTO.CMV), "CMV", FormatoValor.Moneda),
                    new(nameof(ReporteVentasDTO.GananciaBruta), "Ganancia bruta", FormatoValor.Moneda),
                    new(nameof(ReporteVentasDTO.Margen), "Margen", FormatoValor.Porcentaje, 60),
                    new(nameof(ReporteVentasDTO.TicketPromedio), "Ticket promedio", FormatoValor.Moneda),
                },
                Filas = filas,
                TipoFila = typeof(ReporteVentasDTO),
                Graficos = new()
                {
                    new GraficoReporte
                    {
                        Tipo = TipoGrafico.Lineas,
                        Titulo = "Evolución de ventas",
                        Etiquetas = filas.Select(f => f.Periodo).ToArray(),
                        Series = new()
                        {
                            new("Facturado", filas.Select(f => (double)f.Facturado).ToArray()),
                            new("Ganancia bruta", filas.Select(f => (double)f.GananciaBruta).ToArray()),
                        },
                    },
                    new GraficoReporte
                    {
                        Tipo = TipoGrafico.Torta,
                        Titulo = "Facturación por medio de pago",
                        Etiquetas = medios.Select(m => m.Medio).ToArray(),
                        Series = new() { new("Facturado", medios.Select(m => (double)m.Total).ToArray()) },
                    },
                    new GraficoReporte
                    {
                        Tipo = TipoGrafico.Barras,
                        Titulo = "Cantidad de ventas",
                        Etiquetas = filas.Select(f => f.Periodo).ToArray(),
                        Series = new() { new("Ventas", filas.Select(f => (double)f.CantidadVentas).ToArray()) },
                        FormatoValores = FormatoValor.Entero,
                    },
                },
            };
        }

        private static DateTime InicioPeriodo(DateTime dia, Agrupacion agrupacion) => agrupacion switch
        {
            Agrupacion.Semanal => dia.AddDays(-(((int)dia.DayOfWeek + 6) % 7)),   // lunes de esa semana
            Agrupacion.Mensual => new DateTime(dia.Year, dia.Month, 1),
            _ => dia,
        };

        private static string EtiquetaPeriodo(DateTime inicio, Agrupacion agrupacion) => agrupacion switch
        {
            Agrupacion.Semanal => $"Sem. {inicio:dd/MM}",
            Agrupacion.Mensual => Cultura.TextInfo.ToTitleCase(inicio.ToString("MMM yyyy", Cultura)),
            _ => inicio.ToString("dd/MM"),
        };

        private static List<KpiDTO> KpisVentas(ResumenKpisDTO k) => new()
        {
            new("Recaudación total", k.TotalFacturado, FormatoValor.Moneda, $"CMV {k.CMV.ToString("C0", Cultura)}"),
            new("Ganancia bruta", k.GananciaBruta, FormatoValor.Moneda, $"Margen {k.Margen.ToString("P1", Cultura)}"),
            new("Ventas concretadas", k.CantidadVentas, FormatoValor.Entero, $"{k.Unidades:N0} unidades"),
            new("Ticket promedio", k.TicketPromedio, FormatoValor.Moneda),
        };

        #endregion

        #region 2. Ventas por medio de pago

        private static async Task<ResultadoReporte> VentasPorMedioPagoAsync(Libreria db, ParametrosReporte p, CancellationToken ct)
        {
            var kpis = await KpisAsync(db, p, ct);
            var filas = await Ventas(db, p)
                .GroupBy(v => v.VEN_MetodoPago.MP_Nombre)
                .Select(g => new VentasMedioPagoDTO { MedioPago = g.Key, CantidadVentas = g.Count(), Facturado = g.Sum(v => v.VEN_Total) })
                .OrderByDescending(x => x.Facturado)
                .ToListAsync(ct);

            decimal total = filas.Sum(f => f.Facturado);
            filas.ForEach(f => f.Participacion = total == 0 ? 0 : f.Facturado / total);

            return new ResultadoReporte
            {
                Titulo = "Ventas por medio de pago",
                Descripcion = Descripcion(p),
                Kpis = KpisVentas(kpis),
                Columnas = new()
                {
                    new(nameof(VentasMedioPagoDTO.MedioPago), "Medio de pago", FormatoValor.Texto, 150),
                    new(nameof(VentasMedioPagoDTO.CantidadVentas), "Ventas", FormatoValor.Entero, 60),
                    new(nameof(VentasMedioPagoDTO.Facturado), "Facturado", FormatoValor.Moneda),
                    new(nameof(VentasMedioPagoDTO.Participacion), "Participación", FormatoValor.Porcentaje, 70),
                    new(nameof(VentasMedioPagoDTO.TicketPromedio), "Ticket promedio", FormatoValor.Moneda),
                },
                Filas = filas,
                TipoFila = typeof(VentasMedioPagoDTO),
                Graficos = new()
                {
                    new GraficoReporte
                    {
                        Tipo = TipoGrafico.Torta,
                        Titulo = "Participación en la facturación",
                        Etiquetas = filas.Select(f => f.MedioPago).ToArray(),
                        Series = new() { new("Facturado", filas.Select(f => (double)f.Facturado).ToArray()) },
                    },
                    new GraficoReporte
                    {
                        Tipo = TipoGrafico.Barras,
                        Titulo = "Ticket promedio por medio de pago",
                        Etiquetas = filas.Select(f => f.MedioPago).ToArray(),
                        Series = new() { new("Ticket promedio", filas.Select(f => (double)f.TicketPromedio).ToArray()) },
                    },
                },
            };
        }

        #endregion

        #region 3. Ranking de productos

        private static async Task<ResultadoReporte> RankingProductosAsync(Libreria db, ParametrosReporte p, CancellationToken ct)
        {
            var detalles = Detalles(db, p);

            // GROUP BY libro en SQL; ORDER BY + TOP N también en SQL.
            var agrupado = detalles
                .GroupBy(d => new { d.LIB_ID, d.DV_Libro.LIB_Titulo, d.DV_Libro.LIB_Autor, Categoria = d.DV_Libro.LIB_Genero.GEN_Nombre })
                .Select(g => new ReporteProductoRankingDTO
                {
                    LibroId = g.Key.LIB_ID,
                    Titulo = g.Key.LIB_Titulo,
                    Autor = g.Key.LIB_Autor,
                    Categoria = g.Key.Categoria,
                    Unidades = g.Sum(d => d.DV_Cantidad),
                    Recaudado = g.Sum(d => d.DV_Cantidad * d.DV_PrecioUnitario),
                    CMV = g.Sum(d => d.DV_Cantidad * d.DV_CostoUnitario),
                });

            var filas = await (p.Criterio == CriterioRanking.Unidades
                    ? agrupado.OrderByDescending(x => x.Unidades).ThenByDescending(x => x.Recaudado)
                    : agrupado.OrderByDescending(x => x.Recaudado).ThenByDescending(x => x.Unidades))
                .Take(p.TopN)
                .ToListAsync(ct);

            var totales = await detalles
                .GroupBy(_ => 1)
                .Select(g => new { Unidades = g.Sum(d => d.DV_Cantidad), Recaudado = g.Sum(d => d.DV_Cantidad * d.DV_PrecioUnitario), Titulos = g.Select(d => d.LIB_ID).Distinct().Count() })
                .FirstOrDefaultAsync(ct);

            var porCategoria = await detalles
                .GroupBy(d => d.DV_Libro.LIB_Genero.GEN_Nombre)
                .Select(g => new { Categoria = g.Key, Recaudado = g.Sum(d => d.DV_Cantidad * d.DV_PrecioUnitario) })
                .OrderByDescending(x => x.Recaudado)
                .ToListAsync(ct);

            decimal totalRecaudado = totales?.Recaudado ?? 0;
            int totalUnidades = totales?.Unidades ?? 0;
            for (int i = 0; i < filas.Count; i++)
            {
                filas[i].Puesto = i + 1;
                filas[i].Participacion = p.Criterio == CriterioRanking.Unidades
                    ? (totalUnidades == 0 ? 0 : (decimal)filas[i].Unidades / totalUnidades)
                    : (totalRecaudado == 0 ? 0 : filas[i].Recaudado / totalRecaudado);
            }

            bool porUnidades = p.Criterio == CriterioRanking.Unidades;
            var top = filas.FirstOrDefault();
            return new ResultadoReporte
            {
                Titulo = $"Top {p.TopN} productos por {(porUnidades ? "unidades vendidas" : "recaudación")}",
                Descripcion = Descripcion(p),
                Kpis = new()
                {
                    new("Unidades vendidas", totalUnidades, FormatoValor.Entero),
                    new("Recaudado (precio de lista)", totalRecaudado, FormatoValor.Moneda),
                    new("Títulos vendidos", totales?.Titulos ?? 0, FormatoValor.Entero),
                    new($"Top {p.TopN}: participación", filas.Sum(f => f.Participacion), FormatoValor.Porcentaje,
                        top != null ? $"N° 1: {top.Titulo}" : null),
                },
                Columnas = new()
                {
                    new(nameof(ReporteProductoRankingDTO.Puesto), "#", FormatoValor.Entero, 30),
                    new(nameof(ReporteProductoRankingDTO.Titulo), "Título", FormatoValor.Texto, 220),
                    new(nameof(ReporteProductoRankingDTO.Autor), "Autor", FormatoValor.Texto, 130),
                    new(nameof(ReporteProductoRankingDTO.Categoria), "Categoría", FormatoValor.Texto, 100),
                    new(nameof(ReporteProductoRankingDTO.Unidades), "Unidades", FormatoValor.Entero, 60),
                    new(nameof(ReporteProductoRankingDTO.Recaudado), "Recaudado", FormatoValor.Moneda),
                    new(nameof(ReporteProductoRankingDTO.CMV), "CMV", FormatoValor.Moneda),
                    new(nameof(ReporteProductoRankingDTO.Ganancia), "Ganancia", FormatoValor.Moneda),
                    new(nameof(ReporteProductoRankingDTO.Margen), "Margen", FormatoValor.Porcentaje, 60),
                    new(nameof(ReporteProductoRankingDTO.Participacion), "Particip.", FormatoValor.Porcentaje, 60),
                },
                Filas = filas,
                TipoFila = typeof(ReporteProductoRankingDTO),
                Graficos = new()
                {
                    new GraficoReporte
                    {
                        Tipo = TipoGrafico.BarrasHorizontales,
                        Titulo = porUnidades ? "Unidades vendidas" : "Recaudación",
                        Etiquetas = filas.Select(f => Recortar(f.Titulo, 28)).ToArray(),
                        Series = new() { new(porUnidades ? "Unidades" : "Recaudado",
                            filas.Select(f => porUnidades ? f.Unidades : (double)f.Recaudado).ToArray()) },
                        FormatoValores = porUnidades ? FormatoValor.Entero : FormatoValor.Moneda,
                    },
                    new GraficoReporte
                    {
                        Tipo = TipoGrafico.Torta,
                        Titulo = "Recaudación por categoría",
                        Etiquetas = porCategoria.Select(c => c.Categoria).ToArray(),
                        Series = new() { new("Recaudado", porCategoria.Select(c => (double)c.Recaudado).ToArray()) },
                    },
                },
            };
        }

        #endregion

        #region 4. Stock sin movimiento

        private static async Task<ResultadoReporte> StockSinMovimientoAsync(Libreria db, ParametrosReporte p, CancellationToken ct)
        {
            DateTime desde = p.Desde.Date, hastaExclusivo = p.Hasta.Date.AddDays(1);
            var libros = db.Libros.AsNoTracking().Where(l => l.LIB_Activo && l.LIB_Stock > 0);
            if (p.GeneroId is int g) libros = libros.Where(l => l.GEN_ID == g);

            // NOT EXISTS + MAX correlacionados: una sola consulta SQL.
            var filas = await libros
                .Where(l => !db.DetallesVenta.Any(d => d.LIB_ID == l.LIB_ID && !d.DV_Venta.VEN_Anulada
                                                     && d.DV_Venta.VEN_Fecha >= desde && d.DV_Venta.VEN_Fecha < hastaExclusivo))
                .Select(l => new StockSinMovimientoDTO
                {
                    LibroId = l.LIB_ID,
                    Titulo = l.LIB_Titulo,
                    Categoria = l.LIB_Genero.GEN_Nombre,
                    Stock = l.LIB_Stock,
                    CostoUnitario = l.LIB_PrecioCosto,
                    UltimaVenta = db.DetallesVenta.Where(d => d.LIB_ID == l.LIB_ID && !d.DV_Venta.VEN_Anulada)
                                                  .Max(d => (DateTime?)d.DV_Venta.VEN_Fecha),
                })
                .OrderByDescending(x => x.Stock * x.CostoUnitario)
                .ToListAsync(ct);

            var top = filas.Take(10).ToList();
            return new ResultadoReporte
            {
                Titulo = "Stock sin movimiento (sin ventas en el período)",
                Descripcion = Descripcion(p, "Libros activos con stock"),
                Kpis = new()
                {
                    new("Títulos sin ventas", filas.Count, FormatoValor.Entero),
                    new("Unidades dormidas", filas.Sum(f => f.Stock), FormatoValor.Entero),
                    new("Capital inmovilizado", filas.Sum(f => f.CostoInmovilizado), FormatoValor.Moneda),
                    new("Nunca vendidos", filas.Count(f => f.UltimaVenta == null), FormatoValor.Entero),
                },
                Columnas = new()
                {
                    new(nameof(StockSinMovimientoDTO.Titulo), "Título", FormatoValor.Texto, 240),
                    new(nameof(StockSinMovimientoDTO.Categoria), "Categoría", FormatoValor.Texto, 110),
                    new(nameof(StockSinMovimientoDTO.Stock), "Stock", FormatoValor.Entero, 60),
                    new(nameof(StockSinMovimientoDTO.CostoUnitario), "Costo unit.", FormatoValor.Moneda),
                    new(nameof(StockSinMovimientoDTO.CostoInmovilizado), "Capital inmovilizado", FormatoValor.Moneda),
                    new(nameof(StockSinMovimientoDTO.UltimaVentaTexto), "Última venta", FormatoValor.Texto, 90),
                    new(nameof(StockSinMovimientoDTO.DiasSinVenta), "Días sin venta", FormatoValor.Entero, 70),
                },
                Filas = filas,
                TipoFila = typeof(StockSinMovimientoDTO),
                Graficos = new()
                {
                    new GraficoReporte
                    {
                        Tipo = TipoGrafico.BarrasHorizontales,
                        Titulo = "Mayor capital inmovilizado",
                        Etiquetas = top.Select(f => Recortar(f.Titulo, 28)).ToArray(),
                        Series = new() { new("Capital", top.Select(f => (double)f.CostoInmovilizado).ToArray()) },
                    },
                },
            };
        }

        #endregion

        #region 5. Valorización de stock

        private static async Task<ResultadoReporte> ValorizacionStockAsync(Libreria db, ParametrosReporte p, CancellationToken ct)
        {
            var libros = db.Libros.AsNoTracking().Where(l => l.LIB_Activo && l.LIB_Stock > 0);
            if (p.GeneroId is int g) libros = libros.Where(l => l.GEN_ID == g);

            var filas = await libros
                .GroupBy(l => l.LIB_Genero.GEN_Nombre)
                .Select(grp => new ValorizacionStockDTO
                {
                    Categoria = grp.Key,
                    Titulos = grp.Count(),
                    Unidades = grp.Sum(l => l.LIB_Stock),
                    CostoTotal = grp.Sum(l => l.LIB_Stock * l.LIB_PrecioCosto),
                    ValorVenta = grp.Sum(l => l.LIB_Stock * l.LIB_PrecioVenta),
                })
                .OrderByDescending(x => x.CostoTotal)
                .ToListAsync(ct);

            decimal costo = filas.Sum(f => f.CostoTotal), venta = filas.Sum(f => f.ValorVenta);
            filas.ForEach(f => f.Participacion = costo == 0 ? 0 : f.CostoTotal / costo);

            return new ResultadoReporte
            {
                Titulo = "Valorización del stock actual",
                Descripcion = $"Stock al {DateTime.Now:dd/MM/yyyy HH:mm} · libros activos" + (p.GeneroId != null ? " · categoría filtrada" : ""),
                Kpis = new()
                {
                    new("Costo inmovilizado", costo, FormatoValor.Moneda, $"{filas.Sum(f => f.Unidades):N0} unidades"),
                    new("Valor potencial de venta", venta, FormatoValor.Moneda),
                    new("Ganancia potencial", venta - costo, FormatoValor.Moneda, venta == 0 ? null : $"Margen {((venta - costo) / venta).ToString("P1", Cultura)}"),
                    new("Títulos con stock", filas.Sum(f => f.Titulos), FormatoValor.Entero),
                },
                Columnas = new()
                {
                    new(nameof(ValorizacionStockDTO.Categoria), "Categoría", FormatoValor.Texto, 150),
                    new(nameof(ValorizacionStockDTO.Titulos), "Títulos", FormatoValor.Entero, 60),
                    new(nameof(ValorizacionStockDTO.Unidades), "Unidades", FormatoValor.Entero, 60),
                    new(nameof(ValorizacionStockDTO.CostoTotal), "Costo total", FormatoValor.Moneda),
                    new(nameof(ValorizacionStockDTO.ValorVenta), "Valor de venta", FormatoValor.Moneda),
                    new(nameof(ValorizacionStockDTO.GananciaPotencial), "Ganancia potencial", FormatoValor.Moneda),
                    new(nameof(ValorizacionStockDTO.Margen), "Margen", FormatoValor.Porcentaje, 60),
                    new(nameof(ValorizacionStockDTO.Participacion), "% del capital", FormatoValor.Porcentaje, 70),
                },
                Filas = filas,
                TipoFila = typeof(ValorizacionStockDTO),
                Graficos = new()
                {
                    new GraficoReporte
                    {
                        Tipo = TipoGrafico.Torta,
                        Titulo = "Capital inmovilizado por categoría",
                        Etiquetas = filas.Select(f => f.Categoria).ToArray(),
                        Series = new() { new("Costo", filas.Select(f => (double)f.CostoTotal).ToArray()) },
                    },
                    new GraficoReporte
                    {
                        Tipo = TipoGrafico.Barras,
                        Titulo = "Costo vs. valor de venta",
                        Etiquetas = filas.Select(f => f.Categoria).ToArray(),
                        Series = new()
                        {
                            new("Costo", filas.Select(f => (double)f.CostoTotal).ToArray()),
                            new("Valor de venta", filas.Select(f => (double)f.ValorVenta).ToArray()),
                        },
                    },
                },
            };
        }

        #endregion

        #region 6. Mejores clientes

        private static async Task<ResultadoReporte> MejoresClientesAsync(Libreria db, ParametrosReporte p, CancellationToken ct)
        {
            var ventas = Ventas(db, p);
            if (p.ExcluirConsumidorFinal) ventas = ventas.Where(v => !v.VEN_Cliente.CLI_ConsumidorFinal);

            var filas = await ventas
                .GroupBy(v => new { v.CLI_ID, v.VEN_Cliente.CLI_Persona.PER_Nombre, v.VEN_Cliente.CLI_Documento })
                .Select(g => new ClienteRankingDTO
                {
                    ClienteId = g.Key.CLI_ID,
                    Cliente = g.Key.PER_Nombre,
                    Documento = g.Key.CLI_Documento,
                    Compras = g.Count(),
                    Total = g.Sum(v => v.VEN_Total),
                    PrimeraCompra = g.Min(v => v.VEN_Fecha),
                    UltimaCompra = g.Max(v => v.VEN_Fecha),
                })
                .OrderByDescending(x => x.Total)
                .Take(p.TopN)
                .ToListAsync(ct);

            var totales = await ventas
                .GroupBy(_ => 1)
                .Select(g => new { Total = g.Sum(v => v.VEN_Total), Ventas = g.Count(), Clientes = g.Select(v => v.CLI_ID).Distinct().Count() })
                .FirstOrDefaultAsync(ct);

            decimal meses = Math.Max(1m, (decimal)(p.Hasta.Date - p.Desde.Date).TotalDays / 30.4m);
            decimal total = totales?.Total ?? 0;
            for (int i = 0; i < filas.Count; i++)
            {
                filas[i].Puesto = i + 1;
                filas[i].ComprasPorMes = Math.Round(filas[i].Compras / meses, 1);
                filas[i].Participacion = total == 0 ? 0 : filas[i].Total / total;
            }

            return new ResultadoReporte
            {
                Titulo = $"Top {p.TopN} clientes por volumen de compra",
                Descripcion = Descripcion(p, p.ExcluirConsumidorFinal ? "Sin Consumidor Final" : "Incluye Consumidor Final"),
                Kpis = new()
                {
                    new("Clientes con compras", totales?.Clientes ?? 0, FormatoValor.Entero),
                    new("Total comprado", total, FormatoValor.Moneda, $"{totales?.Ventas ?? 0:N0} compras"),
                    new("Ticket promedio", (totales?.Ventas ?? 0) == 0 ? 0 : total / totales!.Ventas, FormatoValor.Moneda),
                    new($"Top {p.TopN}: participación", filas.Sum(f => f.Participacion), FormatoValor.Porcentaje),
                },
                Columnas = new()
                {
                    new(nameof(ClienteRankingDTO.Puesto), "#", FormatoValor.Entero, 30),
                    new(nameof(ClienteRankingDTO.Cliente), "Cliente", FormatoValor.Texto, 200),
                    new(nameof(ClienteRankingDTO.Documento), "Documento", FormatoValor.Texto, 90),
                    new(nameof(ClienteRankingDTO.Compras), "Compras", FormatoValor.Entero, 60),
                    new(nameof(ClienteRankingDTO.ComprasPorMes), "Compras/mes", FormatoValor.Decimal, 70),
                    new(nameof(ClienteRankingDTO.Total), "Total", FormatoValor.Moneda),
                    new(nameof(ClienteRankingDTO.TicketPromedio), "Ticket prom.", FormatoValor.Moneda),
                    new(nameof(ClienteRankingDTO.UltimaCompra), "Última compra", FormatoValor.Fecha, 90),
                    new(nameof(ClienteRankingDTO.Participacion), "Particip.", FormatoValor.Porcentaje, 60),
                },
                Filas = filas,
                TipoFila = typeof(ClienteRankingDTO),
                Graficos = new()
                {
                    new GraficoReporte
                    {
                        Tipo = TipoGrafico.BarrasHorizontales,
                        Titulo = "Total comprado",
                        Etiquetas = filas.Take(10).Select(f => Recortar(f.Cliente, 28)).ToArray(),
                        Series = new() { new("Total", filas.Take(10).Select(f => (double)f.Total).ToArray()) },
                    },
                },
            };
        }

        #endregion

        #region 7. Compras por proveedor y 8. Órdenes de reposición

        private static IQueryable<OrdenReposicion> Ordenes(Libreria db, ParametrosReporte p)
        {
            DateTime desde = p.Desde.Date, hastaExclusivo = p.Hasta.Date.AddDays(1);
            var q = db.OrdenesReposicion.AsNoTracking().Where(o => o.OR_Fecha >= desde && o.OR_Fecha < hastaExclusivo);
            if (p.ProveedorId is int prov) q = q.Where(o => o.OR_PROV_ID == prov);
            return q;
        }

        private static async Task<ResultadoReporte> ComprasPorProveedorAsync(Libreria db, ParametrosReporte p, CancellationToken ct)
        {
            // Dos agregaciones en SQL (cabeceras y detalles) que se combinan por proveedor.
            var porOrden = await Ordenes(db, p)
                .GroupBy(o => new { o.OR_PROV_ID, Nombre = o.OR_Proveedor.PROV_Empresa != "" ? o.OR_Proveedor.PROV_Empresa : o.OR_Proveedor.PER_Proveedor.PER_Nombre })
                .Select(g => new ComprasProveedorDTO
                {
                    ProveedorId = g.Key.OR_PROV_ID,
                    Proveedor = g.Key.Nombre,
                    Ordenes = g.Count(),
                    Recibidas = g.Count(o => o.OR_Estado == EstadoOrden.Recibida),
                    Canceladas = g.Count(o => o.OR_Estado == EstadoOrden.Cancelada),
                    Activas = g.Count(o => o.OR_Estado == EstadoOrden.Pendiente || o.OR_Estado == EstadoOrden.Solicitada),
                })
                .ToListAsync(ct);

            var ordenesIds = Ordenes(db, p).Where(o => o.OR_Estado != EstadoOrden.Cancelada).Select(o => o.OR_ID);
            var porDetalle = await db.DetallesOrdenReposicion.AsNoTracking()
                .Where(d => ordenesIds.Contains(d.OR_ID))
                .GroupBy(d => d.DOR_Orden.OR_PROV_ID)
                .Select(g => new
                {
                    ProveedorId = g.Key,
                    Pedidas = g.Sum(d => d.DOR_CantidadPedida),
                    Recibidas = g.Sum(d => d.DOR_CantidadRecibida ?? 0),
                    PedidasDeRecibidas = g.Where(d => d.DOR_Orden.OR_Estado == EstadoOrden.Recibida).Sum(d => d.DOR_CantidadPedida),
                    Estimado = g.Sum(d => d.DOR_CantidadPedida * d.DOR_CostoUnitario),
                    Recibido = g.Sum(d => (d.DOR_CantidadRecibida ?? 0) * (d.DOR_CostoRecibido ?? d.DOR_CostoUnitario)),
                })
                .ToDictionaryAsync(x => x.ProveedorId, ct);

            foreach (var f in porOrden)
            {
                if (!porDetalle.TryGetValue(f.ProveedorId, out var d)) continue;
                f.UnidadesPedidas = d.Pedidas;
                f.UnidadesRecibidas = d.Recibidas;
                f.MontoEstimado = d.Estimado;
                f.MontoRecibido = d.Recibido;
                f.Cumplimiento = d.PedidasDeRecibidas == 0 ? 0 : (decimal)d.Recibidas / d.PedidasDeRecibidas;
            }
            var filas = porOrden.OrderByDescending(f => f.MontoRecibido).ThenByDescending(f => f.MontoEstimado).ToList();

            return new ResultadoReporte
            {
                Titulo = "Compras por proveedor",
                Descripcion = Descripcion(p, "Órdenes emitidas en el período"),
                Kpis = new()
                {
                    new("Órdenes emitidas", filas.Sum(f => f.Ordenes), FormatoValor.Entero, $"{filas.Sum(f => f.Activas)} activas"),
                    new("Órdenes recibidas", filas.Sum(f => f.Recibidas), FormatoValor.Entero, $"{filas.Sum(f => f.Canceladas)} canceladas"),
                    new("Monto comprado (recibido)", filas.Sum(f => f.MontoRecibido), FormatoValor.Moneda,
                        $"Estimado {filas.Sum(f => f.MontoEstimado).ToString("C0", Cultura)}"),
                    new("Proveedores", filas.Count, FormatoValor.Entero),
                },
                Columnas = new()
                {
                    new(nameof(ComprasProveedorDTO.Proveedor), "Proveedor", FormatoValor.Texto, 200),
                    new(nameof(ComprasProveedorDTO.Ordenes), "Órdenes", FormatoValor.Entero, 60),
                    new(nameof(ComprasProveedorDTO.Recibidas), "Recibidas", FormatoValor.Entero, 60),
                    new(nameof(ComprasProveedorDTO.Activas), "Activas", FormatoValor.Entero, 60),
                    new(nameof(ComprasProveedorDTO.Canceladas), "Canceladas", FormatoValor.Entero, 60),
                    new(nameof(ComprasProveedorDTO.UnidadesPedidas), "Unid. pedidas", FormatoValor.Entero, 70),
                    new(nameof(ComprasProveedorDTO.UnidadesRecibidas), "Unid. recibidas", FormatoValor.Entero, 70),
                    new(nameof(ComprasProveedorDTO.MontoEstimado), "Monto estimado", FormatoValor.Moneda),
                    new(nameof(ComprasProveedorDTO.MontoRecibido), "Monto recibido", FormatoValor.Moneda),
                    new(nameof(ComprasProveedorDTO.Cumplimiento), "Cumplimiento", FormatoValor.Porcentaje, 70),
                },
                Filas = filas,
                TipoFila = typeof(ComprasProveedorDTO),
                Graficos = new()
                {
                    new GraficoReporte
                    {
                        Tipo = TipoGrafico.Barras,
                        Titulo = "Monto estimado vs. recibido",
                        Etiquetas = filas.Take(10).Select(f => Recortar(f.Proveedor, 18)).ToArray(),
                        Series = new()
                        {
                            new("Estimado", filas.Take(10).Select(f => (double)f.MontoEstimado).ToArray()),
                            new("Recibido", filas.Take(10).Select(f => (double)f.MontoRecibido).ToArray()),
                        },
                    },
                    new GraficoReporte
                    {
                        Tipo = TipoGrafico.Torta,
                        Titulo = "Órdenes por estado",
                        Etiquetas = new[] { "Recibidas", "Activas", "Canceladas" },
                        Series = new() { new("Órdenes", new double[] { filas.Sum(f => f.Recibidas), filas.Sum(f => f.Activas), filas.Sum(f => f.Canceladas) }) },
                        FormatoValores = FormatoValor.Entero,
                    },
                },
            };
        }

        private static async Task<ResultadoReporte> OrdenesReposicionAsync(Libreria db, ParametrosReporte p, CancellationToken ct)
        {
            var filas = await Ordenes(db, p)
                .OrderByDescending(o => o.OR_Fecha)
                .Select(o => new OrdenReporteDTO
                {
                    OrdenId = o.OR_ID,
                    Fecha = o.OR_Fecha,
                    Proveedor = o.OR_Proveedor.PROV_Empresa != "" ? o.OR_Proveedor.PROV_Empresa : o.OR_Proveedor.PER_Proveedor.PER_Nombre,
                    EstadoCodigo = o.OR_Estado,
                    Items = o.OR_Detalles.Count(),
                    UnidadesPedidas = o.OR_Detalles.Sum(d => (int?)d.DOR_CantidadPedida) ?? 0,
                    UnidadesRecibidas = o.OR_Detalles.Sum(d => d.DOR_CantidadRecibida) ?? 0,
                    MontoEstimado = o.OR_Detalles.Sum(d => (decimal?)(d.DOR_CantidadPedida * d.DOR_CostoUnitario)) ?? 0m,
                    MontoRecibido = o.OR_Detalles.Sum(d => (decimal?)((d.DOR_CantidadRecibida ?? 0) * (d.DOR_CostoRecibido ?? d.DOR_CostoUnitario))) ?? 0m,
                    FechaRecepcion = o.OR_FechaRecepcion,
                })
                .ToListAsync(ct);

            var porEstado = filas.GroupBy(f => f.Estado).Select(g => (Estado: g.Key, Cantidad: g.Count())).ToList();
            return new ResultadoReporte
            {
                Titulo = "Órdenes de reposición emitidas",
                Descripcion = Descripcion(p),
                Kpis = new()
                {
                    new("Órdenes", filas.Count, FormatoValor.Entero),
                    new("Unidades pedidas", filas.Sum(f => f.UnidadesPedidas), FormatoValor.Entero, $"{filas.Sum(f => f.UnidadesRecibidas):N0} recibidas"),
                    new("Monto estimado", filas.Where(f => f.EstadoCodigo != EstadoOrden.Cancelada).Sum(f => f.MontoEstimado), FormatoValor.Moneda),
                    new("Monto recibido", filas.Sum(f => f.MontoRecibido), FormatoValor.Moneda),
                },
                Columnas = new()
                {
                    new(nameof(OrdenReporteDTO.Numero), "N° orden", FormatoValor.Texto, 80),
                    new(nameof(OrdenReporteDTO.Fecha), "Emisión", FormatoValor.Fecha, 80),
                    new(nameof(OrdenReporteDTO.Proveedor), "Proveedor", FormatoValor.Texto, 180),
                    new(nameof(OrdenReporteDTO.Estado), "Estado", FormatoValor.Texto, 110),
                    new(nameof(OrdenReporteDTO.Items), "Ítems", FormatoValor.Entero, 50),
                    new(nameof(OrdenReporteDTO.UnidadesPedidas), "Unid. pedidas", FormatoValor.Entero, 70),
                    new(nameof(OrdenReporteDTO.UnidadesRecibidas), "Unid. recibidas", FormatoValor.Entero, 70),
                    new(nameof(OrdenReporteDTO.MontoEstimado), "Monto estimado", FormatoValor.Moneda),
                    new(nameof(OrdenReporteDTO.MontoRecibido), "Monto recibido", FormatoValor.Moneda),
                    new(nameof(OrdenReporteDTO.FechaRecepcion), "Recepción", FormatoValor.Fecha, 80),
                },
                Filas = filas,
                TipoFila = typeof(OrdenReporteDTO),
                Graficos = new()
                {
                    new GraficoReporte
                    {
                        Tipo = TipoGrafico.Torta,
                        Titulo = "Órdenes por estado",
                        Etiquetas = porEstado.Select(e => e.Estado).ToArray(),
                        Series = new() { new("Órdenes", porEstado.Select(e => (double)e.Cantidad).ToArray()) },
                        FormatoValores = FormatoValor.Entero,
                    },
                },
            };
        }

        #endregion

        #region Helpers

        private static string Descripcion(ParametrosReporte p, string? extra = null) =>
            $"Período {p.Desde:dd/MM/yyyy} al {p.Hasta:dd/MM/yyyy}"
            + (string.IsNullOrWhiteSpace(p.DescripcionFiltros) ? "" : $" · {p.DescripcionFiltros}")
            + (extra != null ? $" · {extra}" : "");

        private static string Recortar(string texto, int max) => texto.Length <= max ? texto : texto[..(max - 1)] + "…";

        #endregion
    }
}
