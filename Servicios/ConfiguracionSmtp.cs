using Modelo;
using System.Net;
using System.Net.Mail;

namespace Servicios
{
    /// <summary>
    /// Crea el cliente SMTP a partir de la sección "Smtp" de configuración (appsettings.local.json,
    /// que está en .gitignore). Las credenciales nunca deben escribirse en el código fuente.
    /// </summary>
    public static class ConfiguracionSmtp
    {
        /// <returns>El cliente listo para enviar, o null (con <paramref name="error"/>) si falta configuración.</returns>
        public static SmtpClient? CrearCliente(out string remitente, out string? error)
        {
            string? servidor = ConfigurationHelper.Get("Smtp:Servidor");
            string? usuario = ConfigurationHelper.Get("Smtp:Usuario");
            string? clave = ConfigurationHelper.Get("Smtp:Clave");
            remitente = ConfigurationHelper.Get("Smtp:Remitente") ?? usuario ?? string.Empty;
            int puerto = int.TryParse(ConfigurationHelper.Get("Smtp:Puerto"), out var p) ? p : 587;

            if (string.IsNullOrWhiteSpace(servidor) || string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(clave))
            {
                error = "El envío de correos no está configurado (falta la sección Smtp en appsettings.local.json).";
                return null;
            }

            error = null;
            return new SmtpClient(servidor, puerto)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(usuario, clave),
            };
        }

        public static string NombreRemitente => ConfigurationHelper.Get("Smtp:NombreRemitente") ?? "Librería SCRIPTUM";
    }
}
