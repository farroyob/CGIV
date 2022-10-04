using Oracle.ManagedDataAccess.Client;
using ORAInventario.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORAInventario.Logica
{
    public class prv_Producto : prv_General
    {

        #region Metodos
        public eResultado Guardar(prp_Producto pvoInfo, ref Conexion.Conexion proConexion)
        {
            StringBuilder vlcSql = new StringBuilder();
            eResultado vloResultado = new eResultado();
            Int32 vlnMax;

            vloResultado = Max(ref proConexion);

            if (!vloResultado.Estado)
            {
                return vloResultado;
            }

            vlnMax = Convert.ToInt32(vloResultado.Valor);

            vlcSql.AppendLine("INSERT INTO Inv_Producto");
            vlcSql.AppendLine("(");
            vlcSql.AppendLine("        Id");
            vlcSql.AppendLine("        ,Nombre");
            vlcSql.AppendLine("        ,Precio");
            vlcSql.AppendLine("        ,Cantidad");
            vlcSql.AppendLine("        ,Categoria");
            vlcSql.AppendLine("        ,Sucursal");
            vlcSql.AppendLine("        ,Usuario_Registra");
            vlcSql.AppendLine(")");
            vlcSql.AppendLine("VALUES");
            vlcSql.AppendLine("(");
            vlcSql.AppendLine("         :Id");
            vlcSql.AppendLine("        ,:Nombre");
            vlcSql.AppendLine("        ,:Precio");
            vlcSql.AppendLine("        ,:Cantidad");
            vlcSql.AppendLine("        ,:Categoria");
            vlcSql.AppendLine("        ,:Sucursal");
            vlcSql.AppendLine("        ,:Usuario_Registra");
            vlcSql.AppendLine(")");

            OracleCommand vloCmd = new OracleCommand();
            vloCmd.CommandText = vlcSql.ToString();
            vloCmd.Parameters.Clear();
            vloCmd.Parameters.Add(new OracleParameter("Id", OracleDbType.Int32)).Value = vlnMax;
            vloCmd.Parameters.Add(new OracleParameter("Nombre", OracleDbType.Varchar2, 200)).Value = pvoInfo.Nombre;
            vloCmd.Parameters.Add(new OracleParameter("Precio", OracleDbType.Decimal)).Value = pvoInfo.Precio;
            vloCmd.Parameters.Add(new OracleParameter("Cantidad", OracleDbType.Int32)).Value = pvoInfo.Cantidad;
            vloCmd.Parameters.Add(new OracleParameter("Categoria", OracleDbType.Int32)).Value = pvoInfo.Categoria;
            vloCmd.Parameters.Add(new OracleParameter("Sucursal", OracleDbType.Int32)).Value = pvoInfo.Sucursal;
            vloCmd.Parameters.Add(new OracleParameter("Usuario_Registra", OracleDbType.Int32)).Value = pvoInfo.Usuario_Registra;

            vloResultado = proConexion.EjecutarSQL(vloCmd);

            if (vloResultado.Estado)
            {
                vloResultado.Valor = vlnMax;
            }
            else
            {
                vloResultado.Valor = 0;
            }

            return vloResultado;
        }

        public eResultado Modificar(prp_Producto pvoInfo, ref Conexion.Conexion proConexion)
        {
            StringBuilder vlcSql = new StringBuilder();

            vlcSql.AppendLine("UPDATE Inv_Producto SET Nombre = :Nombre");
            vlcSql.AppendLine("                         ,Precio = :Precio");
            vlcSql.AppendLine("                         ,Cantidad = :Cantidad");
            //vlcSql.AppendLine("                         ,Categoria = :Categoria");
            vlcSql.AppendLine("WHERE Id = :Id");

            OracleCommand vloCmd = new OracleCommand();
            vloCmd.CommandText = vlcSql.ToString();
            vloCmd.Parameters.Clear();
            vloCmd.Parameters.Add(new OracleParameter("Id", OracleDbType.Int32)).Value = pvoInfo.Id;
            vloCmd.Parameters.Add(new OracleParameter("Nombre", OracleDbType.Varchar2)).Value = pvoInfo.Nombre;
            vloCmd.Parameters.Add(new OracleParameter("Precio", OracleDbType.Decimal)).Value = pvoInfo.Precio;
            vloCmd.Parameters.Add(new OracleParameter("Cantidad", OracleDbType.Int16)).Value = Convert.ToInt16 (pvoInfo.Cantidad);
            //vloCmd.Parameters.Add(new OracleParameter("Categoria", OracleDbType.Int32)).Value = pvoInfo.Categoria;

            return proConexion.EjecutarSQL(vloCmd);
        }

        public eResultado Eliminar(prp_Producto pvoInfo, ref Conexion.Conexion proConexion)
        {
            StringBuilder vlcSql = new StringBuilder();

            vlcSql.AppendLine("DELETE FROM Inv_Producto");
            vlcSql.AppendLine("WHERE Id = :Id");

            OracleCommand vloCmd = new OracleCommand();
            vloCmd.CommandText = vlcSql.ToString();
            vloCmd.Parameters.Clear();
            vloCmd.Parameters.Add(new OracleParameter("Id", OracleDbType.Int32)).Value = pvoInfo.Id;

            return proConexion.EjecutarSQL(vloCmd);
        }

        public eResultado ObtenerLista(prp_Producto pvoInfo, ref Conexion.Conexion proConexion, Int32 pvnTamanoLista)
        {
            StringBuilder vlcSql = new StringBuilder();

            vlcSql.AppendLine("WITH tLista AS");
            vlcSql.AppendLine("(");
            vlcSql.AppendLine("      SELECT ROW_NUMBER() OVER (ORDER BY tInventario.Id ASC) AS NumeroFila");
            vlcSql.AppendLine("               ,tInventario.Id");
            vlcSql.AppendLine("               ,tInventario.Nombre");
            vlcSql.AppendLine("               ,tInventario.Precio");
            vlcSql.AppendLine("               ,tInventario.Cantidad");
            vlcSql.AppendLine("               ,LOWER(tInventario_Categoria.Nombre) AS Categoria");
            vlcSql.AppendLine("      FROM  Inv_Producto tInventario");
            vlcSql.AppendLine("      INNER JOIN Inv_Producto_Categoria tInventario_Categoria ON tInventario.Categoria = tInventario_Categoria.Id");
            vlcSql.AppendLine("      WHERE (LOWER(CONCAT(CONCAT(CAST(tInventario.Id AS VARCHAR2(30)), tInventario.Nombre), tInventario_Categoria.Nombre))) LIKE '%' || LOWER(:Filtro) || '%'");
            vlcSql.AppendLine(")");
            vlcSql.AppendLine("SELECT Id");
            vlcSql.AppendLine("      ,Nombre");
            vlcSql.AppendLine("      ,Categoria");
            vlcSql.AppendLine("      ,Cantidad");
            vlcSql.AppendLine("      ,Precio");
            vlcSql.AppendLine("FROM  tLista");
            vlcSql.AppendLine("WHERE NumeroFila BETWEEN " + ((((pvoInfo.Index_Paginacion - 1) * pvnTamanoLista) + 1)).ToString() + " AND " + (((pvoInfo.Index_Paginacion) * pvnTamanoLista)).ToString());

            OracleCommand vloCmd = new OracleCommand();
            vloCmd.CommandText = vlcSql.ToString();
            vloCmd.Parameters.Clear();
            vloCmd.Parameters.Add(new OracleParameter("Filtro", OracleDbType.NVarchar2, 100)).Value = pvoInfo.Filtro;

            return proConexion.LlenarDataSet(vloCmd, "Datos");
        }


        public eResultado ObtenerLista_Cantidad(prp_Producto pvoInfo, ref Conexion.Conexion pvo_Conexion, Int32 pvnTamanoLista)
        {
            StringBuilder vlcSql = new StringBuilder();
            eResultado vloResultado = new eResultado();

            vlcSql.AppendLine("SELECT COUNT(tInventario.Id) AS Total");
            vlcSql.AppendLine("FROM Inv_Producto tInventario");
            vlcSql.AppendLine("INNER JOIN Inv_Producto_Categoria tInventario_Categoria ON tInventario.Categoria = tInventario_Categoria.Id");
            vlcSql.AppendLine("WHERE (LOWER(CONCAT(CONCAT(CAST(tInventario.Id AS VARCHAR2(30)), tInventario.Nombre), tInventario_Categoria.Nombre))) LIKE '%' || LOWER(:Filtro) || '%'");

            OracleCommand vloCmd = new OracleCommand();
            vloCmd.CommandText = vlcSql.ToString();
            vloCmd.Parameters.Clear();
            vloCmd.Parameters.Add(new OracleParameter("Filtro", OracleDbType.NVarchar2, 100)).Value = pvoInfo.Filtro;

            vloResultado = pvo_Conexion.LlenarDataSet(vloCmd, "Datos", false);

            if (!vloResultado.Estado)
            {
                vloResultado.Valor = 0;

                return vloResultado;
            }

            if (vloResultado.DatosConsulta.Tables["Datos"].Rows.Count == 0)
            {
                vloResultado.Estado = false;
                vloResultado.Mensaje = "Consulta sin datos";
                vloResultado.Valor = 0;

                return vloResultado;
            }

            vloResultado.Valor = vloResultado.DatosConsulta.Tables["Datos"].Rows[0]["Total"];

            return vloResultado;
        }

        public eResultado CargarPropiedades(prp_Producto pvoInfo, ref Conexion.Conexion proConexion)
        {
            eResultado vloResultado = new eResultado();
            StringBuilder vlcSql = new StringBuilder();

            vlcSql.AppendLine("SELECT    Id");
            vlcSql.AppendLine("         ,LTRIM(RTRIM(Nombre)) AS Nombre");
            vlcSql.AppendLine("         ,NVL2(Precio,Precio,0) AS Precio");
            vlcSql.AppendLine("         ,NVL2(Cantidad,Cantidad,0) AS Cantidad");
            vlcSql.AppendLine("         ,NVL2(Categoria,Categoria,0) AS Categoria");
            vlcSql.AppendLine("         ,NVL2(Sucursal,Sucursal,0) AS Sucursal");
            vlcSql.AppendLine("         ,NVL2(Usuario_Registra,Usuario_Registra,0) AS Usuario_Registra");
            vlcSql.AppendLine("FROM  Inv_Producto");
            vlcSql.AppendLine("WHERE Id = :Id");

            OracleCommand vloCmd = new OracleCommand();
            vloCmd.CommandText = vlcSql.ToString();
            vloCmd.Parameters.Clear();
            vloCmd.Parameters.Add(new OracleParameter("Id", OracleDbType.Int32)).Value = pvoInfo.Id;

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
            pvoInfo.Precio = (Decimal)vloResultado.DatosConsulta.Tables["Datos"].Rows[0]["Precio"];
            pvoInfo.Cantidad = Convert.ToInt32(vloResultado.DatosConsulta.Tables["Datos"].Rows[0]["Cantidad"]);
            pvoInfo.Categoria = Convert.ToInt32(vloResultado.DatosConsulta.Tables["Datos"].Rows[0]["Categoria"]);
            pvoInfo.Sucursal = Convert.ToInt32(vloResultado.DatosConsulta.Tables["Datos"].Rows[0]["Sucursal"]);
            pvoInfo.Usuario_Registra = Convert.ToInt32(vloResultado.DatosConsulta.Tables["Datos"].Rows[0]["Usuario_Registra"]);

            vloResultado.Valor = pvoInfo;

            return vloResultado;
        }

        public eResultado Max(ref Conexion.Conexion proConexion)
        {
            eResultado vloResultado;
            StringBuilder vlcSql = new StringBuilder();

            vlcSql.AppendLine("SELECT NVL2(MAX(Id),MAX(Id),0) + 1 AS Max");
            vlcSql.AppendLine("FROM INV_Producto");

            OracleCommand vloCmd = new OracleCommand();
            vloCmd.CommandText = vlcSql.ToString();
            vloCmd.Parameters.Clear();

            vloResultado = proConexion.LlenarDataSet(vloCmd, "Datos");

            vloResultado.Valor = 0;

            if (vloResultado.Estado)
            {
                if (vloResultado.DatosConsulta.Tables.Count > 0)
                {
                    if (vloResultado.DatosConsulta.Tables["Datos"].Rows.Count > 0)
                    {
                        vloResultado.Valor = Convert.ToInt32(vloResultado.DatosConsulta.Tables["Datos"].Rows[0]["Max"]);
                    }
                }
            }

            return vloResultado;
        }

        #endregion
    }
}