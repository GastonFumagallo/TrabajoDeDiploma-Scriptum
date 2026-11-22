using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using Servicios;

namespace Controladora
{
    /// <summary>
    /// Reportes de auditoría: inicios y cierres de sesión, eventos de seguridad y trazabilidad de libros.
    /// Sólo lectura. Todas las consultas exigen el permiso <see cref="PermisoVer"/>.
    /// </summary>
    public sealed class AuditoriaService
    {
        public const string PermisoVer = "VerAuditoria";
        public const string PermisoExportar = "ExportarAuditoria";

        private static AuditoriaService? instancia;
        public static AuditoriaService Instancia => instancia ??= new AuditoriaService();
        private AuditoriaService() { }

        private static (DateTime Desde, DateTime HastaExclusivo) Rango(FiltroAuditoria f) => (f.Desde.Date, f.Hasta.Date.AddDays(1));

        public async Task<List<string>> ObtenerUsuariosAsync(CancellationToken ct = default)
        {
            PermisoService.Instancia.Exigir(PermisoVer);
            await using var db = new Libreria();
            return await db.Usuarios.AsNoTracking().Select(u => u.USU_Nombre).OrderBy(n => n).ToListAsync(ct);
        }

        #region Sesiones

        public async Task<List<SesionAuditoriaDTO>> ObtenerSesionesAsync(FiltroAuditoria f, CancellationToken ct = default)
        {
            PermisoService.Instancia.Exigir(PermisoVer);
            var (desde, hasta) = Rango(f);
            await using var db = new Libreria();
            var q = db.AuditoriaSesiones.AsNoTracking()
                .Where(a => a.AS_FechaHoraLogin >= desde && a.AS_FechaHoraLogin < hasta);
            if (!string.IsNullOrWhiteSpace(f.Usuario))
                q = q.Where(a => a.AS_Usuario.USU_Nombre == f.Usuario);

            var filas = await q.OrderByDescending(a => a.AS_FechaHoraLogin).Take(f.Maximo)
                .Select(a => new
                {
                    a.AS_ID, Usuario = a.AS_Usuario.USU_Nombre, a.AS_FechaHoraLogin, a.AS_FechaHoraLogout,
                    a.AS_TiempoSesion, a.AS_TipoLogout, a.AS_SesionActiva,
                })
                .ToListAsync(ct);

            return filas.Select(a => new SesionAuditoriaDTO
            {
                Id = a.AS_ID,
                Usuario = a.Usuario,
                Inicio = a.AS_FechaHoraLogin,
                Fin = a.AS_FechaHoraLogout,
                Duracion = a.AS_TiempoSesion is int s ? FormatearDuracion(TimeSpan.FromSeconds(s)) : a.AS_SesionActiva ? "En curso" : "—",
                Cierre = a.AS_SesionActiva ? "Sesión activa" : a.AS_TipoLogout ?? "—",
                Activa = a.AS_SesionActiva,
            }).ToList();
        }

        public static string FormatearDuracion(TimeSpan t) =>
            t.TotalHours >= 1 ? $"{(int)t.TotalHours} h {t.Minutes:00} min" : t.TotalMinutes >= 1 ? $"{t.Minutes} min {t.Seconds:00} s" : $"{t.Seconds} s";

        #endregion

        #region Eventos de seguridad

        public async Task<List<string>> ObtenerTiposEventoAsync(CancellationToken ct = default)
        {
            PermisoService.Instancia.Exigir(PermisoVer);
            await using var db = new Libreria();
            return await db.AuditoriaSeguridad.AsNoTracking().Select(a => a.AUD_Evento).Distinct().OrderBy(e => e).ToListAsync(ct);
        }

