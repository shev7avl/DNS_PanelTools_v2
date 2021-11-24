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

        private static int CompareMarksByName(StructuralApps.Panel.Panel x, StructuralApps.Panel.Panel y)
        {
            string _panelNameX = x.LongMark;

            string _panelNameY = y.LongMark;

            return String.Compare(_panelNameX, _panelNameY);

        }

        private static int CompareElementIdsByZCoord(Document document, ElementId x, ElementId y)
        {
            Element elX = document.GetElement(x);
            Element elY = document.GetElement(y);

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
