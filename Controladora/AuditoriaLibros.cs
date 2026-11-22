using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using Servicios;
using System.Globalization;

namespace Controladora
{
    /// <summary>
    /// Captura la trazabilidad de los libros. Cada operación toma una <see cref="Instantanea"/> del libro antes y
    /// después del cambio y <see cref="Comparar"/> genera una fila por campo modificado, con el valor original y el
    /// nuevo. Las filas se agregan al mismo DbContext de la operación, así se guardan en la misma transacción:
    /// o se registra el cambio y su auditoría, o ninguno de los dos.
    /// </summary>
    public static class AuditoriaLibros
    {
        private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("es-AR");

        /// <summary>Campos auditados, en el orden en que se muestran.</summary>
        public static readonly IReadOnlyList<string> Campos = new[]
        {
            "ISBN", "Título", "Autor", "Editorial", "Género", "Año de publicación", "Descripción",
            "Precio de costo", "Precio de venta", "Stock mínimo", "Punto de reposición", "Stock óptimo",
            "Proveedores", "Estado",
        };

        /// <summary>Usuario de la sesión, para las operaciones que no lo reciben como parámetro.</summary>
        public static string UsuarioActual => PermisoService.Instancia.UsuarioActual?.USU_Nombre ?? "sistema";

        /// <summary>Valores del libro como texto legible, por campo auditado.</summary>
        public static Dictionary<string, string?> Instantanea(Libro libro, string? genero, string? proveedores) => new()
        {
            ["ISBN"] = Vacio(libro.LIB_ISBN),
            ["Título"] = Vacio(libro.LIB_Titulo),
            ["Autor"] = Vacio(libro.LIB_Autor),
            ["Editorial"] = Vacio(libro.LIB_Editorial),
            ["Género"] = Vacio(genero),
            ["Año de publicación"] = libro.LIB_AñoPublicacion > 0 ? libro.LIB_AñoPublicacion.ToString(Cultura) : null,
            ["Descripción"] = Recortar(Vacio(libro.LIB_Descripcion)),
            ["Precio de costo"] = Moneda(libro.LIB_PrecioCosto),
            ["Precio de venta"] = Moneda(libro.LIB_PrecioVenta),
            ["Stock mínimo"] = libro.LIB_StockMinimo.ToString(Cultura),
            ["Punto de reposición"] = libro.LIB_PuntoReposicion.ToString(Cultura),
            ["Stock óptimo"] = libro.LIB_StockOptimo.ToString(Cultura),
            ["Proveedores"] = Vacio(proveedores),
            ["Estado"] = libro.LIB_Activo ? "Activo" : "Inactivo",
        };

        /// <summary>
        /// Una fila por cada campo cuyo valor cambió. <paramref name="antes"/> null = alta (se registran todos los
        /// campos con valor, sin valor anterior).
        /// </summary>
        public static List<AuditoriaLibro> Comparar(IReadOnlyDictionary<string, string?>? antes, IReadOnlyDictionary<string, string?> despues,
            string operacion, string usuario, DateTime fecha, string? referencia = null)
        {
            var lote = Guid.NewGuid();
            var filas = new List<AuditoriaLibro>();
            foreach (var campo in Campos)
            {
                despues.TryGetValue(campo, out var nuevo);
                string? anterior = null;
                if (antes != null) antes.TryGetValue(campo, out anterior);

                bool cambio = antes == null ? nuevo != null : !string.Equals(anterior, nuevo, StringComparison.Ordinal);
                if (!cambio) continue;

                filas.Add(new AuditoriaLibro
                {
                    AUL_Fecha = fecha,
                    AUL_Usuario = Recortar(usuario, 100)!,
                    AUL_Operacion = operacion,
                    AUL_Campo = campo,
                    AUL_ValorAnterior = anterior,
                    AUL_ValorNuevo = nuevo,
                    AUL_Lote = lote,
                    AUL_Referencia = referencia,
                });
            }
            return filas;
        }

        /// <summary>Agrega al contexto las filas de auditoría de <paramref name="libro"/> (sirve también para un alta sin ID todavía).</summary>
        public static void Registrar(Libreria db, Libro libro, IEnumerable<AuditoriaLibro> filas)
        {
            foreach (var f in filas)
            {
                if (libro.LIB_ID > 0) f.LIB_ID = libro.LIB_ID; else f.AUL_Libro = libro;
                db.AuditoriaLibros.Add(f);
            }
        }

