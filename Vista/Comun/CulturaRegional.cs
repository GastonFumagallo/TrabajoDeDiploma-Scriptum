using Modelo;
using System.Globalization;

namespace Vista.Comun
{
    /// <summary>
    /// Fija la cultura de toda la aplicación (moneda, separadores, fechas) en lugar de heredar el "Formato regional"
    /// de Windows. Si Windows está en "English (World)" (en-001), .NET muestra la moneda como "XDR" (código ISO 4217
    /// de los Derechos Especiales de Giro del FMI, la "moneda" de esa región sin país) y los meses en inglés.
    /// <para>Se configura en appsettings.json:</para>
    /// <code>"Regional": { "Cultura": "es-AR", "SimboloMoneda": "$" }</code>
    /// </summary>
    internal static class CulturaRegional
    {
        private const string CulturaPorDefecto = "es-AR";

        /// <summary>Llamar al inicio de Main, antes de crear cualquier formulario o hilo.</summary>
        public static void Configurar()
        {
            var nombre = ConfigurationHelper.Get("Regional:Cultura");
            CultureInfo cultura;
            try
            {
                cultura = (CultureInfo)CultureInfo.GetCultureInfo(string.IsNullOrWhiteSpace(nombre) ? CulturaPorDefecto : nombre.Trim()).Clone();
            }
            catch (CultureNotFoundException)
            {
                cultura = (CultureInfo)CultureInfo.GetCultureInfo(CulturaPorDefecto).Clone();
            }

            // Opcional: otro símbolo para la moneda de la empresa (ej. "ARS") sin cambiar separadores ni fechas.
            var simbolo = ConfigurationHelper.Get("Regional:SimboloMoneda");
            if (!string.IsNullOrWhiteSpace(simbolo))
                cultura.NumberFormat.CurrencySymbol = simbolo.Trim();

            cultura = CultureInfo.ReadOnly(cultura);

            // El hilo de la interfaz y todos los demás (Task.Run, timers, continuaciones async).
            CultureInfo.DefaultThreadCurrentCulture = cultura;
            CultureInfo.DefaultThreadCurrentUICulture = cultura;
            CultureInfo.CurrentCulture = cultura;
            CultureInfo.CurrentUICulture = cultura;
        }
    }
}
