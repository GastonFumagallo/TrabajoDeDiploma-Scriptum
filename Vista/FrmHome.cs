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
        // lblInicio ocupa todo el formulario (Dock = Fill, AutoSize = false, TextAlign = MiddleCenter):
        // el texto queda centrado y se reacomoda solo al redimensionar o al cambiar su longitud.
        public void SetWelcomeText(string text) => lblInicio.Text = text;
    }

}