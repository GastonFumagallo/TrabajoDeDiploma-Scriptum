using Controladora;
using Modelo;
using Xunit;

namespace Pruebas
{
    /// <summary>Pruebas de la trazabilidad de libros y de las funciones auxiliares del resguardo.</summary>
    public class AuditoriaYResguardoTests
    {
        private static Libro LibroDePrueba() => new()
        {
            LIB_ISBN = "9789500000001", LIB_Titulo = "Rayuela", LIB_Autor = "Julio Cortázar", LIB_Editorial = "Alfaguara",
            LIB_Descripcion = "", LIB_AñoPublicacion = 1963, LIB_PrecioCosto = 8000m, LIB_PrecioVenta = 12000m,
            LIB_StockMinimo = 2, LIB_PuntoReposicion = 5, LIB_StockOptimo = 10, LIB_Activo = true,
        };

        [Fact(DisplayName = "Alta: registra todos los campos con valor y sin valor anterior")]
        public void Alta_RegistraCamposConValor()
        {
            var despues = AuditoriaLibros.Instantanea(LibroDePrueba(), "Novela", null);
            var filas = AuditoriaLibros.Comparar(null, despues, OperacionAuditoriaLibro.Alta, "admin", DateTime.Now);

            Assert.All(filas, f => Assert.Null(f.AUL_ValorAnterior));
            Assert.DoesNotContain(filas, f => f.AUL_Campo is "Descripción" or "Proveedores");   // vacíos: no se registran
            Assert.Contains(filas, f => f.AUL_Campo == "Género" && f.AUL_ValorNuevo == "Novela");
            Assert.Single(filas.Select(f => f.AUL_Lote).Distinct());
        }

        [Fact(DisplayName = "Modificación: solo los campos que cambiaron, con valor original y nuevo")]
        public void Modificacion_SoloCamposModificados()
        {
            var libro = LibroDePrueba();
            var antes = AuditoriaLibros.Instantanea(libro, "Novela", null);
            libro.LIB_PrecioVenta = 13500m;
            libro.LIB_StockMinimo = 3;
            var despues = AuditoriaLibros.Instantanea(libro, "Novela", null);

            var filas = AuditoriaLibros.Comparar(antes, despues, OperacionAuditoriaLibro.Modificacion, "encargado", DateTime.Now);

            Assert.Equal(2, filas.Count);
            var precio = Assert.Single(filas, f => f.AUL_Campo == "Precio de venta");
            Assert.Equal(AuditoriaLibros.Moneda(12000m), precio.AUL_ValorAnterior);
            Assert.Equal(AuditoriaLibros.Moneda(13500m), precio.AUL_ValorNuevo);
            Assert.Contains(filas, f => f.AUL_Campo == "Stock mínimo" && f.AUL_ValorAnterior == "2" && f.AUL_ValorNuevo == "3");
            Assert.All(filas, f => Assert.Equal("encargado", f.AUL_Usuario));
        }

        [Fact(DisplayName = "Sin cambios: no genera registros")]
        public void SinCambios_NoRegistra()
        {
            var foto = AuditoriaLibros.Instantanea(LibroDePrueba(), "Novela", "Editorial X ($ 8.000,00)");
            Assert.Empty(AuditoriaLibros.Comparar(foto, new Dictionary<string, string?>(foto), OperacionAuditoriaLibro.Modificacion, "admin", DateTime.Now));
        }

