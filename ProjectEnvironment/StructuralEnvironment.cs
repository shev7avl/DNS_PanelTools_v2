using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using DSKPrim.PanelTools.Panel;

namespace DSKPrim.PanelTools.ProjectEnvironment
{
    public class StructuralEnvironment:IResettable
    {
        private static StructuralEnvironment instance;

        private Document Document;

        private BasePanel Behaviour;

        public List<BasePanel> PanelMarks { get; private set; }

        private StructuralEnvironment(Document document)
        {
            Document = document;

            List<Element> panelsList = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType().ToElements().Cast<Element>().ToList();
            
            panelsList.Sort(CompareByLevel);
            panelsList.Sort(CompareByName);
            panelsList.Sort(CompareByXCoord);
            panelsList.Sort(CompareByYCoord);

            PanelMarks = new List<BasePanel>();

            foreach (var item in panelsList)
            {
                SetPanelBehaviour(item);
                if (Behaviour != null)
                {
                    Behaviour.CreateMarks();
                    PanelMarks.Add(Behaviour);
                    Behaviour = null;
                }
            }
        }


        private StructuralEnvironment(Document document, IList<Element> panels, bool exist = false)
        {
            Document = document;

            List<Element> panelsList = (List<Element>)panels;


            panelsList.Sort(CompareByLevel);
            Debug.WriteLine("Панели отсортированы по уровню");
            //panelsList.Sort(CompareByName);
            //Debug.WriteLine("Панели отсортированы по имени");
            panelsList.Sort(CompareByXCoord);
            Debug.WriteLine("Панели отсортированы по координате Х");
            panelsList.Sort(CompareByYCoord);
            Debug.WriteLine("Панели отсортированы по координате У");

            PanelMarks = new List<BasePanel>();

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

        public void CreateMarks(bool exist = false)
        {
            foreach (BasePanel item in PanelMarks)
            {
                if (exist)
                {
                    item.ReadMarks();
                }
                else
                {
                    item.CreateMarks();
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

        public static StructuralEnvironment GetInstance(Document document)
        {
            if (instance == null)
            {
                instance = new StructuralEnvironment(document);
            }
            return instance;
        }

        public static StructuralEnvironment GetInstance(Document document,IList<Element> panels, bool exist = false)
        {
            if (instance == null)
            {
                instance = new StructuralEnvironment(document, panels, exist);
            }
            return instance;
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

        public void Reset()
        {
            Document = null;
            instance = null;
            Behaviour = null;
            if (PanelMarks != null)
            {
                foreach (var item in PanelMarks)
                {
                    item.Reset();
                }
            }
            PanelMarks = null;
        }
    }
}
