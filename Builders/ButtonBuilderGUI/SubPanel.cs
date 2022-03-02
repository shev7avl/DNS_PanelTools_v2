using System.Collections.Generic;

namespace DSKPrim.PanelTools.GUI
{
    public class SubPanel
    {

        public Ribbon RibbonPanel { get; set; }

        public List<Button> Buttons { get; set; }

        public SubPanel(Ribbon ribbon, List<Button> buttons)
        {
            RibbonPanel = ribbon;
            Buttons = buttons;
        }

        public SubPanel()
        { }
    }

    
}
