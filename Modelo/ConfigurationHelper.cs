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
            // appsettings.local.json (opcional, fuera de git) pisa a appsettings.json:
            // ahí van los secretos, como la clave SMTP. Ver appsettings.local.example.json.
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.local.json", optional: true)
                .Build();
        }

        /// <summary>Lee un valor de configuración (ej. "Smtp:Usuario"). Devuelve null si no existe.</summary>
        public static string? Get(string clave) => _configuration[clave];

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
