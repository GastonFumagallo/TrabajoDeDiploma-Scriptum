namespace Modelo.Seguridad
{
    public sealed class FiltroUsuarios : FiltroAbm
    {
        public int? GrupoId { get; set; }
        /// <summary>Sólo cuentas bloqueadas por intentos fallidos (ignora el filtro de estado).</summary>
        public bool SoloBloqueados { get; set; }
    }

    /// <summary>Fila de la grilla de usuarios. No tiene, ni puede tener, ningún dato de la clave.</summary>
    public sealed class UsuarioListadoDTO
    {
        public int Id { get; init; }
        public string Usuario { get; init; } = "";
        public string NombreCompleto { get; init; } = "";
        public string Email { get; init; } = "";
        public string Grupos { get; init; } = "";
        public bool Activo { get; init; }
        public DateTime? BloqueadoHasta { get; init; }
        public DateTime? UltimoAcceso { get; init; }
        public bool DebeCambiarClave { get; init; }
        public bool EsAdministrador { get; init; }

        public bool Bloqueado => BloqueadoHasta > DateTime.Now;
        public string Estado => !Activo ? "Inactivo" : Bloqueado ? "Bloqueado" : DebeCambiarClave ? "Activo (clave temporal)" : "Activo";
    }

    /// <summary>
    /// Ficha de alta (Id null) y edición. No tiene campo de clave: en el alta la genera el servicio y los cambios
    /// de clave van por operaciones propias (blanqueo administrativo o cambio por el propio usuario).
    /// </summary>
    public sealed class UsuarioEdicionDTO
    {
        public int? Id { get; set; }
        /// <summary>Sólo se puede elegir en el alta.</summary>
        public string Usuario { get; set; } = "";
        public string NombreCompleto { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Telefono { get; set; }
        public string Dni { get; set; } = "";
        public bool Activo { get; set; } = true;
        public int Version { get; set; }
        public List<int> GrupoIds { get; set; } = new();
        /// <summary>Permisos directos del usuario, además de los que hereda de sus grupos.</summary>
        public List<int> AccionIds { get; set; } = new();
    }

    /// <summary>Grupo para asignar a un usuario, con sus acciones (para mostrar los permisos heredados).</summary>
    public sealed record GrupoAsignableDTO(int Id, string Nombre, bool Activo, List<int> AccionIds)
    {
        public override string ToString() => Activo ? Nombre : $"{Nombre} (deshabilitado)";
    }

    /// <param name="ClaveTemporal">Sólo si no se pudo enviar el mail: se le muestra una única vez al operador.</param>
    public sealed record ResultadoClaveTemporal(int UsuarioId, bool MailEnviado, string? ClaveTemporal);

    public enum EstadoLogin { Ok, Invalido, Bloqueado, Inactivo }

    public sealed record ResultadoLogin(EstadoLogin Estado, int UsuarioId = 0, bool DebeCambiarClave = false, DateTime? BloqueadoHasta = null);
}
