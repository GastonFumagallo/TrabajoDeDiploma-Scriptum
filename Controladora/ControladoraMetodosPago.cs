using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Controladora
{
    public class ControladoraMetodosPago
    {
        private static ControladoraMetodosPago instancia;

        public static ControladoraMetodosPago Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new ControladoraMetodosPago();
                }
                return instancia;
            }
        }

        /// <summary>Medios de pago para combos. Con <paramref name="soloActivos"/> se ocultan los dados de baja (punto de venta).</summary>
        public async Task<List<MetodoPago>> ObtenerMetodosPagoAsync(bool soloActivos, CancellationToken ct = default)
        {
            await using var db = new Libreria();
            var query = db.MetodosPago.AsNoTracking();
            if (soloActivos)
                query = query.Where(m => m.MP_Estado);
            return await query.OrderBy(m => m.MP_Nombre).ToListAsync(ct);
        }

        public bool AgregarMetodo(MetodoPago metodoDePago)
        {
            using var db = new Libreria();
            if (db.MetodosPago.Any(x => x.MP_Nombre == metodoDePago.MP_Nombre))
            {
                return false;
            }
            db.MetodosPago.Add(metodoDePago);
            db.SaveChanges();
            return true;
        }

        public bool ModificarMetodo(MetodoPago metodoDePago)
        {
            using var db = new Libreria();
            var metodoExistente = db.MetodosPago.FirstOrDefault(x => x.MP_ID == metodoDePago.MP_ID);
            if (metodoExistente == null)
            {
                return false;
            }
            db.Entry(metodoExistente).CurrentValues.SetValues(metodoDePago);
            db.SaveChanges();
            return true;
        }

        public ReadOnlyCollection<MetodoPago> obtenerMetodosPago()
        {
            using var db = new Libreria();
            return db.MetodosPago.AsNoTracking().ToList().AsReadOnly();
        }
        public MetodoPago obtenerMetodoPagoPorID(int metodoPagoID)
        {
            using var db = new Libreria();
            return db.MetodosPago.AsNoTracking().FirstOrDefault(p => p.MP_ID == metodoPagoID);
        }

    }
}
