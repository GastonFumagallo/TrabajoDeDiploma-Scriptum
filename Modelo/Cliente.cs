using Modelo.Seguridad;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Modelo
{
    public class Cliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CLI_ID { get; set; }
        public int PER_ID { get; set; }
        public Persona CLI_Persona { get; set; }
    }

    public class ClienteDTO
    {
        public int CLIDTO_ID { get; set; }
        public int DNI { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
    }
}
