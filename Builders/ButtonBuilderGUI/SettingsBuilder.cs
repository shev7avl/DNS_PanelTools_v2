using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace DSKPrim.PanelTools.GUI
{
    public class SettingsBuilder : ButtonBuilder
    {
        public SubPanel SubPanelResult;

        private UIControlledApplication ControlledApplication;

        public SettingsBuilder(UIControlledApplication application)
        {
            ControlledApplication = application;

            Reset();
        }

        public void Reset()
        {
            SubPanelResult = new SubPanel();
        }

        public override void BuildButtons()
        {
            List<Button> buttons = new List<Button>
            {
                new Button(SubPanelResult.RibbonPanel.RibbonPanel, Properties.Resource.settings.ToBitmap(), "DSKPrim.PanelTools.PanelMaster.SettingsCommand", "Settings", "Настройки")
            };

            SubPanelResult.Buttons = buttons;
        }

        public override void BuildRibbon()
        {
            SubPanelResult.RibbonPanel = new Ribbon(ControlledApplication, "Настройки");
        }
    }
}
