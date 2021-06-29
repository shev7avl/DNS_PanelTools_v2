using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using DNS_PanelTools_v2.StructuralApps;
using DNS_PanelTools_v2.StructuralApps.Panel;

namespace DNS_PanelTools_v2.Architecture
{
    public class WallParts
    {
        public Document ActiveDoc;

        public Document LinkedDocSTR;

        public Document LinkedDocARCH;

        public Element ActiveElement;

        public IPanel IntersectedPanel;

        public List<XYZ> IntersectedWindows { get; set; }

        public List<Material> FacadeMaterials { get; set; }

        public List<ElementId> FacadeParts { get; set; } 

        public WallParts(Document document, Document linkedDocSTR, Document linkedDocARCH, Element element)
        {
            ActiveDoc = document;
            LinkedDocSTR = linkedDocSTR;
            LinkedDocARCH = linkedDocARCH;
            ActiveElement = element;
        }

        public void GetIntersectedPanel()
        {
            Options options = new Options();
            LocationCurve curve = (LocationCurve)ActiveElement.Location;
            XYZ Start = curve.Curve.GetEndPoint(0);
            XYZ End = curve.Curve.GetEndPoint(0);

            SingleStructDoc marksList = SingleStructDoc.getInstance(LinkedDocSTR);
            List<IPanel> panels = marksList.GetPanelMarks();

            foreach (IPanel item in panels)
            {
                BoundingBoxXYZ boundingBox = item.ActiveElement.get_Geometry(options).GetBoundingBox();

                if (RvtGeomStat.IsPointInsideBbox(boundingBox, Start) && RvtGeomStat.IsPointInsideBbox(boundingBox, End))
                {
                    Debug.WriteLine($"{ActiveElement.Name} пересекается с: {item.LongMark}");
                    Transaction transaction = new Transaction(ActiveDoc, $"Назначение марки: {item.LongMark}");
                    transaction.Start();

                    ActiveElement.LookupParameter("DNS_Код изделия полный").Set(item.LongMark);
                    ActiveElement.LookupParameter("DNS_Марка элемента").Set(item.LongMark);
                    ActiveElement.LookupParameter("ADSK_Марка конструкции").Set(item.LongMark);
                    transaction.Commit();
                }
            }

        }

        public void GetIntersectedWindows()
        {
            SingleArchDoc archDoc = SingleArchDoc.getInstance(LinkedDocARCH);
            List<Element> windows = archDoc.getWindows();

            IEnumerable<Element> familySymbols = new FilteredElementCollector(ActiveDoc).OfCategory(BuiltInCategory.OST_Windows).WhereElementIsElementType().Where(o => o.Name.Contains("DNS_ПроемДляПлитки"));
            FamilySymbol familySymbol = (FamilySymbol)familySymbols.First();

            IntersectedWindows = new List<XYZ>();

            LocationCurve location = (LocationCurve)ActiveElement.Location;
            Curve curve = location.Curve;

            XYZ Start = curve.GetEndPoint(0);
            XYZ End = curve.GetEndPoint(1);

            Options options = new Options();
            
            foreach (Element window in windows)
            {
                FamilyInstance windowFamInst = window as FamilyInstance;
                Element hostWall = windowFamInst.Host;
                BoundingBoxXYZ boundingBoxHostWall = hostWall.get_Geometry(options).GetBoundingBox();
                Debug.WriteLine($"------{window.Name}------");
                Debug.WriteLine($"BoundingBox MAX: {boundingBoxHostWall.Max}");
                Debug.WriteLine($"BoundingBox MIN: {boundingBoxHostWall.Min}");

                Debug.WriteLine($"Начало отрезка: {Start}");
                Debug.WriteLine($"Конец отрезка: {End}");
                
                Debug.WriteLine("------------------");
                if (RvtGeomStat.IsPointInsideBbox(boundingBoxHostWall, Start) || RvtGeomStat.IsPointInsideBbox(boundingBoxHostWall, End))
                {
                    LocationPoint locationPoint = (LocationPoint)window.Location;
                    Debug.WriteLine($"Нашли пересечение {ActiveElement.Name} с окном {window.Name}");

                    Level level = ActiveDoc.GetElement(hostWall.LevelId) as Level;

                    using (Transaction transaction = new Transaction(ActiveDoc, "Create window"))
                    {
                        transaction.Start();
                        ActiveDoc.Create.NewFamilyInstance(locationPoint.Point, familySymbol, ActiveElement, level, StructuralType.NonStructural);
                        transaction.Commit();
                    }

                    IntersectedWindows.Add(locationPoint.Point);
                }
            }


        }

        public void SplitToParts()
        {
            ICollection<ElementId> elementIdsToDivide = new List<ElementId>();

            ICollection<ElementId> elementIds = new List<ElementId>()
            {
                ActiveElement.Id
            };
            Debug.WriteLine($"------------");
            Debug.WriteLine($"@Начало транзакции");
            Debug.WriteLine($"@");
            if (PartUtils.AreElementsValidForCreateParts(ActiveDoc, elementIds))
            {
                using (Transaction transaction = new Transaction(ActiveDoc, "Parts creation"))
                {
                    transaction.Start();
                    PartUtils.CreateParts(ActiveDoc, elementIds);
                    transaction.Commit();
                }                
            }
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Parts);
            elementIdsToDivide = ActiveElement.GetDependentElements(filter);

            Debug.WriteLine($"@Части для {ActiveElement.Name} Созданы успешно");
            Debug.WriteLine($"@");
            Debug.WriteLine($"@Конец транзакции");
            Debug.WriteLine($"------------");

            foreach (var item in elementIdsToDivide)
            {
                SplitGeometry.CreateSketchPlane(ActiveDoc, item);
            }
            FacadeParts = new List<ElementId>();
            FacadeParts = (List <ElementId>)ActiveElement.GetDependentElements(filter);
        }

    }
}
