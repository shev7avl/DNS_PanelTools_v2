using Autodesk.Revit.DB;
using DSKPrim.PanelTools.ProjectEnvironment;
using DSKPrim.PanelTools.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.Panel
{
    class Facade_Panel : BasePanel
    {
        public override Document ActiveDocument { get ; set ; }
        public override Element ActiveElement { get ; set ; }
        public override AssemblyInstance AssemblyInstance { get ; set ; }
        public override string LongMark { get; set; }
        public override string ShortMark { get; set; }
        public override string Index { get; set; }

        public Facade_Panel(Document document, Element element)
        {
            ActiveDocument = document;
            ActiveElement = element;
        }

        public override void CreateMarks()
        {
            string lngMark = "DNS_Код изделия полный";
            string shortMark = "ADSK_Марка конструкции";
            string posMart = "DNS_Марка элемента";

            Element linkedPanel = FindLinkedPanel(ActiveDocument);
            ParameterMap panelMap = linkedPanel.ParametersMap;

            string[] values = new string[3]
                {
                panelMap.get_Item(lngMark).AsString(),
                panelMap.get_Item(shortMark).AsString(),
                panelMap.get_Item(posMart).AsString()
                };

            Transaction transaction = new Transaction(ActiveDocument, "setting facade params");
            TransactionSettings.SetFailuresPreprocessor(transaction);

            List<Part> parts = FindParts();
            List<ParameterMap> partsMaps = parts.Select(o => o.ParametersMap).ToList();

            using (transaction)
            {
                transaction.Start();

                panelMap = ActiveElement.ParametersMap;
                panelMap.get_Item(lngMark).Set(values[0]);
                panelMap.get_Item(shortMark).Set(values[1]);
                panelMap.get_Item(posMart).Set(values[2]);

                transaction.Commit();

                transaction.Start();

                foreach (var map in partsMaps)
                {
                    map.get_Item(lngMark).Set(values[0]);
                    map.get_Item(shortMark).Set(values[1]);
                    map.get_Item(posMart).Set(values[2]);
                }

                transaction.Commit();
            }
            
        }

        private List<Part> FindParts()
        {

            BoundingBoxIntersectsFilter boundingBoxIntersectsFilter = new BoundingBoxIntersectsFilter
                (new Outline(ActiveElement.get_Geometry(new Options())
                .GetBoundingBox().Min,
                ActiveElement.get_Geometry(new Options())
                .GetBoundingBox().Max));

            List<Element> parts = new FilteredElementCollector(ActiveDocument).
                OfCategory(BuiltInCategory.OST_Parts).
                WhereElementIsNotElementType().
                WherePasses(boundingBoxIntersectsFilter).
                ToElements().
                ToList();

            return parts.Cast<Part>().ToList();
        }

        private Element FindLinkedPanel(Document activeDocument)
        {
            CommonProjectEnvironment projectEnvironment = CommonProjectEnvironment.GetInstance(ActiveDocument);

            List<RevitLinkInstance> linkedDocs = CommonProjectEnvironment.FindLinkedDocuments(ActiveDocument);
            Document linkedDocSTR = CommonProjectEnvironment.FindLinkedDocuments(ActiveDocument).
                Where<RevitLinkInstance>(o => o.Name.Contains("_КР") || o.Name.Contains("_КЖ")).
                Select(o => o.GetLinkDocument()).
                FirstOrDefault();


            ElementIntersectsElementFilter facadeIntersectionFilter = new ElementIntersectsElementFilter(ActiveElement);
            BoundingBoxIntersectsFilter boundingBoxIntersectsFilter = new BoundingBoxIntersectsFilter
                (new Outline(ActiveElement.get_Geometry(new Options())
                .GetBoundingBox().Min,
                ActiveElement.get_Geometry(new Options())
                .GetBoundingBox().Max));

            if (linkedDocSTR is null)
            {
                throw new ArgumentNullException("Не найдено файла подложки." +
                    " Проверьте наличие \"_КЖ\" или \"_КР\" в названии связанного файла ");
            }

            Element panel = new FilteredElementCollector(linkedDocSTR).
                OfCategory(BuiltInCategory.OST_StructuralFraming).
                WhereElementIsNotElementType().
                WherePasses(boundingBoxIntersectsFilter).
                WherePasses(facadeIntersectionFilter).
                ToElements().
                FirstOrDefault();

            if (panel is null)
            {
                throw new InvalidOperationException("Фасад не пересекается с панелью");
            }
            return panel;

        }
    }
}
