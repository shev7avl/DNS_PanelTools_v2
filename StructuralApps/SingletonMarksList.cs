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

        }

        public static SingletonMarksList getInstance(Document document)
        {
            if (instance == null)
            {
                instance = new SingletonMarksList(document);
            }
            return instance;
        }

        public static List<IPanelMark> GetPanelMarks()
        {
            return instance.PanelMarks;
        }

        public void Dispose()
        {
            ((IDisposable)instance).Dispose();
            Document = null;
            instance = null;
            Behaviour = null;
            PanelMarks = null;
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
