using Microsoft.EntityFrameworkCore;    
using Microsoft.EntityFrameworkCore.SqlServer;
using Modelo.Seguridad;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Emit;
using System.Text;

namespace Modelo.Contexto
{
    public class Libreria : DbContext
    {
        // Sin instancia global: cada operación crea y descarta su propio contexto
        // (`using var db = new Libreria();`). DbContext no es thread-safe y su ChangeTracker crece sin límite.
        public  Libreria() { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = ConfigurationHelper.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);

        }

        public virtual DbSet<Accion> Acciones { get; set; }
        public virtual DbSet<Estado_Grupo> Estados_Grupos { get; set; }
        public virtual DbSet<Estado_Usuario> Estados_Usuarios { get; set; }
        public virtual DbSet<Formulario> Formularios { get; set; }
        public virtual DbSet<Grupo> Grupos { get; set; }
        public virtual DbSet<Modulo> Modulos { get; set; }
        public virtual DbSet<Persona> Personas { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<AuditoriaSesion> AuditoriaSesiones { get; set; }
        public virtual DbSet<Cliente> Clientes { get; set; }
        public virtual DbSet<Libro> Libros { get; set; }
        public virtual DbSet<Genero> Generos { get; set; }
        public virtual DbSet<Proveedor> Proveedores { get; set; }
        public virtual DbSet<Venta> Ventas { get; set; }
        public virtual DbSet<DetalleVenta> DetallesVenta { get; set; }
        public virtual DbSet<MetodoPago> MetodosPago { get; set; }
        public virtual DbSet<ProveedorLibro> ProveedoresLibros {  get; set; }
        public virtual DbSet<OrdenReposicion> OrdenesReposicion { get; set; }
        public virtual DbSet<PagoVenta> PagosVenta { get; set; }
        public virtual DbSet<DetalleOrdenReposicion> DetallesOrdenReposicion { get; set; }
        public virtual DbSet<MovimientoStock> MovimientosStock { get; set; }
        public virtual DbSet<AuditoriaSeguridad> AuditoriaSeguridad { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // El login busca por nombre de usuario: tiene que ser único.
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.USU_Nombre)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.USU_Mail)
                .IsUnique();

