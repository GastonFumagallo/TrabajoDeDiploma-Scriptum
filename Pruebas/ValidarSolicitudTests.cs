using Modelo;
using Xunit;

namespace Pruebas
{
    /// <summary>
    /// Pruebas de caja blanca de <c>FacadeVentas.ValidarSolicitud</c>: un caso por cada camino independiente
    /// del grafo de flujo (V(G) = 7). Ver la sección "Pruebas" de la Entrega Final.
    /// </summary>
    public class ValidarSolicitudTests
    {
        private const string CarritoVacio = "El carrito está vacío.";
        private const string CantidadInvalida = "Todas las cantidades deben ser mayores a cero.";
        private const string SinMedioDePago = "Seleccione un medio de pago.";
        private const string DescuentoFueraDeRango = "El descuento debe estar entre 0 % y 30 %.";

        private static SolicitudVenta Solicitud(IReadOnlyList<ItemVentaSolicitud>? items, int metodoPagoId = 1, decimal descuento = 0m) =>
            new() { Items = items!, MetodoPagoId = metodoPagoId, PorcentajeDescuento = descuento };

        private static IReadOnlyList<ItemVentaSolicitud> UnLibro(int cantidad) => new[] { new ItemVentaSolicitud(1, cantidad) };

        [Fact(DisplayName = "CB1 - C1: lista de ítems nula")]
        public void CB1_ItemsNulos() =>
            Assert.Equal(CarritoVacio, FacadeVentas.ValidarSolicitud(Solicitud(null)));

        [Fact(DisplayName = "CB2 - C2: lista de ítems vacía")]
        public void CB2_ItemsVacios() =>
            Assert.Equal(CarritoVacio, FacadeVentas.ValidarSolicitud(Solicitud(Array.Empty<ItemVentaSolicitud>())));

        [Fact(DisplayName = "CB3 - C3: cantidad cero")]
        public void CB3_CantidadCero() =>
            Assert.Equal(CantidadInvalida, FacadeVentas.ValidarSolicitud(Solicitud(UnLibro(0))));

        [Fact(DisplayName = "CB4 - C4: sin medio de pago")]
        public void CB4_SinMedioDePago() =>
            Assert.Equal(SinMedioDePago, FacadeVentas.ValidarSolicitud(Solicitud(UnLibro(2), metodoPagoId: 0)));

        [Fact(DisplayName = "CB5 - C5: descuento negativo")]
        public void CB5_DescuentoNegativo() =>
            Assert.Equal(DescuentoFueraDeRango, FacadeVentas.ValidarSolicitud(Solicitud(UnLibro(2), descuento: -5m)));

        [Fact(DisplayName = "CB6 - C6: descuento mayor al tope")]
        public void CB6_DescuentoMayorAlTope() =>
            Assert.Equal(DescuentoFueraDeRango, FacadeVentas.ValidarSolicitud(Solicitud(UnLibro(2), descuento: 35m)));

        [Fact(DisplayName = "CB7 - C7: solicitud válida")]
        public void CB7_SolicitudValida() =>
            Assert.Null(FacadeVentas.ValidarSolicitud(Solicitud(UnLibro(2), descuento: 10m)));

        // Valores límite de caja negra que resuelve esta misma validación.

        [Theory(DisplayName = "CN06/CN07 - límites del descuento")]
        [InlineData(-0.01, DescuentoFueraDeRango)]
        [InlineData(0, null)]
        [InlineData(30, null)]
        [InlineData(30.01, DescuentoFueraDeRango)]
        public void LimitesDelDescuento(double descuento, string? esperado) =>
            Assert.Equal(esperado, FacadeVentas.ValidarSolicitud(Solicitud(UnLibro(1), descuento: (decimal)descuento)));

        [Fact(DisplayName = "CN10 - cantidad cero")]
        public void CN10_CantidadCero() =>
            Assert.Equal(CantidadInvalida, FacadeVentas.ValidarSolicitud(Solicitud(UnLibro(0))));

        [Fact(DisplayName = "CN13 - carrito vacío")]
        public void CN13_CarritoVacio() =>
            Assert.Equal(CarritoVacio, FacadeVentas.ValidarSolicitud(Solicitud(Array.Empty<ItemVentaSolicitud>())));
    }
}
