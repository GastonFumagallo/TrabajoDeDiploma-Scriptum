using Controladora;
using Modelo;
using Vista.Comun;

namespace Vista.Seguridad
{
    /// <summary>
    /// Cambio de clave del propio usuario. Con <c>obligatorio = true</c> se abre desde el login cuando la clave
    /// actual es temporal: si se cancela, no se entra al sistema. La validación real la hace la controladora.
    /// </summary>
    public partial class FrmCambiarClave : Form
    {
        private readonly int usuarioId;

        public FrmCambiarClave(int usuarioId, bool obligatorio = false)
        {
            InitializeComponent();
            this.usuarioId = usuarioId;

            foreach (var txt in new[] { txtClaveActual, txtClaveNueva, txtConfirmar })
                txt.UseSystemPasswordChar = true;

            Text = obligatorio ? "Debe elegir una clave nueva" : "Cambiar clave";
            label1.Text = obligatorio ? "Clave recibida:" : "Clave actual:";
            AcceptButton = btnAceptar;
            CancelButton = btnCancelar;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (txtClaveActual.Text == string.Empty || txtClaveNueva.Text == string.Empty || txtConfirmar.Text == string.Empty)
            {
                MessageBox.Show(this, "Debe completar todos los campos");
                return;
            }
            if (txtClaveNueva.Text != txtConfirmar.Text)
            {
                MessageBox.Show(this, "Las claves no coinciden");
                return;
            }

            try
            {
                ControladoraUsuarios.Instancia.CambiarClave(usuarioId, txtClaveActual.Text, txtClaveNueva.Text);
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo cambiar la clave.");
                return;
            }

            MessageBox.Show(this, "Clave modificada con éxito");
            DialogResult = DialogResult.OK;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
