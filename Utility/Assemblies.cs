using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using DSKPrim.PanelTools.Panel;

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
                        PanelMaster.Routine.GetPanelBehaviour(document, ints, out BasePanel behaviour);
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

        internal static List<AssemblyDetailViewOrientation> DefineViewOrientations(Document document, ElementId assemblyInstanceId)
        {
            Element elAssembly = document.GetElement(assemblyInstanceId);
            AssemblyInstance assembly = (AssemblyInstance)elAssembly;
            ICollection<ElementId> ids = assembly.GetMemberIds();
            List<Element> elements = ids.Select(o => document.GetElement(o)).ToList();
            Element panel = elements.Where(o => o.Category.Name.Contains("Каркас несущий")).First();
            FamilyInstance instance = (FamilyInstance)panel;
            //instance.FacingOrientation;
            List<AssemblyDetailViewOrientation> orientations = null;

            XYZ f = new XYZ(0, 1, 0);
            XYZ b = new XYZ(0, -1, 0);
            XYZ l = new XYZ(1, 0, 0);
            XYZ r = new XYZ(-1, 0, 0);

            if (instance.FacingOrientation.IsAlmostEqualTo(b))
            {
                orientations = new List<AssemblyDetailViewOrientation>()
                    {
                        AssemblyDetailViewOrientation.ElevationFront,
                        AssemblyDetailViewOrientation.DetailSectionA,
                        AssemblyDetailViewOrientation.DetailSectionB,
                        AssemblyDetailViewOrientation.HorizontalDetail
                    };
            }
            else if (instance.FacingOrientation.IsAlmostEqualTo( f))
            {
                orientations = new List<AssemblyDetailViewOrientation>()
                    {
                        AssemblyDetailViewOrientation.ElevationBack,
                        AssemblyDetailViewOrientation.DetailSectionA,
                        AssemblyDetailViewOrientation.DetailSectionB,
                        AssemblyDetailViewOrientation.HorizontalDetail
                    };
            }
            else if(instance.FacingOrientation.IsAlmostEqualTo( r))
            {
                orientations = new List<AssemblyDetailViewOrientation>()
                    {
                        AssemblyDetailViewOrientation.ElevationLeft,
                        AssemblyDetailViewOrientation.DetailSectionB,
                        AssemblyDetailViewOrientation.DetailSectionA,
                        AssemblyDetailViewOrientation.HorizontalDetail
                    };
            }
            else if(instance.FacingOrientation.IsAlmostEqualTo( l))
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



        internal static void LeaveUniquePanels(Document document)
        {

            List<AssemblyInstance> assemblies = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_Assemblies).WhereElementIsNotElementType().Cast<AssemblyInstance>().ToList();
            assemblies.Sort(CompareAssembliesByName);

            List<AssemblyType> assemblyTypes = new FilteredElementCollector(document).OfClass(typeof(AssemblyType)).WhereElementIsElementType().Cast<AssemblyType>().ToList();

            List<List<AssemblyInstance>> instances = new List<List<AssemblyInstance>>();

            foreach (var assemblyType in assemblyTypes)
            {
                List<AssemblyInstance> inst = new List<AssemblyInstance>();
                for (int i = 1; i < assemblies.Count; i++)
                {
                    
                    if (!(AssemblyInstance.CompareAssemblyInstances(assemblies[i-1], assemblies[i]) is AssemblyDifferenceNone))
                    {
                        inst.Add(assemblies[i]);
                    }
                }
                instances.Add(inst);
            }

            List<AssemblyInstance> disposables = new List<AssemblyInstance>();

            IEqualityComparer<AssemblyInstance> comparer = new AssemblyComparer();

            int amount = assemblies.Count;

            foreach (var lst in instances)
            {
                int cnt = lst.Count;
                lst.Sort(CompareAsembliesbyLvl);
                foreach (var assembly in lst)
                {
                    if (cnt > 1)
                    {
                        disposables.Add(assembly);
                    }
                    cnt--;
                }
                Debug.WriteLine($"Число сборок: {disposables.Count} / {amount - assemblyTypes.Count}");
            }

            Debug.WriteLine($"Уникальные сборки определены");

            using (Transaction transaction = new Transaction(document, "Разбираем сборки"))
            {
                IFailuresPreprocessor preprocessor = new TransactionSettings.WarningDiscard();
                FailureHandlingOptions fho = transaction.GetFailureHandlingOptions();
                fho.SetFailuresPreprocessor(preprocessor);
                transaction.SetFailureHandlingOptions(fho);

                Debug.WriteLine("Начинаем разборку");
                int counter = 1;
                foreach (AssemblyInstance assembly in disposables)
                {
                    Debug.WriteLine($"Прогресс {counter} / {disposables.Count}");
                    transaction.Start();
                    //assembly.Disassemble();
                    if (assembly.IsValidObject)
                    {
                        document.Delete(assembly.Id);
                    }

                    transaction.Commit();
                    counter++;
                }
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

            int i = 0;
            int j = 1;

            int[,] vs = new int[i, j];

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

        internal static void DisassembleAll(Document document)
        {
            List<AssemblyInstance> assemblies = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_Assemblies).WhereElementIsNotElementType().Cast<AssemblyInstance>().ToList();

                
            Transaction transaction = new Transaction(document, "Разбираем сборки");
                TransactionSettings.SetFailuresPreprocessor(transaction);
                string index;
                foreach (AssemblyInstance assembly in assemblies)
                {
                    index = "";
                    if (Eligible(assembly))
                    {
                        transaction.Start();

                    FailureResolution fr = DeleteElements.Create(document, assembly.Id);

                        index = assembly.get_Parameter(new Guid(Properties.Resource.ADSK_Номер_изделия)).AsString();
                        if (index != "")
                        {
                            ICollection<ElementId> ids = assembly.GetMemberIds();
                            List<Element> elements = ids.Select(o => document.GetElement(o)).ToList();
                            Element panel = elements.Where(o => o.Category.Name.Contains("Каркас несущий")).First();
                            panel.get_Parameter(new Guid(Properties.Resource.ADSK_Номер_изделия)).Set(index);
                        }
                        assembly.Disassemble();
                        transaction.Commit();
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