            // Usuarios: PER_ID es la FK real hacia Persona (antes EF usaba la columna sombra USU_PersonaPER_ID).
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.USU_Persona)
                .WithMany(p => p.Usuarios)
                .HasForeignKey(u => u.PER_ID);

            // Borrar un usuario no puede borrar su historial de sesiones (antes era Cascade). AS_USU_ID es la FK
            // real (antes se guardaba aparte de la columna sombra AS_UsuarioUSU_ID, con el mismo valor).
            modelBuilder.Entity<AuditoriaSesion>()
                .HasOne(a => a.AS_Usuario)
                .WithMany()
                .HasForeignKey(a => a.AS_USU_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AuditoriaSeguridad>()
                .HasIndex(a => a.AUD_Fecha);

            // El nombre del grupo es único: además de evitar duplicados, impide que otro grupo se llame
            // "Administrador" y herede el acceso total que PermisoService le da a ese nombre.
            modelBuilder.Entity<Grupo>()
                .HasIndex(g => g.GRU_Nombre)
                .IsUnique();

            // Clientes y proveedores: PER_ID es la FK real hacia Persona (antes EF usaba columnas sombra).
            modelBuilder.Entity<Cliente>()
                .HasOne(c => c.CLI_Persona)
                .WithMany(p => p.Clientes)
                .HasForeignKey(c => c.PER_ID);

            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.CLI_Documento)
                .IsUnique()
                .HasFilter("[CLI_Documento] IS NOT NULL");

            modelBuilder.Entity<Proveedor>()
                .HasOne(p => p.PER_Proveedor)
                .WithMany(p => p.Profesores)
                .HasForeignKey(p => p.PER_ID);

            modelBuilder.Entity<Proveedor>()
                .HasIndex(p => p.PROV_CUIT)
                .IsUnique()
                .HasFilter("[PROV_CUIT] IS NOT NULL");

            // Búsqueda por código de barras en el punto de venta.
            modelBuilder.Entity<Libro>()
                .HasIndex(l => l.LIB_ISBN)
                .IsUnique()
                .HasFilter("[LIB_ISBN] IS NOT NULL");   // único sólo entre los libros que tienen ISBN

            // Los pagos pertenecen a la venta (cascada); el medio de pago es un maestro (Restrict).
            modelBuilder.Entity<PagoVenta>()
                .HasOne(p => p.PAG_Venta)
                .WithMany(v => v.VEN_Pagos)
                .HasForeignKey(p => p.VEN_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PagoVenta>()
                .HasOne(p => p.PAG_MetodoPago)
                .WithMany()
                .HasForeignKey(p => p.MP_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProveedorLibro>()
            .HasKey(pl => new { pl.PROV_ID, pl.LIB_ID });

            modelBuilder.Entity<ProveedorLibro>()
                .HasOne(pl => pl.PL_Proveedor)
                .WithMany(p => p.PROV_Libros)
                .HasForeignKey(pl => pl.PROV_ID);

            modelBuilder.Entity<ProveedorLibro>()
                .HasOne(pl => pl.PL_Libro)
                .WithMany(l => l.LIB_Proveedores)
                .HasForeignKey(pl => pl.LIB_ID);

            // Las propiedades CLI_ID, MP_ID, VEN_ID, LIB_ID y GEN_ID no siguen la convención de nombres de EF
            // (<Navegación><PK>), así que hay que declararlas como FK explícitamente; si no, EF crea columnas sombra.
            // Borrado: Restrict en todo lo que apunta a datos maestros, para que eliminar un cliente, libro,
            // método de pago o género nunca arrastre ventas del historial. Sólo los detalles cascadean con su venta.
            modelBuilder.Entity<Venta>()
                .HasOne(v => v.VEN_Cliente)
                .WithMany()
                .HasForeignKey(v => v.CLI_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Venta>()
                .HasOne(v => v.VEN_MetodoPago)
                .WithMany()
                .HasForeignKey(v => v.MP_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.DV_Venta)
                .WithMany(v => v.VEN_Detalles)
                .HasForeignKey(d => d.VEN_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.DV_Libro)
                .WithMany()
                .HasForeignKey(d => d.LIB_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Libro>()
                .HasOne(l => l.LIB_Genero)
                .WithMany(g => g.GEN_Libros)
                .HasForeignKey(l => l.GEN_ID)
                .OnDelete(DeleteBehavior.Restrict);

            // Órdenes de reposición: el proveedor y los libros son maestros (Restrict);
            // los detalles pertenecen a la orden (cascada).
            modelBuilder.Entity<OrdenReposicion>()
                .HasOne(o => o.OR_Proveedor)
                .WithMany()
                .HasForeignKey(o => o.OR_PROV_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrdenReposicion>()
                .HasIndex(o => o.OR_Estado);

            modelBuilder.Entity<DetalleOrdenReposicion>()
                .HasOne(d => d.DOR_Orden)
                .WithMany(o => o.OR_Detalles)
                .HasForeignKey(d => d.OR_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetalleOrdenReposicion>()
                .HasOne(d => d.DOR_Libro)
                .WithMany()
                .HasForeignKey(d => d.LIB_ID)
                .OnDelete(DeleteBehavior.Restrict);

            // Historial de stock: nunca se borra junto con el libro.
            modelBuilder.Entity<MovimientoStock>()
                .HasOne(m => m.MOV_Libro)
                .WithMany()
                .HasForeignKey(m => m.LIB_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MovimientoStock>()
                .HasIndex(m => new { m.LIB_ID, m.MOV_Fecha });
        }
    }

}
