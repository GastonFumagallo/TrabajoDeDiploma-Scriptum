using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Modelo.Seguridad
{
    public partial class Grupo
    {
        /// <summary>
        /// Grupo de sistema con acceso total (ver PermisoService). No se puede renombrar, deshabilitar ni eliminar,
        /// y el índice único sobre GRU_Nombre impide que otro grupo tome este nombre.
        /// </summary>
        public const string NombreAdministrador = "Administrador";

        public Grupo()
        {
            Acciones = new HashSet<Accion>();
            Usuarios = new HashSet<Usuario>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GRU_ID { get; set; }

        [StringLength(60)]
        public string GRU_Nombre { get; set; }

        [StringLength(60)]
        public string GRU_Descripcion { get; set; }

        [ForeignKey("Estado_Grupo")]
        public int EST_GRU_ID { get; set; }

        public virtual Estado_Grupo Estado_Grupo { get; set; }

        public virtual ICollection<Accion> Acciones { get; set; }

        public virtual ICollection<Usuario> Usuarios { get; set; }

        /// <summary>Sus permisos cuentan para los usuarios. Requiere Estado_Grupo cargado.</summary>
        [NotMapped]
        public bool EstaActivo => Estado_Grupo?.EST_GRU_Nombre == Modelo.Seguridad.Estado_Grupo.Activo;

        public override string ToString()
        {
            return GRU_Nombre;
        }


        public bool AgregarAccion(Accion accion)
        {
            var accionExistente = Acciones.FirstOrDefault(x => x.ACC_ID == accion.ACC_ID);
            if (accionExistente == null)
            {
                Acciones.Add(accion);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool QuitarAccion(Accion accion)
        {
            var accionExistente = Acciones.FirstOrDefault(x => x.ACC_ID == accion.ACC_ID);
            if (accionExistente != null)
            {
                Acciones.Remove(accionExistente);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
