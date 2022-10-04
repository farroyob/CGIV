using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORAInventario.Entidades
{
    public class prp_Producto : prp_General
    {
        #region Variables
        public Int32 Id { get; set; }
        public String Nombre { get; set; }
        public Decimal Precio { get; set; }
        public Int32 Cantidad { get; set; }
        public Int32 Categoria { get; set; }
        public Int32 Sucursal { get; set; }
        public Int32 Usuario_Registra { get; set; }
        #endregion
    }
}