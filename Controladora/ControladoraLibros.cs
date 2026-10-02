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
        public List<LibroInventarioDTO> ObtenerBajoStock()
        {
            var resultado = Libreria.Contexto.Libros.Select(l => new LibroInventarioDTO
            {
                LINVDTO_ID = l.LIB_ID,
                Titulo = l.LIB_Titulo,
                Stock = l.LIB_Stock,
                Estado = l.LIB_Stock == 0
                        ? "Sin Stock"
                        : l.LIB_Stock <= 5
                        ? "Stock Bajo"
                        : "Disponible"
            }).OrderBy(x => x.Stock)
            .Where(x => x.Stock <= 5)
            .ToList();

            return resultado;

        }
        public List<LibroInventarioDTO> ObtenerInventario()
        {
            var resultado = Libreria.Contexto.Libros.Select(l => new LibroInventarioDTO
                {
                    LINVDTO_ID = l.LIB_ID,
                    Titulo = l.LIB_Titulo,
                    Stock = l.LIB_Stock,
                    Estado = l.LIB_Stock == 0
                        ? "Sin Stock"
                        : l.LIB_Stock <= 5
                        ? "Stock Bajo"
                        : "Disponible"
                    }).OrderBy(x => x.Stock).ToList();
            
            return resultado;
            
        }
        public Libro ObtenerLibroPorId(int libroID)
        {
            return Libreria.Contexto.Libros
                .Include(p => p.LIB_Genero)
                .FirstOrDefault(p => p.LIB_ID == libroID);
        }
        public void ModificarStock(List<LibroVentaDTO> librosVenta)
        {
            foreach (var p in librosVenta)
            {
                var productoDb = Libreria.Contexto.Libros.FirstOrDefault(x => x.LIB_ID == p.LVDTO_ID);
                productoDb.LIB_Stock -= p.Cantidad;
            }
            Libreria.Contexto.SaveChanges();
        }
        public string ModificarStock(LibroDTO producto, int unidades)
        {
            try
            {
                if (!ObtenerLibros().Any(j => j.LIB_ID == producto.LIBDTO_ID))
                {
                    return "No se encontró el producto a modificar.";
                }
                Libro libro = BuscarLibroIndividual(producto);
                libro.LIB_Stock = unidades;
                Libreria.Contexto.SaveChanges();
                return "Stock modificado correctamente.";
            }
            catch (Exception e)
            {
                return "Ha ocurrido una excepcion: " + e.Message;
            }
        }
        public void AgregarLibro(Libro libro, List<ProveedorLibroDTO> proveedoresLibros)
        {
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
        public void modificarLibroOrden(Libro libroModificado)
        {
            if (libroModificado != null)
            {
                Libreria.Contexto.Libros.Update(libroModificado);
                Libreria.Contexto.SaveChanges();
            }
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
                    libroExistente.LIB_Stock = libroModificado.LIB_Stock;
                    libroExistente.LIB_PrecioVenta = libroModificado.LIB_PrecioVenta;
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
            }).ToListAsync(ct);
        }
        public string EliminarLibro(Libro libroSeleccionado)
        {
            try
            {
                // Las FK DetalleVenta -> Libro y OrdenReposicion -> Libro son Restrict: se valida antes para dar
                // un mensaje claro y no dejar entidades marcadas como Deleted en el contexto compartido.
                if (Libreria.Contexto.DetallesVenta.Any(d => d.LIB_ID == libroSeleccionado.LIB_ID))
                    return "No se puede eliminar el libro porque figura en ventas registradas.";
                if (Libreria.Contexto.OrdenesReposicion.Any(o => o.OR_LIB_ID == libroSeleccionado.LIB_ID))
                    return "No se puede eliminar el libro porque tiene órdenes de reposición.";

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
