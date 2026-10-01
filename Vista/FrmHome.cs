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
        private const int MENU_ANCHO = 321; 
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
        public void SetWelcomeText(string text)
        {
            if (lblInicio != null)
            {
                lblInicio.Text = text;
                CenterInicioLabel();
            }
        }


        private void CenterInicioLabel()
        {
            if (lblInicio == null || this.ClientSize.Width == 0) 
                return;
            int areaUtil = this.ClientSize.Width - MENU_ANCHO;
            if (areaUtil <= 0) return;
            lblInicio.AutoSize = false;
            lblInicio.MaximumSize = new Size(areaUtil - 20, 0); 
            lblInicio.AutoSize = true;
            var px = Math.Max(0, (this.ClientSize.Width - lblInicio.PreferredWidth) / 2);
            var py = Math.Max(0, (this.ClientSize.Height - lblInicio.PreferredHeight) / 2);
            lblInicio.Location = new Point(px, py);
        }

        private void FrmHome_Load(object sender, EventArgs e)
        {
            CenterInicioLabel();
        }
    }

}