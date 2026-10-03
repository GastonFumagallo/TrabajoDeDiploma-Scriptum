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
        /// (ver <see cref="ConfiguracionSmtp"/>); nunca deben estar en el código.
        /// Devuelve null si se envió, o un mensaje de error apto para el usuario.
        /// </summary>
        public static async Task<string?> EnviarSolicitudReposicionAsync(SolicitudReposicionEmail solicitud, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(solicitud.EmailProveedor))
                return "El proveedor no tiene un email cargado.";

            using var client = ConfiguracionSmtp.CrearCliente(out string remitente, out string? errorConfig);
            if (client == null)
                return errorConfig;

            try
            {
                using var mail = new MailMessage
                {
                    From = new MailAddress(remitente, ConfiguracionSmtp.NombreRemitente),
                    Subject = $"Solicitud de reposición {solicitud.NumeroOrden} - Librería SCRIPTUM",
                    Body = ArmarCuerpo(solicitud),
                    IsBodyHtml = true,
                };
                mail.To.Add(solicitud.EmailProveedor);

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
