using Modelo;
using Modelo.Seguridad;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace Servicios
{
    public static class ServiciosUsuario
    {
        public static bool SendMail(Usuario usuario, string claveNueva)
        {

            string Body = $@"
                <div style=""margin:0;padding:24px;background-color:#f4f6fb;font-family:Segoe UI,Arial,sans-serif;"">
                    <div style=""max-width:620px;margin:0 auto;background:#ffffff;border-radius:14px;overflow:hidden;box-shadow:0 8px 25px rgba(0,0,0,0.08);"">
                        <div style=""background:linear-gradient(135deg,#2563eb,#7c3aed);padding:24px 28px;color:#ffffff;"">
                            <h1 style=""margin:0;font-size:28px;font-weight:700;"">¡Bienvenido, {usuario.USU_Persona.PER_Nombre}!</h1>
                            <p style=""margin:8px 0 0 0;font-size:14px;opacity:0.95;"">Sistema SCRIPTUM</p>
                        </div>
                        <div style=""padding:26px 28px;color:#1f2937;"">
                            <p style=""margin:0 0 14px 0;font-size:16px;"">Tu nueva contraseña temporal es:</p>
                            <div style=""display:inline-block;background:#eef2ff;border:1px solid #c7d2fe;border-radius:10px;padding:12px 18px;font-size:22px;font-weight:700;letter-spacing:1px;color:#3730a3;"">{claveNueva}</div>
                            <p style=""margin:18px 0 0 0;font-size:14px;color:#4b5563;line-height:1.6;"">Te recomendamos iniciar sesión y cambiarla por una personal para mantener tu cuenta segura.</p>
                        </div>
                        <div style=""padding:16px 28px;background:#f9fafb;color:#6b7280;font-size:12px;border-top:1px solid #e5e7eb;"">
                            Equipo de soporte · SCRIPTUM
                        </div>  
                    </div>
                </div>";

            string from = "libreriascriptum.notificaciones@gmail.com";
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(from);
                mail.To.Add(usuario.USU_Mail);

                mail.Subject = "Recuperación de acceso - SCRIPTUM";
                mail.Body = Body;
                mail.IsBodyHtml = true;

                SmtpClient client = new SmtpClient("smtp.gmail.com");
                client.Port = 587;
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(from, "tboo mkvp vbfa txmc");
                client.Send(mail);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public static string GenerarPassword()
        {
            var random = new Random();
            return string.Concat(Enumerable.Range(0, 5).Select(_ => random.Next(0, 9).ToString()));
        }
        public static string EncriptarClave(string clave)
        {
            StringBuilder sb = new StringBuilder();
            using (SHA256 sha256 = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                if (!String.IsNullOrEmpty(clave))
                {
                    byte[] resoult = sha256.ComputeHash(enc.GetBytes(clave));
                    foreach (byte b in resoult)
                    {
                        sb.Append(b.ToString("x2"));
                    }
                }
            }
            return sb.ToString();
        }

    }
}
