using DSKPrim.PanelTools_v2.StructuralApps.Panel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using System.Diagnostics;
using System.Collections;

namespace DSKPrim.PanelTools_v2.StructuralApps.Assemblies
{
    public class AssemblyBuilder
    {

        public AssemblyInstance result;

        private Dictionary<int, Panel.Panel> IndexMarkPairs;

        private List<Panel.Panel> MarksList;

        private Document ActiveDoc { get; set; }

        public AssemblyBuilder(Document document)
        {
            ActiveDoc = document;
            SingleStructDoc marksList = SingleStructDoc.getInstance(ActiveDoc, exist: true);
            MarksList = marksList.PanelMarks;
        }

        public AssemblyBuilder()
        { }


        #region Создание сборок

        public void CreateAssemblies()
        {
            TransactionGroup transactionGroup = new TransactionGroup(ActiveDoc, "Создание сборок");
            transactionGroup.Start();
            MarksList.Sort(CompareMarksByName);
            foreach (var item in MarksList)
            {
                FamilyInstance familyInstance = (FamilyInstance)item.ActiveElement;
                ICollection<ElementId> elementIds0 = familyInstance.GetSubComponentIds();

                //Удаляем нижние элементы
                    int q = item.ActiveElement.GetParameters("Количество пазов")[0].AsInteger() * 5;
                    List<ElementId> elements = (List<ElementId>)elementIds0;
                    elements.Sort(CompareElementIdsByZCoord);
                    elements.RemoveRange(0, q);
                    ICollection<ElementId> elementIds = elements;



                elementIds.Add(item.ActiveElement.Id);
                List<ElementId> subSubElIds = new List<ElementId>();
                foreach (var id in elementIds)
                {
                    FamilyInstance element = (FamilyInstance) ActiveDoc.GetElement(id);
                    if (element.Name.Contains("Каркас") || element.Name.Contains("Сетка") || element.Name.Contains("Пенополистирол_Массив"))
                    {
                        subSubElIds.AddRange(element.GetSubComponentIds());
                    }

                }

                if (item is VS_Panel)
                {
                    ICollection<ElementId> essentials = new List<ElementId>();
                    foreach (var id in elementIds)
                    {
                        Element element = ActiveDoc.GetElement(id);
                        if (!element.Name.Contains("Торцевая"))
                        {
                            essentials.Add(id);
                        }
                    }
                    elementIds = essentials;
                    
                }
                if (item is NS_Panel Panel)
                {
                    
                    //if (_Panel.GetPVLList() != null)
                    //{
                    //    foreach (Element frontPVL in _Panel.GetPVLList())
                    //    {
                    //        elementIds.Add(frontPVL.Id);
                    //    }
                    //}

                }

                using (Transaction transaction = new Transaction(ActiveDoc, $"Создание сборки: {item.ShortMark}"))
                {
                    Category category = item.ActiveElement.Category;
                    transaction.Start();
                    AssemblyInstance assembly = AssemblyInstance.Create(ActiveDoc, elementIds, category.Id);
                    transaction.Commit();
                    transaction.Start();
                    assembly.AssemblyTypeName = item.ShortMark;
                    transaction.Commit();
                    transaction.Start();
                    assembly.AddMemberIds(subSubElIds);
                    //создание видов
                    //View3D view = AssemblyViewUtils.Create3DOrthographic(ActiveDoc, assembly.Id);
                    //ViewSheet sheet1 = AssemblyViewUtils.CreateSheet(ActiveDoc, assembly.Id, ElementId.InvalidElementId);
                    //XYZ origin = new XYZ();
                    ////размещение видов
                    //Viewport.Create(ActiveDoc, sheet1.Id, view.Id, origin);
                    transaction.Commit();
                }
            }
            transactionGroup.Assimilate();
        }

        #endregion

        

        #region Разборка сборок

        public void LeaveUniquePanels()
        {
            List<AssemblyInstance> assemblies = new FilteredElementCollector(ActiveDoc).OfCategory(BuiltInCategory.OST_Assemblies).WhereElementIsNotElementType().Cast<AssemblyInstance>().ToList();
            assemblies.Sort(CompareAssembliesByName);

            List<AssemblyType> assemblyTypes = new FilteredElementCollector(ActiveDoc).OfClass(typeof(AssemblyType)).WhereElementIsElementType().Cast<AssemblyType>().ToList();

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
                        if (cnt>1)
                        {
                            disposables.Add(assembly);
                        }
                        cnt--;
                    }
                        Debug.WriteLine($"Число сборок: {disposables.Count} / {amount - assemblyTypes.Count}");
                    }
    
        
            Debug.WriteLine($"Уникальные сборки определены");


