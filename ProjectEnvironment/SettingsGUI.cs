using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSKPrim.PanelTools.ProjectEnvironment
{
    public partial class SettingsGUI : Form
    {
        private static Settings Settings = Settings.GetSettings();
        private SelectionType temp_SelectionType = Settings.GetSelectionType();
        private int temp_TileWidth = Settings.GetTileModule().ModuleWidth;
        private int temp_TileHeight = Settings.GetTileModule().ModuleHeight;
        private int temp_TileGap = Settings.GetTileModule().ModuleGap;

        internal Result settingsResult;

        public SettingsGUI()
        {
            InitializeComponent();
        }

        internal void SetSettings()
        {
            if (settingsResult != Result.Succeeded )
            {

            }
        }

        private void SelectAllButton_CheckedChanged(object sender, EventArgs e)
        {
            if (SelectAllButton.Checked == true)
            {
                temp_SelectionType = SelectionType.AllElements;
                CropBoxButton.Checked = false;
                SelectMultipleButton.Checked = false;
                SelectSingleButton.Checked = false;
            }
        }

        private void CropBoxButton_CheckedChanged(object sender, EventArgs e)
        {
            if (CropBoxButton.Checked == true)
            {
                temp_SelectionType = SelectionType.CropBox;
                SelectAllButton.Checked = false;
                SelectMultipleButton.Checked = false;
                SelectSingleButton.Checked = false;
            }
        }

        private void SelectMultipleButton_CheckedChanged(object sender, EventArgs e)
        {
            if (SelectMultipleButton.Checked == true)
            {
                temp_SelectionType = SelectionType.MultipleElements;
                SelectAllButton.Checked = false;
                CropBoxButton.Checked = false;
                SelectSingleButton.Checked = false;
            }
        }

        private void SelectSingleButton_CheckedChanged(object sender, EventArgs e)
        {
            if (SelectSingleButton.Checked == true)
            {
                temp_SelectionType = SelectionType.SingleElement;
                SelectAllButton.Checked = false;
                CropBoxButton.Checked = false;
                SelectMultipleButton.Checked = false;
            }
        }

        private void WidthValueTB_TextChanged(object sender, EventArgs e)
        {
            double temp;
            if (Double.TryParse(WidthValueTB.Text, out temp))
            {
                WidthValueTB.ForeColor = Color.Black;
                temp_TileWidth = (int)temp;
            }
        }

        private void HeigthValueTB_TextChanged(object sender, EventArgs e)
        {
            double temp;
            if (Double.TryParse(HeigthValueTB.Text, out temp))
            {
                HeigthValueTB.ForeColor = Color.Black;
                temp_TileHeight = (int)temp;
            }
        }

        private void GapValueTB_TextChanged(object sender, EventArgs e)
        {
            double temp;
            if (Double.TryParse(GapValueTB.Text, out temp))
            {
                GapValueTB.ForeColor = Color.Black;
                temp_TileGap = (int)temp;
            }
        }

        private void SaveSettingsButton_MouseEnter(object sender, EventArgs e)
        {
            SaveSettingsButton.BackColor = Color.FromKnownColor(KnownColor.ControlDark);
        }

        private void SaveSettingsButton_MouseLeave(object sender, EventArgs e)
        {
            SaveSettingsButton.BackColor = Color.FromKnownColor(KnownColor.ControlLight);
        }

        private void CancelButton_MouseEnter(object sender, EventArgs e)
        {
            CancelButton.BackColor = Color.FromKnownColor(KnownColor.ControlDark);
        }

        private void CancelButton_MouseLeave(object sender, EventArgs e)
        {
            CancelButton.BackColor = Color.FromKnownColor(KnownColor.ControlLight);
        }

        private void SaveSettingsButton_Click(object sender, EventArgs e)
        {
            Settings.SetSelectionType(temp_SelectionType);
            Settings.SetTileModule(temp_TileWidth, temp_TileHeight, temp_TileGap);
            settingsResult = Result.Succeeded;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            temp_TileWidth = Settings.GetTileModule().ModuleWidth;
            temp_TileHeight = Settings.GetTileModule().ModuleHeight;
            temp_TileGap = Settings.GetTileModule().ModuleGap;
            temp_SelectionType = Settings.GetSelectionType();
            settingsResult = Result.Cancelled;
        }
    }
}
