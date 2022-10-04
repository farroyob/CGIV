using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace ORAInventario.Entidades
{
    public struct eResultado
    {
        #region Variables
        //true sin errores y false con errores
        public Boolean Estado { get; set; }
        //mensaje de respuesta
        public String Mensaje { get; set; }
        //Retorna un DataSet
        public DataSet DatosConsulta { get; set; }
        //Retorna un Arreglo
        public ArrayList Arreglo { get; set; }
        //Retorna un Objeto
        public Object Valor { get; set; }
        //Retorna el Numero de Registros Afectados en Un Update o Insert
        public Int64 RegistrosAfectados { get; set; }
        //mensaje de respuesta
        public String JsonString { get; set; }
        #endregion     
    }
}