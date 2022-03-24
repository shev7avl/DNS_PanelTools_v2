using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using DSKPrim.PanelTools.Panel;
using DSKPrim.PanelTools.ProjectEnvironment;

namespace DSKPrim.PanelTools.Utility
{
    internal static class Assemblies
    {
        internal static void CreateAssembly(Document document, BasePanel item)
        {
            if (item is IAssembler assembler && item.ActiveElement.AssemblyInstanceId.IntegerValue == -1)
            {
                Outline outline = new Outline(item.ActiveElement.get_Geometry(new Options()).GetBoundingBox().Min, item.ActiveElement.get_Geometry(new Options()).GetBoundingBox().Max);
                ElementFilter filter = new BoundingBoxIntersectsFilter(outline);
                FilteredElementCollector fecIntersect = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType().WherePasses(filter);

                List<Element> intersected = fecIntersect.ToList();

                assembler.SetAssemblyElements();
                if (item is NS_Panel || item is VS_Panel)
                {
                    foreach (var ints in intersected)
                    {
                        BasePanel behaviour = StructuralEnvironment.DefinePanelBehaviour(document, ints);
                        if (behaviour is NS_Panel || behaviour is VS_Panel)
                        {
                            IAssembler assembler1 = (IAssembler)behaviour;
                            assembler.TransferFromPanel(assembler1);
                        }
                    }
                }

                AssemblyInstance instance;
                AssemblyCreationTransaction(document, item, assembler, out instance);
                item.AssemblyInstance = instance;

            }
            else if (item is IAssembler && item.ActiveElement.AssemblyInstanceId.IntegerValue != -1)
            {
                item.AssemblyInstance = (AssemblyInstance) document.GetElement(item.ActiveElement.AssemblyInstanceId);
            }
        }

        private static void AssemblyCreationTransaction(Document document, BasePanel item, IAssembler assembler, out AssemblyInstance instance)
        {
            Transaction transaction = new Transaction(document, "CreateAssembly");
            TransactionSettings.SetFailuresPreprocessor(transaction);

            transaction.Start();
            try
            {
                instance = AssemblyInstance.Create(document, assembler.AssemblyElements, item.ActiveElement.Category.Id);
                transaction.Commit();

                transaction.Start();
                try
                {
                    instance.AssemblyTypeName = item.ShortMark;
                }
                catch (Autodesk.Revit.Exceptions.ArgumentException)
                {
                    instance.AssemblyTypeName = $"{item.ShortMark} ID{item.ActiveElement.Id}";
                }

                transaction.Commit();

            }


            catch (Autodesk.Revit.Exceptions.ArgumentException)
            {
                
                instance = AssemblyInstance.Create(document, new List<ElementId>() { item.ActiveElement.Id }, item.ActiveElement.Category.Id);
                transaction.Commit();

                transaction.Start();
                try
                {
                    instance.AssemblyTypeName = item.ShortMark;
                }
                catch (Autodesk.Revit.Exceptions.ArgumentException)
                {
                    instance.AssemblyTypeName = $"{item.ShortMark} ID{item.ActiveElement.Id}";
                }

                transaction.Commit();
            }
            if (transaction.GetStatus() != TransactionStatus.Committed)
            {
                transaction.Commit();
            }
        }

        internal static int CompareElementIdsByZCoord(BasePanel x, BasePanel y)
        {
            Element elX = x.ActiveElement;
            Element elY = y.ActiveElement;

            BoundingBoxXYZ boxX = elX.get_Geometry(new Options()).GetBoundingBox();
            BoundingBoxXYZ boxY = elY.get_Geometry(new Options()).GetBoundingBox();

            if (boxX.Min.Z > boxY.Min.Z)
            {
                return 1;
            }
            else if (boxX.Min.Z == boxY.Min.Z)
            {
                return 0;
            }
            else
            {
                return -1;
            }

        }

