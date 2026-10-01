using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Modelo
{
    public class OrdenReposicion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OR_ID { get; set; }
        public int OR_LIB_ID { get; set; }
        public Libro OR_Libro { get; set; }
        public int OR_PROV_ID { get; set; }
        public Proveedor OR_Proveedor { get; set; }
        public int OR_Cantidad { get; set; }
        public DateTime OR_Fecha { get; set; }
        public string OR_Estado { get; set; }
    }

    public class OrdenReposicionDTO
    {
        public int ORDTO_ID { get; set; }
        public string Libro { get; set; }
        public string Proveedor { get; set; }
        public int Cantidad { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
    }


}
