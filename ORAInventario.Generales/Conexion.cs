using Oracle.ManagedDataAccess.Client;
using ORAInventario.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORAInventario.Conexion
{
    public class Conexion : IDisposable
    {
        public void Dispose() { }

        //Los parametros de this.ConexionDB se heredan de la ventana a la que pertenezca
        //atributo para la coneccion con la base de datos
        private OracleConnection ConexionDB { get; set; }
        private OracleTransaction Transaccion { get; set; }
        private Boolean TransaccionAbierta { get; set; }
        public String vccCadena { get; set; }

        #region Constructor
        /// <summary>
        /// Constructor mandado la cadena de conexion
        /// </summary>
        /// <param name="pvcServidor">Dir Servidor</param>
        /// <param name="pvcUsuario">DBUsuario</param>
        /// <param name="pvcClave">DBClave</param>
        public Conexion(String pvcServidor, String pvcUsuario, String pvcClave)
        {
            String vlcCadenaConexion;

            vlcCadenaConexion = "Data Source=" + pvcServidor + ";User Id=" + pvcUsuario + ";Password=" + pvcClave + ";Persist Security Info=True;Pooling = false;";

            this.ConexionDB = new OracleConnection(vlcCadenaConexion);

            vccCadena = this.ConexionDB.ConnectionString;
        }
        #endregion

        /// <summary>
        /// Constructor mandado la cadena de conexion
        /// </summary>
        /// <param name="pvcCadena">Cadena de Conexion</param>
        public Conexion(String pvcCadena)
        {
            this.ConexionDB = new OracleConnection(pvcCadena);

            vccCadena = this.ConexionDB.ConnectionString;
        }

        #region ConectarBaseDatos
        public eResultado ConectarBaseDatos()
        {
            try
            {
                if (this.ConexionDB.State != ConnectionState.Open)
                {
                    this.ConexionDB.Open();
                }
            }
            catch (Exception vloError)
            {
                return GenerarEnvioInformacion(false, vloError.Message, 0);
            }

            return GenerarEnvioInformacion(true, "", 0);
        }

        #endregion

        #region DesconectarBaseDatos
        public eResultado DesconectarBaseDatos()
        {
            try
            {
                this.ConexionDB.Close();
            }
            catch (Exception vloError)
            {
                return GenerarEnvioInformacion(false, vloError.Message, 0);
            }

            return GenerarEnvioInformacion(true, "", 0);
        }

        #endregion

        #region OpenTrans
        public eResultado AbrirTransaccion()
        {
            try
            {
                this.ConexionDB.Open();

                this.Transaccion = this.ConexionDB.BeginTransaction();

                TransaccionAbierta = true;
            }
            catch (Exception vloError)
            {
                TransaccionAbierta = false;

                return GenerarEnvioInformacion(false, vloError.Message, 0);
            }

            return GenerarEnvioInformacion(true, "", 0);
        }
        #endregion

        #region CerrarTrasaccion
        public eResultado CerrarTrasaccion()
        {
            try
            {
                this.Transaccion.Commit();

                this.ConexionDB.Close();
            }
            catch (Exception vloError)
            {
                return GenerarEnvioInformacion(false, vloError.Message, 0);
            }
            finally
            {
                TransaccionAbierta = false;
            }

            return GenerarEnvioInformacion(true, "", 0);
        }
        #endregion

        #region RealizarRollBack
        public eResultado RealizarRollBack()
        {
            try
            {
                try
                {
                    this.Transaccion.Rollback();
                }
                catch { }

                TransaccionAbierta = false;

                this.ConexionDB.Close();

                //this.ConexionDB.Dispose();
            }
            catch (Exception vloError)
            {
                return GenerarEnvioInformacion(false, vloError.Message, 0);
            }

            TransaccionAbierta = false;

            return GenerarEnvioInformacion(true, "", 0);
        }
        #endregion

        #region EjecutarSQL
        public eResultado EjecutarSQL_Masivo(DataTable pvo_Datos, String pvc_Tabla)
        {
            try
            {
                ConectarBaseDatos();
                OracleBulkCopy vlo_Comando = new OracleBulkCopy(this.ConexionDB);
                vlo_Comando.DestinationTableName = "dbo." + pvc_Tabla;
                vlo_Comando.WriteToServer(pvo_Datos);
                vlo_Comando.Close();
                return GenerarEnvioInformacion(true, "", pvo_Datos.Rows.Count);
            }
            catch (OracleException vloExcepcion)
            {
                RealizarRollBack();

                if (vloExcepcion.Number == 2106 || vloExcepcion.Number == 2627)
                {
                    String vlcMensaje = "El código ingresado ya existe.";

                    return GenerarEnvioInformacion(false, vlcMensaje, 0);
                }

                return GenerarEnvioInformacion(false, vloExcepcion.Message, 0);
            }
            catch (Exception vloError)
            {
                return GenerarEnvioInformacion(false, "Devlis " + vloError.Message, 0);
            }
        }

        public eResultado EjecutarSQL(String pvc_Sql, Boolean pvb_Scalar = false)
        {
            if (TransaccionAbierta)
            {
                if (pvb_Scalar)
                {
                    return EjecutaSQL_Transaccional_Scalar(pvc_Sql);
                }
                else
                {
                    return EjecutaSQL_Transaccional(pvc_Sql);
                }
            }
            else
            {
                if (pvb_Scalar)
                {
                    return EjecutaSQL_NoTransaccional_Scalar(pvc_Sql);
                }
                else
                {
                    return EjecutaSQL_NoTransaccional(pvc_Sql);
                }
            }
        }

        private eResultado EjecutaSQL_Transaccional(String pvc_Sql)
        {
            OracleCommand vlo_Comando = new OracleCommand(pvc_Sql, this.ConexionDB);
            Int64 vln_RegistrosAfectados;

            try
            {
                vlo_Comando.Connection = this.ConexionDB;
                vlo_Comando.Transaction = this.Transaccion;
                vlo_Comando.CommandTimeout = 0;

                vln_RegistrosAfectados = vlo_Comando.ExecuteNonQuery();

                return GenerarEnvioInformacion(true, "", vln_RegistrosAfectados);
            }
            catch (OracleException vloExcepcion)
            {
                RealizarRollBack();
                if (vloExcepcion.Number == 2106 || vloExcepcion.Number == 2627)
                {
                    String vlcMensaje = "El código ingresado ya existe.";
                    return GenerarEnvioInformacion(false, vlcMensaje, 0);
                }
                return GenerarEnvioInformacion(false, vloExcepcion.Message, 0);
            }
            catch (Exception vloError)
            {
                RealizarRollBack();

                return GenerarEnvioInformacion(false, vloError.Message, 0);
            }
        }

        private eResultado EjecutaSQL_NoTransaccional(String pvc_SQL)
        {
            eResultado vloResultado = new eResultado();
            Int64 vln_RegistosAfectados;
            OracleCommand vlo_Comando;

            try
            {
                vloResultado = ConectarBaseDatos();

                if (!vloResultado.Estado)
                {
                    return vloResultado;
                }

                vlo_Comando = new OracleCommand(pvc_SQL, this.ConexionDB);
                vlo_Comando.CommandTimeout = 0;
                vln_RegistosAfectados = vlo_Comando.ExecuteNonQuery();

                vloResultado = DesconectarBaseDatos();

                if (!vloResultado.Estado)
                {
                    return vloResultado;
                }
                else
                {
                    return GenerarEnvioInformacion(true, "", vln_RegistosAfectados);
                }
            }
            catch (OracleException vloExcepcion)
            {
                RealizarRollBack();

                if (vloExcepcion.Number == 2106 || vloExcepcion.Number == 2627)
                {
                    String vlcMensaje = "El código ingresado ya existe.";
                    return GenerarEnvioInformacion(false, vlcMensaje, 0);
                }
                return GenerarEnvioInformacion(false, vloExcepcion.Message, 0);
            }
            catch (Exception vloError)
            {
                return GenerarEnvioInformacion(false, vloError.Message, 0);
            }
        }

        private eResultado EjecutaSQL_Transaccional_Scalar(String pvc_Sql)
        {
            OracleCommand vlo_Comando = new OracleCommand(pvc_Sql, this.ConexionDB);
            Int64 vln_RegistrosAfectados;

            try
            {
                vlo_Comando.Connection = this.ConexionDB;
                vlo_Comando.Transaction = this.Transaccion;
                vlo_Comando.CommandTimeout = 0;

                vlo_Comando.ExecuteScalar();

                vln_RegistrosAfectados = 0;

                return GenerarEnvioInformacion(true, "", vln_RegistrosAfectados);
            }
            catch (OracleException vloExcepcion)
            {
                RealizarRollBack();

                if (vloExcepcion.Number == 2106 || vloExcepcion.Number == 2627)
                {
                    String vlcMensaje = "El código ingresado ya existe.";

                    return GenerarEnvioInformacion(false, vlcMensaje, 0);
                }
                return GenerarEnvioInformacion(false, vloExcepcion.Message, 0);
            }
            catch (Exception vloError)
            {
                RealizarRollBack();

                return GenerarEnvioInformacion(false, vloError.Message, 0);
            }
        }

        private eResultado EjecutaSQL_NoTransaccional_Scalar(String pvc_SQL)
        {
            eResultado vloResultado = new eResultado();
            Int64 vln_RegistosAfectados;
            OracleCommand vlo_Comando;

            try
            {
                vloResultado = ConectarBaseDatos();

                if (!vloResultado.Estado)
                {
                    return vloResultado;
                }

                vlo_Comando = new OracleCommand(pvc_SQL, this.ConexionDB);
                vlo_Comando.CommandTimeout = 0;

                vlo_Comando.ExecuteScalar();

                vln_RegistosAfectados = 0;

                vloResultado = DesconectarBaseDatos();

                if (!vloResultado.Estado)
                {
                    return vloResultado;
                }
                else
                {
                    return GenerarEnvioInformacion(true, "", vln_RegistosAfectados);
                }
            }
            catch (OracleException vloExcepcion)
            {
                RealizarRollBack();
                if (vloExcepcion.Number == 2106 || vloExcepcion.Number == 2627)
                {
                    String vlcMensaje = "El código ingresado ya existe.";

                    return GenerarEnvioInformacion(false, vlcMensaje, 0);
                }
                return GenerarEnvioInformacion(false, vloExcepcion.Message, 0);
            }
            catch (Exception vloError)
            {
                return GenerarEnvioInformacion(false, vloError.Message, 0);
            }
        }

        public eResultado EjecutarSQL(OracleCommand pvo_Comando, Boolean pvb_Seriable = false, Boolean pvb_Scalar = false)
        {
            if (TransaccionAbierta)
            {
                if (pvb_Scalar)
                {
                    return EjecutaSQL_T_Scalar(pvo_Comando, pvb_Seriable);
                }
                else
                {
                    return EjecutaSQL_T(pvo_Comando, pvb_Seriable);
                }
            }
            else
            {
                if (pvb_Scalar)
                {
                    return EjecutaSQL_N_Scalar(pvo_Comando, pvb_Seriable);
                }
                else
                {
                    return EjecutaSQL_N(pvo_Comando, pvb_Seriable);
                }
            }
        }

        private eResultado EjecutaSQL_T(OracleCommand pvo_Comando, Boolean pvb_Seriable)
        {
            Int64 vln_RegistosAfectados;

            try
            {
                vln_RegistosAfectados = 0;

                pvo_Comando.Connection = this.ConexionDB;
                pvo_Comando.Transaction = this.Transaccion;
                pvo_Comando.CommandTimeout = 0;

                if (pvb_Seriable)
                {
                    pvo_Comando.CommandType = CommandType.StoredProcedure;
                }

                vln_RegistosAfectados = pvo_Comando.ExecuteNonQuery();

                return GenerarEnvioInformacion(true, "", vln_RegistosAfectados);
            }
            catch (OracleException vloExcepcion)
            {
                RealizarRollBack();

                if (vloExcepcion.Number == 2106 || vloExcepcion.Number == 2627)
                {
                    String vlcMensaje = "El código ingresado ya existe.";

                    return GenerarEnvioInformacion(false, vlcMensaje, 0);
                }

                return GenerarEnvioInformacion(false, vloExcepcion.Message, 0);
            }
            catch (Exception vloError)
            {
                RealizarRollBack();

                return GenerarEnvioInformacion(false, vloError.Message, 0);
            }
        }

        private eResultado EjecutaSQL_N(OracleCommand pvo_Comando, Boolean pvb_Seriable)
        {
            eResultado vloResultado = new eResultado();
            Int64 vln_RegistosAfectados;

            try
            {
                vln_RegistosAfectados = 0;

                vloResultado = ConectarBaseDatos();

                if (!vloResultado.Estado)
                {
                    return vloResultado;
                }

                if (pvb_Seriable)
                {
                    pvo_Comando.CommandType = CommandType.StoredProcedure;
                }

                pvo_Comando.Connection = this.ConexionDB;
                pvo_Comando.CommandTimeout = 0;

                vln_RegistosAfectados = pvo_Comando.ExecuteNonQuery();

                vloResultado = DesconectarBaseDatos();

                if (!vloResultado.Estado)
                {
                    return vloResultado;
                }
                else
                {
                    return GenerarEnvioInformacion(true, "", vln_RegistosAfectados);
                }
            }
            catch (OracleException vloExcepcion)
            {
                RealizarRollBack();
                if (vloExcepcion.Number == 2106 || vloExcepcion.Number == 2627)
                {
                    String vlcMensaje = "El código ingresado ya existe.";
                    return GenerarEnvioInformacion(false, vlcMensaje, 0);
                }
                return GenerarEnvioInformacion(false, vloExcepcion.Message, 0);
            }
            catch (Exception vloError)
            {
                return GenerarEnvioInformacion(false, vloError.Message, 0);
            }
        }

        private eResultado EjecutaSQL_T_Scalar(OracleCommand pvo_Comando, Boolean pvb_Seriable)
        {
            Int64 vln_RegistosAfectados;

            try
            {
                vln_RegistosAfectados = 0;

                pvo_Comando.Connection = this.ConexionDB;
                pvo_Comando.Transaction = this.Transaccion;
                pvo_Comando.CommandTimeout = 0;

                if (pvb_Seriable)
                {
                    pvo_Comando.CommandType = CommandType.StoredProcedure;
                }

                pvo_Comando.ExecuteScalar();

                vln_RegistosAfectados = 0;

                return GenerarEnvioInformacion(true, "", vln_RegistosAfectados);
            }
            catch (OracleException vloExcepcion)
            {
                RealizarRollBack();
                if (vloExcepcion.Number == 2106 || vloExcepcion.Number == 2627)
                {
                    String vlcMensaje = "El código ingresado ya existe.";
                    return GenerarEnvioInformacion(false, vlcMensaje, 0);
                }
                return GenerarEnvioInformacion(false, vloExcepcion.Message, 0);
            }
            catch (Exception vloError)
            {
                RealizarRollBack();

                return GenerarEnvioInformacion(false, vloError.Message, 0);
            }
        }

        private eResultado EjecutaSQL_N_Scalar(OracleCommand pvo_Comando, Boolean pvb_Seriable)
        {
            eResultado vloResultado = new eResultado();
            Int64 vln_RegistosAfectados;

            try
            {
                vln_RegistosAfectados = 0;

                vloResultado = ConectarBaseDatos();

                if (!vloResultado.Estado)
                {
                    return vloResultado;
                }

                if (pvb_Seriable)
                {
                    pvo_Comando.CommandType = CommandType.StoredProcedure;
                }

                pvo_Comando.Connection = this.ConexionDB;
                pvo_Comando.CommandTimeout = 0;

                pvo_Comando.ExecuteScalar();

                vln_RegistosAfectados = 0;

                vloResultado = DesconectarBaseDatos();

                if (!vloResultado.Estado)
                {
                    return vloResultado;
                }
                else
                {
                    return GenerarEnvioInformacion(true, "", vln_RegistosAfectados);
                }
            }
            catch (OracleException vloExcepcion)
            {
                RealizarRollBack();

                if (vloExcepcion.Number == 2106 || vloExcepcion.Number == 2627)
                {
                    String vlcMensaje = "El código ingresado ya existe.";
                    return GenerarEnvioInformacion(false, vlcMensaje, 0);
                }
                return GenerarEnvioInformacion(false, vloExcepcion.Message, 0);
            }
            catch (Exception vloError)
            {
                return GenerarEnvioInformacion(false, vloError.Message, 0);
            }
        }
        #endregion

        #region LlenarDataSet
        //public eResultado LlenarDataSet(ref DataSet pro_Datos, String pvc_Sql, String pvc_Nombre)
        //{
        //    //verifica que si la trasaccion esta abierta
        //    if (TransaccionAbierta)
        //    {
        //        //llena el data set en una transaccion
        //        return LlenarDS_Transaccional(ref pro_Datos, pvc_Sql, pvc_Nombre);
        //    }
        //    else
        //    {
        //        return LlenarDS_NoTransaccional(ref pro_Datos, pvc_Sql, pvc_Nombre);
        //    }
        //}

        //private eResultado LlenarDS_Transaccional(ref DataSet pro_Datos, String pvc_Sql, String pvc_Nombre)
        //{
        //    OracleCommand vlo_Comando;
        //    OracleDataAdapter vlo_Adaptador;

        //    try
        //    {
        //        //ejecuta el query
        //        vlo_Comando = new OracleCommand(pvc_Sql, this.ConexionDB, this.Transaccion);
        //        vlo_Comando.CommandTimeout = 0;
        //        vlo_Comando.ExecuteNonQuery();

        //        //llena el dataset
        //        vlo_Adaptador = new OracleDataAdapter(vlo_Comando);
        //        vlo_Adaptador.Fill(pro_Datos, pvc_Nombre);

        //        //retorna los parametros
        //        return GenerarEnvioInformacion(true, "", pro_Datos.Tables[pvc_Nombre].Rows.Count);
        //    }
        //    catch (Exception vloError)
        //    {
        //        RealizarRollBack();

        //        //retorna los parametros
        //        return GenerarEnvioInformacion(false, vloError.Message, 0);
        //    }
        //}

        private eResultado LlenarDS_NoTransaccional(ref DataSet pro_Datos, String pvc_Sql, String pvc_Nombre)
        {
            eResultado vloResultado = new eResultado();
            OracleDataAdapter vlo_Adaptador;

            try
            {
                vloResultado = ConectarBaseDatos();

                if (!vloResultado.Estado)
                {
                    // sino se conecto devuelve el Array con el error
                    return vloResultado;
                }

                vlo_Adaptador = new OracleDataAdapter(pvc_Sql, this.ConexionDB);
                vlo_Adaptador.Fill(pro_Datos, pvc_Nombre);

                vloResultado = DesconectarBaseDatos();

                if (!vloResultado.Estado)
                {

                    return vloResultado;
                }
                else
                {
                    //retorna los parametros
                    return GenerarEnvioInformacion(true, "", pro_Datos.Tables[pvc_Nombre].Rows.Count);
                }
            }
            catch (Exception vloError)
            {
                DesconectarBaseDatos();

                //retorna los parametros
                return GenerarEnvioInformacion(false, vloError.Message, 0);
            }
        }

        //public eResultado LlenarDataSet(String pvc_Sql, String pvc_Nombre)
        //{
        //    //verifica que si la trasaccion esta abierta
        //    if (TransaccionAbierta)
        //    {
        //        //llena el data set en una transaccion
        //        return LlenarDS_Transaccional(pvc_Sql, pvc_Nombre);
        //    }
        //    else
        //    {
        //        return LlenarDS_NoTransaccional(pvc_Sql, pvc_Nombre);
        //    }
        //}

        //private eResultado LlenarDS_Transaccional(String pvc_Sql, String pvc_Nombre)
        //{
        //    OracleCommand vlo_Comando;
        //    OracleDataAdapter vlo_Adaptador;
        //    DataSet vlo_Datos = new DataSet();

        //    try
        //    {
        //        //ejecuta el query                
        //        vlo_Comando = new OracleCommand(pvc_Sql, this.ConexionDB, this.Transaccion);
        //        vlo_Comando.CommandTimeout = 0;
        //        vlo_Comando.ExecuteNonQuery();

        //        //llena el dataset
        //        vlo_Adaptador = new OracleDataAdapter(vlo_Comando);
        //        vlo_Adaptador.Fill(vlo_Datos, pvc_Nombre);

        //        //retorna los parametros
        //        return GenerarEnvioInformacion(true, "", vlo_Datos, vlo_Datos.Tables[pvc_Nombre].Rows.Count);
        //    }
        //    catch (Exception vloError)
        //    {
        //        RealizarRollBack();

        //        //retorna los parametros
        //        return GenerarEnvioInformacion(false, vloError.Message, 0);
        //    }
        //}

        private eResultado LlenarDS_NoTransaccional(String pvc_Sql, String pvc_Nombre)
        {
            eResultado vloResultado = new eResultado();
            DataSet vlo_Datos = new DataSet();
            OracleDataAdapter vlo_Adaptador;

            try
            {
                vloResultado = ConectarBaseDatos();

                if (!vloResultado.Estado)
                {
                    // sino se conecto devuelve el Array con el error
                    return vloResultado;
                }

                vlo_Adaptador = new OracleDataAdapter(pvc_Sql, this.ConexionDB);

                vlo_Adaptador.Fill(vlo_Datos, pvc_Nombre);

                vloResultado = DesconectarBaseDatos();

                if (!vloResultado.Estado)
                {

                    return vloResultado;
                }
                else
                {
                    //retorna los parametros
                    return GenerarEnvioInformacion(true, "", vlo_Datos, vlo_Datos.Tables[pvc_Nombre].Rows.Count);
                }
            }
            catch (Exception vloError)
            {
                DesconectarBaseDatos();

                //retorna los parametros
                return GenerarEnvioInformacion(false, vloError.Message, 0);
            }
        }

        public eResultado LlenarDataSet(OracleCommand pvo_Comando, String pvc_Nombre, Boolean pvb_Seriable = false)
        {
            //verifica que si la trasaccion esta abierta
            if (TransaccionAbierta)
            {
                //llena el data set en una transaccion
                return LlenarDS_Transaccional(pvo_Comando, pvc_Nombre, pvb_Seriable);
            }
            else
            {
                return LlenarDS_NoTransaccional(pvo_Comando, pvc_Nombre, pvb_Seriable);
            }
        }

        private eResultado LlenarDS_Transaccional(OracleCommand pvo_Comando, String pvc_Nombre, Boolean pvb_Seriable)
        {
            DataSet vlo_Datos = new DataSet();
            OracleDataAdapter vlo_Adaptador;

            try
            {
                //ejecuta el query
                pvo_Comando.Connection = this.ConexionDB;
                pvo_Comando.Transaction = this.Transaccion;
                pvo_Comando.CommandTimeout = 0;

                if (pvb_Seriable)
                {
                    pvo_Comando.CommandType = CommandType.StoredProcedure;
                }

                pvo_Comando.ExecuteNonQuery();

                //llena el dataset
                vlo_Adaptador = new OracleDataAdapter(pvo_Comando);
                vlo_Adaptador.Fill(vlo_Datos, pvc_Nombre);

                //retorna los parametros
                return GenerarEnvioInformacion(true, "", vlo_Datos, vlo_Datos.Tables[pvc_Nombre].Rows.Count);
            }
            catch (Exception vloError)
            {
                RealizarRollBack();

                //retorna los parametros
                return GenerarEnvioInformacion(false, vloError.Message, 0);
            }
        }

        private eResultado LlenarDS_NoTransaccional(OracleCommand pvo_Comando, String pvc_Nombre, Boolean pvb_Seriable)
        {
            eResultado vloResultado = new eResultado();
            OracleDataAdapter vlo_Adaptador;
            DataSet vlo_Datos = new DataSet();

            try
            {
                vloResultado = ConectarBaseDatos();

                if (!vloResultado.Estado)
                {
                    // sino se conecto devuelve el Array con el error
                    return vloResultado;
                }

                pvo_Comando.Connection = this.ConexionDB;
                pvo_Comando.CommandTimeout = 0;

                if (pvb_Seriable)
                {
                    pvo_Comando.CommandType = CommandType.StoredProcedure;
                }

                vlo_Adaptador = new OracleDataAdapter(pvo_Comando);

                vlo_Adaptador.Fill(vlo_Datos, pvc_Nombre);

                vloResultado = DesconectarBaseDatos();

                if (!vloResultado.Estado)
                {

                    return vloResultado;
                }
                else
                {
                    //retorna los parametros
                    return GenerarEnvioInformacion(true, "", vlo_Datos, vlo_Datos.Tables[pvc_Nombre].Rows.Count);
                }
            }
            catch (Exception vloError)
            {
                DesconectarBaseDatos();

                //retorna los parametros
                return GenerarEnvioInformacion(false, vloError.Message, 0);
            }
        }

        public eResultado LlenarDataSet(ref DataSet pro_Datos, OracleCommand pvo_Comando, String pvc_Nombre, Boolean pvb_Seriable = false)
        {
            //verifica que si la trasaccion esta abierta
            if (TransaccionAbierta)
            {
                //llena el data set en una transaccion
                return LlenarDS_Transaccional(ref pro_Datos, pvo_Comando, pvc_Nombre, pvb_Seriable);
            }
            else
            {
                return LlenarDS_NoTransaccional(ref pro_Datos, pvo_Comando, pvc_Nombre, pvb_Seriable);
            }
        }

        private eResultado LlenarDS_Transaccional(ref DataSet pro_Datos, OracleCommand pvo_Comando, String pvc_Nombre, Boolean pvb_Seriable)
        {
            OracleDataAdapter vlo_Adaptador;

            try
            {
                //ejecuta el query
                pvo_Comando.Transaction = this.Transaccion;
                pvo_Comando.Connection = this.ConexionDB;
                pvo_Comando.CommandTimeout = 0;

                if (pvb_Seriable)
                {
                    pvo_Comando.CommandType = CommandType.StoredProcedure;
                }

                pvo_Comando.ExecuteNonQuery();

                //llena el dataset
                vlo_Adaptador = new OracleDataAdapter(pvo_Comando);
                vlo_Adaptador.Fill(pro_Datos, pvc_Nombre);

                //retorna los parametros
                return GenerarEnvioInformacion(true, "", pro_Datos.Tables[pvc_Nombre].Rows.Count);
            }
            catch (Exception vloError)
            {
                RealizarRollBack();

                //retorna los parametros
                return GenerarEnvioInformacion(false, vloError.Message, 0);
            }
        }

        private eResultado LlenarDS_NoTransaccional(ref DataSet pro_Datos, OracleCommand pvo_Comando, String pvc_Nombre, Boolean pvb_Seriable)
        {
            eResultado vloResultado = new eResultado();
            OracleDataAdapter vlo_Adaptador;

            try
            {
                vloResultado = ConectarBaseDatos();

                if (!vloResultado.Estado)
                {
                    // sino se conecto devuelve el Array con el error
                    return vloResultado;
                }

                pvo_Comando.Connection = this.ConexionDB;
                pvo_Comando.CommandTimeout = 0;

                if (pvb_Seriable)
                {
                    pvo_Comando.CommandType = CommandType.StoredProcedure;
                }

                vlo_Adaptador = new OracleDataAdapter(pvo_Comando);
                vlo_Adaptador.Fill(pro_Datos, pvc_Nombre);

                vloResultado = DesconectarBaseDatos();

                if (!vloResultado.Estado)
                {

                    return vloResultado;
                }
                else
                {
                    //retorna los parametros
                    return GenerarEnvioInformacion(true, "", pro_Datos.Tables[pvc_Nombre].Rows.Count);
                }
            }
            catch (Exception vloError)
            {
                DesconectarBaseDatos();

                //retorna los parametros
                return GenerarEnvioInformacion(false, vloError.Message, 0);
            }
        }
        #endregion

        #region "GenerarEnvioInformacion"
        private eResultado GenerarEnvioInformacion(Boolean pvb_Estado, String pvc_Mensaje, Int64 pvn_RegistrosAfectados)
        {
            eResultado vloResultado = new eResultado();

            vloResultado.Estado = pvb_Estado;
            vloResultado.RegistrosAfectados = pvn_RegistrosAfectados;
            vloResultado.Mensaje = pvc_Mensaje;

            return vloResultado;
        }

        private eResultado GenerarEnvioInformacion(Boolean pvb_Estado, String pvc_Mensaje, DataSet pvo_Datos, Int64 pvn_RegistrosAfectados)
        {
            eResultado vloResultado = new eResultado();

            vloResultado.Estado = pvb_Estado;
            vloResultado.Mensaje = pvc_Mensaje;
            vloResultado.DatosConsulta = pvo_Datos;
            vloResultado.RegistrosAfectados = pvn_RegistrosAfectados;

            return vloResultado;
        }
        #endregion
    }
}