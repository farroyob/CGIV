using Oracle.ManagedDataAccess.Client;
using ORAInventario.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORAInventario.Logica
{
    public class prv_Producto_Categoria : prv_General
    {
        #region Metodos
        public eResultado ObtenerCombo(prp_Producto_Categoria pvoInfo, ref Conexion.Conexion proConexion)
        {
            StringBuilder vlcSql = new StringBuilder();

            vlcSql.AppendLine("SELECT Id AS Codigo");
            vlcSql.AppendLine("      ,Nombre");
            vlcSql.AppendLine("FROM Inv_Producto_Categoria");
            vlcSql.AppendLine("ORDER BY Nombre");

            OracleCommand vloCmd = new OracleCommand();
            vloCmd.CommandText = vlcSql.ToString();
            vloCmd.Parameters.Clear();

            return proConexion.LlenarDataSet(vloCmd, "Datos");
        }
        #endregion
    }
}