using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DSKPrim.PanelTools_v2.StructuralApps.Panel
{
    public interface IPerforable
    {

        void GetOpenings(Document linkedArch, out List<Element> IntersectedWindows);

        void Perforate(List<Element> IntersectedWindows);
    }
}
