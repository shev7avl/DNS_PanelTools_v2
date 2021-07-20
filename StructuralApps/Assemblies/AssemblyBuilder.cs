using DSKPrim.PanelTools_v2.StructuralApps.Panel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using System.Diagnostics;

namespace DSKPrim.PanelTools_v2.StructuralApps.Assemblies
{
    public class AssemblyBuilder
    {

        public AssemblyInstance result;

        private Dictionary<int, Panel.Panel> IndexMarkPairs;

        private List<Panel.Panel> MarksList;

        private Document ActiveDoc;

        public AssemblyBuilder(Document document)
        {
            ActiveDoc = document;
            SingleStructDoc marksList = SingleStructDoc.getInstance(ActiveDoc);
            MarksList = marksList.GetPanelMarks();
        }
        #region Создание коротких индексов
        public void AddIndex(string panelSubString)
        {
            IndexMarkPairs = new Dictionary<int, Panel.Panel>();
            Debug.WriteLine("Словарь Марка - индекс");
            Debug.WriteLine("------Начало словаря------");

            SetIndexes(panelSubString);

            Debug.WriteLine("------Конец словаря------");
        }

        private void SetIndexes(string panelSubString)
        {
            int counter = 1;
            foreach (var mark in MarksList)
            {
                if (mark.LongMark.Contains(panelSubString))
                {
                    if (PanelExists(mark))
                    {
                        int index = 0;
                        foreach (var key in IndexMarkPairs.Keys)
                        {
                            if (IndexMarkPairs[key].Equal(mark))
                            {
                                index = key;
                            }
                        }
                        mark.SetIndex(index);
                    }
                    if (!PanelExists(mark))
                    {
                        IndexMarkPairs.Add(counter, mark);
                        mark.SetIndex(counter, overwrite: true);
                        Debug.WriteLine($"{mark.ShortMark}");
                        counter++;
                    }
                }
            }
        }

        private bool PanelExists(Panel.Panel item)
        {
            bool exists = false;
            foreach (int key in IndexMarkPairs.Keys)
            {
                if (item.Equal(IndexMarkPairs[key]))
                {
                    exists = true;
                    break;
                }
                else
                {
                    exists = false;
                }
            }
            return exists;
        }
        #endregion

        #region Создание сборок

        public void CreateAssemblies()
        {
            TransactionGroup transactionGroup = new TransactionGroup(ActiveDoc, "Создание сборок");
            transactionGroup.Start();
            MarksList.Sort(CompareMarksByName);
            foreach (var item in MarksList)
            {
                FamilyInstance familyInstance = (FamilyInstance)item.ActiveElement;
                ICollection<ElementId> elementIds = familyInstance.GetSubComponentIds();
                
                elementIds.Add(item.ActiveElement.Id);
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
                if (item is NS_Panel _Panel)
                {
                    _Panel = (NS_Panel)item;
                    if (_Panel.GetPVLList() != null)
                    {
                        foreach (Element frontPVL in _Panel.GetPVLList())
                        {
                            elementIds.Add(frontPVL.Id);
                        }
                    }
                    
                }

                using (Transaction transaction = new Transaction(ActiveDoc, $"Создание сборки: {item.ShortMark}"))
                {
                    Category category = item.ActiveElement.Category;
                    transaction.Start();
                    AssemblyInstance assembly = AssemblyInstance.Create(ActiveDoc, elementIds, category.Id);
                    transaction.Commit();
                    transaction.Start();
                    assembly.AssemblyTypeName = item.ShortMark;
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
            List<AssemblyInstance> disposables = new List<AssemblyInstance>();

            assemblies.Sort(CompareAssembliesByName);

            AssemblyInstance as1 = assemblies[0];
            for (int i = 1; i < assemblies.Count; i++)
            {
                if (assemblies[i].AssemblyTypeName == as1.AssemblyTypeName)
                {
                    disposables.Add(assemblies[i]);
                }
                as1 = assemblies[i];
            }

            using (Transaction transaction = new Transaction(ActiveDoc, "Разбираем сборки"))
            {
                
                foreach (AssemblyInstance assembly in disposables)
                {
                    transaction.Start();
                    assembly.Disassemble();
                    transaction.Commit();
                }
               
            }

        }

        public void DisassembleAll()
        {
            List<AssemblyInstance> assemblies = new FilteredElementCollector(ActiveDoc).OfCategory(BuiltInCategory.OST_Assemblies).WhereElementIsNotElementType().Cast<AssemblyInstance>().ToList();

            using (Transaction transaction = new Transaction(ActiveDoc, "Разбираем сборки"))
            {

                foreach (AssemblyInstance assembly in assemblies)
                {
                    transaction.Start();
                    assembly.Disassemble();
                    transaction.Commit();
                }

            }
        }
        #endregion

        #region Сравнение панелей и сборок
        private int CompareAssembliesByName(AssemblyInstance x, AssemblyInstance y)
        {
            string _postfixX = x.AssemblyTypeName;

            string _postfixY = y.AssemblyTypeName;

            int res = String.Compare(_postfixX, _postfixY);

            if (res>0)
            {
                return 1;
            }
            else if (res == 0)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        private int CompareMarksByName(Panel.Panel x, Panel.Panel y)
        {
            string _panelNameX = x.LongMark;

            string _panelNameY = y.LongMark;

            int res = String.Compare(_panelNameX, _panelNameY);

            if (res > 0)
            {
                return 1;
            }
            else if (res == 0)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        #endregion

    }
}
