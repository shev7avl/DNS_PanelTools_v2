using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace DSKPrim.PanelTools.GUI
{
    public class PrecastArchBuilder : ButtonBuilder
    {
        public SubPanel SubPanelResult;

        private UIControlledApplication ControlledApplication;

        public PrecastArchBuilder(UIControlledApplication application)
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
                new Button(SubPanelResult.RibbonPanel.RibbonPanel, Properties.Resource.files.ToBitmap(), "DSKPrim.PanelTools.PanelMaster.ARCH_copyMarks", "Copy marks", "Скопировать марки"),
                new Button(SubPanelResult.RibbonPanel.RibbonPanel, Properties.Resource.window.ToBitmap(), "DSKPrim.PanelTools.PanelMaster.ARCH_Windows", "Create openings", "Создать проемы"),
                new Button(SubPanelResult.RibbonPanel.RibbonPanel, Properties.Resource.wall.ToBitmap(), "DSKPrim.PanelTools.PanelMaster.ARCH_SplitToParts", "Split to parts", "Создать плитку"),
                new Button(SubPanelResult.RibbonPanel.RibbonPanel, Properties.Resource.construction_plan.ToBitmap(), "DSKPrim.PanelTools.PanelMaster.ARCH_CreateDrawings", "Create Drawings", "Создать чертежи")
            };

            SubPanelResult.Buttons = buttons;
        }

        public override void BuildRibbon()
        {
            SubPanelResult.RibbonPanel = new Ribbon(ControlledApplication, "АКР");
        }
    }
}
