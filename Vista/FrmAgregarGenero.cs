using Controladora;
using Modelo;
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
    public partial class FrmAgregarGenero : Form
    {
        public FrmAgregarGenero()
        {
            InitializeComponent();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDescripcion.Text) || string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Por favor, completa todos los campos.");
                return;
            }

            Genero nuevoGenero = new Genero();

            nuevoGenero.GEN_Nombre = txtNombre.Text;
            nuevoGenero.GEN_Descripcion = txtDescripcion.Text;
            ControladoraGeneros.Instancia.AgregarGenero(nuevoGenero);
            MessageBox.Show("Genero agregado correctamente");
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
