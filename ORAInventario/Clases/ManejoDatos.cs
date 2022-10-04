using Infragistics.Win.FormattedLinkLabel;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using System;
using System.Windows.Forms;

namespace ORAInventario
{
    public static class ManejoDatos
    {
        #region EscondeTabs
        /// <summary>
        /// oculta todos los tabs menos el primero
        /// </summary>
        /// <param name="tab"></param>
        public static void EscondeTabs(UltraTabControl tab)
        {
            try
            {
                for (int i = 0; i <= tab.Tabs.Count - 1; i++)
                {
                    if (i != 0)
                    {
                        tab.Tabs[i].Enabled = false;
                    }
                }
                tab.SelectedTab = tab.Tabs[0];
            }
            catch { }
        }
        #endregion

        #region MuestraTabs
        /// <summary>
        /// muestra todos los tabs y selecciona el segundo
        /// </summary>
        /// <param name="tab"></param>
        public static void MuestraTabs(UltraTabControl tab)
        {
            for (int i = 0; i <= tab.Tabs.Count - 1; i++)
            {

                tab.Tabs[i].Enabled = true;

            }
            tab.SelectedTab = tab.Tabs[1];

        }
        #endregion

        #region LimpiarControles
        /// <summary>
        /// Limpia los Controles De Un Grupo Tal Como tap, group box 
        /// </summary>
        /// <param name="objeto"></param>
        public static void LimpiarControles(Control objeto)
        {
            foreach (Control objetoEvaluado in objeto.Controls)
            {
                if ((objetoEvaluado is UltraGroupBox) || (objetoEvaluado is TabControl) || (objetoEvaluado is Panel) || (objetoEvaluado is TabPage) || (objetoEvaluado is UltraTabControl) || (objetoEvaluado is UltraTabPageControl) || (objetoEvaluado is UltraFormattedLinkLabel) || (objetoEvaluado is UltraExpandableGroupBoxPanel) || (objetoEvaluado is UltraExpandableGroupBox))
                {
                    LimpiarControles(objetoEvaluado);
                }
                else
                {
                    if ((objetoEvaluado is UltraTextEditor) || (objetoEvaluado is MaskedTextBox) || (objetoEvaluado is TextBox))
                        objetoEvaluado.Text = "";
                    else
                    {
                        if (objetoEvaluado is UltraCurrencyEditor)
                            ((UltraCurrencyEditor)objetoEvaluado).Value = 0;
                        else
                        {
                            if (objetoEvaluado is ComboBox)
                                ((ComboBox)objetoEvaluado).SelectedIndex = -1;
                            else
                            {
                                if (objetoEvaluado is UltraCombo)
                                    ((UltraCombo)objetoEvaluado).SelectedRow = null;
                                else
                                {
                                    if (objetoEvaluado is DateTimePicker)
                                        ((DateTimePicker)objetoEvaluado).Value = DateTime.Now;
                                    else
                                    {
                                        if (objetoEvaluado is DataGridView)
                                            ((DataGridView)objetoEvaluado).DataSource = null;
                                        else
                                        {
                                            if (objetoEvaluado is PictureBox)
                                                ((PictureBox)objetoEvaluado).Image = null;
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
