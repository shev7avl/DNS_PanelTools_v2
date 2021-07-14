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
using DNS_PanelTools_v2.Utility;

namespace DNS_PanelTools_v2.StructuralApps.Panel
{
    public class WallParts : Base_Panel, IPerforable
    {
        public override Document ActiveDocument { get; set; }

        public override Element ActiveElement { get; set; }

        public override string ShortMark { get; set; }

        public override string LongMark { get; set; }

        public override List<XYZ> IntersectedWindows { get; set; }

        private Document LinkedDocSTR;

        private Document LinkedDocARCH;

        private Base_Panel IntersectedPanel;

        private List<Material> FacadeMaterials { get; set; }

        private List<ElementId> FacadeParts { get; set; } 

        public WallParts(Document document, Document linkedDocSTR, Document linkedDocARCH, Element element)
        {
            ActiveDocument = document;
            LinkedDocSTR = linkedDocSTR;
            LinkedDocARCH = linkedDocARCH;
            ActiveElement = element;
        }

        public WallParts(Document document, Element element)
        {
            ActiveDocument = document;
            ActiveElement = element;
        }

        void IPerforable.GetOpenings(Document linkedArch, out List<Element> IntersectedWindows)
        {
            SingleArchDoc archDoc = SingleArchDoc.getInstance(linkedArch);
            List<Element> windows = archDoc.getWindows();

            IntersectedWindows = new List<Element>();

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
                if (Geometry.InBox(boundingBoxHostWall, Start) || Geometry.InBox(boundingBoxHostWall, End))
                {
                    IntersectedWindows.Add(window);
                }
            }
        }

        void IPerforable.Perforate(List<Element> IntersectedWindows)
        {
            IEnumerable<Element> familySymbols = new FilteredElementCollector(ActiveDocument).OfCategory(BuiltInCategory.OST_Windows).WhereElementIsElementType().Where(o => o.Name.Contains("DNS_ПроемДляПлитки"));
            FamilySymbol familySymbol = (FamilySymbol)familySymbols.First();

            foreach (Element window in IntersectedWindows)
            {
                FamilyInstance windowFamInst = window as FamilyInstance;
                Element hostWall = windowFamInst.Host;
                Level level = ActiveDocument.GetElement(hostWall.LevelId) as Level;

                LocationPoint locationPoint = (LocationPoint)window.Location;

                using (Transaction transaction = new Transaction(ActiveDocument, "Create window"))
                {
                    transaction.Start();
                    ActiveDocument.Create.NewFamilyInstance(locationPoint.Point, familySymbol, ActiveElement, level, StructuralType.NonStructural);
                    transaction.Commit();
                }
            }


        }


        public override void CreateMarks()
        {
            Options options = new Options();
            LocationCurve curve = (LocationCurve)ActiveElement.Location;
            XYZ Start = curve.Curve.GetEndPoint(0);
            XYZ End = curve.Curve.GetEndPoint(0);

            SingleStructDoc marksList = SingleStructDoc.getInstance(LinkedDocSTR);
            List<Base_Panel> panels = marksList.GetPanelMarks();

            foreach (Base_Panel item in panels)
            {
                BoundingBoxXYZ boundingBox = item.ActiveElement.get_Geometry(options).GetBoundingBox();

                if (Geometry.InBox(boundingBox, Start) && Geometry.InBox(boundingBox, End))
                {
                    Debug.WriteLine($"{ActiveElement.Name} пересекается с: {item.LongMark}");
                    Transaction transaction = new Transaction(ActiveDocument, $"Назначение марки: {item.LongMark}");
                    transaction.Start();

                    ActiveElement.LookupParameter("DNS_Код изделия полный").Set(item.LongMark);
                    ActiveElement.LookupParameter("DNS_Марка элемента").Set(item.LongMark);
                    ActiveElement.LookupParameter("ADSK_Марка конструкции").Set(item.LongMark);
                    transaction.Commit();
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
            if (PartUtils.AreElementsValidForCreateParts(ActiveDocument, elementIds))
            {
                using (Transaction transaction = new Transaction(ActiveDocument, "Parts creation"))
                {
                    transaction.Start();
                    PartUtils.CreateParts(ActiveDocument, elementIds);
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
                //SplitGeometry.CreateSketchPlane(ActiveDocument, item);
            }
            FacadeParts = new List<ElementId>();
            FacadeParts = (List <ElementId>)ActiveElement.GetDependentElements(filter);
        }

        
    }
}
