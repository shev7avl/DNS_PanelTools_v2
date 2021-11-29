using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using System.Text;
using System.Threading.Tasks;
using DSKPrim.PanelTools_v2.Utility;
using DSKPrim.PanelTools_v2.StructuralApps.Panel;

namespace DSKPrim.PanelTools_v2.Utility
{
    public static class Assemblies
    {
        public static void CreateAssembly(Document document, Logger.Logger logger, StructuralApps.Panel.Panel item)
        {
            if (item is IAssembler assembler)
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
                        Commands.Routine.GetPanelBehaviour(document, ints, out StructuralApps.Panel.Panel behaviour);
                        if (behaviour is NS_Panel || behaviour is VS_Panel)
                        {
                            IAssembler assembler1 = (IAssembler)behaviour;
                            assembler.TransferFromPanel(assembler1);
                        }
                    }
                }

                AssemblyCreationTransaction(document, logger, item, assembler);

            }
        }

        private static void AssemblyCreationTransaction(Document document, Logger.Logger logger, StructuralApps.Panel.Panel item, IAssembler assembler)
        {
            Transaction transaction = new Transaction(document, "CreateAssembly");
            FailureHandlingOptions opts = transaction.GetFailureHandlingOptions();
            IFailuresPreprocessor preprocessor = new WarningDiscard();
            opts.SetFailuresPreprocessor(preprocessor);
            transaction.SetFailureHandlingOptions(opts);

            transaction.Start();
            try
            {
                AssemblyInstance instance = AssemblyInstance.Create(document, assembler.AssemblyElements, item.ActiveElement.Category.Id);
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
                logger.DebugLog($"Произошла ошибка в панели {item.ShortMark} на уровне {item.ActiveElement.LevelId}");
                AssemblyInstance instance = AssemblyInstance.Create(document, new List<ElementId>() { item.ActiveElement.Id }, item.ActiveElement.Category.Id);
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
        }

        public static int CompareElementIdsByZCoord(StructuralApps.Panel.Panel x, StructuralApps.Panel.Panel y)
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


        public static void LeaveUniquePanels(Document document)
        {
            Logger.Logger logger = Logger.Logger.getInstance();
            List<AssemblyInstance> assemblies = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_Assemblies).WhereElementIsNotElementType().Cast<AssemblyInstance>().ToList();
            assemblies.Sort(CompareAssembliesByName);

            List<AssemblyType> assemblyTypes = new FilteredElementCollector(document).OfClass(typeof(AssemblyType)).WhereElementIsElementType().Cast<AssemblyType>().ToList();

            List<List<AssemblyInstance>> instances = new List<List<AssemblyInstance>>();

            foreach (var assemblyType in assemblyTypes)
            {
                List<AssemblyInstance> inst = new List<AssemblyInstance>();
                foreach (var assembly in assemblies)
                {
                    if (assembly.Name == assemblyType.Name)
                    {
                        inst.Add(assembly);
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
                logger.DebugLog($"Число сборок: {disposables.Count} / {amount - assemblyTypes.Count}");
            }


            logger.DebugLog($"Уникальные сборки определены");


            using (Transaction transaction = new Transaction(document, "Разбираем сборки"))
            {
                logger.DebugLog("Начинаем разборку");
                int counter = 1;
                foreach (AssemblyInstance assembly in disposables)
                {
                    logger.DebugLog($"Прогресс {counter} / {disposables.Count}");
                    transaction.Start();
                    FailureHandlingOptions opts = transaction.GetFailureHandlingOptions();
                    IFailuresPreprocessor preprocessor = new WarningDiscard();
                    opts.SetFailuresPreprocessor(preprocessor);
                    transaction.SetFailureHandlingOptions(opts);
                    //assembly.Disassemble();
                    document.Delete(assembly.Id);
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

        public static void DisassembleAll(Document document)
        {
            List<AssemblyInstance> assemblies = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_Assemblies).WhereElementIsNotElementType().Cast<AssemblyInstance>().ToList();

            using (Transaction transaction = new Transaction(document, "Разбираем сборки"))
            {


                foreach (AssemblyInstance assembly in assemblies)
                {
                    if (Eligible(assembly))
                    {
                        transaction.Start();

                        FailureHandlingOptions failOpt
          = transaction.GetFailureHandlingOptions();

                        failOpt.SetFailuresPreprocessor(
                          new WarningDiscard());

                        transaction.SetFailureHandlingOptions(failOpt);

                        assembly.Disassemble();
                        transaction.Commit();
                    }
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
