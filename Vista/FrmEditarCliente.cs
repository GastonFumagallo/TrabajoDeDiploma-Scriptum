using Controladora.Abm;
using Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Vista.Comun;

namespace Vista
{
    /// <summary>
    /// Modal único de alta y edición de clientes (misma estructura que <see cref="FrmEditarProveedor"/>).
    /// <c>new FrmEditarCliente(null)</c> = alta; <c>new FrmEditarCliente(id)</c> = edición.
    /// Devuelve <see cref="DialogResult.OK"/> y el id en <see cref="ClienteId"/>. También se usa como alta rápida
    /// desde el punto de venta.
    /// </summary>
    public partial class FrmEditarCliente : Form
    {
        private readonly ClienteService servicio = ClienteService.Instancia;
        private readonly ValidadorFormulario validador;
        private readonly EdicionAbm<ClienteEdicionDTO> edicion;
        private readonly int? idInicial;
        private int version;

        public int? ClienteId => edicion.Id;

        private bool EsCuit => cbTipoDocumento.SelectedItem as string == TipoDocumento.CUIT;

        public FrmEditarCliente(int? clienteId)
        {
            InitializeComponent();
            idInicial = clienteId;

            cbTipoDocumento.Items.AddRange(new object[] { TipoDocumento.DNI, TipoDocumento.CUIT });
            cbTipoDocumento.SelectedIndex = 0;
            cbTipoDocumento.SelectedIndexChanged += (_, _) => AjustarEtiquetasPorTipo();

            validador = new ValidadorFormulario(errorProvider)
                .Mapear(nameof(ClienteEdicionDTO.Documento), txtDocumento)
                .Mapear(nameof(ClienteEdicionDTO.Nombre), txtNombre)
                .Mapear(nameof(ClienteEdicionDTO.Telefono), txtTelefono)
                .Mapear(nameof(ClienteEdicionDTO.Email), txtEmail)
                .Mapear(nameof(ClienteEdicionDTO.Direccion), txtDireccion)
                .Mapear(nameof(ClienteEdicionDTO.Localidad), cbLocalidad)
                .Mapear(nameof(ClienteEdicionDTO.LimiteCredito), numLimite);

            edicion = new EdicionAbm<ClienteEdicionDTO>(this, new()
            {
                Validador = validador,
                Guardar = btnGuardar,
                Estado = lblEstado,
                Entidad = "cliente",
                Descripcion = c => c.Nombre,
                Id = c => c.Id,
                Activo = c => c.Activo,
                Obtener = servicio.ObtenerPorIdAsync,
                GuardarServicio = servicio.GuardarAsync,
                Reactivar = servicio.ReactivarAsync,
                Mostrar = Mostrar,
                Armar = Armar,
                ValidarFormulario = ValidarFormulario,
                PrimerControl = txtDocumento,
            });

            edicion.Vigilar(cbTipoDocumento, txtDocumento, txtNombre, txtTelefono, txtEmail, txtDireccion, cbLocalidad, numLimite);
            txtDocumento.Leave += async (_, _) => await ValidarDocumentoAsync();
            txtEmail.Leave += (_, _) => ValidarAlSalir(txtEmail, Identificadores.EsEmailValido, "El email no tiene un formato válido.");
            txtTelefono.Leave += (_, _) => ValidarAlSalir(txtTelefono, Identificadores.EsTelefonoValido,
                "El teléfono no es válido (sólo números, espacios, guiones, paréntesis y +).");
        }

        private async void FrmEditarCliente_Load(object sender, EventArgs e)
        {
            try
            {
                // Localidades existentes como sugerencia (se puede escribir una nueva).
                var localidades = await servicio.ObtenerLocalidadesAsync(edicion.Token);
                cbLocalidad.Items.AddRange(localidades.ToArray());
            }
            catch
            {
                // Las sugerencias son opcionales.
            }
            await edicion.CargarAsync(idInicial, () => new ClienteEdicionDTO());
        }

        #region Binding DTO ↔ controles