            using (Transaction transaction = new Transaction(ActiveDoc, "Разбираем сборки"))
            {
                Debug.WriteLine("Начинаем разборку");
                int counter = 1;
                foreach (AssemblyInstance assembly in disposables)
                {
                    Debug.WriteLine($"Прогресс {counter} / {disposables.Count}");
                    transaction.Start();
                    FailureHandlingOptions opts = transaction.GetFailureHandlingOptions();
                    IFailuresPreprocessor preprocessor = new WarningSwallower();
                    opts.SetFailuresPreprocessor(preprocessor);
                    transaction.SetFailureHandlingOptions(opts);
                    //assembly.Disassemble();
                    ActiveDoc.Delete(assembly.Id);
                    transaction.Commit();
                    counter++;
                }
               
            }

        }

        private static IEnumerable<AssemblyInstance> GetFamilyInstancesByType(AssemblyType familyName, AssemblyInstance typeName)
        {

            //return new FilteredElementCollector(ActiveDoc)
            //  .OfClass(typeof(AssemblyInstance))
            //  .Cast<AssemblyInstance>()
            //  .Where(x => x.AssemblyTypeName.Equals(familyName.Name)
            //  .Where(x => x.Symbol.Family.Name.Equals(familyName)) // family
            //  .Where(x => x.Name.Equals(typeName)); // family type
            return null;
        }

        public void DisassembleAll(Document document)
        {
            List<AssemblyInstance> assemblies = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_Assemblies).WhereElementIsNotElementType().Cast<AssemblyInstance>().ToList();

            using (Transaction transaction = new Transaction(document, "Разбираем сборки"))
            {
                

                foreach (AssemblyInstance assembly in assemblies)
                {
                    if (Eligible(assembly) )
                    {
                        transaction.Start();

                        FailureHandlingOptions failOpt
          = transaction.GetFailureHandlingOptions();

                        failOpt.SetFailuresPreprocessor(
                          new WarningSwallower());

                        transaction.SetFailureHandlingOptions(failOpt);

                        assembly.Disassemble();
                        transaction.Commit();
                    }                  
                }
            }
        }

        private bool Eligible(AssemblyInstance assembly)
        {
            if (assembly.AssemblyTypeName.Contains("НС") ||
                assembly.AssemblyTypeName.Contains("ВС") ||
                assembly.AssemblyTypeName.Contains("ПС") ||
                assembly.AssemblyTypeName.Contains("ПП") ||
                assembly.AssemblyTypeName.Contains("БП")) return true;
            else return false;
        }
        #endregion


        #region Сравнение панелей и сборок
        private int CompareAssembliesByName(AssemblyInstance x, AssemblyInstance y)
        {
            string _postfixX = x.AssemblyTypeName;

            string _postfixY = y.AssemblyTypeName;

            return String.Compare(_postfixX, _postfixY);

        }

        private int CompareMarksByName(Panel.Panel x, Panel.Panel y)
        {
            string _panelNameX = x.LongMark;

            string _panelNameY = y.LongMark;

            return String.Compare(_panelNameX, _panelNameY);

        }

        private int CompareElementIdsByZCoord(ElementId x, ElementId y)
        {
            Element elX = ActiveDoc.GetElement(x);
            Element elY = ActiveDoc.GetElement(y);

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

        private int CompareAsembliesbyLvl(AssemblyInstance x, AssemblyInstance y)
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

        #endregion

        #region убираем ненужные элементы



        #endregion
    }


    public class WarningSwallower : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(
      FailuresAccessor a)
        {
            IList<FailureMessageAccessor> failures
            = a.GetFailureMessages();

            foreach (FailureMessageAccessor f in failures)
            {
                FailureSeverity fseverity = a.GetSeverity();

                if (fseverity == FailureSeverity.Warning)
                {
                    a.DeleteWarning(f);
                }
                else
                {
                    a.ResolveFailure(f);
                    return FailureProcessingResult.ProceedWithCommit;
                }
            }
            return FailureProcessingResult.Continue;
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
