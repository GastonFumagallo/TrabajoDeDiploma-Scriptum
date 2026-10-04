namespace Modelo.Seguridad
{
    /// <summary>Fila del listado de grupos.</summary>
    public sealed record GrupoListadoDTO(int Id, string Nombre, string Descripcion, string Estado, bool Activo,
        int Usuarios, bool EsSistema);

    /// <summary>Ficha de un grupo para editar (Id null = alta).</summary>
    public sealed class GrupoEdicionDTO
    {
        public int? Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public int EstadoId { get; set; }
        public List<int> AccionIds { get; set; } = new();
        /// <summary>Grupo de sistema (Administrador): nombre, estado y permisos fijos.</summary>
        public bool EsSistema { get; set; }
    }

    public sealed record EstadoGrupoDTO(int Id, string Nombre)
    {
        public override string ToString() => Nombre;
    }

    public sealed record GrupoUsuarioDTO(string Usuario, string Nombre, string Estado);

    /// <summary>Catálogo de permisos para el árbol: Módulo → Formulario → Acción.</summary>
    public sealed record ModuloPermisosDTO(string Nombre, List<FormularioPermisosDTO> Formularios);
    public sealed record FormularioPermisosDTO(string Nombre, List<AccionPermisoDTO> Acciones);
    public sealed record AccionPermisoDTO(int Id, string Nombre);
}
