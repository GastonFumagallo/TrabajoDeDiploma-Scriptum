using System;
using System.Collections.Generic;
using System.Text;

namespace Modelo
{
    #region Inventario

    public enum EstadoStock
    {
        Normal,
        /// <summary>Stock ≤ punto de reposición: hay que pedir.</summary>
        AReponer,
        /// <summary>Stock ≤ stock mínimo.</summary>
        Critico,
        /// <summary>Stock = 0.</summary>
        Agotado,
    }

    public enum FiltroEstadoStock
    {
        Todos,
        /// <summary>Agotados + críticos + a reponer.</summary>
        RequierenReposicion,
        /// <summary>Agotados + críticos.</summary>
        BajoMinimo,
        Agotados,
    }

    public sealed class FiltroInventario
    {
        /// <summary>Busca en ISBN, título, autor y editorial.</summary>
        public string? Texto { get; set; }
        public int? GeneroId { get; set; }
        public int? ProveedorId { get; set; }
        public FiltroEstadoStock Estado { get; set; } = FiltroEstadoStock.Todos;
    }

    /// <summary>Fila de la grilla de inventario. Se arma con una única proyección (sin N+1).</summary>
    public sealed class ProductoInventarioDTO
    {
        public int LibroId { get; set; }
        public string? Codigo { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public int? ProveedorHabitualId { get; set; }
        public string? ProveedorHabitual { get; set; }
        public int Stock { get; set; }
        public int StockMinimo { get; set; }
        public int PuntoReposicion { get; set; }
        public int StockOptimo { get; set; }
        public decimal PrecioCosto { get; set; }
        public decimal PrecioVenta { get; set; }
        /// <summary>Unidades ya pedidas en órdenes activas (pendientes o solicitadas).</summary>
        public int EnPedido { get; set; }

        public EstadoStock Estado => Calcular(Stock, StockMinimo, PuntoReposicion);

        public string EstadoTexto => Estado switch
        {
            EstadoStock.Agotado => "Sin stock",
            EstadoStock.Critico => "Crítico",
            EstadoStock.AReponer => "A reponer",
            _ => "Normal",
        };

        /// <summary>Cantidad para llegar al stock óptimo, descontando lo que ya está pedido.</summary>
        public int CantidadSugerida => Math.Max(0, StockOptimo - Stock - EnPedido);

        public decimal ValorCosto => Stock * PrecioCosto;

        public static EstadoStock Calcular(int stock, int minimo, int puntoReposicion) =>
            stock <= 0 ? EstadoStock.Agotado
            : stock <= minimo ? EstadoStock.Critico
            : stock <= puntoReposicion ? EstadoStock.AReponer
            : EstadoStock.Normal;
    }

    /// <summary>Indicadores de la cabecera del inventario (sobre todo el catálogo, no sólo lo filtrado).</summary>
    public sealed class ResumenInventarioDTO
    {
        public int Total { get; set; }
        public int Normales { get; set; }
        public int AReponer { get; set; }
        public int Criticos { get; set; }
        public int Agotados { get; set; }
        public decimal ValorCosto { get; set; }
    }

    public enum TipoAjusteStock
    {
        /// <summary>Entra mercadería fuera de una orden (devolución, hallazgo...).</summary>
        Ingreso,
        /// <summary>Sale mercadería que no es venta (rotura, deterioro, pérdida...).</summary>
        Egreso,
        /// <summary>Se fija el stock al valor contado físicamente.</summary>
        ConteoFisico,
    }

    public sealed class AjusteStockSolicitud
    {
        public int LibroId { get; init; }
        public TipoAjusteStock Tipo { get; init; }
        /// <summary>Unidades a ingresar/egresar, o stock contado si es conteo físico.</summary>
        public int Cantidad { get; init; }
        public string Motivo { get; init; } = string.Empty;
        public string? Observacion { get; init; }
        public string Usuario { get; init; } = string.Empty;
    }

    public sealed class ParametrosStockSolicitud
    {
        public int LibroId { get; init; }
        public int StockMinimo { get; init; }
        public int PuntoReposicion { get; init; }
        public int StockOptimo { get; init; }
    }

    public sealed class MovimientoStockDTO
    {
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public int StockAnterior { get; set; }
        public int StockResultante { get; set; }
        public string? Motivo { get; set; }
        public string? Observacion { get; set; }
        public string? Referencia { get; set; }
        public string? Usuario { get; set; }
    }

    /// <summary>Proveedor asociado a un libro, con su contacto y el precio pactado.</summary>
    public sealed class ProveedorDeLibroDTO
    {
        public string Empresa { get; set; } = string.Empty;
        public string Contacto { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public decimal PrecioCompra { get; set; }
    }

