using Modelo;
using Xunit;

namespace Pruebas
{
    /// <summary>
    /// Caja negra del semáforo de stock que decide qué libros se reponen (iteración 2).
    /// Libro con stock mínimo 2 y punto de reposición 5: valores límite de cada clase.
    /// </summary>
    public class EstadoStockTests
    {
        [Theory(DisplayName = "Estado del stock según mínimo 2 y punto de reposición 5")]
        [InlineData(-1, EstadoStock.Agotado)]
        [InlineData(0, EstadoStock.Agotado)]
        [InlineData(1, EstadoStock.Critico)]
        [InlineData(2, EstadoStock.Critico)]
        [InlineData(3, EstadoStock.AReponer)]
        [InlineData(5, EstadoStock.AReponer)]
        [InlineData(6, EstadoStock.Normal)]
        public void EstadoSegunStock(int stock, EstadoStock esperado) =>
            Assert.Equal(esperado, ProductoInventarioDTO.Calcular(stock, 2, 5));
    }
}
