using System;
using System.Collections.Generic;
using System.Text;

namespace Modelo.Seguridad
{
    public class Sesion
    {
        private static Sesion instancia;
        public static Sesion Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new Sesion();
                }
                return instancia;
            }
        }
        private Sesion()
        {

        }

        Usuario perfil;
        int sesionId;


        public Usuario Usuario
        {
            get { return perfil; }
            set { perfil = value; }
        }

        public int SES_ID
        {
            get { return sesionId; }
            set { sesionId = value; }
        }


        

    }
}
