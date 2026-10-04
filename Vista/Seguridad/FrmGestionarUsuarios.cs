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

namespace Vista.Seguridad
{
    public partial class FrmGestionarUsuarios : Form
    {
        public FrmGestionarUsuarios()
        {
            InitializeComponent();
            LlenarGrilla();
            LlenarCombos();

        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!PermisoService.Instancia.TienePermiso("AgregarUsuario"))
            {
                MessageBox.Show("Necesita permisos");
                return;
            }
            var formUsuario = new FrmUsuario();
            formUsuario.ShowDialog();
            LlenarGrilla();
        }
        void LlenarGrilla()
        {
            dgvUsuarios.DataSource = null;
            dgvUsuarios.DataSource = ControladoraUsuarios.Instancia.obtenerUsuariosGrid();
            dgvUsuarios.Columns["USUDTO_ID"].Visible = false;

        }

        void LlenarCombos()
        {
            var todosEstado = new Estado_Usuario
            {
                EST_USU_Nombre = "Todos"
            };

            var estados = ControladoraUsuarios.Instancia.getAllEstadosUsuario().ToList();

            estados.Insert(0, todosEstado);

            cbEstados.DataSource = null;
            cbEstados.DataSource = estados;
            cbEstados.SelectedIndex = 0;


            var todosGrupo = new Grupo
            {
                GRU_Nombre = "Todos"
            };

            var grupos = ControladoraGrupos.Instancia.getAllGrupos().ToList();

            grupos.Insert(0, todosGrupo);

            cbGrupos.DataSource = null;
            cbGrupos.DataSource = grupos;
            cbGrupos.SelectedIndex = 0;
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (!PermisoService.Instancia.TienePermiso("ModificarUsuario"))
            {
                MessageBox.Show("Necesita permisos");
                return;
            }
            if (dgvUsuarios.CurrentRow != null)
            {
                var usuario = (UsuarioDTO)dgvUsuarios.CurrentRow.DataBoundItem;
                var FormUsuario = new FrmUsuario(usuario);
                FormUsuario.ShowDialog();
                LlenarGrilla();
            }
            else
            {
                MessageBox.Show("Seleccione un usuario para modificar");
                return;
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (!PermisoService.Instancia.TienePermiso("EliminarUsuario"))
            {
                MessageBox.Show("Necesita permisos");
                return;
            }
            if (dgvUsuarios.CurrentRow != null)
            {
                var usuarioDTO = (UsuarioDTO)dgvUsuarios.CurrentRow.DataBoundItem;
                if (MessageBox.Show(this, $"¿Dar de baja al usuario {usuarioDTO.Usuario}? No podrá volver a ingresar, " +
                        "pero se conserva su historial.", "Dar de baja", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                    return;
                try
                {
                    var usuario = ControladoraUsuarios.Instancia.buscarUsuarioIndividual(usuarioDTO);
                    MessageBox.Show(this, ControladoraUsuarios.Instancia.DarDeBajaUsuario(usuario));
                }
                catch (Exception ex)
                {
                    ManejadorErrores.Mostrar(this, ex, "No se pudo dar de baja el usuario.");
                }
            }
            LlenarGrilla();
        }

        private void btnResetear_Click(object sender, EventArgs e)
        {
            if (!PermisoService.Instancia.TienePermiso("ResetearClave"))
            {
                MessageBox.Show("Necesita permisos");
                return;
            }
            if (dgvUsuarios.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un usuario para resetear la clave");
                return;
            }   
            var usuarioDTO = (UsuarioDTO)dgvUsuarios.CurrentRow.DataBoundItem;
            var usuario = ControladoraUsuarios.Instancia.buscarUsuarioIndividual(usuarioDTO);
            DialogResult resul = MessageBox.Show("¿Desea resetear la clave del Usuario?", "Atencion", MessageBoxButtons.YesNo);

            if (resul == DialogResult.Yes)
            {
                try
                {
                    MessageBox.Show(this, ControladoraUsuarios.Instancia.ResetearClaveUsuario(usuario)
                        ? "Se envió una clave temporal a su email registrado. Deberá cambiarla al ingresar."
                        : "No se pudo resetear la clave");
                }
                catch (Exception ex)
                {
                    ManejadorErrores.Mostrar(this, ex, "No se pudo resetear la clave.");
                }
            }
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            ActualizarGrillaFiltros();
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            if (TopLevelControl is FrmMenu principal)
                principal.MostrarInicio();   // cierra y libera esta sección
            else
                Close();
        }

        private void ActualizarGrillaFiltros()
        {
            int? idGrupo = null;
            int? idEstado = null;

            if (cbGrupos.SelectedItem != null)
                idGrupo = ((Grupo)cbGrupos.SelectedItem).GRU_ID;

            if (cbEstados.SelectedItem != null)
                idEstado = ((Estado_Usuario)cbEstados.SelectedItem).EST_USU_ID;

            dgvUsuarios.DataSource = null;
            dgvUsuarios.DataSource =
                ControladoraUsuarios.Instancia.FiltrarUsuarios(
                    txtNombre.Text,
                    idGrupo,
                    idEstado);
            dgvUsuarios.Columns["USUDTO_ID"].Visible = false;

        }

        private void txtNombre_TextChanged(object sender, EventArgs e)
        {
            ActualizarGrillaFiltros();

        }

        private void cbGrupos_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarGrillaFiltros();

        }

        private void cbEstados_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarGrillaFiltros();

        }
    }

}