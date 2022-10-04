using ORAInventario.Entidades;
using ORAInventario.Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORAInventario.Servicios
{
    public class Producto_Categoria : General_Servicio
    {

        private String vccCodigoCadena;
        private prp_Producto_Categoria vcoVariables;

        #region Constructores
        public Producto_Categoria(prp_Producto_Categoria pvoVariables, String pvcCadenaConexion)
        {
            vccCodigoCadena = pvcCadenaConexion;
            vcoVariables = pvoVariables;
        }

        public Producto_Categoria(prp_Producto_Categoria pvoVariables)
        {
            vccCodigoCadena = "";
            vcoVariables = pvoVariables;
        }
        #endregion

        #region Metodos
        public eResultado ObtenerCombo()
        {
            Conexion.Conexion vloConexion = new Conexion.Conexion(vccCodigoCadena);

            using (prv_Producto_Categoria vloClase = new prv_Producto_Categoria())
                return vloClase.ObtenerCombo(vcoVariables, ref vloConexion);
        }
        #endregion
    }
}