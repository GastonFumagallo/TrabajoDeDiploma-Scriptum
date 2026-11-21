using Modelo.Seguridad;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Vista
{
    public partial class FrmHome : Form
    {
        public FrmHome()
        {
            InitializeComponent();
            SetWelcomeText($"Bienvenido, {GetNombreUsuarioParaSaludo()}");
            var hoy = DateTime.Today.ToString("dddd d 'de' MMMM 'de' yyyy");
            lblFecha.Text = char.ToUpper(hoy[0]) + hoy[1..];
            // La tarjeta de bienvenida queda centrada en la sección, sea cual sea el tamaño de la ventana.
            Resize += (_, _) => CentrarTarjeta();
            CentrarTarjeta();
        }

        public string GetNombreUsuarioParaSaludo()
        {
            try
            {
                var usuario = Sesion.Instancia.Usuario;
                if (usuario != null)
                {
                    var persona = usuario.USU_Persona;
                    if (persona != null)
                    {
                        var nom = string.IsNullOrWhiteSpace(persona.PER_Nombre) ? usuario.USU_Nombre : persona.PER_Nombre;
                        return (nom).Trim();
                    }

                    return usuario.USU_Nombre ?? "Usuario";
                }

                return "Usuario";
            }
            catch
            {
                return "Usuario";
            }
        }
        private void CentrarTarjeta() =>
            panelBienvenida.Location = new Point(Math.Max(0, (ClientSize.Width - panelBienvenida.Width) / 2),
                                                 Math.Max(0, (ClientSize.Height - panelBienvenida.Height) / 2 - 30));

        // lblInicio es una etiqueta de alto fijo dentro de la tarjeta (AutoSize = false, TextAlign = BottomLeft):
        // los saludos largos se parten en dos líneas sin mover el resto de la tarjeta.
        public void SetWelcomeText(string text) => lblInicio.Text = text;
    }

}