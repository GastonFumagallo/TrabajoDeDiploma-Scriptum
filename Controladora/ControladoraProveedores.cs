using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora
{
    public class ControladoraProveedores
    {
        private static ControladoraProveedores instancia;

        public static ControladoraProveedores Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new ControladoraProveedores();
                }
                return instancia;
            }
        }
        public List<ProveedorDTO> ObtenerProveedoresGrid()
        {
            return Libreria.Contexto.Proveedores.Include(p => p.PER_Proveedor)
            .Select(p => new ProveedorDTO
            {
                PROVDTO_ID = p.PER_Proveedor.PER_ID,
                Nombre = p.PER_Proveedor.PER_Nombre,
                DNI = p.PER_Proveedor.PER_DNI,
                Email = p.PER_Proveedor.PER_Mail,
                Telefono = p.PER_Proveedor.PER_Telefono,
                Empresa = p.PROV_Empresa,
            }).ToList();
        }
        public List<ProveedorDTO> BuscarProveedor(string filtro)
        {
            return ObtenerProveedoresGrid().Where(p => p.DNI.ToString().ToLower().Contains(filtro)).ToList();
        }

        public void ModificarProveedor(Proveedor proveedorModificado)
        {
            if (proveedorModificado != null)
            {
                Proveedor proveedorExistente = Libreria.Contexto.Proveedores
                                                    .Include(c => c.PER_Proveedor)
                                                    .FirstOrDefault(c => c.PER_Proveedor.PER_ID == proveedorModificado.PER_Proveedor.PER_ID);
                if (proveedorExistente != null && proveedorExistente.PER_Proveedor != null)
                {
                    proveedorExistente.PER_Proveedor.PER_Nombre = proveedorModificado.PER_Proveedor.PER_Nombre;
                    proveedorExistente.PER_Proveedor.PER_Telefono = proveedorModificado.PER_Proveedor.PER_Telefono;
                    proveedorExistente.PER_Proveedor.PER_DNI = proveedorModificado.PER_Proveedor.PER_DNI;
                    proveedorExistente.PER_Proveedor.PER_Mail = proveedorModificado.PER_Proveedor.PER_Mail;
                    proveedorExistente.PROV_Empresa = proveedorModificado.PROV_Empresa;
                    Libreria.Contexto.SaveChanges();
                }
            }
        }


        public Proveedor BuscarProveedorIndividual(ProveedorDTO proveedorSeleccionado)
        {
            return Libreria.Contexto.Proveedores
                          .Include(p => p.PER_Proveedor)
                          .FirstOrDefault(p => p.PER_Proveedor.PER_ID == proveedorSeleccionado.PROVDTO_ID);
        }
        public void AgregarProveedor(Proveedor proveedor)
        {
            Libreria.Contexto.Proveedores.Add(proveedor);
            Libreria.Contexto.SaveChanges();
        }

        public List<Proveedor> obtenerProveedores()
        {
            return Libreria.Contexto.Proveedores.Include(p => p.PER_Proveedor).ToList();
        }
        public string EliminarProveedor(Proveedor proveedorSeleccionado)
        {
            try
            {
                Libreria.Contexto.Proveedores.Remove(proveedorSeleccionado);
                Libreria.Contexto.SaveChanges();
                return "Proveedor eliminado correctamente.";
            }
            catch (Exception e)
            {
                return "Ha ocurrido una excepcion: " + e.Message;
            }
        }

    }
}
