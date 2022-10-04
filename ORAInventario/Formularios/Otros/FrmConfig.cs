using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ORAInventario
{
    public partial class FrmConfig : Form
    {
        public Boolean Modifico { get; set; }

        public FrmConfig()
        {
            InitializeComponent();

            try { Inicializar(); }
            catch { }
        }

        private void General_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void Inicializar()
        {
            DataSet vloDatosConfiguracion;
            String vlcArchivo;
            
            vlcArchivo = @Application.StartupPath + "\\" + "Config.xml";

            vloDatosConfiguracion = new DataSet();
            vloDatosConfiguracion.ReadXml(vlcArchivo);

            txtServidor.Text = Convert.ToString(vloDatosConfiguracion.Tables["Datos"].Rows[0]["DBServidor"]);
            txtUsuario.Text = Convert.ToString(vloDatosConfiguracion.Tables["Datos"].Rows[0]["DBUsuario"]);
            txtClave.Text = Convert.ToString(vloDatosConfiguracion.Tables["Datos"].Rows[0]["DBClave"]);

            txtTitulo.Text = Convert.ToString(vloDatosConfiguracion.Tables["Datos"].Rows[0]["Login_Titulo"]);
            txtDirImagen.Text = Convert.ToString(vloDatosConfiguracion.Tables["Datos"].Rows[0]["Login_Imagen"]);

            txtDirEstilo.Text = Convert.ToString(vloDatosConfiguracion.Tables["Datos"].Rows[0]["Estilo_Ubicacion"]);

            LlenarCombo();

            cboEstilo.Value = Convert.ToString(vloDatosConfiguracion.Tables["Datos"].Rows[0]["Estilo"]);

            CambiarEstilo();
        }

        private void LlenarCombo()
        {
            System.IO.FileInfo[] vloArchivos = null;
            DataTable vloEstilos;
            System.IO.DirectoryInfo vloDirRaiz;

            vloEstilos = new DataTable("Estilos");
            vloEstilos.Columns.Add("Codigo");
            vloEstilos.Columns.Add("Nombre");

            if (!System.IO.Directory.Exists(txtDirEstilo.Text))
            {
                cboEstilo.DataSource = vloEstilos.Copy();
                cboEstilo.ValueMember = "Codigo";
                cboEstilo.DisplayMember = "Nombre";

                return;
            }

            vloDirRaiz = new DirectoryInfo(txtDirEstilo.Text);

            vloArchivos = vloDirRaiz.GetFiles("*.*");

            foreach (FileInfo vloFila in vloArchivos)
            {
                if (vloFila.Name.ToUpper().Contains(".ISL"))
                {
                    vloEstilos.Rows.Add(vloFila.Name, vloFila.Name);
                }
            }

            cboEstilo.DataSource = vloEstilos.Copy();
            cboEstilo.ValueMember = "Codigo";
            cboEstilo.DisplayMember = "Nombre";
            cboEstilo.SelectedIndex = 0;
        }

        private void utm_Principal_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "tlb_Guardar":
                    MetodoModificar();
                    break;

                case "tlb_Salir":
                    MetodoSalir();
                    break;
            }
        }

        private Boolean Validar()
        {
            if (txtServidor.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese la dirección del servidor" + Environment.NewLine + "revisar antes de continuar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }

            if (txtUsuario.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese la información del usuario" + Environment.NewLine + "revisar antes de continuar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }

            if (txtClave.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese la información de la clave" + Environment.NewLine + "revisar antes de continuar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }

            if (txtTitulo.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese la información del título en la pantalla del login" + Environment.NewLine + "revisar antes de continuar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }

            if (cboEstilo.SelectedIndex == -1)
            {
                MessageBox.Show("La dirección de los estilos es incorrecta" + Environment.NewLine + "revisar antes de continuar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }

            return true;
        }

        private void MetodoModificar()
        {
            if (!Validar())
            {
                return;
            }

            DataSet vloDatosConfiguracion;
            String vlcArchivo;

            vlcArchivo = Application.StartupPath + "\\" + "Config.xml";

            vloDatosConfiguracion = new DataSet();
            vloDatosConfiguracion.ReadXml(vlcArchivo);
            
            vloDatosConfiguracion.Tables[0].Rows[0]["DBServidor"] = txtServidor.Text.Trim();
            vloDatosConfiguracion.Tables[0].Rows[0]["DBUsuario"] = txtUsuario.Text.Trim();
            vloDatosConfiguracion.Tables[0].Rows[0]["DBClave"] = txtClave.Text.Trim();
            vloDatosConfiguracion.Tables[0].Rows[0]["Login_Titulo"] = txtTitulo.Text.Trim();
            vloDatosConfiguracion.Tables[0].Rows[0]["Login_Imagen"] = txtDirImagen.Text.Trim();
            vloDatosConfiguracion.Tables[0].Rows[0]["Estilo"] = Convert.ToString (cboEstilo.Value);
            vloDatosConfiguracion.Tables[0].Rows[0]["Estilo_Ubicacion"] = txtDirEstilo.Text;

            vloDatosConfiguracion.WriteXml(vlcArchivo);

            this.Close();
        }

        private void MetodoSalir()
        {
            this.Close();
        }

        private void cboEstilo_SelectionChangeCommitted(object sender, EventArgs e)
        {
            CambiarEstilo();
        }

        private void CambiarEstilo()
        {
            String vlcArchivo_Estilo;

            if (cboEstilo.SelectedIndex == -1)
            {
                return;
            }

            vlcArchivo_Estilo = txtDirEstilo.Text + Convert.ToString(cboEstilo.Value);

            if (System.IO.File.Exists(vlcArchivo_Estilo))
            {
                Infragistics.Win.AppStyling.StyleManager.Load(vlcArchivo_Estilo);
            }
        }

        private void txtDirEstilo_Leave(object sender, EventArgs e)
        {
            LlenarCombo();
        }

        private void txtDirEstilo_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {
                txtDirEstilo.Focus();

                fbd_ruta.ShowDialog();

                if (fbd_ruta.SelectedPath.Length > 0)
                {
                    txtDirEstilo.Text = fbd_ruta.SelectedPath + "\\";

                    LlenarCombo();
                    CambiarEstilo();

                    cboEstilo.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtDirImagen_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            fImagen.InitialDirectory = "c:\\";
            fImagen.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            fImagen.FilterIndex = 2;
            fImagen.RestoreDirectory = true;

            if (fImagen.ShowDialog() == DialogResult.OK)
            {
                txtDirImagen.Text = fImagen.FileName;
            }
        }
    }
}