using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Modelo
{
    #region Parámetros

    public enum TipoReporte
    {
        VentasPorPeriodo,
        VentasPorMedioPago,
        RankingProductos,
        StockSinMovimiento,
        ValorizacionStock,
        MejoresClientes,
        ComprasPorProveedor,
        OrdenesReposicion,
    }

    public enum Agrupacion { Diaria, Semanal, Mensual }

    public enum CriterioRanking { Unidades, Recaudacion }

    /// <summary>Todo lo que el usuario elige en la barra de parámetros. Cada reporte usa sólo lo que le corresponde.</summary>
    public sealed class ParametrosReporte
    {
        public TipoReporte Tipo { get; init; }
        public DateTime Desde { get; init; }
        /// <summary>Inclusive: se toma el día completo.</summary>
        public DateTime Hasta { get; init; }
        public Agrupacion Agrupacion { get; init; } = Agrupacion.Diaria;
        public int? MetodoPagoId { get; init; }
        public int? GeneroId { get; init; }
        public int? ProveedorId { get; init; }
        public int TopN { get; init; } = 10;
        public CriterioRanking Criterio { get; init; } = CriterioRanking.Unidades;
        public bool ExcluirConsumidorFinal { get; init; } = true;
        /// <summary>Texto legible de los filtros elegidos (ej. "Medio: Efectivo · Categoría: Novela"), para encabezados.</summary>
        public string? DescripcionFiltros { get; init; }
    }

    #endregion

    #region Resultado genérico (lo que muestran la pantalla y los exportadores)

    public enum FormatoValor { Texto, Entero, Decimal, Moneda, Porcentaje, Fecha }

    public enum TipoGrafico { Lineas, Barras, BarrasHorizontales, Torta }

    /// <summary>Tarjeta de KPI: título, valor destacado y un detalle opcional (ej. "Margen 32,5 %").</summary>
    public sealed record KpiDTO(string Titulo, decimal Valor, FormatoValor Formato, string? Detalle = null);

    /// <summary>Columna de la grilla/exportación: qué propiedad del DTO, encabezado, formato y ancho relativo.</summary>
    public sealed record ColumnaReporte(string Propiedad, string Encabezado, FormatoValor Formato = FormatoValor.Texto, float Peso = 100);

    public sealed record SerieGrafico(string Nombre, double[] Valores);

    public sealed class GraficoReporte
    {
        public TipoGrafico Tipo { get; init; }
        public string Titulo { get; init; } = string.Empty;
        public string[] Etiquetas { get; init; } = Array.Empty<string>();
        public List<SerieGrafico> Series { get; init; } = new();
        public FormatoValor FormatoValores { get; init; } = FormatoValor.Moneda;
    }

    /// <summary>
    /// Resultado de cualquier reporte. La pantalla y los exportadores (Excel/PDF) lo dibujan de forma genérica,
    /// así un reporte nuevo sólo requiere una consulta en ReporteService.
    /// </summary>
    public sealed class ResultadoReporte
    {
        public string Titulo { get; init; } = string.Empty;
        /// <summary>Período y filtros aplicados, en una línea.</summary>
        public string Descripcion { get; init; } = string.Empty;
        public DateTime Generado { get; init; } = DateTime.Now;
        public List<KpiDTO> Kpis { get; init; } = new();
        public List<ColumnaReporte> Columnas { get; init; } = new();
        /// <summary>Filas (lista tipada de DTOs).</summary>
        public IList Filas { get; init; } = new List<object>();
        public Type TipoFila { get; init; } = typeof(object);
        public List<GraficoReporte> Graficos { get; init; } = new();
    }

    #endregion

    #region DTOs de cada reporte

    /// <summary>KPIs de ventas del período (todas calculadas en SQL).</summary>
    public sealed class ResumenKpisDTO
    {
        public int CantidadVentas { get; set; }
        public int Unidades { get; set; }
        public decimal TotalFacturado { get; set; }
        /// <summary>Costo de mercadería vendida.</summary>
        public decimal CMV { get; set; }
        public decimal GananciaBruta => TotalFacturado - CMV;
        public decimal Margen => TotalFacturado == 0 ? 0 : GananciaBruta / TotalFacturado;
        public decimal TicketPromedio => CantidadVentas == 0 ? 0 : TotalFacturado / CantidadVentas;
    }

    /// <summary>Ventas agregadas por período (día, semana o mes).</summary>
    public sealed class ReporteVentasDTO
    {
        public DateTime Inicio { get; set; }
        public string Periodo { get; set; } = string.Empty;
        public int CantidadVentas { get; set; }
        public int Unidades { get; set; }
        public decimal Facturado { get; set; }
        public decimal CMV { get; set; }
        public decimal GananciaBruta => Facturado - CMV;
        public decimal Margen => Facturado == 0 ? 0 : GananciaBruta / Facturado;
        public decimal TicketPromedio => CantidadVentas == 0 ? 0 : Facturado / CantidadVentas;
    }

    public sealed class VentasMedioPagoDTO
    {
        public string MedioPago { get; set; } = string.Empty;
        public int CantidadVentas { get; set; }
        public decimal Facturado { get; set; }
        public decimal Participacion { get; set; }
        public decimal TicketPromedio => CantidadVentas == 0 ? 0 : Facturado / CantidadVentas;
    }

    public sealed class ReporteProductoRankingDTO
    {
        public int Puesto { get; set; }
        public int LibroId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public int Unidades { get; set; }
        /// <summary>Recaudado a precio de lista (antes de descuentos/recargos de la venta).</summary>
        public decimal Recaudado { get; set; }
        public decimal CMV { get; set; }
        public decimal Ganancia => Recaudado - CMV;
        public decimal Margen => Recaudado == 0 ? 0 : Ganancia / Recaudado;
        public decimal Participacion { get; set; }
    }

    public sealed class StockSinMovimientoDTO
    {
        public int LibroId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public int Stock { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal CostoInmovilizado => Stock * CostoUnitario;
        public DateTime? UltimaVenta { get; set; }
        public string UltimaVentaTexto => UltimaVenta?.ToString("dd/MM/yyyy") ?? "Nunca";
        public int? DiasSinVenta => UltimaVenta is DateTime f ? (int)(DateTime.Today - f.Date).TotalDays : null;
    }

    public sealed class ValorizacionStockDTO
    {
        public string Categoria { get; set; } = string.Empty;
        public int Titulos { get; set; }
        public int Unidades { get; set; }
        public decimal CostoTotal { get; set; }
        public decimal ValorVenta { get; set; }
        public decimal GananciaPotencial => ValorVenta - CostoTotal;
        public decimal Margen => ValorVenta == 0 ? 0 : GananciaPotencial / ValorVenta;
        public decimal Participacion { get; set; }
    }

    public sealed class ClienteRankingDTO
    {
        public int Puesto { get; set; }
        public int ClienteId { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string? Documento { get; set; }
        public int Compras { get; set; }
        public decimal Total { get; set; }
        public decimal TicketPromedio => Compras == 0 ? 0 : Total / Compras;
        public DateTime PrimeraCompra { get; set; }
        public DateTime UltimaCompra { get; set; }
        /// <summary>Compras promedio por mes dentro del período consultado.</summary>
        public decimal ComprasPorMes { get; set; }
        public decimal Participacion { get; set; }
    }

    public sealed class ComprasProveedorDTO
    {
        public int ProveedorId { get; set; }
        public string Proveedor { get; set; } = string.Empty;
        public int Ordenes { get; set; }
        public int Recibidas { get; set; }
        public int Activas { get; set; }
        public int Canceladas { get; set; }
        public int UnidadesPedidas { get; set; }
        public int UnidadesRecibidas { get; set; }
        public decimal MontoEstimado { get; set; }
        public decimal MontoRecibido { get; set; }
        /// <summary>Unidades recibidas / pedidas (sólo órdenes recibidas).</summary>
        public decimal Cumplimiento { get; set; }
    }

    public sealed class OrdenReporteDTO
    {
        public int OrdenId { get; set; }
        public string Numero => OrdenReposicion.FormatearNumero(OrdenId);
        public DateTime Fecha { get; set; }
        public string Proveedor { get; set; } = string.Empty;
        public string EstadoCodigo { get; set; } = string.Empty;
        public string Estado => EstadoOrden.Descripcion(EstadoCodigo);
        public int Items { get; set; }
        public int UnidadesPedidas { get; set; }
        public int UnidadesRecibidas { get; set; }
        public decimal MontoEstimado { get; set; }
        public decimal MontoRecibido { get; set; }
        public DateTime? FechaRecepcion { get; set; }
    }

    #endregion
}
