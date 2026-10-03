using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora
{
    public class ControladoraLibros
    {
        private static ControladoraLibros instancia;

        public static ControladoraLibros Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new ControladoraLibros();
                }
                return instancia;
            }
        }
        public Libro ObtenerLibroPorId(int libroID)
        {
            return Libreria.Contexto.Libros
                .Include(p => p.LIB_Genero)
                .FirstOrDefault(p => p.LIB_ID == libroID);
        }
        public void AgregarLibro(Libro libro, List<ProveedorLibroDTO> proveedoresLibros)
        {
            // Costo inicial: el menor precio pactado con los proveedores elegidos.
            if (libro.LIB_PrecioCosto == 0 && proveedoresLibros.Count > 0)
                libro.LIB_PrecioCosto = proveedoresLibros.Min(p => p.Precio);

            // El stock inicial también queda en el historial, como cualquier otro movimiento.
            if (libro.LIB_Stock > 0)
            {
                Libreria.Contexto.MovimientosStock.Add(new MovimientoStock
                {
                    MOV_Libro = libro,
                    MOV_Fecha = DateTime.Now,
                    MOV_Tipo = TipoMovimientoStock.AltaInicial,
                    MOV_Cantidad = libro.LIB_Stock,
                    MOV_StockAnterior = 0,
                    MOV_StockResultante = libro.LIB_Stock,
                    MOV_Motivo = "Alta del libro",
                    MOV_Usuario = Servicios.PermisoService.Instancia.UsuarioActual?.USU_Nombre,
                });
            }

            Libreria.Contexto.Libros.Add(libro);
            Libreria.Contexto.SaveChanges();

            foreach (var item in proveedoresLibros)
            {
                ProveedorLibro pl = new ProveedorLibro();
                Proveedor proveedor1 = ControladoraProveedores.Instancia.ObtenerProveedorPorId(item.PLDTO_PROVID);
                Libro libro1 = ObtenerLibroPorId(libro.LIB_ID);
                pl.PL_Proveedor = proveedor1;
                pl.PL_Libro = libro1;
                pl.PL_PrecioCompra = item.Precio;
                Libreria.Contexto.ProveedoresLibros.Add(pl);
            }
            Libreria.Contexto.SaveChanges();
        }

        public List<LibroDTO> BuscarLibros(string filtro)
        {
            return ObtenerLibrosGrid().Where(p => p.Titulo.ToString().ToLower().Contains(filtro)).ToList();
        }
        

        public Libro BuscarLibroIndividual(LibroDTO libroSeleccionado)
        {
            return Libreria.Contexto.Libros
                          .Include(p => p.LIB_Genero)
                          .FirstOrDefault(p => p.LIB_ID == libroSeleccionado.LIBDTO_ID);
        }
        public void ModificarLibro(Libro libroModificado, List<ProveedorLibroDTO> proveedoresLibros)
        {
            if (libroModificado != null)
            {
                Libro libroExistente = Libreria.Contexto.Libros
                                                    .Include(c => c.LIB_Genero)
                                                    .FirstOrDefault(c => c.LIB_ID == libroModificado.LIB_ID);
                if (libroExistente != null && libroExistente.LIB_Genero != null)
                {
                    libroExistente.LIB_Titulo = libroModificado.LIB_Titulo;
                    libroExistente.LIB_Autor = libroModificado.LIB_Autor;
                    libroExistente.LIB_Descripcion = libroModificado.LIB_Descripcion;
                    libroExistente.LIB_Editorial = libroModificado.LIB_Editorial;
                    libroExistente.LIB_AñoPublicacion = libroModificado.LIB_AñoPublicacion;
                    libroExistente.LIB_Genero = libroModificado.LIB_Genero;
                    // El stock NO se modifica desde el ABM: se cambia sólo con ventas, recepciones o ajustes
                    // auditados en Inventario, para que todo cambio quede en el historial.
                    libroExistente.LIB_PrecioVenta = libroModificado.LIB_PrecioVenta;
                    libroExistente.LIB_ISBN = libroModificado.LIB_ISBN;
                    var proveedoresViejos = Libreria.Contexto.ProveedoresLibros.Where(pl => pl.LIB_ID == libroExistente.LIB_ID).ToList();
                    Libreria.Contexto.ProveedoresLibros.RemoveRange(proveedoresViejos);
                    foreach (var item in proveedoresLibros)
                    {
                        ProveedorLibro pl = new ProveedorLibro();
                        Proveedor proveedor1 = ControladoraProveedores.Instancia.ObtenerProveedorPorId(item.PLDTO_PROVID);
                        Libro libro1 = ObtenerLibroPorId(libroExistente.LIB_ID);
                        pl.PL_Proveedor = proveedor1;
                        pl.PL_Libro = libro1;
                        pl.PL_PrecioCompra = item.Precio;
                        Libreria.Contexto.ProveedoresLibros.Add(pl);
                    }
                    Libreria.Contexto.SaveChanges();
                }
            }
        }
        public List<LibroDTO> ObtenerLibrosGrid()
        {
            return Libreria.Contexto.Libros.Include(p => p.LIB_Genero)
            .Select(p => new LibroDTO
            {
                LIBDTO_ID = p.LIB_ID,
                Titulo = p.LIB_Titulo,
                Autor = p.LIB_Autor,
                Descripcion = p.LIB_Descripcion,
                Editorial = p.LIB_Editorial,
                Stock = p.LIB_Stock,
                AñoPublicacion = p.LIB_AñoPublicacion,
                Genero = p.LIB_Genero.GEN_Nombre,
                Precio = p.LIB_PrecioVenta,
                ISBN = p.LIB_ISBN,
            }).ToList();
        }
        /// <summary>Versión async con contexto propio y sin tracking, para no bloquear la UI.</summary>
        public async Task<List<LibroDTO>> ObtenerLibrosGridAsync(CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await db.Libros.AsNoTracking()
            .Select(p => new LibroDTO
            {
                LIBDTO_ID = p.LIB_ID,
                Titulo = p.LIB_Titulo,
                Autor = p.LIB_Autor,
                Descripcion = p.LIB_Descripcion,
                Editorial = p.LIB_Editorial,
                Stock = p.LIB_Stock,
                AñoPublicacion = p.LIB_AñoPublicacion,
                Genero = p.LIB_Genero.GEN_Nombre,
                Precio = p.LIB_PrecioVenta,
                ISBN = p.LIB_ISBN,
            }).ToListAsync(ct);
        }
        public string EliminarLibro(Libro libroSeleccionado)
        {
            try
            {
                // Las FK DetalleVenta -> Libro y DetalleOrdenReposicion -> Libro son Restrict: se valida antes para dar
                // un mensaje claro y no dejar entidades marcadas como Deleted en el contexto compartido.
                if (Libreria.Contexto.DetallesVenta.Any(d => d.LIB_ID == libroSeleccionado.LIB_ID))
                    return "No se puede eliminar el libro porque figura en ventas registradas.";
                if (Libreria.Contexto.DetallesOrdenReposicion.Any(d => d.LIB_ID == libroSeleccionado.LIB_ID))
                    return "No se puede eliminar el libro porque figura en órdenes de reposición.";

                // Sin ventas ni órdenes, su historial sólo tiene altas/ajustes manuales: se elimina junto con el libro.
                var movimientos = Libreria.Contexto.MovimientosStock.Where(m => m.LIB_ID == libroSeleccionado.LIB_ID).ToList();
                Libreria.Contexto.MovimientosStock.RemoveRange(movimientos);

                var proveedoresLibro =Libreria.Contexto.ProveedoresLibros.Where(pl => pl.LIB_ID == libroSeleccionado.LIB_ID).ToList();
                Libreria.Contexto.ProveedoresLibros.RemoveRange(proveedoresLibro);
                Libreria.Contexto.Libros.Remove(libroSeleccionado);
                Libreria.Contexto.SaveChanges();
                return "Libro eliminado correctamente.";
            }
            catch (Exception e)
            {
                return "Ha ocurrido una excepcion: " + e.Message;
            }
        }

        public List<Libro> ObtenerLibros()
        {
            return Libreria.Contexto.Libros.Include(l => l.LIB_Genero).ToList();
        }
    }
}