        public async Task<List<EventoSeguridadDTO>> ObtenerEventosAsync(FiltroAuditoria f, CancellationToken ct = default)
        {
            PermisoService.Instancia.Exigir(PermisoVer);
            var (desde, hasta) = Rango(f);
            await using var db = new Libreria();
            var q = db.AuditoriaSeguridad.AsNoTracking().Where(a => a.AUD_Fecha >= desde && a.AUD_Fecha < hasta);
            if (!string.IsNullOrWhiteSpace(f.Usuario)) q = q.Where(a => a.AUD_Usuario == f.Usuario);
            if (!string.IsNullOrWhiteSpace(f.Operacion)) q = q.Where(a => a.AUD_Evento == f.Operacion);
            return await q.OrderByDescending(a => a.AUD_Fecha).Take(f.Maximo)
                .Select(a => new EventoSeguridadDTO
                {
                    Fecha = a.AUD_Fecha,
                    Evento = a.AUD_Evento,
                    Usuario = a.AUD_Usuario ?? "",
                    Equipo = a.AUD_Equipo ?? "",
                    Detalle = a.AUD_Detalle ?? "",
                })
                .ToListAsync(ct);
        }

        #endregion

        #region Trazabilidad de libros

        public static IReadOnlyList<string> Operaciones { get; } = new[]
        {
            OperacionAuditoriaLibro.Alta, OperacionAuditoriaLibro.Modificacion, OperacionAuditoriaLibro.Baja,
            OperacionAuditoriaLibro.Reactivacion, OperacionAuditoriaLibro.ParametrosStock, OperacionAuditoriaLibro.RecepcionOrden,
        };

        /// <summary>Cambios de datos de libros (sin los movimientos de stock, que se ven en la trazabilidad de cada libro).</summary>
        public async Task<List<CambioLibroDTO>> ObtenerCambiosLibrosAsync(FiltroAuditoria f, CancellationToken ct = default)
        {
            PermisoService.Instancia.Exigir(PermisoVer);
            var (desde, hasta) = Rango(f);
            await using var db = new Libreria();
            var q = db.AuditoriaLibros.AsNoTracking().Where(a => a.AUL_Fecha >= desde && a.AUL_Fecha < hasta);
            if (!string.IsNullOrWhiteSpace(f.Usuario)) q = q.Where(a => a.AUL_Usuario == f.Usuario);
            if (!string.IsNullOrWhiteSpace(f.Operacion)) q = q.Where(a => a.AUL_Operacion == f.Operacion);
            if (!string.IsNullOrWhiteSpace(f.Texto))
            {
                var t = f.Texto.Trim();
                q = q.Where(a => a.AUL_Libro.LIB_Titulo.Contains(t) || (a.AUL_Libro.LIB_ISBN != null && a.AUL_Libro.LIB_ISBN.Contains(t)));
            }
            return await q.OrderByDescending(a => a.AUL_Fecha).ThenBy(a => a.AUL_ID).Take(f.Maximo)
                .Select(a => new CambioLibroDTO
                {
                    LibroId = a.LIB_ID,
                    ISBN = a.AUL_Libro.LIB_ISBN ?? "",
                    Titulo = a.AUL_Libro.LIB_Titulo,
                    Fecha = a.AUL_Fecha,
                    Usuario = a.AUL_Usuario,
                    Operacion = a.AUL_Operacion,
                    Campo = a.AUL_Campo,
                    ValorAnterior = a.AUL_ValorAnterior,
                    ValorNuevo = a.AUL_ValorNuevo,
                    Referencia = a.AUL_Referencia,
                })
                .ToListAsync(ct);
        }

        /// <summary>Títulos e ISBN de todos los libros (activos e inactivos), para autocompletar el filtro.</summary>
        public async Task<List<string>> ObtenerSugerenciasLibrosAsync(CancellationToken ct = default)
        {
            PermisoService.Instancia.Exigir(PermisoVer);
            await using var db = new Libreria();
            var libros = await db.Libros.AsNoTracking().Select(l => new { l.LIB_Titulo, l.LIB_ISBN }).ToListAsync(ct);
            return libros.Select(l => l.LIB_Titulo)
                .Concat(libros.Where(l => !string.IsNullOrEmpty(l.LIB_ISBN)).Select(l => l.LIB_ISBN!))
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .OrderBy(t => t, StringComparer.CurrentCulture)
                .ToList();
        }

