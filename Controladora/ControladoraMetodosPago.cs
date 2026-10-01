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

        public bool AgregarMetodo(MetodoPago metodoDePago)
        {
            var metodoExistente = Libreria.Contexto.MetodosPago.FirstOrDefault(x => x.MP_Nombre == metodoDePago.MP_Nombre);
            if (metodoExistente == null)
            {
                Libreria.Contexto.MetodosPago.Add(metodoDePago);
                Libreria.Contexto.SaveChanges();
                return true;
            }
            return false;
        }

        public bool ModificarMetodo(MetodoPago metodoDePago)
        {
            var metodoExistente = Libreria.Contexto.MetodosPago.FirstOrDefault(x => x.MP_ID == metodoDePago.MP_ID);
            if (metodoExistente != null)
            {
                Libreria.Contexto.MetodosPago.Update(metodoDePago);
                Libreria.Contexto.SaveChanges(true);
                return true;
            }
            return false;
        }

        public ReadOnlyCollection<MetodoPago> obtenerMetodosPago()
        {
            return Libreria.Contexto.MetodosPago.ToList().AsReadOnly();
        }
        public MetodoPago obtenerMetodoPagoPorID(int metodoPagoID)
        {
            return Libreria.Contexto.MetodosPago.FirstOrDefault(p => p.MP_ID == metodoPagoID);
        }

    }
}
