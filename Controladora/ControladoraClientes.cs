using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using Modelo.Seguridad;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora
{
    public class ControladoraClientes
    {
        private static ControladoraClientes instancia;

        public static ControladoraClientes Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new ControladoraClientes();
                }
                return instancia;
            }
        }

        public void AgregarCliente(Cliente cliente)
        {
            Libreria.Contexto.Clientes.Add(cliente);
            Libreria.Contexto.SaveChanges();
        }
        public Cliente BuscarClienteIndividual(ClienteDTO clienteSeleccionado)
        {
            return Libreria.Contexto.Clientes
                          .Include(p => p.CLI_Persona)
                          .FirstOrDefault(p => p.CLI_Persona.PER_ID == clienteSeleccionado.CLIDTO_ID);
        }
        public void ModificarCliente(Cliente clienteModificado)
        {
            if (clienteModificado != null)
            {
                Cliente clienteExistente = Libreria.Contexto.Clientes
                                                    .Include(c => c.CLI_Persona)
                                                    .FirstOrDefault(c => c.CLI_Persona.PER_ID == clienteModificado.CLI_Persona.PER_ID);
                if (clienteExistente != null && clienteExistente.CLI_Persona != null)
                {
                    clienteExistente.CLI_Persona.PER_Nombre = clienteModificado.CLI_Persona.PER_Nombre;
                    clienteExistente.CLI_Persona.PER_Telefono = clienteModificado.CLI_Persona.PER_Telefono;
                    clienteExistente.CLI_Persona.PER_DNI = clienteModificado.CLI_Persona.PER_DNI;
                    clienteExistente.CLI_Persona.PER_Mail = clienteModificado.CLI_Persona.PER_Mail;

                    Libreria.Contexto.SaveChanges();
                }
            }
        }

        public string EliminarCliente(Cliente clienteSeleccionado)
        {
            try
            {
                Libreria.Contexto.Clientes.Remove(clienteSeleccionado);
                Libreria.Contexto.SaveChanges();
                return "Cliente eliminado correctamente.";
            }
            catch (Exception e)
            {
                return "Ha ocurrido una excepcion: " + e.Message;
            }
        }


        public List<ClienteDTO> BuscarCliente(string filtro)
        {
            return ObtenerClientesGrid().Where(p => p.DNI.ToString().ToLower().Contains(filtro)).ToList();
        }


        public List<ClienteDTO> ObtenerClientesGrid()
        {
            return Libreria.Contexto.Clientes.Include(p => p.CLI_Persona)
            .Select(p => new ClienteDTO
            {
                CLIDTO_ID = p.CLI_Persona.PER_ID,
                Nombre = p.CLI_Persona.PER_Nombre,
                DNI = p.CLI_Persona.PER_DNI,
                Email = p.CLI_Persona.PER_Mail,
                Telefono = p.CLI_Persona.PER_Telefono,
            }).ToList();
        }

    }
}
