using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Vista.Comun
{
    /// <summary>
    /// BindingList que soporta ordenamiento al hacer clic en el encabezado de una columna del DataGridView
    /// (un List&lt;T&gt; enlazado no se puede ordenar). Ordena en memoria sobre filas ya agregadas.
    /// </summary>
    internal sealed class ListaOrdenable<T> : BindingList<T>
    {
        private bool ordenada;
        private ListSortDirection direccion;
        private PropertyDescriptor? propiedad;

        public ListaOrdenable(IEnumerable<T> items) : base(new List<T>(items)) { }

        protected override bool SupportsSortingCore => true;
        protected override bool IsSortedCore => ordenada;
        protected override ListSortDirection SortDirectionCore => direccion;
        protected override PropertyDescriptor? SortPropertyCore => propiedad;

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            var lista = (List<T>)Items;
            var comparador = Comparer<object>.Create((a, b) =>
            {
                if (a == null && b == null) return 0;
                if (a == null) return -1;   // los vacíos (ej. "nunca vendido") quedan al principio en ascendente
                if (b == null) return 1;
                return a is IComparable c ? c.CompareTo(b) : string.Compare(a.ToString(), b.ToString(), StringComparison.CurrentCulture);
            });

            lista.Sort((x, y) =>
            {
                int r = comparador.Compare(prop.GetValue(x!), prop.GetValue(y!));
                return direction == ListSortDirection.Ascending ? r : -r;
            });

            propiedad = prop;
            direccion = direction;
            ordenada = true;
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override void RemoveSortCore()
        {
            ordenada = false;
            propiedad = null;
        }
    }

    /// <summary>Fábrica no genérica: crea una ListaOrdenable del tipo de fila indicado a partir de una lista no tipada.</summary>
    internal static class ListaOrdenable
    {
        public static IBindingList Crear(Type tipoFila, System.Collections.IEnumerable filas)
        {
            var tipo = typeof(ListaOrdenable<>).MakeGenericType(tipoFila);
            var lista = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast))!.MakeGenericMethod(tipoFila).Invoke(null, new object[] { filas })!;
            return (IBindingList)Activator.CreateInstance(tipo, lista)!;
        }
    }
}
