using ORAInventario.Entidades;
using ORAInventario.Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORAInventario.Servicios
{
    public class Usuario : General_Servicio
    {

        private String vccCodigoCadena;
        private prp_Usuario vcoVariables;

        #region Constructores
        public Usuario(prp_Usuario pvoVariables, String pvcCadenaConexion)
        {
            vccCodigoCadena = pvcCadenaConexion;
            vcoVariables = pvoVariables;
        }

        public Usuario(prp_Usuario pvoVariables)
        {
            vccCodigoCadena = "";
            vcoVariables = pvoVariables;
        }
        #endregion

        #region Metodos
        public eResultado CargarPropiedades()
        {
            Conexion.Conexion vloConexion = new Conexion.Conexion(vccCodigoCadena);

            using (prv_Usuario vloClase = new prv_Usuario())
                return vloClase.CargarPropiedades(vcoVariables, ref vloConexion);
        }
        #endregion
    }
}