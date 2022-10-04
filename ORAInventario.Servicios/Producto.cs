using ORAInventario.Entidades;
using ORAInventario.Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORAInventario.Servicios
{
    public class Producto : General_Servicio
    {

        private String vccCodigoCadena;
        private prp_Producto vcoVariables;

        #region Constructores
        public Producto(prp_Producto pvoVariables, String pvcCadenaConexion)
        {
            vccCodigoCadena = pvcCadenaConexion;
            vcoVariables = pvoVariables;
        }

        public Producto(prp_Producto pvoVariables)
        {
            vccCodigoCadena = "";
            vcoVariables = pvoVariables;
        }
        #endregion

        #region Metodos
        public eResultado Guardar()
        {
            Conexion.Conexion vloConexion = new Conexion.Conexion(vccCodigoCadena);
            eResultado vloResultado = new eResultado();

            vloResultado = vloConexion.AbrirTransaccion();

            if (!vloResultado.Estado)
            {
                return vloResultado;
            }

            using (prv_Producto vloClase = new prv_Producto())
                vloResultado = vloClase.Guardar(vcoVariables, ref vloConexion);

            if (!vloResultado.Estado)
            {
                vloConexion.RealizarRollBack();

                return vloResultado;
            }

            vloResultado = vloConexion.CerrarTrasaccion();

            if (!vloResultado.Estado)
            {
                vloConexion.RealizarRollBack();

                return vloResultado;
            }

            return vloResultado;
        }

        public eResultado Modificar()
        {
            Conexion.Conexion vloConexion = new Conexion.Conexion(vccCodigoCadena);
            eResultado vloResultado = new eResultado();

            vloResultado = vloConexion.AbrirTransaccion();

            if (!vloResultado.Estado)
            {
                return vloResultado;
            }

            using (prv_Producto vloClase = new prv_Producto())
                vloResultado = vloClase.Modificar(vcoVariables, ref vloConexion);

            if (!vloResultado.Estado)
            {
                vloConexion.RealizarRollBack();

                return vloResultado;
            }

            vloResultado = vloConexion.CerrarTrasaccion();

            if (!vloResultado.Estado)
            {
                vloConexion.RealizarRollBack();

                return vloResultado;
            }

            return vloResultado;
        }

        public eResultado Eliminar()
        {
            Conexion.Conexion vloConexion = new Conexion.Conexion(vccCodigoCadena);
            eResultado vloResultado = new eResultado();

            vloResultado = vloConexion.AbrirTransaccion();

            if (!vloResultado.Estado)
            {
                return vloResultado;
            }

            using (prv_Producto vloClase = new prv_Producto())
                vloResultado = vloClase.Eliminar(vcoVariables, ref vloConexion);

            if (!vloResultado.Estado)
            {
                vloConexion.RealizarRollBack();

                return vloResultado;
            }

            vloResultado = vloConexion.CerrarTrasaccion();

            if (!vloResultado.Estado)
            {
                vloConexion.RealizarRollBack();

                return vloResultado;
            }

            return vloResultado;
        }

        public eResultado ObtenerLista()
        {
            Conexion.Conexion vloConexion = new Conexion.Conexion(vccCodigoCadena);

            using (prv_Producto vloClase = new prv_Producto())
                return vloClase.ObtenerLista(vcoVariables, ref vloConexion, vcnTamanoLista);
        }

        public eResultado ObtenerLista_Cantidad()
        {
            Conexion.Conexion vloConexion = new Conexion.Conexion(vccCodigoCadena);

            using (prv_Producto vloClase = new prv_Producto())
                return vloClase.ObtenerLista_Cantidad(vcoVariables, ref vloConexion, vcnTamanoLista);
        }

        public eResultado CargarPropiedades()
        {
            Conexion.Conexion vloConexion = new Conexion.Conexion(vccCodigoCadena);

            using (prv_Producto vloClase = new prv_Producto())
                return vloClase.CargarPropiedades(vcoVariables, ref vloConexion);
        }

        public eResultado Max()
        {
            Conexion.Conexion vloConexion = new Conexion.Conexion(vccCodigoCadena);

            using (prv_Producto vloClase = new prv_Producto())
                return vloClase.Max(ref vloConexion);
        }

        #endregion
    }
}