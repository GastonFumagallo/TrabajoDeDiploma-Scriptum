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
    /// Modal único de alta y edición de proveedores: <c>new FrmEditarProveedor(null)</c> = alta,
    /// <c>new FrmEditarProveedor(id)</c> = edición. Devuelve <see cref="DialogResult.OK"/> y el id en <see cref="ProveedorId"/>.
    ///
    /// El ciclo de vida (carga, título, cambios sin guardar, guardado, reactivación y concurrencia) lo maneja
    /// <see cref="EdicionAbm{TEdicion}"/>; acá sólo se vuelcan datos a controles y se valida el formulario.
    /// </summary>
    public partial class FrmEditarProveedor : Form
    {
        private const string SinCondicion = "(sin especificar)";

        private readonly IProveedorService servicio = ProveedorService.Instancia;
        private readonly ValidadorFormulario validador;
        private readonly EdicionAbm<ProveedorEdicionDTO> edicion;
        private readonly int? idInicial;
        private int version;

        public int? ProveedorId => edicion.Id;

        public FrmEditarProveedor(int? proveedorId)
        {
            InitializeComponent();
            idInicial = proveedorId;

            cbCondicion.Items.Add(SinCondicion);
            cbCondicion.Items.AddRange(CondicionFiscal.Todas);

            // Mapa "campo del DTO → control": los errores del servicio se marcan en el control correcto.
            validador = new ValidadorFormulario(errorProvider)
                .Mapear(nameof(ProveedorEdicionDTO.CUIT), txtCuit)
                .Mapear(nameof(ProveedorEdicionDTO.RazonSocial), txtRazonSocial)
                .Mapear(nameof(ProveedorEdicionDTO.CondicionFiscal), cbCondicion)
                .Mapear(nameof(ProveedorEdicionDTO.Contacto), txtContacto)
                .Mapear(nameof(ProveedorEdicionDTO.Telefono), txtTelefono)
                .Mapear(nameof(ProveedorEdicionDTO.Email), txtEmail)
                .Mapear(nameof(ProveedorEdicionDTO.Direccion), txtDireccion)
                .Mapear(nameof(ProveedorEdicionDTO.Observaciones), txtObservaciones);

            edicion = new EdicionAbm<ProveedorEdicionDTO>(this, new()
            {
                Validador = validador,
                Guardar = btnGuardar,
                Estado = lblEstado,
                Entidad = "proveedor",
                Descripcion = p => p.RazonSocial,
                Id = p => p.Id,
                Activo = p => p.Activo,
                Obtener = servicio.ObtenerPorIdAsync,
                GuardarServicio = servicio.GuardarAsync,
                Reactivar = servicio.ReactivarAsync,
                Mostrar = Mostrar,
                Armar = Armar,
                ValidarFormulario = ValidarFormulario,
                PrimerControl = txtCuit,
            });

            edicion.Vigilar(txtCuit, txtRazonSocial, cbCondicion, txtContacto, txtTelefono, txtEmail, txtDireccion, txtObservaciones);
            txtCuit.Leave += async (_, _) => await ValidarCuitAsync();
            txtEmail.Leave += (_, _) => ValidarEmailAlSalir();
        }

        private async void FrmEditarProveedor_Load(object sender, EventArgs e) =>
            await edicion.CargarAsync(idInicial, () => new ProveedorEdicionDTO());

        #region Binding DTO ↔ controles

        private void Mostrar(ProveedorEdicionDTO p)
        {
            version = p.Version;
            txtCuit.Text = Proveedor.FormatearCuit(p.CUIT);
            txtRazonSocial.Text = p.RazonSocial;
            cbCondicion.SelectedItem = p.CondicionFiscal is { } c && CondicionFiscal.Todas.Contains(c) ? c : SinCondicion;
            txtContacto.Text = p.Contacto;
            txtTelefono.Text = p.Telefono;
            txtEmail.Text = p.Email;
            txtDireccion.Text = p.Direccion;
            txtObservaciones.Text = p.Observaciones;
            lblCuitInfo.Text = "11 dígitos (se aceptan guiones).";
        }

        private ProveedorEdicionDTO Armar() => new()
        {
            Id = edicion.Id,
            Version = version,
            CUIT = txtCuit.Text,
            RazonSocial = txtRazonSocial.Text,
            CondicionFiscal = cbCondicion.SelectedItem as string is { } c && c != SinCondicion ? c : null,
            Contacto = txtContacto.Text,
            Telefono = txtTelefono.Text,
            Email = txtEmail.Text,
            Direccion = txtDireccion.Text,
            Observaciones = txtObservaciones.Text,
        };

        #endregion

        #region Validación (primera capa: feedback inmediato)

        private bool ValidarFormulario()
        {
            validador.Limpiar();
            string? cuit = Identificadores.SoloDigitos(txtCuit.Text);

            validador.Regla(txtCuit, cuit == null, "El CUIT es obligatorio.");
            validador.Regla(txtCuit, cuit != null && !Identificadores.EsCuitValido(cuit), "El CUIT no es válido.");
            validador.Requerido(txtRazonSocial, "La razón social");
            validador.Requerido(txtContacto, "El nombre de contacto");
            validador.Requerido(txtTelefono, "El teléfono");
            validador.Regla(txtTelefono, !string.IsNullOrWhiteSpace(txtTelefono.Text) && !Identificadores.EsTelefonoValido(txtTelefono.Text),
                "El teléfono no es válido (sólo números, espacios, guiones, paréntesis y +).");
            validador.Regla(txtEmail, !string.IsNullOrWhiteSpace(txtEmail.Text) && !Identificadores.EsEmailValido(txtEmail.Text),
                "El email no tiene un formato válido.");

            validador.EnfocarPrimero();
            return validador.EsValido;
        }

        /// <summary>Al salir del CUIT: formato con dígito verificador y unicidad contra la base (sin bloquear).</summary>
        private async Task ValidarCuitAsync()
        {
            validador.Limpiar(txtCuit);
            string? cuit = Identificadores.SoloDigitos(txtCuit.Text);
            if (cuit == null)
            {
                lblCuitInfo.Text = "11 dígitos (se aceptan guiones).";
                return;
            }
            if (!Identificadores.EsCuitValido(cuit))
            {
                validador.Marcar(txtCuit, "El CUIT no es válido: revisá los dígitos (el último es verificador).");
                lblCuitInfo.Text = "CUIT inválido.";
                return;
            }

            txtCuit.Text = Proveedor.FormatearCuit(cuit);   // se muestra normalizado: 30-12345678-9
            try
            {
                if (await servicio.ExisteIdentificadorAsync(cuit, edicion.Id, edicion.Token))
                {
                    validador.Marcar(txtCuit, "Ya hay otro proveedor con este CUIT.");
                    lblCuitInfo.Text = "Ya existe otro proveedor con este CUIT.";
                }
                else
                {
                    lblCuitInfo.Text = "CUIT válido ✓";
                }
            }
            catch
            {
                // La verificación en vivo es una ayuda: al guardar, el servicio vuelve a validar.
            }
        }

        private void ValidarEmailAlSalir()
        {
            validador.Limpiar(txtEmail);
            if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !Identificadores.EsEmailValido(txtEmail.Text))
                validador.Marcar(txtEmail, "El email no tiene un formato válido.");
        }

        #endregion
    }
}
