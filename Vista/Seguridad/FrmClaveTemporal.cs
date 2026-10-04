using Modelo.Seguridad;

namespace Vista.Seguridad
{
    /// <summary>
    /// Muestra una clave temporal una única vez, sólo cuando no se pudo enviar por email, para que el operador
    /// se la entregue en persona al usuario. No queda guardada en ningún lado en texto plano.
    /// </summary>
    public partial class FrmClaveTemporal : Form
    {
        public FrmClaveTemporal(string usuario, string claveTemporal)
        {
            InitializeComponent();
            lblMensaje.Text = $"No se pudo enviar el email (revisá la configuración SMTP). Entregale esta clave temporal a " +
                              $"\"{usuario}\" en persona: no se vuelve a mostrar. Va a tener que cambiarla al iniciar sesión.";
            txtClave.Text = claveTemporal;
            btnCopiar.Click += (_, _) =>
            {
                try
                {
                    Clipboard.SetText(claveTemporal);
                    btnCopiar.Text = "Copiada ✓";
                }
                catch (System.Runtime.InteropServices.ExternalException)
                {
                    // El portapapeles puede estar ocupado por otra aplicación: se puede copiar a mano.
                }
            };
            FormClosed += (_, _) => txtClave.Clear();
        }

        /// <summary>Muestra el resultado de un alta o blanqueo: un aviso si el mail salió, o la clave si no.</summary>
        public static void Mostrar(IWin32Window owner, string usuario, ResultadoClaveTemporal resultado)
        {
            if (resultado.MailEnviado || resultado.ClaveTemporal is null)
            {
                MessageBox.Show(owner, $"Se envió la clave temporal de \"{usuario}\" por email. Deberá cambiarla al iniciar sesión.",
                    "Clave temporal enviada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using var dialogo = new FrmClaveTemporal(usuario, resultado.ClaveTemporal);
            dialogo.ShowDialog(owner);
        }
    }
}
