using Modelo.Seguridad;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Modelo
{
    public static class TipoDocumento
    {
        public const string DNI = "DNI";
        public const string CUIT = "CUIT";
    }

    public class Cliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CLI_ID { get; set; }

        /// <summary>Datos personales (nombre o razón social, teléfono, email).</summary>
        public int PER_ID { get; set; }
        public Persona CLI_Persona { get; set; }

        [StringLength(4)]
        public string CLI_TipoDocumento { get; set; } = TipoDocumento.DNI;

        /// <summary>DNI (7-8 dígitos) o CUIT (11 dígitos), sólo dígitos. Único. null sólo para Consumidor Final.</summary>
        [StringLength(11)]
        public string? CLI_Documento { get; set; }

        [StringLength(200)]
        public string? CLI_Direccion { get; set; }

        [StringLength(100)]
        public string? CLI_Localidad { get; set; }

        /// <summary>Límite de cuenta corriente. 0 = sin cuenta corriente.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal CLI_LimiteCredito { get; set; }

        /// <summary>Cliente genérico para ventas sin identificar: no se edita ni se da de baja.</summary>
        public bool CLI_ConsumidorFinal { get; set; }

        /// <summary>Baja lógica: un cliente inactivo no se ofrece en el punto de venta, pero conserva sus ventas.</summary>
        public bool CLI_Activo { get; set; } = true;

        [ConcurrencyCheck]
        public int CLI_Version { get; set; }
    }

    /// <summary>Cliente tal como lo usa el punto de venta (selección rápida).</summary>
    public class ClienteDTO
    {
        /// <summary>CLI_ID.</summary>
        public int CLIDTO_ID { get; set; }
        public string? Documento { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public bool EsConsumidorFinal { get; set; }
    }
}
