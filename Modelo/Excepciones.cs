using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modelo
{
    /// <summary>
    /// Error de validación de negocio. Lleva los errores por campo para que la UI los marque con ErrorProvider
    /// (la clave es el nombre de la propiedad del DTO, ej. "ISBN"). El mensaje ya es apto para el usuario.
    /// Se llama "ValidacionException" para no chocar con System.ComponentModel.DataAnnotations.ValidationException.
    /// </summary>
    public sealed class ValidacionException : Exception
    {
        public IReadOnlyDictionary<string, string> Errores { get; }

        public ValidacionException(IReadOnlyDictionary<string, string> errores)
            : base(string.Join(Environment.NewLine, errores.Values))
        {
            Errores = errores;
        }

        public ValidacionException(string campo, string mensaje)
            : this(new Dictionary<string, string> { [campo] = mensaje }) { }
    }

    /// <summary>
    /// Se intentó dar de alta un registro cuyo identificador único (ISBN, CUIT, DNI) ya existe pero está inactivo.
    /// La UI puede ofrecer reactivarlo con <see cref="EntidadId"/> en lugar de crear un duplicado.
    /// </summary>
    public sealed class EntidadInactivaException : Exception
    {
        public int EntidadId { get; }
        public EntidadInactivaException(int entidadId, string mensaje) : base(mensaje) => EntidadId = entidadId;
    }

    /// <summary>Otro usuario modificó o eliminó el registro mientras se editaba (concurrencia optimista).</summary>
    public sealed class ConcurrenciaException : Exception
    {
        public ConcurrenciaException(string mensaje) : base(mensaje) { }
    }

    /// <summary>Error de persistencia traducido a un mensaje claro (sin detalles de SQL).</summary>
    public sealed class PersistenciaException : Exception
    {
        public PersistenciaException(string mensaje, Exception interna) : base(mensaje, interna) { }
    }
}
