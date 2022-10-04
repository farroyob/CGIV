using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORAInventario.Servicios
{
    public class General_Servicio : IDisposable
    {
        protected Int32 vcnTamanoLista;

        public void Dispose() { }

        public General_Servicio()
        {
            vcnTamanoLista = 100;
        }
    }
}