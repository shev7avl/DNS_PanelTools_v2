using DNS_PanelTools_v2.StructuralApps.Panel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DNS_PanelTools_v2.StructuralApps
{
    public class SingleStructDoc
    {
        private static SingleStructDoc instance;

        private Document Document;

        private IPanel Behaviour;

        private List<IPanel> PanelMarks;

        private List<XYZ> frontPVLpts;

        private SingleStructDoc(Document document)
        {
            Document = document;

            List<Element> panelsList = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType().ToElements().Cast<Element>().ToList();
            PanelMarks = new List<IPanel>();

            foreach (var item in panelsList)
            {
                SetPanelBehaviour(item);
                Behaviour.FillMarks();
                PanelMarks.Add(Behaviour);
                Behaviour = null;
            }
            FillPVLList();
        }

        public static SingleStructDoc getInstance(Document document)
        {
            if (instance == null)
            {
                instance = new SingleStructDoc(document);
            }
            return instance;
        }

        public List<IPanel> GetPanelMarks()
        {
            return PanelMarks;
        }

        public List<XYZ> getPVLpts()
        {
            return frontPVLpts;
        }

        public void Dispose()
        {
            Document = null;
            instance = null;
            Behaviour = null;
            PanelMarks = null;
            frontPVLpts = null;
        }

        private void FillPVLList()
        {
            IEnumerable<Element> frontPVLelements = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_GenericModel).WhereElementIsNotElementType().ToElements().Where(o => o.Name.Contains("PVL_Торцевая"));
            frontPVLpts = new List<XYZ>();

            foreach (var item in frontPVLelements)
            {
                LocationPoint point = (LocationPoint)item.Location;
                frontPVLpts.Add(point.Point);
            }

        }

        protected void SetPanelBehaviour(Element element)
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
