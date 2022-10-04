using ORAInventario.Entidades;
using ORAInventario.Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ORAInventario
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();

            Inicializar();
        }

        private void Inicializar()
        {
            DataSet vloDatosConfiguracion;
            String vlcArchivo;
            String vlcArchivo_Estilo;
            String vlcArchivo_Imagen;
            String vlcServidor;
            String vlcUsuario;
            String vlcClave;

            vlcArchivo = @Application.StartupPath + "\\" + "Config.xml";


            if (System.IO.File.Exists(vlcArchivo))
            {
                try
                {
                    vloDatosConfiguracion = new DataSet();
                    vloDatosConfiguracion.ReadXml(vlcArchivo);
                }
                catch
                {
                    System.IO.File.Delete(vlcArchivo);

                    CrearArchivoConfiguracion();

                    vloDatosConfiguracion = new DataSet();
                    vloDatosConfiguracion.ReadXml(vlcArchivo);
                }
            }
            else
            {
                CrearArchivoConfiguracion();

                vloDatosConfiguracion = new DataSet();
                vloDatosConfiguracion.ReadXml(vlcArchivo);
            }

            vlcArchivo_Estilo = Convert.ToString(vloDatosConfiguracion.Tables["Datos"].Rows[0]["Estilo_Ubicacion"]) + Convert.ToString(vloDatosConfiguracion.Tables["Datos"].Rows[0]["Estilo"]);

            if (System.IO.File.Exists(vlcArchivo_Estilo))
            {
                Infragistics.Win.AppStyling.StyleManager.Load(vlcArchivo_Estilo);
            }

            lblTitulo.Text = Convert.ToString(vloDatosConfiguracion.Tables["Datos"].Rows[0]["Login_Titulo"]);

            vlcServidor = Convert.ToString(vloDatosConfiguracion.Tables["Datos"].Rows[0]["DBServidor"]);
            vlcUsuario = Convert.ToString(vloDatosConfiguracion.Tables["Datos"].Rows[0]["DBUsuario"]);
            vlcClave = Convert.ToString(vloDatosConfiguracion.Tables["Datos"].Rows[0]["DBClave"]);

            Properties.Settings.Default.CadenaConexion = "Data Source=" + vlcServidor + ";User Id=" + vlcUsuario + ";Password=" + vlcClave + ";";

            LlenarCombos();

            if (cboSucursal.Items.Count != 0 && Convert.ToInt32(vloDatosConfiguracion.Tables["Datos"].Rows[0]["Ultimo_Sucursal"]) != 0)
            {
                cboSucursal.Value = Convert.ToInt32(vloDatosConfiguracion.Tables["Datos"].Rows[0]["Ultimo_Sucursal"]);
            }

            vlcArchivo_Imagen = Convert.ToString(vloDatosConfiguracion.Tables["Datos"].Rows[0]["Login_Imagen"]);

            if (System.IO.File.Exists(vlcArchivo_Imagen))
            {
                ptbPrincipal.Image = Image.FromFile(vlcArchivo_Imagen);
            }
            else
            {
                ptbPrincipal.Image = Properties.Resources.login;
            }

            if (Convert.ToInt32(vloDatosConfiguracion.Tables["Datos"].Rows[0]["Ultimo_Usuario"]) != 0)
            {
                ObtenerUltimoUsuario(Convert.ToInt32(vloDatosConfiguracion.Tables["Datos"].Rows[0]["Ultimo_Usuario"]));
            }
        }

        private void ObtenerUltimoUsuario(Int32 pvnId)
        {
            eResultado vloResultado;
            prp_Usuario vloVariables;

            vloVariables = new prp_Usuario();
            vloVariables.Filtro_Tipo = 0;
            vloVariables.Id = pvnId;

            using (Usuario vloClase = new Usuario(vloVariables, Properties.Settings.Default.CadenaConexion))
                vloResultado = vloClase.CargarPropiedades();

            if (!vloResultado.Estado)
            {
                txtUsuario.Focus();

                return;
            }

            vloVariables = (prp_Usuario)vloResultado.Valor;

            txtUsuario.Text = vloVariables.Login;

            txtClave.Focus();
        }

        #region LlenarCombos
        private void LlenarCombos()
        {
            eResultado vloResultado = new eResultado();
            prp_Sucursal vloVariables = new prp_Sucursal();

            using (Sucursal vloClase = new Sucursal(vloVariables, Properties.Settings.Default.CadenaConexion))
                vloResultado = vloClase.ObtenerCombo();

            if (vloResultado.Estado)
            {
                cboSucursal.DataSource = vloResultado.DatosConsulta.Tables["Datos"].Copy();
                cboSucursal.ValueMember = "Codigo";
                cboSucursal.DisplayMember = "Nombre";
                cboSucursal.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Error al Obtener los valores de la sucursales, " + vloResultado.Mensaje, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region CrearArchivoConfiguracion
        private void CrearArchivoConfiguracion()
        {
            DataSet vloDatos = new DataSet("Configuracion");
            DataTable vloTablaDefault = new DataTable("Datos");
            String vlcArchivo;

            vlcArchivo = @Application.StartupPath + "\\" + "Config.xml";

            vloTablaDefault.Columns.Add("DBServidor");
            vloTablaDefault.Columns.Add("DBUsuario");
            vloTablaDefault.Columns.Add("DBClave");
            vloTablaDefault.Columns.Add("Login_Titulo");
            vloTablaDefault.Columns.Add("Login_Imagen");
            vloTablaDefault.Columns.Add("Estilo");
            vloTablaDefault.Columns.Add("Estilo_Ubicacion");
            vloTablaDefault.Columns.Add("Ultimo_Usuario", Type.GetType("System.Int32"));
            vloTablaDefault.Columns.Add("Ultimo_Sucursal", Type.GetType("System.Int32"));

            vloTablaDefault.Rows.Add();

            vloTablaDefault.Rows[0]["DBServidor"] = String.Empty;
            vloTablaDefault.Rows[0]["DBUsuario"] = String.Empty;
            vloTablaDefault.Rows[0]["DBClave"] = String.Empty;
            vloTablaDefault.Rows[0]["Login_Titulo"] = "Login / ORA Inventario";
            vloTablaDefault.Rows[0]["Login_Imagen"] = String.Empty;
            vloTablaDefault.Rows[0]["Estilo"] = "Nautilus.isl";
            vloTablaDefault.Rows[0]["Estilo_Ubicacion"] = @Application.StartupPath + "\\SistemaEstilo2017\\";
            vloTablaDefault.Rows[0]["Ultimo_Usuario"] = 0;
            vloTablaDefault.Rows[0]["Ultimo_Sucursal"] = 0;

            vloDatos.Tables.Add(vloTablaDefault);

            vloDatos.WriteXml(vlcArchivo);
        }
        #endregion

        private void txtGeneral_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void FrmLogin_Activated(object sender, EventArgs e)
        {
            if (txtUsuario.Text.Trim() == "")
            {
                txtUsuario.Focus();
            }
            else
            {
                txtClave.Focus();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private Boolean Validar()
        {
            if (txtUsuario.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el usuario" + Environment.NewLine + "antes de continuar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }

            if (txtClave.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese la clave" + Environment.NewLine + "antes de continuar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }

            if (cboSucursal.SelectedIndex == -1)
            {
                MessageBox.Show("NO existe una sucursal seleccionada" + Environment.NewLine + "revisar antes de continuar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }

            return true;
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            if (!Validar())
            {
                return;
            }

            eResultado vloResultado;
            prp_Usuario vloVariables;

            vloVariables = new prp_Usuario();
            vloVariables.Filtro_Tipo = 1;
            vloVariables.Login = txtUsuario.Text.Trim();
            vloVariables.Clave = txtClave.Text.Trim();

            using (Usuario vloClase = new Usuario(vloVariables, Properties.Settings.Default.CadenaConexion))
                vloResultado = vloClase.CargarPropiedades();

            if (!vloResultado.Estado)
            {
                MessageBox.Show("Error al Obtener la información del usuario, " + vloResultado.Mensaje, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            vloVariables = (prp_Usuario)vloResultado.Valor;

            DataSet vloDatosConfiguracion;
            String vlcArchivo;

            vlcArchivo = Application.StartupPath + "\\" + "Config.xml";

            vloDatosConfiguracion = new DataSet();
            vloDatosConfiguracion.ReadXml(vlcArchivo);

            vloDatosConfiguracion.Tables[0].Rows[0]["Ultimo_Usuario"] = vloVariables.Id;
            vloDatosConfiguracion.Tables[0].Rows[0]["Ultimo_Sucursal"] = Convert.ToInt32(cboSucursal.Value);

            vloDatosConfiguracion.WriteXml(vlcArchivo);            

            Properties.Settings.Default.Ingreso = true;
            Properties.Settings.Default.Usuario = vloVariables.Id;
            Properties.Settings.Default.Sucursal = Convert.ToInt32(cboSucursal.Value);

            this.Close();
        }

        private void btnConfigurar_Click(object sender, EventArgs e)
        {
            FrmConfig vloConfig = new FrmConfig();
            vloConfig.ShowDialog();

            if(vloConfig.Modifico)
            {
                Inicializar();
            }

        }
    }
}