        [Fact(DisplayName = "Estado original con alta registrada: se toma el valor del alta")]
        public void EstadoOriginal_DesdeElAlta()
        {
            var t0 = new DateTime(2026, 1, 1);
            var historial = new List<CambioLibroDTO>
            {
                new() { Fecha = t0, Operacion = OperacionAuditoriaLibro.Alta, Campo = "Precio de venta", ValorNuevo = "$ 100,00" },
                new() { Fecha = t0.AddDays(1), Operacion = OperacionAuditoriaLibro.Modificacion, Campo = "Precio de venta", ValorAnterior = "$ 100,00", ValorNuevo = "$ 120,00" },
                new() { Fecha = t0.AddDays(2), Operacion = OperacionAuditoriaLibro.Modificacion, Campo = "Precio de venta", ValorAnterior = "$ 120,00", ValorNuevo = "$ 150,00" },
            };
            var actual = new Dictionary<string, string?> { ["Precio de venta"] = "$ 150,00", ["Autor"] = "X" };

            var estado = AuditoriaLibros.ReconstruirEstado(historial, actual, new[] { "Precio de venta", "Autor" });

            var precio = estado.Single(e => e.Campo == "Precio de venta");
            Assert.Equal("$ 100,00", precio.Original);
            Assert.Equal("$ 150,00", precio.Actual);
            Assert.Equal(2, precio.Cambios);
            var autor = estado.Single(e => e.Campo == "Autor");
            Assert.Null(autor.Original);          // el alta no registró autor: estaba vacío
            Assert.False(autor.Modificado);
        }

        [Fact(DisplayName = "Estado original sin alta registrada: valor anterior al primer cambio")]
        public void EstadoOriginal_Reconstruido()
        {
            var t0 = new DateTime(2026, 1, 1);
            var historial = new List<CambioLibroDTO>
            {
                new() { Fecha = t0, Operacion = OperacionAuditoriaLibro.Modificacion, Campo = "Título", ValorAnterior = "Rayuel", ValorNuevo = "Rayuela" },
                new() { Fecha = t0.AddDays(1), Operacion = "Stock: Venta", Campo = "Stock", ValorAnterior = "10", ValorNuevo = "8" },
            };
            var actual = new Dictionary<string, string?> { ["Título"] = "Rayuela", ["Stock"] = "8", ["Autor"] = "Cortázar" };

            var estado = AuditoriaLibros.ReconstruirEstado(historial, actual, new[] { "Título", "Autor", "Stock" });

            Assert.Equal("Rayuel", estado.Single(e => e.Campo == "Título").Original);
            Assert.Equal("Cortázar", estado.Single(e => e.Campo == "Autor").Original);   // nunca cambió
            Assert.Equal("10", estado.Single(e => e.Campo == "Stock").Original);
        }

        [Fact(DisplayName = "Stock original: el del alta inicial, no el cero anterior")]
        public void StockOriginal_DesdeAltaInicial()
        {
            var t0 = new DateTime(2026, 1, 1);
            var historial = new List<CambioLibroDTO>
            {
                new() { Fecha = t0, Operacion = AuditoriaLibros.OperacionStockAltaInicial, Campo = "Stock", ValorAnterior = "0", ValorNuevo = "10" },
                new() { Fecha = t0.AddDays(1), Operacion = "Stock: Venta", Campo = "Stock", ValorAnterior = "10", ValorNuevo = "8" },
            };
            var estado = AuditoriaLibros.ReconstruirEstado(historial, new Dictionary<string, string?> { ["Stock"] = "8" }, new[] { "Stock" });
            Assert.Equal("10", estado.Single().Original);
        }

        [Theory(DisplayName = "Usuario del resguardo leído de la descripción")]
        [InlineData("SCRIPTUM · usuario: admin", "admin")]
        [InlineData("SCRIPTUM · usuario: encargado · antes de actualizar precios", "encargado")]
        [InlineData("Resguardo hecho con SSMS", null)]
        public void UsuarioDeDescripcion(string descripcion, string? esperado) =>
            Assert.Equal(esperado, ResguardoService.UsuarioDeDescripcion(descripcion));

        [Fact(DisplayName = "Nombre del archivo de resguardo con base, fecha y hora")]
        public void NombreArchivo() =>
            Assert.Equal("Libreria_20261118_093005.bak", ResguardoService.NombreArchivo("Libreria", new DateTime(2026, 11, 18, 9, 30, 5)));
    }
}
