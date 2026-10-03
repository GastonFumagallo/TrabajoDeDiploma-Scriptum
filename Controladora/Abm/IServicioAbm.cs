using Modelo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora.Abm
{
    /// <summary>
    /// Contrato estándar de los ABM maestros (Libros, Proveedores, Clientes).
    /// <list type="bullet">
    /// <item>Las lecturas devuelven DTOs proyectados (nunca entidades de EF).</item>
    /// <item>Los errores de negocio se informan con <see cref="ValidacionException"/>, <see cref="EntidadInactivaException"/>
    /// o <see cref="ConcurrenciaException"/>, siempre con mensajes aptos para el usuario.</item>
    /// <item>No hay borrado físico: <see cref="CambiarEstadoAsync"/> activa o desactiva (baja lógica).</item>
    /// </list>
    /// </summary>
    /// <typeparam name="TListado">Fila de la grilla de gestión.</typeparam>
    /// <typeparam name="TEdicion">Ficha completa para el modal de alta/edición.</typeparam>
    /// <typeparam name="TFiltro">Criterios de búsqueda del listado.</typeparam>
    public interface IServicioAbm<TListado, TEdicion, TFiltro> where TFiltro : FiltroAbm
    {
        Task<List<TListado>> ObtenerTodosAsync(TFiltro filtro, CancellationToken ct = default);

        /// <summary>Ficha para editar, o null si no existe.</summary>
        Task<TEdicion?> ObtenerPorIdAsync(int id, CancellationToken ct = default);

        /// <summary>Alta (Id null) o modificación. Devuelve el Id guardado.</summary>
        Task<int> GuardarAsync(TEdicion dto, string usuario, CancellationToken ct = default);

        /// <summary>Baja lógica (activo = false) o reactivación (activo = true).</summary>
        Task CambiarEstadoAsync(int id, bool activo, CancellationToken ct = default);

        /// <summary>true si el identificador único (ISBN, CUIT, DNI) ya lo usa otro registro (activo o no).</summary>
        Task<bool> ExisteIdentificadorAsync(string identificador, int? excluirId = null, CancellationToken ct = default);
    }
}
