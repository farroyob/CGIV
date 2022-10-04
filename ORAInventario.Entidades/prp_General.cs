using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORAInventario.Entidades
{
    public class prp_General
    {

        public Int32 Compania { get; set; }
        public String Usuario_Login { get; set; }
        public Int32 Usuario_Id { get; set; }
        public DateTime FDesde { get; set; }
        public DateTime FHasta { get; set; }
        public String Filtro { get; set; }
        public String Ordenamiento_Campo { get; set; }
        public String Ordenamiento_Tipo { get; set; }
        public Int32 Filtro_Tipo { get; set; }
        public Int32 Index_Paginacion { get; set; }
        public Int32 Indice { get; set; }
        public Int32 Tamano_Lista { get; set; }
    }
}
