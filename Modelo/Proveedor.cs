using Modelo.Seguridad;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Modelo
{
    /// <summary>Condiciones fiscales (AFIP) admitidas para proveedores y clientes. Se guardan como texto.</summary>
    public static class CondicionFiscal
    {
        public const string ResponsableInscripto = "Responsable Inscripto";
        public const string Monotributo = "Monotributo";
        public const string Exento = "Exento";
        public const string ConsumidorFinal = "Consumidor Final";

        public static readonly string[] Todas = { ResponsableInscripto, Monotributo, Exento, ConsumidorFinal };
    }

    public class Proveedor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PROV_ID { get; set; }

        /// <summary>Persona de contacto (nombre, teléfono, email).</summary>
        public int PER_ID { get; set; }
        public Persona PER_Proveedor { get; set; }

        /// <summary>Razón social.</summary>
        public string PROV_Empresa { get; set; }

        /// <summary>CUIT sin guiones (11 dígitos). Obligatorio al guardar; único entre los que lo tienen cargado.</summary>
        [StringLength(11)]
        public string? PROV_CUIT { get; set; }

        [StringLength(200)]
        public string? PROV_Direccion { get; set; }

        [StringLength(30)]
        public string? PROV_CondicionFiscal { get; set; }

        [StringLength(500)]
        public string? PROV_Observaciones { get; set; }

        /// <summary>Baja lógica: un proveedor inactivo no se usa en nuevas órdenes, pero conserva su historial.</summary>
        public bool PROV_Activo { get; set; } = true;

        /// <summary>Token de concurrencia optimista de la ficha.</summary>
        [ConcurrencyCheck]
        public int PROV_Version { get; set; }

        public ICollection<ProveedorLibro> PROV_Libros { get; set; } = new List<ProveedorLibro>();

        /// <summary>CUIT con formato 20-12345678-9 (para mostrar).</summary>
        public static string? FormatearCuit(string? cuit) =>
            cuit is { Length: 11 } ? $"{cuit[..2]}-{cuit[2..10]}-{cuit[10]}" : cuit;
    }
}
