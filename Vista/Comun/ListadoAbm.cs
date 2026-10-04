using Modelo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Vista.Comun
{
    /// <summary>
    /// Comportamiento común de los formularios de gestión de ABM (Libros, Proveedores, Clientes), para que los
    /// tres se usen exactamente igual sin duplicar código:
    /// <list type="bullet">
    /// <item>Búsqueda en tiempo real (espera 300 ms sin tipear; cancela la consulta anterior).</item>
    /// <item>Filtro de estado: Todos / Solo activos / Inactivos.</item>
    /// <item>Atajos: F2 nuevo · F3 o Enter o doble clic modificar · F4 o Supr dar de baja/reactivar · F5 refrescar · Ctrl+P imprimir.</item>
    /// <item>Modal de alta/edición, recarga sin parpadeo y re-selección del registro guardado.</item>
    /// <item>Inactivos en gris y cursiva; exportar a Excel e imprimir la grilla tal como se ve.</item>
    /// </list>
    /// El formulario sólo define columnas, filtros propios y qué servicio llamar.
    /// </summary>
    internal sealed class ListadoAbm<TListado> where TListado : class
    {
        public sealed class Opciones
        {
            public required DataGridView Grilla { get; init; }
            public required TextBox Buscar { get; init; }
            public required ComboBox Estado { get; init; }
            public required Label Resumen { get; init; }
            public required Button Nuevo { get; init; }
            public required Button Editar { get; init; }
            public required Button CambiarEstado { get; init; }
            public Button? Exportar { get; init; }
            public Button? Imprimir { get; init; }

            /// <summary>"libro", "proveedor", "cliente" (para mensajes).</summary>
            public required string Entidad { get; init; }
            /// <summary>"libros", "proveedores", "clientes".</summary>
            public required string EntidadPlural { get; init; }

            public required Func<TListado, int> Id { get; init; }
            public required Func<TListado, bool> Activo { get; init; }
            public required Func<TListado, string> Descripcion { get; init; }
            /// <summary>Registros de sistema que no se pueden dar de baja ni editar (ej. Consumidor Final).</summary>
            public Func<TListado, bool>? Protegido { get; init; }

            /// <summary>Consulta al servicio. Recibe texto y estado; los filtros propios del formulario se capturan en el lambda.</summary>
            public required Func<string, FiltroEstadoActivo, CancellationToken, Task<List<TListado>>> Cargar { get; init; }
            /// <summary>Abre el modal (null = alta) y devuelve el id guardado, o null si se canceló.</summary>
            public required Func<int?, int?> AbrirEdicion { get; init; }
            public required Func<int, bool, CancellationToken, Task> CambiarEstadoServicio { get; init; }
            /// <summary>Texto extra para la confirmación de baja (ej. "Tiene 3 órdenes activas.").</summary>
            public Func<TListado, string?>? AdvertenciaBaja { get; init; }
        }

        private sealed record OpcionEstado(string Texto, FiltroEstadoActivo Valor)
        {
            public override string ToString() => Texto;
        }

        private readonly Form form;
        private readonly Opciones o;
        private readonly BindingSource bs = new();
        private readonly System.Windows.Forms.Timer timer = new() { Interval = 300 };
        private readonly CancellationTokenSource ctsForm = new();
        private CancellationTokenSource? ctsCarga;
        private Font? fuenteInactivo;
        private bool listo;

        public ListadoAbm(Form form, Opciones opciones)
        {
            this.form = form;
            o = opciones;

            o.Grilla.DataSource = bs;
            o.Estado.DropDownStyle = ComboBoxStyle.DropDownList;
            o.Estado.Items.AddRange(new object[]
            {
                new OpcionEstado("Todos", FiltroEstadoActivo.Todos),
                new OpcionEstado("Solo activos", FiltroEstadoActivo.Activos),
                new OpcionEstado("Inactivos", FiltroEstadoActivo.Inactivos),
            });
            o.Estado.SelectedIndex = 1;

            o.Buscar.TextChanged += (_, _) => { timer.Stop(); timer.Start(); };
            o.Buscar.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Down && o.Grilla.Rows.Count > 0) { e.SuppressKeyPress = true; o.Grilla.Focus(); }
            };
            timer.Tick += async (_, _) => { timer.Stop(); await CargarAsync(); };
            o.Estado.SelectedIndexChanged += async (_, _) => await CargarAsync();

            o.Nuevo.Click += async (_, _) => await NuevoAsync();
            o.Editar.Click += async (_, _) => await EditarAsync();
            o.CambiarEstado.Click += async (_, _) => await CambiarEstadoAsync();
            if (o.Exportar != null) o.Exportar.Click += async (_, _) => await ExportarAsync();
            if (o.Imprimir != null) o.Imprimir.Click += (_, _) => Imprimir();

            o.Grilla.SelectionChanged += (_, _) => ActualizarAcciones();
            o.Grilla.CellDoubleClick += async (_, e) => { if (e.RowIndex >= 0) await EditarAsync(); };
            o.Grilla.CellFormatting += (_, e) =>
            {
                if (e.RowIndex < 0 || e.CellStyle == null || o.Grilla.Rows[e.RowIndex].DataBoundItem is not TListado item) return;
                if (!o.Activo(item))
                {
                    e.CellStyle.ForeColor = Color.Gray;
                    e.CellStyle.Font = fuenteInactivo ??= new Font(o.Grilla.Font, FontStyle.Italic);
                }
            };

            form.KeyPreview = true;
            form.KeyDown += Form_KeyDown;
            form.FormClosing += (_, _) => { timer.Stop(); ctsForm.Cancel(); };
            form.Disposed += (_, _) => { timer.Dispose(); fuenteInactivo?.Dispose(); ctsForm.Dispose(); };
        }

        public CancellationToken Token => ctsForm.Token;
        public TListado? Seleccionado => o.Grilla.CurrentRow?.DataBoundItem as TListado;
        public IReadOnlyList<TListado> Filas => bs.DataSource as List<TListado> ?? new List<TListado>();

        /// <summary>Habilita la carga (llamar al final del Load, cuando los combos propios ya tienen datos).</summary>
        public async Task IniciarAsync()
        {
            listo = true;
            await CargarAsync();
            o.Buscar.Focus();
        }

        /// <summary>Recarga con los filtros actuales, conservando la fila seleccionada (o la indicada).</summary>
        public async Task CargarAsync(int? seleccionarId = null)
        {
            if (!listo) return;

            ctsCarga?.Cancel();
            ctsCarga = CancellationTokenSource.CreateLinkedTokenSource(ctsForm.Token);
            var token = ctsCarga.Token;
            int? idActual = seleccionarId ?? (Seleccionado is TListado s ? o.Id(s) : null);
            var estado = (o.Estado.SelectedItem as OpcionEstado)?.Valor ?? FiltroEstadoActivo.Activos;

            try
            {
                form.UseWaitCursor = true;
                var filas = await o.Cargar(o.Buscar.Text, estado, token);
                if (token.IsCancellationRequested) return;

                bs.DataSource = filas;   // reemplazo único del origen: sin parpadeo ni columnas regeneradas
                o.Resumen.Text = filas.Count == 0
                    ? $"No hay {o.EntidadPlural} que coincidan con los filtros."
                    : $"{filas.Count} {o.EntidadPlural} · F2 nuevo · F3 modificar · F4 dar de baja/reactivar · Ctrl+P imprimir";
                if (idActual is int id)
                    GrillaHelper.Seleccionar<TListado>(o.Grilla, x => o.Id(x) == id);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(form, ex, $"No se pudo cargar el listado de {o.EntidadPlural}.");
            }
            finally
            {
                if (!token.IsCancellationRequested)
                {
                    form.UseWaitCursor = false;
                    ActualizarAcciones();
                }
            }
        }

        /// <summary>Vuelve al estado inicial de filtros (los propios del formulario se limpian antes de llamarlo).</summary>
        public async Task LimpiarFiltrosAsync()
        {
            listo = false;
            o.Buscar.Clear();
            o.Estado.SelectedIndex = 1;
            listo = true;
            await CargarAsync();
            o.Buscar.Focus();
        }

        private void ActualizarAcciones()
        {
            var item = Seleccionado;
            bool protegido = item != null && (o.Protegido?.Invoke(item) ?? false);
            o.Editar.Enabled = item != null && !protegido;
            o.CambiarEstado.Enabled = item != null && !protegido;
            o.CambiarEstado.Text = item != null && !o.Activo(item) ? "Reactivar (F4)" : "Dar de baja (F4)";
            if (o.Exportar != null) o.Exportar.Enabled = bs.Count > 0;
            if (o.Imprimir != null) o.Imprimir.Enabled = bs.Count > 0;
        }

        private async Task NuevoAsync()
        {
            if (!o.Nuevo.Visible || !o.Nuevo.Enabled) return;
            int? id = o.AbrirEdicion(null);
            if (id != null) await CargarAsync(id);
        }

        private async Task EditarAsync()
        {
            if (!o.Editar.Visible || !o.Editar.Enabled || Seleccionado is not TListado item) return;
            int? id = o.AbrirEdicion(o.Id(item));
            if (id != null) await CargarAsync(id);
        }

        private async Task CambiarEstadoAsync()
        {
            if (!o.CambiarEstado.Visible || !o.CambiarEstado.Enabled || Seleccionado is not TListado item) return;

            bool activar = !o.Activo(item);
            string mensaje = activar
                ? $"¿Reactivar el {o.Entidad} \"{o.Descripcion(item)}\"?"
                : $"¿Dar de baja el {o.Entidad} \"{o.Descripcion(item)}\"?\n\n" +
                  "• No se elimina: queda inactivo y conserva todo su historial.\n" +
                  "• No se podrá usar en operaciones nuevas.\n" +
                  "• Se puede reactivar en cualquier momento." +
                  (o.AdvertenciaBaja?.Invoke(item) is string extra ? $"\n\nAtención: {extra}" : "");

            if (MessageBox.Show(form, mensaje, activar ? "Reactivar" : "Dar de baja", MessageBoxButtons.YesNo,
                    activar ? MessageBoxIcon.Question : MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            try
            {
                await o.CambiarEstadoServicio(o.Id(item), activar, ctsForm.Token);
                await CargarAsync(o.Id(item));
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(form, ex, $"No se pudo cambiar el estado del {o.Entidad}.");
            }
        }

        private async Task ExportarAsync()
        {
            using var dialogo = new SaveFileDialog
            {
                Filter = "Archivo CSV (Excel)|*.csv",
                FileName = $"{char.ToUpper(o.EntidadPlural[0])}{o.EntidadPlural[1..]}_{DateTime.Now:yyyyMMdd_HHmm}.csv",
            };
            if (dialogo.ShowDialog(form) != DialogResult.OK) return;

            try
            {
                int filas = await ExportadorGrilla.ExportarCsvAsync(o.Grilla, dialogo.FileName, ctsForm.Token);
                MessageBox.Show(form, $"Se exportaron {filas} {o.EntidadPlural}.", "Exportación completa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (IOException ex)
            {
                MessageBox.Show(form, $"No se pudo escribir el archivo. ¿Está abierto en Excel?\n\n{ex.Message}", "Exportar",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(form, ex, "No se pudo exportar el listado.");
            }
        }

        private void Imprimir()
        {
            if (bs.Count == 0) return;
            string titulo = $"Listado de {o.EntidadPlural}";
            ImpresorGrilla.Previsualizar(form, o.Grilla, titulo, o.Resumen.Text.Split('·')[0].Trim());
        }

        private async void Form_KeyDown(object? sender, KeyEventArgs e)
        {
            // Si hay un combo desplegado o se está escribiendo en otro campo, Enter/Supr no se interceptan.
            bool enGrilla = o.Grilla.Focused;
            switch (e.KeyCode)
            {
                case Keys.F2:
                    e.Handled = true; await NuevoAsync(); break;
                case Keys.F3:
                case Keys.Enter when enGrilla:
                    e.Handled = e.SuppressKeyPress = true; await EditarAsync(); break;
                case Keys.F4:
                case Keys.Delete when enGrilla:
                    e.Handled = true; await CambiarEstadoAsync(); break;
                case Keys.F5:
                    e.Handled = true; await CargarAsync(); break;
                case Keys.P when e.Control:
                    e.Handled = e.SuppressKeyPress = true; Imprimir(); break;
            }
        }
    }
}
