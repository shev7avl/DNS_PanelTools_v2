using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace DSKPrim.PanelTools.GUI
{
    internal class PrecastLod400Builder : ButtonBuilder
    {
        public SubPanel SubPanelResult;
        private UIControlledApplication ControlledApplication;

        public PrecastLod400Builder(UIControlledApplication application)
        {
            ControlledApplication = application;

            this.Reset();
        }

        public void Reset()
        {
            SubPanelResult = new SubPanel();
        }

        public override void BuildButtons()
        {
            List<Button> buttons = new List<Button>
            {
                new Button(SubPanelResult.RibbonPanel.RibbonPanel, PanelTools.Properties.Resource.element.ToBitmap(), "DSKPrim.PanelTools.PanelMaster.STR_UniqueAssemblies", "Leave Unique Assemblies", "Оставить уникальные сборки"),

                new Button(SubPanelResult.RibbonPanel.RibbonPanel, PanelTools.Properties.Resource.technical_drawing.ToBitmap(), "DSKPrim.PanelTools.PanelMaster.STR_CreateDrawings", "Create panel drawings", "Создать чертежи КЖ.И")
            };

            SubPanelResult.Buttons = buttons;
        }

        public override void BuildRibbon()
        {
            SubPanelResult.RibbonPanel = new Ribbon(ControlledApplication, "КЖ.И - LOD 400");
        }
    }
}
