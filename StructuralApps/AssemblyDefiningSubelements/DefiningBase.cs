using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DSKPrim.PanelTools_v2.StructuralApps.AssemblyDefiningSubelements
{
    public abstract class DefiningBase
    {
        public abstract Document Document { get; set; }
        public abstract Element Element { get; set; }

        public virtual ElementId Id
        {
            get { return Element.Id; }
        }

        public abstract string Name { get; set; }

    }
}
