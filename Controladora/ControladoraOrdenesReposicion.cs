using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora
{
    public class ControladoraOrdenesReposicion
    {
        private static ControladoraOrdenesReposicion instancia;

        public static ControladoraOrdenesReposicion Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new ControladoraOrdenesReposicion();
                }
                return instancia;
            }
        }
        public void AgregarOrdenReposicion(OrdenReposicion orden1)
        {
            Libreria.Contexto.OrdenesReposicion.Add(orden1);
            Libreria.Contexto.SaveChanges();
        }

        public OrdenReposicion buscarOrdenIndividual(OrdenReposicionDTO ordenSeleccionada)
        {
            return Libreria.Contexto.OrdenesReposicion
                          .Include(p => p.OR_Libro)
                          .Include(p => p.OR_Proveedor)
                          .FirstOrDefault(p => p.OR_ID == ordenSeleccionada.ORDTO_ID);
        }

        public bool ModificarOrdenReposicion(OrdenReposicion ordenModificada)
        {
            if (ordenModificada == null)
            {
                return false;
            }
            Libreria.Contexto.OrdenesReposicion.Update(ordenModificada);
            Libreria.Contexto.SaveChanges();
            return true;
        }
        public List<OrdenReposicionDTO> ObtenerOrdenesGrid()
        {
            return Libreria.Contexto.OrdenesReposicion
                .Include(p => p.OR_Proveedor)
                .Include(p => p.OR_Libro)
                .Where(p => p.OR_Estado == "PENDIENTE" || p.OR_Estado == "ENVIADA")
                .Select(p => new OrdenReposicionDTO
                {
                    ORDTO_ID = p.OR_ID,
                    Libro = p.OR_Libro.LIB_Titulo,
                    Proveedor = p.OR_Proveedor.PER_Proveedor.PER_Nombre,
                    Cantidad = p.OR_Cantidad,
                    Fecha = p.OR_Fecha,
                    Estado = p.OR_Estado
                })
                .ToList();
        }
        public List<OrdenReposicionDTO> ObtenerOrdenesHistoricas()
        {
            return Libreria.Contexto.OrdenesReposicion
                .Include(p => p.OR_Proveedor)
                .Include(p => p.OR_Libro)
                .Where(p => p.OR_Estado == "RECIBIDA" || p.OR_Estado == "CANCELADA")
                .Select(p => new OrdenReposicionDTO
                {
                    ORDTO_ID = p.OR_ID,
                    Libro = p.OR_Libro.LIB_Titulo,
                    Proveedor = p.OR_Proveedor.PER_Proveedor.PER_Nombre,
                    Cantidad = p.OR_Cantidad,
                    Fecha = p.OR_Fecha,
                    Estado = p.OR_Estado
                })
                .ToList();
        }
        public bool ExisteOrdenActiva(int idLibro)
        {
            return Libreria.Contexto.OrdenesReposicion.Any(o => o.OR_LIB_ID == idLibro && (o.OR_Estado == "PENDIENTE" || o.OR_Estado == "ENVIADA"));
        }





    }
}
