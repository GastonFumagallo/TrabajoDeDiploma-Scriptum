using Modelo;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Servicios
{
    public static class ServiciosOrdenReposicion
    {
        public static bool EnviarSolicitudReposicion(Proveedor proveedor, Libro libro, int cantidad)
        {
            string body = $@"
                <div style='font-family:Segoe UI;padding:20px'>
                    <h2>Solicitud de Reposición - SCRIPTUM</h2>

                     <p>Estimados,</p>

                     <p>Necesitamos reponer stock del siguiente libro:</p>

                     <table style='border-collapse:collapse'>
                        <tr>
                            <td><b>Libro:</b></td>
                            <td>{libro.LIB_Titulo}</td>
                        </tr>
                        <tr>
                            <td><b>Cantidad:</b></td>
                            <td>{cantidad}</td>
                        </tr>
                    </table>

                    <br>

                    <p>
                        Solicitamos reservar dicha cantidad para una futura compra.
                    </p>

                    <p>
                        Quedamos atentos a su confirmación.
                    </p>

                    <br>

                    <b>Librería SCRIPTUM</b>
                </div>";

            string from = "libreriascriptum.notificaciones@gmail.com";

            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(from);
                mail.To.Add(proveedor.PER_Proveedor.PER_Mail);

                mail.Subject = $"Solicitud de reposición - {libro.LIB_Titulo}";
                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpClient client = new SmtpClient("smtp.gmail.com");
                client.Port = 587;
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(from, "tboo mkvp vbfa txmc");
                client.Send(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
