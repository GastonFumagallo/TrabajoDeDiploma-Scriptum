using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora.Abm
{
    /// <summary>
    /// ABM de libros. Plantilla de referencia para los demás maestros (ver GUIA-ABM.md):
    /// DbContext propio por operación, lecturas AsNoTracking con proyección a DTO, validación completa en el
    /// servicio (la UI valida antes sólo para dar feedback inmediato), baja lógica y concurrencia optimista.
    /// </summary>
    public sealed class LibroService : ServicioAbmBase, ILibroService
    {
        private static LibroService? instancia;
        public static LibroService Instancia => instancia ??= new LibroService();
        private LibroService() { }

        protected override string NombreEntidad => "el libro";

        #region Lecturas

        public async Task<List<LibroListadoDTO>> ObtenerTodosAsync(FiltroLibros filtro, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            var query = db.Libros.AsNoTracking();

            query = filtro.Estado switch
            {
                FiltroEstadoActivo.Activos => query.Where(l => l.LIB_Activo),
                FiltroEstadoActivo.Inactivos => query.Where(l => !l.LIB_Activo),
                _ => query,
            };

            if (!string.IsNullOrWhiteSpace(filtro.Texto))
            {
                string texto = filtro.Texto.Trim();
                string? isbn = Libro.NormalizarISBN(texto);
                query = query.Where(l => l.LIB_Titulo.Contains(texto)
                                      || l.LIB_Autor.Contains(texto)
                                      || l.LIB_Editorial.Contains(texto)
                                      || (isbn != null && l.LIB_ISBN != null && l.LIB_ISBN.Contains(isbn)));
            }

            if (filtro.GeneroId is int generoId)
                query = query.Where(l => l.GEN_ID == generoId);

            return await query
                .OrderBy(l => l.LIB_Titulo)
                .Select(l => new LibroListadoDTO
                {
                    Id = l.LIB_ID,
                    ISBN = l.LIB_ISBN,
                    Titulo = l.LIB_Titulo,
                    Autor = l.LIB_Autor,
                    Editorial = l.LIB_Editorial,
                    Genero = l.LIB_Genero.GEN_Nombre,
                    PrecioCosto = l.LIB_PrecioCosto,
                    PrecioVenta = l.LIB_PrecioVenta,
                    Stock = l.LIB_Stock,
                    StockMinimo = l.LIB_StockMinimo,
                    Proveedores = l.LIB_Proveedores.Count(),
                    Activo = l.LIB_Activo,
                })
                .ToListAsync(ct);
        }

        public async Task<LibroEdicionDTO?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await db.Libros.AsNoTracking()
                .Where(l => l.LIB_ID == id)
                .Select(l => new LibroEdicionDTO
                {
                    Id = l.LIB_ID,
                    ISBN = l.LIB_ISBN,
                    Titulo = l.LIB_Titulo,
                    Autor = l.LIB_Autor,
                    Editorial = l.LIB_Editorial,
                    Descripcion = l.LIB_Descripcion,
                    AnioPublicacion = l.LIB_AñoPublicacion,
                    GeneroId = l.GEN_ID,
                    PrecioCosto = l.LIB_PrecioCosto,
                    PrecioVenta = l.LIB_PrecioVenta,
                    Stock = l.LIB_Stock,
                    StockMinimo = l.LIB_StockMinimo,
                    PuntoReposicion = l.LIB_PuntoReposicion,
                    StockOptimo = l.LIB_StockOptimo,
                    Activo = l.LIB_Activo,
                    Version = l.LIB_Version,
                    Proveedores = l.LIB_Proveedores
                        .OrderBy(pl => pl.PL_PrecioCompra)
                        .Select(pl => new ProveedorPrecioDTO
                        {
                            ProveedorId = pl.PROV_ID,
                            Proveedor = pl.PL_Proveedor.PROV_Empresa != "" ? pl.PL_Proveedor.PROV_Empresa : pl.PL_Proveedor.PER_Proveedor.PER_Nombre,
                            PrecioCompra = pl.PL_PrecioCompra,
                        }).ToList(),
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<bool> ExisteIdentificadorAsync(string identificador, int? excluirId = null, CancellationToken ct = default)
        {
            string? isbn = Libro.NormalizarISBN(identificador);
            if (isbn == null) return false;

            await using var db = new Libreria();
            return await db.Libros.AnyAsync(l => l.LIB_ISBN == isbn && l.LIB_ID != excluirId, ct);
        }

        /// <summary>Libros activos para el punto de venta (los inactivos no se venden).</summary>
        public async Task<List<LibroCatalogoDTO>> ObtenerCatalogoVentaAsync(CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await db.Libros.AsNoTracking()
                .Where(l => l.LIB_Activo)
                .OrderBy(l => l.LIB_Titulo)
                .Select(l => new LibroCatalogoDTO
                {
                    Id = l.LIB_ID,
                    ISBN = l.LIB_ISBN,
                    Titulo = l.LIB_Titulo,
                    Autor = l.LIB_Autor,
                    Editorial = l.LIB_Editorial,
                    Precio = l.LIB_PrecioVenta,
                    Stock = l.LIB_Stock,
                })
                .ToListAsync(ct);
        }

        public async Task<List<OpcionDTO>> ObtenerGenerosAsync(CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await db.Generos.AsNoTracking()
                .OrderBy(g => g.GEN_Nombre)
                .Select(g => new OpcionDTO(g.GEN_ID, g.GEN_Nombre))
                .ToListAsync(ct);
        }

        /// <summary>Proveedores activos para asociar al libro.</summary>
        public Task<List<OpcionDTO>> ObtenerProveedoresAsync(CancellationToken ct = default) =>
            ProveedorService.Instancia.ObtenerOpcionesAsync(ct);

        #endregion

        #region Escrituras

        public async Task<int> GuardarAsync(LibroEdicionDTO dto, string usuario, CancellationToken ct = default)
        {
            Normalizar(dto);

            await using var db = new Libreria();
            await ValidarAsync(db, dto, ct);

            await using var tx = await db.Database.BeginTransactionAsync(ct);
            try
            {
                Libro libro;
                if (dto.Id is int id)
                {
                    libro = await db.Libros.Include(l => l.LIB_Proveedores).FirstOrDefaultAsync(l => l.LIB_ID == id, ct)
                            ?? throw new ConcurrenciaException("El libro ya no existe: otro usuario lo eliminó.");

                    // Concurrencia optimista: EF agrega "WHERE LIB_Version = <la que se leyó al abrir la ficha>".
                    db.Entry(libro).Property(l => l.LIB_Version).OriginalValue = dto.Version;
                    libro.LIB_Version = dto.Version + 1;
                    // El stock NO se modifica desde la ficha (se ajusta en Inventario, con auditoría).
                }
                else
                {
                    libro = new Libro { LIB_Stock = dto.Stock, LIB_Activo = true };
                    db.Libros.Add(libro);

                    if (dto.Stock > 0)
                    {
                        db.MovimientosStock.Add(new MovimientoStock
                        {
                            MOV_Libro = libro,
                            MOV_Fecha = DateTime.Now,
                            MOV_Tipo = TipoMovimientoStock.AltaInicial,
                            MOV_Cantidad = dto.Stock,
                            MOV_StockAnterior = 0,
                            MOV_StockResultante = dto.Stock,
                            MOV_Motivo = "Alta del libro",
                            MOV_Usuario = usuario,
                        });
                    }
                }

                libro.LIB_ISBN = dto.ISBN;
                libro.LIB_Titulo = dto.Titulo;
                libro.LIB_Autor = dto.Autor;
                libro.LIB_Editorial = dto.Editorial;
                libro.LIB_Descripcion = dto.Descripcion ?? string.Empty;
                libro.LIB_AñoPublicacion = dto.AnioPublicacion;
                libro.GEN_ID = dto.GeneroId;
                libro.LIB_PrecioVenta = dto.PrecioVenta;
                // Si no se informó costo, se toma el menor precio pactado con los proveedores.
                libro.LIB_PrecioCosto = dto.PrecioCosto > 0 || dto.Proveedores.Count == 0
                    ? dto.PrecioCosto
                    : dto.Proveedores.Min(p => p.PrecioCompra);
                libro.LIB_StockMinimo = dto.StockMinimo;
                libro.LIB_PuntoReposicion = dto.PuntoReposicion;
                libro.LIB_StockOptimo = dto.StockOptimo;

                SincronizarProveedores(db, libro, dto.Proveedores);

                await GuardarCambiosAsync(db, ct, campoUnico: nameof(LibroEdicionDTO.ISBN), etiquetaUnico: "ISBN");
                await tx.CommitAsync(ct);
                return libro.LIB_ID;
            }
            catch
            {
                await InventarioService.RollbackSeguroAsync(tx);
                throw;
            }
        }

        public async Task CambiarEstadoAsync(int id, bool activo, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            int filas = await db.Libros
                .Where(l => l.LIB_ID == id)
                .ExecuteUpdateAsync(s => s.SetProperty(l => l.LIB_Activo, activo), ct);
            if (filas == 0)
                throw new ConcurrenciaException("El libro ya no existe.");
        }

        /// <summary>Reactiva un libro inactivo y devuelve su ficha para revisarla (flujo "ya existe pero está inactivo").</summary>
        public async Task<LibroEdicionDTO?> ReactivarAsync(int id, CancellationToken ct = default)
        {
            await CambiarEstadoAsync(id, true, ct);
            return await ObtenerPorIdAsync(id, ct);
        }

        #endregion

        #region Validación y helpers

        private static void Normalizar(LibroEdicionDTO dto)
        {
            dto.ISBN = Libro.NormalizarISBN(dto.ISBN);
            dto.Titulo = dto.Titulo?.Trim() ?? string.Empty;
            dto.Autor = dto.Autor?.Trim() ?? string.Empty;
            dto.Editorial = dto.Editorial?.Trim() ?? string.Empty;
            dto.Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim();
            dto.PrecioCosto = Math.Round(dto.PrecioCosto, 2);
            dto.PrecioVenta = Math.Round(dto.PrecioVenta, 2);
        }

        /// <summary>
        /// Reglas de negocio completas. La UI hace las mismas validaciones de formato para dar feedback inmediato,
        /// pero ésta es la que vale: ningún dato inválido llega a la base aunque la UI tenga un error.
        /// </summary>
        private static async Task ValidarAsync(Libreria db, LibroEdicionDTO dto, CancellationToken ct)
        {
            var e = new Errores();
            e.Requerido(dto.Titulo, nameof(dto.Titulo), "El título", 200);
            e.Requerido(dto.Autor, nameof(dto.Autor), "El autor", 150);
            e.Requerido(dto.Editorial, nameof(dto.Editorial), "La editorial", 150);
            e.Si(dto.ISBN != null && !Identificadores.EsISBNValido(dto.ISBN), nameof(dto.ISBN),
                "El ISBN no es válido (debe tener 10 o 13 dígitos con dígito verificador correcto).");
            e.Si(dto.AnioPublicacion != 0 && (dto.AnioPublicacion < 1450 || dto.AnioPublicacion > DateTime.Today.Year + 1),
                nameof(dto.AnioPublicacion), "El año de publicación no es válido.");
            e.Si(dto.PrecioVenta <= 0, nameof(dto.PrecioVenta), "El precio de venta debe ser mayor a 0.");
            e.Si(dto.PrecioCosto < 0, nameof(dto.PrecioCosto), "El precio de costo no puede ser negativo.");
            e.Si(dto.PrecioCosto > 0 && dto.PrecioVenta <= dto.PrecioCosto, nameof(dto.PrecioVenta),
                "El precio de venta debe ser mayor al de costo (margen positivo).");
            e.Si(dto.Id == null && dto.Stock < 0, nameof(dto.Stock), "El stock inicial no puede ser negativo.");
            e.Si(dto.StockMinimo < 0, nameof(dto.StockMinimo), "El stock mínimo no puede ser negativo.");
            e.Si(dto.PuntoReposicion < dto.StockMinimo, nameof(dto.PuntoReposicion), "El punto de reposición no puede ser menor que el mínimo.");
            e.Si(dto.StockOptimo < dto.PuntoReposicion || dto.StockOptimo <= 0, nameof(dto.StockOptimo),
                "El stock óptimo debe ser mayor a 0 y no menor que el punto de reposición.");
            e.Si(dto.Proveedores.GroupBy(p => p.ProveedorId).Any(g => g.Count() > 1), nameof(dto.Proveedores), "Hay proveedores repetidos.");
            e.Si(dto.Proveedores.Any(p => p.PrecioCompra <= 0), nameof(dto.Proveedores), "El precio de compra de cada proveedor debe ser mayor a 0.");

            if (!await db.Generos.AnyAsync(g => g.GEN_ID == dto.GeneroId, ct))
                e.Si(true, nameof(dto.GeneroId), "Seleccione un género válido.");

            // Unicidad del ISBN. Si choca con un libro inactivo, en un alta se ofrece reactivarlo.
            if (dto.ISBN != null && !e.HayErrores)
            {
                var existente = await db.Libros.AsNoTracking()
                    .Where(l => l.LIB_ISBN == dto.ISBN && l.LIB_ID != dto.Id)
                    .Select(l => new { l.LIB_ID, l.LIB_Titulo, l.LIB_Activo })
                    .FirstOrDefaultAsync(ct);

                if (existente != null)
                {
                    if (!existente.LIB_Activo && dto.Id == null)
                        throw new EntidadInactivaException(existente.LIB_ID,
                            $"Ya existe un libro dado de baja con ese ISBN: \"{existente.LIB_Titulo}\".");
                    e.Si(true, nameof(dto.ISBN), $"El ISBN ya está asignado a \"{existente.LIB_Titulo}\"" +
                                                 (existente.LIB_Activo ? "." : " (inactivo)."));
                }
            }

            e.Lanzar();
        }

        /// <summary>Deja las asociaciones libro-proveedor exactamente como vienen en el DTO (alta, baja y cambio de precio).</summary>
        private static void SincronizarProveedores(Libreria db, Libro libro, List<ProveedorPrecioDTO> deseados)
        {
            var porId = deseados.ToDictionary(p => p.ProveedorId);

            foreach (var actual in libro.LIB_Proveedores.ToList())
            {
                if (!porId.TryGetValue(actual.PROV_ID, out var deseado))
                    db.ProveedoresLibros.Remove(actual);
                else if (actual.PL_PrecioCompra != deseado.PrecioCompra)
                    actual.PL_PrecioCompra = Math.Round(deseado.PrecioCompra, 2);
            }

            var existentes = libro.LIB_Proveedores.Select(pl => pl.PROV_ID).ToHashSet();
            foreach (var nuevo in deseados.Where(p => !existentes.Contains(p.ProveedorId)))
            {
                libro.LIB_Proveedores.Add(new ProveedorLibro
                {
                    PROV_ID = nuevo.ProveedorId,
                    PL_Libro = libro,
                    PL_PrecioCompra = Math.Round(nuevo.PrecioCompra, 2),
                });
            }
        }

        #endregion
    }
}
