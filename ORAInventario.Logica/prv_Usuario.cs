using Oracle.ManagedDataAccess.Client;
using ORAInventario.Entidades;
using System;
using System.Text;

namespace ORAInventario.Logica
{
    public class prv_Usuario : prv_General
    {
        #region Metodos
        public eResultado CargarPropiedades(prp_Usuario pvoInfo, ref Conexion.Conexion proConexion)
        {
            eResultado vloResultado = new eResultado();
            StringBuilder vlcSql = new StringBuilder();

            vlcSql.AppendLine("SELECT NVL2(Id,Id,0) AS Id");
            vlcSql.AppendLine("         ,LTRIM(RTRIM(NVL2(Nombre,Nombre,''))) AS Nombre");
            vlcSql.AppendLine("         ,LTRIM(RTRIM(NVL2(Apellido,Apellido,''))) AS Apellido");
            vlcSql.AppendLine("         ,LTRIM(RTRIM(NVL2(Telefono,Telefono,''))) AS Telefono");
            vlcSql.AppendLine("         ,LTRIM(RTRIM(NVL2(Correo,Correo,''))) AS Correo");
            vlcSql.AppendLine("         ,LTRIM(RTRIM(NVL2(Login,Login,''))) AS Login");
            vlcSql.AppendLine("         ,LTRIM(RTRIM(NVL2(Clave,Clave,''))) AS Clave");
            vlcSql.AppendLine("FROM Seg_Usuario");
            vlcSql.AppendLine("WHERE");

            if (pvoInfo.Filtro_Tipo == 0)
            {
                vlcSql.AppendLine("       Id = :Id");
            }
            else
            {
                vlcSql.AppendLine("       Login = :Login");
                vlcSql.AppendLine("       AND Clave = :Clave");
            }

            OracleCommand vloCmd = new OracleCommand();
            vloCmd.CommandText = vlcSql.ToString();
            vloCmd.Parameters.Clear();

            if (pvoInfo.Filtro_Tipo == 0)
            {
                vloCmd.Parameters.Add(new OracleParameter("Id", OracleDbType.Int32)).Value = pvoInfo.Id;
            }
            else
            {
                vloCmd.Parameters.Add(new OracleParameter("Login", OracleDbType.NVarchar2, 50)).Value = pvoInfo.Login;
                vloCmd.Parameters.Add(new OracleParameter("Clave", OracleDbType.NVarchar2, 50)).Value = pvoInfo.Clave;
            }

            vloResultado = proConexion.LlenarDataSet(vloCmd, "Datos");

            if (!vloResultado.Estado)
            {
                return vloResultado;
            }

            if (vloResultado.DatosConsulta.Tables["Datos"].Rows.Count == 0)
            {
                vloResultado.Estado = false;
                vloResultado.Mensaje = "Consulta sin datos";

                return vloResultado;
            }

            pvoInfo.Id = Convert.ToInt32(vloResultado.DatosConsulta.Tables["Datos"].Rows[0]["Id"]);
            pvoInfo.Nombre = (String)vloResultado.DatosConsulta.Tables["Datos"].Rows[0]["Nombre"];
            pvoInfo.Apellido = (String)vloResultado.DatosConsulta.Tables["Datos"].Rows[0]["Apellido"];
            pvoInfo.Telefono = (String)vloResultado.DatosConsulta.Tables["Datos"].Rows[0]["Telefono"];
            pvoInfo.Correo = (String)vloResultado.DatosConsulta.Tables["Datos"].Rows[0]["Correo"];
            pvoInfo.Login = (String)vloResultado.DatosConsulta.Tables["Datos"].Rows[0]["Login"];
            pvoInfo.Clave = (String)vloResultado.DatosConsulta.Tables["Datos"].Rows[0]["Clave"];

            vloResultado.Valor = pvoInfo;

            return vloResultado;
        }
        #endregion
    }
}