using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORAInventario.Entidades
{
    public class prp_Producto_Categoria : prp_General
    {
        #region Variables
        public Int32 Id { get; set; }
        public String Nombre { get; set; }
        #endregion
    }
}

