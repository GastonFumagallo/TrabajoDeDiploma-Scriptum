using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora.Abm
{
    /// <summary>
    /// Base de los servicios ABM: acumula errores de validación y traduce las excepciones de EF/SQL Server
    /// a excepciones de dominio con mensajes claros (nunca llega un stack trace de SQL a la pantalla).
    /// </summary>
    public abstract class ServicioAbmBase
    {
        /// <summary>Nombre de la entidad en singular, para los mensajes ("el libro", "el proveedor"...).</summary>
        protected abstract string NombreEntidad { get; }

        /// <summary>Acumulador de errores por campo. Llamar a <see cref="Lanzar"/> al final de la validación.</summary>
        protected sealed class Errores
        {
            private readonly Dictionary<string, string> errores = new();

            public void Si(bool condicion, string campo, string mensaje)
            {
                if (condicion && !errores.ContainsKey(campo))
                    errores[campo] = mensaje;
            }

            public void Requerido(string? valor, string campo, string etiqueta, int maximo = int.MaxValue)
            {
                Si(string.IsNullOrWhiteSpace(valor), campo, $"{etiqueta} es obligatorio.");
                Si(valor?.Trim().Length > maximo, campo, $"{etiqueta} no puede superar los {maximo} caracteres.");
            }

            public bool HayErrores => errores.Count > 0;

            public void Lanzar()
            {
                if (HayErrores) throw new ValidacionException(errores);
            }
        }

        /// <summary>
        /// SaveChanges con traducción de errores:
        /// concurrencia → <see cref="ConcurrenciaException"/>; clave única → <see cref="ValidacionException"/> sobre
        /// <paramref name="campoUnico"/>; FK y demás → <see cref="PersistenciaException"/>.
        /// </summary>
        protected async Task GuardarCambiosAsync(Libreria db, CancellationToken ct, string? campoUnico = null, string? etiquetaUnico = null)
        {
            try
            {
                await db.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrenciaException(
                    $"Otro usuario modificó o eliminó {NombreEntidad} mientras lo editabas. Cerrá y volvé a abrir la ficha para ver los datos actuales.");
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sql)
            {
                throw sql.Number switch
                {
                    // 2601/2627: violación de índice único o clave primaria.
                    2601 or 2627 when campoUnico != null =>
                        new ValidacionException(campoUnico, $"Ya existe otro registro con ese {etiquetaUnico ?? campoUnico}."),
                    2601 or 2627 => new PersistenciaException($"Ya existe un registro igual a {NombreEntidad}.", ex),
                    // 547: conflicto de clave foránea.
                    547 => new PersistenciaException($"No se puede completar la operación porque {NombreEntidad} está relacionado con otros datos.", ex),
                    // 1205: deadlock; -2: timeout.
                    1205 or -2 => new PersistenciaException("La base de datos está ocupada. Intentá de nuevo en unos segundos.", ex),
                    _ => new PersistenciaException($"No se pudo guardar {NombreEntidad} en la base de datos.", ex),
                };
            }
            catch (DbUpdateException ex)
            {
                throw new PersistenciaException($"No se pudo guardar {NombreEntidad} en la base de datos.", ex);
            }
        }
    }
}
