using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DNS_PanelTools_v2.StructuralApps.Mark
{
    public interface IPanelMark
    {
        Document ActiveDocument { get; set; }
        Element ActiveElement { get; set; }

        string LongMark { get; set; }

        string ShortMark { get; set; }

    }
}
