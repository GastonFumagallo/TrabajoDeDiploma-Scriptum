using System.Net;
using System.Net.Mail;

namespace Servicios
{
    /// <summary>Mails de la cuenta de usuario: clave temporal (alta o blanqueo) y código de recuperación.</summary>
    public static class ServiciosUsuario
    {
        /// <summary>Envía la clave temporal. Devuelve false si el SMTP no está configurado o el envío falla.</summary>
        public static bool EnviarClaveTemporal(string nombre, string mail, string usuario, string claveTemporal, bool esAlta)
        {
            var titulo = esAlta ? $"¡Bienvenido, {Html(nombre)}!" : $"Hola, {Html(nombre)}";
            var intro = esAlta
                ? $"Se creó tu cuenta en SCRIPTUM. Tu usuario es <b>{Html(usuario)}</b> y tu contraseña temporal es:"
                : $"Un administrador blanqueó la contraseña del usuario <b>{Html(usuario)}</b>. Tu nueva contraseña temporal es:";
            var cuerpo = $@"
                <p style=""margin:0 0 14px 0;font-size:16px;"">{intro}</p>
                {Destacado(claveTemporal)}
                <p style=""margin:18px 0 0 0;font-size:14px;color:#4b5563;line-height:1.6;"">Al iniciar sesión el sistema te va a pedir que la cambies por una personal.</p>";
            return Enviar(mail, esAlta ? "Tu cuenta en SCRIPTUM" : "Contraseña temporal - SCRIPTUM", titulo, cuerpo);
        }

        /// <summary>Envía el código de recuperación. La clave actual sigue funcionando hasta que se use el código.</summary>
        public static bool EnviarCodigoRecuperacion(string nombre, string mail, string codigo, int minutosVigencia)
        {
            var cuerpo = $@"
                <p style=""margin:0 0 14px 0;font-size:16px;"">Recibimos un pedido para recuperar el acceso a tu cuenta. Tu código es:</p>
                {Destacado(codigo)}
                <p style=""margin:18px 0 0 0;font-size:14px;color:#4b5563;line-height:1.6;"">Vence en {minutosVigencia} minutos. Si no lo pediste, ignorá este mensaje: tu contraseña no cambió.</p>";
            return Enviar(mail, "Código de recuperación - SCRIPTUM", $"Hola, {Html(nombre)}", cuerpo);
        }

        private static string Html(string? texto) => WebUtility.HtmlEncode(texto ?? "");

        private static string Destacado(string valor) =>
            $@"<div style=""display:inline-block;background:#eef2ff;border:1px solid #c7d2fe;border-radius:10px;padding:12px 18px;font-size:22px;font-weight:700;letter-spacing:1px;color:#3730a3;font-family:Consolas,monospace;"">{Html(valor)}</div>";

        private static bool Enviar(string para, string asunto, string titulo, string cuerpoHtml)
        {
            string body = $@"
                <div style=""margin:0;padding:24px;background-color:#f4f6fb;font-family:Segoe UI,Arial,sans-serif;"">
                    <div style=""max-width:620px;margin:0 auto;background:#ffffff;border-radius:14px;overflow:hidden;box-shadow:0 8px 25px rgba(0,0,0,0.08);"">
                        <div style=""background:linear-gradient(135deg,#2563eb,#7c3aed);padding:24px 28px;color:#ffffff;"">
                            <h1 style=""margin:0;font-size:28px;font-weight:700;"">{titulo}</h1>
                            <p style=""margin:8px 0 0 0;font-size:14px;opacity:0.95;"">Sistema SCRIPTUM</p>
                        </div>
                        <div style=""padding:26px 28px;color:#1f2937;"">
                            {cuerpoHtml}
                        </div>
                        <div style=""padding:16px 28px;background:#f9fafb;color:#6b7280;font-size:12px;border-top:1px solid #e5e7eb;"">
                            Equipo de soporte · SCRIPTUM
                        </div>
                    </div>
                </div>";

            // Credenciales SMTP desde configuración (appsettings.local.json), nunca en el código.
            using var client = ConfiguracionSmtp.CrearCliente(out string from, out _);
            if (client == null)
                return false;
            try
            {
                using MailMessage mail = new MailMessage();
                mail.From = new MailAddress(from);
                mail.To.Add(para);
                mail.Subject = asunto;
                mail.Body = body;
                mail.IsBodyHtml = true;
                client.Send(mail);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
