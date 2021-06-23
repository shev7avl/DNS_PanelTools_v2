using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DNS_PanelTools_v2.StructuralApps.LongMark
{
    public interface IPanelLongMark
    {
        Document ActiveDocument { get; set; }
        Element ActiveElement { get; set; }

        string LongMarkLogic();

        void SetLongMark();
    }
}
