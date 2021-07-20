using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools_v2.StructuralApps.AssemblyDefiningSubelements
{
    public class AnchorDefining : DefiningBase
    {
        public override Document Document { get; set; }

        public override Element Element { get; set; }
        public override string Name { get; set; }

    }
}