        public async Task<List<LibroAuditableDTO>> BuscarLibrosAsync(string texto, CancellationToken ct = default)
        {
            PermisoService.Instancia.Exigir(PermisoVer);
            texto = texto?.Trim() ?? "";
            await using var db = new Libreria();
            var q = db.Libros.AsNoTracking();
            if (texto.Length > 0) q = q.Where(l => l.LIB_Titulo.Contains(texto) || (l.LIB_ISBN != null && l.LIB_ISBN.Contains(texto)) || l.LIB_Autor.Contains(texto));
            return await q.OrderBy(l => l.LIB_Titulo).Take(200)
                .Select(l => new LibroAuditableDTO { Id = l.LIB_ID, Texto = l.LIB_Titulo + (l.LIB_ISBN != null ? " — " + l.LIB_ISBN : "") + (l.LIB_Activo ? "" : " (inactivo)") })
                .ToListAsync(ct);
        }

        /// <summary>
        /// Trazabilidad completa de un libro: estado original, estado actual y todas sus transformaciones (cambios de
        /// datos más movimientos de stock), con usuario, fecha y valores anteriores y nuevos.
        /// </summary>
        public async Task<TrazabilidadLibroDTO?> ObtenerTrazabilidadAsync(int libroId, CancellationToken ct = default)
        {
            PermisoService.Instancia.Exigir(PermisoVer);
            await using var db = new Libreria();
            var libro = await db.Libros.AsNoTracking().FirstOrDefaultAsync(l => l.LIB_ID == libroId, ct);
            if (libro == null) return null;

            var cambios = await db.AuditoriaLibros.AsNoTracking()
                .Where(a => a.LIB_ID == libroId)
                .Select(a => new CambioLibroDTO
                {
                    LibroId = a.LIB_ID, Fecha = a.AUL_Fecha, Usuario = a.AUL_Usuario, Operacion = a.AUL_Operacion,
                    Campo = a.AUL_Campo, ValorAnterior = a.AUL_ValorAnterior, ValorNuevo = a.AUL_ValorNuevo,
                    Referencia = a.AUL_Referencia,
                })
                .ToListAsync(ct);

            var movimientos = await db.MovimientosStock.AsNoTracking()
                .Where(m => m.LIB_ID == libroId)
                .Select(m => new { m.MOV_Fecha, m.MOV_Usuario, m.MOV_Tipo, m.MOV_StockAnterior, m.MOV_StockResultante, m.MOV_Referencia, m.MOV_Motivo, m.MOV_ID })
                .ToListAsync(ct);

            cambios.AddRange(movimientos.Select(m => new CambioLibroDTO
            {
                LibroId = libroId,
                Fecha = m.MOV_Fecha,
                Usuario = m.MOV_Usuario ?? "",
                Operacion = m.MOV_Tipo == TipoMovimientoStock.AltaInicial ? AuditoriaLibros.OperacionStockAltaInicial : "Stock: " + TipoMovimientoStock.Descripcion(m.MOV_Tipo),
                Campo = "Stock",
                ValorAnterior = m.MOV_StockAnterior.ToString(),
                ValorNuevo = m.MOV_StockResultante.ToString(),
                Referencia = m.MOV_Referencia ?? m.MOV_Motivo,
            }));

            var ascendente = cambios.OrderBy(c => c.Fecha).ToList();
            var actual = await AuditoriaLibros.InstantaneaAsync(db, libro, ct);
            actual["Stock"] = libro.LIB_Stock.ToString();

            foreach (var c in cambios) { c.Titulo = libro.LIB_Titulo; c.ISBN = libro.LIB_ISBN ?? ""; }
            return new TrazabilidadLibroDTO
            {
                LibroId = libro.LIB_ID,
                Titulo = libro.LIB_Titulo,
                ISBN = libro.LIB_ISBN ?? "",
                AltaRegistrada = ascendente.Any(c => c.Operacion == OperacionAuditoriaLibro.Alta),
                Estado = AuditoriaLibros.ReconstruirEstado(ascendente, actual, AuditoriaLibros.Campos.Append("Stock")),
                Historial = ascendente.AsEnumerable().Reverse().ToList(),
            };
        }

        #endregion
    }
}
