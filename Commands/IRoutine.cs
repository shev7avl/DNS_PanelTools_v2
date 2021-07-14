using DNS_PanelTools_v2.StructuralApps.Panel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;

namespace DNS_PanelTools_v2.Commands
{
    public abstract class Base_Routine
    {

        public abstract void ExecuteRoutine(ExternalCommandData commandData);

    }
}
