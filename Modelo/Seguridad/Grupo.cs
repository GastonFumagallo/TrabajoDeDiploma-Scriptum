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

    public class GrupoDTO
    {
        public int GRUDTO_ID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string EstadoGrupo { get; set; }
    }

}
