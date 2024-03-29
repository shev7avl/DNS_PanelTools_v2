﻿using System;
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

        public List<BasePanel> PanelMarks { get; private set; }

        private StructuralEnvironment(Document document)
        {
            List<Element> panelsList = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType().ToElements().Cast<Element>().ToList();
            
            panelsList.Sort(CompareByLevel);
            panelsList.Sort(CompareByName);
            panelsList.Sort(CompareByXCoord);
            panelsList.Sort(CompareByYCoord);

            PanelMarks = new List<BasePanel>();

            foreach (var item in panelsList)
            {
                BasePanel panel = DefinePanelBehaviour(document, item);
                if (panel != null)
                {
                    panel.CreateMarks();
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

            PanelMarks = new List<BasePanel>();

            foreach (var item in panelsList)
            {
                BasePanel panel = DefinePanelBehaviour(document, item);
                if (panel != null)
                {
                    if (exist)
                    {
                        panel.ReadMarks();
                    }
                    else
                    {
                        panel.CreateMarks();
                    }

                    PanelMarks.Add(panel);
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

        public static BasePanel DefinePanelBehaviour(Document doc, Element element)
        {
            StructureType structureType = new StructureType(element);
            StructureType.PanelTypes type = structureType.GetPanelType(element);

            if (type != StructureType.PanelTypes.NOT_A_PANEL)
            {
                if (type == StructureType.PanelTypes.NS_PANEL)
                {
                    NS_Panel nS = new NS_Panel(doc, element);
                    return nS;
                }
                if (type == StructureType.PanelTypes.VS_PANEL)
                {
                    VS_Panel vS = new VS_Panel(doc, element);
                    return vS;
                }
                if (type == StructureType.PanelTypes.BP_PANEL)
                {
                    BP_Panel bP = new BP_Panel(doc, element);
                    return bP;
                }
                if (type == StructureType.PanelTypes.PS_PANEL)
                {
                    PS_Panel pS = new PS_Panel(doc, element);
                    return pS;
                }
                if (type == StructureType.PanelTypes.PP_PANEL)
                {
                    PP_Panel pP = new PP_Panel(doc, element);
                    return pP;
                }
                if (type == StructureType.PanelTypes.FACADE_PANEL)
                {
                    Facade_Panel facade = new Facade_Panel(doc, element);
                    return facade;
                }
            }
            return null;
        }

        public void Reset()
        {
            instance = null;

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
