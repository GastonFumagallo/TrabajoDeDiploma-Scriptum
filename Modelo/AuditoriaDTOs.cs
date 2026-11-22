namespace Modelo
{
    /// <summary>Filtros comunes de los reportes de auditoría. Las fechas son inclusivas (día completo).</summary>
    public sealed class FiltroAuditoria
    {
        public DateTime Desde { get; init; }
        public DateTime Hasta { get; init; }
        public string? Usuario { get; init; }
        /// <summary>Evento de seguridad, operación sobre libros o texto del libro, según el reporte.</summary>
        public string? Texto { get; init; }
        public string? Operacion { get; init; }
        public int Maximo { get; init; } = 2000;
    }

    public sealed class SesionAuditoriaDTO
    {
        public int Id { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public DateTime Inicio { get; set; }
        public DateTime? Fin { get; set; }
        public string Duracion { get; set; } = string.Empty;
        public string Cierre { get; set; } = string.Empty;
        public bool Activa { get; set; }
    }

    public sealed class EventoSeguridadDTO
    {
        public DateTime Fecha { get; set; }
        public string Evento { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Equipo { get; set; } = string.Empty;
        public string Detalle { get; set; } = string.Empty;
    }

    public sealed class CambioLibroDTO
    {
        public int LibroId { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Operacion { get; set; } = string.Empty;
        public string Campo { get; set; } = string.Empty;
        public string? ValorAnterior { get; set; }
        public string? ValorNuevo { get; set; }
        public string? Referencia { get; set; }
    }

    /// <summary>Estado original y actual de cada campo del libro.</summary>
    public sealed class EstadoCampoDTO
    {
        public string Campo { get; set; } = string.Empty;
        public string? Original { get; set; }
        public string? Actual { get; set; }
        public int Cambios { get; set; }
        public bool Modificado => Cambios > 0;
    }

    public sealed class TrazabilidadLibroDTO
    {
        public int LibroId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        /// <summary>true si el alta quedó registrada; si no, el estado original se reconstruye desde los cambios.</summary>
        public bool AltaRegistrada { get; set; }
        public List<EstadoCampoDTO> Estado { get; set; } = new();
        /// <summary>Cambios de datos y movimientos de stock, del más reciente al más antiguo.</summary>
        public List<CambioLibroDTO> Historial { get; set; } = new();
    }

    public sealed class LibroAuditableDTO
    {
        public int Id { get; set; }
        public string Texto { get; set; } = string.Empty;
        public override string ToString() => Texto;
    }

    #region Resguardo

    public sealed class ResguardoDTO
    {
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Archivo { get; set; } = string.Empty;
        public decimal TamanioMB { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool ArchivoDisponible { get; set; }
    }

    public sealed class VerificacionResguardoDTO
    {
        public bool Valido { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string? BaseDeDatos { get; set; }
        public DateTime? FechaResguardo { get; set; }
    }

    #endregion
}
