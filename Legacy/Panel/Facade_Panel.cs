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

        public Facade_Panel(Element element)
        {
            ActiveDocument = element.Document;
            ActiveElement = element;
        }

        public override void CreateMarks()
        {
            string lngMark = "DNS_Код изделия полный";
            string shortMark = "ADSK_Марка конструкции";
            string posMart = "DNS_Марка элемента";

            Element linkedPanel = FindLinkedPanel(ActiveDocument);
            ParameterMap panelMap = linkedPanel.ParametersMap;

            List<Part> parts = FindParts();
            List<ParameterMap> partsMaps = parts.Select(o => o.ParametersMap).ToList();

            


            string[] values = new string[3]
                {
                panelMap.get_Item(lngMark).AsString(),
                panelMap.get_Item(shortMark).AsString(),
                panelMap.get_Item(posMart).AsString()
                };

            Transaction transaction = new Transaction(ActiveDocument, "setting facade params");
            TransactionSettings.SetFailuresPreprocessor(transaction);


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

            ElementCategoryFilter categoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Parts);

            List<Part> parts = new FilteredElementCollector(ActiveDocument).
                OfCategory(BuiltInCategory.OST_Parts).
                WhereElementIsNotElementType().
                WherePasses(boundingBoxIntersectsFilter).
                Cast<Part>().
                Where(o => ActiveElement.GetDependentElements(categoryFilter).Contains(o.Id)).
                ToList();

            return parts;
        }

        private Element FindLinkedPanel(Document activeDocument)
        {
            CommonProjectEnvironment projectEnvironment = CommonProjectEnvironment.GetInstance(ActiveDocument);

            List<RevitLinkInstance> linkedDocs = CommonProjectEnvironment.FindLinkedDocuments(ActiveDocument);
            RevitLinkInstance linkedDocSTR = CommonProjectEnvironment.FindLinkedDocuments(ActiveDocument).
                Where<RevitLinkInstance>(o => o.Name.Contains("_КР") || o.Name.Contains("_КЖ")).
                FirstOrDefault();

            if (linkedDocSTR is null)
            {
                throw new ArgumentNullException("Не найдено файла подложки." +
                    " Проверьте наличие \"_КЖ\" или \"_КР\" в названии связанного файла ");
            }

            var linkTransform = linkedDocSTR.GetTotalTransform().Inverse;
            var box = ActiveElement.get_Geometry(new Options()).GetBoundingBox();
            var transformedBox = new BoundingBoxXYZ
            {
                Max = new XYZ
            (x: box.Max.X + linkTransform.Origin.X,
                y: box.Max.Y + linkTransform.Origin.Y,
                z: box.Max.Z + linkTransform.Origin.Z),

                Min = new XYZ
            (x: box.Min.X + linkTransform.Origin.X,
                y: box.Min.Y + linkTransform.Origin.Y,
                z: box.Min.Z + linkTransform.Origin.Z),

                Transform = linkTransform
            };

            LocationCurve curve = ActiveElement.Location as LocationCurve;
            Line line = curve.Curve as Line;
            var transformedCentroid = new XYZ(
                x: curve.Curve.GetEndPoint(0).X + 0.5 * curve.Curve.Length * line.Direction.X + linkTransform.Origin.X,
                y: curve.Curve.GetEndPoint(0).Y + 0.5 * curve.Curve.Length * line.Direction.Y + linkTransform.Origin.Y,
                z: curve.Curve.GetEndPoint(0).Z + 0.5 * (transformedBox.Max.Z - transformedBox.Min.Z));

            BoundingBoxIntersectsFilter boundingBoxIntersectsFilter = new BoundingBoxIntersectsFilter
                (new Outline(transformedBox.Min,
                transformedBox.Max));

            var wallLevel = ActiveDocument.GetElement(ActiveElement.LevelId).Name;


            Predicate<Element> passes = (Element element) => {
                bool levelPasses = element.Document.GetElement(element.LevelId).Name == wallLevel;
                bool intersects = Geometry.InBox(element.get_Geometry(new Options()).GetBoundingBox(), transformedCentroid);

                return levelPasses;

            };

            Element panel = new FilteredElementCollector(linkedDocSTR.GetLinkDocument()).
                OfCategory(BuiltInCategory.OST_StructuralFraming).
                WhereElementIsNotElementType().
                WherePasses(boundingBoxIntersectsFilter).
                ToElements().
                Where(o => passes(o)).
                FirstOrDefault();

            if (panel is null)
            {
                throw new InvalidOperationException("Фасад не пересекается с панелью");
            }
            return panel;

        }
    }
}
