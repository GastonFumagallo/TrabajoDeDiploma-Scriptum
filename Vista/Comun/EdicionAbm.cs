using Modelo;
using Servicios;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Vista.Comun
{
    /// <summary>
    /// Comportamiento común de los modales de alta/edición (un único formulario para ambos casos):
    /// carga asíncrona, título dinámico, detección de cambios sin guardar, guardado con manejo uniforme de
    /// errores (validación → ErrorProvider, registro inactivo → ofrecer reactivar, concurrencia → recargar)
    /// y cierre con <see cref="DialogResult.OK"/> para que el listado recargue.
    /// </summary>
    internal sealed class EdicionAbm<TEdicion> where TEdicion : class
    {
        public sealed class Opciones
        {
            public required ValidadorFormulario Validador { get; init; }
            public required Button Guardar { get; init; }
            public required Label Estado { get; init; }

            /// <summary>"proveedor", "cliente"...</summary>
            public required string Entidad { get; init; }
            /// <summary>Texto del registro para el título ("Editar proveedor: {Descripcion}").</summary>
            public required Func<TEdicion, string> Descripcion { get; init; }
            public required Func<TEdicion, int?> Id { get; init; }
            public required Func<TEdicion, bool> Activo { get; init; }

            public required Func<int, CancellationToken, Task<TEdicion?>> Obtener { get; init; }
            public required Func<TEdicion, string, CancellationToken, Task<int>> GuardarServicio { get; init; }
            public required Func<int, CancellationToken, Task<TEdicion?>> Reactivar { get; init; }

            /// <summary>Vuelca el DTO en los controles.</summary>
            public required Action<TEdicion> Mostrar { get; init; }
            /// <summary>Arma el DTO desde los controles (incluyendo Id y Version).</summary>
            public required Func<TEdicion> Armar { get; init; }
            /// <summary>Validación de formulario (primera capa). Devuelve true si se puede llamar al servicio.</summary>
            public required Func<bool> ValidarFormulario { get; init; }
            /// <summary>Control que recibe el foco al abrir.</summary>
            public Control? PrimerControl { get; init; }
        }

        private readonly Form form;
        private readonly Opciones o;
        private readonly CancellationTokenSource cts = new();
        private bool cargando = true;
        private bool guardando;
        private bool reactivado;

        public EdicionAbm(Form form, Opciones opciones)
        {
            this.form = form;
            o = opciones;
            o.Guardar.Click += async (_, _) => await GuardarAsync();
            form.FormClosing += Form_FormClosing;
            form.Disposed += (_, _) => cts.Dispose();
        }

        /// <summary>Id del registro guardado o reactivado (válido cuando el diálogo devuelve OK).</summary>
        public int? Id { get; private set; }
        public bool Modificado { get; private set; }
        public bool EsAlta => Id == null;
        public CancellationToken Token => cts.Token;

        /// <summary>Marca la ficha como modificada cuando cambia cualquiera de estos controles (y limpia su error).</summary>
        public void Vigilar(params Control[] controles)
        {
            foreach (var c in controles)
            {
                EventHandler marcar = (_, _) => MarcarModificado(c);
                switch (c)
                {
                    case NumericUpDown n: n.ValueChanged += marcar; break;
                    case ComboBox cb: cb.SelectedIndexChanged += marcar; cb.TextChanged += marcar; break;
                    case CheckBox chk: chk.CheckedChanged += marcar; break;
                    case RadioButton rb: rb.CheckedChanged += marcar; break;
                    default: c.TextChanged += marcar; break;
                }
            }
        }

        public void MarcarModificado(Control? control = null)
        {
            if (cargando) return;
            Modificado = true;
            if (control != null) o.Validador.Limpiar(control);
        }

        /// <summary>Carga el registro (id) o prepara el alta con <paramref name="nuevo"/>.</summary>
        public async Task CargarAsync(int? id, Func<TEdicion> nuevo)
        {
            try
            {
                form.UseWaitCursor = true;
                if (id is int existente)
                {
                    var dto = await o.Obtener(existente, cts.Token);
                    if (dto == null)
                    {
                        MessageBox.Show(form, $"El {o.Entidad} ya no existe.", form.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        form.DialogResult = DialogResult.Cancel;
                        return;
                    }
                    Mostrar(dto);
                }
                else
                {
                    Mostrar(nuevo());
                }
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(form, ex, $"No se pudo abrir la ficha del {o.Entidad}.");
                form.DialogResult = DialogResult.Cancel;
            }
            finally
            {
                form.UseWaitCursor = false;
            }
            o.PrimerControl?.Focus();
        }

        private void Mostrar(TEdicion dto)
        {
            cargando = true;
            Id = o.Id(dto);
            o.Mostrar(dto);

            string entidad = char.ToUpper(o.Entidad[0]) + o.Entidad[1..];
            form.Text = Id == null ? $"Nuevo {o.Entidad}" : $"Editar {o.Entidad}: {o.Descripcion(dto)}";
            o.Estado.Text = o.Activo(dto)
                ? "Los campos con * son obligatorios."
                : $"{entidad} INACTIVO: no se usa en operaciones nuevas. Se reactiva desde el listado (F4).";
            o.Estado.ForeColor = o.Activo(dto) ? SystemColors.ControlText : Color.Firebrick;

            o.Validador.Limpiar();
            cargando = false;
            Modificado = false;
        }

        private async Task GuardarAsync()
        {
            if (guardando || !o.ValidarFormulario()) return;

            guardando = true;
            o.Guardar.Enabled = false;
            form.UseWaitCursor = true;
            try
            {
                string usuario = PermisoService.Instancia.UsuarioActual?.USU_Nombre ?? "desconocido";
                Id = await o.GuardarServicio(o.Armar(), usuario, cts.Token);
                Modificado = false;
                form.DialogResult = DialogResult.OK;
            }
            catch (ValidacionException ex)
            {
                // Errores de negocio (ej. CUIT duplicado): se marcan en su control.
                if (o.Validador.MostrarErrores(ex) is string resto)
                    MessageBox.Show(form, resto, "Revise los datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (EntidadInactivaException ex)
            {
                await OfrecerReactivacionAsync(ex);
            }
            catch (ConcurrenciaException ex)
            {
                if (MessageBox.Show(form, ex.Message + "\n\n¿Recargar los datos actuales? (se perderán tus cambios)",
                        "Datos modificados por otro usuario", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes
                    && Id is int id)
                {
                    var actual = await o.Obtener(id, cts.Token);
                    if (actual != null) Mostrar(actual);
                    else form.DialogResult = DialogResult.Cancel;
                }
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(form, ex, $"No se pudo guardar el {o.Entidad}.");
            }
            finally
            {
                guardando = false;
                o.Guardar.Enabled = true;
                form.UseWaitCursor = false;
            }
        }

        /// <summary>El identificador pertenece a un registro dado de baja: se ofrece reactivarlo en lugar de duplicarlo.</summary>
        private async Task OfrecerReactivacionAsync(EntidadInactivaException ex)
        {
            if (MessageBox.Show(form,
                    $"{ex.Message}\n\n¿Reactivarlo en lugar de crear uno nuevo?\nSe abrirá su ficha para que revises los datos.",
                    $"{char.ToUpper(o.Entidad[0])}{o.Entidad[1..]} existente inactivo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                var ficha = await o.Reactivar(ex.EntidadId, cts.Token);
                if (ficha == null) return;
                Mostrar(ficha);
                reactivado = true;
                o.Estado.Text = $"Se reactivó el {o.Entidad}. Revisá los datos y guardá si hace falta actualizar algo.";
                o.Estado.ForeColor = Color.DarkGreen;
            }
            catch (Exception error)
            {
                ManejadorErrores.Mostrar(form, error, $"No se pudo reactivar el {o.Entidad}.");
            }
        }

        private void Form_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (guardando)
            {
                e.Cancel = true;
                return;
            }

            if (form.DialogResult != DialogResult.OK && Modificado &&
                MessageBox.Show(form, "Hay cambios sin guardar. ¿Descartarlos?", "Cambios sin guardar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            // Si se reactivó un registro, el listado tiene que refrescarse aunque no se haya vuelto a guardar.
            if (form.DialogResult != DialogResult.OK && reactivado)
                form.DialogResult = DialogResult.OK;

            cts.Cancel();
        }
    }
}
