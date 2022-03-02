using Autodesk.Revit.UI;
using DSKPrim.PanelTools.Facade;
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
        private static AddinSettings Settings;
        private SelectionType temp_SelectionType;
        private TileSectionType temp_TileSectionType;
        private int temp_TileWidth;
        private int temp_TileHeight;
        private int temp_TileGap;

        internal Result settingsResult;

        public SettingsGUI()
        {
            InitializeComponent();

            InitializeFields();
            InitializeTileSectionTypeButtons();
            InitializeSelectionTypeCheckboxes();

            this.ShowDialog();
        }

        private void InitializeFields()
        {
            Settings = AddinSettings.GetSettings();
            temp_SelectionType = Settings.GetSelectionType();
            temp_TileWidth = Settings.GetTileModule().ModuleWidth;
            temp_TileHeight = Settings.GetTileModule().ModuleHeight;
            temp_TileGap = Settings.GetTileModule().ModuleGap;
            temp_TileSectionType = Settings.GetTileSectionType();

            WidthValueTB.Text = Settings.GetTileModule().ModuleWidth.ToString();
            HeigthValueTB.Text = Settings.GetTileModule().ModuleHeight.ToString();
            GapValueTB.Text = Settings.GetTileModule().ModuleGap.ToString();
        }

        private void InitializeTileSectionTypeButtons()
        {
            if (temp_TileSectionType == TileSectionType.TILE_LAYOUT_BRICK)
            {
                brickLayoutBtn.BackColor = Color.FromKnownColor(KnownColor.ControlDark);
            }
            if (temp_TileSectionType == TileSectionType.TILE_LAYOUT_STRAIGHT)
            {
                straightLayoutBtn.BackColor = Color.FromKnownColor(KnownColor.ControlDark);
            }
        }

        private void InitializeSelectionTypeCheckboxes()
        {
            switch (Settings.GetSelectionType())
            {

                case SelectionType.AllElements:
                    {
                        SelectAllButton.Checked = true;
                        CropBoxButton.Checked = false;
                        SelectMultipleButton.Checked = false;
                        SelectSingleButton.Checked = false;
                        break;
                    }

                case SelectionType.CropBox:
                    {
                        SelectAllButton.Checked = false;
                        CropBoxButton.Checked = true;
                        SelectMultipleButton.Checked = false;
                        SelectSingleButton.Checked = false;
                        break;
                    }
                case SelectionType.MultipleElements:
                    {
                        SelectAllButton.Checked = false;
                        CropBoxButton.Checked = false;
                        SelectMultipleButton.Checked = true;
                        SelectSingleButton.Checked = false;
                        break;
                    }
                case SelectionType.SingleElement:
                    {
                        SelectAllButton.Checked = false;
                        CropBoxButton.Checked = false;
                        SelectMultipleButton.Checked = false;
                        SelectSingleButton.Checked = true;
                        break;
                    }
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
            Settings.SetTileSectionType(temp_TileSectionType);
            settingsResult = Result.Succeeded;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            temp_TileWidth = Settings.GetTileModule().ModuleWidth;
            temp_TileHeight = Settings.GetTileModule().ModuleHeight;
            temp_TileGap = Settings.GetTileModule().ModuleGap;
            temp_SelectionType = Settings.GetSelectionType();
            temp_TileSectionType = Settings.GetTileSectionType();
            settingsResult = Result.Cancelled;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void WidthValueTB_MouseClick(object sender, MouseEventArgs e)
        {
            WidthValueTB.Text = "";
            if (HeigthValueTB.Text == "")
            {
                HeigthValueTB.Text = Settings.GetTileModule().ModuleHeight.ToString();
                HeigthValueTB.ForeColor = Color.FromKnownColor(KnownColor.InactiveCaption);
            }
            if (GapValueTB.Text == "")
            {
                GapValueTB.Text = Settings.GetTileModule().ModuleGap.ToString();
                GapValueTB.ForeColor = Color.FromKnownColor(KnownColor.InactiveCaption);
            }
        }

        private void HeigthValueTB_MouseClick(object sender, MouseEventArgs e)
        {
            HeigthValueTB.Text = "";
            if (WidthValueTB.Text == "")
            {
                WidthValueTB.Text = Settings.GetTileModule().ModuleWidth.ToString();
                WidthValueTB.ForeColor = Color.FromKnownColor(KnownColor.InactiveCaption);
            }
            if (GapValueTB.Text == "")
            {
                GapValueTB.Text = Settings.GetTileModule().ModuleGap.ToString();
                GapValueTB.ForeColor = Color.FromKnownColor(KnownColor.InactiveCaption);
            }
        }

        private void GapValueTB_MouseClick(object sender, MouseEventArgs e)
        {
            GapValueTB.Text = "";
            if (HeigthValueTB.Text == "")
            {
                HeigthValueTB.Text = Settings.GetTileModule().ModuleHeight.ToString();
                HeigthValueTB.ForeColor = Color.FromKnownColor(KnownColor.InactiveCaption);
            }
            if (WidthValueTB.Text == "")
            {
                WidthValueTB.Text = Settings.GetTileModule().ModuleWidth.ToString();
                WidthValueTB.ForeColor = Color.FromKnownColor(KnownColor.InactiveCaption);
            }
        }

        private void panel2_Click(object sender, EventArgs e)
        {
            if (HeigthValueTB.Text == "")
            {
                HeigthValueTB.Text = Settings.GetTileModule().ModuleHeight.ToString();
                HeigthValueTB.ForeColor = Color.FromKnownColor(KnownColor.InactiveCaption);
            }
            if (WidthValueTB.Text == "")
            {
                WidthValueTB.Text = Settings.GetTileModule().ModuleWidth.ToString();
                WidthValueTB.ForeColor = Color.FromKnownColor(KnownColor.InactiveCaption);
            }
            if (GapValueTB.Text == "")
            {
                GapValueTB.Text = Settings.GetTileModule().ModuleGap.ToString();
                GapValueTB.ForeColor = Color.FromKnownColor(KnownColor.InactiveCaption);
            }
        }

        private void SettingsGUI_MouseClick(object sender, MouseEventArgs e)
        {
            if (HeigthValueTB.Text == "")
            {
                HeigthValueTB.Text = Settings.GetTileModule().ModuleHeight.ToString();
                HeigthValueTB.ForeColor = Color.FromKnownColor(KnownColor.InactiveCaption);
            }
            if (WidthValueTB.Text == "")
            {
                WidthValueTB.Text = Settings.GetTileModule().ModuleWidth.ToString();
                WidthValueTB.ForeColor = Color.FromKnownColor(KnownColor.InactiveCaption);
            }
            if (GapValueTB.Text == "")
            {
                GapValueTB.Text = Settings.GetTileModule().ModuleGap.ToString();
                GapValueTB.ForeColor = Color.FromKnownColor(KnownColor.InactiveCaption);
            }
        }

        private void brickLayoutBtn_MouseEnter(object sender, EventArgs e)
        {
            brickLayoutBtn.BackColor = Color.FromKnownColor(KnownColor.MenuHighlight);
        }

        private void brickLayoutBtn_MouseLeave(object sender, EventArgs e)
        {
            if (temp_TileSectionType == TileSectionType.TILE_LAYOUT_BRICK)
            {
                brickLayoutBtn.BackColor = Color.FromKnownColor(KnownColor.ControlDark);
            }
            else
            {
                brickLayoutBtn.BackColor = Color.FromKnownColor(KnownColor.Control);
            }
        }

        private void straightLayoutBtn_MouseEnter(object sender, EventArgs e)
        {
            straightLayoutBtn.BackColor = Color.FromKnownColor(KnownColor.MenuHighlight);
        }

        private void straightLayoutBtn_MouseLeave(object sender, EventArgs e)
        {
            if (temp_TileSectionType == TileSectionType.TILE_LAYOUT_STRAIGHT)
            {
                straightLayoutBtn.BackColor = Color.FromKnownColor(KnownColor.ControlDark);
            }
            else
            {
                straightLayoutBtn.BackColor = Color.FromKnownColor(KnownColor.Control);
            }
        }

        private void brickLayoutBtn_Click(object sender, EventArgs e)
        {
            temp_TileSectionType = TileSectionType.TILE_LAYOUT_BRICK;
            straightLayoutBtn.BackColor = Color.FromKnownColor(KnownColor.Control);
        }

        private void straightLayoutBtn_Click(object sender, EventArgs e)
        {
            temp_TileSectionType = TileSectionType.TILE_LAYOUT_STRAIGHT;
            brickLayoutBtn.BackColor = Color.FromKnownColor(KnownColor.Control);
        }

    }
}