        /// <summary>Texto de los proveedores de un libro: "Editorial X ($ 1.200,00) · Distribuidora Y ($ 1.150,00)".</summary>
        public static async Task<string?> DescribirProveedoresAsync(Libreria db, IEnumerable<(int ProveedorId, decimal Precio)> proveedores, CancellationToken ct)
        {
            var lista = proveedores.ToList();
            if (lista.Count == 0) return null;
            var ids = lista.Select(p => p.ProveedorId).Distinct().ToList();
            var nombres = await db.Proveedores.AsNoTracking()
                .Where(p => ids.Contains(p.PROV_ID))
                .Select(p => new { p.PROV_ID, Nombre = p.PROV_Empresa != "" ? p.PROV_Empresa : p.PER_Proveedor.PER_Nombre })
                .ToDictionaryAsync(p => p.PROV_ID, p => p.Nombre, ct);
            return string.Join(" · ", lista
                .Select(p => $"{(nombres.TryGetValue(p.ProveedorId, out var n) ? n : $"Proveedor {p.ProveedorId}")} ({Moneda(p.Precio)})")
                .OrderBy(t => t, StringComparer.CurrentCulture));
        }

        public static async Task<string?> NombreGeneroAsync(Libreria db, int generoId, CancellationToken ct) =>
            await db.Generos.AsNoTracking().Where(g => g.GEN_ID == generoId).Select(g => g.GEN_Nombre).FirstOrDefaultAsync(ct);

        /// <summary>Instantánea de un libro leído de la base, con su género y sus proveedores.</summary>
        public static async Task<Dictionary<string, string?>> InstantaneaAsync(Libreria db, Libro libro, CancellationToken ct)
        {
            var genero = await NombreGeneroAsync(db, libro.GEN_ID, ct);
            var provs = await db.ProveedoresLibros.AsNoTracking()
                .Where(pl => pl.LIB_ID == libro.LIB_ID)
                .Select(pl => new { pl.PROV_ID, pl.PL_PrecioCompra })
                .ToListAsync(ct);
            var texto = await DescribirProveedoresAsync(db, provs.Select(p => (p.PROV_ID, p.PL_PrecioCompra)), ct);
            return Instantanea(libro, genero, texto);
        }

        /// <summary>
        /// Estado original y actual de cada campo a partir del historial (ordenado del más antiguo al más reciente).
        /// Si el alta está registrada, el original es el valor del alta; si no (libros cargados antes de activar la
        /// auditoría), es el valor anterior del primer cambio del campo, o el actual si nunca cambió.
        /// </summary>
        public static List<EstadoCampoDTO> ReconstruirEstado(IReadOnlyList<CambioLibroDTO> historialAscendente,
            IReadOnlyDictionary<string, string?> actual, IEnumerable<string> campos)
        {
            bool altaRegistrada = historialAscendente.Any(c => c.Operacion == OperacionAuditoriaLibro.Alta);
            var resultado = new List<EstadoCampoDTO>();
            foreach (var campo in campos)
            {
                var delCampo = historialAscendente.Where(c => c.Campo == campo).ToList();
                actual.TryGetValue(campo, out var valorActual);
                string? original;
                if (altaRegistrada && campo != "Stock")
                    original = delCampo.FirstOrDefault(c => c.Operacion == OperacionAuditoriaLibro.Alta)?.ValorNuevo;
                else if (delCampo.Count == 0)
                    original = valorActual;
                else
                    // El stock con que se dio de alta el libro es su valor original (no el 0 anterior al alta).
                    original = delCampo[0].Operacion == OperacionStockAltaInicial ? delCampo[0].ValorNuevo : delCampo[0].ValorAnterior;

                resultado.Add(new EstadoCampoDTO
                {
                    Campo = campo,
                    Original = original,
                    Actual = valorActual,
                    Cambios = delCampo.Count(c => c.Operacion != OperacionAuditoriaLibro.Alta),
                });
            }
            return resultado;
        }

        /// <summary>Operación con que se muestra el movimiento de stock del alta en la trazabilidad.</summary>
        public static readonly string OperacionStockAltaInicial = "Stock: " + TipoMovimientoStock.Descripcion(TipoMovimientoStock.AltaInicial);

        public static string Moneda(decimal valor) => valor.ToString("C2", Cultura);

        private static string? Vacio(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        private static string? Recortar(string? s, int maximo = 500) => s == null || s.Length <= maximo ? s : s[..(maximo - 1)] + "…";
    }
}
