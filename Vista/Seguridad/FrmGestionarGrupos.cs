using Modelo.Seguridad;
using Controladora;
using Vista.Comun;
using Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Vista.Seguridad
{
    public partial class FrmGestionarGrupos : Form
    {
        public FrmGestionarGrupos()
        {
            InitializeComponent();
            LlenarGrilla();
        }

        void LlenarGrilla()
        {
            dgvGrupos.DataSource = null;
            dgvGrupos.DataSource = ControladoraGrupos.Instancia.obtenerGruposGrid();
            dgvGrupos.Columns["GRUDTO_ID"].Visible = false;

            var todos = new Estado_Grupo
            {
                EST_GRU_Nombre = "Todos"
            };

            var estados = ControladoraGrupos.Instancia.getAllEstadosGrupo().ToList();

            estados.Insert(0, todos);

            CBestados.DataSource = null;
            CBestados.DataSource = estados;
            CBestados.SelectedIndex = 0;
        }
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            var form = new FrmGrupo();
            form.ShowDialog();
            LlenarGrilla();
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            if (TopLevelControl is FrmMenu principal)
                principal.MostrarInicio();   // cierra y libera esta sección
            else
                Close();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvGrupos.CurrentRow != null)
            {
                var grupo = (GrupoDTO)dgvGrupos.CurrentRow.DataBoundItem;
                Grupo grupoEliminar = ControladoraGrupos.Instancia.buscarGrupoIndividual(grupo);
                try
                {
                    MessageBox.Show(this, ControladoraGrupos.Instancia.EliminarGrupo(grupoEliminar));
                }
                catch (Exception ex)
                {
                    ManejadorErrores.Mostrar(this, ex, "No se pudo completar la operación.");
                    return;
                }
            }
            LlenarGrilla();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvGrupos.CurrentRow != null)
            {
                var grupo = (GrupoDTO)dgvGrupos.CurrentRow.DataBoundItem;
                Grupo grupoModificar = ControladoraGrupos.Instancia.buscarGrupoIndividual(grupo);
                var form = new FrmGrupo(grupoModificar);
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("Seleccione un grupo para modificar");
                return;
            }
            LlenarGrilla();
        }
        private void actualizarGrillaFiltro()
        {
            int? idEstado = null;

            if (CBestados.SelectedItem != null)
            {
                idEstado = ((Estado_Grupo)CBestados.SelectedItem).EST_GRU_ID;
            }

            dgvGrupos.DataSource = null;
            dgvGrupos.DataSource = ControladoraGrupos.Instancia.FiltrarGrupos(txtNombre.Text, idEstado);
            dgvGrupos.Columns["GRUDTO_ID"].Visible = false;

        }
        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            if (txtNombre.Text == "")
            {
                MessageBox.Show("Ingrese un nombre para filtrar");
                LlenarGrilla();
                return;
            }
            actualizarGrillaFiltro();
        }


        private void txtNombre_TextChanged(object sender, EventArgs e)
        {
            actualizarGrillaFiltro();
        }

        private void CBestados_SelectedIndexChanged(object sender, EventArgs e)
        {
            actualizarGrillaFiltro();

        }
        private void AplicarSeguridad()
        {
            btnAgregar.Visible = PermisoService.Instancia.TienePermiso("AgregarGrupo");
            btnModificar.Visible = PermisoService.Instancia.TienePermiso("ModificarGrupo");
            btnEliminar.Visible = PermisoService.Instancia.TienePermiso("EliminarGrupo");
        }
        private void FrmGestionarGrupos_Load(object sender, EventArgs e)
        {
            AplicarSeguridad();
        }
    }
}
