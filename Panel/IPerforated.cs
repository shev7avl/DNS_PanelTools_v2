using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace DSKPrim.PanelTools

{
    public interface IPerforable
    {

        List<XYZ> IntersectedWindows { get; set; }

        void GetOpeningsFromLink(Document linkedArch, RevitLinkInstance revitLink, out List<Element> IntersectedWindows);

        void Perforate(List<Element> IntersectedWindows, RevitLinkInstance revitLink);
    }
}
