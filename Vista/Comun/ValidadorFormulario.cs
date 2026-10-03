using Modelo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Vista.Comun
{
    /// <summary>
    /// Validación de formularios con ErrorProvider. Marca cada control inválido con su mensaje, enfoca el primero
    /// y también sabe mostrar los errores que devuelve el servicio (<see cref="ValidacionException"/>) en el
    /// control correspondiente, usando el mapa "propiedad del DTO → control".
    /// </summary>
    internal sealed class ValidadorFormulario
    {
        private readonly ErrorProvider errorProvider;
        private readonly Dictionary<string, Control> controlesPorCampo = new();
        private readonly List<Control> invalidos = new();

        public ValidadorFormulario(ErrorProvider errorProvider)
        {
            this.errorProvider = errorProvider;
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        }

        /// <summary>Asocia un campo del DTO (ej. nameof(LibroEdicionDTO.ISBN)) con el control que lo edita.</summary>
        public ValidadorFormulario Mapear(string campo, Control control)
        {
            controlesPorCampo[campo] = control;
            return this;
        }

        public bool EsValido => invalidos.Count == 0;

        public void Limpiar()
        {
            errorProvider.Clear();
            invalidos.Clear();
        }

        /// <summary>Limpia el error de un solo control (útil al corregir un campo).</summary>
        public void Limpiar(Control control)
        {
            errorProvider.SetError(control, string.Empty);
            invalidos.Remove(control);
        }

        public void Requerido(Control control, string etiqueta)
        {
            if (string.IsNullOrWhiteSpace(control.Text))
                Marcar(control, $"{etiqueta} es obligatorio.");
        }

        /// <summary>Marca el control si <paramref name="invalido"/> es true (y no tenía ya otro error).</summary>
        public void Regla(Control control, bool invalido, string mensaje)
        {
            if (invalido && !invalidos.Contains(control))
                Marcar(control, mensaje);
        }

        public void Marcar(Control control, string mensaje)
        {
            errorProvider.SetIconPadding(control, 2);
            errorProvider.SetError(control, mensaje);
            if (!invalidos.Contains(control)) invalidos.Add(control);
        }

        /// <summary>Lleva los errores del servicio a los controles. Los campos sin control se devuelven como texto.</summary>
        public string? MostrarErrores(ValidacionException ex)
        {
            Limpiar();
            var sinControl = new List<string>();
            foreach (var (campo, mensaje) in ex.Errores)
            {
                if (controlesPorCampo.TryGetValue(campo, out var control)) Marcar(control, mensaje);
                else sinControl.Add(mensaje);
            }
            EnfocarPrimero();
            return sinControl.Count > 0 ? string.Join(Environment.NewLine, sinControl) : null;
        }

        /// <summary>Lleva el foco al primer control inválido (en orden de tabulación) y, si está en otra pestaña, la muestra.</summary>
        public void EnfocarPrimero()
        {
            var primero = invalidos.OrderBy(c => c.TabIndex).FirstOrDefault();
            if (primero == null) return;
            for (var padre = primero.Parent; padre != null; padre = padre.Parent)
                if (padre is TabPage tab && tab.Parent is TabControl tc) tc.SelectedTab = tab;
            primero.Focus();
        }
    }
}
