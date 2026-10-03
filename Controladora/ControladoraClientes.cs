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
                // La FK Venta -> Cliente es Restrict: se valida antes para dar un mensaje claro y no dejar
                // la entidad marcada como Deleted en el contexto compartido si el borrado fallara.
                if (Libreria.Contexto.Ventas.Any(v => v.CLI_ID == clienteSeleccionado.CLI_ID))
                    return "No se puede eliminar el cliente porque tiene ventas registradas.";

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

        public const string NombreConsumidorFinal = "Consumidor Final";
        public const int DniConsumidorFinal = 0;

        /// <summary>
        /// Devuelve el cliente genérico "Consumidor Final" y lo crea si todavía no existe.
        /// Se usa cuando la venta no se asigna a un cliente identificado.
        /// </summary>
        public async Task<ClienteDTO> ObtenerConsumidorFinalAsync(CancellationToken ct = default)
        {
            await using var db = new Libreria();
            var cliente = await ObtenerOCrearConsumidorFinalAsync(db, ct);
            return new ClienteDTO
            {
                CLIDTO_ID = cliente.CLI_Persona.PER_ID,
                Nombre = cliente.CLI_Persona.PER_Nombre,
                DNI = cliente.CLI_Persona.PER_DNI,
                Email = cliente.CLI_Persona.PER_Mail,
                Telefono = cliente.CLI_Persona.PER_Telefono,
            };
        }

        /// <summary>Variante que trabaja sobre un contexto dado, para usarla dentro de la transacción de la venta.</summary>
        internal static async Task<Cliente> ObtenerOCrearConsumidorFinalAsync(Libreria db, CancellationToken ct)
        {
            var existente = await db.Clientes.Include(c => c.CLI_Persona)
                .FirstOrDefaultAsync(c => c.CLI_Persona.PER_DNI == DniConsumidorFinal
                                       && c.CLI_Persona.PER_Nombre == NombreConsumidorFinal, ct);
            if (existente != null)
                return existente;

            var nuevo = new Cliente
            {
                CLI_Persona = new Persona
                {
                    PER_Nombre = NombreConsumidorFinal,
                    PER_DNI = DniConsumidorFinal,
                    PER_Mail = "-",
                    PER_Telefono = "-",
                },
            };
            db.Clientes.Add(nuevo);
            await db.SaveChangesAsync(ct);
            return nuevo;
        }

        /// <summary>Versión async con contexto propio y sin tracking, para no bloquear la UI.</summary>
        public async Task<List<ClienteDTO>> ObtenerClientesGridAsync(CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await db.Clientes.AsNoTracking()
            .Select(p => new ClienteDTO
            {
                CLIDTO_ID = p.CLI_Persona.PER_ID,
                Nombre = p.CLI_Persona.PER_Nombre,
                DNI = p.CLI_Persona.PER_DNI,
                Email = p.CLI_Persona.PER_Mail,
                Telefono = p.CLI_Persona.PER_Telefono,
            }).ToListAsync(ct);
        }

    }
}
