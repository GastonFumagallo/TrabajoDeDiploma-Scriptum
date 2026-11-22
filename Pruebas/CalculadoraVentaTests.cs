using Controladora;
using Modelo;
using Xunit;

namespace Pruebas
{
    /// <summary>
    /// Pruebas de caja negra de los importes de una venta (<see cref="CalculadoraVenta"/> y las estrategias de
    /// medio de pago). Libro A: precio $ 10.000,00. Ver la sección "Pruebas" de la Entrega Final.
    /// </summary>
    public class CalculadoraVentaTests
    {
        private const decimal PrecioLibroA = 10_000m;

        private static MetodoPago Medio(string nombre) => new() { MP_ID = 1, MP_Nombre = nombre, MP_Estado = true };

        [Fact(DisplayName = "CN01 - 2 × Libro A en efectivo, recibido $ 20.000")]
        public void CN01_EfectivoExacto()
        {
            var c = CalculadoraVenta.Calcular(2 * PrecioLibroA, 0m, Medio("Efectivo"), 20_000m, 0m);
            Assert.Equal(20_000m, c.Total);
            Assert.Equal(0m, c.Vuelto);
            Assert.False(c.PagoInsuficiente);
        }

        [Fact(DisplayName = "CN02 - descuento 10 % con tarjeta de crédito (+10 %)")]
        public void CN02_Credito()
        {
            var c = CalculadoraVenta.Calcular(PrecioLibroA, 10m, Medio("Tarjeta de credito"), null, 0m);
            Assert.Equal(1_000m, c.Descuento);
            Assert.Equal(9_900m, c.Total);
            Assert.Equal(900m, c.AjusteMedioPago);
        }

        [Fact(DisplayName = "CN03 - tarjeta de débito (−5 %)")]
        public void CN03_Debito() =>
            Assert.Equal(9_500m, CalculadoraVenta.Calcular(PrecioLibroA, 0m, Medio("Tarjeta de debito"), null, 0m).Total);

        [Fact(DisplayName = "CN04 - transferencia (−2 %)")]
        public void CN04_Transferencia() =>
            Assert.Equal(9_800m, CalculadoraVenta.Calcular(PrecioLibroA, 0m, Medio("Transferencia"), null, 0m).Total);

        [Fact(DisplayName = "CN05 - descuento 30 % (límite) en efectivo, recibido $ 7.000")]
        public void CN05_DescuentoMaximo()
        {
            var c = CalculadoraVenta.Calcular(PrecioLibroA, 30m, Medio("Efectivo"), 7_000m, 0m);
            Assert.Equal(7_000m, c.Total);
            Assert.Equal(0m, c.Vuelto);
            Assert.False(c.PagoInsuficiente);
        }

        [Fact(DisplayName = "CN11 - efectivo, recibido $ 9.999,99 (un centavo menos)")]
        public void CN11_PagoInsuficiente() =>
            Assert.True(CalculadoraVenta.Calcular(PrecioLibroA, 0m, Medio("Efectivo"), 9_999.99m, 0m).PagoInsuficiente);

        [Fact(DisplayName = "CN12 - efectivo, recibido $ 10.000,01 (un centavo más)")]
        public void CN12_VueltoDeUnCentavo()
        {
            var c = CalculadoraVenta.Calcular(PrecioLibroA, 0m, Medio("Efectivo"), 10_000.01m, 0m);
            Assert.False(c.PagoInsuficiente);
            Assert.Equal(0.01m, c.Vuelto);
        }

        [Theory(DisplayName = "Estrategia según el nombre del medio de pago (con y sin tilde)")]
        [InlineData("Efectivo", 10_000)]
        [InlineData("Tarjeta de debito", 9_500)]
        [InlineData("Tarjeta de débito", 9_500)]
        [InlineData("Tarjeta de credito", 11_000)]
        [InlineData("Tarjeta de crédito", 11_000)]
        [InlineData("Transferencia", 9_800)]
        public void EstrategiaPorMedioDePago(string nombre, int totalEsperado) =>
            Assert.Equal(totalEsperado, CalculadoraVenta.Calcular(PrecioLibroA, 0m, Medio(nombre), null, 0m).Total);
    }
}
