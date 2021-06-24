using DNS_PanelTools_v2.StructuralApps.Mark;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DNS_PanelTools_v2.StructuralApps.Assemblies
{
    class AssemblyBuilder
    {
        List<IPanelMark> MarksList;

        List<XYZ> frontPVLPts;

        Element ActiveElement;

        Document ActiveDoc;

        public AssemblyBuilder(Document document, Element element)
        {
            ActiveDoc = document;
            ActiveElement = element;

            SingletonMarksList marksList = SingletonMarksList.getInstance(ActiveDoc);
            MarksList = marksList.GetPanelMarks();
        }



    }
}
