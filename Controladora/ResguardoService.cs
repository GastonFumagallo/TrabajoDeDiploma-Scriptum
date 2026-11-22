using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using Servicios;

namespace Controladora
{
    /// <summary>
    /// Resguardo y restauración de la base de datos con los comandos nativos de SQL Server:
    /// <c>BACKUP DATABASE … WITH CHECKSUM</c>, <c>RESTORE VERIFYONLY</c> y <c>RESTORE DATABASE … WITH REPLACE</c>.
    /// <para>
    /// El historial se lee de <c>msdb</c> (no de la base de la aplicación): así sobrevive a una restauración, que
    /// devuelve la base al estado del resguardo. Las rutas son del equipo donde corre SQL Server.
    /// </para>
    /// </summary>
    public sealed class ResguardoService
    {
        public const string PermisoRealizar = "RealizarResguardo";
        public const string PermisoRestaurar = "RestaurarResguardo";
        private const string PrefijoDescripcion = "SCRIPTUM";

        private static ResguardoService? instancia;
        public static ResguardoService Instancia => instancia ??= new ResguardoService();
        private ResguardoService() { }

        private static SqlConnectionStringBuilder Cadena() =>
            new(ConfigurationHelper.GetConnectionString("DefaultConnection"));

        /// <summary>Nombre de la base de la aplicación (Initial Catalog de la cadena de conexión).</summary>
        public static string NombreBase => Cadena().InitialCatalog;

        private static SqlConnection ConexionMaster()
        {
            var b = Cadena();
            b.InitialCatalog = "master";
            b.Pooling = false;   // la conexión no debe quedar abierta sobre la base que se restaura
            return new SqlConnection(b.ConnectionString);
        }

        private static string Corchetes(string nombre) => "[" + nombre.Replace("]", "]]") + "]";

        public static string NombreArchivo(string baseDatos, DateTime fecha) => $"{baseDatos}_{fecha:yyyyMMdd_HHmmss}.bak";

        public async Task<string> ObtenerCarpetaPredeterminadaAsync(CancellationToken ct = default)
        {
            await using var cn = ConexionMaster();
            await cn.OpenAsync(ct);
            await using var cmd = new SqlCommand("SELECT CAST(SERVERPROPERTY('InstanceDefaultBackupPath') AS nvarchar(4000))", cn);
            return (await cmd.ExecuteScalarAsync(ct)) as string ?? string.Empty;
        }

        /// <summary>Resguardo completo con suma de verificación, seguido de su verificación.</summary>
        public async Task<(ResguardoDTO Resguardo, VerificacionResguardoDTO Verificacion)> RealizarAsync(string carpeta, string? descripcion = null,
            CancellationToken ct = default, string? sufijoArchivo = null)
        {
            PermisoService.Instancia.Exigir(PermisoRealizar);
            if (string.IsNullOrWhiteSpace(carpeta))
                throw new ArgumentException("Indique la carpeta del servidor donde guardar el resguardo.", nameof(carpeta));
            string ruta;

            string usuario = AuditoriaLibros.UsuarioActual;
            DateTime fecha = DateTime.Now;
            string desc = $"{PrefijoDescripcion} · usuario: {usuario}" + (string.IsNullOrWhiteSpace(descripcion) ? "" : " · " + descripcion.Trim());

            await using (var cn = ConexionMaster())
            {
                await cn.OpenAsync(ct);

                // Nombre único: nunca se sobrescribe un resguardo existente (dos resguardos en el mismo segundo,
                // como el automático previo a una restauración, tendrían el mismo nombre).
                string ruta0 = Path.Combine(carpeta.Trim(), NombreArchivo(NombreBase, fecha));
                if (!string.IsNullOrEmpty(sufijoArchivo)) ruta0 = Path.ChangeExtension(ruta0, null) + "_" + sufijoArchivo + ".bak";
                ruta = ruta0;
                for (int i = 2; await ArchivoExisteAsync(cn, ruta, ct) == true; i++)
                    ruta = Path.ChangeExtension(ruta0, null) + $"_{i}.bak";

                // NOINIT + FORMAT no: si el archivo existiera igual, se agrega un conjunto nuevo en vez de borrarlo.
                await using var cmd = new SqlCommand(
                    $"BACKUP DATABASE {Corchetes(NombreBase)} TO DISK = @ruta WITH NOINIT, CHECKSUM, NAME = @nombre, DESCRIPTION = @desc", cn)
                { CommandTimeout = 0 };
                cmd.Parameters.AddWithValue("@ruta", ruta);
                cmd.Parameters.AddWithValue("@nombre", $"{NombreBase} — resguardo completo");
                cmd.Parameters.AddWithValue("@desc", desc.Length > 255 ? desc[..255] : desc);
                await cmd.ExecuteNonQueryAsync(ct);
            }

            var verificacion = await VerificarAsync(ruta, ct);
            BitacoraSeguridad.Registrar(BitacoraSeguridad.ResguardoRealizado,
                $"{ruta} · verificación: {(verificacion.Valido ? "correcta" : verificacion.Mensaje)}");

            var resguardo = (await ObtenerHistorialAsync(1, ct)).FirstOrDefault(r => string.Equals(r.Archivo, ruta, StringComparison.OrdinalIgnoreCase))
                            ?? new ResguardoDTO { Fecha = fecha, Tipo = "Completo", Archivo = ruta, Usuario = usuario, Descripcion = desc, ArchivoDisponible = true };
            return (resguardo, verificacion);
        }

