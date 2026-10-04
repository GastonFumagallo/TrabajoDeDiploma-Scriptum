namespace Controladora
{
    /// <summary>
    /// Aplica a una colección muchos-a-muchos rastreada por un DbContext la selección que viene de la UI.
    /// Las entidades de la UI se cargaron en otro contexto (o en ninguno), así que no se adjuntan:
    /// se compara por ID y lo que falta se carga del contexto actual.
    /// </summary>
    internal static class SincronizadorColecciones
    {
        public static void Sincronizar<T>(ICollection<T> actual, IEnumerable<int> idsDeseados,
            Func<T, int> obtenerId, Func<List<int>, IEnumerable<T>> cargarPorIds)
        {
            var deseados = idsDeseados.ToHashSet();

            foreach (var sobrante in actual.Where(x => !deseados.Contains(obtenerId(x))).ToList())
                actual.Remove(sobrante);

            var faltantes = deseados.Except(actual.Select(obtenerId)).ToList();
            if (faltantes.Count == 0)
                return;

            foreach (var entidad in cargarPorIds(faltantes))
                actual.Add(entidad);
        }
    }
}
