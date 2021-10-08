using DSKPrim.PanelTools_v2.StructuralApps.Panel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DSKPrim.PanelTools_v2.StructuralApps
{
    public class SingleStructDoc
    {
        private static SingleStructDoc instance;

        private Document Document;

        private Panel.Panel Behaviour;

        public List<Panel.Panel> PanelMarks { get; private set; }

        private List<Element> frontPVL;

        private SingleStructDoc(Document document, bool exist = false)
        {
            Document = document;

            List<Element> panelsList = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType().ToElements().Cast<Element>().ToList();
            panelsList.Sort(CompareByLevel);
            panelsList.Sort(CompareByName);
            panelsList.Sort(CompareByXCoord);
            panelsList.Sort(CompareByYCoord);

            PanelMarks = new List<Panel.Panel>();

            foreach (var item in panelsList)
            {
                SetPanelBehaviour(item);
                if (Behaviour != null)
                {
                    if (exist)
                    {
                        Behaviour.ReadMarks();
                    }
                    else
                    {
                        Behaviour.CreateMarks();
                    }
                    
                    PanelMarks.Add(Behaviour);
                    Behaviour = null;
                }
                
            }
        }

        private SingleStructDoc(Document document, IList<Element> panels, bool exist = false)
        {
            Document = document;

            List<Element> panelsList = (List<Element>)panels;

            panelsList.Sort(CompareByLevel);
            panelsList.Sort(CompareByName);
            panelsList.Sort(CompareByXCoord);
            panelsList.Sort(CompareByYCoord);

            PanelMarks = new List<Panel.Panel>();

            foreach (var item in panelsList)
            {
                SetPanelBehaviour(item);
                if (Behaviour != null)
                {
                    if (exist)
                    {
                        Behaviour.ReadMarks();
                    }
                    else
                    {
                        Behaviour.CreateMarks();
                    }

                    PanelMarks.Add(Behaviour);
                    Behaviour = null;
                }

            }
        }

        public int CompareByName(Element x, Element y)
        {
            return String.Compare(x.Name, y.Name);
        }
        public int CompareByLevel(Element x, Element y)
        {
            BoundingBoxXYZ XboxXYZ = x.get_Geometry(new Options()).GetBoundingBox();
            BoundingBoxXYZ YboxXYZ = y.get_Geometry(new Options()).GetBoundingBox();

            if (XboxXYZ.Min.Z < YboxXYZ.Min.Z)
            {
                return -1;
            }
            else if (XboxXYZ.Min.Z > YboxXYZ.Min.Z)
            {
                return 1;
            }
            else return 0;

        }
        public int CompareByXCoord(Element x, Element y)
        {
            BoundingBoxXYZ XboxXYZ = x.get_Geometry(new Options()).GetBoundingBox();
            BoundingBoxXYZ YboxXYZ = y.get_Geometry(new Options()).GetBoundingBox();

            if (XboxXYZ.Min.X < YboxXYZ.Min.X)
            {
                return -1;
            }
            else if (XboxXYZ.Min.X > YboxXYZ.Min.X)
            {
                return 1;
            }
            else return 0;
        }
        public int CompareByYCoord(Element x, Element y)
        {
            BoundingBoxXYZ XboxXYZ = x.get_Geometry(new Options()).GetBoundingBox();
            BoundingBoxXYZ YboxXYZ = y.get_Geometry(new Options()).GetBoundingBox();

            if (XboxXYZ.Min.Y < YboxXYZ.Min.Y)
            {
                return -1;
            }
            else if (XboxXYZ.Min.Y > YboxXYZ.Min.Y)
            {
                return 1;
            }
            else return 0;
        }

        public static SingleStructDoc getInstance(Document document, bool exist = false)
        {
            if (instance == null)
            {
                instance = new SingleStructDoc(document, exist);
            }
            return instance;
        }

        public static SingleStructDoc getInstance(Document document,IList<Element> panels, bool exist = false)
        {
            if (instance == null)
            {
                instance = new SingleStructDoc(document, panels, exist);
            }
            return instance;
        }


        public void Dispose()
        {
            Document = null;
            instance = null;
            Behaviour = null;
            PanelMarks = null;
            frontPVL = null;
        }

        public void SetPanelBehaviour(Element element)
        {
            StructureType structureType = new StructureType(element);
            string type = structureType.GetPanelType(element);
            if (type != StructureType.Panels.None.ToString())
            {
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
            else
            {
                Behaviour = null;
            }
        }
    }
}