        private void Mostrar(ClienteEdicionDTO c)
        {
            version = c.Version;
            cbTipoDocumento.SelectedItem = c.TipoDocumento;
            txtDocumento.Text = c.TipoDocumento == TipoDocumento.CUIT ? Proveedor.FormatearCuit(c.Documento) : c.Documento;
            txtNombre.Text = c.Nombre;
            txtTelefono.Text = c.Telefono;
            txtEmail.Text = c.Email;
            txtDireccion.Text = c.Direccion;
            cbLocalidad.Text = c.Localidad;
            numLimite.Value = Math.Min(numLimite.Maximum, c.LimiteCredito);
            AjustarEtiquetasPorTipo();

            // "Consumidor Final" es de sistema: se puede ver pero no modificar.
            if (c.EsConsumidorFinal)
            {
                gbDatos.Enabled = false;
                btnGuardar.Enabled = false;
                lblEstado.Text = "Cliente del sistema: no se puede modificar.";
            }
        }

        private ClienteEdicionDTO Armar() => new()
        {
            Id = edicion.Id,
            Version = version,
            TipoDocumento = cbTipoDocumento.SelectedItem as string ?? TipoDocumento.DNI,
            Documento = txtDocumento.Text,
            Nombre = txtNombre.Text,
            Telefono = txtTelefono.Text,
            Email = txtEmail.Text,
            Direccion = txtDireccion.Text,
            Localidad = cbLocalidad.Text,
            LimiteCredito = numLimite.Value,
        };

        private void AjustarEtiquetasPorTipo()
        {
            lblNombre.Text = EsCuit ? "Razón social *" : "Nombre y apellido *";
            lblDocumentoInfo.Text = EsCuit ? "11 dígitos (se aceptan guiones)." : "7 u 8 dígitos.";
        }

        #endregion

        #region Validación (primera capa: feedback inmediato)

        private bool ValidarFormulario()
        {
            validador.Limpiar();
            string? documento = Identificadores.SoloDigitos(txtDocumento.Text);

            validador.Regla(txtDocumento, documento == null, "El documento es obligatorio.");
            validador.Regla(txtDocumento, documento != null && EsCuit && !Identificadores.EsCuitValido(documento), "El CUIT no es válido.");
            validador.Regla(txtDocumento, documento != null && !EsCuit && !Identificadores.EsDniValido(documento), "El DNI debe tener 7 u 8 dígitos.");
            validador.Requerido(txtNombre, EsCuit ? "La razón social" : "El nombre y apellido");
            validador.Regla(txtTelefono, !string.IsNullOrWhiteSpace(txtTelefono.Text) && !Identificadores.EsTelefonoValido(txtTelefono.Text),
                "El teléfono no es válido.");
            validador.Regla(txtEmail, !string.IsNullOrWhiteSpace(txtEmail.Text) && !Identificadores.EsEmailValido(txtEmail.Text),
                "El email no tiene un formato válido.");

            validador.EnfocarPrimero();
            return validador.EsValido;
        }

        private async Task ValidarDocumentoAsync()
        {
            validador.Limpiar(txtDocumento);
            string? documento = Identificadores.SoloDigitos(txtDocumento.Text)?.TrimStart('0');
            if (string.IsNullOrEmpty(documento)) return;

            bool valido = EsCuit ? Identificadores.EsCuitValido(documento) : Identificadores.EsDniValido(documento);
            if (!valido)
            {
                validador.Marcar(txtDocumento, EsCuit ? "El CUIT no es válido: revisá los dígitos." : "El DNI debe tener 7 u 8 dígitos.");
                return;
            }

            try
            {
                if (await servicio.ExisteIdentificadorAsync(documento, edicion.Id, edicion.Token))
                {
                    validador.Marcar(txtDocumento, "Ya hay otro cliente con este documento.");
                    lblDocumentoInfo.Text = "Ya existe otro cliente.";
                }
                else
                {
                    lblDocumentoInfo.Text = $"{(EsCuit ? "CUIT" : "DNI")} válido ✓";
                }
            }
            catch
            {
                // Ayuda en vivo: al guardar, el servicio vuelve a validar.
            }
        }

        private void ValidarAlSalir(Control control, Func<string?, bool> esValido, string mensaje)
        {
            validador.Limpiar(control);
            if (!string.IsNullOrWhiteSpace(control.Text) && !esValido(control.Text))
                validador.Marcar(control, mensaje);
        }

        #endregion
    }
}
