using ORAInventario.Entidades;
using ORAInventario.Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORAInventario.Servicios
{
    public class Sucursal : General_Servicio
    {

        private String vccCodigoCadena;
        private prp_Sucursal vcoVariables;

        #region Constructores
        public Sucursal(prp_Sucursal pvoVariables, String pvcCadenaConexion)
        {
            vccCodigoCadena = pvcCadenaConexion;
            vcoVariables = pvoVariables;
        }

        public Sucursal(prp_Sucursal pvoVariables)
        {
            vccCodigoCadena = "";
            vcoVariables = pvoVariables;
        }
        #endregion

        #region Metodos
        public eResultado ObtenerCombo()
        {
            Conexion.Conexion vloConexion = new Conexion.Conexion(vccCodigoCadena);

            using (prv_Sucursal vloClase = new prv_Sucursal())
                return vloClase.ObtenerCombo(vcoVariables, ref vloConexion);
        }
     
        #endregion
    }
}