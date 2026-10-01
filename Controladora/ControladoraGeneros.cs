using Microsoft.EntityFrameworkCore;
using Modelo;
using Modelo.Contexto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controladora
{
    public class ControladoraGeneros
    {
        private static ControladoraGeneros instancia;

        public static ControladoraGeneros Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new ControladoraGeneros();
                }
                return instancia;
            }
        }

        public List<Genero> obtenerGeneros()
        {
            return Libreria.Contexto.Generos.ToList();
        }
        public int obtenerIDporNombre(string generoNombre)
        {
            return Libreria.Contexto.Generos.Where(p => p.GEN_Nombre == generoNombre).Select(p => p.GEN_ID).FirstOrDefault();
        }
        public Genero ObtenerGeneroPorId(int generoID)
        {
            return Libreria.Contexto.Generos.FirstOrDefault(p => p.GEN_ID == generoID);
        }
        public void AgregarGenero(Genero genero)
        {
            Libreria.Contexto.Generos.Add(genero);
            Libreria.Contexto.SaveChanges();
        }


















    }

}