        internal static List<AssemblyDetailViewOrientation> DefineViewOrientations(Document document, BasePanel basePanel)
        {
            ElementId assemblyInstanceId = basePanel.AssemblyInstance.Id;
            Element elAssembly = document.GetElement(assemblyInstanceId);
            AssemblyInstance assembly = (AssemblyInstance)elAssembly;
            ICollection<ElementId> ids = assembly.GetMemberIds();
            List<Element> elements = ids.Select(o => document.GetElement(o)).ToList();
            Element panel;
            if (basePanel is Facade_Panel)
            {
                Part part = elements.Where(o => o.Category.Name.Contains("Части")).Cast<Part>().First();
                ICollection<LinkElementId> links = part.GetSourceElementIds();
                ElementId elementId = links.First().HostElementId;
                panel = document.GetElement(elementId);
                while (panel is Part)
                {
                    part = (Part)panel;
                    links = part.GetSourceElementIds();
                    elementId = links.First().HostElementId;
                    panel = document.GetElement(elementId);
                }
                

            }
            else
            {
                panel = elements.Where(o => o.Category.Name.Contains(basePanel.ActiveElement.Category.Name)).First();
            }

            XYZ facingOrientation;
            if (panel is Wall)
            {
                Wall wall = (Wall)panel;
                facingOrientation = wall.Orientation;
            }
            else
            {
                FamilyInstance instance = (FamilyInstance)panel;
                facingOrientation = instance.FacingOrientation;
            }

            List<AssemblyDetailViewOrientation> orientations = null;

            XYZ f = new XYZ(0, 1, 0);
            XYZ b = new XYZ(0, -1, 0);
            XYZ l = new XYZ(1, 0, 0);
            XYZ r = new XYZ(-1, 0, 0);

            if (facingOrientation.IsAlmostEqualTo(b))
            {
                orientations = new List<AssemblyDetailViewOrientation>()
                    {
                        AssemblyDetailViewOrientation.ElevationFront,
                        AssemblyDetailViewOrientation.DetailSectionA,
                        AssemblyDetailViewOrientation.DetailSectionB,
                        AssemblyDetailViewOrientation.HorizontalDetail
                    };
            }
            else if (facingOrientation.IsAlmostEqualTo( f))
            {
                orientations = new List<AssemblyDetailViewOrientation>()
                    {
                        AssemblyDetailViewOrientation.ElevationBack,
                        AssemblyDetailViewOrientation.DetailSectionA,
                        AssemblyDetailViewOrientation.DetailSectionB,
                        AssemblyDetailViewOrientation.HorizontalDetail
                    };
            }
            else if(facingOrientation.IsAlmostEqualTo( r))
            {
                orientations = new List<AssemblyDetailViewOrientation>()
                    {
                        AssemblyDetailViewOrientation.ElevationLeft,
                        AssemblyDetailViewOrientation.DetailSectionB,
                        AssemblyDetailViewOrientation.DetailSectionA,
                        AssemblyDetailViewOrientation.HorizontalDetail
                    };
            }
            else if(facingOrientation.IsAlmostEqualTo( l))
            {
                orientations = new List<AssemblyDetailViewOrientation>()
                    {
                        AssemblyDetailViewOrientation.ElevationRight,
                        AssemblyDetailViewOrientation.DetailSectionB,
                        AssemblyDetailViewOrientation.DetailSectionA,
                        AssemblyDetailViewOrientation.HorizontalDetail
                    };
            }
            else
            {
                orientations = new List<AssemblyDetailViewOrientation>()
                    {
                        AssemblyDetailViewOrientation.ElevationFront,
                        AssemblyDetailViewOrientation.DetailSectionA,
                        AssemblyDetailViewOrientation.DetailSectionB,
                        AssemblyDetailViewOrientation.HorizontalDetail
                    };
            }
            return orientations;
        }

        internal static void UniquePanels(Document document, ICollection<AssemblyInstance> assemblies)
        {
            SortedDictionary<string, int> assembliesSortedHashMap = new SortedDictionary<string, int>();
            foreach (AssemblyInstance assembly in assemblies)
            {
                if (assembly.IsValidObject)
                {
                    string keyName = assembly.AssemblyTypeName;
                    if (assembliesSortedHashMap.ContainsKey(keyName))
                    {
                        assembliesSortedHashMap[keyName]++;
                    }
                    else
                    {
                        assembliesSortedHashMap.Add(keyName, 1);
                    }
                }
                
            }

            Transaction transaction = new Transaction(document, "Deleting duplicates");
            TransactionSettings.SetFailuresPreprocessor(transaction);
            using (transaction)
            {
                transaction.Start();
                foreach (var item in assemblies)
                {
                    if (item.IsValidObject)
                    {
                        string keyName = item.AssemblyTypeName;
                        if (assembliesSortedHashMap[keyName] > 1)
                        {
                            document.Delete(item.Id);
                            assembliesSortedHashMap[keyName]--;
                        }
                    }
                }
                transaction.Commit();
            }
        }

        private static int CompareAssembliesByName(AssemblyInstance x, AssemblyInstance y)
        {
            string _postfixX = x.AssemblyTypeName;

            string _postfixY = y.AssemblyTypeName;

            return String.Compare(_postfixX, _postfixY);

        }

        private static int CompareAsembliesbyLvl(AssemblyInstance x, AssemblyInstance y)
        {
            LocationPoint locoPocoX = (LocationPoint)x.Location;
            LocationPoint locoPocoY = (LocationPoint)y.Location;

            XYZ X = locoPocoX.Point;
            XYZ Y = locoPocoY.Point;

            if (X.Z > Y.Z)
            {
                return 1;
            }
            else if (X.Z == Y.Z)
            {
                return 0;
            }
            else
            {
                return -1;
            }


         
        }

        internal static void DisassembleAssembliesCollection(Document document, ICollection<AssemblyInstance> assemblies)
        {
                string index;
                foreach (AssemblyInstance assembly in assemblies)
                {
                    index = "";
                    if (Eligible(assembly))
                    {

                    //FailureResolution fr = DeleteElements.Create(document, assembly.Id);

                        index = assembly.get_Parameter(new Guid(Properties.Resource.ADSK_Номер_изделия)).AsString();
                        if (index != "")
                        {
                            ICollection<ElementId> ids = assembly.GetMemberIds();
                            List<Element> elements = ids.Select(o => document.GetElement(o)).ToList();
                            Element panel = elements.Where(o => o.Category.Name.Contains("Каркас несущий")).First();
                            panel.get_Parameter(new Guid(Properties.Resource.ADSK_Номер_изделия)).Set(index);
                        }
                    //document.Delete(assembly.Id);
                    assembly.Disassemble();
                    }
                }
         
        }

        private static bool Eligible(AssemblyInstance assembly)
        {
            if (assembly.AssemblyTypeName.Contains("НС") ||
                assembly.AssemblyTypeName.Contains("ВС") ||
                assembly.AssemblyTypeName.Contains("ПС") ||
                assembly.AssemblyTypeName.Contains("ПП") ||
                assembly.AssemblyTypeName.Contains("БП")) return true;
            else return false;
        }



    }

    public class AssemblyComparer : IEqualityComparer<AssemblyInstance>
    {
        public bool Equals(AssemblyInstance x, AssemblyInstance y)
        {
            return x.AssemblyTypeName == y.AssemblyTypeName;
        }

        public int GetHashCode(AssemblyInstance obj)
        {
            return 1;
        }
    }
}
