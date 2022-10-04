using Oracle.ManagedDataAccess.Client;
using ORAInventario.Entidades;
using System;
using System.Text;

namespace ORAInventario.Logica
{
    public class prv_Sucursal : prv_General
    {

        #region Metodos
        public eResultado ObtenerCombo(prp_Sucursal pvoInfo, ref Conexion.Conexion proConexion)
        {
            StringBuilder vlcSql = new StringBuilder();

            vlcSql.AppendLine("SELECT Id AS Codigo");
            vlcSql.AppendLine("      ,Nombre");
            vlcSql.AppendLine("FROM  Gen_Sucursal");
            vlcSql.AppendLine("ORDER BY Nombre");

            OracleCommand vloCmd = new OracleCommand();
            vloCmd.CommandText = vlcSql.ToString();
            vloCmd.Parameters.Clear();

            return proConexion.LlenarDataSet(vloCmd, "Datos");
        }
        #endregion
    }
}