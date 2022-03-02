using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace DSKPrim.PanelTools.GUI
{
    class PrecastLod100Builder : ButtonBuilder
    {
        public SubPanel SubPanelResult;
        private UIControlledApplication ControlledApplication;

        public PrecastLod100Builder(UIControlledApplication application)
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
                new Button(SubPanelResult.RibbonPanel.RibbonPanel, Properties.Resource.id_card.ToBitmap(), "DSKPrim.PanelTools.PanelMaster.STR_SetMarks", "Create marks", "Создать марки"),
                new Button(SubPanelResult.RibbonPanel.RibbonPanel, Properties.Resource.open_door.ToBitmap(), "DSKPrim.PanelTools.PanelMaster.STR_CreateOpenings", "Create panel openings", "Создать проемы"),
                new Button(SubPanelResult.RibbonPanel.RibbonPanel, Properties.Resource.product.ToBitmap(), "DSKPrim.PanelTools.PanelMaster.STR_CreateAssemblies", "Create assemblies", "Создать сборки"),
                new Button(SubPanelResult.RibbonPanel.RibbonPanel, Properties.Resource.demolition.ToBitmap(), "DSKPrim.PanelTools.PanelMaster.STR_DisassembleAll", "Disassemble all", "Разобрать сборки")
            };

            SubPanelResult.Buttons = buttons;
        }

        public override void BuildRibbon()
        {
            SubPanelResult.RibbonPanel = new Ribbon(ControlledApplication, "КЖ - LOD 100");
        }
    }
}
