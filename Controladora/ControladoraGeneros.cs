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

        public void AgregarGenero(Genero genero)
        {
            using var db = new Libreria();
            db.Generos.Add(genero);
            db.SaveChanges();
        }


















    }

}
