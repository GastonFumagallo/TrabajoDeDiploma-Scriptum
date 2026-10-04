using Vista.Comun;
using Vista.Theme;

namespace Vista
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Moneda, separadores y fechas de la aplicación, sin depender del formato regional de Windows.
            CulturaRegional.Configurar();
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            FuturisticTheme.Initialize();
            // El timeout de sesión cuenta desde la última interacción con mouse o teclado.
            Application.AddMessageFilter(new FiltroActividadUsuario());
            Application.Run(new FrmIniciarSesión());
        }
    }
}