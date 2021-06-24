using DNS_PanelTools_v2.StructuralApps.Mark;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DNS_PanelTools_v2.StructuralApps
{
    public class SingletonMarksList: IDisposable
    {
        private static SingletonMarksList instance;

        private Document Document;

        private IPanelMark Behaviour;

        private List<IPanelMark> PanelMarks;

        private List<XYZ> frontPVLpts;

        private SingletonMarksList(Document document)
        {
            Document = document;

            List<Element> panelsList = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType().ToElements().Cast<Element>().ToList();
            PanelMarks = new List<IPanelMark>();

            foreach (var item in panelsList)
            {
                SetPanelBehaviour(item);
                Behaviour.FillMarks();
                PanelMarks.Add(Behaviour);
                Behaviour = null;
            }
            FillPVLList();
        }

        public static SingletonMarksList getInstance(Document document)
        {
            if (instance == null)
            {
                instance = new SingletonMarksList(document);
            }
            return instance;
        }

        public List<IPanelMark> GetPanelMarks()
        {
            return PanelMarks;
        }

        public void Dispose()
        {
            ((IDisposable)instance).Dispose();
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
                NSMark nS = new NSMark(Document, element);
                Behaviour = nS;
            }
            if (type == StructureType.Panels.VS.ToString())
            {
                VSMark vS = new VSMark(Document, element);
                Behaviour = vS;
            }
            if (type == StructureType.Panels.BP.ToString())
            {
                BPMark bP = new BPMark(Document, element);
                Behaviour = bP;
            }
            if (type == StructureType.Panels.PS.ToString())
            {
                PSMark pS = new PSMark(Document, element);
                Behaviour = pS;
            }
            if (type == StructureType.Panels.PP.ToString())
            {
                PPMark pP = new PPMark(Document, element);
                Behaviour = pP;
            }

        }
    }
}