    /// <summary>Opción simple para combos (géneros, proveedores).</summary>
    public sealed record OpcionDTO(int Id, string Nombre)
    {
        public override string ToString() => Nombre;
    }

    #endregion

    #region Órdenes de reposición

    public sealed class FiltroOrdenes
    {
        /// <summary>null = todas; "ACTIVAS" = pendientes + solicitadas; o un estado puntual.</summary>
        public string? Estado { get; set; } = "ACTIVAS";
        public int? ProveedorId { get; set; }
    }

    /// <summary>Fila del listado de órdenes.</summary>
    public sealed class OrdenReposicionDTO
    {
        public int OrdenId { get; set; }
        public string Numero => OrdenReposicion.FormatearNumero(OrdenId);
        public DateTime Fecha { get; set; }
        public string Proveedor { get; set; } = string.Empty;
        public string EstadoCodigo { get; set; } = string.Empty;
        public string Estado => EstadoOrden.Descripcion(EstadoCodigo);
        public int Items { get; set; }
        public int Unidades { get; set; }
        public decimal TotalEstimado { get; set; }
        public string? Usuario { get; set; }
        public DateTime? FechaRecepcion { get; set; }
    }

    public sealed class ProveedorContactoDTO
    {
        public int ProveedorId { get; set; }
        public string Empresa { get; set; } = string.Empty;
        public string Contacto { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Email { get; set; }

        public override string ToString() =>
            string.IsNullOrWhiteSpace(Empresa) ? Contacto : $"{Empresa} ({Contacto})";
    }

    /// <summary>Línea de la orden tal como la edita el formulario.</summary>
    public sealed class ItemOrdenDTO
    {
        public int? DetalleId { get; set; }
        public int LibroId { get; set; }
        public string? Codigo { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public int StockActual { get; set; }
        public int StockOptimo { get; set; }
        public int CantidadPedida { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal Subtotal => CantidadPedida * CostoUnitario;

        // Recepción: se completan al recibir (por defecto, lo pedido).
        public int? CantidadRecibida { get; set; }
        public decimal? CostoRecibido { get; set; }
    }

    /// <summary>Orden completa (cabecera + ítems) para abrirla en el formulario.</summary>
    public sealed class OrdenReposicionDetalleDTO
    {
        public int OrdenId { get; set; }
        public string Numero => OrdenReposicion.FormatearNumero(OrdenId);
        public DateTime Fecha { get; set; }
        public int ProveedorId { get; set; }
        public string EstadoCodigo { get; set; } = EstadoOrden.Pendiente;
        public string? Usuario { get; set; }
        public string? Observaciones { get; set; }
        public DateTime? FechaRecepcion { get; set; }
        public string? UsuarioRecepcion { get; set; }
        public string? MotivoCancelacion { get; set; }
        public List<ItemOrdenDTO> Items { get; set; } = new();
    }

    /// <summary>Datos para crear o actualizar una orden en estado Pendiente.</summary>
    public sealed class GuardarOrdenSolicitud
    {
        /// <summary>null = orden nueva.</summary>
        public int? OrdenId { get; init; }
        public int ProveedorId { get; init; }
        public string? Observaciones { get; init; }
        public string Usuario { get; init; } = string.Empty;
        public IReadOnlyList<(int LibroId, int Cantidad, decimal CostoUnitario)> Items { get; init; } = Array.Empty<(int, int, decimal)>();
    }

    public sealed class RecepcionOrdenSolicitud
    {
        public int OrdenId { get; init; }
        public string Usuario { get; init; } = string.Empty;
        /// <summary>Si es true, actualiza el último costo del libro y el precio del proveedor cuando varió.</summary>
        public bool ActualizarCostos { get; init; } = true;
        public IReadOnlyList<(int DetalleId, int CantidadRecibida, decimal CostoRecibido)> Lineas { get; init; } = Array.Empty<(int, int, decimal)>();
    }

    /// <summary>Resultado de una operación de inventario/órdenes. Si falla, <see cref="Mensaje"/> es apto para el usuario.</summary>
    public sealed class ResultadoOperacion
    {
        public bool Exito { get; private init; }
        public string Mensaje { get; private init; } = string.Empty;
        public int Id { get; private init; }

        public static ResultadoOperacion Ok(int id = 0, string mensaje = "") => new() { Exito = true, Id = id, Mensaje = mensaje };
        public static ResultadoOperacion Error(string mensaje) => new() { Exito = false, Mensaje = mensaje };
    }

    #endregion
}
