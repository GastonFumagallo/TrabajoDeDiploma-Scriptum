using Controladora;
using Modelo.Seguridad;
using Servicios;
using Vista.Comun;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Vista.Seguridad
{
    public partial class FrmUsuario : Form
    {
        private List<Grupo> gruposSeleccionados;
        private List<Accion> accionesSeleccionadas;
        private Usuario usuario;
        private bool _actualizandoNodos = false;
        public FrmUsuario()
        {
            gruposSeleccionados = new List<Grupo>();
            accionesSeleccionadas = new List<Accion>();
            InitializeComponent();
            LlenarEstados();
            LlenarGrupos();
            LlenarAcciones();
        }

        public FrmUsuario(UsuarioDTO usuario1)
        {
            gruposSeleccionados = new List<Grupo>();
            accionesSeleccionadas = new List<Accion>();
            usuario = ControladoraUsuarios.Instancia.buscarUsuarioIndividual(usuario1);
            InitializeComponent();
            LlenarEstados();
            LlenarCombos();
            LlenarGrupos();
            LlenarAcciones();
        }
        void LlenarEstados()
        {
            cbEstados.DataSource = null;
            cbEstados.DataSource = ControladoraUsuarios.Instancia.getAllEstadosUsuario();
        }
        void LlenarCombos()
        {
            accionesSeleccionadas = usuario.Acciones.ToList();
            gruposSeleccionados = usuario.Grupos.ToList();
            txtNombre.Text = usuario.USU_Persona.PER_Nombre;
            txtUsuario.Text = usuario.USU_Nombre;
            txtEmail.Text = usuario.USU_Mail;
            txtDNI.Text = usuario.USU_Persona.PER_DNI.ToString();
            txtTelefono.Text = usuario.USU_Persona.PER_Telefono;
            var estados = ControladoraUsuarios.Instancia.getAllEstadosUsuario();
            cbEstados.DataSource = estados;
            // Los estados vienen de otra consulta (otras instancias): se selecciona por ID.
            cbEstados.SelectedItem = estados.FirstOrDefault(e => e.EST_USU_ID == usuario.EST_USU_ID);
            txtEmail.Enabled = false;
            txtEmail.BackColor = Color.LightGray;
        }

        void LlenarGrupos()
        {
            var grupos = ControladoraGrupos.Instancia.getAllGrupos();
            clbGrupos.Items.Clear();

            foreach (var grupo in grupos)
            {
                int index = clbGrupos.Items.Add(grupo);
                if (gruposSeleccionados.Any(g => g.GRU_ID == grupo.GRU_ID))
                    clbGrupos.SetItemChecked(index, true);
            }
        }

        void LlenarAcciones()
        {
            var modulos = ControladoraGrupos.Instancia.getAllModulos();
            tvAcciones1.Nodes.Clear();

            var accionesDeGrupos = gruposSeleccionados
                .SelectMany(g => g.Acciones)
                .ToList();

            foreach (var modulo in modulos)
            {
                TreeNode nodoModulo = tvAcciones1.Nodes.Add(modulo.MOD_Nombre);
                nodoModulo.Tag = modulo;
                bool moduloCompleto = true;

                foreach (var formulario in modulo.Formularios)
                {
                    TreeNode nodoFormulario = nodoModulo.Nodes.Add(formulario.FORM_Nombre);
                    nodoFormulario.Tag = formulario;
                    bool formularioCompleto = true;

                    foreach (var accion in formulario.Acciones)
                    {
                        TreeNode nodoAccion = nodoFormulario.Nodes.Add(accion.ACC_Nombre);
                        nodoAccion.Tag = accion;

                        bool tienePermiso = accionesDeGrupos.Any(a => a.ACC_ID == accion.ACC_ID) || (usuario != null && usuario.Acciones.Any(a => a.ACC_ID == accion.ACC_ID));

                        nodoAccion.Checked = tienePermiso;

                        if (!tienePermiso)
                            formularioCompleto = false;
                    }

                    nodoFormulario.Checked = formularioCompleto;

                    if (!formularioCompleto)
                        moduloCompleto = false;
                }

                nodoModulo.Checked = moduloCompleto;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Validaciones()) return;

            if (EsModoAlta())
                GuardarAlta();
            else
                GuardarModificacion();
        }

        private bool EsModoAlta() => txtEmail.Enabled;
        private void GuardarAlta()
        {
            usuario = new Usuario
            {
                USU_Nombre = txtUsuario.Text,
                USU_Mail = txtEmail.Text,
                Estado_Usuario = (Estado_Usuario)cbEstados.SelectedItem,
                // La clave inicial la genera la controladora y se envía por email.

                USU_Persona = new Persona
                {
                    PER_Nombre = txtNombre.Text,
                    PER_Mail = txtEmail.Text,
                    PER_Telefono = txtTelefono.Text,
                    PER_DNI = int.Parse(txtDNI.Text),
                }
            };

            AplicarAccionesYGrupos();

            try
            {
                MessageBox.Show(this, ControladoraUsuarios.Instancia.AgregarUsuario(usuario));
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo agregar el usuario.");
                return;
            }
            VaciarTxt();
            this.Close();
        }

        private void GuardarModificacion()
        {
            usuario.USU_Persona.PER_Nombre = txtNombre.Text;
            usuario.USU_Persona.PER_Telefono = txtTelefono.Text;
            usuario.USU_Persona.PER_DNI = int.Parse(txtDNI.Text);
            usuario.USU_Nombre = txtUsuario.Text;
            usuario.Estado_Usuario = (Estado_Usuario)cbEstados.SelectedItem;

            usuario.Acciones.Clear();
            usuario.Grupos.Clear();

            AplicarAccionesYGrupos();

            try
            {
                var ok = ControladoraUsuarios.Instancia.ModificarUsuario(usuario);
                MessageBox.Show(this, ok ? "Usuario modificado con éxito" : "El usuario no pudo ser modificado");
            }
            catch (Exception ex)
            {
                ManejadorErrores.Mostrar(this, ex, "No se pudo modificar el usuario.");
                return;
            }
            this.Close();
        }

        private void AplicarAccionesYGrupos()
        {
            foreach (var accion in accionesSeleccionadas)
                usuario.AgregarAccion(accion);

            foreach (var grupo in gruposSeleccionados)
                usuario.AgregarGrupo(grupo);
        }


        private void GestionarAccion(TreeNode nodo, Accion accion)
        {
            bool esDeGrupo = gruposSeleccionados
                .SelectMany(g => g.Acciones)
                .Any(a => a.ACC_ID == accion.ACC_ID);

            if (!nodo.Checked)
            {
                if (esDeGrupo)
                {
                    MessageBox.Show("No se puede eliminar una acción asociada a uno de los grupos seleccionados.", "Acción heredada",MessageBoxButtons.OK,MessageBoxIcon.Warning);

                    nodo.Checked = true;
                    return;
                }

                accionesSeleccionadas.RemoveAll(a => a.ACC_ID == accion.ACC_ID);
            }
            else
            {
                bool yaExiste = accionesSeleccionadas
                    .Any(a => a.ACC_ID == accion.ACC_ID);

                if (!yaExiste)
                {
                    accionesSeleccionadas.Add(accion);
                }
            }
        }

        private void MarcarHijos(TreeNode nodo, bool marcado)
        {
            foreach (TreeNode hijo in nodo.Nodes)
            {
                if (hijo.Tag is Accion accion)
                {
                    bool esDeGrupo = gruposSeleccionados
                        .SelectMany(g => g.Acciones)
                        .Any(x => x.ACC_ID == accion.ACC_ID);

                    if (!marcado && esDeGrupo)
                    {
                        hijo.Checked = true;
                        continue;
                    }

                    if (!marcado)
                        accionesSeleccionadas.RemoveAll(a => a.ACC_ID == accion.ACC_ID);
                    else if (!esDeGrupo && !accionesSeleccionadas.Any(a => a.ACC_ID == accion.ACC_ID))
                    {
                        accionesSeleccionadas.Add(accion);
                    }
                }

                hijo.Checked = marcado;

                if (hijo.Nodes.Count > 0)
                    MarcarHijos(hijo, marcado);
            }
        }

        private void ActualizarPadre(TreeNode padre)
        {
            if (padre == null) return;

            padre.Checked = padre.Nodes
                .Cast<TreeNode>()
                .All(n => n.Checked);

            ActualizarPadre(padre.Parent);
        }


        private bool Validaciones()
        {
            string nombre = txtNombre.Text;
            string usuario = txtUsuario.Text;
            string email = txtEmail.Text;

            if (new[] { nombre, usuario, email }.Any(string.IsNullOrWhiteSpace))
            {
                MessageBox.Show("Debe completar todos los campos.");
                return false;
            }

            bool emailValido = System.Text.RegularExpressions.Regex
                .IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            if (!emailValido)
            {
                MessageBox.Show("El correo electrónico no es válido.");
                return false;
            }

            if (!int.TryParse(txtDNI.Text, out _))
            {
                MessageBox.Show("El DNI debe ser numérico.");
                return false;
            }

            if (!long.TryParse(txtTelefono.Text, out _))
            {
                MessageBox.Show("El teléfono debe ser numérico.");
                return false;
            }

            if (!gruposSeleccionados.Any())
            {
                MessageBox.Show("Debe seleccionar al menos un grupo.");
                return false;
            }

            return true;
        }


        private void VaciarTxt()
        {
            txtNombre.Text = string.Empty;
            txtUsuario.Text = string.Empty;
            txtEmail.Text = string.Empty;
        }

        private void tvAcciones1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (_actualizandoNodos) return;
            _actualizandoNodos = true;

            try
            {
                var nodo = e.Node;
                if (nodo == null) return;

                if (nodo.Nodes.Count > 0)
                    MarcarHijos(nodo, nodo.Checked);

                if (nodo.Tag is Accion accion)
                    GestionarAccion(nodo, accion);

                ActualizarPadre(nodo.Parent);
            }
            finally
            {
                _actualizandoNodos = false;
            }
        }

        private void clbGrupos_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (clbGrupos.Items[e.Index] is not Grupo grupo) return;

            if (e.NewValue == CheckState.Checked)
            {
                if (!gruposSeleccionados.Any(g => g.GRU_ID == grupo.GRU_ID))
                    gruposSeleccionados.Add(grupo);
            }
            else
            {
                gruposSeleccionados.RemoveAll(g => g.GRU_ID == grupo.GRU_ID);
            }

            LlenarAcciones();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
