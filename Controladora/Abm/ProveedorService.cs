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
    /// ABM de proveedores. Mismo patrón que <see cref="LibroService"/>:
    /// DbContext por operación, lecturas AsNoTracking proyectadas a DTO, validación completa con unicidad de CUIT,
    /// baja lógica, reactivación de registros inactivos y concurrencia optimista (PROV_Version).
    /// Los datos de contacto (nombre, teléfono, email) viven en <see cref="Persona"/> y se guardan en la misma transacción.
    /// </summary>
    public sealed class ProveedorService : ServicioAbmBase, IProveedorService
    {
        private static ProveedorService? instancia;
        public static ProveedorService Instancia => instancia ??= new ProveedorService();
        private ProveedorService() { }

        protected override string NombreEntidad => "el proveedor";

        #region Lecturas

        public async Task<List<ProveedorListadoDTO>> ObtenerTodosAsync(FiltroProveedores filtro, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            var query = db.Proveedores.AsNoTracking();

            query = filtro.Estado switch
            {
                FiltroEstadoActivo.Activos => query.Where(p => p.PROV_Activo),
                FiltroEstadoActivo.Inactivos => query.Where(p => !p.PROV_Activo),
                _ => query,
            };

            if (!string.IsNullOrWhiteSpace(filtro.Texto))
            {
                string texto = filtro.Texto.Trim();
                string? digitos = Identificadores.SoloDigitos(texto);
                query = query.Where(p => p.PROV_Empresa.Contains(texto)
                                      || p.PER_Proveedor.PER_Nombre.Contains(texto)
                                      || p.PER_Proveedor.PER_Mail.Contains(texto)
                                      || (digitos != null && p.PROV_CUIT != null && p.PROV_CUIT.Contains(digitos)));
            }

            if (!string.IsNullOrEmpty(filtro.CondicionFiscal))
                query = query.Where(p => p.PROV_CondicionFiscal == filtro.CondicionFiscal);

            return await query
                .OrderBy(p => p.PROV_Empresa)
                .Select(p => new ProveedorListadoDTO
                {
                    Id = p.PROV_ID,
                    CUIT = p.PROV_CUIT,
                    RazonSocial = p.PROV_Empresa,
                    Contacto = p.PER_Proveedor.PER_Nombre,
                    Telefono = p.PER_Proveedor.PER_Telefono,
                    Email = p.PER_Proveedor.PER_Mail,
                    CondicionFiscal = p.PROV_CondicionFiscal,
                    Libros = p.PROV_Libros.Count(),
                    OrdenesActivas = db.OrdenesReposicion.Count(o => o.OR_PROV_ID == p.PROV_ID
                        && (o.OR_Estado == EstadoOrden.Pendiente || o.OR_Estado == EstadoOrden.Solicitada)),
                    Activo = p.PROV_Activo,
                })
                .ToListAsync(ct);
        }

        public async Task<ProveedorEdicionDTO?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await db.Proveedores.AsNoTracking()
                .Where(p => p.PROV_ID == id)
                .Select(p => new ProveedorEdicionDTO
                {
                    Id = p.PROV_ID,
                    CUIT = p.PROV_CUIT,
                    RazonSocial = p.PROV_Empresa,
                    Contacto = p.PER_Proveedor.PER_Nombre,
                    Telefono = p.PER_Proveedor.PER_Telefono,
                    Email = p.PER_Proveedor.PER_Mail,
                    Direccion = p.PROV_Direccion,
                    CondicionFiscal = p.PROV_CondicionFiscal,
                    Observaciones = p.PROV_Observaciones,
                    Activo = p.PROV_Activo,
                    Version = p.PROV_Version,
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<bool> ExisteIdentificadorAsync(string identificador, int? excluirId = null, CancellationToken ct = default)
        {
            string? cuit = Identificadores.SoloDigitos(identificador);
            if (cuit == null) return false;

            await using var db = new Libreria();
            return await db.Proveedores.AnyAsync(p => p.PROV_CUIT == cuit && p.PROV_ID != excluirId, ct);
        }

        public async Task<List<OpcionDTO>> ObtenerOpcionesAsync(CancellationToken ct = default)
        {
            await using var db = new Libreria();
            return await db.Proveedores.AsNoTracking()
                .Where(p => p.PROV_Activo)
                .Select(p => new OpcionDTO(p.PROV_ID, p.PROV_Empresa != "" ? p.PROV_Empresa : p.PER_Proveedor.PER_Nombre))
                .OrderBy(o => o.Nombre)
                .ToListAsync(ct);
        }

        #endregion

        #region Escrituras

        public async Task<int> GuardarAsync(ProveedorEdicionDTO dto, string usuario, CancellationToken ct = default)
        {
            Normalizar(dto);

            await using var db = new Libreria();
            await ValidarAsync(db, dto, ct);

            Proveedor proveedor;
            if (dto.Id is int id)
            {
                proveedor = await db.Proveedores.Include(p => p.PER_Proveedor).FirstOrDefaultAsync(p => p.PROV_ID == id, ct)
                            ?? throw new ConcurrenciaException("El proveedor ya no existe: otro usuario lo eliminó.");

                // Concurrencia optimista: el UPDATE sólo aplica si nadie guardó la ficha desde que se abrió.
                db.Entry(proveedor).Property(p => p.PROV_Version).OriginalValue = dto.Version;
                proveedor.PROV_Version = dto.Version + 1;
            }
            else
            {
                proveedor = new Proveedor
                {
                    PROV_Activo = true,
                    PER_Proveedor = new Persona { PER_DNI = 0 },   // el proveedor se identifica por CUIT, no por DNI
                };
                db.Proveedores.Add(proveedor);
            }

            proveedor.PROV_CUIT = dto.CUIT;
            proveedor.PROV_Empresa = dto.RazonSocial;
            proveedor.PROV_Direccion = dto.Direccion;
            proveedor.PROV_CondicionFiscal = dto.CondicionFiscal;
            proveedor.PROV_Observaciones = dto.Observaciones;
            proveedor.PER_Proveedor.PER_Nombre = dto.Contacto;
            proveedor.PER_Proveedor.PER_Telefono = dto.Telefono ?? string.Empty;
            proveedor.PER_Proveedor.PER_Mail = dto.Email ?? string.Empty;

            // Un solo SaveChanges: proveedor y persona se guardan juntos (EF usa una transacción implícita).
            await GuardarCambiosAsync(db, ct, campoUnico: nameof(ProveedorEdicionDTO.CUIT), etiquetaUnico: "CUIT");
            return proveedor.PROV_ID;
        }

        /// <summary>Baja lógica o reactivación. Nunca se borra físicamente: las órdenes y precios históricos lo referencian.</summary>
        public async Task CambiarEstadoAsync(int id, bool activo, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            int filas = await db.Proveedores
                .Where(p => p.PROV_ID == id)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.PROV_Activo, activo), ct);
            if (filas == 0)
                throw new ConcurrenciaException("El proveedor ya no existe.");
        }

        public async Task<ProveedorEdicionDTO?> ReactivarAsync(int id, CancellationToken ct = default)
        {
            await CambiarEstadoAsync(id, true, ct);
            return await ObtenerPorIdAsync(id, ct);
        }

        #endregion

        #region Validación

        private static void Normalizar(ProveedorEdicionDTO dto)
        {
            dto.CUIT = Identificadores.SoloDigitos(dto.CUIT);
            dto.RazonSocial = dto.RazonSocial?.Trim() ?? string.Empty;
            dto.Contacto = dto.Contacto?.Trim() ?? string.Empty;
            dto.Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim();
            dto.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim().ToLowerInvariant();
            dto.Direccion = string.IsNullOrWhiteSpace(dto.Direccion) ? null : dto.Direccion.Trim();
            dto.CondicionFiscal = string.IsNullOrWhiteSpace(dto.CondicionFiscal) ? null : dto.CondicionFiscal.Trim();
            dto.Observaciones = string.IsNullOrWhiteSpace(dto.Observaciones) ? null : dto.Observaciones.Trim();
        }

        private static async Task ValidarAsync(Libreria db, ProveedorEdicionDTO dto, CancellationToken ct)
        {
            var e = new Errores();
            e.Si(dto.CUIT == null, nameof(dto.CUIT), "El CUIT es obligatorio.");
            e.Si(dto.CUIT != null && !Identificadores.EsCuitValido(dto.CUIT), nameof(dto.CUIT),
                "El CUIT no es válido (11 dígitos con dígito verificador correcto).");
            e.Requerido(dto.RazonSocial, nameof(dto.RazonSocial), "La razón social", 200);
            e.Requerido(dto.Contacto, nameof(dto.Contacto), "El nombre de contacto", 60);
            e.Si(dto.Telefono == null, nameof(dto.Telefono), "El teléfono es obligatorio.");
            e.Si(dto.Telefono != null && !Identificadores.EsTelefonoValido(dto.Telefono), nameof(dto.Telefono),
                "El teléfono no es válido (sólo números, espacios, guiones, paréntesis y +).");
            e.Si(dto.Email != null && !Identificadores.EsEmailValido(dto.Email), nameof(dto.Email), "El email no tiene un formato válido.");
            e.Si(dto.CondicionFiscal != null && !CondicionFiscal.Todas.Contains(dto.CondicionFiscal), nameof(dto.CondicionFiscal),
                "Seleccione una condición fiscal de la lista.");
            e.Si(dto.Direccion?.Length > 200, nameof(dto.Direccion), "La dirección no puede superar los 200 caracteres.");
            e.Si(dto.Observaciones?.Length > 500, nameof(dto.Observaciones), "Las observaciones no pueden superar los 500 caracteres.");

            // Unicidad del CUIT. En un alta, si choca con un proveedor dado de baja, se ofrece reactivarlo.
            if (dto.CUIT != null && !e.HayErrores)
            {
                var existente = await db.Proveedores.AsNoTracking()
                    .Where(p => p.PROV_CUIT == dto.CUIT && p.PROV_ID != dto.Id)
                    .Select(p => new { p.PROV_ID, p.PROV_Empresa, p.PROV_Activo })
                    .FirstOrDefaultAsync(ct);

                if (existente != null)
                {
                    if (!existente.PROV_Activo && dto.Id == null)
                        throw new EntidadInactivaException(existente.PROV_ID,
                            $"Ya existe un proveedor dado de baja con ese CUIT: \"{existente.PROV_Empresa}\".");
                    e.Si(true, nameof(dto.CUIT), $"El CUIT ya está asignado a \"{existente.PROV_Empresa}\"" +
                                                 (existente.PROV_Activo ? "." : " (inactivo)."));
                }
            }

            e.Lanzar();
        }

        #endregion
    }
}
