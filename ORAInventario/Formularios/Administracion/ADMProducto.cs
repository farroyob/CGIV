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
    public partial class ADMProducto : Form
    {
        private Int32 vcnPaginaIndex = 1;
        private Int32 vcnPaginaIndexTotal = 0;
        private Int32 vcnPaginaTotal = 0;
        private Int32 vcnCantidadItemsxPagina = 100;

        public ADMProducto()
        {
            InitializeComponent();
        }

        private void ADMProducto_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;

            Inicializar();

            LlenarCombos();
        }

        #region LlenarCombos
        private void LlenarCombos()
        {
            eResultado vloResultado;
            prp_Sucursal vloPrp_Sucursal;
            prp_Usuario vloPrp_Usuario;
            prp_Producto_Categoria vloPrp_Categoria;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // SUCURSAl
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            vloPrp_Sucursal = new prp_Sucursal();

            using (Sucursal vloClase = new Sucursal(vloPrp_Sucursal, Properties.Settings.Default.CadenaConexion))
                vloResultado = vloClase.ObtenerCombo();

            if (vloResultado.Estado)
            {
                cboSucursal.DataSource = vloResultado.DatosConsulta.Tables["Datos"].Copy();
                cboSucursal.ValueMember = "Codigo";
                cboSucursal.DisplayMember = "Nombre";
                cboSucursal.SelectedIndex = 0;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // USUARIO
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            vloPrp_Usuario = new prp_Usuario();

            using (Usuario vloClase = new Usuario(vloPrp_Usuario, Properties.Settings.Default.CadenaConexion))
                vloResultado = vloClase.ObtenerCombo();

            if (vloResultado.Estado)
            {
                cboUsuario.DataSource = vloResultado.DatosConsulta.Tables["Datos"].Copy();
                cboUsuario.ValueMember = "Codigo";
                cboUsuario.DisplayMember = "Nombre";
                cboUsuario.SelectedIndex = 0;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // CATEGORIA
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            vloPrp_Categoria = new prp_Producto_Categoria();

            using (Producto_Categoria vloClase = new Producto_Categoria(vloPrp_Categoria, Properties.Settings.Default.CadenaConexion))
                vloResultado = vloClase.ObtenerCombo();

            if (vloResultado.Estado)
            {
                cboCategoria.DataSource = vloResultado.DatosConsulta.Tables["Datos"].Copy();
                cboCategoria.ValueMember = "Codigo";
                cboCategoria.DisplayMember = "Nombre";
                cboCategoria.SelectedIndex = 0;
            }
        }
        #endregion

        private void Inicializar()
        {
            utm_Principal.Toolbars["utb_Principal"].Tools["tlb_Modificar"].SharedProps.Visible = false;
            utm_Principal.Toolbars["utb_Principal"].Tools["tlb_Guardar"].SharedProps.Visible = false;
            utm_Principal.Toolbars["utb_Principal"].Tools["tlb_Cancelar"].SharedProps.Visible = false;
            utm_Principal.Toolbars["utb_Principal"].Tools["tlb_Eliminar"].SharedProps.Visible = false;
        }

        private void utm_Principal_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "tlb_Nuevo":
                    Nuevo();
                    break;

                case "tlb_Guardar":
                    Grabar(true);
                    break;

                case "tlb_Modificar":
                    Grabar(false);
                    break;

                case "tlb_Eliminar":
                    Borrar();
                    break;

                case "tlb_Cancelar":
                    if (MessageBox.Show("¿Esta seguro que desea cancelar la edición? Se perderán todos los datos que no hayan sido guardados.", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        Cancelar();
                    }
                    break;

                case "tlb_Salir":
                    this.Close();
                    break;
            }
        }

        private void TabPrincipal_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
        {
            if (e.Tab.Text == "Lista")
            {
                Cancelar();
            }
            else
            {
                utm_Principal.Toolbars["utb_Principal"].Tools["tlb_Cancelar"].SharedProps.Visible = true;
            }

            if (e.Tab.Text == "Detalle")
            {
                txtNombre.Focus();
            }
        }

        #region Cancelar
        //********************* [ Cancelar ] ********************************************//
        private void Cancelar()
        {
            TabPrincipal.Tabs[0].Text = "";
            ManejoDatos.EscondeTabs(TabPrincipal);
            TabPrincipal.Tabs[0].Text = "Lista";

            Limpiar();

            utm_Principal.Toolbars["utb_Principal"].Tools["tlb_Modificar"].SharedProps.Visible = false;
            utm_Principal.Toolbars["utb_Principal"].Tools["tlb_Guardar"].SharedProps.Visible = false;
            utm_Principal.Toolbars["utb_Principal"].Tools["tlb_Cancelar"].SharedProps.Visible = false;
            utm_Principal.Toolbars["utb_Principal"].Tools["tlb_Nuevo"].SharedProps.Visible = true;
            utm_Principal.Toolbars["utb_Principal"].Tools["tlb_Eliminar"].SharedProps.Visible = false;

            LlenarGrid();
        }
        #endregion

        #region Limpiar
        //********************* [ Limpiar ] ********************************************//
        private void Limpiar()
        {
            prp_Producto vloVariables = new prp_Producto();
            eResultado vloResultado;

            using (Producto vloClase = new Producto(vloVariables, Properties.Settings.Default.CadenaConexion))
                vloResultado = vloClase.Max();

            if (vloResultado.Estado)
            {
                txtId.Value = vloResultado.Valor;
            }
            else
            {
                MessageBox.Show("Error al consultar el Consecutivo, " + vloResultado.Mensaje, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            lblUsuario.Visible = false;
            cboUsuario.Visible = false;
            cboUsuario.SelectedIndex = 0;

            lblSucursal.Visible = false;
            cboSucursal.Visible = false;
            cboSucursal.SelectedIndex = 0;

            txtNombre.Text = String.Empty;
            cboCategoria.SelectedIndex = 0;
            txtUnidades.Value = 0;
            txtPrecio.Value = 0;

            cboCategoria.ReadOnly = false;

            txtNombre.Focus();
        }
        #endregion

        #region Nuevo
        //********************* [ Nuevo ] ********************************************//
        private void Nuevo()
        {
            utm_Principal.Toolbars["utb_Principal"].Tools["tlb_Modificar"].SharedProps.Visible = false;
            utm_Principal.Toolbars["utb_Principal"].Tools["tlb_Guardar"].SharedProps.Visible = true;
            utm_Principal.Toolbars["utb_Principal"].Tools["tlb_Nuevo"].SharedProps.Visible = true;
            utm_Principal.Toolbars["utb_Principal"].Tools["tlb_Cancelar"].SharedProps.Visible = true;
            utm_Principal.Toolbars["utb_Principal"].Tools["tlb_Eliminar"].SharedProps.Visible = false;

            Limpiar();

            ManejoDatos.MuestraTabs(TabPrincipal);
        }
        #endregion

        //********************* [ LlenaGrid ] ********************************************//
        public void LlenarGrid()
        {
            eResultado vloResultado = new eResultado();
            prp_Producto vloVariables = new prp_Producto();

            vloVariables.Index_Paginacion = vcnPaginaIndex;
            vloVariables.Filtro = txtFiltro.Text.Trim();

            using (Producto vloClase = new Producto(vloVariables, Properties.Settings.Default.CadenaConexion))
                vloResultado = vloClase.ObtenerLista_Cantidad();

            if (!vloResultado.Estado)
            {
                MessageBox.Show(vloResultado.Mensaje, "Cantidad Registros");

                return;
            }

            vcnPaginaTotal = Convert.ToInt32(vloResultado.Valor);

            ds_Adm_Producto.Tables["Datos"].Rows.Clear();

            using (Producto vloClase = new Producto(vloVariables, Properties.Settings.Default.CadenaConexion))
                vloResultado = vloClase.ObtenerLista();

            if (vloResultado.Estado)
            {
                ds_Adm_Producto.Tables["Datos"].Merge(vloResultado.DatosConsulta.Tables["Datos"].Copy());

                Calcular_Paginacion();
            }
            else
            {
                MessageBox.Show(vloResultado.Mensaje);
            }
        }

        private void Calcular_Paginacion()
        {
            Int32 vln_Inicio = 0;
            Int32 vln_Final = 0;

            try
            {
                vln_Inicio = (vcnCantidadItemsxPagina * (vcnPaginaIndex - 1));
                vln_Final = (vcnCantidadItemsxPagina * (vcnPaginaIndex));

                if (vln_Final > vcnPaginaTotal)
                {
                    vln_Final = vcnPaginaTotal;
                }

                if (vln_Inicio == 0 && vcnPaginaTotal != 0)
                {
                    vln_Inicio = 1;
                }

                lblPagina.Text = vln_Inicio.ToString() + "-" + vln_Final.ToString() + " de " + vcnPaginaTotal.ToString();

                vcnPaginaIndexTotal = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(vcnPaginaTotal) / Convert.ToDecimal(vcnCantidadItemsxPagina)));

                //Si la cantidad de registros son la misma que la paginacion se enabilita los siguientes y los primeros
                if (vln_Final == vcnPaginaTotal)
                {
                    btnFinal.Enabled = false;
                    btnSiguiente.Enabled = false;
                    btnAtras.Enabled = false;
                    btnInicio.Enabled = false;
                }

                //Si la paginacion es mayor que el total se enabilita los siguientes
                if (vln_Final >= vcnPaginaTotal)
                {
                    btnFinal.Enabled = false;
                    btnSiguiente.Enabled = false;
                }
                else
                {
                    btnFinal.Enabled = true;
                    btnSiguiente.Enabled = true;
                }

                if (vcnPaginaIndex == 1)
                {
                    btnAtras.Enabled = false;
                    btnInicio.Enabled = false;
                }
                else
                {
                    btnAtras.Enabled = true;
                    btnInicio.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error paginación, " + ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LlenarCampos(Int32 pvnCodigo)
        {
            prp_Producto vloVariables;
            eResultado vloResultado;

            try
            {
                vloVariables = new prp_Producto();
                vloVariables.Id = pvnCodigo;

                using (Producto vloClase = new Producto(vloVariables, Properties.Settings.Default.CadenaConexion))
                    vloResultado = vloClase.CargarPropiedades();

                if (!vloResultado.Estado)
                {
                    MessageBox.Show(vloResultado.Mensaje, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                vloVariables = (prp_Producto)vloResultado.Valor;

                txtId.Text = Convert.ToString(vloVariables.Id);
                txtNombre.Text = vloVariables.Nombre;
                cboCategoria.Value = vloVariables.Categoria;
                txtPrecio.Value = vloVariables.Precio;
                txtUnidades.Value = vloVariables.Cantidad;
                cboSucursal.Value = vloVariables.Sucursal;
                cboUsuario.Value = vloVariables.Usuario_Registra;

                cboCategoria.ReadOnly = true;

                lblUsuario.Visible = true;
                cboUsuario.Visible = true;

                lblSucursal.Visible = true;
                cboSucursal.Visible = true;

                vcnPaginaIndex = 1;

                txtNombre.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando datos LOG: " + ex.Message, "Error");
            }
        }

        //********************* [ Grabar ] ********************************************//
        private bool Validar()
        {
            if (txtNombre.Text.Trim() == "")
            {
                MessageBox.Show("NO se ha registrado el nombre, revisar antes de continuar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                txtNombre.Focus();

                return false;
            }

            if (Convert.ToDecimal(txtUnidades.Value) == 0)
            {
                MessageBox.Show("No se ha Incluido la Cantidad, favor revisar antes de continuar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                txtUnidades.Focus();

                return false;
            }

            if (Convert.ToDecimal(txtUnidades.Value) == 0)
            {
                MessageBox.Show("No se ha Incluido el precio, favor revisar antes de continuar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                txtUnidades.Focus();

                return false;
            }

            if (cboCategoria.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione la categoría antes de Continuar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                cboCategoria.Focus();

                return false;
            }

            return true;
        }

        private void Grabar(bool pvb_Guardar)
        {
            eResultado vloResultado;
            prp_Producto vloVariables;

            if (!Validar())
            {
                return;
            }
            
            vloVariables = new prp_Producto();
            vloVariables.Id = Convert.ToInt32(txtId.Text);
            vloVariables.Nombre = txtNombre.Text.Trim();
            vloVariables.Categoria = Convert.ToInt32(cboCategoria.Value);
            vloVariables.Cantidad = Convert.ToInt32(txtUnidades.Value);
            vloVariables.Precio = Convert.ToDecimal(txtPrecio.Value);
            vloVariables.Usuario_Registra = Properties.Settings.Default.Usuario;
            vloVariables.Sucursal = Properties.Settings.Default.Sucursal;

            using (Producto vloClase = new Producto(vloVariables, Properties.Settings.Default.CadenaConexion))
            {
                if (pvb_Guardar)
                {
                    vloResultado = vloClase.Guardar();
                }
                else
                {
                    vloResultado = vloClase.Modificar();
                }
            }

            if (vloResultado.Estado)
            {
                MessageBox.Show("La operación fue realizada con Éxito!");

                Cancelar();
            }
            else
            {
                MessageBox.Show("Error en la operación, " + vloResultado.Mensaje, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool Validar_Borrar()
        {
            return true;
        }

        private void Borrar()
        {
            prp_Producto vloVariables;
            eResultado vloResultado;

            if (!Validar_Borrar())
            {
                return;
            }

            vloVariables = new prp_Producto();
            vloVariables.Id = Convert.ToInt32(txtId.Text);

            using (Producto vloClase = new Producto(vloVariables, Properties.Settings.Default.CadenaConexion))
                vloResultado = vloClase.Eliminar();

            if (!vloResultado.Estado)
            {
                MessageBox.Show(vloResultado.Mensaje);
            }

            LlenarGrid();

            ManejoDatos.EscondeTabs(TabPrincipal);

            TabPrincipal.Tabs[0].Text = "Lista";
        }

        private void grdDatos_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            try
            {
                if (e.Row.Index == -1)
                {
                    return;
                }

                LlenarCampos(Convert.ToInt32(e.Row.Cells["ID"].Value));

                ManejoDatos.MuestraTabs(TabPrincipal);

                utm_Principal.Toolbars["utb_Principal"].Tools["tlb_Modificar"].SharedProps.Visible = true;
                utm_Principal.Toolbars["utb_Principal"].Tools["tlb_Guardar"].SharedProps.Visible = false;
                utm_Principal.Toolbars["utb_Principal"].Tools["tlb_Nuevo"].SharedProps.Visible = true;
                utm_Principal.Toolbars["utb_Principal"].Tools["tlb_Cancelar"].SharedProps.Visible = true;
                utm_Principal.Toolbars["utb_Principal"].Tools["tlb_Eliminar"].SharedProps.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando datos LOG: " + ex.Message, "Error");

                return;
            }
        }

        private void txtFiltro_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                vcnPaginaIndex = 1;

                LlenarGrid();
            }
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            vcnPaginaIndex += 1;

            LlenarGrid();
        }

        private void btnFinal_Click(object sender, EventArgs e)
        {
            vcnPaginaIndex = vcnPaginaIndexTotal;

            if (vcnPaginaIndex == 0)
            {
                vcnPaginaTotal = 1;
            }

            LlenarGrid();
        }

        private void btnAtras_Click(object sender, EventArgs e)
        {
            if (vcnPaginaIndex != 1)
            {
                vcnPaginaIndex -= 1;

                LlenarGrid();
            }
        }

        private void btnInicio_Click(object sender, EventArgs e)
        {
            vcnPaginaIndex = 1;

            LlenarGrid();
        }

        private void ADMProducto_Activated(object sender, EventArgs e)
        {
            txtFiltro.Focus();
        }

        private void cboFiltro_SelectionChangeCommitted(object sender, EventArgs e)
        {
            vcnPaginaIndex = 1;

            LlenarGrid();
        }

        private void General_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                SendKeys.Send("{TAB}");
            }
        }
    }
}
