using System;
using System.Collections.Generic;
using System.Text;

namespace Modelo
{
    /// <summary>Filtro de estado común a todos los ABM con baja lógica.</summary>
    public enum FiltroEstadoActivo { Activos, Inactivos, Todos }

    /// <summary>Filtro base de los listados de ABM. Cada entidad agrega lo suyo heredando.</summary>
    public class FiltroAbm
    {
        /// <summary>Texto libre: se busca en los campos principales de la entidad.</summary>
        public string? Texto { get; set; }
        public FiltroEstadoActivo Estado { get; set; } = FiltroEstadoActivo.Activos;
    }

    #region Libros

    public sealed class FiltroLibros : FiltroAbm
    {
        public int? GeneroId { get; set; }
    }

    /// <summary>Fila de la grilla de libros (proyección liviana, sin entidades relacionadas).</summary>
    public sealed class LibroListadoDTO
    {
        public int Id { get; set; }
        public string? ISBN { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string Editorial { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public decimal PrecioCosto { get; set; }
        public decimal PrecioVenta { get; set; }
        public int Stock { get; set; }
        public int StockMinimo { get; set; }
        public int Proveedores { get; set; }
        public bool Activo { get; set; }

        /// <summary>Margen sobre el precio de venta, en porcentaje. null si no hay costo cargado.</summary>
        public decimal? Margen => PrecioCosto > 0 && PrecioVenta > 0 ? Math.Round((PrecioVenta - PrecioCosto) / PrecioVenta * 100, 1) : null;
        public string Estado => Activo ? "Activo" : "Inactivo";
    }

    /// <summary>Ficha completa de un libro para el modal de alta/edición. Id null = alta.</summary>
    public sealed class LibroEdicionDTO
    {
        public int? Id { get; set; }
        public string? ISBN { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string Editorial { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int AnioPublicacion { get; set; }
        public int GeneroId { get; set; }
        public decimal PrecioCosto { get; set; }
        public decimal PrecioVenta { get; set; }

        /// <summary>Stock con el que se da de alta. En edición es informativo: el stock se cambia desde Inventario.</summary>
        public int Stock { get; set; }
        public int StockMinimo { get; set; } = 2;
        public int PuntoReposicion { get; set; } = 5;
        public int StockOptimo { get; set; } = 10;
        public bool Activo { get; set; } = true;

        /// <summary>Token de concurrencia leído al abrir la ficha; si cambió al guardar, otro usuario la editó.</summary>
        public int Version { get; set; }

        public List<ProveedorPrecioDTO> Proveedores { get; set; } = new();
    }

    /// <summary>Proveedor asociado a un libro con su precio de compra pactado.</summary>
    public sealed class ProveedorPrecioDTO
    {
        public int ProveedorId { get; set; }
        public string Proveedor { get; set; } = string.Empty;
        public decimal PrecioCompra { get; set; }
    }

    /// <summary>Datos que el punto de venta necesita de cada libro activo.</summary>
    public sealed class LibroCatalogoDTO
    {
        public int Id { get; set; }
        public string? ISBN { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string Editorial { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }

    #endregion

    /// <summary>Validaciones de formato de identificadores, compartidas por la UI y los servicios.</summary>
    public static class Identificadores
    {
        /// <summary>Valida un ISBN-10 o ISBN-13 (ya normalizado con <see cref="Libro.NormalizarISBN"/>) por su dígito verificador.</summary>
        public static bool EsISBNValido(string? isbn)
        {
            if (string.IsNullOrEmpty(isbn)) return false;

            if (isbn.Length == 13 && isbn.All(char.IsDigit))
            {
                int suma = 0;
                for (int i = 0; i < 12; i++)
                    suma += (isbn[i] - '0') * (i % 2 == 0 ? 1 : 3);
                return (10 - suma % 10) % 10 == isbn[12] - '0';
            }

            if (isbn.Length == 10 && isbn[..9].All(char.IsDigit) && (char.IsDigit(isbn[9]) || isbn[9] == 'X'))
            {
                int suma = 0;
                for (int i = 0; i < 9; i++)
                    suma += (isbn[i] - '0') * (10 - i);
                suma += isbn[9] == 'X' ? 10 : isbn[9] - '0';
                return suma % 11 == 0;
            }

            return false;
        }

        /// <summary>Formato de email razonable (no exhaustivo): algo@dominio.ext.</summary>
        public static bool EsEmailValido(string? email) =>
            !string.IsNullOrWhiteSpace(email) &&
            System.Text.RegularExpressions.Regex.IsMatch(email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$");

        /// <summary>Valida un CUIT/CUIL argentino (11 dígitos) por su dígito verificador.</summary>
        public static bool EsCuitValido(string? cuit)
        {
            var digitos = new string((cuit ?? "").Where(char.IsDigit).ToArray());
            if (digitos.Length != 11) return false;
            int[] pesos = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            int suma = pesos.Select((p, i) => p * (digitos[i] - '0')).Sum();
            int verificador = 11 - suma % 11;
            verificador = verificador == 11 ? 0 : verificador == 10 ? 9 : verificador;
            return verificador == digitos[10] - '0';
        }
    }
}
