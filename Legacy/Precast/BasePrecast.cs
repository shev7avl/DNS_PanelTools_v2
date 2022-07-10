using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.Precast
{
    internal class BasePrecast
    {
        internal Document Document;

        internal Element ActiveElement;

        internal Mark Mark;      

        internal PrecastType PrecastType;

        internal AssemblyInstance Assembly;

        internal BasePrecast(Element element)
        {
            ActiveElement = element;
            Document = element.Document;
        }


    }
}