        /// <summary>Comprueba que el archivo sea un resguardo legible y completo (RESTORE HEADERONLY + VERIFYONLY WITH CHECKSUM).</summary>
        public async Task<VerificacionResguardoDTO> VerificarAsync(string ruta, CancellationToken ct = default)
        {
            PermisoService.Instancia.Exigir(PermisoRealizar);
            var r = new VerificacionResguardoDTO();
            try
            {
                await using var cn = ConexionMaster();
                await cn.OpenAsync(ct);
                await using (var cab = new SqlCommand("RESTORE HEADERONLY FROM DISK = @ruta", cn) { CommandTimeout = 0 })
                {
                    cab.Parameters.AddWithValue("@ruta", ruta);
                    await using var lector = await cab.ExecuteReaderAsync(ct);
                    if (await lector.ReadAsync(ct))
                    {
                        r.BaseDeDatos = lector["DatabaseName"] as string;
                        r.FechaResguardo = lector["BackupFinishDate"] as DateTime?;
                    }
                }
                await using (var ver = new SqlCommand("RESTORE VERIFYONLY FROM DISK = @ruta WITH CHECKSUM", cn) { CommandTimeout = 0 })
                {
                    ver.Parameters.AddWithValue("@ruta", ruta);
                    await ver.ExecuteNonQueryAsync(ct);
                }
                r.Valido = true;
                r.Mensaje = $"El resguardo es válido: base {r.BaseDeDatos}, realizado el {r.FechaResguardo:dd/MM/yyyy HH:mm}.";
            }
            catch (SqlException ex)
            {
                r.Valido = false;
                r.Mensaje = "El resguardo no es válido o no se puede leer: " + ex.Message;
            }
            return r;
        }

        /// <summary>Resguardos de la base registrados por SQL Server en msdb, del más reciente al más antiguo.</summary>
        public async Task<List<ResguardoDTO>> ObtenerHistorialAsync(int maximo = 100, CancellationToken ct = default)
        {
            PermisoService.Instancia.Exigir(PermisoRealizar);
            var lista = new List<ResguardoDTO>();
            await using var cn = ConexionMaster();
            await cn.OpenAsync(ct);
            await using var cmd = new SqlCommand(@"
SELECT TOP (@max) bs.backup_finish_date, bs.type, mf.physical_device_name, bs.backup_size, bs.user_name, bs.description
FROM msdb.dbo.backupset bs
JOIN msdb.dbo.backupmediafamily mf ON mf.media_set_id = bs.media_set_id
WHERE bs.database_name = @db
ORDER BY bs.backup_finish_date DESC", cn);
            cmd.Parameters.AddWithValue("@max", maximo);
            cmd.Parameters.AddWithValue("@db", NombreBase);
            await using (var lector = await cmd.ExecuteReaderAsync(ct))
            {
                while (await lector.ReadAsync(ct))
                {
                    string desc = lector.IsDBNull(5) ? "" : lector.GetString(5);
                    lista.Add(new ResguardoDTO
                    {
                        Fecha = lector.GetDateTime(0),
                        Tipo = lector.GetString(1) switch { "D" => "Completo", "I" => "Diferencial", "L" => "Registro de transacciones", var t => t },
                        Archivo = lector.GetString(2),
                        TamanioMB = Math.Round(Convert.ToDecimal(lector.GetValue(3)) / 1024m / 1024m, 2),
                        Usuario = UsuarioDeDescripcion(desc) ?? (lector.IsDBNull(4) ? "" : lector.GetString(4)),
                        Descripcion = desc,
                    });
                }
            }
            // La existencia la comprueba el servidor (la carpeta de SQL Server suele no ser legible para el usuario).
            foreach (var r in lista)
                r.ArchivoDisponible = await ArchivoExisteAsync(cn, r.Archivo, ct) ?? true;
            return lista;
        }

        /// <summary>true/false si el servidor puede comprobarlo; null si no hay permiso para preguntar (se asume que existe).</summary>
        private static async Task<bool?> ArchivoExisteAsync(SqlConnection cn, string ruta, CancellationToken ct)
        {
            if (File.Exists(ruta)) return true;
            try
            {
                await using var cmd = new SqlCommand("DECLARE @r int; EXEC master.dbo.xp_fileexist @ruta, @r OUTPUT; SELECT @r;", cn);
                cmd.Parameters.AddWithValue("@ruta", ruta);
                return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct)) == 1;
            }
            catch (SqlException)
            {
                return null;
            }
        }

