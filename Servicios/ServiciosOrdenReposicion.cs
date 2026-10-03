using Modelo;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Servicios
{
    /// <summary>Datos que necesita el correo de solicitud de reposición (independiente de EF).</summary>
    public sealed class SolicitudReposicionEmail
    {
        public string NumeroOrden { get; init; } = string.Empty;
        public string EmailProveedor { get; init; } = string.Empty;
        public string NombreProveedor { get; init; } = string.Empty;
        public string? Observaciones { get; init; }
        public IReadOnlyList<(string Titulo, string? Codigo, int Cantidad)> Items { get; init; } = Array.Empty<(string, string?, int)>();
    }

    public static class ServiciosOrdenReposicion
    {
        /// <summary>
        /// Envía al proveedor el pedido de reposición. Las credenciales SMTP se leen de configuración
        /// (sección "Smtp" de appsettings.local.json, que no se versiona); nunca deben estar en el código.
        /// Devuelve null si se envió, o un mensaje de error apto para el usuario.
        /// </summary>
        public static async Task<string?> EnviarSolicitudReposicionAsync(SolicitudReposicionEmail solicitud, CancellationToken ct = default)
        {
            string? servidor = ConfigurationHelper.Get("Smtp:Servidor");
            string? usuario = ConfigurationHelper.Get("Smtp:Usuario");
            string? clave = ConfigurationHelper.Get("Smtp:Clave");
            string remitente = ConfigurationHelper.Get("Smtp:Remitente") ?? usuario ?? string.Empty;
            string nombreRemitente = ConfigurationHelper.Get("Smtp:NombreRemitente") ?? "Librería SCRIPTUM";
            int puerto = int.TryParse(ConfigurationHelper.Get("Smtp:Puerto"), out var p) ? p : 587;

            if (string.IsNullOrWhiteSpace(servidor) || string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(clave))
                return "El envío de correos no está configurado (falta la sección Smtp en appsettings.local.json).";

            if (string.IsNullOrWhiteSpace(solicitud.EmailProveedor))
                return "El proveedor no tiene un email cargado.";

            try
            {
                using var mail = new MailMessage
                {
                    From = new MailAddress(remitente, nombreRemitente),
                    Subject = $"Solicitud de reposición {solicitud.NumeroOrden} - Librería SCRIPTUM",
                    Body = ArmarCuerpo(solicitud),
                    IsBodyHtml = true,
                };
                mail.To.Add(solicitud.EmailProveedor);

                using var client = new SmtpClient(servidor, puerto)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(usuario, clave),
                };
                await client.SendMailAsync(mail, ct);
                return null;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return $"No se pudo enviar el correo: {ex.GetBaseException().Message}";
            }
        }

        private static string ArmarCuerpo(SolicitudReposicionEmail s)
        {
            var filas = new StringBuilder();
            foreach (var item in s.Items)
            {
                filas.Append("<tr>")
                     .Append($"<td style='padding:4px 12px;border:1px solid #ccc'>{WebUtility.HtmlEncode(item.Titulo)}</td>")
                     .Append($"<td style='padding:4px 12px;border:1px solid #ccc'>{WebUtility.HtmlEncode(item.Codigo ?? "-")}</td>")
                     .Append($"<td style='padding:4px 12px;border:1px solid #ccc;text-align:right'>{item.Cantidad}</td>")
                     .Append("</tr>");
            }

            string observaciones = string.IsNullOrWhiteSpace(s.Observaciones)
                ? string.Empty
                : $"<p><b>Observaciones:</b> {WebUtility.HtmlEncode(s.Observaciones)}</p>";

            return $@"
<div style='font-family:Segoe UI;padding:20px'>
    <h2>Solicitud de reposición {WebUtility.HtmlEncode(s.NumeroOrden)} - SCRIPTUM</h2>
    <p>Estimados {WebUtility.HtmlEncode(s.NombreProveedor)},</p>
    <p>Necesitamos reponer stock de los siguientes libros:</p>
    <table style='border-collapse:collapse'>
        <tr><th style='padding:4px 12px;border:1px solid #ccc'>Libro</th>
            <th style='padding:4px 12px;border:1px solid #ccc'>ISBN</th>
            <th style='padding:4px 12px;border:1px solid #ccc'>Cantidad</th></tr>
        {filas}
    </table>
    {observaciones}
    <p>Quedamos atentos a su confirmación.</p>
    <br><b>Librería SCRIPTUM</b>
</div>";
        }
    }
}
