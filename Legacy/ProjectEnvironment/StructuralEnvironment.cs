using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using DSKPrim.PanelTools.Legacy.Controllers;
using DSKPrim.PanelTools.Panel;

namespace DSKPrim.PanelTools.ProjectEnvironment
{
    public class StructuralEnvironment:IResettable
    {
        private static StructuralEnvironment instance;

        public List<PrecastPanel> PanelMarks { get; private set; }

        private StructuralEnvironment(Document document)
        {
            List<Element> panelsList = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType().ToElements().Cast<Element>().ToList();
            
            panelsList.Sort(CompareByLevel);
            panelsList.Sort(CompareByName);
            panelsList.Sort(CompareByXCoord);
            panelsList.Sort(CompareByYCoord);

            PanelMarks = new List<PrecastPanel>();

            foreach (var item in panelsList)
            {
                PrecastPanel panel = new PrecastPanel(document, item);
                if (panel != null)
                {
                    PanelMarks.Add(panel);
                }
            }
        }


        private StructuralEnvironment(Document document, IList<Element> panels, bool exist = false)
        {

            List<Element> panelsList = (List<Element>)panels;


            panelsList.Sort(CompareByLevel);
            Debug.WriteLine("Панели отсортированы по уровню");
            //panelsList.Sort(CompareByName);
            //Debug.WriteLine("Панели отсортированы по имени");
            panelsList.Sort(CompareByXCoord);
            Debug.WriteLine("Панели отсортированы по координате Х");
            panelsList.Sort(CompareByYCoord);
            Debug.WriteLine("Панели отсортированы по координате У");

            PanelMarks = new List<PrecastPanel>();

            foreach (var item in panelsList)
            {
                PrecastPanel panel = new PrecastPanel(document, item);
                if (panel != null)
                {
                    if (exist)
                    {
                        //panel.ReadMarks();
                    }
                    else
                    {
                        //panel.CreateMarks();
                    }

                    PanelMarks.Add(panel);
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


        public void Reset()
        {
            instance = null;

            if (PanelMarks != null)
            {
                foreach (var item in PanelMarks)
                {
                    //item.Reset();
                }
            }
            PanelMarks = null;
        }
    }
}
