using Modelo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Vista.Comun
{
    /// <summary>
    /// Muestra cualquier excepción con un mensaje claro según su tipo y registra las inesperadas en un log
    /// local para soporte. El usuario nunca ve un stack trace ni un error de SQL.
    /// </summary>
    internal static class ManejadorErrores
    {
        private static readonly string RutaLog = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Scriptum", "errores.log");

        /// <param name="contexto">Qué se estaba haciendo, ej. "No se pudo guardar el libro."</param>
        public static void Mostrar(IWin32Window? owner, Exception ex, string contexto)
        {
            switch (ex)
            {
                case OperationCanceledException:
                    return;
                case ValidacionException v:
                    MessageBox.Show(owner, v.Message, "Revise los datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                case AccesoDenegadoException a:
                    MessageBox.Show(owner, a.Message, "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                case ConcurrenciaException c:
                    MessageBox.Show(owner, c.Message, "Datos modificados por otro usuario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                case PersistenciaException p:
                    Registrar(p);
                    MessageBox.Show(owner, $"{contexto}\n\n{p.Message}", "No se pudo completar la operación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                default:
                    Registrar(ex);
                    MessageBox.Show(owner,
                        $"{contexto}\n\nOcurrió un error inesperado. Si se repite, avisá a soporte (el detalle quedó registrado en {RutaLog})." +
                        DetalleDesarrollo(ex),
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
            }
        }

        /// <summary>
        /// true si la excepción es la consecuencia de haber cancelado <paramref name="token"/> (una carga más nueva
        /// reemplazó a la anterior, o se cerró el formulario). Se decide por el token y no por el tipo: al cancelar una
        /// consulta en curso, SqlClient a veces lanza <see cref="TaskCanceledException"/>, pero otras una
        /// <c>SqlException</c> ("Operación cancelada por el usuario") o una <see cref="InvalidOperationException"/>.
        /// Uso: <c>catch (Exception cancelada) when (ManejadorErrores.EsCancelacion(cancelada, token)) { }</c>
        /// </summary>
        public static bool EsCancelacion(Exception ex, CancellationToken token) =>
            ex is OperationCanceledException || token.IsCancellationRequested;

        /// <summary>En compilaciones Debug, el tipo y el mensaje de la excepción más interna; en Release, nada.</summary>
        private static string DetalleDesarrollo(Exception ex)
        {
#if DEBUG
            var raiz = ex.GetBaseException();
            return $"\n\n[Debug] {raiz.GetType().Name}: {raiz.Message}";
#else
            return string.Empty;
#endif
        }

        private static void Registrar(Exception ex)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(RutaLog)!);
                File.AppendAllText(RutaLog, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex}{Environment.NewLine}{Environment.NewLine}");
            }
            catch
            {
                // Si no se puede escribir el log, no se interrumpe al usuario por eso.
            }
        }
    }
}