        public static string? UsuarioDeDescripcion(string descripcion)
        {
            const string marca = "usuario: ";
            int i = descripcion.IndexOf(marca, StringComparison.Ordinal);
            if (!descripcion.StartsWith(PrefijoDescripcion, StringComparison.Ordinal) || i < 0) return null;
            var resto = descripcion[(i + marca.Length)..];
            int fin = resto.IndexOf(" · ", StringComparison.Ordinal);
            return fin < 0 ? resto : resto[..fin];
        }

        /// <summary>
        /// Restaura la base desde <paramref name="ruta"/>. Antes verifica el archivo y hace un resguardo automático del
        /// estado actual, para poder volver atrás. Desconecta a los demás usuarios (SINGLE_USER … ROLLBACK IMMEDIATE).
        /// Al terminar, la aplicación debe reiniciarse: los datos en memoria ya no corresponden a la base.
        /// </summary>
        public async Task<ResultadoOperacion> RestaurarAsync(string ruta, string carpetaResguardoPrevio, CancellationToken ct = default)
        {
            PermisoService.Instancia.Exigir(PermisoRestaurar);
            string usuario = AuditoriaLibros.UsuarioActual;

            var verificacion = await VerificarAsync(ruta, ct);
            if (!verificacion.Valido)
                return ResultadoOperacion.Error(verificacion.Mensaje);
            if (!string.Equals(verificacion.BaseDeDatos, NombreBase, StringComparison.OrdinalIgnoreCase))
                return ResultadoOperacion.Error($"El archivo es un resguardo de la base \"{verificacion.BaseDeDatos}\", no de \"{NombreBase}\".");

            var (previo, _) = await RealizarAsync(carpetaResguardoPrevio, "automático, previo a restaurar", ct, "previo_restauracion");

            string db = Corchetes(NombreBase);
            await using (var cn = ConexionMaster())
            {
                await cn.OpenAsync(ct);
                try
                {
                    await Ejecutar(cn, $"ALTER DATABASE {db} SET SINGLE_USER WITH ROLLBACK IMMEDIATE", null, ct);
                    await Ejecutar(cn, $"RESTORE DATABASE {db} FROM DISK = @ruta WITH REPLACE, CHECKSUM", ruta, ct);
                }
                finally
                {
                    // Siempre se devuelve la base a multiusuario, aunque la restauración falle.
                    await Ejecutar(cn, $"IF DATABASEPROPERTYEX(N'{NombreBase.Replace("'", "''")}', 'UserAccess') = 'SINGLE_USER' ALTER DATABASE {db} SET MULTI_USER", null, CancellationToken.None);
                }
            }
            SqlConnection.ClearAllPools();

            // La bitácora volvió al estado del resguardo: se vuelve a dejar constancia del resguardo previo.
            BitacoraSeguridad.Registrar(BitacoraSeguridad.ResguardoRealizado, $"{previo.Archivo} · automático, previo a restaurar", usuario);
            BitacoraSeguridad.Registrar(BitacoraSeguridad.BaseRestaurada,
                $"Desde {ruta} (resguardo del {verificacion.FechaResguardo:dd/MM/yyyy HH:mm}). Resguardo previo: {previo.Archivo}", usuario);

            string aviso = "";
            try
            {
                await using var ctx = new Libreria();
                var pendientes = (await ctx.Database.GetPendingMigrationsAsync(ct)).ToList();
                if (pendientes.Count > 0)
                    aviso = $"\n\nAtención: el resguardo es de una versión anterior del sistema ({pendientes.Count} actualizaciones de esquema pendientes). " +
                            "Pida al administrador que actualice la base antes de seguir operando.";
            }
            catch { /* el aviso es informativo */ }

            return ResultadoOperacion.Ok(0,
                $"La base se restauró al {verificacion.FechaResguardo:dd/MM/yyyy HH:mm}.\nEl estado anterior quedó resguardado en:\n{previo.Archivo}{aviso}");
        }

        private static async Task Ejecutar(SqlConnection cn, string sql, string? ruta, CancellationToken ct)
        {
            await using var cmd = new SqlCommand(sql, cn) { CommandTimeout = 0 };
            if (ruta != null) cmd.Parameters.AddWithValue("@ruta", ruta);
            await cmd.ExecuteNonQueryAsync(ct);
        }
    }
}
