using Controladora;
using Modelo.Seguridad;
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
    public partial class FrmGrupo : Form
    {
        List<Accion> acciones;
        Grupo grupo;

        public FrmGrupo()
        {
            acciones = new List<Accion>();
            InitializeComponent();
            LlenarAcciones();
            LlenarEstados();
        }
        void LlenarEstados()
        {
            cbEstados.DataSource = null;
            cbEstados.DataSource = ControladoraGrupos.Instancia.getAllEstadosGrupo();

        }

        public FrmGrupo(Grupo Grupo)
        {
            acciones = new List<Accion>();
            grupo = Grupo;
            InitializeComponent();
            LlenarAcciones();
            LlenarEstados();
            LlenarTxt();
        }
        void LlenarTxt()
        {
            txtNombre.Text = grupo.GRU_Nombre.ToString();
            txtDescripcion.Text = grupo.GRU_Descripcion.ToString();
            cbEstados.SelectedItem = grupo.Estado_Grupo;
        }

        void LlenarAcciones()
        {
            var modulos = ControladoraGrupos.Instancia.getAllModulos();
            tvAcciones.Nodes.Clear();
            foreach (var modulo in modulos)
            {
                TreeNode nodoModulo = tvAcciones.Nodes.Add(modulo.MOD_Nombre);
                nodoModulo.Tag = modulo;
                var modulocheck = true;
                foreach (var formulario in modulo.Formularios)
                {
                    TreeNode nodoFormulario = nodoModulo.Nodes.Add(formulario.FORM_Nombre);
                    nodoFormulario.Tag = formulario;
                    var Formcheck = true;
                    foreach (var accion in formulario.Acciones)
                    {
                        TreeNode nodoAccion = nodoFormulario.Nodes.Add(accion.ACC_Nombre);
                        nodoAccion.Tag = accion;
                        if (grupo != null && grupo.Acciones.Any(a => a.ACC_ID == accion.ACC_ID))
                        {
                            nodoAccion.Checked = true;
                        }
                        else
                        {
                            Formcheck = false;
                        }
                    }
                    nodoFormulario.Checked = Formcheck;
                    if (!Formcheck) { modulocheck = false; }
                }
                nodoModulo.Checked = modulocheck;
            }
        }

        bool Validaciones()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtDescripcion.Text) || cbEstados.SelectedItem == null)
            {
                MessageBox.Show("Debe completar todos los campos");
                return false;
            }

            if (txtNombre.Text.Length > 60)
            {
                MessageBox.Show("El nombre es demasiado largo.");
                return false;
            }

            if (txtDescripcion.Text.Length > 500)
            {
                MessageBox.Show("La descripción es demasiado larga.");
                return false;
            }

            if (!acciones.Any())
            {
                MessageBox.Show("Debe seleccionar al menos una acción");
                return false;
            }

            return true;
        }
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            acciones.Clear();

            foreach (TreeNode modulo in tvAcciones.Nodes)
            {
                foreach (TreeNode formulario in modulo.Nodes)
                {
                    foreach (TreeNode accionNode in formulario.Nodes)
                    {
                        if (accionNode.Checked && accionNode.Tag is Accion accion)
                        {
                            if (!acciones.Any(a => a.ACC_ID == accion.ACC_ID))
                            {
                                acciones.Add(accion);
                            }
                        }
                    }
                }
            }

            if (!Validaciones())
            {
                return;
            }

            if (grupo != null)
            {
                grupo.GRU_Nombre = txtNombre.Text;
                grupo.Estado_Grupo = cbEstados.SelectedItem as Estado_Grupo;
                grupo.GRU_Descripcion = txtDescripcion.Text;

                grupo.Acciones.Clear();

                foreach (var ac in acciones)
                {
                    grupo.AgregarAccion(ac);
                }

                var msj = ControladoraGrupos.Instancia.ModificarGrupo(grupo);
                MessageBox.Show(msj);
                this.Close();
            }
            else
            {
                grupo = new Grupo();

                grupo.GRU_Nombre = txtNombre.Text;
                grupo.Estado_Grupo = cbEstados.SelectedItem as Estado_Grupo;
                grupo.GRU_Descripcion = txtDescripcion.Text;

                foreach (var ac in acciones)
                {
                    grupo.AgregarAccion(ac);
                }

                var msj = ControladoraGrupos.Instancia.AgregarGrupo(grupo);
                MessageBox.Show(msj);
                this.Close();
            }
        }
        

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
