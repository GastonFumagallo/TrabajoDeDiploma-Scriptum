using Controladora;

namespace Vista
{
    /// <summary>
    /// Ve pasar todos los mensajes de mouse y teclado de la aplicación y reinicia el contador de
    /// inactividad de la sesión. No consume los mensajes: siempre devuelve false.
    /// </summary>
    internal sealed class FiltroActividadUsuario : IMessageFilter
    {
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_MBUTTONDOWN = 0x0207;
        private const int WM_MOUSEWHEEL = 0x020A;

        public bool PreFilterMessage(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_KEYDOWN:
                case WM_SYSKEYDOWN:
                case WM_MOUSEMOVE:
                case WM_LBUTTONDOWN:
                case WM_RBUTTONDOWN:
                case WM_MBUTTONDOWN:
                case WM_MOUSEWHEEL:
                    ControladoraSesiones.Instancia.RegistrarActividad();
                    break;
            }
            return false;
        }
    }
}
