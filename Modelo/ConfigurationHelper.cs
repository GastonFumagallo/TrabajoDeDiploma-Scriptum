using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modelo
{
    public static class ConfigurationHelper
    {
        private static IConfigurationRoot _configuration;

        static ConfigurationHelper()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
        }

        public static string GetConnectionString(string name)
        {
            return _configuration.GetConnectionString(name);
        }

        /// <summary>Lee un decimal de appsettings.json (ej. "Ventas:TasaIVA"). Si falta o es inválido, devuelve el valor por defecto.</summary>
        public static decimal GetDecimal(string clave, decimal porDefecto)
        {
            var valor = _configuration[clave];
            return decimal.TryParse(valor, System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.InvariantCulture, out var resultado)
                ? resultado
                : porDefecto;
        }
    }
}
