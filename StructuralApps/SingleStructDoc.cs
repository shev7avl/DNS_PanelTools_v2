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

        private Base_Panel Behaviour;

        private List<Base_Panel> PanelMarks;

        private List<Element> frontPVL;

        private SingleStructDoc(Document document)
        {
            Document = document;

            List<Element> panelsList = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType().ToElements().Cast<Element>().ToList();
            PanelMarks = new List<Base_Panel>();

            foreach (var item in panelsList)
            {
                SetPanelBehaviour(item);
                Behaviour.CreateMarks();
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

        public List<Base_Panel> GetPanelMarks()
        {
            return PanelMarks;
        }

        public List<Element> getPVLpts()
        {
            return frontPVL;
        }

        public void Dispose()
        {
            Document = null;
            instance = null;
            Behaviour = null;
            PanelMarks = null;
            frontPVL = null;
        }

        private void FillPVLList()
        {
            frontPVL = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_GenericModel).WhereElementIsNotElementType().ToElements().Where(o => o.Name.Contains("PVL_Торцевая")).ToList();
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
