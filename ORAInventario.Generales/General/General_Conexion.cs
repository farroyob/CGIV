using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORAInventario.Conexion
{
    public static class General_Conexion
    {
        #region CadenaConexion
        /// <summary>
        /// Constructor mandado todos los parametros para la cadena de conexion
        /// </summary>
        /// <param name="pvcServidor">Direccion del servidor</param>
        /// <param name="pvcUsuario">Nombre del usuario</param>
        /// <param name="pvcClave">Clave del usuario</param>
        /// <returns></returns>
        public static String Obtener_CadenaConexion(String pvcServidor, String pvcUsuario, String pvcClave)
        {
            String vlcCadenaConexion;
            OracleConnection vloConexion;

            vlcCadenaConexion = "Data Source=" + pvcServidor + ";User Id=" + pvcUsuario + ";Password=" + pvcClave + ";";

            vloConexion = new OracleConnection(vlcCadenaConexion);

            return vloConexion.ConnectionString;
        }
        #endregion
    }
}
