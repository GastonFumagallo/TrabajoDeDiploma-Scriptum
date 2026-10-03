using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using Modelo.Seguridad;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora.Abm
{
    /// <summary>
    /// ABM de clientes, con el mismo patrón que <see cref="ProveedorService"/>.
    /// Particularidades: documento DNI o CUIT único, cliente de sistema "Consumidor Final" (no editable ni
    /// desactivable) y límite de cuenta corriente. Los datos personales viven en <see cref="Persona"/>.
    /// </summary>
    public sealed class ClienteService : ServicioAbmBase, IClienteService
    {
        public const string NombreConsumidorFinal = "Consumidor Final";

        private static ClienteService? instancia;
        public static ClienteService Instancia => instancia ??= new ClienteService();
        private ClienteService() { }

        protected override string NombreEntidad => "el cliente";

        #region Lecturas

        public async Task<List<ClienteListadoDTO>> ObtenerTodosAsync(FiltroClientes filtro, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            var query = db.Clientes.AsNoTracking();

            query = filtro.Estado switch
            {
                FiltroEstadoActivo.Activos => query.Where(c => c.CLI_Activo),
                FiltroEstadoActivo.Inactivos => query.Where(c => !c.CLI_Activo),
                _ => query,
            };

            if (!string.IsNullOrWhiteSpace(filtro.Texto))
            {
                string texto = filtro.Texto.Trim();
                string? digitos = Identificadores.SoloDigitos(texto);
                query = query.Where(c => c.CLI_Persona.PER_Nombre.Contains(texto)
                                      || c.CLI_Persona.PER_Mail.Contains(texto)
                                      || (c.CLI_Localidad != null && c.CLI_Localidad.Contains(texto))
                                      || (digitos != null && c.CLI_Documento != null && c.CLI_Documento.Contains(digitos)));
            }

            if (!string.IsNullOrWhiteSpace(filtro.Localidad))
                query = query.Where(c => c.CLI_Localidad == filtro.Localidad);

            return await query
                .OrderByDescending(c => c.CLI_ConsumidorFinal)   // Consumidor Final siempre arriba
                .ThenBy(c => c.CLI_Persona.PER_Nombre)
                .Select(c => new ClienteListadoDTO
                {
                    Id = c.CLI_ID,
                    TipoDocumento = c.CLI_TipoDocumento,
                    Documento = c.CLI_Documento,
                    Nombre = c.CLI_Persona.PER_Nombre,
                    Telefono = c.CLI_Persona.PER_Telefono,
                    Email = c.CLI_Persona.PER_Mail,
                    Localidad = c.CLI_Localidad,
                    LimiteCredito = c.CLI_LimiteCredito,
                    Ventas = db.Ventas.Count(v => v.CLI_ID == c.CLI_ID),
                    EsConsumidorFinal = c.CLI_ConsumidorFinal,
                    Activo = c.CLI_Activo,
                })
                .ToListAsync(ct);
        }

        public async Task<ClienteEdicionDTO?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await db.Clientes.AsNoTracking()
                .Where(c => c.CLI_ID == id)
                .Select(c => new ClienteEdicionDTO
                {
                    Id = c.CLI_ID,
                    TipoDocumento = c.CLI_TipoDocumento,
                    Documento = c.CLI_Documento,
                    Nombre = c.CLI_Persona.PER_Nombre,
                    Telefono = c.CLI_Persona.PER_Telefono,
                    Email = c.CLI_Persona.PER_Mail,
                    Direccion = c.CLI_Direccion,
                    Localidad = c.CLI_Localidad,
                    LimiteCredito = c.CLI_LimiteCredito,
                    EsConsumidorFinal = c.CLI_ConsumidorFinal,
                    Activo = c.CLI_Activo,
                    Version = c.CLI_Version,
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<bool> ExisteIdentificadorAsync(string identificador, int? excluirId = null, CancellationToken ct = default)
        {
            string? documento = Identificadores.SoloDigitos(identificador);
            if (documento == null) return false;

            await using var db = new Libreria();
            return await db.Clientes.AnyAsync(c => c.CLI_Documento == documento && c.CLI_ID != excluirId, ct);
        }

        /// <summary>Localidades cargadas (distintas), para el filtro del listado.</summary>
        public async Task<List<string>> ObtenerLocalidadesAsync(CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await db.Clientes.AsNoTracking()
                .Where(c => c.CLI_Localidad != null && c.CLI_Localidad != "")
                .Select(c => c.CLI_Localidad!)
                .Distinct()
                .OrderBy(l => l)
                .ToListAsync(ct);
        }

        public async Task<List<ClienteDTO>> ObtenerParaVentaAsync(CancellationToken ct = default)
        {
            await using var db = new Libreria();
            await ObtenerOCrearConsumidorFinalAsync(db, ct);
            return await db.Clientes.AsNoTracking()
                .Where(c => c.CLI_Activo)
                .OrderByDescending(c => c.CLI_ConsumidorFinal)
                .ThenBy(c => c.CLI_Persona.PER_Nombre)
                .Select(c => new ClienteDTO
                {
                    CLIDTO_ID = c.CLI_ID,
                    Documento = c.CLI_Documento,
                    Nombre = c.CLI_Persona.PER_Nombre,
                    Telefono = c.CLI_Persona.PER_Telefono,
                    Email = c.CLI_Persona.PER_Mail,
                    EsConsumidorFinal = c.CLI_ConsumidorFinal,
                })
                .ToListAsync(ct);
        }

        public async Task<ClienteDTO> ObtenerConsumidorFinalAsync(CancellationToken ct = default)
        {
            await using var db = new Libreria();
            var cf = await ObtenerOCrearConsumidorFinalAsync(db, ct);
            return new ClienteDTO { CLIDTO_ID = cf.CLI_ID, Nombre = cf.CLI_Persona.PER_Nombre, EsConsumidorFinal = true };
        }

        /// <summary>
        /// Devuelve el cliente de sistema "Consumidor Final" y lo crea si no existe. Recibe el contexto para poder
        /// usarse dentro de la transacción de una venta.
        /// </summary>
        internal static async Task<Cliente> ObtenerOCrearConsumidorFinalAsync(Libreria db, CancellationToken ct)
        {
            var existente = await db.Clientes.Include(c => c.CLI_Persona).FirstOrDefaultAsync(c => c.CLI_ConsumidorFinal, ct);
            if (existente != null)
                return existente;

            var nuevo = new Cliente
            {
                CLI_ConsumidorFinal = true,
                CLI_Activo = true,
                CLI_TipoDocumento = TipoDocumento.DNI,
                CLI_Persona = new Persona { PER_Nombre = NombreConsumidorFinal, PER_DNI = 0, PER_Mail = string.Empty, PER_Telefono = string.Empty },
            };
            db.Clientes.Add(nuevo);
            await db.SaveChangesAsync(ct);
            return nuevo;
        }

        #endregion

        #region Escrituras

        public async Task<int> GuardarAsync(ClienteEdicionDTO dto, string usuario, CancellationToken ct = default)
        {
            Normalizar(dto);

            await using var db = new Libreria();

            Cliente? cliente = null;
            if (dto.Id is int id)
            {
                cliente = await db.Clientes.Include(c => c.CLI_Persona).FirstOrDefaultAsync(c => c.CLI_ID == id, ct)
                          ?? throw new ConcurrenciaException("El cliente ya no existe: otro usuario lo eliminó.");
                if (cliente.CLI_ConsumidorFinal)
                    throw new ValidacionException(nameof(dto.Nombre), "\"Consumidor Final\" es un cliente del sistema y no se puede modificar.");
            }

            await ValidarAsync(db, dto, ct);

            if (cliente != null)
            {
                db.Entry(cliente).Property(c => c.CLI_Version).OriginalValue = dto.Version;
                cliente.CLI_Version = dto.Version + 1;
            }
            else
            {
                cliente = new Cliente { CLI_Activo = true, CLI_Persona = new Persona() };
                db.Clientes.Add(cliente);
            }

            cliente.CLI_TipoDocumento = dto.TipoDocumento;
            cliente.CLI_Documento = dto.Documento;
            cliente.CLI_Direccion = dto.Direccion;
            cliente.CLI_Localidad = dto.Localidad;
            cliente.CLI_LimiteCredito = dto.LimiteCredito;
            cliente.CLI_Persona.PER_Nombre = dto.Nombre;
            cliente.CLI_Persona.PER_Telefono = dto.Telefono ?? string.Empty;
            cliente.CLI_Persona.PER_Mail = dto.Email ?? string.Empty;
            // PER_DNI se mantiene sincronizado para las pantallas que buscan por DNI (en un CUIT, el DNI son los 8 dígitos centrales).
            cliente.CLI_Persona.PER_DNI = DniDesdeDocumento(dto.TipoDocumento, dto.Documento);

            await GuardarCambiosAsync(db, ct, campoUnico: nameof(ClienteEdicionDTO.Documento), etiquetaUnico: "documento");
            return cliente.CLI_ID;
        }

        /// <summary>Baja lógica o reactivación. Consumidor Final no se puede desactivar.</summary>
        public async Task CambiarEstadoAsync(int id, bool activo, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            int filas = await db.Clientes
                .Where(c => c.CLI_ID == id && !c.CLI_ConsumidorFinal)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.CLI_Activo, activo), ct);

            if (filas == 0)
            {
                bool esConsumidorFinal = await db.Clientes.AnyAsync(c => c.CLI_ID == id && c.CLI_ConsumidorFinal, ct);
                throw esConsumidorFinal
                    ? new ValidacionException(nameof(ClienteEdicionDTO.Activo), "\"Consumidor Final\" es un cliente del sistema y no se puede dar de baja.")
                    : new ConcurrenciaException("El cliente ya no existe.");
            }
        }

        public async Task<ClienteEdicionDTO?> ReactivarAsync(int id, CancellationToken ct = default)
        {
            await CambiarEstadoAsync(id, true, ct);
            return await ObtenerPorIdAsync(id, ct);
        }

        #endregion

        #region Validación

        private static void Normalizar(ClienteEdicionDTO dto)
        {
            dto.TipoDocumento = dto.TipoDocumento == TipoDocumento.CUIT ? TipoDocumento.CUIT : TipoDocumento.DNI;
            dto.Documento = Identificadores.SoloDigitos(dto.Documento)?.TrimStart('0');
            if (string.IsNullOrEmpty(dto.Documento)) dto.Documento = null;
            dto.Nombre = dto.Nombre?.Trim() ?? string.Empty;
            dto.Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim();
            dto.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim().ToLowerInvariant();
            dto.Direccion = string.IsNullOrWhiteSpace(dto.Direccion) ? null : dto.Direccion.Trim();
            dto.Localidad = string.IsNullOrWhiteSpace(dto.Localidad) ? null : dto.Localidad.Trim();
            dto.LimiteCredito = Math.Round(dto.LimiteCredito, 2);
        }

        private static async Task ValidarAsync(Libreria db, ClienteEdicionDTO dto, CancellationToken ct)
        {
            var e = new Errores();
            bool esCuit = dto.TipoDocumento == TipoDocumento.CUIT;

            e.Si(dto.Documento == null, nameof(dto.Documento), "El documento es obligatorio.");
            e.Si(dto.Documento != null && esCuit && !Identificadores.EsCuitValido(dto.Documento), nameof(dto.Documento),
                "El CUIT no es válido (11 dígitos con dígito verificador correcto).");
            e.Si(dto.Documento != null && !esCuit && !Identificadores.EsDniValido(dto.Documento), nameof(dto.Documento),
                "El DNI debe tener 7 u 8 dígitos.");
            e.Requerido(dto.Nombre, nameof(dto.Nombre), esCuit ? "La razón social" : "El nombre y apellido", 60);
            e.Si(string.Equals(dto.Nombre, NombreConsumidorFinal, StringComparison.OrdinalIgnoreCase), nameof(dto.Nombre),
                "\"Consumidor Final\" está reservado para el cliente del sistema.");
            e.Si(dto.Telefono != null && !Identificadores.EsTelefonoValido(dto.Telefono), nameof(dto.Telefono),
                "El teléfono no es válido (sólo números, espacios, guiones, paréntesis y +).");
            e.Si(dto.Email != null && !Identificadores.EsEmailValido(dto.Email), nameof(dto.Email), "El email no tiene un formato válido.");
            e.Si(dto.LimiteCredito < 0, nameof(dto.LimiteCredito), "El límite de crédito no puede ser negativo.");
            e.Si(dto.Direccion?.Length > 200, nameof(dto.Direccion), "La dirección no puede superar los 200 caracteres.");
            e.Si(dto.Localidad?.Length > 100, nameof(dto.Localidad), "La localidad no puede superar los 100 caracteres.");

            // Unicidad del documento. En un alta, si choca con un cliente dado de baja, se ofrece reactivarlo.
            if (dto.Documento != null && !e.HayErrores)
            {
                var existente = await db.Clientes.AsNoTracking()
                    .Where(c => c.CLI_Documento == dto.Documento && c.CLI_ID != dto.Id)
                    .Select(c => new { c.CLI_ID, c.CLI_Persona.PER_Nombre, c.CLI_Activo })
                    .FirstOrDefaultAsync(ct);

                if (existente != null)
                {
                    if (!existente.CLI_Activo && dto.Id == null)
                        throw new EntidadInactivaException(existente.CLI_ID,
                            $"Ya existe un cliente dado de baja con ese documento: \"{existente.PER_Nombre}\".");
                    e.Si(true, nameof(dto.Documento), $"El documento ya está asignado a \"{existente.PER_Nombre}\"" +
                                                      (existente.CLI_Activo ? "." : " (inactivo)."));
                }
            }

            e.Lanzar();
        }

        private static int DniDesdeDocumento(string tipo, string? documento)
        {
            if (documento == null) return 0;
            string dni = tipo == TipoDocumento.CUIT && documento.Length == 11 ? documento[2..10] : documento;
            return int.TryParse(dni, out int numero) ? numero : 0;
        }

        #endregion
    }
}
