using DSKPrim.PanelTools_v2.StructuralApps.Panel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using DSKPrim.PanelTools_v2.StructuralApps;

namespace DSKPrim.PanelTools_v2.Commands
{
    public abstract class Routine
    {

        public abstract StructuralApps.Panel.Panel Behaviour { get; set; }

        public abstract Document Document { get; set; }

        public abstract void ExecuteRoutine(ExternalCommandData commandData);

        public virtual void SetPanelBehaviour(Element element)
        {
            StructureType structureType = new StructureType(element);
            string type = structureType.GetPanelType(element);
            if (type == StructureType.Panels.NS.ToString())
            {
                NS_Panel nS = new NS_Panel(Document, element);
                Behaviour = nS;
            }
            if (type == StructureType.Panels.VS.ToString())
            {
                VS_Panel vS = new VS_Panel(Document, element);
                Behaviour = vS;
            }
            if (type == StructureType.Panels.BP.ToString())
            {
                BP_Panel bP = new BP_Panel(Document, element);
                Behaviour = bP;
            }
            if (type == StructureType.Panels.PS.ToString())
            {
                PS_Panel pS = new PS_Panel(Document, element);
                Behaviour = pS;
            }
            if (type == StructureType.Panels.PP.ToString())
            {
                PP_Panel pP = new PP_Panel(Document, element);
                Behaviour = pP;
            }

        }

    }
}
