using Autodesk.Revit.DB;
using DSKPrim.PanelTools.Panel;
using DSKPrim.PanelTools.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.Legacy.Builders.WindowBuilder
{
    public class WindowBuilder
    {
        private protected PrecastPanel _panel;
        public WindowBuilder(PrecastPanel panel)
        { 
        _panel = panel;
        }



        public void Perforate(List<Element> IntersectedWindows, RevitLinkInstance revitLink)
        {

            TransactionGroup transaction = new TransactionGroup(_panel.ActiveDocument, $"Создание проемов - {_panel.ActiveElement.Name}");
            transaction.Start();

            if (IntersectedWindows.Count == 1)
            {
                Element window = IntersectedWindows[0];
                Utility.Openings.SetOpeningParams(_panel.ActiveDocument, revitLink, _panel.ActiveElement, window);
            }
            else if (IntersectedWindows.Count == 2)
            {
                Element window1 = IntersectedWindows[0];
                Element window2 = IntersectedWindows[1];
                Utility.Openings.SetOpeningParams(_panel.ActiveDocument, revitLink, _panel.ActiveElement, window1, window2);
            }
            transaction.Assimilate();
        }

        public void GetOpeningsFromLink(Document linkedArch, RevitLinkInstance revitLink, out List<Element> IntersectedWindows)
        {

            IntersectedWindows = Geometry.IntersectedOpenings(_panel.ActiveElement, revitLink, linkedArch, windows: true);
            if (IntersectedWindows.Count == 0)
            {
                IntersectedWindows = Geometry.IntersectedOpenings(_panel.ActiveElement, revitLink, linkedArch, windows: false);
            }

        }
    }
}
