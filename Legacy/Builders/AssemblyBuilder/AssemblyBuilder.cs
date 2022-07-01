using Autodesk.Revit.DB;
using DSKPrim.PanelTools.Panel;
using DSKPrim.PanelTools.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.Legacy.Builders.AssemblyBuilder
{
    public class AssemblyBuilder
    {
        private readonly PrecastPanel _panel;
        private static readonly string _embeddedFamPath = 
            "\\\\ir-dsk\\DNSDevelopment\\1_ДСК Приморье\\13. Проектный институт\\ДНС ПРОЕКТ РАБОЧАЯ\\BIM\00_DNSPanel\\02_DNS_Семейства\\2. КР\\02_Закладные Детали";
        private static readonly string _rebarFamPath =
            "\\\\ir-dsk\\DNSDevelopment\\1_ДСК Приморье\\13. Проектный институт\\ДНС ПРОЕКТ РАБОЧАЯ\\BIM\\00_DNSPanel\\02_DNS_Семейства\\2. КР\\04_Вложенные";

        private readonly List<string> _embeddedFamNames;
        private readonly List<string> _rebarFamNames;

        private readonly List<ElementId> _assemblyElements;

        public AssemblyBuilder(PrecastPanel panel)
        {
            _panel = panel;
            _embeddedFamNames = ParseFamilyNames(_embeddedFamPath);
            _rebarFamNames = ParseFamilyNames(_rebarFamPath);
            _assemblyElements = new List<ElementId>();
        }

        public void CollectAssemblyElements()
        {

            FamilyInstance family = (FamilyInstance)_panel.ActiveElement;

            foreach (var item in family.GetSubComponentIds())
            {
                FamilyInstance instance = (FamilyInstance)_panel.ActiveElement.Document.GetElement(item);
                if (_rebarFamNames.Contains(instance.Symbol.FamilyName))
                {
                    _assemblyElements.Add(item);
                }

                if (instance.Name.Contains("Каркас") || instance.Name.Contains("Сетка") || instance.Name.Contains("Пенополистирол_Массив"))
                {
                    _assemblyElements.AddRange(instance.GetSubComponentIds());
                }
            }

            ElementFilter lvlFilter = new ElementLevelFilter(_panel.ActiveElement.LevelId);
            ElementFilter bbIntersection = new ElementIntersectsElementFilter(_panel.ActiveElement);

            List<ElementId> els = new FilteredElementCollector(_panel.ActiveElement.Document)
                .OfClass(typeof(FamilyInstance))
                .OfCategory(BuiltInCategory.OST_GenericModel)
                .WherePasses(lvlFilter)
                .WherePasses(bbIntersection)
                .ToElements()
                .Where(o => _embeddedFamNames.Contains(GetFamilyName(o)))
                .Select(o => o.Id)
                .ToList();

            _assemblyElements.AddRange(els);
        }

        public void CreateAssembly()
        {
            Transaction transaction = new Transaction(_panel.ActiveElement.Document);
            TransactionSettings.SetFailuresPreprocessor(transaction);

            transaction.Start("--> Creating initial assembly");
            var ass = AssemblyInstance.Create(_panel.ActiveElement.Document, 
                new List<ElementId>(){ _panel.ActiveElement.Id }, 
                _panel.ActiveElement.Category.Id);
            transaction.Commit();

            transaction.Start("--> Adding items to assembly");
            var filteredAssemblyElements = _assemblyElements.
                Where(o => AssemblyInstance.AreElementsValidForAssembly(_panel.ActiveElement.Document, new List<ElementId>() {o}, ass.Id)).ToList();

            ass.AddMemberIds(filteredAssemblyElements);
            transaction.Commit();

            transaction.Start("--> Setting assembly name");
            try
            {
                ass.AssemblyTypeName = _panel.Mark.ShortMark;
            }
            catch (Exception)
            {
                ass.AssemblyTypeName = $"{_panel.Mark.ShortMark}-ID{_panel.ActiveElement.Id}";
            }
            transaction.Commit();
        }

        private static List<string> ParseFamilyNames(string path)
        {

            List<string> windowFamilyNames = new List<string>();

            string[] filePaths = Directory.GetFiles(path);
            foreach (var item in filePaths)
            {
                string name = item.Split('\\').Last().Replace(".rfa", "");
                windowFamilyNames.Add(name);
            }

            return windowFamilyNames;
        }

       

        private string GetFamilyName(Element element)
        {
            FamilyInstance instance = element as FamilyInstance;
            return instance.Symbol.FamilyName;
        }
    }
}
