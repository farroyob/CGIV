using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORAInventario.Entidades
{
    public class prp_Usuario : prp_General
    {
        #region Variables
        public Int32 Id { get; set; }
        public String Nombre { get; set; }
        public String Apellido { get; set; }
        public String Telefono { get; set; }
        public String Correo { get; set; }
        public String Login { get; set; }
        public String Clave { get; set; }
        #endregion
    }
}