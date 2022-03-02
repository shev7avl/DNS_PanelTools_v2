using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace DSKPrim.PanelTools.GUI
{
    public class TestBuilder : ButtonBuilder
    {
        public SubPanel SubPanelResult;

        private UIControlledApplication ControlledApplication;

        public TestBuilder(UIControlledApplication application)
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
                new Button(SubPanelResult.RibbonPanel.RibbonPanel, Properties.Resource.files.ToBitmap(), "DSKPrim.PanelTools.PanelMaster.TestCommand", "Test function", "Тестовая функция")      
            };

            SubPanelResult.Buttons = buttons;
        }

        public override void BuildRibbon()
        {
            SubPanelResult.RibbonPanel = new Ribbon(ControlledApplication, "Test");
        }
    }
}
