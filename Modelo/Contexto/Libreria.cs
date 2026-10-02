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
        private static Libreria contexto;
        public static Libreria Contexto
        {
            get
            {
                if (contexto == null) { contexto = new Libreria(); }
                return contexto;
            }
        }
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
            modelBuilder.Entity<Venta>()
                .HasOne(v => v.VEN_Cliente)
                .WithMany()
                .HasForeignKey(v => v.CLI_ID);

            modelBuilder.Entity<Venta>()
                .HasOne(v => v.VEN_MetodoPago)
                .WithMany()
                .HasForeignKey(v => v.MP_ID);

            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.DV_Venta)
                .WithMany(v => v.VEN_Detalles)
                .HasForeignKey(d => d.VEN_ID);

            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.DV_Libro)
                .WithMany()
                .HasForeignKey(d => d.LIB_ID);

            modelBuilder.Entity<Libro>()
                .HasOne(l => l.LIB_Genero)
                .WithMany(g => g.GEN_Libros)
                .HasForeignKey(l => l.GEN_ID);

            modelBuilder.Entity<OrdenReposicion>()
                .HasOne(o => o.OR_Libro)
                .WithMany()
                .HasForeignKey(o => o.OR_LIB_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrdenReposicion>()
                .HasOne(o => o.OR_Proveedor)
                .WithMany()
                .HasForeignKey(o => o.OR_PROV_ID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
